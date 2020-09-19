using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ItemsManager : Singleton<ItemsManager>
{
    [SerializeField]
    private List<ItemData> items = new List<ItemData>();

    private void Awake()
    {
        Instance = this;

        items = Resources.LoadAll<ItemData>("Data/Items").ToList();
    }

    public ItemData GetItemData(int id)
    {
        return items.Find(s => s.id == id);
    }

    public bool GetItemData(int id, out ItemData itemData)
    {
        ItemData handler = items.Find(s => s.id == id);
        itemData = handler;
        return handler != null;
    }
}
