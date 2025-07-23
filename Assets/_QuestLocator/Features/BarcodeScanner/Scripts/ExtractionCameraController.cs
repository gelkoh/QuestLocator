// using UnityEngine;

// public class ExtractionCameraController : MonoBehaviour
// {
//     [Header("References")]
//     public Camera extractionCamera;
//     public GameObject webCamDisplayQuad;
//     public HandRelativePositionCalculator handPositionCalculator;

//     // Define the size of the extraction area relative to the webCamDisplayQuad (e.g., 0.2 means 20% of quad's width/height)
//     // Make this public to adjust in inspector
//     public Vector2 extractionAreaRelativeSize = new Vector2(1f, 1f);
//     public bool followHandPosition = true;

//     private Renderer quadRenderer;
//     private Vector3 quadWorldSize; // Store the actual world size of the quad

//     private RenderTexture _extractionCameraRenderTexture;

//     private int TEXTURE_DIMENSIONS = 512; // Resolution of the extracted texture

//     void Start()
//     {
//         if (extractionCamera == null)
//         {
//             Debug.LogError("[ExtractionCameraController] Extraction camera is null!");
//             enabled = false;
//             return;
//         }

//         if (handPositionCalculator == null)
//         {
//             Debug.LogError("[ExtractionCameraController] Hand position calculator is null!");
//             enabled = false;
//             return;
//         }

//         if (webCamDisplayQuad != null)
//         {
//             quadRenderer = webCamDisplayQuad.GetComponent<Renderer>();

//             if (quadRenderer != null)
//             {
//                 quadWorldSize = quadRenderer.bounds.size; // Get the world size of the quad
//             }
//             else
//             {
//                 Debug.LogWarning("[ExtractionCameraController] WebCamDisplayQuad doesn't have a Renderer component!");
//                 enabled = false;
//                 return;
//             }
//         }
//         else
//         {
//             Debug.LogError("[ExtractionCameraController] WebCamDisplayQuad is null!");
//             enabled = false;
//             return;
//         }

//         // Ensure extraction camera is orthographic for pixel extraction
//         extractionCamera.orthographic = true;

//         // Setup the RenderTexture for the extraction camera
//         if (_extractionCameraRenderTexture == null)
//         {
//             _extractionCameraRenderTexture = new RenderTexture(TEXTURE_DIMENSIONS, TEXTURE_DIMENSIONS, 24)
//             {
//                 filterMode = FilterMode.Bilinear, // Good for scaling, adjust if pixel-perfect needed
//                 enableRandomWrite = false // Not needed for standard rendering
//             };

//             extractionCamera.targetTexture = _extractionCameraRenderTexture;
//         }
//         else if (extractionCamera.targetTexture == null)
//         {
//             extractionCamera.targetTexture = _extractionCameraRenderTexture;
//         }
//     }

//     void Update()
//     {
//         if (!followHandPosition || handPositionCalculator == null || extractionCamera == null || webCamDisplayQuad == null)
//             return;

//         UpdateExtractionCameraPosition();
//     }

//     void UpdateExtractionCameraPosition()
//     {
//         Vector3 handRelativePos = handPositionCalculator.GetHandRelativePosition();

//         // Convert the 0-1 relative position to a world position on the quad's surface
//         Vector3 quadWorldTargetPos = ConvertRelativeToQuadWorldPosition(handRelativePos);

//         // Position the extraction camera to look at this target
//         PositionExtractionCamera(quadWorldTargetPos);

//         // Update the orthographic size and aspect ratio of the extraction camera
//         UpdateOrthographicSize();
//     }

//     // This method now correctly maps the relative (0-1) position to a world point on the quad's surface,
//     // accounting for the quad's rotation.
//     Vector3 ConvertRelativeToQuadWorldPosition(Vector3 relativePos)
//     {
//         if (webCamDisplayQuad == null || quadRenderer == null) return Vector3.zero;

//         // Quad's center in world space
//         Vector3 quadCenter = webCamDisplayQuad.transform.position;

//         // Get the quad's half-extents in its local space
//         Vector3 localHalfSize = quadRenderer.bounds.extents;

//         // Calculate the local x and y offset from the quad's center based on relativePos (0-1 range)
//         // relativePos.x and y are from 0 (bottom/left) to 1 (top/right).
//         // We convert this to a -0.5 to 0.5 range for local offset calculation, then multiply by full size.
//         float localXOffset = (relativePos.x - 0.5f) * 2f * localHalfSize.x;
//         float localYOffset = (relativePos.y - 0.5f) * 2f * localHalfSize.y;

//         // Calculate the world space offset using the quad's local axes (right and up)
//         Vector3 offset = webCamDisplayQuad.transform.right * localXOffset +
//                          webCamDisplayQuad.transform.up * localYOffset;

//         // The target world position is the quad's center plus the calculated offset
//         return quadCenter + offset;
//     }

//     void UpdateOrthographicSize()
//     {
//         // Calculate the desired height of the view based on the quad's world height and the relative extraction size.
//         // quadWorldSize.y is the total height of the webCamDisplayQuad in world units.
//         float extractionHeightWorld = quadWorldSize.y * extractionAreaRelativeSize.y;

//         // For an orthographic camera, orthographicSize is half the vertical size of the viewing frustum.
//         extractionCamera.orthographicSize = extractionHeightWorld * 0.5f;

//         // Set the aspect ratio of the extraction camera to match the desired extraction area
//         // This ensures the extracted area isn't stretched/squashed if extractionAreaRelativeSize.x != extractionAreaRelativeSize.y
//         float aspectRatio = extractionAreaRelativeSize.x / extractionAreaRelativeSize.y;
//         extractionCamera.aspect = aspectRatio;
//     }

//     public Texture2D ExtractPixels()
//     {
//         if (extractionCamera == null || _extractionCameraRenderTexture == null) return null;

//         RenderTexture currentRT = RenderTexture.active; // Store current active RT
//         extractionCamera.Render(); // Force the extraction camera to render

//         RenderTexture.active = _extractionCameraRenderTexture; // Set the target RT as active
//         // Create a new Texture2D and read pixels from the active RenderTexture
//         Texture2D result = new Texture2D(TEXTURE_DIMENSIONS, TEXTURE_DIMENSIONS, TextureFormat.RGB24, false);
//         result.ReadPixels(new Rect(0, 0, TEXTURE_DIMENSIONS, TEXTURE_DIMENSIONS), 0, 0);
//         result.Apply(); // Apply changes to the texture

//         RenderTexture.active = currentRT; // Restore previous active RT

//         return result;
//     }

//     void PositionExtractionCamera(Vector3 targetWorldPos)
//     {
//         // The camera should always look directly at the quad's surface.
//         // It should be positioned along the quad's *backward* normal to look *at* the quad.
//         Vector3 quadNormal = webCamDisplayQuad.transform.forward;
//         // A small distance behind the quad is usually sufficient for orthographic cameras.
//         // The exact distance is less critical for orthographic cameras regarding what's visible,
//         // but it must be within clipping planes.
//         float cameraDistanceOffset = 1f; // Adjust this value as needed

//         // Position the camera behind the target point on the quad, looking forward.
//         Vector3 cameraPosition = targetWorldPos - (quadNormal * cameraDistanceOffset);
//         extractionCamera.transform.position = cameraPosition;

//         // Make the camera look directly at the target point.
//         // The 'up' vector helps define the camera's orientation. Vector3.up is standard.
//         extractionCamera.transform.LookAt(targetWorldPos, Vector3.up);
//     }

//     void OnDestroy()
//     {
//         if (_extractionCameraRenderTexture != null)
//         {
//             _extractionCameraRenderTexture.Release();
//             Destroy(_extractionCameraRenderTexture);
//         }
//     }
// }




using UnityEngine;

public class ExtractionCameraController : MonoBehaviour
{
    [Header("References")]
    public Camera extractionCamera;
    public GameObject webCamDisplayQuad;
    public HandRelativePositionCalculator handPositionCalculator;

    // Keep extractionAreaSize as it's now how you define the size on the quad
    private Vector2 extractionAreaSize = new Vector2(2f, 2f);
    public bool followHandPosition = true;

    private Renderer quadRenderer;
    private Vector3 quadSize; // Will be used if webCamDisplayQuad has a renderer

    private RenderTexture _extractionCameraRenderTexture;

    private int TEXTURE_DIMENSIONS = 1024;

    void Start()
    {
        if (extractionCamera == null)
        {
            Debug.LogError("[ExtractionCameraController] Extraction camera is null");
        }

        if (handPositionCalculator == null)
        {
            Debug.LogError("[ExtractionCameraController] Hand position calculator is null");
        }

        if (webCamDisplayQuad != null)
        {
            quadRenderer = webCamDisplayQuad.GetComponent<Renderer>();

            if (quadRenderer != null)
            {
                quadSize = quadRenderer.bounds.size;
            }
            else
            {
                Debug.LogWarning("[ExtractionCameraController] WebCamDisplayQuad doesn't have a Renderer component");
            }
        }

        if (extractionCamera != null)
        {
            extractionCamera.orthographic = true;
        }

        // Initialize or re-use the RenderTexture
        if (_extractionCameraRenderTexture == null || _extractionCameraRenderTexture.width != TEXTURE_DIMENSIONS || _extractionCameraRenderTexture.height != TEXTURE_DIMENSIONS)
        {
            if (_extractionCameraRenderTexture != null)
            {
                _extractionCameraRenderTexture.Release(); // Release existing if dimensions changed
            }
            _extractionCameraRenderTexture = new RenderTexture(TEXTURE_DIMENSIONS, TEXTURE_DIMENSIONS, 24);
            extractionCamera.targetTexture = _extractionCameraRenderTexture;
        }
        else if (extractionCamera.targetTexture == null)
        {
            extractionCamera.targetTexture = _extractionCameraRenderTexture;
        }


        ValidateSetup();
    }

    void Update()
    {
        if (!followHandPosition || handPositionCalculator == null || extractionCamera == null || webCamDisplayQuad == null)
            return;

        UpdateExtractionCameraPosition();
    }

    void UpdateExtractionCameraPosition()
    {
        // Get the relative position of the hand (which corresponds to the scan frame's desired location on the quad)
        Vector3 handRelativePos = handPositionCalculator.GetHandRelativePosition();

        // Convert this relative position to a world position on the webCamDisplayQuad
        Vector3 quadWorldTargetPos = ConvertRelativeToQuadWorldPosition(handRelativePos);

        // Position the extraction camera to look at this quadWorldTargetPos
        // We no longer directly use handWorldRotation for the camera's rotation
        PositionExtractionCamera(quadWorldTargetPos);

        // Update the orthographic size based on the defined extraction area size
        UpdateOrthographicSize();
    }


    // MODIFIED: This method now only takes the target position
    void PositionExtractionCamera(Vector3 targetWorldPos)
    {
        // Get the normal direction of the webCamDisplayQuad's surface.
        // The camera should be positioned 'behind' this surface relative to its normal.
        Vector3 quadNormal = webCamDisplayQuad.transform.forward; // Assuming the quad's forward is its normal.

        // Define the distance the camera should be from the target position.
        // This needs to be carefully chosen so the camera is far enough to capture
        // the desired area of the quad without clipping, but not so far that the
        // quad appears too small. 1f might be too far, especially for an orthographic camera
        // capturing a small area. Let's make it a serialized field or a more reasonable default.
        float cameraDistance = 0.5f; // Adjusted from 1f - you'll need to fine-tune this!

        // Calculate the camera's position: move 'back' from the target along the quad's normal
        Vector3 cameraPosition = targetWorldPos - (quadNormal * cameraDistance);
        extractionCamera.transform.position = cameraPosition;

        // NEW LOGIC: Make the camera look directly at the target position.
        // The 'up' direction for LookAt should typically be world's up (Vector3.up),
        // unless your quad (and thus barcode) can be arbitrarily rotated.
        // For general barcode scanning, keeping it upright relative to the world is usually best.
        extractionCamera.transform.LookAt(targetWorldPos, Vector3.up);

        // Debugging logs
        Debug.Log($"[ExtractionCameraController] Target World Pos: {targetWorldPos}");
        Debug.Log($"[ExtractionCameraController] Camera Pos: {extractionCamera.transform.position}, Rot: {extractionCamera.transform.rotation}");
    }


    Vector3 ConvertRelativeToQuadWorldPosition(Vector3 relativePos)
    {
        if (webCamDisplayQuad == null) return Vector3.zero;

        // Ensure quadRenderer is not null before accessing its bounds
        if (quadRenderer == null)
        {
            Debug.LogWarning("[ExtractionCameraController] quadRenderer is null in ConvertRelativeToQuadWorldPosition. Returning Vector3.zero.");
            return Vector3.zero;
        }

        Bounds quadBounds = quadRenderer.bounds;

        // relativePos.x and relativePos.y are expected to be 0-1 (viewport coordinates)
        // These are then linearly interpolated across the world bounds of the quad.
        float worldX = Mathf.Lerp(quadBounds.min.x, quadBounds.max.x, relativePos.x);
        float worldY = Mathf.Lerp(quadBounds.min.y, quadBounds.max.y, relativePos.y);
        // The Z component from handRelativePos is likely depth relative to the main camera,
        // and its interpretation here for placing on the quad might need careful consideration.
        // For a flat quad, you might want to project the target point onto the quad's plane.
        // For now, we'll keep the Z from relativePos as it was, but this is a potential source of error.
        float worldZ = Mathf.Lerp(quadBounds.min.z, quadBounds.max.z, relativePos.z);


        // OPTIONAL IMPROVEMENT: Project onto the quad's plane for more accuracy
        // This would ensure the target point is *exactly* on the quad's surface.
        // However, it makes the Z-component of relativePos less directly used.
        // Plane quadPlane = new Plane(webCamDisplayQuad.transform.forward, webCamDisplayQuad.transform.position);
        // Ray rayFromRelativePoint = extractionCamera.ViewportPointToRay(relativePos); // Not ideal as it uses extractionCamera's view
        // Instead, calculate the point on the quad's plane:
        // Vector3 localPosOnQuad = new Vector3(
        //     Mathf.Lerp(-quadSize.x / 2, quadSize.x / 2, relativePos.x),
        //     Mathf.Lerp(-quadSize.y / 2, quadSize.y / 2, relativePos.y),
        //     0 // Assuming quad is flat on its local XY plane
        // );
        // return webCamDisplayQuad.transform.TransformPoint(localPosOnQuad);


        return new Vector3(worldX, worldY, worldZ);
    }

    void UpdateOrthographicSize()
    {
        if (quadRenderer == null)
        {
            Debug.LogWarning("[ExtractionCameraController] quadRenderer is null, cannot update orthographic size.");
            return;
        }

        float quadHeight = quadSize.y; // This assumes quadSize was correctly set in Start
        float extractionHeight = quadHeight * extractionAreaSize.y; // extractionAreaSize is 0.2f

        extractionCamera.orthographicSize = extractionHeight * 0.5f;

        float aspectRatio = extractionAreaSize.x / extractionAreaSize.y;
        extractionCamera.aspect = aspectRatio;

        Debug.Log($"[ExtractionCameraController] Ortho Size: {extractionCamera.orthographicSize}, Aspect: {extractionCamera.aspect}");
    }

    void ValidateSetup()
    {
        bool isValid = true;

        if (extractionCamera == null)
        {
            Debug.LogError("[ExtractionCameraController] Extraction camera is not assigned!");
            isValid = false;
        }

        if (webCamDisplayQuad == null)
        {
            Debug.LogError("[ExtractionCameraController] WebCamDisplayQuad is not assigned!");
            isValid = false;
        }

        if (handPositionCalculator == null)
        {
            Debug.LogError("[ExtractionCameraController] HandRelativePositionCalculator not found!");
            isValid = false;
        }

        if (!isValid)
        {
            enabled = false;
        }
    }

    public Texture2D ExtractPixels()
    {
        if (extractionCamera == null || _extractionCameraRenderTexture == null)
        {
            Debug.LogWarning("[ExtractionCameraController] Extraction camera or render texture is null, cannot extract pixels.");
            return null;
        }

        RenderTexture currentRT = RenderTexture.active;
        extractionCamera.Render(); // Force the camera to render its view to the targetTexture

        RenderTexture.active = _extractionCameraRenderTexture;
        Texture2D result = new Texture2D(TEXTURE_DIMENSIONS, TEXTURE_DIMENSIONS, TextureFormat.RGB24, false);
        result.ReadPixels(new Rect(0, 0, TEXTURE_DIMENSIONS, TEXTURE_DIMENSIONS), 0, 0);
        result.Apply();

        RenderTexture.active = currentRT; // Restore previous active RT

        return result;
    }
}