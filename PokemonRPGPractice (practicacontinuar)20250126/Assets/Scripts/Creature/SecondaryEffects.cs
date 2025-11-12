using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SecondaryEffects : SkillEffects
{
    [SerializeField] int triggerChance;
    [SerializeField] SkillTargetEnum target;
    public int TriggerChance
    {
        get
        {
            return triggerChance;
        }
    }
    public SkillTargetEnum Target
    {
        get
        {
            return target;
        }
    }
}
