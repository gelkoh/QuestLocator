using UnityEngine;
using ZXing;

public class BarcodeScanVisualizer : MonoBehaviour
{
    private LineRenderer lineRenderer;
    private bool loggingEnabled = false;

    void Awake()
    {
        // Grab the line renderer that is on this object
        lineRenderer = GetComponent<LineRenderer>();

        if (lineRenderer == null)
        {
            if (loggingEnabled)
                Debug.LogError("❌ Kein LineRenderer gefunden!");
        }
    }

    public void DrawBoundingBox(ResultPoint[] points)
    {
        loggingEnabled = OVRInput.Get(OVRInput.Button.One);

        if (points == null)
        {
            if (loggingEnabled)
                Debug.LogWarning("⚠️ points is null");

            return;
        }

        if (points.Length < 2)
        {
            if (loggingEnabled)
                Debug.LogWarning("⚠️ not enough points");

            return;
        }

        var pointA = new Vector2(points[0].X, points[0].Y);
        var pointB = new Vector2(points[1].X, points[1].Y);

        var width = (pointB - pointA).magnitude;
        var height = 50f;

        var dir = (pointB - pointA).normalized;

        var perp = new Vector2(-dir.y, dir.x);

        var topLeft = pointA;
        var topRight = pointB;
        var bottomRight = pointB + perp * height;
        var bottomLeft = pointA + perp * height;

        var rectangleCorners = new Vector2[] {
            topLeft, topRight, bottomRight, bottomLeft
        };

        lineRenderer.positionCount = 5;
        for (var i = 0; i < rectangleCorners.Length; i++)
        {
            var p = rectangleCorners[i];
            lineRenderer.SetPosition(i, new Vector3(p.x / width, p.y / height, 0.5f));
        }

        lineRenderer.SetPosition(4, new Vector3(topLeft.x / width, topLeft.y / height, 0.5f));
    }
}
