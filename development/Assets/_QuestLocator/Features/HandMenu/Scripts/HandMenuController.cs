using UnityEngine;
using UnityEngine.XR.Hands;
using System.Collections.Generic;
using Oculus.Interaction;

public class HandMenuController : MonoBehaviour
{
    [SerializeField] private GameObject _menuUI;

    private Camera _mainCamera;
    private XRHandSubsystem _handSubsystem;
    private float _menuOffsetUp = 0.07f;
    private float _menuOffsetForward = -0.05f;
    private float _menuOffsetSide = -0.135f;
    private bool _isMenuActive;
    private bool _isLeftHandGesture;

    private Vector3 _menuLocalEulerRotationOffset = new Vector3(-60, 0, 180);

    [SerializeField] private GameObject _settingsPanel;
    private PanelPositioner _settingsPanelPositioner;

    private UIThemeManagerLocal themeManager;

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
        GameObject parentE = transform.parent.gameObject;
        themeManager = parentE.transform.parent.GetComponent<UIThemeManagerLocal>();
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
        themeManager.ApplyCurrentTheme();
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
        if (_handSubsystem == null || _menuUI == null || _mainCamera == null) return;

        XRHand currentHand;
        bool handIsTracked = false;

        if (_isLeftHandGesture)
        {
            currentHand = _handSubsystem.leftHand;
            handIsTracked = true;
        }
        else
        {
            currentHand = _handSubsystem.rightHand;
            handIsTracked = true;
        }

        if (!handIsTracked)
        {
            return;
        }

        XRHandJoint littleTipJoint = currentHand.GetJoint(XRHandJointID.LittleTip);
        XRHandJoint littleMetacarpalJoint = currentHand.GetJoint(XRHandJointID.LittleMetacarpal);
        XRHandJoint wristJoint = currentHand.GetJoint(XRHandJointID.Wrist);

        if (littleTipJoint.TryGetPose(out Pose littleTipPose) &&
            littleMetacarpalJoint.TryGetPose(out Pose littleMetacarpalPose) &&
            wristJoint.TryGetPose(out Pose wristPose))
        {
            Vector3 targetPosition = littleTipPose.position;

            targetPosition += littleTipPose.up * _menuOffsetUp;

            targetPosition += littleTipPose.forward * _menuOffsetForward;

            if (_isLeftHandGesture)
            {
            }
            else
            {
                targetPosition += littleMetacarpalPose.right * -_menuOffsetSide;
            }

            Quaternion baseRotation = littleMetacarpalPose.rotation;

            Quaternion rotationOffset = Quaternion.Euler(_menuLocalEulerRotationOffset);

            Quaternion targetRotation = baseRotation * rotationOffset;

            _menuUI.transform.position = targetPosition;
            _menuUI.transform.rotation = targetRotation;
        }
        else
        {
            Debug.LogWarning($"[HandMenuController] IndexTip pose could not be retrieved for a hand.");
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