using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BookPage
{
    //Elemento de diálogo a reproducir
    public DialogueElement dialogueElement;//Elemento de diálogo a reproducir

    public FlagCheckTrigger[] triggers;//Lista de condiciones de reproducción
    //Un objeto FlagManager es necesario en la escena para su funcionamiento

}
