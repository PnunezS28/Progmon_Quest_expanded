using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Dialogue Element/Dialogue Weighted Random Trigger")]
public class DialogueWeightedRandomTrigger : DialogueElement
{
    [SerializeField] List<WeightedDialogue> WeightedElements;

    public override void TriggerElement(Action onfinished=null)
    {
        int totalWeight = 0;
        //Conseguir peso total
        foreach(WeightedDialogue wd in WeightedElements)
        {
            totalWeight += wd.wheight;
        }
        //Sacar número aleatorio

        int selectedWeight = UnityEngine.Random.Range(0, totalWeight+1);
        //activar dialogo seleccionado

        int checkedWeight = 0;
        foreach (WeightedDialogue wd in WeightedElements)
        {
            checkedWeight += wd.wheight;
            if (checkedWeight >= selectedWeight)
            {
                wd.dialogueElement.TriggerElement(onfinished);
                return;
            }
        }

    }
    

}
