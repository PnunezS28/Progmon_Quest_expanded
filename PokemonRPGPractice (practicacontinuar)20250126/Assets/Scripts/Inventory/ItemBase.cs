using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemBase : ScriptableObject
{
    //clase base para todos los objetos del jugador
    [SerializeField] int itemDexId;
    [SerializeField] string itemName;
    [TextArea(3,5)]
    [SerializeField] string itemDescription;
    [SerializeField] string itemUsedText;
    [SerializeField] Sprite itemIcon;
    [Min(0)]
    [SerializeField] float shopPrice = 1;
    [SerializeField] bool isSellable = true;
    
    public string ItemName => itemName;

    public string ItemDescription => itemDescription;
    public string ItemUsedText => itemUsedText;


    public Sprite ItemIcon => itemIcon;

    public int ItemDexId => itemDexId;

    public float ShopPrice => shopPrice;

    public bool IsSellable => isSellable;

    public virtual bool Use(Creature creature)//Implementar desde subclase
    {
        //devuelve booleano indicando si se ha usado o no
        return false;
    }

    public virtual bool CanUseInBattle => true;
    public virtual bool CanUseOutsideBattle => true;
}