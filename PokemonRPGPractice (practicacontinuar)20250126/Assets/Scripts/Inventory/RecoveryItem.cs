using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="Items/new Recovery Item")]
public class RecoveryItem : ItemBase
{
    [Header("Health Point restore")]
    //Un objeto diseñado para restaurar la salud de una criatura, IE. pociones
    [Min(0)] [SerializeField] int hpRecoverAmount;
    //Determina si recupera la salud entera de una criatura
    [SerializeField] bool restoreMaxHp;

    [Header("Skill use restore")]
    //recupera usos habilidad de critura, IE ether
    [Min(0)] [SerializeField] int useRecoverAmount;
    //Determina si recupera los usos de habilidad enteros de una criatura
    [SerializeField] bool restoreMaxUses;

    [Header("Creature status condition restore")]
    //Cura los estados anomalos de una criatura
    [SerializeField] ConditionID status;
    //Cura todos los estado anomalos y volátiles de una criatura
    [SerializeField] bool recoverAllStatus;

    [Header("Creature revival")]
    //restaura criaturas debilitadas
    [SerializeField] bool revive;
    //restaura criaturas debilitadas al máximo
    [SerializeField] bool MaxRevive;

    public override bool Use(Creature creature)
    {
        //revivir criaturas
        if (revive || MaxRevive)
        {
            //verificar si criatura está devilitada
            if (creature.HP > 0)
            {
                return false;//no puede usarse
            }
            if (revive)
            {//implementar diferentes grados de revivir
                creature.IncreaseHP(creature.MaxHP/2);
            }else if (MaxRevive)
            {
                creature.IncreaseHP(creature.MaxHP);
            }

            creature.CureStatus();
            creature.CureVolatileStatus();

            return true;
        }

        //a partir de aquí los objetos deben ser usados en criaturas aún con HP
        if (creature.HP == 0)
        {
            return false;
        }
        //recuperar vida
        if (hpRecoverAmount > 0 || restoreMaxHp)
        {
            if (creature.HP == creature.MaxHP)
            {
                //no pude usarse porque está lleno
                return false;
            }if (restoreMaxHp)
            {
                creature.IncreaseHP(creature.MaxHP);

            }
            else
            {
                creature.IncreaseHP(hpRecoverAmount);

            }
        }

        //recuperar status
        if (recoverAllStatus || status != ConditionID.none)
        {
            if(creature.Status==null && creature.VolatileStatus == null)
            {
                return false;
            }

            if (recoverAllStatus)
            {
                creature.CureStatus();
                creature.CureVolatileStatus();
            }
            else
            {
                //curasr estado individual
                if (creature.Status.Id == status)
                {
                    creature.CureStatus();
                }else if (creature.VolatileStatus != null)
                {//falla si este es null
                    if(creature.VolatileStatus.Id==status)
                        creature.CureVolatileStatus();
                }
                else
                {
                    return false;
                }
            }

        }

        //restore uses
        //normalmente restauraría un solo movimiento impementar si es posible en otro momento

        if (restoreMaxHp)
        {
            //restaura usos
            creature.Skills.ForEach(m => m.IncreaseUses(m.SkillBase.MaxUses));
        }else if (useRecoverAmount > 0)
        {
            creature.Skills.ForEach(m => m.IncreaseUses(useRecoverAmount));
        }

        
        return true;

    }

}
