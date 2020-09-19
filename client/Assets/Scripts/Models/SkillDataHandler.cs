using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[CreateAssetMenu(menuName = "Data/Skill Base")]
public class SkillDataHandler : ScriptableObject
{
    public int id;
    public string name;
    public Class charClass;
    public int reqLvl;
    public Sprite icon;

    public PlayableAsset animationClip;
}