using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillButton : MonoBehaviour
{
    public int skillId = 0;

    [SerializeField]
    private Image icon;

    public void Fill(int v)
    {
        skillId = v;
        if(SkillsManager.Instance.GetSkillData(skillId, out SkillDataHandler handler))
        {
            icon.sprite = handler.icon;
        }
    }
}
