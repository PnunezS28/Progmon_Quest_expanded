using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "new Creature Index")]
public class CreatureDex : ScriptableObject
{

    public List<CreatureDexEntry> creatureEntries;

    public List<SkillDexEntry> skillEntries;

    public List<ItemDexEntry> itemEntries;

    //ItemDex
    public bool Initialized = false;

    public void Init()
    {//Puede optimizarse, metiendo en el id su posición en la lista en lugar de asignar manualmente el id
        if (Initialized == false)
        {
            Debug.Log("Inicializando CreatureDex: " + this.name);

            //Si uso la posición en la lista de cada entrada como entry id puedo reservar el baseId para el sistema de guardado
            //Solo debo perocuparme por que las criaturas tengan ids de base únicos.
            int icreature = 0;
            foreach (CreatureDexEntry entry in creatureEntries)
            {
                icreature++;
                //var entryId = entry.baseCreature.creatureDexId;
                entry.entryId = icreature;
                entry.baseCreature.displayCreatureId = icreature;
            }
            foreach (SkillDexEntry skill in skillEntries)
            {
                var skillId = skill.skillBase.skillId;
                skill.skillId = skillId;

            }
            //para los ids seguramente añada varios, añadiré en el int un código, 
            //Primero item type y luego id único
            //(itemType) (item id within type)
            //No haré sumas con los ids así que debiera manetenerse
            foreach(ItemDexEntry item in itemEntries)
            {
                var itemId = item.itemBase.ItemDexId;
                item.itemId = itemId;
            }
        }
        

    }
}
