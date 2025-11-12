using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "Items/new Key Item")]
public class KeyItem : ItemBase
{

    public override bool CanUseInBattle => false;
    public override bool CanUseOutsideBattle => false;
}
