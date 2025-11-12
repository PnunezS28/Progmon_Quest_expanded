using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BattlePassiveID { ImEmFuego,ImEmAgua,ImEmPlanta,DoublePower,Intimidate,RoughSkin,Isolation,AntiPoison,Medicine,PriceDisk}

public class BattlePassiveDB
{
    // plain C# class que lista las habilidades de batalla pasivas equipables en un diccionario
    //TODO: añadir lista de objetos equipables con un get objeto por battlePassiveId
    public static void Init()
    {
        foreach(var kvp in BattlePassives)
        {
            var passiveId = kvp.Key;
            var battlePassive = kvp.Value;
            battlePassive.Id = passiveId;
        }
        Debug.Log("BattlePassiveDB Initialized");
    }

    public static Dictionary<BattlePassiveID, BattlePassive> BattlePassives { get; set; } = new Dictionary<BattlePassiveID, BattlePassive>()
    {
        {
            BattlePassiveID.ImEmFuego,
            new BattlePassive(){
                Name="Impulso Emergencia (Fuego)",
                Description="Si el usuario es de tipo fuego y sus PS están a 1/3 o menos aumenta la potencia de los ataques de fuego en 20%.",
                IsOneUse=false,
                OnSkillUsed=(BattleArgs args) =>
                {
                    Creature holder=args.Holder;
                    Debug.Log($"Checking ImEmFuego OnSkillUsed for {args.Holder}");
                    if ((holder.Base.Type1 == CreatureTypeEnum.FIRE || holder.Base.Type2 == CreatureTypeEnum.FIRE)&& args.UsedSkill.SkillBase.Type==CreatureTypeEnum.FIRE)
                    {
                        Debug.Log("Habilidad activado un aumento del 20% se aplica al modificador de daño");
                        float mod=Mathf.FloorToInt( args.modifiers+(args.modifiers*20)/100);
                        Debug.Log($"Modificadores original {args.modifiers},ModTotal {mod}");
                        return mod;
                    }
                    return args.modifiers;
                }
            }
        },
        {
            BattlePassiveID.ImEmAgua,
            new BattlePassive(){
                Name="Impulso Emergencia (Agua)",
                Description="Si el usuario es de tipo agua y sus PS están a 1/3 o menos aumenta la potencia de los ataques de agua en 20%.",
                IsOneUse=false,
                OnSkillUsed=(BattleArgs args) =>
                {
                    Creature holder=args.Holder;
                    Debug.Log($"Checking ImEmAgua OnSkillUsed for {args.Holder}");
                    if ((holder.Base.Type1 == CreatureTypeEnum.WATER || holder.Base.Type2 == CreatureTypeEnum.WATER)&& args.UsedSkill.SkillBase.Type==CreatureTypeEnum.WATER)
                    {
                        Debug.Log("Habilidad activado un aumento del 20% se aplica al modificador de daño");
                        float mod=Mathf.FloorToInt( args.modifiers+(args.modifiers*20)/100);
                        Debug.Log($"Modificadores original {args.modifiers},ModTotal {mod}");
                        return mod;
                    }
                    return args.modifiers;
                }
            }
        },
        {
            BattlePassiveID.ImEmPlanta,
            new BattlePassive(){
                Name="Impulso Emergencia (Planta)",
                Description="Si el usuario es de tipo planta y sus PS están a 1/3 o menos aumenta la potencia de los ataques de planta en 20%.",
                IsOneUse=false,
                OnSkillUsed=(BattleArgs args) =>
                {
                    Creature holder=args.Holder;
                    Debug.Log($"Checking ImEmPlanta OnSkillUsed for {args.Holder}");
                    if ((holder.Base.Type1 == CreatureTypeEnum.GRASS || holder.Base.Type2 == CreatureTypeEnum.GRASS)&& args.UsedSkill.SkillBase.Type==CreatureTypeEnum.GRASS)
                    {
                        Debug.Log("Habilidad activado un aumento del 20% se aplica al modificador de daño");
                        float mod=Mathf.FloorToInt( args.modifiers+(args.modifiers*20)/100);
                        Debug.Log($"Modificadores original {args.modifiers},ModTotal {mod}");
                        return mod;
                    }
                    return args.modifiers;
                }
            }
        },
        {
            BattlePassiveID.DoublePower,
            new BattlePassive()
            {
                Name="Potencia doble",
                Description="Si el usuario es de tipo neutral, duplica la estadística de ataque físico.",
                IsOneUse=false,
                OnStatGet=(Creature Holder,CreatureStat statGet,int originalStatValue) =>
                {
                    if ((Holder.Base.Type1 == CreatureTypeEnum.NEUTRAL || Holder.Base.Type2 == CreatureTypeEnum.NEUTRAL)&&statGet==CreatureStat.ATTACK)
                    {
                        return originalStatValue*2;
                    }
                    else
                    {
                        return originalStatValue;
                    }
                }
            }
        },
        {
            BattlePassiveID.Intimidate,
            new BattlePassive()
            {
                Name="Entrada bruta",
                Description="Al entrar en combate reduce el ataque del oponente un nivel.",
                IsOneUse=false,
                OnEnter=(BattleArgs args) =>
                {
                    args.Holder.StatusChanges.Enqueue($"Entrada bruta de {args.Holder.Base.Name} se activó");
                    args.Opponent.ApplyBoosts(new List<StatBoostEffect>(){ new StatBoostEffect(CreatureStat.ATTACK,-1)});
                }
            }
        },
        {
            BattlePassiveID.RoughSkin,
            new BattlePassive()
            {
                Name="Toque desgastador",
                Description="Al recivir daño físico aplica daño ligero al oponente",
                IsOneUse=false,
                AfterGetHit=(BattleArgs args) =>
                {
                    Debug.Log($"Checking AfterGetHit para {args.Holder.Base.Name}");
                    if (args.UsedSkill.SkillBase.Category == SkillCategory.PHYSICAL)
                    {
                        args.Holder.StatusChanges.Enqueue($"Toque desgastador de {args.Holder.Base.Name} se activó");
                        Debug.Log($"Toque desgastador activado realizando daño 12.5% de los maximos del oponente");
                        int damage=Mathf.FloorToInt( (float)((args.Opponent.MaxHP*12.5)/100));
                        Debug.Log($"Realizando daño: {damage}");
                        args.Opponent.DecreaseHP(damage);
                        args.Opponent.StatusChanges.Enqueue($"Toque desgastador de {args.Holder.Base.Name} hizo daño a su oponente");
                    }
                }
            }
        },
        {
            BattlePassiveID.Isolation,
            new BattlePassive()
            {
                Name="Aislador",
                Description="Reduce a la mitad daño recivido de ataques de agua y fuego",
                IsOneUse=false,
                OnDamagetaken=(BattleArgs args,int originalDamage) =>
                {
                    Debug.Log($"Checking OnDamagetaken para {args.Holder.Base.Name}");
                    if (args.UsedSkill.SkillBase.Type==CreatureTypeEnum.FIRE||args.UsedSkill.SkillBase.Type==CreatureTypeEnum.WATER)
                    {
                        Debug.Log($"Aislador reduciendo a la mitad el daño recivido");
                        int damage=Mathf.FloorToInt( originalDamage/2);
                        Debug.Log($"Daño total: {damage}");
                        return damage;
                    }
                    return originalDamage;
                }
            }
        },
        {
            BattlePassiveID.AntiPoison,
            new BattlePassive()
            {
                Name="Antiveneno",
                Description="Impide recivir el estatus de veneno",
                IsOneUse=false,
                OnStatusRecieved=(BattleArgs args) =>
                {
                    if (args.Holder.Status.Id == ConditionID.PSN)
                    {
                        args.Holder.CureStatus();
                        args.Holder.StatusChanges.Enqueue($"Antiveneno protegió a {args.Holder.Base.Name}");
                        return true;
                    }
                    return false;
                }
            }
        },
        {
            BattlePassiveID.Medicine,
            new BattlePassive()
            {
                Name="Medicina",
                Description="Cura los estados una sola vez en combate",
                IsOneUse=true,
                OnTurnEnd=(BattleArgs args) =>
                {
                    Debug.Log($"Checking OnTurnEnd para {args.Holder.Base.Name}");
                    if (args.Holder.OneUseItemUsed==false && (args.Holder.VolatileStatus!=null || args.Holder.Status!=null))
                    {
                        Debug.Log($"Medicina activada, curando estados anómalos");
                        args.Holder.CureStatus();
                        args.Holder.CureVolatileStatus();
                        args.Holder.OneUseItemUsed=true;
                        args.Holder.StatusChanges.Enqueue($"{args.Holder.Base.Name} usó medicina para curar su estado anómalo");
                    }
                }
            }
        },
        {
            BattlePassiveID.PriceDisk,
            new BattlePassive(){
                Name="Price Disk",
                Description="Posee un objeto de alto valor monetario",
                IsOneUse=false,
                OnEnter=(BattleArgs args) =>
                {
                    Creature holder=args.Holder;
                    Debug.Log($"OnEnter for ability Price Disk for {args.Holder}");
                    holder.StatusChanges.Enqueue($"¡{holder.Base.Name} tiene un objeto brillante!");
                }
            }
        }
    };
}
