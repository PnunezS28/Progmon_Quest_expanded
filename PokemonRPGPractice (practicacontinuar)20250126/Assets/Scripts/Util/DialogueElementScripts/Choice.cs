using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Choice
{
    //Contiene la información para realizar la elección durante un diálogo
    public string name;

    //Dialogue trigger que ejecutará cuando se elija.
    public DialogueElement dialogueTrigger;

    public Choice(string name,DialogueElement element)
    {
        this.name = name;
        this.dialogueTrigger = element;
    }
}
