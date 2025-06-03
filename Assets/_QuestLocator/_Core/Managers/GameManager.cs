using UnityEngine;
using TMPro;
using System;

public class GameManager : MonoBehaviour
{
    private APIManager apiManager;
    private OVRCameraRig cameraRig;
    [SerializeField] private OVRInput.Button inputAction;
    [SerializeField] private GameObject infoDisplay;
    [SerializeField] private string promptWord = "Nutella";
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        cameraRig = FindAnyObjectByType<OVRCameraRig>();
        apiManager = GameObject.FindGameObjectWithTag("API_Manager").GetComponent<APIManager>();

    }

    // Update is called once per frame
    void Update()
    {
        HandleInputs(OVRInput.Hand.HandLeft);
    }

    private void HandleInputs(OVRInput.Hand hand){ 
        
    }

    private void SelectAction() //
    {
        throw new NotImplementedException();
    }
}
 