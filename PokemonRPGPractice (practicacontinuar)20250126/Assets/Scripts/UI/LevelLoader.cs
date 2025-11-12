using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour
{
    public static LevelLoader instance = null;
    public Animator transition;
    public float transitionWaitTime = 1f;

    public int CreatureHealCenterSceneId=3;
    public int CreatureHealCenterTransitId=4;

    public Action OnTransitionStart;
    int activeScene = 0;

    /*
    public void LoadStartLevel()
    {
        StartCoroutine(LoadLevel(SceneManager.GetActiveScene().buildIndex + 1));
    }*/

    /*
    public void FinishGame()
    {
        Debug.Log("Saliendo del juego");
        FindObjectOfType<FlagManager>().ClearFlags();//clear flags
        SceneManager.LoadScene(0);
    }*/

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        Debug.Log("LevelLoader Awake");
        //DontDestroyOnLoad(gameObject);
    }
    public IEnumerator LoadLevel(int sceneIndex)
    {
        OnTransitionStart?.Invoke();
        transition.SetTrigger("crossfadeTrigger");
        //Rediseñar para trabajar con las escenas como sifueran prefabs, activando uno y desactibando otro a medida que transcurre

        yield return new WaitForSeconds(transitionWaitTime);
        SetPrefabSceneActive(activeScene,false);
        SetPrefabSceneActive(sceneIndex, true);

        //SceneManager.LoadScene(sceneIndex,LoadSceneMode.Single);
        GameController.Instance.SpawnPlayer();
        activeScene = sceneIndex;

        GameController.Instance.OnTransitionEnd();
    }

    public void TransitLevels(int transitionDestinationId)
    {
        GameController.Instance.OnTransitionStart();
        Debug.Log("Now transitioning levels, transit destination id= " + transitionDestinationId);

        FlagManager.instance.SetFlag(FlagManager.SPAWN_ON_TRANSIT_POINT, "true");
        FlagManager.instance.SetFlag(FlagManager.DESTINATION_TRANSIT_ID, transitionDestinationId.ToString());
        
        int sceneDestinationId = TransitPointDB.transitPointDB[transitionDestinationId].DestinationSceneId;

        Debug.Log("Transitioning into scene id= " + sceneDestinationId);
        StartCoroutine(LoadLevel(sceneDestinationId));
    }

    void SetPrefabSceneActive(int sceneId,bool active)
    {
        ScenePrefabTag[] scenes= FindObjectsOfType<ScenePrefabTag>(true);

        for (int i = 0; i < scenes.Length; i++)
        {
            if (scenes[i].SceneId == sceneId)
            {
                scenes[i].gameObject.SetActive(active);
                return;
            }
        }
    }

    public int GetCurrentSceneId()
    {
        return activeScene;
    }

    public void TransitToHealCenter()
    {
        FlagManager.instance.SetFlag(FlagManager.TRANSIT_TO_HEAL_CENTER_ON_DEFEAT, true.ToString());
        FlagManager.instance.SetFlag(FlagManager.DESTINATION_TRANSIT_ID, CreatureHealCenterTransitId.ToString());

        int sceneDestinationId = CreatureHealCenterSceneId;

        Debug.Log("Transitioning into scene id= " + sceneDestinationId);
        StartCoroutine(LoadLevel(sceneDestinationId));
    }
}
