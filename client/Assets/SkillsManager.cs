using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SkillsManager : Singleton<SkillsManager>
{
    [SerializeField]
    private List<SkillDataHandler> skills = new List<SkillDataHandler>();

    private void Awake()
    {
        Instance = this;

        skills = Resources.LoadAll<SkillDataHandler>("Data/Skills").ToList();
    }

    public SkillDataHandler GetSkillData(int id)
    {
        return skills.Find(s => s.id == id);
    }

    public bool GetSkillData(int id, out SkillDataHandler skillData)
    {
        SkillDataHandler handler = skills.Find(s => s.id == id);
        skillData = handler;
        return handler != null;
    }
}
