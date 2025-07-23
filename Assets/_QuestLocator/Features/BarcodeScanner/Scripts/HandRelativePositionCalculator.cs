using System;
using TMPro;
using UnityEngine;

public class HandRelativePositionCalculator : MonoBehaviour
    {
    [Header("Camera References")]
    public Camera mainCamera;

    public GameObject webCamDisplayQuad;


    [Header("Debug")]
    public bool showDebugInfo = true;

    [SerializeField] private RectTransform scanFrameRect;
    [SerializeField] private TextMeshPro _handRelativePositionDisplay;

    void Start()
    {
        if (mainCamera == null)
        {
            Debug.LogError("[HandRelativePositionCalculator] Main camera not found!");
            enabled = false;
        }
    }

    // void Update()
    // {
    // if (showDebugInfo && scanFrameRect != null)
    // {
    // Vector3 relativePos = GetHandRelativePosition();
    // if (Time.frameCount % 30 == 0)
    // {
    // _handRelativePositionDisplay.SetText($"({relativePos.x:F3}, {relativePos.y:F3}, {relativePos.z:F3})");
    // }
    // }
    // }

    public Vector2 GetHandScreenPosition()
    {
        if (scanFrameRect == null || mainCamera == null)
        {
            return Vector2.zero;
        }

        Vector3 worldPosition = scanFrameRect.position;
        Vector3 screenPoint = mainCamera.WorldToScreenPoint(worldPosition);

        return new Vector2(screenPoint.x, screenPoint.y);
    }

    public Vector3 GetHandRelativePosition()
    {
        if (scanFrameRect == null || mainCamera == null)
        {
            Debug.LogWarning("[HandRelativePositionCalculator] ScanFrameRect or MainCamera is null");
            return Vector3.zero;
        }

        Vector3 worldPosition = scanFrameRect.position;
        Vector3 viewportPoint = mainCamera.WorldToViewportPoint(worldPosition);
        Vector3 relativePosition = new Vector3(viewportPoint.x, viewportPoint.y, viewportPoint.z);

        relativePosition.x = Mathf.Clamp01(relativePosition.x);
        relativePosition.y = Mathf.Clamp01(relativePosition.y);
        relativePosition.z = Mathf.Clamp01(relativePosition.z);

        if (showDebugInfo)
        {
            Debug.Log($"[HandRelativePosition] World: {worldPosition}, Viewport: {viewportPoint}, Relative: {relativePosition}");
        }

        return relativePosition;
    }
}