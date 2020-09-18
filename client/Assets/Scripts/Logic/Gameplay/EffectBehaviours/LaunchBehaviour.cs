using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class LaunchBehaviour : EffectBehaviour
{
    [SerializeField]
    private float speed = 15;

    private Character user;
    private Character target;
    private Rigidbody rb;

    public override void Run(Character user, Character target)
    {
        this.user = user;
        this.target = target;
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        if(target == null)
        {
            Destroy(gameObject);
            return;
        }

        if (Vector3.Distance(transform.position, target.transform.position) > 1)
        {
            rb.velocity = (target.transform.position - user.transform.position).normalized * speed;
            transform.rotation = Quaternion.LookRotation(rb.velocity);
        }
        else
        {
            var explode = GetComponent<ExplodeBehaviour>();
            if (explode)
            {
                explode.Explode(user, target);
            }

            Destroy(gameObject);
        }
    }
}
