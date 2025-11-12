using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Skill", menuName = "Creature Data/ New skill")]
public class SkillBase : ScriptableObject
{
    public int skillId;
    [SerializeField] string displayName;

    [TextArea]
    [SerializeField] string description;

    [SerializeField] CreatureTypeEnum type;
    [SerializeField] int power;
    [SerializeField] int accuracy;
    [SerializeField] bool alwaysHit;
    [SerializeField] int maxUses; //Cambiar a coste de energía Skill Points.
    //nivel de prioridad, se ejecutará antes que otros movimiento aun si el usuario es más lento, por defecto 0
    [SerializeField] int priority;
    //Tipo de movimiento, si es físico, especial o de estado
    [SerializeField] SkillCategory category;

    //Efectos de estado que se inflinjen en el objetivo
    [SerializeField] SkillEffects effects;
    //Efectos secundarios con la probabilidad de activarse aun si el movimiento no es de estado
    [NonReorderable]
    [SerializeField] List<SecondaryEffects> secondaries;

    //Objetivo del movimiento, puede ser un enemigo o uno mismo
    [SerializeField] SkillTargetEnum target;

    [SerializeField] AudioClip sound;
    [SerializeField] ParticleFXContainer skillParticleFXPrefab;

    #region Properties
    public string SkillName
    {
        get { return displayName; }
    }
    public string Description
    {
        get { return description; }
    }
    public CreatureTypeEnum Type
    {
        get { return type; }
    }
    public int Power
    {
        get { return power; }
    }
    public int Accuracy
    {
        get { return accuracy; }
    }

    public bool AlwaysHit
    {
        get
        {
            return alwaysHit;
        }
    }
    public int MaxUses
    {
        get { return maxUses; }
    }

    public int Priority
    {
        get { return priority; }
    }

    public SkillCategory Category
    {
        get {
        
            return category;
        }
    }
    
    public SkillEffects Effects
    {
        get
        {
            return effects;
        }
    }
    
    public List<SecondaryEffects> Secondaries
    {
        get
        {
            return secondaries;
        }
    }

    public SkillTargetEnum Target
    {
        get
        {
            return target;
        }
    }

    public ParticleFXContainer SkillParticleFXPrefab
    {
        get
        {
            return skillParticleFXPrefab;
        }
    }

    public AudioClip Sound => sound;
    #endregion
}
