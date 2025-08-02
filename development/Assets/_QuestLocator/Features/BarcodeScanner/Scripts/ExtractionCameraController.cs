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
        Vector3 handRelativePos = handPositionCalculator.GetHandRelativePosition();

        Vector3 quadWorldTargetPos = ConvertRelativeToQuadWorldPosition(handRelativePos);

        PositionExtractionCamera(quadWorldTargetPos);

        UpdateOrthographicSize();
    }


    void PositionExtractionCamera(Vector3 targetWorldPos)
    {
        Vector3 quadNormal = webCamDisplayQuad.transform.forward;

        float cameraDistance = 0.5f;

        Vector3 cameraPosition = targetWorldPos - (quadNormal * cameraDistance);
        extractionCamera.transform.position = cameraPosition;

        extractionCamera.transform.LookAt(targetWorldPos, Vector3.up);

        Debug.Log($"[ExtractionCameraController] Target World Pos: {targetWorldPos}");
        Debug.Log($"[ExtractionCameraController] Camera Pos: {extractionCamera.transform.position}, Rot: {extractionCamera.transform.rotation}");
    }


    Vector3 ConvertRelativeToQuadWorldPosition(Vector3 relativePos)
    {
        if (webCamDisplayQuad == null) return Vector3.zero;

        if (quadRenderer == null)
        {
            Debug.LogWarning("[ExtractionCameraController] quadRenderer is null in ConvertRelativeToQuadWorldPosition. Returning Vector3.zero.");
            return Vector3.zero;
        }

        Bounds quadBounds = quadRenderer.bounds;

        float worldX = Mathf.Lerp(quadBounds.min.x, quadBounds.max.x, relativePos.x);
        float worldY = Mathf.Lerp(quadBounds.min.y, quadBounds.max.y, relativePos.y);

        float worldZ = Mathf.Lerp(quadBounds.min.z, quadBounds.max.z, relativePos.z);

        return new Vector3(worldX, worldY, worldZ);
    }

    void UpdateOrthographicSize()
    {
        if (quadRenderer == null)
        {
            Debug.LogWarning("[ExtractionCameraController] quadRenderer is null, cannot update orthographic size.");
            return;
        }

        float quadHeight = quadSize.y;
        float extractionHeight = quadHeight * extractionAreaSize.y;

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
        extractionCamera.Render();

        RenderTexture.active = _extractionCameraRenderTexture;
        Texture2D result = new Texture2D(TEXTURE_DIMENSIONS, TEXTURE_DIMENSIONS, TextureFormat.RGB24, false);
        result.ReadPixels(new Rect(0, 0, TEXTURE_DIMENSIONS, TEXTURE_DIMENSIONS), 0, 0);
        result.Apply();

        RenderTexture.active = currentRT;

        return result;
    }
}