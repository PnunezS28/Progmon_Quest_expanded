using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransitionTrigger : MonoBehaviour,IPlayerTrigerable
{
    [SerializeField] int transitionDestinationId;

    public void OnPlayerTriggered(PlayerController player)
    {
        Debug.Log("Player has entered the portal");
        
        Debug.Log("Transition Trigger activated");
        LevelLoader.instance.TransitLevels(transitionDestinationId);
        
    }

    //todo modd para que no se active hasta que el jugador haya acabado de moverse
    /*
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("Transition Trigger activated");
            FindObjectOfType<LevelLoader>().TransitLevels(transitionDestinationId);
        }
    }*/
}
