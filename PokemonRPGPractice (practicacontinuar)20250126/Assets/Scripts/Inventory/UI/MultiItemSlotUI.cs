using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MultiItemSlotUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI itemName;

    public void SetData(ItemBase item)
    {
        itemName.text = item.ItemName;
    }
}
