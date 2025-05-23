using UnityEngine.XR.Hands;
using System.Collections.Generic;
using UnityEngine;

// Dieses Skript gehört auf das ROOT-GameObject deines manuellen Scan-Frame-Prefabs.
// Es kümmert sich nur um die Platzierung, Rotation und Skalierung dieses GameObjects.
public class BarcodeScanFrameExtractor : MonoBehaviour
{
    private XRHandSubsystem handSubsystem;
    // testFrame und _scanFrameTestPrefab werden hier NICHT MEHR benötigt,
    // da dieses Skript direkt das GameObject manipuliert, an dem es hängt (this.gameObject).

    // Feste Größe für den ScanFrame
    [SerializeField] private float fixedFrameWidth = 0.05f; // Beispiel: 5 cm Breite
    private float originalPrefabAspectRatio = 1.0f; // Standardwert 1:1 (Quadrat), falls nicht ermittelbar
    private float originalPrefabDepth = 0.01f; // Standardtiefe, falls nicht ermittelbar oder gewünscht

    // Offset für die Position des ScanFrames relativ zur Mitte der Finger
    [SerializeField] private float verticalOffset = 0.05f; // Verschiebung nach oben (z.B. 5 cm)
    [SerializeField] private float horizontalOffset = 0.05f; // Verschiebung zur Seite (z.B. 5 cm nach rechts, relativ zur Kamera)


    void Awake()
    {
        // Hole das Hand-Subsystem direkt in Awake, um es früh verfügbar zu haben
        var handSubsystems = new List<XRHandSubsystem>();
        SubsystemManager.GetSubsystems(handSubsystems);

        if (handSubsystems.Count > 0)
        {
            handSubsystem = handSubsystems[0];
            Debug.Log("BarcodeScanFrameExtractor: XRHandSubsystem gefunden!");
        }
        else
        {
            Debug.LogError("BarcodeScanFrameExtractor: Kein XRHandSubsystem gefunden. Gestenverfolgung nicht möglich.");
            // enabled = false; // Nicht hier deaktivieren, da der Holder es steuert
        }

        // Ermittle das ursprüngliche Seitenverhältnis und die Tiefe des Prefabs, an dem dieses Skript hängt
        // (Annahme: Dieses Skript ist am Root des visuellen Frames)
        MeshFilter meshFilter = GetComponent<MeshFilter>();
        if (meshFilter != null && meshFilter.sharedMesh != null)
        {
            Vector3 localMeshSize = meshFilter.sharedMesh.bounds.size;
            if (localMeshSize.y != 0)
            {
                originalPrefabAspectRatio = localMeshSize.x / localMeshSize.y;
            }
            else
            {
                Debug.LogWarning("BarcodeScanFrameExtractor: Prefab-Mesh hat keine Höhe. Annahme 1:1 Seitenverhältnis.");
            }
            originalPrefabDepth = localMeshSize.z;
        }
        else
        {
            RectTransform rectTransform = GetComponent<RectTransform>();
            if (rectTransform != null && rectTransform.rect.height != 0)
            {
                originalPrefabAspectRatio = rectTransform.rect.width / rectTransform.rect.height;
                originalPrefabDepth = 0.01f; 
            }
            else
            {
                Debug.LogWarning("BarcodeScanFrameExtractor: Prefab hat weder MeshFilter/Mesh noch RectTransform. Annahme 1:1 Seitenverhältnis und Standardtiefe.");
            }
        }
    }

    void Update()
    {
        // Dieses Skript läuft nur, wenn sein GameObject aktiv ist (gesteuert vom Holder).
        // Wir müssen hier nur noch die Handverfolgung prüfen.
        if (handSubsystem == null || !handSubsystem.leftHand.isTracked)
        {
            // Wenn die Hand nicht verfolgt wird, können wir den Frame nicht positionieren.
            // Das GameObject wird vom Holder deaktiviert, wenn der Scan stoppt.
            return;
        }

        var indexJoint = handSubsystem.leftHand.GetJoint(XRHandJointID.IndexTip);
        var thumbJoint = handSubsystem.leftHand.GetJoint(XRHandJointID.ThumbTip);

        if (!thumbJoint.TryGetPose(out Pose thumbPose) || !indexJoint.TryGetPose(out Pose indexPose))
        {
            // Wenn Posen nicht verfügbar sind, können wir den Frame nicht positionieren.
            return;
        }

        Vector3 thumbPos = thumbPose.position;
        Vector3 indexPos = indexPose.position;

        Vector3 midPoint = (thumbPos + indexPos) / 2f;

        // Positioniere den Frame mit Offset
        Vector3 offsetPosition = midPoint + Vector3.up * verticalOffset + Camera.main.transform.right * horizontalOffset;
        this.transform.position = offsetPosition; // Manipuliere die eigene Position

        // Rotiere den Frame, sodass er immer zur Kamera zeigt und aufrecht ist
        if (Camera.main != null)
        {
            this.transform.rotation = Quaternion.LookRotation(Camera.main.transform.forward, Vector3.up); // Manipuliere die eigene Rotation
        }
        else
        {
            this.transform.rotation = Quaternion.identity; // Keine Rotation
        }

        // Skaliere den Frame mit fester Breite und proportionaler Höhe
        float newWidth = fixedFrameWidth;
        float newHeight = newWidth / originalPrefabAspectRatio;
        
        this.transform.localScale = new Vector3(newWidth, newHeight, originalPrefabDepth); // Manipuliere die eigene Skalierung
    }
}















// using UnityEngine;

// public class BarcodeScanFrameExtractor : MonoBehaviour
// {
//     [SerializeField] private Camera _passthroughCamera; // z.B. Camera.main oder deine AR/OVR-Kamera
//     [SerializeField] private Renderer quadRenderer;

//     public Texture2D GetExtractedTexture(WebCamTexture texture)
//     {
//         var minX = 440;
//         var maxX = 840;
//         var minY = 255;
//         var maxY = 705;
//         //Debug.LogWarning("minX: " + minX + ", maxX: " + maxX + ", minY: " + minY + ", maxY: " + maxY);

//         var width = maxX - minX;
//         var height = maxY - minY;

//         //Debug.LogWarning("width: " + width + ", height: " + height);

//         // Sicherheits-Check
//         if (width <= 0 || height <= 0)
//         {
//             Debug.LogWarning("⚠️ Berechnete Breite oder Höhe ist ungültig");
//             // return new Frame(null, 0, 0);
//         }

//         var pixelsInQuad = new Color32[width * height];
//         var originalPixels = texture.GetPixels32();


//         for (var y = 0; y < height; y++)
//         {
//             // var sourceY = minY + y;
//             var sourceY = maxY + y;
//             for (var x = 0; x < width; x++)
//             {
//                 var sourceX = minX + x;

//                 if (sourceX >= 0 && sourceX < texture.width &&
//                     sourceY >= 0 && sourceY < texture.height)
//                 {
//                     var sourceIndex = sourceY * texture.width + sourceX;
//                     var destIndex = y * width + x;
//                     pixelsInQuad[destIndex] = originalPixels[sourceIndex];
//                 }
//             }
//         }

//         var newTexture = new Texture2D(width, height);
//         newTexture.SetPixels32(pixelsInQuad);
//         newTexture.Apply();

//         return newTexture;
//     }

    // public Frame GetSubFrameFromScanFrame(Frame currentFrame)
    // {
    //     // ScanFrame-Quad Mesh holen
    //     var meshFilter = quadRenderer.GetComponent<MeshFilter>();
    //     if (meshFilter == null || meshFilter.mesh == null)
    //     {
    //         Debug.LogError("MeshFilter or mesh missing.");
    //         return new Frame();
    //     }

    //     // Die 4 Eckpunkte des Quads in Weltkoordinaten
    //     var quadCorners = new Vector3[4];
    //     var bounds = quadRenderer.bounds;
    //     quadCorners[0] = new Vector3(bounds.min.x, bounds.min.y, bounds.center.z); // bottom left
    //     quadCorners[1] = new Vector3(bounds.max.x, bounds.min.y, bounds.center.z); // bottom right
    //     quadCorners[2] = new Vector3(bounds.min.x, bounds.max.y, bounds.center.z); // top left
    //     quadCorners[3] = new Vector3(bounds.max.x, bounds.max.y, bounds.center.z); // top right

    //     // Viewport ist 0..1 in x/y
    //     Vector2 min = new Vector2(1f, 1f);
    //     Vector2 max = new Vector2(0f, 0f);

    //     foreach (var corner in quadCorners)
    //     {
    //         Vector3 viewportPos = _passthroughCamera.WorldToViewportPoint(corner);
    //         min = Vector2.Min(min, viewportPos);
    //         max = Vector2.Max(max, viewportPos);
    //     }

    //     int x = Mathf.RoundToInt(min.x * currentFrame.Width);
    //     int y = Mathf.RoundToInt(min.y * currentFrame.Height);

    //     int width = Mathf.RoundToInt((max.x - min.x) * currentFrame.Width);
    //     int height = Mathf.RoundToInt((max.y - min.y) * currentFrame.Height);

    //     Color32[] pixels = new Color32[width * height];

    //     for (int row = 0; row < height; row++)
    //     {
    //         for (int col = 0; col < width; col++)
    //         {
    //             int sourceIndex = (y + row) * currentFrame.Width + (x + col);
    //             int destIndex = row * width + col;

    //             if (sourceIndex < currentFrame.Pixels.Length && sourceIndex >= 0)
    //             {
    //                 pixels[destIndex] = currentFrame.Pixels[sourceIndex];
    //             }
    //         }
    //     }

    //     Frame newFrame = new Frame(pixels, width, height);
    //     newFrame.Pixels = ToBlackAndWhite(newFrame.Pixels, 80);
    //     return newFrame;
    // }
    // public Frame GetSubFrameFromScanFrame(Frame currentFrame)
    // {
    //     Vector3[] localCorners = new Vector3[]
    //     {
    //         new Vector3(-0.5f, -0.5f, 0), // unten links
    //         new Vector3(-0.5f,  0.5f, 0), // oben links
    //         new Vector3( 0.5f,  0.5f, 0), // oben rechts
    //         new Vector3( 0.5f, -0.5f, 0), // unten rechts
    //     };
    //     Vector3[] worldCorners = new Vector3[4];
    //     for (int i = 0; i < 4; i++)
    //         worldCorners[i] = quadRenderer.transform.TransformPoint(localCorners[i]);

    //     // 2) Projiziere sie in Screen-Koordinaten (Pixel) der previewCamera
    //     Vector2[] screenPts = new Vector2[4];
    //     for (int i = 0; i < 4; i++)
    //     {
    //         Vector3 sp = passthroughCamera.WorldToScreenPoint(worldCorners[i]);
    //         screenPts[i] = new Vector2(sp.x, sp.y);
    //     }

    //     // 3) Ermittle das umschließende Rechteck in Screen-Pixeln
    //     float minX = Mathf.Min(screenPts[0].x, screenPts[1].x, screenPts[2].x, screenPts[3].x);
    //     float maxX = Mathf.Max(screenPts[0].x, screenPts[1].x, screenPts[2].x, screenPts[3].x);
    //     float minY = Mathf.Min(screenPts[0].y, screenPts[1].y, screenPts[2].y, screenPts[3].y);
    //     float maxY = Mathf.Max(screenPts[0].y, screenPts[1].y, screenPts[2].y, screenPts[3].y);

    //     // 4) Normiere auf [0,1] je nach Kamerapixel-Maßen
    //     float camW = passthroughCamera.pixelWidth;
    //     float camH = passthroughCamera.pixelHeight;
    //     float uMin = Mathf.Clamp01(minX / camW);
    //     float uMax = Mathf.Clamp01(maxX / camW);
    //     float vMin = Mathf.Clamp01(minY / camH);
    //     float vMax = Mathf.Clamp01(maxY / camH);

    //     // 5) Rechne in Frame-Pixels um (Frame origin: oben links)
    //     int texX = Mathf.RoundToInt(uMin * currentFrame.Width);
    //     int texW = Mathf.RoundToInt((uMax - uMin) * currentFrame.Width);

    //     // Wichtig: Screen-Y=0 ist unten, Frame.Y=0 ist oben ⇒ invertiere v
    //     float vMinInv = 1f - vMax; // oberes Ende des Rechtecks im Frame
    //     float vMaxInv = 1f - vMin; // unteres Ende

    //     int texY = Mathf.RoundToInt(vMinInv * currentFrame.Height);
    //     int texH = Mathf.RoundToInt((vMaxInv - vMinInv) * currentFrame.Height);

    //     // 6) Clamp, damit wir nicht aus dem Array laufen
    //     texX = Mathf.Clamp(texX, 0, currentFrame.Width);
    //     texY = Mathf.Clamp(texY, 0, currentFrame.Height);
    //     texW = Mathf.Clamp(texW, 0, currentFrame.Width - texX);
    //     texH = Mathf.Clamp(texH, 0, currentFrame.Height - texY);

    //     if (texW == 0 || texH == 0)
    //         return new Frame(null, 0, 0);

    //     // 7) Extrahiere die Pixel aus dem Frame-Array
    //     var resultPixels = new Color32[texW * texH];
    //     var src = currentFrame.Pixels;
    //     int srcW = currentFrame.Width;
    //     for (int y = 0; y < texH; y++)
    //     {
    //         int srcRow = (texY + y) * srcW;
    //         int dstRow = y * texW;
    //         for (int x = 0; x < texW; x++)
    //             resultPixels[dstRow + x] = src[srcRow + texX + x];
    //     }

    //     return new Frame(resultPixels, texW, texH);
    // }






    // public Frame GetSubFrameFromScanFrame(Frame currentFrame)
    // {
    //     // Lokale Ecken des Quads
    //     // var localCorners = new Vector3[]
    //     // {
    //     //     new(-0.5f, -0.5f, 0),
    //     //     new(0.5f, -0.5f, 0),
    //     //     new(-0.5f, 0.5f, 0),
    //     //     new(0.5f, 0.5f, 0)
    //     // };

    //     // // Weltkoordinaten der Ecken
    //     // var pixelCorners = new Vector2Int[4];
    //     // for (var i = 0; i < localCorners.Length; i++)
    //     // {
    //     //     var worldCorner = ScanFrame.transform.TransformPoint(localCorners[i]);
    //     //     pixelCorners[i] = WorldToPixelCoords(worldCorner, currentFrame.Width, currentFrame.Height);
    //     // }

    //     //Debug.LogWarning("pixelCorners: " + pixelCorners[0] + ", " + pixelCorners[1] + ", " + pixelCorners[2] + ", " + pixelCorners[3]);

    //     // Pixel-Min/Max berechnen
    //     // var minX = pixelCorners.Min(p => p.x);
    //     // var maxX = pixelCorners.Max(p => p.x);
    //     // var minY = pixelCorners.Min(p => p.y);
    //     // var maxY = pixelCorners.Max(p => p.y);

    //     // 1280 - 
    //     // 600 breit
    //     // 960
    //     var minX = 440;
    //     var maxX = 840;
    //     var minY = 255;
    //     var maxY = 705;
    //     //Debug.LogWarning("minX: " + minX + ", maxX: " + maxX + ", minY: " + minY + ", maxY: " + maxY);

    //     var width = maxX - minX;
    //     var height = maxY - minY;

    //     //Debug.LogWarning("width: " + width + ", height: " + height);

    //     // Sicherheits-Check
    //     if (width <= 0 || height <= 0)
    //     {
    //         Debug.LogWarning("⚠️ Berechnete Breite oder Höhe ist ungültig");
    //         return new Frame(null, 0, 0);
    //     }

    //     var pixelsInQuad = new Color32[width * height];
    //     var originalPixels = currentFrame.Pixels;

    //     for (var y = 0; y < height; y++)
    //     {
    //         // var sourceY = minY + y;
    //         var sourceY = maxY + y;
    //         for (var x = 0; x < width; x++)
    //         {
    //             var sourceX = minX + x;

    //             if (sourceX >= 0 && sourceX < currentFrame.Width &&
    //                 sourceY >= 0 && sourceY < currentFrame.Height)
    //             {
    //                 var sourceIndex = sourceY * currentFrame.Width + sourceX;
    //                 var destIndex = y * width + x;
    //                 pixelsInQuad[destIndex] = originalPixels[sourceIndex];
    //             }
    //         }
    //     }

    //     var newFrame = new Frame(pixelsInQuad, width, height);
    //     newFrame.Pixels = ToBlackAndWhite(newFrame.Pixels, 80);

    //     return newFrame;
    // }

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

    // private Vector2Int WorldToPixelCoords(Vector3 worldPosition, int frameWidth, int frameHeight)
    // {
    //     var viewportPoint = PassthroughCamera.WorldToViewportPoint(worldPosition);
    //     var x = Mathf.RoundToInt(viewportPoint.x * frameWidth);
    //     var y = Mathf.RoundToInt(frameHeight - viewportPoint.y * frameHeight);
    //     var y2 = Mathf.RoundToInt(viewportPoint.y * frameHeight);
    //     Debug.LogWarning("y: " + y + ", y2:" + y2);

    //     return new Vector2Int(x, y);
    // }

//     private Color32[] ToBlackAndWhite(Color32[] pixels, int threshold)
//     {
//         var bwPixels = new Color32[pixels.Length];

//         for (var i = 0; i < pixels.Length; i++)
//         {
//             var p = pixels[i];
//             var gray = (byte)(0.299f * p.r + 0.587f * p.g + 0.114f * p.b);
//             var bw = (gray < threshold) ? (byte)0 : (byte)255;
//             bwPixels[i] = new Color32(bw, bw, bw, 255);
//         }

//         return bwPixels;
//     }
// }