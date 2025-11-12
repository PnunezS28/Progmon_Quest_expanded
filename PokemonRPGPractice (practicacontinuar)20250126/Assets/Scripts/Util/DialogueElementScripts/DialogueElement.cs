using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class DialogueElement :ScriptableObject
{
    //Clase abstracta que permite crear ScriptableObject que pueden encadenarse con diálogos
    //o ser activados en esa cadena

    //Puede crear su propio elemento de diálogo creando una clase que herede de esta.
    public abstract void TriggerElement(Action onFinished=null);
}
