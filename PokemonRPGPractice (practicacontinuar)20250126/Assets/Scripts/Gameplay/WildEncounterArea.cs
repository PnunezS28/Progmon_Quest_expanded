using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WildEncounterArea : MonoBehaviour,IPlayerTrigerable
{
    public void OnPlayerTriggered(PlayerController player)
    {
        
            Debug.Log("CheckingEncounter");
            //Encounter
            //Si el númeroaleatorio entre 1 y 100 es menor o igual a 10 inicia batalla
            if (Random.Range(1, 101) <= 10)
            {
                Debug.Log("Encountered a wild monster");
                GameController.Instance.StartWildBattle();
            }
        
        //no encounter
    }

}
