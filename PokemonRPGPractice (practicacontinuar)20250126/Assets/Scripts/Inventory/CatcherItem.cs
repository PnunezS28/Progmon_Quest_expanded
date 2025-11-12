using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/new Catcher Item")]
public class CatcherItem : ItemBase
{
    [SerializeField] float catchRateModifier=1;
    public override bool Use(Creature creature)
    {
        
        return true;
    }

    public override bool CanUseOutsideBattle => false;

    public float CatchRateModifier => catchRateModifier;
}
