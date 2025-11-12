using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlagManager : MonoBehaviour
{
    //Clase clave para el funcionamiento del sistema de flags en Dialogue y BookTrigger
    //Debe ser implementado en un objeto en la escena siempre que se quiera acceder a FlagDictionary

    [SerializeField]
    private FlagDictionary flagMemory;

    //booleano que guarda si el jugador debe aparecer en un punto concreto de la escena diferente del inicial
    public static string SPAWN_ON_TRANSIT_POINT = "spawn_on_transit_point";
    //int que lleva el id de la BD con los datos del punto donde spawnear
    public static string DESTINATION_TRANSIT_ID = "destination_transit_id";

    //bool para saver si spawnear en nua posición guardada en el fichero de guardado
    public static string SPAWN_ON_SAVE_FILE_LOADING = "spawn_on_save_file_loading";

    public static string BOOT_LOAD_GAME = "load_file_spawn";

    //base string para guardar los datos de entrenadores derrotados
    public static string TRAINER_DEFEATED_BASE = "trainer_defeated_id_";
    //base string para guardar los pickups del mundo obtenidos
    public static string PICKUP_OBTANINED_BASE = "pickup_obtanied_";
    public static string ENCOUNTER_CAPTURED_BASE = "encounter_captured_";



    public static string TRANSIT_TO_HEAL_CENTER_ON_DEFEAT = "transit_to_heal_center_on_defeat";

    public event Action<FlagSetting> OnFlagValueChanged;

    public static FlagManager instance { get; private set; }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        Debug.Log("FlagManager awake");

    }

    //Al llamarse introducirá o actualizará en el diccionario el valor indicado
    public void SetFlag(string flagKey,string flagValue)
    {
        flagMemory.SetFlag(flagKey, flagValue);
        OnFlagValueChanged?.Invoke(new FlagSetting(){ flagKey = flagKey, flagValue=flagValue });
    }

    //Al llamarse limpia el diccionario de todas las flags guardadas
    public void ClearFlags()
    {
        flagMemory.ClearFlags();
    }

    //Intenta devolver el valor asociado a la llave aportada como string
    public string GetFlagAsString(string flagKey,string defaultValue=null)
    {
        Debug.Log("Retrieving flag value as string value, flagKey= " + flagKey);
        string flagVal = defaultValue;
        if (flagMemory.getFlags().ContainsKey(flagKey))
        {
            flagVal = flagMemory.getFlags()[flagKey];
        }
        return flagVal;
    }

    //Intenta devolver el valor asociado a la llave aportada como bool
    public bool GetFlagAsBool(string flagKey,bool defaultValue=false)
    {
        Debug.Log("Retrieving flag value as boolean value, flagKey= " + flagKey);
        bool flagVal= defaultValue;
        if (flagMemory.getFlags().ContainsKey(flagKey))
        {
            flagVal =  bool.Parse(flagMemory.getFlags()[flagKey]);
        }

        Debug.Log("Retrieving flag value as boolean value, flagvalue= " + flagVal);
        return flagVal;
    }

    //Intenta devolver el valor asociado a la llave aportada como float
    public float? GetFlagAsFloat(string flagKey,float? defaultValue=null)
    {
        Debug.Log("Retrieving flag value as a float value, flagKey= " + flagKey);
        float? value = defaultValue;

        if (flagMemory.getFlags().ContainsKey(flagKey))
        {
            value = float.Parse(flagMemory.getFlags()[flagKey]);
        }

        return value;

    }

    public Dictionary<string,string> GetFlagDictionary()
    {
        return flagMemory.getFlags();
    }
    public void SetFlagDictionary(Dictionary<string, string> dict)
    {
        flagMemory.setDictionary(dict);
        OnFlagValueChanged?.Invoke(null);
    }
}
