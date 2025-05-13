using UnityEngine;
using System;
using System.Linq;
using Unity.XR.CoreUtils;
using System.Collections.Generic;

public class BarcodeScanFrame : MonoBehaviour
{
    public Renderer ScanFrame;
    public BarcodeAnalyzer BarcodeAnalyzer;
    //[SerializeField] private BarcodeAnalyzer m_barcodeAnalyzer;
    private bool m_isScanning = false;
    private bool m_isCapturingPictures = true;
    public PassthroughCameraFrameProvider PassthroughCameraFrameProvider;
    public Camera PassthroughCamera;
    private OpenFoodFactsClient _openFoodFactsClient;
    [SerializeField] private GameObject productInfoPrefab;


    public Renderer LiveDebugRenderer; // z. B. ein kleines Quad darunter


    private void Start()
    {
        ScanFrame.enabled = false;
        LiveDebugRenderer.enabled = false;
        BarcodeAnalyzer = new BarcodeAnalyzer();

        try
        {
            _openFoodFactsClient = new OpenFoodFactsClient();
        }
        catch (Exception exception)
        {
            Debug.LogError("Open Food Facts Client could not be initialised " + exception.Message);
            _openFoodFactsClient = null;
        }

        // This can probably be improved on
        float time = 1.0f; // Start 1 second from the beginning
        float repeatRate = 1f; // Repeat every second
        InvokeRepeating("Scan", time, repeatRate);
    }

    private void Update()
    {
        if (OVRInput.GetDown(OVRInput.Button.One))
        {
            m_isScanning = !m_isScanning;
            ScanFrame.enabled = m_isScanning;
            LiveDebugRenderer.enabled = m_isScanning;
        }
    }

    private void Scan()
    {
        if (!m_isScanning)
            return;

        var newPixelsFrame = GetSubFrameFromQuad(PassthroughCameraFrameProvider.CurrentFrame);
        newPixelsFrame.Pixels = ToBlackAndWhite(newPixelsFrame.Pixels, 80);
        //Debug.LogError("PIXELS: " + newPixelsFrame.Pixels);

        if (m_isCapturingPictures && m_isScanning)
        {
            if (LiveDebugRenderer != null && newPixelsFrame.Pixels != null)
            {
                var tex = new Texture2D(newPixelsFrame.Width, newPixelsFrame.Height, TextureFormat.RGBA32, false);
                tex.SetPixels32(newPixelsFrame.Pixels);
                tex.Apply();

                LiveDebugRenderer.material.mainTexture = tex;
            }
        }
    
        string ean = BarcodeAnalyzer.Analyze(newPixelsFrame);
        if (!string.IsNullOrEmpty(ean))
        {
            StartCoroutine(_openFoodFactsClient.GetProductByEan(ean,
                onSuccess: (root) => {
                    //Debug.LogWarning($"ProduktEAN: {root.product._id}");
                    //Debug.LogWarning($"ProduktName: {root.product.product_name}");
                    EndScan();
                    ShowProductInfo(root);
                },
                onError: (err) => Debug.LogError(err)
            ));
        }
        //CapturePicture(newPixelsFrame.Pixels, newPixelsFrame.Width, newPixelsFrame.Height);
        // var couldAnalyze = BarcodeAnalyzer.Analyze(newPixelsFrame);
        // if (couldAnalyze == true)
        // {
        //     EndScan();
        // }
    }

    public void EndScan()
    {
        if (m_isScanning)
        {
            m_isScanning = false;
            ScanFrame.enabled = m_isScanning;
            LiveDebugRenderer.enabled = m_isScanning;
        }
    }


  
    // private Frame GetSubFrameFromQuad(Frame currentFrame)
    // {
    //     // 1) Mesh‑UVs holen
    //     var mesh = ScanFrame.GetComponent<MeshFilter>().sharedMesh;
    //     var uvs  = mesh.uv; // 4 Einträge für dein Quad

    //     // 2) UV‑Min/Max
    //     float minU = uvs.Min(u => u.x), maxU = uvs.Max(u => u.x);
    //     float minV = uvs.Min(u => u.y), maxV = uvs.Max(u => u.y);

    //     // 3) in Pixel‑Koordinaten übersetzen
    //     int startX = Mathf.FloorToInt(minU * currentFrame.Width);
    //     int startY = Mathf.FloorToInt(minV * currentFrame.Height);
    //     int w      = Mathf.CeilToInt((maxU - minU) * currentFrame.Width);
    //     int h      = Mathf.CeilToInt((maxV - minV) * currentFrame.Height);

    //     // 4) Sicherheit
    //     if (w <= 0 || h <= 0) return new Frame(null, 0, 0);

    //     // 5) Pixel kopieren wie zuvor
    //     var outPixels = new Color32[w * h];
    //     var src       = currentFrame.Pixels;
    //     for (int yy = 0; yy < h; yy++)
    //     {
    //         int sy = startY + yy;
    //         if (sy < 0 || sy >= currentFrame.Height) continue;
    //         for (int xx = 0; xx < w; xx++)
    //         {
    //             int sx = startX + xx;
    //             if (sx < 0 || sx >= currentFrame.Width) continue;
    //             outPixels[yy * w + xx] = src[sy * currentFrame.Width + sx];
    //         }
    //     }

    //     return new Frame(outPixels, w, h);
    // }

    // private Frame GetSubFrameFromQuad(Frame currentFrame)
    // {
    //     // 1) Eckpunkte in Screen‑Koordinaten
    //     var localCorners = new Vector3[]
    //     {
    //         new(-0.5f, -0.5f, 0),
    //         new( 0.5f, -0.5f, 0),
    //         new(-0.5f,  0.5f, 0),
    //         new( 0.5f,  0.5f, 0)
    //     };
    //     var pix = new List<Vector2Int>(4);
    //     foreach (var lc in localCorners)
    //     {
    //         Vector3 world = ScanFrame.transform.TransformPoint(lc);
    //         pix.Add(WorldToPixelCoords(world, currentFrame.Width, currentFrame.Height));
    //     }
    //     int minX = pix.Min(p => p.x), maxX = pix.Max(p => p.x);
    //     int minY = pix.Min(p => p.y), maxY = pix.Max(p => p.y);

    //     int w = maxX - minX, h = maxY - minY;
    //     if (w<=0 || h<=0) return new Frame(null,0,0);
    //     var outPixels = new Color32[w*h];
    //     var src = currentFrame.Pixels;
    //     // 2) Copy mit Bounds‑Check
    //     for(int yy=0; yy<h; yy++){
    //         int sy = minY + yy;
    //         if (sy<0 || sy>=currentFrame.Height) continue;
    //         for(int xx=0; xx<w; xx++){
    //             int sx = minX + xx;
    //             if (sx<0 || sx>=currentFrame.Width) continue;
    //             outPixels[yy*w + xx] = src[sy*currentFrame.Width + sx];
    //         }
    //     }
    //     return new Frame(outPixels, w, h);
    // }
    // private Frame GetSubFrameFromQuad(Frame currentFrame)
    // {
    //     if (ScanFrame == null)
    //     {
    //         Debug.LogError("❌ ScanFrame oder currentFrame ist null");
    //         return new Frame(null, 0, 0);
    //     }

    //     // 1) Quad‑Ecken in Screen‑Pixel
    //     var localCorners = new Vector3[]
    //     {
    //         new(-0.5f, -0.5f, -2),
    //         new( 0.5f, -0.5f, -2),
    //         new(-0.5f,  0.5f, -2),
    //         new( 0.5f,  0.5f, -2)
    //     };
    //     var pts = new List<Vector2Int>(4);
    //     foreach (var lc in localCorners)
    //     {
    //         Vector3 world = ScanFrame.transform.TransformPoint(lc);
    //         Vector3 sp = PassthroughCamera.WorldToScreenPoint(world);
    //         // Wandle direkt in Passthrough‑Texture‑Pixel um:
    //         float normX = sp.x / PassthroughCamera.pixelWidth;
    //         float normY = sp.y / PassthroughCamera.pixelHeight;
    //         int px = Mathf.RoundToInt(normX * currentFrame.Width);
    //         int py = Mathf.RoundToInt(normY * currentFrame.Height);
    //         pts.Add(new Vector2Int(px, py));
    //     }

    //     // 2) Min/Max bestimmen
    //     int minX = pts.Min(p => p.x), maxX = pts.Max(p => p.x);
    //     int minY = pts.Min(p => p.y), maxY = pts.Max(p => p.y);
    //     int w = maxX - minX, h = maxY - minY;
    //     if (w <= 0 || h <= 0) return new Frame(null, 0, 0);

    //     // 3) Pixel ausschneiden
    //     var outPixels = new Color32[w * h];
    //     var src      = currentFrame.Pixels;
    //     for (int y = 0; y < h; y++)
    //     {
    //         // Drehe Y um, wenn dein Array [0] oben ist:
    //         //int sourceY = /* wenn Pixel[0] ganz oben: */ (currentFrame.Height - 1) - (minY + y);
    //         int sourceY = minY + y;
    //         // ansonsten einfach: int sourceY = minY + y;
    //         for (int x = 0; x < w; x++)
    //         {
    //             int sourceX = minX + x;
    //             if (sourceX < 0 || sourceX >= currentFrame.Width ||
    //                 sourceY < 0 || sourceY >= currentFrame.Height)
    //                 continue;

    //             outPixels[y * w + x] = src[sourceY * currentFrame.Width + sourceX];
    //         }
    //     }

    //     return new Frame(outPixels, w, h);
    // }


    private Frame GetSubFrameFromQuad(Frame currentFrame)
    {
        if (ScanFrame == null)
        {
            Debug.LogError("❌ ScanFrame oder currentFrame ist null");
            return new Frame(null, 0, 0);
        }

        // Lokale Ecken des Quads
        // var localCorners = new Vector3[]
        // {
        //     new(-0.5f, -0.5f, 0),
        //     new(0.5f, -0.5f, 0),
        //     new(-0.5f, 0.5f, 0),
        //     new(0.5f, 0.5f, 0)
        // };

        // // Weltkoordinaten der Ecken
        // var pixelCorners = new Vector2Int[4];
        // for (var i = 0; i < localCorners.Length; i++)
        // {
        //     var worldCorner = ScanFrame.transform.TransformPoint(localCorners[i]);
        //     pixelCorners[i] = WorldToPixelCoords(worldCorner, currentFrame.Width, currentFrame.Height);
        // }

        //Debug.LogWarning("pixelCorners: " + pixelCorners[0] + ", " + pixelCorners[1] + ", " + pixelCorners[2] + ", " + pixelCorners[3]);

        // Pixel-Min/Max berechnen
        // var minX = pixelCorners.Min(p => p.x);
        // var maxX = pixelCorners.Max(p => p.x);
        // var minY = pixelCorners.Min(p => p.y);
        // var maxY = pixelCorners.Max(p => p.y);

        // 1280 - 
        // 600 breit
        // 960
        var minX = 340;
        var maxX = 940;
        var minY = 380;
        var maxY = 580;
        //Debug.LogWarning("minX: " + minX + ", maxX: " + maxX + ", minY: " + minY + ", maxY: " + maxY);

        var width = maxX - minX;
        var height = maxY - minY;

        //Debug.LogWarning("width: " + width + ", height: " + height);

        // Sicherheits-Check
        if (width <= 0 || height <= 0)
        {
            Debug.LogWarning("⚠️ Berechnete Breite oder Höhe ist ungültig");
            return new Frame(null, 0, 0);
        }

        var pixelsInQuad = new Color32[width * height];
        var originalPixels = currentFrame.Pixels;

        for (var y = 0; y < height; y++)
        {
            //var sourceY = minY + y;
            // var sourceY = minY - y; = MIRROR
            var sourceY = maxY + y;
            for (var x = 0; x < width; x++)
            {
                var sourceX = minX + x;

                if (sourceX >= 0 && sourceX < currentFrame.Width &&
                    sourceY >= 0 && sourceY < currentFrame.Height)
                {
                    var sourceIndex = sourceY * currentFrame.Width + sourceX;
                    var destIndex = y * width + x;
                    pixelsInQuad[destIndex] = originalPixels[sourceIndex];
                }
            }
        }

        return new Frame(pixelsInQuad, width, height);
    }

    // private Vector2Int WorldToPixelCoords(Vector3 worldPosition, int frameWidth, int frameHeight)
    // {
    //     Vector3 screenPoint = Camera.main.WorldToScreenPoint(worldPosition);
    //     // **Wichtig:** hier pixelWidth/Height der Camera nutzen, nicht Screen.width
    //     float camPW = Camera.main.pixelWidth;
    //     float camPH = Camera.main.pixelHeight;
    //     float scaleX = frameWidth  / camPW;
    //     float scaleY = frameHeight / camPH;
    //     int x = Mathf.RoundToInt(screenPoint.x * scaleX);
    //     int y = Mathf.RoundToInt(screenPoint.y * scaleY);
    //     return new Vector2Int(x, y);
    // }

    // private Vector2Int WorldToPixelCoords(Vector3 worldPosition, int frameWidth, int frameHeight)
    // {
    //     // liefert x/y in Screen‑Pixeln (0…Screen.width, 0…Screen.height)
    //     Vector3 screenPoint = Camera.main.WorldToScreenPoint(worldPosition);
    //     // Passe auf, wenn die Kamera andere Auflösung als dein Frame hat
    //     float scaleX = frameWidth  / (float)Screen.width;
    //     float scaleY = frameHeight / (float)Screen.height;
    //     int x = Mathf.RoundToInt(screenPoint.x * scaleX);
    //     int y = Mathf.RoundToInt(screenPoint.y * scaleY);
    //     return new Vector2Int(x, y);
    // }

    private Vector2Int WorldToPixelCoords(Vector3 worldPosition, int frameWidth, int frameHeight)
    {
        var viewportPoint = PassthroughCamera.WorldToViewportPoint(worldPosition);
        var x = Mathf.RoundToInt(viewportPoint.x * frameWidth);
        var y = Mathf.RoundToInt(frameHeight - viewportPoint.y * frameHeight);
        var y2 = Mathf.RoundToInt(viewportPoint.y * frameHeight);
        Debug.LogWarning("y: " + y + ", y2:" + y2);

        return new Vector2Int(x, y);
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
        Texture2D currentPicture = new Texture2D(width, height, TextureFormat.RGBA32, false);
        currentPicture.SetPixels32(pixels);
        currentPicture.Apply();

        var currentDate = DateTime.Now;
        var formattedDateTime = currentDate.ToString("yyyy-MM-dd_HH:mm:ss");
        System.IO.File.WriteAllBytes(Application.persistentDataPath + "/" + formattedDateTime + ".png", currentPicture.EncodeToPNG());
    }

    // void ShowProductInfo(Root root)
    // {
    //     var panel = GetComponent<ProductInfoPanel>();
    //     panel.SetData(root.product.product_name);
    // }

    void ShowProductInfo(Root root)
    {
        var panel = productInfoPrefab.GetComponent<ProductInfoPanel>();
        
        if (panel == null)
        {
            Debug.LogError("ProductInfoPanel component not found on the specified GameObject.");
            return;
        }

        Debug.Log("ProductInfoPanel found, updating data.");
        panel.SetData(root.product.product_name);
    }
}