using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CreatureEntryItemUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI creatureName;
    [SerializeField] TextMeshProUGUI creatureId;

    RectTransform rectTransformUI;

    public bool seleccionable;

    CreatureDexEntry entry;

    public void SetData(CreatureDexEntry creatureSlot,bool seleccionable)
    {
        rectTransformUI = GetComponent<RectTransform>();
        this.seleccionable=seleccionable;
        this.entry = creatureSlot;
        if (seleccionable)
        {
            creatureName.text = creatureSlot.baseCreature.Name;
        }
        else
        {
            creatureName.text = "???";
        }
        creatureId.text = $"#{creatureSlot.baseCreature.displayCreatureId.ToString("000")}";
    }

    public void SetColor(Color color)
    {
        creatureName.color = color;
        creatureId.color = color;
    }

    public CreatureDexEntry Entry => entry;
    public float Height => rectTransformUI.rect.height;
}
