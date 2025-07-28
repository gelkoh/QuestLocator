using UnityEngine;

public class ColliderResizer : MonoBehaviour
{
    private RectTransform rectTransform;
    private Transform backplate;
    private RectTransform cubeRect;
    private RectTransform cube2Rect;
    private BoxCollider boxCollider;
    private RectTransform selfRect;

    void Start()
    {
        backplate = transform.Find("UIBackplate");

        if (backplate == null)
        {
            Debug.LogError("UIBackplate not found!");
            return;
        }

        rectTransform = backplate.GetComponent<RectTransform>();

        if (rectTransform == null)
        {
            Debug.LogError("RectTransform not found on UIBackplate!");
            return;
        }

        boxCollider = GetComponent<BoxCollider>();

        selfRect = GetComponent<RectTransform>();
    }

    int a = 0;

    void Update()
    {

        if (a < 50)
        {
            a++;
        }
        else if (a == 50)
        {
            UpdateCollider();
            UpdateCanvas();
            a++;
        }
    }

    public void UpdateCollider()
    {
        if (rectTransform == null || cubeRect == null) return;

        Canvas.ForceUpdateCanvases();

        Vector3[] worldCorners = new Vector3[4];
        rectTransform.GetWorldCorners(worldCorners); // Get the world-space corners of the UI element

        // Calculate size in world space
        Vector3 bottomLeft = worldCorners[0];
        Vector3 topRight = worldCorners[2];

        Vector3 worldSize = topRight - bottomLeft;

        // Convert world size to local space of the parent object
        Vector3 localSize = transform.InverseTransformVector(worldSize);
        Vector3 localCenter = transform.InverseTransformPoint((bottomLeft + topRight) / 2f) - transform.localPosition;

        boxCollider.size = new Vector3(localSize.x, localSize.y, 0.01f); // Use thin Z size
        boxCollider.center = localCenter;

        Debug.Log("Collider size: " + boxCollider.size + ", center: " + boxCollider.center);
    }

    public void UpdateCanvas()
    {
        Vector2 childSize = rectTransform.rect.size;
        selfRect.sizeDelta = new Vector2(childSize.x, childSize.y);
    }
}