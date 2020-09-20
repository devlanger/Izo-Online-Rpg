using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIInventory : ItemsContainerInventory
{
    protected override void OnInitializedButton(int slot, InventoryButton button)
    {
        button.GetComponentInChildren<DraggableButton>().OnDrop.AddListener((ev) =>
        {
            DraggableButton dragged = ev.pointerDrag.GetComponent<DraggableButton>();
            InventoryButton sourceButton = dragged.GetComponentInParent<InventoryButton>();

            Connection.Instance.SendData(new ItemActionPacket(new ItemActionData()
            {
                sourceSlot = sourceButton.Slot,
                targetSlot = slot,
                action = ItemAction.MOVE,
                sourceContainer = sourceButton.ContainerId,
                targetContainer = button.ContainerId
            }));
        });
    }
}
