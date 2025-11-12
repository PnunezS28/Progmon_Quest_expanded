using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ItemSlot
{
    [SerializeField] ItemBase item;
    [SerializeField] int countInPossesion;

    public ItemBase Item
    {
        get => item;
        set => item = value;
    }

    public int CountInPossesion
    {
        get => countInPossesion;
        set => countInPossesion = value;
    }

    public ItemSlot()
    {
        //constructor por defecto
    }

    public ItemSlot(SerializedItemData data)
    {
        this.item = CreatureDexHandler.i.GetItemBase(data.itemBaseId);
        this.countInPossesion = data.countInPossesion;
    }
}
