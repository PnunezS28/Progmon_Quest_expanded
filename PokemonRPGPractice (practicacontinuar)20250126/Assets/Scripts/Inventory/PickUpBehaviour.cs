using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpBehaviour : MonoBehaviour, Interactable
{
    [SerializeField] ItemBase item;
    [SerializeField] string pickupTag;
    public bool Used { get; set; } = false;

    private void OnEnable()
    {
        FlagManager.instance.OnFlagValueChanged += UpdateState;
        if (LevelLoader.instance != null)
        {
            LevelLoader.instance.OnTransitionStart += DesubscribeActivator;
        }
    }

    void DesubscribeActivator()
    {
        FlagManager.instance.OnFlagValueChanged -= UpdateState;
        if (LevelLoader.instance != null)
        {
            LevelLoader.instance.OnTransitionStart -= DesubscribeActivator;
        }
    }

    void UpdateState(FlagSetting setting)//ligero bug, no se actualiza bien
    {
        Used = FlagManager.instance.GetFlagAsBool(FlagManager.PICKUP_OBTANINED_BASE + pickupTag);
        Debug.Log("PICKUP BEHACOIUR: "+pickupTag+" obtained= "+Used);
        if (Used == true)
        {
            GetComponent<SpriteRenderer>().enabled = false;
            GetComponent<BoxCollider2D>().enabled = false;
        }
    }
    public void Interact(Transform initiator)
    {
        Debug.Log("Picking up item");
        if (Used == false)
        {
            initiator.GetComponent<Inventory>().AddItem(item);
            Used = true;
            GetComponent<SpriteRenderer>().enabled = false;
            GetComponent<BoxCollider2D>().enabled = false;
            AudioManager.i.PlaySfx(AudioId.getItem);
            GameController.Instance.StartShortDialogue();

            StartCoroutine(DialogueManager.Instance.MostrarTextoDialogoSimple($"Encontraste {item.ItemName}"));

            Debug.Log($"Se ha obtenido el pickup {pickupTag}");
            FlagManager.instance.SetFlag(FlagManager.PICKUP_OBTANINED_BASE + pickupTag, true.ToString());
        }
        //añadir objeto
    }
}
