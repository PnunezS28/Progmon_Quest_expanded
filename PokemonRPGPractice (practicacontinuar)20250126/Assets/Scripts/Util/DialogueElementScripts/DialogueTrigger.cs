using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="Dialogue Element/Dialogue Trigger")]
public class DialogueTrigger : DialogueElement
{
    //Esta clase activa el elemento de dialogo asociados
    [SerializeField]
    //controla el inicio de un diálogo o elementos de historia
    public Dialogue dialogue;

    //TODO: implementar función para sustituir string <PlayerName> por el nombre del jugador antes de pasarlo al diálogo
    public override void TriggerElement(Action onFinished=null)
    {
        if (dialogue.flagSet.Length>0)
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
        if (dialogue.HealAllCreturesInParty)
        {
            CreatureParty.GetPlayerParty().FullCureAllCreatures();
        }
        DialogueManager.Instance.IniciarDialogo(dialogue,onFinished);
    }

}
