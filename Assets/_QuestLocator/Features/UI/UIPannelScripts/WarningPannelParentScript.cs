using UnityEngine;
using Oculus.Interaction;
using System.Collections.Generic;
using System;
using Unity.XR.CoreUtils;


public class WarningPannelParentScript : MonoBehaviour
{
    [SerializeField] GameObject WarnigPannelPrefab;

    [SerializeField] int maxPannelCount;

    private UIThemeManagerLocal themeManager;

    //private GameObject[] warningPanels;
    private List<GameObject> warningPanels = new List<GameObject>();

    private String lastErrorMessage;

    void Start()
    {
        themeManager = GetComponentInParent<UIThemeManagerLocal>();
    }

    public void SetUpWarning(string warningText)
    {
        if (lastErrorMessage != warningText) // if the error pannel already exist dont spawn new one
        {
            lastErrorMessage = warningText; // save the new error message

            // create new pannel and set the text
            GameObject newPanel = Instantiate(WarnigPannelPrefab, transform);
            newPanel.GetComponent<WarningPannelScript>().SetUpWarning(warningText);
            newPanel.name = warningText.Substring(warningText.Length - 4);


            warningPanels.Clear();
            gameObject.GetChildGameObjects(warningPanels);

            //Debug.LogError(warningPanels.Count);
            if (warningPanels.Count > maxPannelCount) //if the pannel count is bigger than the max allowed, destroy the first one
            {
                Destroy(warningPanels[0]); //destroy first pannel
                Debug.LogError("destroyed");
            }

            themeManager.ApplyCurrentTheme();
        }

    }

    public void ClearWarningPannels()
    {
        warningPanels.Clear();
        gameObject.GetChildGameObjects(warningPanels);
        foreach (GameObject panel in warningPanels)
        {
            Destroy(panel);
        }
    }

    public void ClearLastErrorMassage()
    {
        lastErrorMessage = "";
    }
}
