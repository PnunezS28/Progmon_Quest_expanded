using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldDebugSys : MonoBehaviour
{
    public void OpenCloseWorld2()
    {
        Debug.Log("OpenCloseWorld2 debug activated");
        bool world2IsOpen = FlagManager.instance.GetFlagAsBool("Mundo2Open");
        if (world2IsOpen)
        {
            FlagManager.instance.SetFlag("Mundo2Open", "false");

        }
        else
        {
            FlagManager.instance.SetFlag("Mundo2Open", "true");
        }
    }

    public void OpenCloseWorld3()
    {
        Debug.Log("OpenCloseWorld3 To be implemented");
        
        bool world3IsOpen = FlagManager.instance.GetFlagAsBool("Mundo3Open");
        if (world3IsOpen)
        {
            FlagManager.instance.SetFlag("Mundo3Open", "false");
        }
        else
        {
            FlagManager.instance.SetFlag("Mundo3Open", "true");
        }

        
    }

    public void DebugTargetTransit(int transitionDestinationId)
    {
        Debug.Log("DebugTargetTransit now transiting to transittionDestinationId= "+transitionDestinationId);

        LevelLoader.instance.TransitLevels(transitionDestinationId);
    }

    public void DebugSwitchOnOffRandomEncounters()
    {
        bool encountersOn = FlagManager.instance.GetFlagAsBool("DEBUG_encountersOn",true);
        if (encountersOn)
        {
            FlagManager.instance.SetFlag("DEBUG_encountersOn", "false");
            Debug.Log("DEBUG: switched OFF random encounters");
        }
        else
        {
            FlagManager.instance.SetFlag("DEBUG_encountersOn", "true");
            Debug.Log("DEBUG: switched ON random encounters");
        }
    }

}
