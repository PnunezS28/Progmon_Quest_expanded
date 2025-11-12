using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Dialogue Element/ItemGive Trigger")]
public class DialogueItemGiveTrigger : DialogueElement
{
    //Esta clase activa el elemento de dialogo asociados
    [SerializeField]
    //controla el inicio de un diálogo o elementos de historia
    public Dialogue dialogue;
    public ItemBase itemToGive;
    [Min(0)]public int amountOfItem;

    //Llamará al mánager de diálogo e iniciará la conversación con el objeto que lo contiene
    /*
    public void triggerElement()
    {
        FindObjectOfType<DialogueManager>().StartDialogue(dialogue);
    }*/

    public override void TriggerElement(Action onFinished = null)
    {
        if (dialogue.flagSet.Length > 0)
        {//activará las banderas indicadas cuando pase por este diálogo. Solo si no es null.
            Debug.Log("Flag Trigger detectado, activando flags");
            foreach (FlagSetting flag in dialogue.flagSet)
            {
                FindObjectOfType<FlagManager>().SetFlag(flag.flagKey, flag.flagValue);
            }
        }
        if (dialogue.audioClip != null)
        {
            AudioManager.i.PlaySfx(dialogue.audioClip);
        }
        if (itemToGive != null && amountOfItem > 0)
        {
            Inventory.GetInventory().AddItem(itemToGive, amountOfItem);
        }
        DialogueManager.Instance.IniciarDialogo(dialogue, onFinished);
    }

}

