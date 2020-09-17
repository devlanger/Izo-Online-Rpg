using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DamageInfoLabel : MonoBehaviour
{
    [SerializeField]
    private Text text;

    public void Fill(DamageInfo obj)
    {
        text.text = obj.damage.ToString();
    }
}
