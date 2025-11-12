using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Flag Dictionary")]
public class FlagDictionary : ScriptableObject
{
    //ScriptableObject que permite la persistencia de flags para guardar progreso.
    //Este objeto es clave para el uso de la clase BookPage, BookTrigger y la funcion Flag de Dialogue
    private Dictionary<string, string> _flags = new Dictionary<string, string>();
    public Dictionary<string, string> getFlags()
    {
        return _flags;
    }
    public void SetFlag(string flagKey, string flagValue)
    {//Al llamarse introducirá o actualizará en el diccionario el valor indicado
        if (_flags.ContainsKey(flagKey))
        {
            //update flag
            Debug.Log("Flag of Key: " + flagKey + " Is being updated, previus value= " + _flags[flagKey]);
            _flags[flagKey] = flagValue;
        }
        else
        {
            //add flag
            Debug.Log("Adding new flag, flagKey= " + flagKey + ", new flag value= " + flagValue);
            _flags.Add(flagKey, flagValue);

        }
    }

    public void ClearFlags()
    {
        Debug.Log("Flags were cleared!");
        _flags.Clear();
    }

    public void setDictionary(Dictionary<string,string> dict)
    {
        _flags = dict;
    }
}
