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

        float skewPower = 2f;

        relativePosition.x = Skew(relativePosition.x, skewPower);
        relativePosition.y = Skew(relativePosition.y, skewPower);

        if (showDebugInfo)
        {
            Debug.Log($"[HandRelativePosition] World: {worldPosition}, Viewport: {viewportPoint}, Relative: {relativePosition}");
        }

        return relativePosition;
    }

    private float Skew(float value, float power)
    {
        if (value < 0.5f)
            return 0.5f * Mathf.Pow(value * 2f, power);
        else
            return 1f - 0.5f * Mathf.Pow((1f - value) * 2f, power);
    }
}