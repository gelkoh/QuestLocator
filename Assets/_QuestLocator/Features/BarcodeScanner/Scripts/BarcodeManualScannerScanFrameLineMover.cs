using UnityEngine;

enum Direction {
    UP,
    DOWN
}

public class BarcodeManualScannerScanFrameLineMover : MonoBehaviour
{
    [SerializeField] private float speed = 1.0f;
    private float topY = 0.35f;
    private float bottomY = -0.35f;

    private Vector3 startPosition;
    private Direction currentDirection = Direction.DOWN;
    private RectTransform _parentRectTransform;

    void Start()
    {
        startPosition = transform.localPosition;
        startPosition.y = topY;
        transform.localPosition = startPosition;
        _parentRectTransform = transform.parent.GetComponent<RectTransform>();
    }

    void Update()
    {
        Vector3 position = transform.localPosition;

        if (currentDirection == Direction.DOWN)
        {
            position.y -= speed * Time.deltaTime;

            if (position.y <= bottomY)
            {
                currentDirection = Direction.UP;
            }
        }
        else
        {
            position.y += speed * Time.deltaTime;

            if (position.y >= topY)
            {
                currentDirection = Direction.DOWN;
            }
        }

        transform.localPosition = position;
    }
}