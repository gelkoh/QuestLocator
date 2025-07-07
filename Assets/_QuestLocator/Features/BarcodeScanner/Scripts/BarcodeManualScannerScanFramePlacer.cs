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

    private float _lateralOffset = 0.012f;
    private float _verticalOffset = 0f;
    private float _depthOffset = 0.06f;

    private float _baseRollAngle = 10f;
    private Vector3 _finalLocalRotationOffset = new Vector3(0, 0, 90); 

    void Awake()
    {
        _scanFrameRect = GetComponent<RectTransform>();

        if (_scanFrameRect == null)
        {
            Debug.LogError("BarcodeManualScannerScanFramePlacer: RectTransform not found on this GameObject.");
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
            Debug.LogError("BarcodeScannBarcodeManualScannerScanFramePlacererHandGesture: No XRHandSubsystem found. Hand tracking not possible.");
            enabled = false;
        }
    }

    void Update()
    {
        if (BarcodeScannerStatusManagerInstance == null)
        {
            Debug.LogWarning("[BarcodeScannerHandGesture] BarcodeScannerStatusManagerInstance is null.");
            return;
        }
        if (BarcodeScannerStatusManagerInstance.ActiveScannerType != BarcodeScannerType.MANUAL)
        {
            return;
        }

        if (_handSubsystem == null)
        {
            Debug.LogWarning("[BarcodeScannerHandGesture] _handSubsystem is null in Update.");
            return;
        }

        if (BarcodeScannerGestureControllerInstance == null)
        {
            Debug.LogError("[BarcodeScannerHandGesture] BarcodeScannerGestureControllerInstance is null! Cannot determine active hand.");
            return;
        }

        XRHand currentHand;
        string activeHand;

        bool isLeftHandTracked = _handSubsystem.leftHand.isTracked;
        bool isRightHandTracked = _handSubsystem.rightHand.isTracked;

        if (isLeftHandTracked && BarcodeScannerGestureControllerInstance.isLeftHandManualScanner == true)
        {
            currentHand = _handSubsystem.leftHand;
            activeHand = "left";
        }
        else if (isRightHandTracked && BarcodeScannerGestureControllerInstance.isLeftHandManualScanner == false)
        {
            currentHand = _handSubsystem.rightHand;
            activeHand = "right";
        }
        else
        {
            return;
        }

        Debug.Log($"[BarcodeScannerHandGesture] Active Hand: {activeHand}. Is Tracked: {currentHand.isTracked}");

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
            Debug.LogWarning($"[BarcodeScannerHandGesture] Could not retrieve all necessary joint poses for {activeHand} hand. Returning.");
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

        Vector3 currentLocalOffset;

        if (activeHand == "left")
        {
            currentLocalOffset = new Vector3(_lateralOffset, _verticalOffset, _depthOffset);
        }
        else
        {
            currentLocalOffset = new Vector3(_lateralOffset / 2, _verticalOffset, _depthOffset * 2);
        }

        if (activeHand == "right")
        {
            currentLocalOffset.x *= -1;
        }

        Vector3 finalFramePosition = gestureCenter + (handBaseRotationForPosition * currentLocalOffset);
        _scanFrameRect.position = finalFramePosition;

        Vector3 thumbDirection = (thumbTipPos - thumbMetacarpalPos).normalized;
        Vector3 indexDirection = (indexTipPos - indexMetacarpalPos).normalized;

        Vector3 potentialScanFrameForward = Vector3.Cross(thumbDirection, indexDirection).normalized;

        Vector3 handForwardDirection = wristPose.forward;

        if (Vector3.Dot(potentialScanFrameForward, handForwardDirection) < 0)
        {
            potentialScanFrameForward = -potentialScanFrameForward;
            Debug.Log($"[BarcodeScannerHandGesture] Scan Frame Forward corrected (flipped): {potentialScanFrameForward}");
        }

        Vector3 scanFrameForward = potentialScanFrameForward;

        Vector3 projectedIndexUp = Vector3.ProjectOnPlane(indexDirection, scanFrameForward).normalized;

        Vector3 scanFrameUp;
        if (projectedIndexUp.sqrMagnitude < 0.001f)
        {
            Debug.LogWarning("[BarcodeScannerHandGesture] Projected Index Up is near zero. Using fallback.");
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

        if (currentHand == _handSubsystem.rightHand)
        {
            scanFrameUp = -scanFrameUp;
        }

        Quaternion baseRotation = Quaternion.LookRotation(scanFrameForward, scanFrameUp);

        Quaternion rollCorrection;

        if (activeHand == "left")
        {
            rollCorrection = Quaternion.Euler(0, 0, -_baseRollAngle);
            Debug.Log($"[BarcodeScannerHandGesture] Left Hand Roll Correction: {rollCorrection.eulerAngles}");
        }
        else
        {
            rollCorrection = Quaternion.Euler(0, 0, _baseRollAngle);
            Debug.Log($"[BarcodeScannerHandGesture] Right Hand Roll Correction: {rollCorrection.eulerAngles}");
        }

        Quaternion finalLocalCorrection = Quaternion.Euler(_finalLocalRotationOffset);

        _scanFrameRect.rotation = baseRotation * rollCorrection * finalLocalCorrection;
    }
}