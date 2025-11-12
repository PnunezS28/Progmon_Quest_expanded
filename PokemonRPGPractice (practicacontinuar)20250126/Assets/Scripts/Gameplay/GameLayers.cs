using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLayers : MonoBehaviour
{
    [SerializeField] LayerMask solidObjectsLayer;
    [SerializeField] LayerMask tallGrassLayer;
    [SerializeField] LayerMask interactablesLayer;
    [SerializeField] LayerMask playerLayer;
    [SerializeField] LayerMask fOVLayer;
    [SerializeField] LayerMask portalsLayer;
    //Clase singleton para aceder a las diferentes Layers del juego

    public static GameLayers i { get; set; }

    private void Awake()
    {
        if (i == null)
        {
            i = this;
        }
        /*
        else if (i != this)
        {
            Destroy(gameObject);
        }*/
    }
    public LayerMask SolidLayer
    {
        get => solidObjectsLayer;
    }

    public LayerMask InteractableLayer
    {
        get => interactablesLayer;
    }

    public LayerMask GrassLayer
    {
        get => tallGrassLayer;
    }

    public LayerMask PlayerLayer
    {
        get => playerLayer;
    }

    public LayerMask FOVLayer
    {
        get => fOVLayer;
    }
    public LayerMask PortalsLayer
    {
        get => portalsLayer;
    }

    public LayerMask TriggerableLayer
    {
        get => GrassLayer|fOVLayer| portalsLayer;
    }
}
