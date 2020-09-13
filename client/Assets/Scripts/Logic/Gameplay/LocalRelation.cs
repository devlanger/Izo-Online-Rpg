using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalRelation : MonoBehaviour
{
    public Relation relation;
}

public enum Relation
{
    NEUTRAL = 1,
    INTERACTABLE = 2,
    ATTACKABLE = 3
}