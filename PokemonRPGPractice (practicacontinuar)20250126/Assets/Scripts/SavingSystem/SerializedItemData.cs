using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SerializedItemData
{
    public int itemBaseId;
    public int countInPossesion;
    
    public SerializedItemData(ItemSlot slot)
    {
        this.itemBaseId = slot.Item.ItemDexId;
        this.countInPossesion = slot.CountInPossesion;
    }
}
