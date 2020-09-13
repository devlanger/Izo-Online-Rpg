using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate : MonoBehaviour
{
    [SerializeField]
    private Vector3 rotationAxis;

    void Update()
    {
        transform.localEulerAngles += rotationAxis * Time.deltaTime;
    }
}
