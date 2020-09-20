using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;

public class ItemHoverPanel : Singleton<ItemHoverPanel>
{
    public UIPanel panel;

    [SerializeField]
    private Text nameText;

    [SerializeField]
    private Text descriptionText;

    private void Awake()
    {
        Instance = this;
    }

    public void Fill(ItemData data)
    {
        nameText.text = data.name;
        descriptionText.text = data.description;
    }

    private void LateUpdate()
    {
        if(panel.Active)
        {
            panel.transform.position = Input.mousePosition;
        }
    }
}
