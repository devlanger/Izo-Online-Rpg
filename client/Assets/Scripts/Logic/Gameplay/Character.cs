using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    [SerializeField]
    private CharacterController controller;

    public GameObject baseModel;
    public Animator Animator;

    [SerializeField]
    private float speed = 3;

    private Vector3 destination;
    private bool isMoving = false;

    public CharacterData Data { get; set; }
    public int LastTargetId { get; set; }

    public Dictionary<int, ItemInstance> items = new Dictionary<int, ItemInstance>();

    public Dictionary<StatType, object> stats = new Dictionary<StatType, object>()
    {
        { StatType.NAME, "Object" },
        { StatType.LEVEL, (short)1 },
        { StatType.HEALTH, (int)100 },
        { StatType.MAX_HEALTH, (int)100 },
        { StatType.MANA, (int)100 },
        { StatType.MAX_MANA, (int)100 },
        { StatType.EXPERIENCE, (int)0 },
        { StatType.GOLD, (int)0 },
        { StatType.RACE, (byte)0 },
        { StatType.CLASS, (byte)0 },
        { StatType.POS_X, (short)0 },
        { StatType.POS_Z, (short)0 },
        { StatType.TARGET_ID, (int)-1 },
    };

    public event Action<StatType, object> OnStatChanged = delegate { };
    public event Action<Dictionary<int, ItemInstance>> OnInventoryChanged = delegate { };

    public void MoveTo(Vector3 point)
    {
        this.destination = point;
    }

    public void SetStat(StatType stat, object val)
    {
        stats[stat] = val;
        OnStatChanged(stat, val);
    }

    public void SetItem(ushort slot, int itemId)
    {
        items[slot] = new ItemInstance()
        {
            id = itemId
        };
    }

    public void RefreshInventory()
    {
        OnInventoryChanged(items);
    }

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        destination = transform.position;
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
            if (isMoving)
            {
                isMoving = false;
                Animator.SetBool("move", false);
            }
        }
    }

    public void LookAt(Vector3 pos)
    {
        Vector3 target = pos - transform.position;
        target.y = 0;

        transform.rotation = Quaternion.LookRotation(target);
    }
}
