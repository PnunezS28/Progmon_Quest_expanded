using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//clase que contiene datos para impulsar o reducir estadísticas de criatura
//TODO: fallo para mostrarse en el inspector
[System.Serializable]
public class StatBoostEffect
{
    public CreatureStat stat;
    public int boostLevel;

    public StatBoostEffect()
    {

    }
    
    public StatBoostEffect(CreatureStat stat,int boostLevel)
    {
        this.stat = stat;
        this.boostLevel = boostLevel;
    }

    public CreatureStat Stat
    {
        get
        {
            return stat;
        }
        set { stat = value; }
    }

    public int BoostLevel
    {
        get
        {
            return boostLevel;
        }
        set
        {
            boostLevel = value;
        }
    }

}
