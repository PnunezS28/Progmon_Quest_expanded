using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Dialogue
{
    // Guarda la información de un diálogo
    public string name;
    //propiedad TextArea: int minimo líneas int max líneas
    [TextArea(3,10)]
    public string[] sentences;

    public FlagSetting[] flagSet;//flags que se deben activar cuando se reproduzca el diálogo
    [NonReorderable]
    public DialogueElement nextTrigger;//el siguiente elemento de diálogo que se debe activar después

    public AudioClip audioClip;//Adio a reproducir cuando empieze el trigger asociado a este diálogo
    public float audioClipVolume=.5f;

    public bool HealAllCreturesInParty;

}
