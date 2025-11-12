using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ItemSlotUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI itemName;
    [SerializeField] TextMeshProUGUI itemCount;

    RectTransform rectTransformUI;

    public void SetData(ItemSlot itemSlot)
    {
        rectTransformUI = GetComponent<RectTransform>();
        itemName.text = itemSlot.Item.ItemName;
        itemCount.text = $"X {itemSlot.CountInPossesion}";
    }

    public void SetShopData(ItemBase item)
    {
        rectTransformUI = GetComponent<RectTransform>();
        itemName.text = item.ItemName;
        itemCount.text = item.ShopPrice + " G";
    }


    public void SetColor(Color color)
    {
        itemName.color = color;
        itemCount.color = color;
    }

    public float Height => rectTransformUI.rect.height;
}
