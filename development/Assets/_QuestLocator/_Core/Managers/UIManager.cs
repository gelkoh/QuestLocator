using System.Collections.Generic;
using Meta.WitAi.TTS.Utilities;
using UnityEngine;
using static TutorialStateManager;

public class UIManager : MonoBehaviour
{
    public static UIManager UIManagerInstance { get; private set; }

    [SerializeField] private List<GameObject> _persitantPanels = new();
    [SerializeField] private GameObject _contentRoot;
    [SerializeField] private GameObject WarningPannelManager;

    void Awake()
    {
        if (UIManagerInstance == null)
        {
            UIManagerInstance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void CloseAllPanels()
    {
        TTSSpeaker ttsSpeaker = GameObject.FindGameObjectWithTag("TTS").GetComponent<TTSSpeaker>();
        if (ttsSpeaker != null)
        {
            ttsSpeaker.Stop();
        }
        else
        {
            Debug.LogWarning("TTSSpeaker not found on IntroPanel. Please add a TTSSpeaker component as a child.");
        }

        // Set "persistant" panels inactive instead of destroying them (because they will never exist multiple times in the scene anyways)
        if (_persitantPanels != null)
        {
            foreach (var panel in _persitantPanels)
            {
                if (panel != null && panel.activeSelf)
                {
                    panel.SetActive(false);
                }
            }
        }

        TutorialStateManagerInstance.SkipTutorial();

        // Really destroy product related panels because there might be many at some point and maybe use up a lot of memory
        if (_contentRoot != null)
        {
            Debug.Log($"Destroying all instantiated panels under: {_contentRoot.name}");

            Transform[] allChildren = _contentRoot.GetComponentsInChildren<Transform>();

            foreach (Transform childTransform in allChildren)
            {
                if (childTransform == _contentRoot.transform)
                {
                    continue;
                }
                else if (childTransform.gameObject.activeSelf)
                {
                    // childTransform.gameObject.SetActive(false);
                    Destroy(childTransform.gameObject);
                    Debug.Log($"Destroyed product panel: {childTransform.name}");
                }
            }
        }

        WarningPannelParentScript warningPannelParentScript = WarningPannelManager.GetComponent<WarningPannelParentScript>();

        if (WarningPannelManager != null)
        {
            warningPannelParentScript.ClearWarningPannels();
        }

    }
}