using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EffectBehaviour : MonoBehaviour
{
    public abstract void Run(Character user, Character target);
}
