using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryButton : MonoBehaviour
{
    [SerializeField]
    private Text itemText;

    [SerializeField]
    private Text descriptionText;

    [SerializeField]
    private Image icon;

    [SerializeField]
    private CanvasGroup grp;

    public void Fill(ItemInstance inst)
    {
        if(inst == null)
        {
            itemText.text = "";
            grp.alpha = 0;
            return;
        }

        if(ItemsManager.Instance.GetItemData(inst.id, out ItemData data))
        {
            icon.sprite = data.icon;
            itemText.text = data.name;
            icon.GetComponent<CanvasGroup>().alpha = 0;
            //descriptionText.text = data.description;
            grp.alpha = 1;
        }
        else 
        {
            grp.alpha = 0;
        }
    }
}
