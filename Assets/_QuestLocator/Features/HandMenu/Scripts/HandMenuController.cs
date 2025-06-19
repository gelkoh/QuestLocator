using UnityEngine;
using UnityEngine.XR.Hands;
using System.Collections.Generic;
using UnityEngine.XR;

public class HandMenuController : MonoBehaviour
{
    public GameObject menuUI;
    private float handOffsetForward = 0.2f;
    private float handOffsetUp = 0.28f;
    private float handOffsetSide = 0.2f;

    public float distanceFromFace = 0.5f;

    // public bool isMenuVisible = false;
    private bool _isMenuActive;

    [SerializeField] private GameObject _settingsPanel;
    private PanelPositioner _settingsPanelPositioner;

    Camera _mainCamera;
    XRHandSubsystem _handSubsystem;

    private bool _isLeftHandGesture;

    void Awake()
    {
        var handSubsystems = new List<XRHandSubsystem>();
        SubsystemManager.GetSubsystems(handSubsystems);

        if (handSubsystems.Count > 0)
        {
            _handSubsystem = handSubsystems[0];
        }
        else
        {
            Debug.LogError("HandMenuController: No XRHandSubsystem found. Hand tracking not possible for hand menu.");
            enabled = false;
        }

        _settingsPanelPositioner = _settingsPanel.GetComponentInChildren<Canvas>().GetComponent<PanelPositioner>();
    }

    void OnEnable()
    {
        _settingsPanel.SetActive(false);
    }

    void Start()
    {
        _mainCamera = Camera.main;

        if (_mainCamera == null)
        {
            Debug.LogError("HandMenuController: Couldn't find main camera.");
            enabled = false;
        }

        if (menuUI != null)
        {
            menuUI.SetActive(false);
        }
    }

    void Update()
    {
        if (_isMenuActive)
        {
            UpdateMenuPositionAndRotation();
        }
    }

    public void ShowMenu()
    {
        if (menuUI == null) return;

        UpdateMenuPositionAndRotation();
        _isMenuActive = true;
        menuUI.SetActive(true);
    }

    public void HideMenu()
    {
        menuUI.SetActive(false);
    }

    public void ToggleSettingsVisible()
    {
        if (_settingsPanel.activeSelf)
        {
            _settingsPanel.SetActive(false);
        }
        else
        {
            _settingsPanel.SetActive(true);
            _settingsPanelPositioner.PositionPanelInFrontOfCamera();
        }
    }

    private void UpdateMenuPositionAndRotation()
    {
        if (_handSubsystem == null || menuUI == null) return;

        XRHand currentHand = new XRHand();
        bool handFound = false;

        if (_isLeftHandGesture)
        {
            currentHand = _handSubsystem.leftHand;
            handFound = true;
        }
        else
        {
            currentHand = _handSubsystem.rightHand;
            handFound = true;
        }

        if (!handFound)
        {
            return;
        }

        XRHandJoint wristJoint = currentHand.GetJoint(XRHandJointID.Wrist);

        if (wristJoint.TryGetPose(out Pose wristPose))
    {
            // Welt‑Y statt local‑up
            Vector3 targetPosition = wristPose.position 
                                + Vector3.up * handOffsetUp 
                                + wristPose.forward * handOffsetForward;
            // Vector3 targetPosition;
            // if (_isLeftHandGesture)
            // {
            //     targetPosition = wristPose.position
            //            + Vector3.up * handOffsetUp
            //            + wristPose.forward * handOffsetForward
            //            + Vector3.right * -handOffsetSide; // hier der seitliche Offset
            // }
            // else
            // {
            //     targetPosition = wristPose.position
            //            + Vector3.up * handOffsetUp
            //            + wristPose.forward * handOffsetForward
            //            + Vector3.right * -handOffsetSide; // hier der seitliche Offset
            // }
            

            Quaternion targetRotation = Quaternion.LookRotation(targetPosition - _mainCamera.transform.position, Vector3.up);

            menuUI.transform.position = targetPosition;
            menuUI.transform.rotation = targetRotation;
        }
    }

    public void OnLeftHandMenuGesturePerformed()
    {
        if (!_isMenuActive)
        {
            _isLeftHandGesture = true;
            ShowMenu();
        }
    }

    public void OnLeftHandMenuGestureEnded()
    {
        if (_isLeftHandGesture)
        {
            _isMenuActive = false;
            HideMenu();
        }
    }

    public void OnRightHandMenuGesturePerformed()
    {
        if (!_isMenuActive)
        {
            _isLeftHandGesture = false;
            ShowMenu();
        }
    }

    public void OnRightHandMenuGestureEnded()
    {
        if (!_isLeftHandGesture)
        {
            _isMenuActive = false;
            HideMenu();
        }
    }
}
