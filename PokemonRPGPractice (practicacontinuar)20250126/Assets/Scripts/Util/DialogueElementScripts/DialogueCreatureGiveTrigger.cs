using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Dialogue Element/Creature Give Trigger")]
public class DialogueCreatureGiveTrigger : DialogueElement
{
    //Esta clase activa el elemento de dialogo asociados
    [SerializeField]
    //controla el inicio de un diálogo o elementos de historia
    public Dialogue dialogue;
    public CreatureBase creatureToGive;
    [Min(1)]
    public int creatureLevel;

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
            Camera.main.GetComponent<AudioSource>().PlayOneShot(dialogue.audioClip, dialogue.audioClipVolume);
        }
        if (creatureToGive != null)
        {
            CreatureParty.GetPlayerParty().AddCreature(new Creature(creatureToGive, creatureLevel));
        }
        DialogueManager.Instance.IniciarDialogo(dialogue, onFinished);
    }
}
