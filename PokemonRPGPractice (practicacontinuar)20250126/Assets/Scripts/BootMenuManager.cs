using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BootMenuManager : MonoBehaviour
{
    [SerializeField] FlagManager flagManager;
    [SerializeField] SceneLoader loader;

    public static BootMenuManager instance;

    private void Start()
    {
       if(instance == null)
        {
            instance = this;
        }
        loader.EndLoadingFade();
    }
    public void StartNewGame()
    {
        Debug.Log("BootMenuManager Start new game");
        loader.LoadStartLevel();
    }

    public void ToggleFullScreen()
    {
        if (Screen.fullScreen == true)
        {
            Debug.Log("OptionsUI: Toggle Fullscreen false");
            Screen.fullScreen = false;
        }
        else
        {
            Debug.Log("OptionsUI: Toggle Fullscreen true");
            Screen.fullScreen = true;
        }
    }

    public void LoadGame()
    {
        Debug.Log("BootMenuManager Start load game");
        if(SaveSystem.SaveFileExists()==false)
        {
            return;
        }

        flagManager.SetFlag(FlagManager.BOOT_LOAD_GAME, true.ToString());
        loader.LoadStartLevel();
    }

    public void ExitGame()
    {
        Debug.Log("BootMenuManager Closing game");
        Application.Quit();
    }
}
