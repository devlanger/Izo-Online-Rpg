using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UITarget : MonoBehaviour
{
    [SerializeField]
    private GameObject targetPanel;

    [SerializeField]
    private Text targetName;

    [SerializeField]
    private Text lvlText;

    [SerializeField]
    private Slider healthBar;

    private Character target;

    private void Start()
    {
        TargetController.Instance.OnTargetChanged += Instance_OnTargetChanged;
    }

    private void Instance_OnTargetChanged(Character obj)
    {
        if(target != null)
        {
            target.OnStatChanged -= Obj_OnStatChanged;
        }

        targetPanel.SetActive(obj != null);
        if (obj != null)
        {
            healthBar.value = (int)obj.stats[StatType.HEALTH];
            healthBar.maxValue = (int)obj.stats[StatType.MAX_HEALTH];

            targetName.text = obj.Data.nickname;
            lvlText.text = string.Format("Lv. {0}", obj.Data.lvl);
            obj.OnStatChanged += Obj_OnStatChanged;
            target = obj;
        }
    }

    private void Obj_OnStatChanged(StatType arg1, object arg2)
    {
        switch(arg1)
        {
            case StatType.HEALTH:
                healthBar.value = (int)arg2;
                break;

            case StatType.MAX_HEALTH:
                healthBar.maxValue = (int)arg2;
                break;
        }
    }
}
