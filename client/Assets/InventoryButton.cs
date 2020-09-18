using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryButton : MonoBehaviour
{
    [SerializeField]
    private Text itemText;

    public void Fill(ItemData data)
    {
        if(data == null)
        {
            itemText.text = "";
            return;
        }

        itemText.text = data.id.ToString();
    }
}
