using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplodeBehaviour : EffectBehaviour
{
    [SerializeField]
    private GameObject explosion;

    [SerializeField]
    private float destroyAfter = 1;

    [SerializeField]
    private bool parent = false;

    [SerializeField]
    private bool useTargetRotation = false;

    public override void Run(Character user, Character target)
    {
    }

    public void Explode(Character user, Character target)
    {
        GameObject inst = Instantiate(explosion, target.transform.position, useTargetRotation ? target.transform.rotation : Quaternion.identity, parent ? target.transform : null);
        Destroy(inst, destroyAfter);
    }
}
