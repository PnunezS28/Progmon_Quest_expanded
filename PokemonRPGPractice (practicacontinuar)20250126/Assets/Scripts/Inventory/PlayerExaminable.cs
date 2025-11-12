using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerExaminable : MonoBehaviour, Interactable
{

    [SerializeField] DialogueElement dialogueElement;

    public void Interact(Transform initiatior)
    {
        Debug.Log("Interacting with NPC");
        //Solo interactuable si está en idle
        if (dialogueElement != null)
        {
            
            dialogueElement.TriggerElement();
        }
        //StartCoroutine());
    }
}
