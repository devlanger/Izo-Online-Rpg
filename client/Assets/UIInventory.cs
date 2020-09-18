using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIInventory : MonoBehaviour
{
    private Dictionary<int, InventoryButton> items = new Dictionary<int, InventoryButton>();

    [SerializeField]
    private Transform itemsParent;

    private void Awake()
    {
        InventoryButton[] bts = itemsParent.GetComponentsInChildren<InventoryButton>();
        for (int i = 0; i < bts.Length; i++)
        {
            items.Add(i, bts[i]);
        }
    }

    private void Start()
    {
        PlayerController.Instance.OnLocalPlayerChanged += Instance_OnLocalPlayerChanged;
    }

    private void Instance_OnLocalPlayerChanged(Character obj)
    {
        obj.OnInventoryChanged += Obj_OnInventoryChanged;
    }

    private void Obj_OnInventoryChanged(Dictionary<int, ItemData> obj)
    {
        for (int i = 0; i < items.Count; i++)
        {
            if(obj.ContainsKey(i))
            {
                items[i].Fill(obj[i]);
            }
            else
            {
                items[i].Fill(null);
            }
        }
    }
}
