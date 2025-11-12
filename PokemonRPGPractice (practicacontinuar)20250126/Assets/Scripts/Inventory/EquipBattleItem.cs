using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/new Equip Battle Item")]
public class EquipBattleItem : ItemBase
{
    [SerializeField] BattlePassiveID equipmentEffect;

    public override bool Use(Creature creature)
    {
        Debug.Log("EQUIP_ITEM. EquipItem was Used");
        if (creature.EquipedItemId == -1)
        {
            creature.SetEquipedAbility(this);
            return true;
        }
        return false;
    }
    public override bool CanUseInBattle => false;
    public BattlePassiveID EquipmentEffect => equipmentEffect;
}
