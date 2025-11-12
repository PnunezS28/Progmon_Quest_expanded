using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleArgs
{
    public Creature Holder { get; set; }//el poseedor de la habilidad
    public Skill UsedSkill { get; set; }//el ataque que utilizó el poseedor de la habilidad o el oponente en base al contexto
    public float modifiers { get; set; }//Los modificadores de daño del ataque usado
    public Creature Opponent { get; set; }//El oponente, siendo el objetivo de la criatura que posee la habilidad

    public DamageDetails damageDetails { get; set; }
    //Permite usar habilidades en base a efectividad de tipo o crítico

    public BattleArgs(Creature holder, float modifiers=1, Skill usedSkill = null, Creature Oponent = null, DamageDetails damageDetails=null)
    {
        this.Holder = holder;
        this.UsedSkill = usedSkill;
        this.modifiers = modifiers;
        this.Opponent = Oponent;
        this.damageDetails = damageDetails;
    }

}
