using UnityEngine;

public class HandMenuController : MonoBehaviour
{
    public GameObject menuUI;
    public OVRHand hand;
    public float distanceFromFace = 0.1f;

    public bool isMenuVisible = false;
    public bool wasPinchingLastFrame = false;

    void Start()
    {
        menuUI.SetActive(false);
    }

    void Update()
    {
        bool isPinching = hand.GetFingerIsPinching(OVRHand.HandFinger.Index);

        // nur reagieren, wenn Pinch von nicht gedrückt zu gedrückt wechselt
        if (isPinching && !wasPinchingLastFrame)
        {
            isMenuVisible = !isMenuVisible;

            if (isMenuVisible)
            {
                ShowMenu();
            }
            else
            {
                HideMenu();
            }
        }

        wasPinchingLastFrame = isPinching;

        // Menu bleibt immer vor deinem Gesicht
        if (isMenuVisible)
        {
            UpdateMenuPosition();
        }
    }

    public void ShowMenu()
    {
        UpdateMenuPosition();
        menuUI.SetActive(true);
    }

    public void HideMenu()
    {
        menuUI.SetActive(false);
    }

    public void UpdateMenuPosition()
    {
        Transform cam = Camera.main.transform;

        Vector3 forward = cam.forward;
        Vector3 menuPosition = cam.position + forward * distanceFromFace;

        menuUI.transform.position = menuPosition;

        // das Menü soll zur Kamera schauen
        menuUI.transform.rotation = Quaternion.LookRotation(menuUI.transform.position - cam.position, Vector3.up);
    }
}
