using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SerializedSkillData
{
    public int skillId;
    public int uses;

    public SerializedSkillData(Skill skill)
    {
        this.skillId = skill.SkillBase.skillId;
        this.uses = skill.Uses;
    }
}
