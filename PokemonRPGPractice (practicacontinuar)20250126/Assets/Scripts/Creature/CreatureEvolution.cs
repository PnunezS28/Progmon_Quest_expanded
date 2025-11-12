using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EvoTypeEnum { NONE,EVOLUTION,REGRESSION,MODECHANGE}

[System.Serializable]
public class CreatureEvolution
{
    [SerializeField] CreatureBase evoCreature;
    [SerializeField] string[] evoConditions;
    [SerializeField] EvoTypeEnum evoType=EvoTypeEnum.NONE;

    public CreatureBase Creature { get { return evoCreature; } }
    public string[] EvoConditions { get {  return evoConditions; } }
    public EvoTypeEnum EvoType { get {  return evoType; } }

    /*
     EvoConditions Format
    "atributo;valor"
    atributo:
    stats criatura (ATTACK,DEFENSE,SP_ATTACK,SP_DEFENSE,SPEED,LEVEL):
    - solo igual o mayor
    skill (SKILL_ID,SKILL_TYPE):
    - ID: requiere un id específico
    - type: requiere un skill con tipo de criatura concreto
    Equip (ITEM):
    - ID: requiere un determinado objeto equipado
    Se tienen que cumplir todas las condiciones
     */
    public bool ConditionIsTrue(Creature creature,int conditionIndex)
    {
        if(evoType == EvoTypeEnum.NONE) { return false; }
        if(evoType==EvoTypeEnum.REGRESSION) {  return true; }

        string[] conditionTokens= EvoConditions[conditionIndex].Split(';');
        string atributo = conditionTokens[0];
        string valorObjetivo = conditionTokens[1];

        int checkedValue;


        switch (atributo)
        {
            case "ATTACK":
                checkedValue = creature.Attack;
                return checkedValue>= int.Parse(valorObjetivo);
            case "DEFENSE":
                checkedValue = creature.Defense;
                return checkedValue >= int.Parse(valorObjetivo);
            case "SP_ATTACK":
                checkedValue = creature.SpAttack;
                return checkedValue >= int.Parse(valorObjetivo);
            case "SP_DEFENSE":
                checkedValue = creature.SpDefense;
                return checkedValue >= int.Parse(valorObjetivo);
            case "SPEED":
                checkedValue = creature.Speed;
                return checkedValue >= int.Parse(valorObjetivo);
            case "LEVEL":
                checkedValue = creature.Level;
                return checkedValue >= int.Parse(valorObjetivo);
            case "ITEM":
                checkedValue = creature.EquipedItemId;
                return checkedValue == int.Parse(valorObjetivo);
            case "SKILL_ID":
                foreach (Skill skill in creature.Skills)
                {
                    if(skill.SkillBase.skillId== int.Parse(valorObjetivo))
                    {
                        return true;
                    }
                }
                break;
            case "SKILL_TYPE":
                CreatureTypeEnum targetType=CreatureTypeEffectivenessChart.ParseType(valorObjetivo);
                foreach (Skill skill in creature.Skills)
                {
                    if (skill.SkillBase.Type==targetType)
                    {
                        return true;
                    }
                }
                break;

        }
        return false;
    }

    public string GetPrettyConditionString(Creature thisCreature,bool discovered=false)
    {
        string prettyText = "";

        if (evoType == EvoTypeEnum.NONE) { return "???"; }
        if (evoType == EvoTypeEnum.REGRESSION) { return "LEVEL >= 1"; }

        for (int i=0;i<evoConditions.Length;i++)
        {
            if ((ConditionIsTrue(thisCreature, i) == false) && discovered==false)
            {
                prettyText += "???\n";
            }
            else
            {
                string cond = evoConditions[i];
                string[] conditionTokens = cond.Split(';');
                string atributo = conditionTokens[0];
                string valorObjetivo = conditionTokens[1];


                switch (atributo)
                {
                    case "ATTACK":
                        prettyText += "ATTACK >= " + valorObjetivo + "\n";
                        break;
                    case "DEFENSE":
                        prettyText += "DEFENSE >= " + valorObjetivo + "\n";
                        break;
                    case "SP_ATTACK":
                        prettyText += "SP_ATTACK >= " + valorObjetivo + "\n";
                        break;
                    case "SP_DEFENSE":
                        prettyText += "SP_DEFENSE >= " + valorObjetivo + "\n";
                        break;
                    case "SPEED":
                        prettyText += "SPEED >= " + valorObjetivo + "\n";
                        break;
                    case "LEVEL":
                        prettyText += "LEVEL >= " + valorObjetivo + "\n";
                        break;
                    case "ITEM":
                        ItemBase searchedItem = CreatureDexHandler.i.GetItemBase(int.Parse(valorObjetivo));
                        prettyText += "Equipado con " + searchedItem.ItemName + "\n";
                        break;
                    case "SKILL_ID":
                        SkillBase searchedSkill = CreatureDexHandler.i.GetSkillBase(int.Parse(valorObjetivo));
                        prettyText += "Puede usar " + searchedSkill.SkillName + "\n";
                        break;
                    case "SKILL_TYPE":
                        prettyText += "Tiene skill tipo " + valorObjetivo + "\n";
                        break;
                    default:
                        prettyText += "???\n";
                        break;
                }

            }

            
        }
        
        return prettyText;
    }
    public bool CanEvolve(Creature creature)
    {
        for(int i=0;i<evoConditions.Length;i++)
        {
            if (ConditionIsTrue(creature, i) == false)
            {
                return false;
            }
        }
        return true;
    }
}
