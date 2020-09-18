using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UISkillBar : MonoBehaviour
{
    private void Start()
    {
        foreach (var item in GetComponentsInChildren<DraggableButton>())
        {
            item.OnDrop.AddListener((ev) =>
            {
                SkillBarSlot slot = item.GetComponentInParent<SkillBarSlot>();
                DraggableButton dragged = ev.pointerDrag.GetComponent<DraggableButton>();
                slot.Fill(dragged.GetComponentInParent<SkillButton>().skillId);
            });
        }
    }

}
