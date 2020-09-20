using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UISkillsPanel : MonoBehaviour
{
    private Vector3 draggableStartPosition;

    [SerializeField]
    private List<int> skills = new List<int>();

    private void Start()
    {
        PlayerController.Instance.OnLocalPlayerChanged += Instance_OnLocalPlayerChanged;
    }

    private void Instance_OnLocalPlayerChanged(Character obj)
    {
        List<SkillDataHandler> classSkills = SkillsManager.Instance.GetSkillsForClass((Class)obj.Data.@class);
        SkillButton[] bts = GetComponentsInChildren<SkillButton>();
        for (int i = 0; i < bts.Length; i++)
        {
            bts[i].Fill(classSkills[i].id);
        }
    }
}
