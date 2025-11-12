using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LearnableSkill
{
    // Define las habilidades obtenibles por una ciratura
    [SerializeField] SkillBase skillBase;
    [SerializeField] int level;

    public SkillBase SkillBase
    {
        get { return skillBase; }
    }

    public int Level
    {
        get { return level; }
    }
}
