using UnityEngine;
using UnityEngine.XR.Hands;
using System.Collections.Generic;

public class HandMenuController : MonoBehaviour
{
    [SerializeField] private GameObject _menuUI;

    private Camera _mainCamera;
    private XRHandSubsystem _handSubsystem;
    private float _handOffsetUp = 0.085f;
    private float _handOffsetForward = 0.1f;
    private float _handOffsetSide = 0.24f;
    private bool _isMenuActive;
    private bool _isLeftHandGesture;

    [SerializeField] private GameObject _settingsPanel;
    private PanelPositioner _settingsPanelPositioner;

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

        if (_menuUI != null)
        {
            _menuUI.SetActive(false);
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
        if (_menuUI == null) return;

        UpdateMenuPositionAndRotation();
        _isMenuActive = true;
        _menuUI.SetActive(true);
    }

    public void HideMenu()
    {
        _menuUI.SetActive(false);
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
        if (_handSubsystem == null || _menuUI == null) return;

        XRHand currentHand;
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
            Vector3 targetPosition;

            if (_isLeftHandGesture)
            {
                targetPosition = wristPose.position
                    + Vector3.up * _handOffsetUp
                    + wristPose.forward * _handOffsetForward
                    + wristPose.right * -_handOffsetSide;
            }
            else
            {
                targetPosition = wristPose.position
                    + Vector3.up * _handOffsetUp
                    + wristPose.forward * _handOffsetForward
                    + wristPose.right * _handOffsetSide;
            }

            Quaternion targetRotation = Quaternion.LookRotation(targetPosition - _mainCamera.transform.position, Vector3.up);

            _menuUI.transform.position = targetPosition;
            _menuUI.transform.rotation = targetRotation;
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
