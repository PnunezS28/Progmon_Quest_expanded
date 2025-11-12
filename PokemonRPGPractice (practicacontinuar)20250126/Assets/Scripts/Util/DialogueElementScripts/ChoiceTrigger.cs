using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Dialogue Element/Choice Trigger")]
public class ChoiceTrigger : DialogueElement
{
    [SerializeField]
    public Choice[] choices;//Array que contiene las elecciones disponibles en un diálogo
    /*
    public void triggerElement()
    {
        FindObjectOfType<DialogueManager>().PromptChoices(choices);
    }*/

    public override void TriggerElement(Action onFinished=null)//Cuando el elemento es activado el DialogueManager comienza a mostrar las elecciones
    {
        
        DialogueManager.Instance.EscribirAreaElecciones(choices,onFinished);
    }
}
