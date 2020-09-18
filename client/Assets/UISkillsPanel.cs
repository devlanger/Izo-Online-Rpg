using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UISkillsPanel : MonoBehaviour
{
    private Vector3 draggableStartPosition;

    [SerializeField]
    private List<int> skills = new List<int>();

    private void Awake()
    {
        foreach (var item in GetComponentsInChildren<SkillButton>())
        {
            DraggableButton bt = item.GetComponentInChildren<DraggableButton>();
            bt.OnPickup.AddListener((ev) =>
            {
                bt.SetInteractable(false);
            });

            bt.OnRelease.AddListener((ev) =>
            {
                bt.SetInteractable(true);
                bt.ReturnToPickupPosition();
            });

            bt.OnDrag.AddListener((ev) =>
            {
                bt.transform.position = Input.mousePosition;
            });
        }
    }

    private void Start()
    {
        SkillButton[] bts = GetComponentsInChildren<SkillButton>();
        for (int i = 0; i < bts.Length; i++)
        {
            bts[i].Fill(skills[i]);
        }
    }
}
