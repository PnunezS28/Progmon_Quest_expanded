using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BattleParticleFXID { 
    NONE,
    BUFF,
    DEBUFF,
    APPLY_EFFECT,
    PHYSICAL,
    SPECIAL,
    NEUTRAL,
    FIRE,
    WATER,
    GRASS,
    ELECTRIC,
    WIND,
    EARTH,
    GHOST,
    METAL,
    PSYCHIC,
    DRAGON
};
//Contiene animaciones genéricas
[System.Serializable]
public class BattleParticleFX
{
    public BattleParticleFXID id=BattleParticleFXID.NONE;
    public ParticleFXContainer ParticleFXPrefab;
}
