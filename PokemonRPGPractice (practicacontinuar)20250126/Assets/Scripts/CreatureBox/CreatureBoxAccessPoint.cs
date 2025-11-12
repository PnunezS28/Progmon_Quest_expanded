using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatureBoxAccessPoint : MonoBehaviour,Interactable
{
    public void Interact(Transform initiator)
    {
        StartCoroutine( GameController.Instance.OpenCreatureBoxMenu());
    }

    
}
