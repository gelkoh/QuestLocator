using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class ScanFrameOutline : MonoBehaviour
{
    public float width = 0.4f;
    public float height = 0.2f;
    public float lineWidth = 0.01f;

    void Start()
    {
        LineRenderer lr = GetComponent<LineRenderer>();
        lr.positionCount = 5;
        lr.loop = true;
        lr.widthMultiplier = lineWidth;
        lr.useWorldSpace = false;
        lr.material = new Material(Shader.Find("Sprites/Default"));
        lr.startColor = lr.endColor = Color.green;

        Vector3[] corners = new Vector3[5]
        {
            new Vector3(-width/2,  height/2, 0),
            new Vector3( width/2,  height/2, 0),
            new Vector3( width/2, -height/2, 0),
            new Vector3(-width/2, -height/2, 0),
            new Vector3(-width/2,  height/2, 0) // zurück zum Anfang
        };

        lr.SetPositions(corners);
    }
}