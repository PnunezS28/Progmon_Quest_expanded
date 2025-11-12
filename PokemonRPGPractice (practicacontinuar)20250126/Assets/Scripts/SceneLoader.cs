using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public Animator transition;
    public Animator loadingTransition;
    public float transitionWaitTime = 1f;

    public void LoadStartLevel()
    {
        StartLoadingFade();
        StartCoroutine(LoadLevel(SceneManager.GetActiveScene().buildIndex + 1));
    }

    public void FinishGame()
    {
        Debug.Log("Saliendo del juego");
        FindObjectOfType<FlagManager>().ClearFlags();//clear flags
        SceneManager.LoadScene(0);
    }
    IEnumerator LoadLevel(int sceneIndex)
    {
        //transition.SetTrigger("crossfadeTrigger");


        yield return new WaitForSeconds(transitionWaitTime);

        SceneManager.LoadScene(sceneIndex);
    }

    public void StartLoadingFade()
    {
        loadingTransition.SetTrigger("CrossfadeStart");

    }

    public void EndLoadingFade()
    {
        loadingTransition.SetTrigger("CrossfadeEnd");

    }

}
