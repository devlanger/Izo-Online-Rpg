using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryButton : MonoBehaviour
{
    [SerializeField]
    private Text itemText;

    [SerializeField]
    private Text descriptionText;

    [SerializeField]
    private Image icon;

    [SerializeField]
    private CanvasGroup grp;

    public ItemInstance ItemInstanceData;
    public int Slot { get; set; }

    public ItemContainerId ContainerId { get; set; }

    private void Awake()
    {
        var db = GetComponentInChildren<DraggableButton>();
        db.OnHover.AddListener(Hover);
        db.OnExitHover.AddListener(EndHover);
    }

    private void Hover(PointerEventData ev)
    {
        if (ItemInstanceData != null && ItemInstanceData.id != 0 && ItemsManager.Instance.GetItemData(ItemInstanceData.id, out ItemData data))
        {
            ItemHoverPanel.Instance.panel.Activate();
            ItemHoverPanel.Instance.Fill(data);
        }
    }

    private void EndHover(PointerEventData ev)
    {
        ItemHoverPanel.Instance.panel.Deactivate();
    }

    public void Fill(ItemInstance inst)
    {
        ItemInstanceData = inst;

        if(inst == null)
        {
            itemText.text = "";
            grp.alpha = 0;
            return;
        }

        if(ItemsManager.Instance.GetItemData(inst.id, out ItemData data))
        {
            icon.sprite = data.icon;
            itemText.text = data.name;
            icon.GetComponent<CanvasGroup>().alpha = 0;
            //descriptionText.text = data.description;
            grp.alpha = 1;
        }
        else 
        {
            grp.alpha = 0;
        }
    }
}
