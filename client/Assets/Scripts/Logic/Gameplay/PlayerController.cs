using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance { get; private set; }
    public Character Target { get => target; private set => target = value; }

    [SerializeField]
    private Character target;

    [SerializeField]
    private LayerMask clickMask;

    public event Action<Character> OnLocalPlayerChanged = delegate { };

    private void Awake()
    {
        Instance = this;
    }

    public static bool MouseOverUi()
    {
        return EventSystem.current.IsPointerOverGameObject();
    }

    private void Update()
    {
        if(Input.GetMouseButtonDown(0) && !MouseOverUi())
        {
            Ray r = Camera.main.ScreenPointToRay(Input.mousePosition);
            if(Physics.Raycast(r, out RaycastHit hit, clickMask))
            {
                Character target = hit.collider.GetComponent<Character>();
                
                if (target != null)
                {
                    SelectionController.Instance.RequestSelectTarget(target.Data.id, 1);
                }
                else
                {

                    Vector3 destination = hit.point;
                    destination.x = Mathf.RoundToInt(destination.x);
                    destination.y = Mathf.RoundToInt(destination.y);
                    destination.z = Mathf.RoundToInt(destination.z);

                    Vector3 dir = destination - Target.transform.position;
                    dir.y = 0;

                    Target.transform.rotation = Quaternion.LookRotation(dir);

                    Target.MoveTo(hit.point);

                    Packet packet = new Packet();
                    packet.writer.Write((byte)11);
                    packet.writer.Write((short)destination.x);
                    packet.writer.Write((short)destination.z);

                    Connection.Instance.SendData(packet);
                }
            }
        }
    }

    public void SetPlayer(Character c)
    {
        Target = c;
        c.gameObject.layer = 9;
        FindObjectOfType<CameraController>().target = target.transform;
        OnLocalPlayerChanged(c);
    }
}
