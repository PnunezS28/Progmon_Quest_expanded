using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CreatureBoxItemUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI creatureName;
    [SerializeField] TextMeshProUGUI creatureLevel;

    RectTransform rectTransformUI;

    public void SetData(Creature creature)
    {
        rectTransformUI = GetComponent<RectTransform>();
        creatureName.text = creature.Base.Name;
        creatureLevel.text = $"LV.: {creature.Level}";
    }

    public void SetEmpty()
    {
        rectTransformUI = GetComponent<RectTransform>();
        creatureName.text = "---";
        creatureLevel.text = "LV.:---";
    }

    public void SetColor(Color color)
    {
        creatureName.color = color;
        creatureLevel.color = color;
    }

    public float Height => rectTransformUI.rect.height;
}
