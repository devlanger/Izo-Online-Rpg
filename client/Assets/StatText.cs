using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatText : MonoBehaviour
{
    [SerializeField]
    private StatType stat;
    
    [SerializeField]
    private string format;

    private Text text;

    private void Start()
    {
        text = GetComponent<Text>();
        PlayerController.Instance.OnLocalPlayerChanged += Instance_OnLocalPlayerChanged;
    }

    private void Instance_OnLocalPlayerChanged(Character obj)
    {
        obj.OnStatChanged += Obj_OnStatChanged;
        Obj_OnStatChanged(stat, obj.stats[stat]);//
    }

    private void Obj_OnStatChanged(StatType arg1, object arg2)
    {
        if(stat == arg1)
        {
            text.text = string.Format(format, arg2.ToString());
        }
    }
}
