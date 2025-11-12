using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class FlagCheckTrigger
{
    //Clase que contiene datos para realizar comparaciones de datos en el diccionario de flags
    
    public FlagTypeEnum flagType;//Tipo de variable que contiene el flag
    public string flagKey;//nombre del flag
    public FlagCompareEnum compare;//condición de comparación
    public string flagValue;//valor con el que se compara

    public bool IsTriggered(FlagManager flagManager)
    {
        bool triggered = true;

        switch (flagType)
        {
            case FlagTypeEnum.BOOL://Comprobación para booleano
                bool boolFlagValue = flagManager.GetFlagAsBool(flagKey);

                switch (compare)
                {
                    case FlagCompareEnum.EQUALS:
                        if (boolFlagValue != bool.Parse(flagValue))
                        {
                            return false;
                            //detecta que no se cumple la condición salta el resto de flags e indica que es false
                        }
                        //pasa sin hacer nada
                        break;
                    case FlagCompareEnum.NOT_EQUALS:
                        if (boolFlagValue == bool.Parse(flagValue))
                        {
                            return false;
                        }
                        break;
                }
                break;
            case FlagTypeEnum.STRING://comprobación para string
                string stringFlagValue = flagManager.GetFlagAsString(flagKey);
                switch (compare)
                {
                    case FlagCompareEnum.EQUALS:
                        if (stringFlagValue.Equals(flagValue) == false)
                        {
                            return false;
                        }
                        break;
                    case FlagCompareEnum.NOT_EQUALS:
                        if (stringFlagValue.Equals(flagValue))
                        {
                            return false;
                        }
                        break;
                }

                break;
            case FlagTypeEnum.FLOAT://comparación para float
                float floatFlagValue = (float)flagManager.GetFlagAsFloat(flagKey);
                switch (compare)
                {
                    case FlagCompareEnum.EQUALS:
                        if (floatFlagValue != float.Parse(flagValue))
                        {
                            return false;
                        }
                        break;
                    case FlagCompareEnum.NOT_EQUALS:
                        if (floatFlagValue == float.Parse(flagValue))
                        {
                            return false;
                        }
                        break;

                    case FlagCompareEnum.GREATER_THAN:
                        if (floatFlagValue <= float.Parse(flagValue))
                        {
                            return false;
                        }
                        break;

                    case FlagCompareEnum.GREATER_THAN_OR_EQUAL:
                        if (floatFlagValue < float.Parse(flagValue))
                        {
                            return false;
                        }
                        break;
                    case FlagCompareEnum.LESSER_THAN:
                        if (floatFlagValue >= float.Parse(flagValue))
                        {
                            return false;
                        }
                        break;

                    case FlagCompareEnum.LESSER_THAN_OR_EQUAL:
                        if (floatFlagValue > float.Parse(flagValue))
                        {
                            return false;
                        }
                        break;
                }
                break;
        }

        return triggered;
    }
}
