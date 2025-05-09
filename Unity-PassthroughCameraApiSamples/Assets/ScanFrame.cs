using UnityEngine;
using ZXing;
using System.Collections.Generic;
using System.Collections;
using Unity.XR.CoreUtils;
using System;
using UnityEditor;

public class ScanFrame : MonoBehaviour
{
    private BarcodeReader reader;
    private bool isScanning = false;
    private bool isCapturingPictures = true;
    public PassthroughCamera camera;
    private Texture2D currentPicture;
    public OpenFoodFactsAPI openFoodFactsApi;

    private IEnumerator Start()
    {
        reader = new BarcodeReader
        {
            AutoRotate = true,
            Options = new ZXing.Common.DecodingOptions
            {
                TryHarder = true,
                TryInverted = true,
                PossibleFormats = new List<BarcodeFormat> {
                    BarcodeFormat.EAN_8,
                    BarcodeFormat.EAN_13
                }
            }
        };

        while (reader == null)
        {
            Debug.LogError("❌ reader is null");
            yield return null;
        }

        float time = 1.0f; // Start 1 second from the beginning
        float repeatRate = 1.0f; // Repeat every second
        InvokeRepeating("Scan", time, repeatRate);
    }

    void Update()
    {
        isScanning = OVRInput.Get(OVRInput.Button.One);
    }

    public void Scan()
    {
        if (!isScanning)
            return;

        var (pixels, width, height) = camera.GetFrameData();

        if (pixels == null || pixels.Length == 0)
        {
            Debug.LogWarning("🚫 Not a valid pixel array");
            return;
        }

        var scanRect = GetScanRectFromQuad(transform, Camera.main, width, height);

        // 🔧 Get actual pixels and dimensions
        var (scanRegion, scanWidth, scanHeight) = ExtractScanRegion(pixels, width, height, scanRect);
        //scanRegion = ToBlackAndWhite(scanRegion, 100);

        if (isCapturingPictures && isScanning)
            CapturePicture(scanRegion, scanWidth, scanHeight);

        // ✅ Now we can trust scanWidth and scanHeight
        if (scanRegion.Length != scanWidth * scanHeight)
        {
            Debug.LogError($"❌ scanRegion size mismatch: expected {scanWidth * scanHeight}, got {scanRegion.Length}");
        }

        try
        {
            var result = reader.Decode(scanRegion, scanWidth, scanHeight);

            if (result != null)
            {
                Debug.LogWarning("✅ Barcode read successfully: " + result.Text);
                openFoodFactsApi.MakeAPICall(result.Text);
            }
            else
            {
                Debug.LogWarning("🚫 Barcode could not be read");
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError("❌ Error while scanning: " + ex);
        }
    }

    private Rect GetScanRectFromQuad(Transform quadTransform, Camera cam, int frameWidth, int frameHeight)
    {
        Vector3 quadCenter = quadTransform.position;
        Vector3 quadRight = quadTransform.right * 0.5f * quadTransform.localScale.x;
        Vector3 quadUp = quadTransform.up * 0.5f * quadTransform.localScale.y;

        Vector3[] worldCorners = new Vector3[4];
        worldCorners[0] = quadCenter - quadRight - quadUp; // Bottom Left
        worldCorners[1] = quadCenter + quadRight - quadUp; // Bottom Right
        worldCorners[2] = quadCenter + quadRight + quadUp; // Top Right
        worldCorners[3] = quadCenter - quadRight + quadUp; // Top Left

        Vector2 min = new Vector2(float.MaxValue, float.MaxValue);
        Vector2 max = new Vector2(float.MinValue, float.MinValue);

        foreach (var corner in worldCorners)
        {
            Vector3 viewportPos = cam.WorldToViewportPoint(corner); // ✅ Viewport statt Screen

            if (viewportPos.z < 0)
            {
                Debug.LogWarning("🚫 Corner is behind the camera");
                continue;
            }

            min = Vector2.Min(min, viewportPos);
            max = Vector2.Max(max, viewportPos);
        }

        // Clamp in viewport space (0..1), then scale to frame resolution
        // float x = Mathf.Clamp01(min.x) * frameWidth;
        // float y = Mathf.Clamp01(min.y) * frameHeight;
        // float w = Mathf.Clamp01(max.x - min.x) * frameWidth;
        // float h = Mathf.Clamp01(max.y - min.y) * frameHeight;

        float x = Mathf.Clamp(min.x / Screen.width * frameWidth, 0, frameWidth);

        // Invertiere die Y-Koordinate hier:
        float yFromTop = Screen.height - max.y;
        float y = Mathf.Clamp(yFromTop / Screen.height * frameHeight, 0, frameHeight);

        float w = Mathf.Clamp((max.x - min.x) / Screen.width * frameWidth, 0, frameWidth - x);
        float h = Mathf.Clamp((max.y - min.y) / Screen.height * frameHeight, 0, frameHeight - y);

        Debug.DrawLine(worldCorners[0], worldCorners[1], Color.red, 1.0f);
        Debug.DrawLine(worldCorners[1], worldCorners[2], Color.green, 1.0f);
        Debug.DrawLine(worldCorners[2], worldCorners[3], Color.blue, 1.0f);
        Debug.DrawLine(worldCorners[3], worldCorners[0], Color.yellow, 1.0f);

        return new Rect(x, y, w, h);
    }

    private (Color32[] pixels, int width, int height) ExtractScanRegion(
    Color32[] fullPixels, int fullWidth, int fullHeight, Rect scanRect)
    {
        int startX = Mathf.Clamp(Mathf.FloorToInt(scanRect.x), 0, fullWidth - 1);
        int startY = Mathf.Clamp(Mathf.FloorToInt(scanRect.y), 0, fullHeight - 1);
        int endX = Mathf.Clamp(Mathf.CeilToInt(scanRect.x + scanRect.width), 0, fullWidth);
        int endY = Mathf.Clamp(Mathf.CeilToInt(scanRect.y + scanRect.height), 0, fullHeight);

        int regionWidth = endX - startX;
        int regionHeight = endY - startY;

        Color32[] result = new Color32[regionWidth * regionHeight];

        for (int y = 0; y < regionHeight; y++)
        {
            for (int x = 0; x < regionWidth; x++)
            {
                int sourceIndex = (startY + y) * fullWidth + (startX + x);
                int targetIndex = y * regionWidth + x;
                result[targetIndex] = fullPixels[sourceIndex];
            }
        }

        return (result, regionWidth, regionHeight);
    }

    private Color32[] ToBlackAndWhite(Color32[] pixels, int threshold)
    {
        var bwPixels = new Color32[pixels.Length];

        for (var i = 0; i < pixels.Length; i++)
        {
            var p = pixels[i];
            var gray = (byte)(0.299f * p.r + 0.587f * p.g + 0.114f * p.b);
            var bw = (gray < threshold) ? (byte)0 : (byte)255;
            bwPixels[i] = new Color32(bw, bw, bw, 255);
        }

        return bwPixels;
    }

    private void CapturePicture(Color32[] pixels, int width, int height)
    {
        currentPicture = new Texture2D(width, height, TextureFormat.RGBA32, false);
        currentPicture.SetPixels32(pixels);
        currentPicture.Apply();

        var currentDate = DateTime.Now;
        var formattedDateTime = currentDate.ToString("yyyy-MM-dd_HH:mm:ss");
        System.IO.File.WriteAllBytes(Application.persistentDataPath + "/" + formattedDateTime + ".png", currentPicture.EncodeToPNG());
    }
}
