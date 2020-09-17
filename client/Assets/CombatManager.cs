using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatManager : Singleton<CombatManager>
{
    public event Action<DamageInfo> OnDamageReceived = delegate { };

    private void Awake()
    {
        Instance = this;
    }

    public void DealDamage(DamageInfo info)
    {
        OnDamageReceived(info);
    }
}

public class DamageInfo
{
    public int targetId;
    public ushort damage;
    public byte type;
}