using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ListHandler<T> where T : MonoBehaviour
{
    public T prefab;
    public List<T> items = new List<T>();
    public Transform parent;

    public T SpawnItem()
    {
        T inst = GameObject.Instantiate(prefab, parent);
        items.Add(inst);
        return inst;
    }

    public void DestroyAll()
    {
        foreach (var item in items)
        {
            if (item.gameObject != null)
            {
                GameObject.Destroy(item.gameObject);
            }
        }

        items.Clear();
    }

    public void Destroy(T item, float v)
    {
        GameObject.Destroy(item.gameObject, v);
        items.Remove(item);
    }
}
