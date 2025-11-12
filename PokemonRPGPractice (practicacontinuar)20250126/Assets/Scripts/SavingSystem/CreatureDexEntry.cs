using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CreatureDexEntry
{
    [SerializeField] CreatureBase creature;
    [HideInInspector]public int entryId;

    public CreatureBase baseCreature
    {
        get
        {
            return creature;
        }
    }

}
