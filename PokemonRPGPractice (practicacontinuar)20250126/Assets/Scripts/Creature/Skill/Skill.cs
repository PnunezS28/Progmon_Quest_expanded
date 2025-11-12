using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Skill
{
    // Contains data of muves tha will affect in battle
    public SkillBase SkillBase { get; set; }  //cambiar a string nombre de recurso
    public int Uses { get; set; }

    public Skill(SkillBase @base)
    {
        SkillBase = @base;
        Uses = @base.MaxUses;
    }

    public Skill(SerializedSkillData data)
    {
        SkillBase = CreatureDexHandler.i.GetSkillBase(data.skillId);
        this.Uses = data.uses;
    }

    public void IncreaseUses(int amount)
    {
        Uses = Mathf.Clamp(Uses+amount, 0, SkillBase.MaxUses);
        Debug.Log($"La habilidad {SkillBase.SkillName} ha recuperado {amount} usos");
    }
}
