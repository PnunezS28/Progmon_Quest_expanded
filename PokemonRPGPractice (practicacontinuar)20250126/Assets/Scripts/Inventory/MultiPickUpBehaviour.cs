using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiPickUpBehaviour : MonoBehaviour, Interactable
{
    [SerializeField] ItemBase[] itemList;
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
        Debug.Log("PICKUP BEHACOIUR: " + pickupTag + " obtained= " + Used);
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
            Debug.Log($"MultiPickUpBehaviour: Se ha obtenido el pickup {pickupTag}");

            GameController.Instance.StartMultiPickupMenu(itemList, () =>
            {
                GameController.Instance.CloseMultiPickupMenu();
                Used = true;
                GetComponent<SpriteRenderer>().enabled = false;
                GetComponent<BoxCollider2D>().enabled = false;
                AudioManager.i.PlaySfx(AudioId.getItem);
                foreach (ItemBase item in itemList)
                {
                    initiator.GetComponent<Inventory>().AddItem(item);
                }
                FlagManager.instance.SetFlag(FlagManager.PICKUP_OBTANINED_BASE + this.pickupTag, true.ToString());
            });
            //abrir pantalla de pickup
            //manjear en la pantalla de pickup el añadir los objetos
            
        }
        //añadir objeto
    }
}
