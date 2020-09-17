using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIStatsBar : MonoBehaviour
{
    [SerializeField]
    private Slider healthBar;

    [SerializeField]
    private Slider manaBar;

    [SerializeField]
    private Slider expBar;

    [SerializeField]
    private Text levelText;

    private void Start()
    {
        PlayerController.Instance.OnLocalPlayerChanged += Instance_OnLocalPlayerChanged;
    }

    private void Instance_OnLocalPlayerChanged(Character obj)
    {
        obj.OnStatChanged += Obj_OnStatChanged;

        Obj_OnStatChanged(StatType.LEVEL, (short)obj.stats[StatType.LEVEL]);
        Obj_OnStatChanged(StatType.EXPERIENCE, (int)obj.stats[StatType.EXPERIENCE]);
    }

    private void Obj_OnStatChanged(StatType arg1, object arg2)
    {
        switch(arg1)
        {
            case StatType.HEALTH:
                healthBar.value = (int)arg2;
                break;
            case StatType.EXPERIENCE:
                expBar.value = (int)arg2;
                break;
            case StatType.MANA:
                manaBar.value = (int)arg2;
                break;
            case StatType.LEVEL:
                levelText.text = "Lv. " + arg2;
                break;
        }
    }
}
