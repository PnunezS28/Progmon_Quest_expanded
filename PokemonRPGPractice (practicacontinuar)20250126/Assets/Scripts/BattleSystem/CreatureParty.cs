using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CreatureParty : MonoBehaviour
{
    [SerializeField] List<Creature> creatures;


    public event Action OnUpdated;

    public List<Creature> Creatures
    {
        get
        {
            return creatures;
        }
        set
        {
            creatures = value;
            OnUpdated?.Invoke();
        }
    }

    private void Awake()
    {
        PlayerController player = gameObject.GetComponent<PlayerController>();
        foreach(var creature in creatures)
        {//inicializa las criaturas en este equipo, si es el jugador añade a su prograso de index
            creature.Init();
            
            if (player != null)
            {
                //Si empieza en el equipo del jugador
                Debug.Log($"Criatura inicializada id {creature.Base.creatureDexId} registrada en progreso CreatureDex");
                CreatureDexHandler.i.SetCreatureGet(creature.Base.creatureDexId);
            }
        }
    }

    public Creature GetFirstHealthyCreature()
    {
        //Lambda para buscar x donde HP de x es mayor a 0
        return creatures.Where(x=>x.HP>0).FirstOrDefault();
    }

    public void AddCreature(Creature newCreature)
    {
        PlayerController player = gameObject.GetComponent<PlayerController>();
        if (player != null)
        {
            Debug.Log($"Criatura id {newCreature.Base.creatureDexId} registrada en progreso CreatureDex");
            //Si pertenece al jugador
            CreatureDexHandler.i.SetCreatureGet(newCreature.Base.creatureDexId);
        }
        if (creatures.Count < 6)
        {
            Debug.Log($"Criatura añadida {newCreature.Base.Name}");
            creatures.Add(newCreature);
            OnUpdated?.Invoke();
        }
        else
        {
            //transferir a caja TODO: implementar para enviar pokemon exceso a la caja
            Debug.Log($"Criatura id {newCreature.Base.creatureDexId} guardada en caja");
            CreatureBox.GetCreatureBox().saveCreature(newCreature);
        }

    }

    //remove creature y retrieveCreture

    public void RemoveCreature(Creature RemovedCreature)
    {
        creatures.Remove(RemovedCreature);
        OnUpdated?.Invoke();
    }
    public void swapOrder(int a,int b)
    {
        var firstCreature = creatures[a];
        creatures[a] = creatures[b];
        creatures[b] = firstCreature;
        OnUpdated?.Invoke();
    }

    public static CreatureParty GetPlayerParty()
    {
        return FindObjectOfType<PlayerController>().GetComponent<CreatureParty>();
    }

    public void FullCureAllCreatures()
    {
        Debug.Log("Resotring all creatures");
        foreach(Creature c in creatures)
        {
            c.HP = c.MaxHP;
            c.CureStatus();
            c.CureVolatileStatus();
            foreach(Skill s in c.Skills)
            {
                s.Uses = s.SkillBase.MaxUses;
            }
            Debug.Log($"{c.Base.Name} has been fully cured");
        }
        OnUpdated?.Invoke();
    }

    public int getCountOfHealthyMembers()
    {
        int i = 0;
        foreach(Creature c in creatures)
        {
            if (c.HP > 0)
            {
                i++;
            }
        }
        return i;
    }
}
