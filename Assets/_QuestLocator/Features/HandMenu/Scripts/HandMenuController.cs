using UnityEngine;
using UnityEngine.XR.Hands;
using System.Collections.Generic;

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

    private Vector3 _menuLocalEulerRotationOffset = new Vector3(-60, 0, 180); // Beispiel: 90 Grad Y-Rotation, um es seitlich zu drehen


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

        // XRHandJoint littleTipJoint = currentHand.GetJoint(XRHandJointID.LittleTip);

        // if (littleTipJoint.TryGetPose(out Pose littleTipPose))
        // {
        //     Vector3 targetPosition = littleTipPose.position;

        //     if (_isLeftHandGesture)
        //     {
        //         targetPosition = targetPosition + Vector3.right * _menuOffsetSide;
        //     }
        //     else
        //     {
        //         targetPosition = targetPosition - Vector3.right * _menuOffsetSide;
        //     }

        //     targetPosition = targetPosition - Vector3.back * _menuOffsetForward;
        //     targetPosition = targetPosition - Vector3.up * _menuOffsetUp;

        //     // Quaternion targetRotation = Quaternion.LookRotation(targetPosition - _mainCamera.transform.position, Vector3.up);
        //     Quaternion targetRotation = littleTipPose.rotation;
        //     targetRotation[0] = 0.71f;
        //     targetRotation[3] = 0.71f;

        //     _menuUI.transform.position = targetPosition;
        //     _menuUI.transform.rotation = targetRotation;
        // }
        // Verwende den LittleTipJoint für die Position
        XRHandJoint littleTipJoint = currentHand.GetJoint(XRHandJointID.LittleTip);
        // Für eine stabilere Rotation, nutze das Handgelenk oder den Metacarpal Joint des kleinen Fingers
        XRHandJoint littleMetacarpalJoint = currentHand.GetJoint(XRHandJointID.LittleMetacarpal); // Basis des kleinen Fingers
        XRHandJoint wristJoint = currentHand.GetJoint(XRHandJointID.Wrist); // Handgelenk

        if (littleTipJoint.TryGetPose(out Pose littleTipPose) &&
            littleMetacarpalJoint.TryGetPose(out Pose littleMetacarpalPose) && // Holen der Pose
            wristJoint.TryGetPose(out Pose wristPose)) // Holen der Pose
        {
            Vector3 targetPosition = littleTipPose.position;

            // Offset nach oben (relativ zur Welt-Up für stabile Höhe, oder wristPose.up für Hand-relative Up)
            // targetPosition += wristPose.up * _menuOffsetUp;
            // targetPosition += wristPose.forward * _menuOffsetForward;
            targetPosition += littleTipPose.up * _menuOffsetUp;

            // Offset nach vorne (relativ zur Vorwärtsrichtung des kleinen Fingers)
            targetPosition += littleTipPose.forward * _menuOffsetForward;

            // Offset zur Seite (außen von der Hand)
            if (_isLeftHandGesture)
            {
                // Linke Hand: Menü soll rechts vom Finger sein
                targetPosition += littleMetacarpalPose.right * _menuOffsetSide; // Verwende wristPose.right oder littleMetacarpalPose.right für seitlichen Offset
            }
            else // Right Hand
            {
                // Rechte Hand: Menü soll links vom Finger sein
                targetPosition += littleMetacarpalPose.right * -_menuOffsetSide; // Verwende wristPose.right oder littleMetacarpalPose.right für seitlichen Offset
            }

            // --- NEUE ROTATIONSLOGIK ---
            // Nimm die Rotation des LittleMetacarpal (Basis des kleinen Fingers) oder des Handgelenks für die Grundausrichtung
            // Die LittleMetacarpalPose ist oft besser, da sie die generelle Ausrichtung des Fingers/der Handfläche besser widerspiegelt
            Quaternion baseRotation = littleMetacarpalPose.rotation;

            // Wende den lokalen Euler-Rotations-Offset an
            // Dies dreht das Menü relativ zu seiner "flachen" Ausrichtung zur Hand
            Quaternion rotationOffset = Quaternion.Euler(_menuLocalEulerRotationOffset);

            // Kombiniere die Basis-Rotation mit dem Offset
            // baseRotation * rotationOffset -> wendet den Offset relativ zur Handrotation an
            Quaternion targetRotation = baseRotation * rotationOffset;

            _menuUI.transform.position = targetPosition;
            _menuUI.transform.rotation = targetRotation;
        }
        else
        {
            Debug.LogWarning($"[HandMenuController] IndexTip pose could not be retrieved for a hand.");
        }
    }

    // private void UpdateMenuPositionAndRotation()
    // {
    //     if (_handSubsystem == null || _menuUI == null) return;

    //     XRHand currentHand;
    //     bool handFound = false;

    //     if (_isLeftHandGesture)
    //     {
    //         currentHand = _handSubsystem.leftHand;
    //         handFound = true;
    //     }
    //     else
    //     {
    //         currentHand = _handSubsystem.rightHand;
    //         handFound = true;
    //     }

    //     if (!handFound)
    //     {
    //         return;
    //     }

    //     XRHandJoint wristJoint = currentHand.GetJoint(XRHandJointID.Wrist);

    //     if (wristJoint.TryGetPose(out Pose wristPose))
    //     {
    //         Vector3 targetPosition;

    //         if (_isLeftHandGesture)
    //         {
    //             targetPosition = wristPose.position
    //                 + Vector3.up * _handOffsetUp
    //                 + wristPose.forward * _handOffsetForward
    //                 + wristPose.right * -_handOffsetSide;
    //         }
    //         else
    //         {
    //             targetPosition = wristPose.position
    //                 + Vector3.up * _handOffsetUp
    //                 + wristPose.forward * _handOffsetForward
    //                 + wristPose.right * _handOffsetSide;
    //         }

    //         Quaternion targetRotation = Quaternion.LookRotation(targetPosition - _mainCamera.transform.position, Vector3.up);

    //         _menuUI.transform.position = targetPosition;
    //         _menuUI.transform.rotation = targetRotation;
    //     }
    // }

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
