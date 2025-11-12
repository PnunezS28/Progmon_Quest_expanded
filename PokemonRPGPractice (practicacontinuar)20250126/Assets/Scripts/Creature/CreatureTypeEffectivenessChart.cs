using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatureTypeEffectivenessChart
{
    //Attacker(Down) x defender(Across)
    static float[][] effectivenessChart=
    {
        //                   NEU   FIR   WAT   GRA   ELE   WIN   EAR    GHO   MET   PSY   DRA
        /*NEU*/new float[] { 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.5f},

        /*FIR*/new float[] { 1.0f, 0.5f, 0.5f, 2.0f, 1.0f, 1.0f, 0.5f, 1.0f, 2.0f, 1.0f, 1.5f},
        /*WAT*/new float[] { 1.0f, 2.0f, 0.5f, 0.5f, 0.5f, 1.0f, 1.0f, 2.0f, 1.0f, 1.0f, 1.5f},
        /*GRA*/new float[] { 1.0f, 0.5f, 2.0f, 0.5f, 1.0f, 0.5f, 1.0f, 1.0f, 1.0f, 2.0f, 1.5f},

        /*ELE*/new float[] { 1.0f, 1.0f, 2.0f, 0.5f, 1.0f, 2.0f, 0.5f, 0.5f, 1.0f, 1.0f, 1.5f},
        /*WIN*/new float[] { 1.0f, 1.0f, 1.0f, 2.0f, 0.5f, 1.0f, 2.0f, 1.0f, 0.5f, 1.0f, 1.5f},
        /*EAR*/new float[] { 1.0f, 2.0f, 1.0f, 1.0f, 2.0f, 0.5f, 1.0f, 0.5f, 1.0f, 1.0f, 1.5f},

        /*GHO*/new float[] { 1.0f, 1.0f, 0.5f, 1.0f, 1.0f, 1.0f, 2.0f, 1.0f, 0.5f, 2.0f, 2.0f},
        /*MET*/new float[] { 1.0f, 0.5f, 1.0f, 1.0f, 2.0f, 2.0f, 1.0f, 2.0f, 1.0f, 0.5f, 2.0f},
        /*PSY*/new float[] { 1.0f, 1.0f, 1.0f, 0.5f, 1.0f, 1.0f, 1.0f, 0.5f, 2.0f, 1.0f, 2.0f},

        /*DRA*/new float[] { 0f, 1.5f, 1.5f, 1.5f, 1.5f, 1.5f, 1.5f, 0.5f, 0.5f, 0.5f, 1.5f}
        };

    public static float GetEffectiveness(CreatureTypeEnum attackType,CreatureTypeEnum defenseType)
    {
        if (attackType == CreatureTypeEnum.NONE || defenseType == CreatureTypeEnum.NONE)
        {
            return 1.0f;
        }

        //Para obtener el valor int del enumerado, como NONE es el primero, hay que restar uno antes de obtener el dato
        int row = (int)attackType - 1;
        int col = (int)defenseType - 1;

        return effectivenessChart[row][col];
    }

    public static CreatureTypeEnum ParseType(string typeString)
    {
        switch (typeString)
        {
            case "NONE":
                return CreatureTypeEnum.NONE;
            case "NEUTRAL":
                return CreatureTypeEnum.NEUTRAL;
            case "FIRE":
                return CreatureTypeEnum.FIRE;
            case "WATER":
                return CreatureTypeEnum.WATER;
            case "ELECTRIC":
                return CreatureTypeEnum.ELECTRIC;
            case "WIND":
                return CreatureTypeEnum.WIND;
            case "EARTH":
                return CreatureTypeEnum.EARTH;
            case "GHOST":
                return CreatureTypeEnum.GHOST;
            case "METAL":
                return CreatureTypeEnum.METAL;
            case "PSYCHIC":
                return CreatureTypeEnum.PSYCHIC;
            case "DRAGON":
                return CreatureTypeEnum.DRAGON;
        }

        return CreatureTypeEnum.NONE;
    }
}
