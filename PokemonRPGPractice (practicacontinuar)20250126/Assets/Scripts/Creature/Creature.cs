
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class Creature
{
    //Atributos para serializar atributos y mostrarlos en una lista en el editor
    [SerializeField] CreatureBase _base; //cambiar a string nombre de recurso
    [SerializeField] int level;
    [SerializeField] EquipBattleItem initEquippedItem; //permite forzar una habilidad equipada en iniciación

    // Clase que calcula los vaores de una criatura un determinado nivel
    public CreatureBase Base { get { return _base; } }
    public int Level { get { return level; } }
    public int HP { get; set; }//Puntos de vida actuales

    public int Exp { get; set; }

    public List<Skill> Skills { get; set; }

    public Skill currentMove { get; set; }


    //Constructor copia
    public Creature(CreatureBase cBase, int level)
    {
        _base = cBase;
        this.level = level;
        Init();
    }

    public Creature(SerializedCreatureData data)
    {
        
        this._base = CreatureDexHandler.i.GetCreatureBase(data._baseId);
        this.level = data.level;
        this.Exp = data.Exp;

        if (data.conditionId != ConditionID.none)
        {
            this.Status = ConditionsDB.Conditions[data.conditionId];
        }
        this.Skills = new List<Skill>();
        foreach (SerializedSkillData d in data.Skills)
        {
            Skills.Add(new Skill(d));
        }//Hay un fayo al cargar y/o guardar la habilidad
        this.EquipedItemId = data.equippedItemid;
        this.EquipAbility = BattlePassiveDB.BattlePassives[data.equippedAbility];
        //Init();

        CalculateStats();
        HP = data.HP;

        StatusChanges = new Queue<string>();

        ResetStatBoosts();
        VolatileStatus = null;

    }


    //Stats calculados en memoria
    public Dictionary<CreatureStat, int> Stats { get; private set; }

    //Contiene los niveles que una estadística se ha impulsado o frenado max 6, min -6.
    public Dictionary<CreatureStat, int> StatsBoosts { get; private set; }

    //Fila que lista los cambios de estadísticas aplicados
    public Queue<string> StatusChanges { get; private set; } = new Queue<string>();

    //Campo para indicar estado de batalla de una criatura
    public Condition Status { get; set; }
    //Variable int que algunas condiciones emplearán
    public int StatusTime { get; set; }

    public Condition VolatileStatus { get; set; }
    public int VolatileStatusTime { get; set; }

    //variable de habilidadPasiva
    [SerializeField]
    public BattlePassive EquipAbility{get;set;}
    public int EquipedItemId { get; set; } = -1;
    [HideInInspector]
    public bool OneUseItemUsed = false;


    //Variable para hacer más eficiente la actualización del HUD de batalla
    //Evento empleado para actualizar HUD de batalla asociado
    public event System.Action OnStatusChanged;
    public event System.Action OnHpChanged;
    public event System.Action OnLevelUp;

    //fórmula usada en Pokémon ((base stat*level)/100)+5

    #region Calculated Properties


    public void Init()
    {//Código de inicialización
        Skills=new List<Skill>();
        //generar moves
        foreach(var skill in Base.LearnableSkills)
        {
            if (skill.Level <= level)
            {//añadir movimientos que pueda aprender en base a su nivel
                Skills.Add(new Skill(skill.SkillBase));
            }
            if (Skills.Count >= CreatureBase.MaxNumOfSkills) break;
        }
        Exp = Base.GetExpForLevel(level);

        CalculateStats();
        HP = MaxHP;

        StatusChanges = new Queue<string>();

        ResetStatBoosts();
        Status = null;
        VolatileStatus = null;
        OneUseItemUsed = false;
        EquipAbility = null;
        EquipedItemId = -1;

        if (initEquippedItem != null)
        {
            Debug.Log("Initializing with equipped item");
            SetEquipedAbility(initEquippedItem);
        }
    }  
    
    void CalculateStats()
    {
        Stats = new Dictionary<CreatureStat, int>();
        Stats.Add(CreatureStat.ATTACK, Mathf.FloorToInt((Base.Attack * Level) / 100f) + 5);
        Stats.Add(CreatureStat.DEFENSE, Mathf.FloorToInt((Base.Defense * Level) / 100f) + 5);
        Stats.Add(CreatureStat.SP_ATTACK, Mathf.FloorToInt((Base.SpAttack * Level) / 100f) + 5);
        Stats.Add(CreatureStat.SP_DEFENSE, Mathf.FloorToInt((Base.SpDefense * Level) / 100f) + 5);
        Stats.Add(CreatureStat.SPEED, Mathf.FloorToInt((Base.Speed * Level) / 100f) + 5);

        MaxHP = Mathf.FloorToInt((Base.MaxHP * Level) / 100f) + 10 + level;
    }

    void ResetStatBoosts()
    {
        //Stat boosts inicializados a 0 (valor neutral)
        StatsBoosts = new Dictionary<CreatureStat, int>() {
            {CreatureStat.ATTACK,0},
            {CreatureStat.DEFENSE,0},
            {CreatureStat.SP_ATTACK,0},
            {CreatureStat.SP_DEFENSE,0},
            {CreatureStat.SPEED,0},
            {CreatureStat.Accuracy,0},
            {CreatureStat.Evasion,0}
        };
    }

    int GetStat(CreatureStat stat)
    {//Además de sacar el valor de la estadística indicada del diccionario, aplica el impulso apropiado, positivo o negativo.
        int valueStat = Stats[stat];
        // apply stat boost
        int boostLevel = StatsBoosts[stat];
        //grados de incremento de estadísticas
        var boostvalues = new float[] { 1f, 1.5f, 2f, 2.5f, 3f, 3.5f, 4f };
        //TODO:añadir aplicación de habilidad pasiva
        if (boostLevel >= 0)
        {//boost en aumento
            valueStat = Mathf.FloorToInt(valueStat * boostvalues[boostLevel]);
        }
        else
        {//boost en decremento
            valueStat = Mathf.FloorToInt(valueStat / boostvalues[-boostLevel]);
        }

        if (EquipedItemId != -1)
        {
            if (EquipAbility?.OnStatGet != null)
            {
                int newValue = EquipAbility.OnStatGet(this, stat, valueStat);
                valueStat = newValue;
            }
        }

        return valueStat;
    }



    public int Attack
    {
        get
        {
            return GetStat(CreatureStat.ATTACK);
        }
    }

    public int Defense
    {
        get
        {
            return GetStat(CreatureStat.DEFENSE);
        }
    }

    public int SpAttack
    {
        get
        {
            return GetStat(CreatureStat.SP_ATTACK);
        }
    }

    public int SpDefense
    {
        get
        {
            return GetStat(CreatureStat.SP_DEFENSE);
        }
    }
    public int Speed
    {
        get
        {
            return GetStat(CreatureStat.SPEED);
        }
    }
    public int MaxHP
    {//No boosteable
        get; private set;
    }
    #endregion

    public bool ApplyBoosts(List<StatBoostEffect> boosts)
    {//recive una lista de estadísticas a modificar y las aplica
        foreach (var effect in boosts)
        {
            
            var affectedStat = effect.stat;
            var effectValue = effect.boostLevel;
            
            //Por cada valor en los boosts se aplica en el diccionario de incrementos máximo 6 y min -6 en total
            StatsBoosts[affectedStat]=Mathf.Clamp(StatsBoosts[affectedStat] + effectValue,-6,6);

            if (effectValue > 0)
            {
                if (StatsBoosts[affectedStat]==6)
                {
                    //si el boost no se puede aplicar más sa hace un aviso
                    StatusChanges.Enqueue($"{affectedStat} de {Base.Name} ha llegado a su aumento máximo.");
                    Debug.Log($"{Base.Name}'s {affectedStat} has been boosted to {StatsBoosts[affectedStat]}");
                    return false;
                }
                else
                {
                    StatusChanges.Enqueue($"{affectedStat} de {Base.Name} aumentó en {effectValue} niveles.");
                    Debug.Log($"{Base.Name}'s {affectedStat} has been boosted to {StatsBoosts[affectedStat]}");
                    return true;
                }
                
            }
            else
            {
                if (StatsBoosts[affectedStat] == -6)
                {
                    //si el boost no se puede aplicar más sa hace un aviso
                    StatusChanges.Enqueue($"{affectedStat} de {Base.Name} ha bajado al mínimo.");
                    Debug.Log($"{Base.Name}'s {affectedStat} has been boosted to {StatsBoosts[affectedStat]}");
                    return false;
                }
                else
                {
                    StatusChanges.Enqueue($"{affectedStat} de {Base.Name} bajó en {-effectValue} niveles.");
                Debug.Log($"{Base.Name}'s {affectedStat} has been boosted to {StatsBoosts[affectedStat]}");
                    return true;
                }
                
            }

        }
        return true;
    }

    public bool CheckForLevelUp()
    {
        if (Exp > _base.GetExpForLevel(level+1) && level<CreatureBase.MaxCreatureLevel)
        {
            ++level;
            OnLevelUp?.Invoke();
            return true;
        }
        return false;
    }

    public LearnableSkill getLearnableSkillAtCurrentLevel()
    {
        return Base.LearnableSkills.Where(x => x.Level == this.level).FirstOrDefault();
    }

    public void LearnSkill(SkillBase newSkill)
    {
        if (Skills.Count >= CreatureBase.MaxNumOfSkills) return;
        Skills.Add(new Skill(newSkill));
    }
    public bool HasSkill(SkillBase skillToCheck)
    {
        return Skills.Count(m => m.SkillBase == skillToCheck) > 0;
    }

    public DamageDetails TakeDamage(Skill skill, Creature attacker)
    {
        //cáclulo de golpe crítico
        float critical = 1f;
        if (Random.value * 100f <= 6.25)
        {
            //provabilidad golpe crítuco
            critical = 2f;
        }

        float typeEffectiveness = CreatureTypeEffectivenessChart.GetEffectiveness(skill.SkillBase.Type,this.Base.Type1)
            * CreatureTypeEffectivenessChart.GetEffectiveness(skill.SkillBase.Type, this.Base.Type2);
        //Cálculo de la efectividad multiplicando la efectividad del tipo 1 por el tipo 2.

        //Detalles del daño para informar al usuario
        var damageDetails = new DamageDetails()
        {
            TypeEffectiveness = typeEffectiveness,
            Critical = critical,
            Fainted = false
        };

        //Operador condicional
        //se almacena en la variable el ataque especial del atacante o el ataque físico si el ataque era especial o no, respectivamente.
        float attackUsed = (skill.SkillBase.Category!=SkillCategory.SPECIAL) ? attacker.SpAttack : attacker.Attack;
        //Se alamacena la defensa especial si el ataque era especial, sino, es almacena la defensa física
        float defenseUsed = (skill.SkillBase.Category != SkillCategory.SPECIAL) ? this.SpDefense : this.Defense;


        //Devuelve booleano true si se debilita
        //Formula usada en pokémon
        float modifiers = Random.Range(0.85f, 1f) * typeEffectiveness * critical;//múltiples modificadores
        float a = (2 * attacker.Level + 10) / 250f;// ataque efectivo de la criatura atacante
        float d = a * skill.SkillBase.Power * (attackUsed / defenseUsed) + 2;//defensa effectiva

        if (attacker.EquipedItemId != -1)
        {
            if (attacker.EquipAbility?.OnSkillUsed != null)
            {
                Debug.Log("attacker tiene habilidad OnSkillUsed");
        //OnSkillUsed; de attacker, función de EquipAbility, varía el daño que realiza, devuelve el mismo valor si no cambia o uno nuevo
                float skillModdedModifiers = EquipAbility.OnSkillUsed(new BattleArgs(attacker,modifiers,skill,this, damageDetails));
                modifiers = skillModdedModifiers;
            }
        }



        int damage = Mathf.FloorToInt(d * modifiers);//Daño total

        //OnDamageTaken, de esta criatura, EquipAbility, puede reducir el daño que recive
        if(EquipedItemId != -1)
        {
            if (EquipAbility?.OnDamagetaken != null)
            {
                Debug.Log("El defensor tiene una habilidad OnDamageTaken");
                int newDamage = EquipAbility.OnDamagetaken(new BattleArgs(this, modifiers, skill, attacker,damageDetails), damage);
                damage = newDamage;
            }
        }

        DecreaseHP(damage);

        //OnOponentHit de attacker,
        //AfterGetHit(), de esta criatura,
        if (EquipedItemId != -1)
        {
            if (EquipAbility?.AfterGetHit != null)
            {
                Debug.Log("El defensor tiene una habilidad AfterGetHit");
                EquipAbility.AfterGetHit(new BattleArgs(this, modifiers, skill, attacker));
            }
        }
        Debug.Log($"{Base.Name} took {damage} {skill.SkillBase.Category} damage. Effectiveness: {typeEffectiveness}, Critical: {critical} ");
        Debug.Log($"{Base.Name} now has {HP} Health Points.");
        
        return damageDetails;

    }

    public bool SetStatus(ConditionID conditionID)
    {//Establece el estado de una criatura y añade a la cola de mensajes el cambio realizado
        //No se permite inflinjir un estado si ya está afectado por uno previamente
        //devuelve true o false si ha surtido efecto o no
        if (Status != null)
        {
            StatusChanges.Enqueue($"No surtió efecto...");
            return false;
        }
        Status= ConditionsDB.Conditions[conditionID];
        //OnStatusRecieved, del usuario cuando recive un estado puede devolver un bool para determinar si recive el estado y otros efectos.
        //Si tuviera inmunidad la curaría y pasaría false para no iniciarlo
        bool abilityCure = false;
        if (EquipedItemId != -1)
        {
            if (EquipAbility?.OnStatusRecieved != null)
            {
                Debug.Log("El defensor tiene una habilidad OnStatusRecieved");
                abilityCure = EquipAbility.OnStatusRecieved(new BattleArgs(this));
            }
        }
        if (abilityCure)
        {
            return true;
        }
        Status?.OnStart?.Invoke(this);
        StatusChanges.Enqueue($"{Base.Name} {Status.StartMessage}");

        OnStatusChanged?.Invoke();
        return true;
    }

    public void CureStatus()
    {
        Status = null;
        OnStatusChanged?.Invoke();
    }

    public bool SetVolatileStatus(ConditionID conditionID)
    {//Establece el estado de una criatura y añade a la cola de mensajes el cambio realizado
        //No se permite inflinjir un estado si ya está afectado por uno previamente
        if (VolatileStatus != null)
        {
            StatusChanges.Enqueue($"No surtió efecto...");
            return false;
        }
        VolatileStatus = ConditionsDB.Conditions[conditionID];
        //OnStatusRecieved, del usuario cuando recive un estado puede devolver un bool para determinar si recive el estado y otros efectos.
        //Si tuviera inmunidad la curaría y pasaría false para no iniciarlo
        bool abilityCure = false;

        if (EquipedItemId != -1)
        {
            if (EquipAbility?.OnStatusRecieved != null)
            {
                Debug.Log("El defensor tiene una habilidad OnStatusRecieved");
                abilityCure = EquipAbility.OnStatusRecieved(new BattleArgs(this));
            }
        }
        if (abilityCure)
        {
            return true;
        }
        VolatileStatus?.OnStart?.Invoke(this);
        StatusChanges.Enqueue($"{Base.Name} {VolatileStatus.StartMessage}");
        return true;
    }

    public void CureVolatileStatus()
    {
        VolatileStatus = null;
    }

    public void DecreaseHP(int damage)
    {

        HP =Mathf.Clamp(HP - damage,0,MaxHP);
        Debug.Log($"CREATURE: this {_base.Name} creature lost {damage} HP");
        OnHpChanged?.Invoke();
        //Usado para actualizar hp en hud, TODo: combinar funciones con evento
    }

    public void IncreaseHP(int heal)
    {
        HP = Mathf.Clamp(HP + heal, 0, MaxHP);
        Debug.Log($"CREATURE: this {_base.Name} creature gained {heal} HP");
        OnHpChanged?.Invoke();
        //Usado para actualizar hp en hud, TODo: combinar funciones con evento
    }

    public Skill GetRandomSkill()
    {
        var useableMoves = Skills.Where(x => x.Uses > 0).ToList();
        int r = Random.Range(0, useableMoves.Count);
        return Skills[r];
    }

    public bool OnBeforeSkill()
    {
        bool canPerformSkill = true;
        //Función que aplica efectos antes de realizar skill, si devuelbe falso, la criatura no podrá realizar su skill, por alguna razón
        if (Status?.OnBeforeSkill != null)
        {
            if (Status.OnBeforeSkill(this)==false) { 
                canPerformSkill = false; 
            }
        }
        if (VolatileStatus?.OnBeforeSkill != null)
        {
            if (VolatileStatus.OnBeforeSkill(this)==false)
            {
                canPerformSkill = false;
            }
        }
        return canPerformSkill;
    }

    public void OnAfterTurn()
    {//Aplicar efectos que ocurran al final del turno, encapsulado para estados volátiles, contínuos, objetos y habilidades pasivas
        //Invoke permite hacer un intento de llamar la función de una acción si no es nulo
        //La ?, es un operador que comprueba si no es nulo. protege contra null returns
        Status?.OnAfterTurn?.Invoke(this);
        VolatileStatus?.OnAfterTurn?.Invoke(this);
        //equippedAbility after turn
    }

    public void OnBattleOver()
    {
        VolatileStatus = null;
        ResetStatBoosts();
    }
    
    public void SetEquipedAbility(EquipBattleItem item)
    {
        Debug.Log($"Ability Equipped to {Base.Name}: ID= {item.EquipmentEffect}");
        this.EquipedItemId = item.ItemDexId;
        this.EquipAbility = BattlePassiveDB.BattlePassives[item.EquipmentEffect];
        OnHpChanged?.Invoke();
    }

    public int UnsetEquipedAbility()
    {
        Debug.Log($"Ability Unequipped from {Base.Name}");
        int itemDexId = this.EquipedItemId;
        this.EquipedItemId = -1;
        this.EquipAbility = null;
        OnHpChanged?.Invoke();

        return itemDexId;
    }

    public bool EvolveCreature(int evolveIndex)
    {
        CreatureEvolution targetEvo = _base.Evolutions[evolveIndex];

        if (targetEvo.CanEvolve(this))
        {
            this._base = targetEvo.Creature;

            if (targetEvo.EvoType == EvoTypeEnum.REGRESSION || targetEvo.EvoType == EvoTypeEnum.EVOLUTION)
            {
                //Reiniciar nivel a 1 en regresión o evolución
                this.level = 1;
                this.Exp = 0;
            }

            CalculateStats();
            this.HP = MaxHP;
            OnHpChanged?.Invoke();

            CreatureDexHandler.i.SetCreatureGet(targetEvo.Creature.creatureDexId);
            return true;
        }

        return false;
    }
}
