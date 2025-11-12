using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Dialogue Element/Book Trigger")]
public class BookTrigger : DialogueElement
{
    //Comprende los múltiples diálogos que puede contener un único objeto y las condiciones de reproducción
    [SerializeField]
    public BookPage[] bookPages;

    //Cuando se activa el elemento empieza a listar las páginas contenidas desde el final hasta el principio
    //buscando el primer elemento activado
    public override void TriggerElement(Action onFinished=null)
    {
        Debug.Log("Book Triggered");
        for(int i = bookPages.Length-1; i >= 0; i--)
        {
            
            if (isTriggered(bookPages[i])==true)
            {
                Debug.Log("Book page"+i+" Triggered");
                bookPages[i].dialogueElement.TriggerElement(onFinished);
                return;//cuando se encuentra la página activada se dsalta el resto de la lista
            }
        }
    }

    public bool isTriggered(BookPage page)
    {
        Debug.Log("Checking flag triggers");//Comprueba si todos los triggers se han activado con las condiciones indicadas
        bool triggered = true;
        FlagManager flagManager = FindObjectOfType<FlagManager>();
        //bool flagValue = FindObjectOfType<FlagManager>().GetFlagAsBool(trigger.flagKey);
        foreach (FlagCheckTrigger trigger in page.triggers)
        {
            if (trigger.IsTriggered(flagManager) == false)
            {//En el momento que uno de los triggers no se cumpla devuelve falso y salta del bucle
                return false;
            }
        }
        //si no hubiera flags automaticamente devuelve true
        return triggered;
    }

}
