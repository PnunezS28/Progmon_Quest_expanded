using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Dialogue Element/WildBattleStart Trigger")]
public class WildBattleStartTrigger : DialogueElement
{
    public CreatureBase creatureEncounter;
    [Min(1)]
    public int creatureLevel;

    public bool mustBeCaptured = false;
    public string encounterID;

    public FlagSetting[] flagSet;//flags que se deben activar cuando se reproduzca el diálogo

    public DialogueTrigger dialogueAfterBattleCleared;
    public DialogueTrigger dialogueAfterBattleCaptureFailed;


    public override void TriggerElement(Action onFinished = null)
    {
        Action onVictory = () => {
            if (mustBeCaptured)
            {
                bool wasCaptured = CreatureDexHandler.i.CreatureDexCompletion[creatureEncounter.creatureDexId];
                if (wasCaptured)
                {
                    FlagManager.instance.SetFlag(FlagManager.ENCOUNTER_CAPTURED_BASE + encounterID, true.ToString());
                    //activará las banderas indicadas cuando la criatura sea capturada. además de su propia bandera de captura
                    ActivateFlags();
                    if (dialogueAfterBattleCleared != null)
                    {
                        dialogueAfterBattleCleared.TriggerElement();
                    }
                }
                else
                {
                    if (dialogueAfterBattleCaptureFailed != null)
                    {
                        dialogueAfterBattleCaptureFailed.TriggerElement();
                    }
                }
            }
            else
            {
                ActivateFlags();
                if (dialogueAfterBattleCleared != null)
                {
                    dialogueAfterBattleCleared.TriggerElement();
                }
            }
            
        };
        GameController.Instance.StartForcedEncounterWildBattle(new Creature(creatureEncounter, creatureLevel),onVictory);
    }

    void ActivateFlags()
    {
        if (flagSet.Length > 0)
        {
            Debug.Log("Flag Trigger detectado, activando flags");
            foreach (FlagSetting flag in flagSet)
            {
                FindObjectOfType<FlagManager>().SetFlag(flag.flagKey, flag.flagValue);
            }
        }
    }
}
