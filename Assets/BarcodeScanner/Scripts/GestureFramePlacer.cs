using UnityEngine.XR.Hands;
using System.Collections.Generic;
using UnityEngine;

// Dieses Skript gehört auf das ROOT-GameObject deines manuellen Scan-Frame-Prefabs.
// Es kümmert sich nur um die Platzierung, Rotation und Skalierung dieses GameObjects.
public class GestureFramePlacer : MonoBehaviour
{
    private XRHandSubsystem handSubsystem;
    // testFrame und _scanFrameTestPrefab werden hier nicht mehr benötigt,
    // da dieses Skript direkt das GameObject manipuliert, an dem es hängt (this.gameObject).

    // Feste Größe für den ScanFrame
    [SerializeField] private float fixedFrameWidth = 0.05f; // Beispiel: 5 cm Breite
    private float originalPrefabAspectRatio = 1.0f; // Standardwert 1:1 (Quadrat), falls nicht ermittelbar
    private float originalPrefabDepth = 0.01f; // Standardtiefe, falls nicht ermittelbar oder gewünscht

    // Offset für die Position des ScanFrames relativ zur Mitte der Finger
    [SerializeField] private float verticalOffset = 0.05f; // Verschiebung nach oben (z.B. 5 cm)
    [SerializeField] private float horizontalOffset = 0.05f; // Verschiebung zur Seite (z.B. 5 cm nach rechts, relativ zur Kamera)


    void Awake()
    {
        // Hole das Hand-Subsystem direkt in Awake, um es früh verfügbar zu haben
        var handSubsystems = new List<XRHandSubsystem>();
        SubsystemManager.GetSubsystems(handSubsystems);

        if (handSubsystems.Count > 0)
        {
            handSubsystem = handSubsystems[0];
            Debug.Log("GestureFramePlacer: XRHandSubsystem gefunden!");
        }
        else
        {
            Debug.LogError("GestureFramePlacer: Kein XRHandSubsystem gefunden. Gestenverfolgung nicht möglich.");
            // Deaktiviere das Skript, wenn kein Subsystem vorhanden ist
            // enabled = false; // Nicht hier deaktivieren, da der Holder es steuert
        }

        // Ermittle das ursprüngliche Seitenverhältnis und die Tiefe des Prefabs, an dem dieses Skript hängt
        // (Annahme: Dieses Skript ist am Root des visuellen Frames)
        MeshFilter meshFilter = GetComponent<MeshFilter>();
        if (meshFilter != null && meshFilter.sharedMesh != null)
        {
            Vector3 localMeshSize = meshFilter.sharedMesh.bounds.size;
            if (localMeshSize.y != 0)
            {
                originalPrefabAspectRatio = localMeshSize.x / localMeshSize.y;
            }
            else
            {
                Debug.LogWarning("GestureFramePlacer: Prefab-Mesh hat keine Höhe. Annahme 1:1 Seitenverhältnis.");
            }
            originalPrefabDepth = localMeshSize.z;
        }
        else
        {
            RectTransform rectTransform = GetComponent<RectTransform>();
            if (rectTransform != null && rectTransform.rect.height != 0)
            {
                originalPrefabAspectRatio = rectTransform.rect.width / rectTransform.rect.height;
                originalPrefabDepth = 0.01f; 
            }
            else
            {
                Debug.LogWarning("GestureFramePlacer: Prefab hat weder MeshFilter/Mesh noch RectTransform. Annahme 1:1 Seitenverhältnis und Standardtiefe.");
            }
        }
    }

    void Update()
    {
        // Dieses Skript läuft nur, wenn sein GameObject aktiv ist (gesteuert vom Holder).
        // Wir müssen hier nur noch die Handverfolgung prüfen.
        if (handSubsystem == null || !handSubsystem.leftHand.isTracked)
        {
            // Wenn die Hand nicht verfolgt wird, können wir den Frame nicht positionieren.
            // Das GameObject wird vom Holder deaktiviert, wenn der Scan stoppt.
            return;
        }

        var indexJoint = handSubsystem.leftHand.GetJoint(XRHandJointID.IndexTip);
        var thumbJoint = handSubsystem.leftHand.GetJoint(XRHandJointID.ThumbTip);

        if (!thumbJoint.TryGetPose(out Pose thumbPose) || !indexJoint.TryGetPose(out Pose indexPose))
        {
            // Wenn Posen nicht verfügbar sind, können wir den Frame nicht positionieren.
            return;
        }

        Vector3 thumbPos = thumbPose.position;
        Vector3 indexPos = indexPose.position;

        Vector3 midPoint = (thumbPos + indexPos) / 2f;

        // Positioniere den Frame mit Offset
        Vector3 offsetPosition = midPoint + Vector3.up * verticalOffset + Camera.main.transform.right * horizontalOffset;
        this.transform.position = offsetPosition; // Manipuliere die eigene Position

        // Rotiere den Frame, sodass er immer zur Kamera zeigt und aufrecht ist
        if (Camera.main != null)
        {
            this.transform.rotation = Quaternion.LookRotation(Camera.main.transform.forward, Vector3.up); // Manipuliere die eigene Rotation
        }
        else
        {
            this.transform.rotation = Quaternion.identity; // Keine Rotation
        }

        // Skaliere den Frame mit fester Breite und proportionaler Höhe
        float newWidth = fixedFrameWidth;
        float newHeight = newWidth / originalPrefabAspectRatio;
        
        this.transform.localScale = new Vector3(newWidth, newHeight, originalPrefabDepth); // Manipuliere die eigene Skalierung
    }
}















// using UnityEngine.XR.Hands;
// using System.Collections.Generic;
// using UnityEngine;

// public class GestureFramePlacer : MonoBehaviour
// {
//     private XRHandSubsystem handSubsystem;
//     private GameObject testFrame;
//     public GameObject _scanFrameTestPrefab;

//     // Feste Größe für den ScanFrame
//     [SerializeField] private float fixedFrameWidth = 0.05f; // Beispiel: 20 cm Breite
//     private float originalPrefabAspectRatio = 1.0f; // Standardwert 1:1 (Quadrat), falls nicht ermittelbar
//     private float originalPrefabDepth = 0.01f; // Standardtiefe, falls nicht ermittelbar oder gewünscht

//     // NEU: Offset für die Position des ScanFrames relativ zur Mitte der Finger
//     [SerializeField] private float verticalOffset = 0.05f; // Verschiebung nach oben (z.B. 5 cm)
//     [SerializeField] private float horizontalOffset = 0.05f; // Verschiebung zur Seite (z.B. 5 cm nach rechts, relativ zur Kamera)


//     void Start()
//     {
//         var handSubsystems = new List<XRHandSubsystem>();
//         SubsystemManager.GetSubsystems(handSubsystems);

//         if (handSubsystems.Count > 0)
//         {
//             handSubsystem = handSubsystems[0];
//             Debug.Log("XRHandSubsystem gefunden!");
//         }
//         else
//         {
//             Debug.LogError("Kein XRHandSubsystem gefunden. Gestenverfolgung nicht möglich.");
//             enabled = false; // Deaktiviere das Skript, wenn kein Subsystem vorhanden ist
//             return;
//         }

//         // Ermittle das ursprüngliche Seitenverhältnis und die Tiefe des Prefabs
//         if (_scanFrameTestPrefab != null)
//         {
//             MeshFilter meshFilter = _scanFrameTestPrefab.GetComponent<MeshFilter>();
//             if (meshFilter != null && meshFilter.sharedMesh != null)
//             {
//                 Vector3 localMeshSize = meshFilter.sharedMesh.bounds.size;
//                 if (localMeshSize.y != 0)
//                 {
//                     originalPrefabAspectRatio = localMeshSize.x / localMeshSize.y;
//                 }
//                 else
//                 {
//                     Debug.LogWarning("Prefab-Mesh hat keine Höhe. Annahme 1:1 Seitenverhältnis.");
//                 }
//                 originalPrefabDepth = localMeshSize.z;
//             }
//             else
//             {
//                 RectTransform rectTransform = _scanFrameTestPrefab.GetComponent<RectTransform>();
//                 if (rectTransform != null && rectTransform.rect.height != 0)
//                 {
//                     originalPrefabAspectRatio = rectTransform.rect.width / rectTransform.rect.height;
//                     originalPrefabDepth = 0.01f;
//                 }
//                 else
//                 {
//                     Debug.LogWarning("Prefab hat weder MeshFilter/Mesh noch RectTransform. Annahme 1:1 Seitenverhältnis und Standardtiefe.");
//                 }
//             }
//         }
//         else
//         {
//             Debug.LogError("_scanFrameTestPrefab wurde nicht zugewiesen!");
//             enabled = false;
//         }
//     }

//     void Update()
//     {
//         // Überprüfe, ob das Subsystem und die linke Hand verfolgt werden
//         if (handSubsystem == null || !handSubsystem.leftHand.isTracked)
//         {
//             if (testFrame != null)
//             {
//                 testFrame.SetActive(false); // Verstecke den Frame, wenn die Hand nicht verfolgt wird
//             }
//             return;
//         }
//         else
//         {
//             if (testFrame != null && !testFrame.activeSelf)
//             {
//                 testFrame.SetActive(true); // Zeige den Frame wieder, wenn die Hand verfolgt wird
//             }
//         }

//         var indexJoint = handSubsystem.leftHand.GetJoint(XRHandJointID.IndexTip);
//         var thumbJoint = handSubsystem.leftHand.GetJoint(XRHandJointID.ThumbTip);

//         if (!thumbJoint.TryGetPose(out Pose thumbPose) || !indexJoint.TryGetPose(out Pose indexPose))
//         {
//             if (testFrame != null)
//             {
//                 testFrame.SetActive(false); // Verstecke den Frame, wenn Posen nicht verfügbar sind
//             }
//             return;
//         }

//         Vector3 thumbPos = thumbPose.position;
//         Vector3 indexPos = indexPose.position;

//         Vector3 midPoint = (thumbPos + indexPos) / 2f;
//         // Vector3 directionBetweenFingers = (indexPos - thumbPos).normalized; // Nicht mehr für Rotation benötigt

//         if (testFrame == null)
//         {
//             testFrame = Instantiate(_scanFrameTestPrefab);
//             testFrame.SetActive(true);
//         }

//         // NEU: Positioniere den Frame mit Offset
//         // Verschiebung nach oben (world up) und zur Seite (relativ zur Kamera-Rechts-Achse)
//         Vector3 offsetPosition = midPoint + Vector3.up * verticalOffset + Camera.main.transform.right * horizontalOffset;
//         testFrame.transform.position = offsetPosition;

//         // NEU: Rotiere den Frame, sodass er immer zur Kamera zeigt und aufrecht ist
//         if (Camera.main != null)
//         {
//             // Der Frame soll zur Kamera zeigen (seine Z-Achse zeigt zur Kamera)
//             // Die Y-Achse des Frames soll nach oben zeigen (world up)
//             testFrame.transform.rotation = Quaternion.LookRotation(Camera.main.transform.forward, Vector3.up);
//         }
//         else
//         {
//             // Fallback, falls Camera.main nicht verfügbar ist
//             testFrame.transform.rotation = Quaternion.identity; // Keine Rotation
//         }

//         // Skaliere den Frame mit fester Breite und proportionaler Höhe
//         float newWidth = fixedFrameWidth; // Feste Breite
//         float newHeight = newWidth / originalPrefabAspectRatio; // Höhe proportional zur Breite und dem ursprünglichen Seitenverhältnis

//         testFrame.transform.localScale = new Vector3(newWidth, newHeight, originalPrefabDepth);
//     }
// }
































// using UnityEngine.XR.Hands;
// using System.Collections.Generic;
// using UnityEngine;

// public class GestureFramePlacer : MonoBehaviour
// {
//     private XRHandSubsystem handSubsystem;
//     private GameObject testFrame;
//     public GameObject _scanFrameTestPrefab;

//     // NEU: Feste Größe für den ScanFrame
//     [SerializeField] private float fixedFrameWidth = 0.05f;
//     private float originalPrefabAspectRatio = 1.0f; // Standardwert 1:1 (Quadrat), falls nicht ermittelbar
//     private float originalPrefabDepth = 0.01f; // Standardtiefe, falls nicht ermittelbar oder gewünscht

//     void Start()
//     {
//         var handSubsystems = new List<XRHandSubsystem>();
//         SubsystemManager.GetSubsystems(handSubsystems);

//         if (handSubsystems.Count > 0)
//         {
//             handSubsystem = handSubsystems[0];
//             Debug.Log("XRHandSubsystem gefunden!");
//         }
//         else
//         {
//             Debug.LogError("Kein XRHandSubsystem gefunden. Gestenverfolgung nicht möglich.");
//             enabled = false; // Deaktiviere das Skript, wenn kein Subsystem vorhanden ist
//             return;
//         }

//         // Ermittle das ursprüngliche Seitenverhältnis und die Tiefe des Prefabs
//         if (_scanFrameTestPrefab != null)
//         {
//             MeshFilter meshFilter = _scanFrameTestPrefab.GetComponent<MeshFilter>();
//             if (meshFilter != null && meshFilter.sharedMesh != null)
//             {
//                 Vector3 localMeshSize = meshFilter.sharedMesh.bounds.size;
//                 if (localMeshSize.y != 0)
//                 {
//                     originalPrefabAspectRatio = localMeshSize.x / localMeshSize.y;
//                 }
//                 else
//                 {
//                     Debug.LogWarning("Prefab-Mesh hat keine Höhe. Annahme 1:1 Seitenverhältnis.");
//                 }
//                 originalPrefabDepth = localMeshSize.z;
//             }
//             else
//             {
//                 RectTransform rectTransform = _scanFrameTestPrefab.GetComponent<RectTransform>();
//                 if (rectTransform != null && rectTransform.rect.height != 0)
//                 {
//                     originalPrefabAspectRatio = rectTransform.rect.width / rectTransform.rect.height;
//                     originalPrefabDepth = 0.01f; 
//                 }
//                 else
//                 {
//                     Debug.LogWarning("Prefab hat weder MeshFilter/Mesh noch RectTransform. Annahme 1:1 Seitenverhältnis und Standardtiefe.");
//                 }
//             }
//         }
//         else
//         {
//             Debug.LogError("_scanFrameTestPrefab wurde nicht zugewiesen!");
//             enabled = false;
//         }
//     }

//     void Update()
//     {
//         // Überprüfe, ob das Subsystem und die linke Hand verfolgt werden
//         if (handSubsystem == null || !handSubsystem.leftHand.isTracked)
//         {
//             if (testFrame != null)
//             {
//                 testFrame.SetActive(false); // Verstecke den Frame, wenn die Hand nicht verfolgt wird
//             }
//             return;
//         }
//         else
//         {
//             if (testFrame != null && !testFrame.activeSelf)
//             {
//                 testFrame.SetActive(true); // Zeige den Frame wieder, wenn die Hand verfolgt wird
//             }
//         }

//         var indexJoint = handSubsystem.leftHand.GetJoint(XRHandJointID.IndexTip);
//         var thumbJoint = handSubsystem.leftHand.GetJoint(XRHandJointID.ThumbTip);

//         if (!thumbJoint.TryGetPose(out Pose thumbPose) || !indexJoint.TryGetPose(out Pose indexPose))
//         {
//             if (testFrame != null)
//             {
//                 testFrame.SetActive(false); // Verstecke den Frame, wenn Posen nicht verfügbar sind
//             }
//             return;
//         }

//         Vector3 thumbPos = thumbPose.position;
//         Vector3 indexPos = indexPose.position;

//         Vector3 midPoint = (thumbPos + indexPos) / 2f;
//         Vector3 directionBetweenFingers = (indexPos - thumbPos).normalized; // Richtung von Daumen zu Zeigefinger
//         // float distance = Vector3.Distance(thumbPos, indexPos); // Abstand zwischen Daumen und Zeigefinger (nicht mehr für Skalierung verwendet)

//         if (testFrame == null)
//         {
//             testFrame = Instantiate(_scanFrameTestPrefab);
//             testFrame.SetActive(true); 
//         }

//         // Positioniere den Frame in der Mitte der Finger
//         testFrame.transform.position = midPoint;

//         // NEU: Rotiere den Frame
//         // Der Frame soll zur Kamera zeigen (Camera.main.transform.forward)
//         // Die untere Kante des Rechtecks (lokale X-Achse des Frames) soll horizontal zur Richtung der Finger sein.
//         // Die Y-Achse des Frames soll senkrecht zur Richtung der Finger und der Kamera-Blickrichtung sein.
//         if (Camera.main != null)
//         {
//             Vector3 frameForward = Camera.main.transform.forward;
//             Vector3 frameRight = directionBetweenFingers; // Die X-Achse des Frames soll entlang der Fingerlinie sein
//             Vector3 frameUp = Vector3.Cross(frameRight, frameForward).normalized; // Die Y-Achse des Frames

//             testFrame.transform.rotation = Quaternion.LookRotation(frameForward, frameUp);
//         }
//         else
//         {
//             // Fallback, falls Camera.main nicht verfügbar ist
//             testFrame.transform.rotation = Quaternion.LookRotation(Vector3.forward, directionBetweenFingers);
//         }

//         // NEU: Skaliere den Frame mit fester Breite und proportionaler Höhe
//         float newWidth = fixedFrameWidth; // Feste Breite
//         float newHeight = newWidth / originalPrefabAspectRatio; // Höhe proportional zur Breite und dem ursprünglichen Seitenverhältnis
        
//         testFrame.transform.localScale = new Vector3(newWidth, newHeight, originalPrefabDepth);
//     }
// }
