using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager UIManagerInstance { get; private set; }

    [SerializeField] private List<GameObject> _persitantPanels = new();
    [SerializeField] private GameObject _contentRoot;

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
        

        if (_contentRoot != null)
        {
            Debug.Log($"Closing all instantiated panels under: {_contentRoot.name}");

            Transform[] allChildren = _contentRoot.GetComponentsInChildren<Transform>();

            foreach (Transform childTransform in allChildren)
            {
                if (childTransform == _contentRoot.transform)
                {
                    continue;
                }
                else if (childTransform.gameObject.activeSelf)
                {
                    childTransform.gameObject.SetActive(false);
                    Debug.Log($"Closed product panel: {childTransform.name}");
                }
            }
        }
    }
}
