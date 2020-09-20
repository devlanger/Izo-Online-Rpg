using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIEquipment : ItemsContainerInventory
{
}

[System.Serializable]
public class ItemsContainerInventory : MonoBehaviour
{
    private Dictionary<int, InventoryButton> items = new Dictionary<int, InventoryButton>();

    public bool syncOnDrop = false;

    [SerializeField]
    private Transform itemsParent;

    [SerializeField]
    private ItemContainerId containerId;

    private void Awake()
    {
        InventoryButton[] bts = itemsParent.GetComponentsInChildren<InventoryButton>();
        for (int i = 0; i < bts.Length; i++)
        {
            int slot = i;
            bts[i].ContainerId = containerId;
            bts[i].Slot = slot;
            items.Add(i, bts[i]);

            OnInitializedButton(slot, bts[i]);
        }
    }

    private void Start()
    {
        PlayerController.Instance.OnLocalPlayerChanged += Instance_OnLocalPlayerChanged;
    }

    private void Instance_OnLocalPlayerChanged(Character obj)
    {
        obj.itemContainers[containerId].OnInventoryChanged += OnInventoryChanged;
    }

    protected virtual void OnInitializedButton(int slot, InventoryButton button)
    {
        if (syncOnDrop)
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

    protected virtual void OnInventoryChanged(ItemsContainer ic)
    {
        var list = ic.items;

        foreach (var item in items)
        {
            item.Value.Fill(null);
        }

        for (int i = 0; i < items.Count; i++)
        {
            if (list.ContainsKey(i))
            {
                items[i].Fill(list[i]);
            }
        }
    }
}