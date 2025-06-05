using UnityEngine;
using UnityEngine.UI;

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

        //get box collider
        boxCollider = GetComponent<BoxCollider>();
        //boxCollider.center += new Vector3(0.5f, 0f, 0f); // Shifts collider within the object
        //boxCollider.size = new Vector3(2f, 1f, 1f); // Resize collider


        selfRect = GetComponent<RectTransform>();

        // Create a UI Image as a "cube"
        GameObject cube = new GameObject("UICube");
        cube.transform.SetParent(transform, false);  // Maintain local position

        Image cubeImage = cube.AddComponent<Image>();
        cubeImage.color = new Color32(255, 0, 0, 100); ; // So it's visible


        cubeRect = cube.GetComponent<RectTransform>();
        cubeRect.sizeDelta = new Vector2(50f, 50f); // Small square
        cubeRect.anchoredPosition = Vector2.zero;   // Place at (0,0) of parent




        GameObject cube2 = new GameObject("UICube");
        cube2.transform.SetParent(transform, false);  // Maintain local position


        Image cube2Image = cube2.AddComponent<Image>();
        cube2Image.color = new Color32(255, 0, 0, 100); // So it's visible


        cube2Rect = cube2.GetComponent<RectTransform>();
        cube2Rect.sizeDelta = new Vector2(50f, 50f); // Small square
        cube2Rect.anchoredPosition = Vector2.zero;   // Place at (0,0) of parent 

        //UpdateCollider();
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

    /* public void UpdateCollider()
    {
        if (rectTransform == null || cubeRect == null) return;

        Canvas.ForceUpdateCanvases();
        Vector2 size = rectTransform.rect.size;
        Debug.Log("Corrected Size: " + size.x + ", " + size.y);

        // Optional: Move the "cube" to match bottom-left of backplate
        cubeRect.anchoredPosition = new Vector2(-size.x/2, size.y/2);
        cube2Rect.anchoredPosition = new Vector2(-size.x,size.y);

        boxCollider.center += new Vector3(-size.x/2, size.y/2); // Shifts collider within the object
        boxCollider.size = new Vector3(size.x, size.y, 0f); // Resize collider
    } */
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
