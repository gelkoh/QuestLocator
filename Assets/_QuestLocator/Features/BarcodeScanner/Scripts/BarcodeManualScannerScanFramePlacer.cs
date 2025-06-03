using UnityEngine;
using UnityEngine.XR.Hands;
using System.Collections.Generic;
using static BarcodeScannerStatusManager;
using static BarcodeScannerEventManager;
using System;

public class BarcodeScannerHandGesture : MonoBehaviour
{
    private XRHandSubsystem _handSubsystem;
    private RectTransform _scanFrameRect;

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
        if (BarcodeScannerStatusManagerInstance.ActiveScannerType != BarcodeScannerType.MANUAL)
        {
            return;
        }

        if (_handSubsystem == null)
        {
            return;
        }

        XRHand currentHand;

        if (_handSubsystem.leftHand.isTracked)
        {
            currentHand = _handSubsystem.leftHand;
        }
        // else if (handSubsystem.rightHand.isTracked)
        // {
        //     currentHand = handSubsystem.rightHand;
        // }
        else
        {
            return;
        }

        var thumbMetacarpalJoint = currentHand.GetJoint(XRHandJointID.ThumbMetacarpal);
        var thumbTipJoint = currentHand.GetJoint(XRHandJointID.ThumbTip);
        var thumbProximalJoint = currentHand.GetJoint(XRHandJointID.ThumbProximal);
        var indexTipJoint = currentHand.GetJoint(XRHandJointID.IndexTip);
        var indexIntermediateJoint = currentHand.GetJoint(XRHandJointID.IndexIntermediate);
        var indexMetacarpalJoint = currentHand.GetJoint(XRHandJointID.IndexMetacarpal);
        var wristJoint = currentHand.GetJoint(XRHandJointID.Wrist);

        if (!thumbMetacarpalJoint.TryGetPose(out Pose thumbMetacarpalPose) ||
            !thumbTipJoint.TryGetPose(out Pose thumbTipPose) ||
            !thumbProximalJoint.TryGetPose(out Pose thumbProximalPose) ||
            !indexTipJoint.TryGetPose(out Pose indexTipPose) ||
            !indexIntermediateJoint.TryGetPose(out Pose indexIntermediatePose) ||
            !indexMetacarpalJoint.TryGetPose(out Pose indexMetacarpalPose) ||
            !wristJoint.TryGetPose(out Pose wristPose))
        {
            return;
        }

        Vector3 thumbMetacarpalPos = thumbMetacarpalPose.position;
        Vector3 thumbTipPos = thumbTipPose.position;
        Vector3 thumbProximalPos = thumbProximalPose.position;
        Vector3 indexTipPos = indexTipPose.position;
        Vector3 indexMetacarpalPos = indexMetacarpalPose.position;
        Vector3 indexIntermediatePos = indexIntermediatePose.position;

        float posX = thumbTipPos.x + _scanFrameRect.rect.width * _scanFrameRect.lossyScale.x / 2;
        float posY = indexIntermediatePos.y + _scanFrameRect.rect.height * _scanFrameRect.lossyScale.y / 2;
        float posZ = Mathf.Lerp(thumbTipPos.z, indexIntermediatePos.z, 0.5f);

        Vector3 baseFramePosition = new Vector3(posX, posY, posZ);
        _scanFrameRect.position = baseFramePosition;

        Vector3 handForwardDirection = wristPose.forward;

        Vector3 thumbNormal = (thumbTipPos - thumbMetacarpalPos).normalized;
        Vector3 indexNormal = (indexTipPos - indexMetacarpalPos).normalized;

        Vector3 potentialScanFrameForward = Vector3.Cross(thumbNormal, indexNormal).normalized;

        if (Vector3.Dot(potentialScanFrameForward, handForwardDirection) < 0)
        {
            potentialScanFrameForward = -potentialScanFrameForward;
        }

        Vector3 scanFrameForward = potentialScanFrameForward;

        Vector3 scanFrameUp = Vector3.Cross(scanFrameForward, indexNormal).normalized;

        Quaternion baseRotation = Quaternion.LookRotation(scanFrameForward, scanFrameUp);

        Quaternion rollCorrection = Quaternion.Euler(0, 0, -10f);
        // Quaternion rollCorrection = Quaternion.Euler(0, 0, 0);

        _scanFrameRect.rotation = baseRotation * rollCorrection;
    }
}