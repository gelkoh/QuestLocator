using UnityEngine;

public class HandMenuController : MonoBehaviour
{
    public GameObject menuUI;
    public float distanceFromFace = 0.5f;

    public bool isMenuVisible = false;

    void Start()
    {
        if (menuUI != null)
        {
            menuUI.SetActive(false);
        }
    }

    public void ToggleMenu()
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

    public void ShowMenu()
    {
        if (menuUI == null) return;

        Transform cam = Camera.main.transform;
        Vector3 menuPosition = cam.position + cam.forward * distanceFromFace;

        menuUI.transform.position = menuPosition;
        menuUI.transform.rotation = Quaternion.LookRotation(menuUI.transform.position - cam.position, Vector3.up);
        menuUI.SetActive(true);
    }

    public void HideMenu()
    {
        if (menuUI == null) return;

        menuUI.SetActive(false);
    }
}
