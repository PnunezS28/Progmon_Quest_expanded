using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName ="Creature", menuName ="Creature Data/ New creature")]
public class CreatureBase : ScriptableObject
{
    [SerializeField] public int creatureDexId;
    public int displayCreatureId;
    
    //Clase que permite generar scriptableObjects con los datos de las criaturas creadas
    [SerializeField] string creatureName;
    [TextArea]
    [SerializeField] string description;
    [SerializeField] Sprite battleSprite;
    [SerializeField] CreatureTypeEnum type1=CreatureTypeEnum.NONE;
    [SerializeField] CreatureTypeEnum type2=CreatureTypeEnum.NONE;//Valor por defecto

    //BaseStats
    [Header("Base Stats")]
    [Min(1)][SerializeField] int maxHP;
    [Min(1)] [SerializeField] int attack;
    [Min(1)] [SerializeField] int defense;
    [Min(1)] [SerializeField] int spAttack;
    [Min(1)] [SerializeField] int spDefense;
    [Min(1)] [SerializeField] int speed;

    //Valor entre 0 (dificil) y 255 (facil)
    [Range(0,255)] [SerializeField] int catchRate=255;
    //Cantidad de experiencia aportada a una criatura cuando derrota a esta especie
    [Min(0)]
    [SerializeField] int expYield=100;

    [Header("Growth")]
    [SerializeField] GrowthRateEnum growthRate=GrowthRateEnum.MediumFast;

    [SerializeField] List<CreatureEvolution> evolutions=new List<CreatureEvolution>();

    [SerializeField] List<LearnableSkill> learnableSkills;
    [SerializeField] List<SkillBase> learnableSkillsByItems;

    public static int MaxNumOfSkills { get; set; } = 4;
    public static int MaxCreatureLevel { get; set; } = 100;
    //TODO: implementar nivel máximo, establecer a 100

    public int GetExpForLevel(int level)
    {
        if (growthRate == GrowthRateEnum.Fast)
        {
            return 4 * (level * level * level) / 5;
        }
        else if (growthRate == GrowthRateEnum.MediumFast)
        {
            return level* level *level;
        }

        //devuelve error en este caso
        return -1;
    }

    #region Properties
    //Propiedades para obtener los datos
    public string Name
    {
        get
        {
            return creatureName;
        }
    }

    public string Description
    {
        get
        {
            return description;
        }
    }

    public Sprite CreatureBattleSprite
    {
        get
        {
            return battleSprite;
        }
    }

    public CreatureTypeEnum Type1
    {
        get
        {
            return type1;
        }
    }

    public CreatureTypeEnum Type2
    {
        get
        {
            return type2;
        }
    }

    public int MaxHP
    {
        get
        {
            return maxHP;
        }
    }

    public int Attack
    {
        get { return attack; }
    }
    public int Defense { get { return defense; } }
    public int SpAttack { get { return spAttack; } }

    public int SpDefense
    {
        get { return spDefense; }
    }
    public int Speed
    {
        get { return speed; }
    }
    //fomrmato para solo getter
    public int CatchRate => catchRate;

    public List<LearnableSkill> LearnableSkills
    {
        get
        {
            return learnableSkills;
        }
    }

    public List<SkillBase> LearnableSkillsByItem
    {
        get
        {
            return learnableSkillsByItems;
        }
    }

    public List<CreatureEvolution> Evolutions
    {
        get
        {
            return evolutions;
        }
    }

    public int ExpYield => expYield;
    public GrowthRateEnum GrowthRate => growthRate;
    #endregion
}
