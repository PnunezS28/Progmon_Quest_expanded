using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlagActiveBasedObject : MonoBehaviour
{
    [SerializeField] GameObject DependentObject;

    [SerializeField] bool activateOnTriggered = true;
    [NonReorderable]
    public FlagCheckTrigger[] triggers;//Lista de condiciones de aparición
    //Un objeto FlagManager es necesario en la escena para su funcionamiento

    void OnEnable()
    {//Se suscribe al manager de banderas para controlar si la bandera pertinenete a este objeto se activa
        Debug.Log("FlagActiveBasedObject: "+gameObject.name+" enabled");
        FlagManager.instance.OnFlagValueChanged += OnFlagChanged;
        if (LevelLoader.instance != null)
        {
            LevelLoader.instance.OnTransitionStart += DesubscribeActivator;
        }
        if (isTriggered())
        {
            
            if (isTriggered())
            {
                DependentObject.SetActive(activateOnTriggered);
            }
            else
            {
                DependentObject.SetActive(!activateOnTriggered);
            }
        }
    }
    //Hacer método para dessuscrivirse cuando cambia la escena

    void DesubscribeActivator()
    {
        FlagManager.instance.OnFlagValueChanged -= OnFlagChanged;
        if (LevelLoader.instance != null)
        {
            LevelLoader.instance.OnTransitionStart -= DesubscribeActivator;
        }
    }

    void OnFlagChanged(FlagSetting setting)
    {

        if (setting == null)
        {
            return;
        }
        Debug.Log($"Flag value change detected: settings= key= {setting.flagKey}, value= {setting.flagValue}");
        if (isTriggered())
        {
            DependentObject.SetActive(activateOnTriggered);
        }
        else
        {
            DependentObject.SetActive(!activateOnTriggered);
        }
    }

    bool isTriggered()
    {
        Debug.Log("Checking flag triggers");//Comprueba si todos los triggers se han activado con las condiciones indicadas
        bool triggered = true;
        FlagManager flagManager = FlagManager.instance;
        //bool flagValue = FindObjectOfType<FlagManager>().GetFlagAsBool(trigger.flagKey);
        foreach (FlagCheckTrigger trigger in triggers)
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
