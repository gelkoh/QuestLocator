using UnityEngine;
using UnityEngine.XR.Hands;
using System.Collections.Generic;
using UnityEngine.XR;

public class HandMenuController : MonoBehaviour
{
    public GameObject menuUI;
    public float handOffsetForward = 0.15f;
    public float handOffsetUp = 0.5f;

    public float distanceFromFace = 0.5f;

    public bool isMenuVisible = false;

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
        if (isMenuVisible)
        {
            UpdateMenuPositionAndRotation();
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

        UpdateMenuPositionAndRotation();
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
            Vector3 targetPosition = wristPose.position;
            targetPosition += wristPose.up * handOffsetUp;
            targetPosition += wristPose.forward * handOffsetForward;

            Quaternion targetRotation = Quaternion.LookRotation(targetPosition - _mainCamera.transform.position, Vector3.up);

            menuUI.transform.position = targetPosition;
            menuUI.transform.rotation = targetRotation;
        }
    }

    public void OnLeftHandMenuGesturePerformed()
    {
        _isLeftHandGesture = true;
        ToggleMenu();
    }

    public void OnLeftHandMenuGestureEnded()
    {
        HideMenu();
    }

    public void OnRightHandMenuGesturePerformed()
    {
        _isLeftHandGesture = false;
        ToggleMenu();
    }

    public void OnRightHandMenuGestureEnded()
    {
        HideMenu();
    }
}
