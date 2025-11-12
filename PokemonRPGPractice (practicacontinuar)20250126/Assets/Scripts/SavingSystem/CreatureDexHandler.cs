using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CreatureDexHandler : MonoBehaviour
{
    [SerializeField] CreatureDex creatureDex;
    //array booleano que contiene las criaturas coleccionadas del jugador

    Dictionary<int,bool> creatureDexCompletion;
    public Dictionary<int, bool> CreatureDexCompletion => creatureDexCompletion;

    public static CreatureDexHandler i;

    public Action OnUpdated;
    public void Init()
    {
        creatureDex.Init();
        if (i == null)
        {
            i = this;
        }
        if (creatureDex.Initialized == false)
        {
            Debug.Log("Inicializando CreatureDexHandler progreso de colección");
            //inicializar a nueva
            creatureDexCompletion =new Dictionary<int, bool>();
            //inicializar nueva dex
            for(int i=0;i<creatureDex.creatureEntries.Count;i++)
            {
                Debug.Log("CreatureDexHandler init, adding key pair{"+(i+1)+", false}");
                creatureDexCompletion.Add((i+1), false);
            }
        }
    }

    public void SetCompletionFlags(Dictionary<int, bool> CompletionFlags)
    {
        this.creatureDexCompletion = CompletionFlags;
        OnUpdated?.Invoke();
    }

    public void SetCreatureGet(int dexId)
    {
        creatureDexCompletion[dexId]= true;
        OnUpdated?.Invoke();
    }

    public List<CreatureDexEntry> getAllCretaureDexEntries()
    {
        //Devuelve la lista de criaturas ordenadas por entry ID.
        return creatureDex.creatureEntries.OrderBy(x=> x.entryId).ToList();
    }

    public int GetCreatureDexLength()
    {
        return creatureDex.creatureEntries.Count();
    }

    public CreatureBase GetCreatureBase(int dexId)
    {
        var entrySearched = creatureDex.creatureEntries.Where(x => x.entryId == dexId).FirstOrDefault();
        return entrySearched.baseCreature;
    }

    public SkillBase GetSkillBase(int dexId)
    {
        var entrySearched = creatureDex.skillEntries.Where(x => x.skillId == dexId).FirstOrDefault();
        return entrySearched.skillBase;
    }

    public int GetItemDexLength()
    {
        return creatureDex.itemEntries.Count();
    }

    public List<ItemBase> GetAllItemBase()
    {
        Debug.Log("CreatureDexHandler: GetAllItemBase");
        List<ItemBase> result = new List<ItemBase>();
        foreach (ItemDexEntry item in creatureDex.itemEntries)
        {
            Debug.Log("CreatureDexHandler: GetAllItemBase: number:"+item.itemId);
            result.Add(item.itemBase);
        }
        return result;
    }

    public ItemBase GetItemBase(int dexId)
    {
        var entrySearched = creatureDex.itemEntries.Where(x => x.itemId == dexId).FirstOrDefault();
        return entrySearched.itemBase;
    }

    
}
