using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class UIPanel : MonoBehaviour
{
    [SerializeField]
    private bool activeOnStart = false;

    [SerializeField]
    private bool overrideRaycasts = true;

    private CanvasGroup group;

    public bool Active { get; private set; }

    private void Awake()
    {
        group = GetComponent<CanvasGroup>();
    }

    private void Start()
    {
        if(activeOnStart)
        {
            Activate();
        }
        else
        {
            Deactivate();
        }
    }

    [ContextMenu("Activate")]
    public void Activate()
    {
        if(group == null)
        {
            group = GetComponent<CanvasGroup>();
        }

        group.alpha = 1;
        if (overrideRaycasts)
        {
            group.interactable = true;
            group.blocksRaycasts = true;
        }
        Active = true;
    }

    [ContextMenu("Deactivate")]
    public void Deactivate()
    {
        if (group == null)
        {
            group = GetComponent<CanvasGroup>();
        }

        group.alpha = 0; 
        if (overrideRaycasts)
        {
            group.interactable = false;
            group.blocksRaycasts = false;
        }
        Active = false;
    }

    public void Toggle()
    {
        if(Active)
        {
            Deactivate();
        }
        else
        {
            Activate();
        }
    }
}
