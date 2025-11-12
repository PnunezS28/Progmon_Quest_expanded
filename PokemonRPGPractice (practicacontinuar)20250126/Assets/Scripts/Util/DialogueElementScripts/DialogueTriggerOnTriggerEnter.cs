using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTriggerOnTriggerEnter : MonoBehaviour
{
    [SerializeField] DialogueElement dialogue;
    [SerializeField] string triggerTag= "Player";
    [SerializeField] bool triggersByChance;
    [SerializeField] int triggerChance=100;
    // El dialogo se activa cuando el jugador, con el tag de 'Player' entra en el trigger
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(triggerTag))
        {
            if (triggersByChance)
            {//Si se elije en base a provabilidad, se realiza una tirada con provabilidad de triggerChance base 100 para activar el diálogo
                triggerChance = Mathf.Clamp(triggerChance, 1, 100);
                int r = Random.Range(0, 101);
                if (r <= triggerChance)
                {
                    dialogue.TriggerElement();
                }
            }
            else
            {//Por defecto, en cuanto el jugador pase por el trigger, se activa el diálogo
                dialogue.TriggerElement();
            }
        }
    }
}
