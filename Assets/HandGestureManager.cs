using UnityEngine;
using System.Collections;

public class HandGestureManager : MonoBehaviour
{
    [Header("References")]
    public Transform leftHandRoot;
    public Transform rightHandRoot;
    public OVRHand   leftOvrHand;
    public OVRHand   rightOvrHand;
    public GameObject HandMenuCanvasPrefab;

    GameObject leftMenuInstance;
    bool       isScanning;
    bool       leftMenuVisible;

    void Start()
    {
        // 1. Hände ausblenden
        HideHandVisuals(leftHandRoot);
        HideHandVisuals(rightHandRoot);
    }

    void Update()
    {
        // 2. Rechts pinchen → Scan
        // if (IsRightPinching() && !isScanning)
        // {
        //     isScanning = true;
        //     StartCoroutine( BeginScanCoroutine(() => isScanning = false) );
        // }

        // 3. Links pinchen → Menü toggle
        bool pinched = IsLeftPinching();
        if (pinched && !leftMenuVisible)
            ShowLeftMenu();
        else if (!pinched && leftMenuVisible)
            HideLeftMenu();

        // 4. Automatisch ausblenden, wenn Hand wegdreht
        if (leftMenuVisible && !IsLeftFacingCamera())
            HideLeftMenu();
    }

    void HideHandVisuals(Transform handRoot)
    {
        foreach (var mr in handRoot.GetComponentsInChildren<MeshRenderer>())
            mr.enabled = false;
    }

    bool IsRightPinching() => rightOvrHand.GetFingerIsPinching(OVRHand.HandFinger.Index);
    bool IsLeftPinching()  => leftOvrHand.GetFingerIsPinching(OVRHand.HandFinger.Index);

    bool IsLeftFacingCamera()
    {
        Vector3 palmNormal = leftHandRoot.forward; // oder .up/.right je nach Rig
        Vector3 toCam      = (Camera.main.transform.position - leftHandRoot.position).normalized;
        return Vector3.Dot(palmNormal, toCam) > 0.5f;
    }

    void ShowLeftMenu()
    {
        leftMenuVisible = true;
        Vector3 spawnPos = leftHandRoot.position + leftHandRoot.forward * 0.5f;
        leftMenuInstance = Instantiate(HandMenuCanvasPrefab, spawnPos, Quaternion.LookRotation(leftHandRoot.forward));
        // Hier kannst du im Prefab Buttons hooken und Events setzen, z.B. auf Scan starten:
        //var btn = leftMenuInstance.GetComponentInChildren<StartScanButton>();
        //btn.OnClickStartScan();
        // btn.onClick.AddListener(() => {
        //     if (!isScanning)
        //     {
        //         isScanning = true;
        //         StartCoroutine( BeginScanCoroutine(() => isScanning = false) );
        //     }
        // });
    }

    void HideLeftMenu()
    {
        leftMenuVisible = false;
        if (leftMenuInstance != null) Destroy(leftMenuInstance);
    }

    // IEnumerator BeginScanCoroutine(Action onComplete)
    // {
    //     // Dein bestehender Scan‑Workflow:
    //     yield return ScanRoutine();
    //     onComplete();
    // }
}
