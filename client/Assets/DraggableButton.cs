using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class DraggableButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDropHandler, IDragHandler
{
    private CanvasGroup group;

    public UnityEvent<PointerEventData> OnDrag;
    public UnityEvent<PointerEventData> OnDrop;
    public UnityEvent<PointerEventData> OnPickup;
    public UnityEvent<PointerEventData> OnRelease;

    public Vector3 PickupPosition { get; private set; }

    public void ReturnToPickupPosition()
    {
        transform.position = PickupPosition;
    }

    private void Awake()
    {
        if(group == null)
        {
            group = gameObject.AddComponent<CanvasGroup>();
        }
    }

    public void SetInteractable(bool interactable)
    {
        group.blocksRaycasts = interactable;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        PickupPosition = transform.position;
        OnPickup.Invoke(eventData);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        OnRelease.Invoke(eventData);
    }

    void IDragHandler.OnDrag(PointerEventData eventData)
    {
        OnDrag.Invoke(eventData);
    }

    void IDropHandler.OnDrop(PointerEventData eventData)
    {
        OnDrop.Invoke(eventData);
    }
}
