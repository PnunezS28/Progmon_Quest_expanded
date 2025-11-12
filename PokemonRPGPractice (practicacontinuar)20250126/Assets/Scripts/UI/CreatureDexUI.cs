using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CreatureDexUI : MonoBehaviour
{
    [SerializeField] GameObject creatureList;
    [SerializeField] CreatureEntryItemUI cretureEntryPrefab;

    [SerializeField] Image creatureImage;
    [SerializeField] TextMeshProUGUI creatureName;

    [Header("Creature details")]
    [SerializeField] GameObject creatureDetailsUI;
    [SerializeField] TextMeshProUGUI creatureDetails;
    [SerializeField] Image imageTipo1;
    [SerializeField] Image imageTipo2;

    List<CreatureDexEntry> creatureEntries;
    RectTransform creatureListRect;

    List<CreatureEntryItemUI> creatureEntryUIList;

    int selectedEntry = 0;
    const int itemsInViewPort = 8;

    public CreatureDexUIState state;

    private void Awake()
    {
        creatureListRect = creatureList.GetComponent<RectTransform>();
        //UpdateEntryList();
        CreatureDexHandler.i.OnUpdated += UpdateEntryList;
    }

    private void Start()
    {
        UpdateEntryList();
        UpdateEntrySelection();
    }

    void UpdateEntryList()
    {
        Debug.Log("Updating entry list");
        //Obtener lista de criaturas
        creatureEntries = CreatureDexHandler.i.getAllCretaureDexEntries();
        foreach (Transform item in creatureList.transform)
        {//Destruir todos los objetos vacios
            Destroy(item.gameObject);
        }
        creatureEntryUIList = new List<CreatureEntryItemUI>();
        foreach (var itemSlot in creatureEntries)
        {//instaciar los objetos como hijos de la lista
            var instatiatedObject = Instantiate(cretureEntryPrefab, creatureList.transform);
            bool seleccionable = CreatureDexHandler.i.CreatureDexCompletion[itemSlot.baseCreature.creatureDexId];
            instatiatedObject.SetData(itemSlot,seleccionable);
            creatureEntryUIList.Add(instatiatedObject);
        }
    }

    // HandleUpdate
    public void HandleUpdate(Action OnBack)
    {
        if (state == CreatureDexUIState.LIST)
        {
            int prevSelection = selectedEntry;

            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                ++selectedEntry;
            }
            else if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                --selectedEntry;
            }

            if (prevSelection != selectedEntry)
            {
                UpdateEntrySelection();
            }

            if (Input.GetKeyDown(KeyCode.Z))
            {
                if (creatureEntryUIList[selectedEntry].seleccionable == true)
                {
                    //mostrar detalles
                    creatureDetailsUI.gameObject.SetActive(true);
                    state = CreatureDexUIState.DETAILS;
                    creatureDetails.text = creatureEntries[selectedEntry].baseCreature.Description;
                    imageTipo1.sprite = GlobalSettings.i.CreatureTypeUISprites[creatureEntries[selectedEntry].baseCreature.Type1];
                    imageTipo2.sprite = GlobalSettings.i.CreatureTypeUISprites[creatureEntries[selectedEntry].baseCreature.Type2];
                }
                
            }
            else if (Input.GetKeyDown(KeyCode.X))
            {
                OnBack?.Invoke();
            }
        }else if (state == CreatureDexUIState.DETAILS)
        {
            if (Input.GetKeyDown(KeyCode.X))
            {
                state = CreatureDexUIState.LIST;
                creatureDetailsUI.gameObject.SetActive(false);
            }
        }

        
    }
    void HandleScrolling()
    {
        float scrollPos = Mathf.Clamp(selectedEntry - itemsInViewPort / 2, 0, selectedEntry) * creatureEntryUIList[0].Height;//scroll position

        creatureListRect.localPosition = new Vector2(creatureListRect.localPosition.x, scrollPos);
    }

    //UpdateEntrySelection + handleScrolling
    public void UpdateEntrySelection()
    {
        var selectedSlots = creatureEntries;
        selectedEntry = Mathf.Clamp(selectedEntry, 0, selectedSlots.Count - 1);

        for (int i = 0; i < creatureEntryUIList.Count; i++)
        {
            if (i == selectedEntry)
            {
                creatureEntryUIList[i].SetColor(GlobalSettings.i.HighlightedColor);
                if (creatureEntryUIList[i].seleccionable==false)
                {
                    creatureName.text = "???";
                    creatureImage.sprite = null;
                }
                else
                {
                    creatureImage.sprite = creatureEntryUIList[i].Entry.baseCreature.CreatureBattleSprite;
                    creatureName.text = creatureEntryUIList[i].Entry.baseCreature.Name;
                }
            }
            else
            {
                creatureEntryUIList[i].SetColor(Color.black);
            }
        }


        HandleScrolling();
    }
    //OpenInfoScreen
    //CloseInfoScreen
}
