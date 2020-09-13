using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    [SerializeField]
    private CharacterController controller;

    public Animator Animator;

    [SerializeField]
    private float speed = 3;

    private Vector3 destination;
    private bool isMoving = false;

    public CharacterData Data { get; set; }

    public void MoveTo(Vector3 point)
    {
        this.destination = point;
    }

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        destination = transform.position;
    }

    private void OnMouseDown()
    {
        if (Data != null)
        {
            SelectionController.Instance.RequestSelectTarget(Data.id, 1);
        }
    }

    private void Update()
    {
        Vector3 pos1 = transform.position;
        pos1.y = 0;
        Vector3 pos2 = destination;
        pos2.y = 0;


        if (Vector3.Distance(pos1, pos2) > 0.1f)
        {
            Vector3 dir = destination - transform.position;
            dir.y = 0;
            dir = dir.normalized * speed;

            controller.SimpleMove(dir);

            if (!isMoving)
            {
                isMoving = true;
                Animator.SetBool("move", true);
            }
        }
        else
        {
            if(isMoving)
            {
                isMoving = false;
                Animator.SetBool("move", false);
            }
        }
    }
}
