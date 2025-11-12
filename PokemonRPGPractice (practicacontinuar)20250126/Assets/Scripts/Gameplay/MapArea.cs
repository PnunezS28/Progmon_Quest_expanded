using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapArea : MonoBehaviour
{
    //Guarda los datos de criaturas salvajes en un area
    [SerializeField] List<Creature> wildCreatures;
    [SerializeField] AudioClip sceneBGM;

    private void Awake()
    {
        Debug.Log("MapArea awake");

    }
    private void Start()
    {
        Debug.Log("MapArea Start");
    }

    private void OnEnable()
    {
        if (sceneBGM != null)
        {
            AudioManager.i.PlayMusic(sceneBGM,fade:true);
        }
    }
    public Creature GetRandomWildCreature()
    {
        Debug.Log("Getting random wild creature: list count= "+wildCreatures.Count);
        var wildCreature= wildCreatures[Random.Range(0, wildCreatures.Count)];
        wildCreature.Init();
        return wildCreature;
    }

    public AudioClip SceneBGM => sceneBGM;

}
