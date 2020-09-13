using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NicknameHoverLabel : MonoBehaviour
{
    [SerializeField]
    private Text nameText;

    private Transform target;

    private void LateUpdate()
    {
        if(target == null)
        {
            return;
        }

        nameText.transform.position = Camera.main.WorldToScreenPoint(target.transform.position + new Vector3(0,1));
    }

    public void Fill(Character target)
    {
        nameText.text = target.name;
        this.target = target.transform;
    }
}
