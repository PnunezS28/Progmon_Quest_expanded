using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalSettings : MonoBehaviour
{

    [Header("Color")]
    [SerializeField] Color highlightedColor;

    [Header("Type sprites")]
    [SerializeField] Sprite fireTypeSpriteUI;
    [SerializeField] Sprite grassTypeSpriteUI;
    [SerializeField] Sprite waterTypeSpriteUI;
    [SerializeField] Sprite earthTypeSpriteUI;
    [SerializeField] Sprite windTypeSpriteUI;
    [SerializeField] Sprite elecTypeSpriteUI;
    [SerializeField] Sprite ghostTypeSpriteUI;
    [SerializeField] Sprite steelTypeSpriteUI;
    [SerializeField] Sprite psyTypeSpriteUI;
    [SerializeField] Sprite dragonTypeSpriteUI;
    [SerializeField] Sprite neutralTypeSpriteUI;
    [SerializeField] Sprite noneTypeSpriteUI;

    [Header("Status condition color")]
    [SerializeField] Color psnColor;
    [SerializeField] Color brnColor;
    [SerializeField] Color slpColor;
    [SerializeField] Color parColor;
    [SerializeField] Color frzColor;

    [Header("Default volume Settings")]
    [SerializeField] float defaultBGMVolumen = .25f;
    [SerializeField] float defaultSFXVolumen = .4f;

    //Extraer keys de aceptar y cancelar

    Dictionary<CreatureTypeEnum, Sprite> creatureTypeUISprites;

    Dictionary<ConditionID, Color> statusColors;

    public Color HighlightedColor => highlightedColor;

    public Dictionary<ConditionID, Color> StatusColors => statusColors;

    public Dictionary<CreatureTypeEnum, Sprite> CreatureTypeUISprites=> creatureTypeUISprites;

    public float DefaultBGMVolumen => defaultBGMVolumen;
    public float DefaultSFXVolumen => defaultSFXVolumen;

    public static GlobalSettings i;

    private void Awake()
    {
        if (i == null)
        {
            i = this;
        }
        statusColors = new Dictionary<ConditionID, Color>() {
            {ConditionID.PSN,psnColor },
            {ConditionID.BRN,brnColor },
            {ConditionID.SLP,slpColor },
            {ConditionID.PAR,parColor },
            {ConditionID.FRZ,frzColor },
        };

        creatureTypeUISprites = new Dictionary<CreatureTypeEnum, Sprite>() {
            {CreatureTypeEnum.NONE,noneTypeSpriteUI },
            {CreatureTypeEnum.FIRE,fireTypeSpriteUI },
            {CreatureTypeEnum.WATER,waterTypeSpriteUI },
            {CreatureTypeEnum.GRASS,grassTypeSpriteUI },
            {CreatureTypeEnum.EARTH,earthTypeSpriteUI },
            {CreatureTypeEnum.WIND,windTypeSpriteUI },
            {CreatureTypeEnum.ELECTRIC,elecTypeSpriteUI },
            {CreatureTypeEnum.GHOST,ghostTypeSpriteUI },
            {CreatureTypeEnum.METAL,steelTypeSpriteUI },
            {CreatureTypeEnum.PSYCHIC,psyTypeSpriteUI },
            {CreatureTypeEnum.DRAGON,dragonTypeSpriteUI },
            {CreatureTypeEnum.NEUTRAL,neutralTypeSpriteUI }
        };
    }

}
