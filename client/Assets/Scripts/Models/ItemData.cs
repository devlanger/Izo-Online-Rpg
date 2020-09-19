using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/Item Data")]
public class ItemData : ScriptableObject
{
    public int id;
    public string name;
    public Sprite icon;
    public string description;
    public int reqLevel;
}
