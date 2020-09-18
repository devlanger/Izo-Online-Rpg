using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectController : MonoBehaviour
{
    public Character User { get; set; }
    public Character Target { get; set; }

    public void Run(Character user, Character target)
    {
        foreach (var item in GetComponentsInChildren<EffectBehaviour>())
        {
            item.Run(user, target);
        }
    }
}
