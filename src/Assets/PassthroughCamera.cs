using UnityEngine;
using PassthroughCameraSamples;
using System;
using System.Collections;

public class PassthroughCamera : MonoBehaviour
{
    public WebCamTextureManager webcamManager;
    public Texture2D currentPicture { get; private set; }
    private bool isCapturingPictures = false;

    private IEnumerator Start()
    {
        while (webcamManager == null)
        {
            Debug.LogError("❌ webcamManager is null");
            yield return null;
        }

        _ = StartCoroutine(PictureLoop());
    }

    private IEnumerator PictureLoop()
    {
        while (true)
        {
            UpdateCurrentPicture();
            yield return new WaitForSeconds(1f / 3f);
        }
    }

    void UpdateCurrentPicture()
    {
        var tex = webcamManager.WebCamTexture;

        if (tex == null || !tex.isPlaying)
        {
            Debug.LogWarning("🚫 WebCamTexture is not available");
            return;
        }

        if (tex.width == 0 || tex.height == 0)
        {
            Debug.LogError("❌ Invalid camera resolution: " + tex.width + "x" + tex.height);
            return;
        }

        var pixels = tex.GetPixels32();

        if (pixels == null || pixels.Length == 0)
        {
            Debug.LogWarning("🚫 Not a valid pixel array");
            return;
        }

        if (currentPicture == null || currentPicture.width != tex.width || currentPicture.height != tex.height)
        {
            currentPicture = new Texture2D(tex.width, tex.height, TextureFormat.RGBA32, false);
        }

        currentPicture.SetPixels32(pixels);
        currentPicture.Apply();
    }

    private void CapturePictures(Color32[] pixels, int width, int height)
    {
        currentPicture = new Texture2D(width, height, TextureFormat.RGBA32, false);
        currentPicture.SetPixels32(pixels);
        currentPicture.Apply();

        var currentDate = DateTime.Now;
        var formattedDateTime = currentDate.ToString("yyyy-MM-dd_HH:mm:ss");
        System.IO.File.WriteAllBytes(Application.persistentDataPath + "/" + formattedDateTime + ".png", currentPicture.EncodeToPNG());
    }

    public (Color32[] pixels, int width, int height) GetFrameData()
    {
        if (currentPicture == null) return (null, 0, 0);
        return (currentPicture.GetPixels32(), currentPicture.width, currentPicture.height);
    }
}