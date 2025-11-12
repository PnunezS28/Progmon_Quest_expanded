using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConditionsDB
{
    // plain C# class que lista las condiciones en un diccionario

    public static void Init()
    {
        //inicializador para introducir el id de cada estado en su objeto para qu sea accesible
        foreach(var kvp in Conditions)
        {
            var conditionId = kvp.Key;
            var condition = kvp.Value;
            condition.Id = conditionId;
        }
        Debug.Log("Initialized ConditionsDB");
    }

    public static Dictionary<ConditionID, Condition> Conditions { get; set; } = new Dictionary<ConditionID, Condition>()
    {
        {
            ConditionID.PSN,
            new Condition()
            {
                Name="Poison",
                StartMessage="ha sido envenenado",
                OnAfterTurn=(Creature creature) =>
                {//Los efectos de veneno es restar los hp de la criatura por una pequeña cantidad, 1/8 parte de sus HPMax
                    int poisonDamage=creature.MaxHP/8;
                    if (poisonDamage == 0)
                    {
                        poisonDamage=1;
                    }
                    creature.DecreaseHP(poisonDamage);
                    creature.StatusChanges.Enqueue($"{creature.Base.Name} perdió vida por veneno");
                    Debug.Log($"{creature.Base.name} lost {poisonDamage} from poisoned status");
                }
            }
        },

        {
            ConditionID.BRN,
            new Condition()
            {
                Name="Burn",
                StartMessage="ha sido quemado",
                OnAfterTurn=(Creature creature) =>
                {//Los efectos de quemado es restar los hp de la criatura por una pequeña cantidad, 1/16 parte de sus HPMax
                    //este estado también modifica ligeramente el daño realizado, TODO: Implementar
                    int burnDamage=creature.MaxHP/16;
                    if (burnDamage == 0)
                    {
                        burnDamage=1;
                    }
                    creature.DecreaseHP(burnDamage);
                    creature.StatusChanges.Enqueue($"{creature.Base.Name} se resiente las quemaduras");
                    Debug.Log($"{creature.Base.name} lost {burnDamage} from burned status");
                }
            }
        },

        {
            ConditionID.PAR,
            new Condition()
            {
                Name="Paralyzed",
                StartMessage="ha sido paralizado",
                OnBeforeSkill=(Creature creature)=>{
                    //Una criatura paralizada no podrá realizar su skil 1/4 de las veces
                    //devolver false significa que la criatura no podrá realizar su skill
                    int r=Random.Range(1, 5);
                    if ( r== 1)
                    {
                        creature.StatusChanges.Enqueue($"{creature.Base.Name} no puede moverse por parálisis");
                        Debug.Log($"{creature.Base.name} couldn't perform their skill due to paralyzed status");
                        return false;
                    }


                    return true;
                }
            }
        },

        {
            ConditionID.FRZ,
            new Condition()
            {
                Name="Frozen",
                StartMessage="ha sido congelado",
                OnBeforeSkill=(Creature creature)=>{
                    //Una criatura congelada no podrá realizar su skill
                    //Antes de atacar intentará curarse
                    int r=Random.Range(1, 5);
                    if ( r== 1)
                    {
                        creature.CureStatus();
                        creature.StatusChanges.Enqueue($"¡{creature.Base.Name} se liberó del hielo!");
                        Debug.Log($"{creature.Base.name} cured itself from freeze");
                        return true;
                    }

                    creature.StatusChanges.Enqueue($"{creature.Base.Name} está congelado y no puede atacar");
                        Debug.Log($"{creature.Base.name} cannot attack due to frozen status");

                    return false;
                }
            }
        },

        {
            ConditionID.SLP,
            new Condition()
            {
                Name="Sleep",
                StartMessage="está dormido",
                OnStart=(Creature creature)=>{
                    //Duerme aleatoriamente un número de turnos entre 1 y 3
                    creature.StatusTime=Random.Range(1,4);
                    Debug.Log($"{creature.Base.Name} has fallen asleep and will not attack for {creature.StatusTime} turns");
                },
                OnBeforeSkill=(Creature creature)=>{
                    //Una criatura dormida no podrá realizar su skill
                    //pasado un número de turnos se curará su estado
                    if (creature.StatusTime <= 0)
                    {
                        creature.CureStatus();
                        creature.StatusChanges.Enqueue($"¡{creature.Base.Name} despertó y puede continuar!");
                        return true;
                    }
                    
                    creature.StatusTime--;
                    Debug.Log($"{creature.Base.Name} will be asleep for {creature.StatusTime} more turns");
                    creature.StatusChanges.Enqueue($"{creature.Base.Name} está dormido y no puede atacar");
                    return false;
                }
            }
        },
        //Volatile status conditions

        {
            ConditionID.CONFUSION,
            new Condition()
            {
                Name="Confusion",
                StartMessage="está confuso",
                OnStart=(Creature creature)=>{
                    //Duerme aleatoriamente un número de turnos entre 1 y 3
                    creature.VolatileStatusTime=Random.Range(1,4);
                    Debug.Log($"{creature.Base.Name} has been confused and may hurt itself for {creature.VolatileStatusTime} turns");
                },
                OnBeforeSkill=(Creature creature)=>{
                    //Una criatura dormida no podrá realizar su skill
                    //pasado un número de turnos se curará su estado
                    if (creature.VolatileStatusTime <= 0)
                    {
                        creature.CureVolatileStatus();
                        creature.StatusChanges.Enqueue($"¡{creature.Base.Name} recuperó el sentido!");
                        return true;
                    }

                    creature.VolatileStatusTime--;
                    //50% chance to use skill
                    if (Random.Range(1, 3) == 1)
                    {
                        return true;
                    }

                    creature.StatusChanges.Enqueue($"{creature.Base.Name} está confuso");
                    int confusionDamage=creature.MaxHP/8;
                    creature.DecreaseHP(confusionDamage);

                    Debug.Log($"{creature.Base.Name} lost {confusionDamage} HP from confusion");
                    Debug.Log($"{creature.Base.Name} will be confused for {creature.VolatileStatusTime} more turns");
                    creature.StatusChanges.Enqueue($"Se hizo daño a sí mismo");
                    return false;
                }
            }
        }
    };

    public static float GetCatchStatusBonus(Condition condition)
    {
        if (condition == null)
        {
            return 1f;
        }else if (condition.Id == ConditionID.SLP || condition.Id==ConditionID.FRZ)
        {
            return 2f;
        }else if(condition.Id == ConditionID.PAR|| condition.Id == ConditionID.PSN|| condition.Id == ConditionID.BRN)
        {
            return 1.5f;
        }
        return 1f;
    }
}
