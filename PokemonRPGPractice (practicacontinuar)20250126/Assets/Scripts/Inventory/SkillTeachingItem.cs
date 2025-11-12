using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/new Skill Teaching Item")]
public class SkillTeachingItem : ItemBase
{
    //Este objeto puede añadir nuevas habilidades a una criatura
    [SerializeField] SkillBase skill;

    public SkillBase Skill => skill;

    public override bool Use(Creature creature)
    {
        //Learning move se maneja desde InventoryUI, devolver true si se obtuvo el movimiento

        return creature.HasSkill(skill);
    }

    public bool CanBeTaught(Creature creature)
    {
        return creature.Base.LearnableSkillsByItem.Contains(Skill);
    }

    public override bool CanUseInBattle => false;
}
