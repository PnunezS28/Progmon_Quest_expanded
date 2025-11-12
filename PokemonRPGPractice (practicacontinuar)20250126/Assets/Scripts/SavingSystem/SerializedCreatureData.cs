using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SerializedCreatureData
{
    public int _baseId; //cambiar a string nombre de recurso
    public int level;
    public int HP { get; set; }//Puntos de vida actuales

    public int Exp { get; set; }

    public List<SerializedSkillData> Skills { get; set; } //hacer SkillData

    public ConditionID conditionId;

    public BattlePassiveID equippedAbility;
    public int equippedItemid;

    public SerializedCreatureData(Creature creature)
    {
        //Guarda usando el id único de la base
        this._baseId = creature.Base.creatureDexId;
        this.level = creature.Level;
        this.HP = creature.HP;
        this.Exp = creature.Exp;
        //simpre nunca es null
        if (creature.Status != null)
        {
            this.conditionId = creature.Status.Id;
        }
        else
        {
            this.conditionId = ConditionID.none;
        }
        Skills = new List<SerializedSkillData>();
        
        //convertir skills
        foreach(Skill s in creature.Skills)
        {
            this.Skills.Add(new SerializedSkillData(s));
        }//no se actualiza correctamente
        this.equippedItemid = creature.EquipedItemId;
        if (creature.EquipAbility != null)
        {
            this.equippedAbility = creature.EquipAbility.Id;
        }

    }
}
