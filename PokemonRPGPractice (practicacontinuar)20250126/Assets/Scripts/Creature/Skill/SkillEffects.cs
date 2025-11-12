using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Clase que contiene los datos de efectos de skills de Status
[System.Serializable]
public class SkillEffects
{
    [NonReorderable]
    [SerializeField] List<StatBoostEffect> boosts;

    //[SerializeField] CreatureStat stat;
    //[SerializeField] int boostLevel;
    [SerializeField] ConditionID status;
    [SerializeField] ConditionID volatileStatus;

    public List<StatBoostEffect> Boosts
    {
        get {
            return boosts;
        }
    }
    public ConditionID Status
    {
        get
        {
            return status;
        }
    }
    public ConditionID VolatileStatus
    {
        get
        {
            return volatileStatus;
        }
    }
}
