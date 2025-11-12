using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EvoSlotUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI textoNombre;
    [SerializeField] Image OKSprite;
    [SerializeField] Color OKColor;

    [SerializeField] Image NGSprite;
    [SerializeField] Color NGColor;

    [SerializeField] Color SpriteUnselected;

    [SerializeField] Image BackgroundImage;
    [SerializeField] Color EntrySelected;
    [SerializeField] Color EntryUnSelectedColor;

    [SerializeField] TextMeshProUGUI textoTipoEvo;

    public CreatureEvolution evolution;
    public bool discoveredEvolution;
    public void SetData(Creature thisCreature,CreatureEvolution evo)
    {
        this.evolution = evo;
        if (evo.CanEvolve(thisCreature))
        {
            OKSprite.color = OKColor;
            NGSprite.color = SpriteUnselected;
        }
        else
        {
            OKSprite.color = SpriteUnselected;
            NGSprite.color = NGColor;
        }

        discoveredEvolution = CreatureDexHandler.i.CreatureDexCompletion[evolution.Creature.creatureDexId];

        switch (evo.EvoType)
        {
            case EvoTypeEnum.NONE:
                textoTipoEvo.text = "???"; break;
            case EvoTypeEnum.EVOLUTION:
                textoTipoEvo.text = "EVOLUTION";break;
            case EvoTypeEnum.REGRESSION:
                textoTipoEvo.text = "REGRESSION";break;
            case EvoTypeEnum.MODECHANGE:
                textoTipoEvo.text = "MODE_CHANGE";break;
        }

        if(discoveredEvolution)
        {
            textoNombre.text = evo.Creature.Name;
        }
        else
        {
            textoNombre.text = "???";

        }
;    }

    public void SetSelected(bool selected)
    {
        if (selected)
        {
            BackgroundImage.color = EntrySelected;

        }
        else
        {
            BackgroundImage.color = EntryUnSelectedColor;

        }
    }
}
