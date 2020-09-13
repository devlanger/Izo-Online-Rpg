using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    private Vector3 offset;

    public Transform target;

    private void Update()
    {
        if(target == null)
        {
            return;
        }

        transform.position = target.position + offset;
    }
}
