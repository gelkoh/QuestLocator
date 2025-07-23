using UnityEngine;
using UnityEngine.XR.Hands;
using System.Collections.Generic;
using static BarcodeScannerStatusManager;
using static BarcodeScannerEventManager;
using static BarcodeScannerGestureController;

public class BarcodeScannerHandGesture : MonoBehaviour
{
    private XRHandSubsystem _handSubsystem;
    private RectTransform _scanFrameRect;

    private float _offsetX = 0.018f;
    private float _offsetY = 0f;
    private float _offsetZ = 0.1f;

    private float _baseRollAngle = 10f;
    private Vector3 _finalLocalRotationOffset = new Vector3(0, 0, 90);

    void Awake()
    {
        _scanFrameRect = GetComponent<RectTransform>();

        if (_scanFrameRect == null)
        {
            Debug.LogError("BarcodeScannerHandGesture: RectTransform not found on this GameObject.");
            enabled = false;
            return;
        }

        var handSubsystems = new List<XRHandSubsystem>();
        SubsystemManager.GetSubsystems(handSubsystems);

        if (handSubsystems.Count > 0)
        {
            _handSubsystem = handSubsystems[0];
        }
        else
        {
            Debug.LogError("BarcodeScannerHandGesture: No XRHandSubsystem found. Hand tracking not possible.");
            enabled = false;
        }
    }

    void Update()
    {
        if (BarcodeScannerStatusManagerInstance == null || BarcodeScannerStatusManagerInstance.ActiveScannerType != BarcodeScannerType.MANUAL)
        {
            return;
        }

        if (_handSubsystem == null)
        {
            Debug.LogWarning("[BarcodeScannerHandGesture] _handSubsystem is null in Update. Skipping hand tracking.");
            return;
        }

        if (BarcodeScannerGestureControllerInstance == null)
        {
            Debug.LogError("[BarcodeScannerHandGesture] BarcodeScannerGestureControllerInstance is null! Cannot determine active hand. Skipping.");
            return;
        }

        XRHand currentHand;
        bool isLeftHandActive = BarcodeScannerGestureControllerInstance.isLeftHandManualScanner;
        bool isHandTracked = false;

        if (isLeftHandActive && _handSubsystem.leftHand.isTracked)
        {
            currentHand = _handSubsystem.leftHand;
            isHandTracked = true;
        }
        else if (!isLeftHandActive && _handSubsystem.rightHand.isTracked)
        {
            currentHand = _handSubsystem.rightHand;
            isHandTracked = true;
        }
        else
        {
            Debug.Log("[BarcodeScannerHandGesture] No active or tracked hand for manual scanner.");
            return;
        }
        
        Pose thumbMetacarpalPose, thumbTipPose, thumbProximalPose, indexTipPose, indexIntermediatePose, indexMetacarpalPose, wristPose;

        bool success = true;
        success &= currentHand.GetJoint(XRHandJointID.ThumbMetacarpal).TryGetPose(out thumbMetacarpalPose);
        success &= currentHand.GetJoint(XRHandJointID.ThumbTip).TryGetPose(out thumbTipPose);
        success &= currentHand.GetJoint(XRHandJointID.ThumbProximal).TryGetPose(out thumbProximalPose);
        success &= currentHand.GetJoint(XRHandJointID.IndexTip).TryGetPose(out indexTipPose);
        success &= currentHand.GetJoint(XRHandJointID.IndexIntermediate).TryGetPose(out indexIntermediatePose);
        success &= currentHand.GetJoint(XRHandJointID.IndexMetacarpal).TryGetPose(out indexMetacarpalPose);
        success &= currentHand.GetJoint(XRHandJointID.Wrist).TryGetPose(out wristPose);

        if (!success)
        {
            Debug.LogWarning("[BarcodeScannerHandGesture] Could not retrieve all necessary joint poses for active hand. Skipping frame.");
            return;
        }

        Vector3 thumbMetacarpalPos = thumbMetacarpalPose.position;
        Vector3 thumbTipPos = thumbTipPose.position;
        Vector3 thumbProximalPos = thumbProximalPose.position;
        Vector3 indexTipPos = indexTipPose.position;
        Vector3 indexMetacarpalPos = indexMetacarpalPose.position;
        Vector3 indexIntermediatePos = indexIntermediatePose.position;

        if (_scanFrameRect == null)
        {
            Debug.LogError("[BarcodeScannerHandGesture] _scanFrameRect is null at Update! This should have been caught in Awake.");
            return;
        }

        Vector3 gestureCenter = Vector3.Lerp(thumbProximalPos, indexMetacarpalPos, 0.5f);

        Quaternion handBaseRotationForPosition = wristPose.rotation;

        Vector3 localOffset = new Vector3(_offsetX, _offsetY, _offsetZ);

        if (!isLeftHandActive)
        {
            localOffset.x *= -1;
        }

        Vector3 finalFramePosition = gestureCenter + (handBaseRotationForPosition * localOffset);
        _scanFrameRect.position = finalFramePosition;

        Vector3 thumbDirection = (thumbTipPos - thumbMetacarpalPos).normalized;
        Vector3 indexDirection = (indexTipPos - indexMetacarpalPos).normalized;
        Vector3 potentialScanFrameForward = Vector3.Cross(thumbDirection, indexDirection).normalized;

        Vector3 thumbIndexVector = (indexMetacarpalPos - thumbMetacarpalPos).normalized;
        Vector3 handPalmNormal = Vector3.Cross(thumbIndexVector, (wristPose.up)).normalized;
        
        Vector3 scanFrameForward = potentialScanFrameForward;

        if (Vector3.Dot(scanFrameForward, wristPose.forward) < 0)
        {
            scanFrameForward = -scanFrameForward;
            Debug.Log($"[BarcodeScannerHandGesture] Scan Frame Forward corrected (flipped) for dot product: {scanFrameForward}");
        }

        Vector3 projectedIndexUp = Vector3.ProjectOnPlane(indexDirection, scanFrameForward).normalized;

        Vector3 scanFrameUp;
        if (projectedIndexUp.sqrMagnitude < 0.001f)
        {
            Debug.LogWarning("[BarcodeScannerHandGesture] Projected Index Up is near zero. Using wrist up as fallback.");
            Vector3 projectedWristUp = Vector3.ProjectOnPlane(wristPose.up, scanFrameForward).normalized;
            if (projectedWristUp.sqrMagnitude < 0.001f)
            {
                scanFrameUp = Vector3.up; 
            }
            else
            {
                scanFrameUp = projectedWristUp;
            }
        }
        else
        {
            scanFrameUp = projectedIndexUp;
        }

        if (!isLeftHandActive)
        {
            scanFrameUp = -scanFrameUp;
            Debug.Log($"[BarcodeScannerHandGesture] Scan Frame Up inverted for Right Hand: {scanFrameUp}");
        }

        Quaternion baseRotation = Quaternion.LookRotation(scanFrameForward, scanFrameUp);

        Quaternion rollCorrection;
        if (isLeftHandActive)
        {
            rollCorrection = Quaternion.Euler(0, 0, -_baseRollAngle);
        }
        else
        {
            rollCorrection = Quaternion.Euler(0, 0, _baseRollAngle);
        }

        Quaternion finalLocalCorrection = Quaternion.Euler(_finalLocalRotationOffset);

        _scanFrameRect.rotation = baseRotation * rollCorrection * finalLocalCorrection;

        Debug.Log($"[BarcodeScannerHandGesture] Final Scan Frame Position: {_scanFrameRect.position}");
        Debug.Log($"[BarcodeScannerHandGesture] Final Scan Frame Rotation (Euler): {_scanFrameRect.rotation.eulerAngles}");
    }
}