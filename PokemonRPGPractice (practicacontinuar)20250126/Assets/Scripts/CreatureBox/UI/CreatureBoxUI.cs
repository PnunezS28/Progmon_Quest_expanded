using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Input = UnityEngine.Input;

public class CreatureBoxUI : MonoBehaviour
{
    //controla el UI de la pantalla de inventario y lista los objetos en posesión del jugador
    [SerializeField] GameObject boxList;
    //para hacer el scroll automático se va a mover el transform y para suvir o bajar la lista
    [SerializeField] CreatureBoxItemUI boxSlotUIPrefab;

    [SerializeField] Image itemIcon;

    [SerializeField] TextMeshProUGUI itemCategoryText;
    [SerializeField] GameObject SecondaryDeleteConfirmUI;

    CreatureBox creatureBox;

    List<CreatureBoxItemUI> creatureSlotUIList;
    int selectedCreature = 0;
    int selectedBox = 0;

    RectTransform boxListRect;

    const int itemsInViewPort = 8;
    CreatureBoxStateEnum operation; //indica la operación

    bool busy = false;
    bool deleteConfirmOpen = false;

    private void Awake()
    {
        creatureBox = CreatureBox.GetCreatureBox();
        boxListRect = boxList.GetComponent<RectTransform>();
    }

    // Start is called before the first frame update
    void Start()
    {
        UpdateBoxList();
        UpdateCreatureSelection();
        //subscribir update box list a creaturebox
        creatureBox.OnUpdated += UpdateBoxList;
    }

    public void SetOperation(CreatureBoxStateEnum state)
    {
        this.operation = state;
    }

    // Update is called once per frame
    public void HandleUpdate(Action OnBack)
    {
        if (busy == false)
        {

            if (deleteConfirmOpen == true)
            {
                if (Input.GetKeyDown(KeyCode.Z))
                {
                    busy = true;
                    creatureBox.RemoveCreature(selectedBox, selectedCreature);
                    StartCoroutine( ShowBoxMessage("El prog-mon ha sido borrado"));
                    SecondaryDeleteConfirmUI.gameObject.SetActive(false);
                    deleteConfirmOpen = false;
                    //confirm
                }
                else if (Input.GetKeyDown(KeyCode.X))
                {
                    //Cancelar
                    SecondaryDeleteConfirmUI.gameObject.SetActive(false);
                    deleteConfirmOpen = false;
                }
            }
            else
            {
                int prevSelection = selectedCreature;
                int prevBox = selectedBox;

                if (Input.GetKeyDown(KeyCode.DownArrow))
                {
                    ++selectedCreature;
                }
                else if (Input.GetKeyDown(KeyCode.UpArrow))
                {
                    --selectedCreature;
                }
                else if (Input.GetKeyDown(KeyCode.RightArrow))
                {
                    selectedBox++;
                }
                else if (Input.GetKeyDown(KeyCode.LeftArrow))
                {
                    selectedBox--;
                }

                if (selectedBox > CreatureBox.boxStrings.Count - 1)
                {
                    selectedBox = 0;
                }
                else if (selectedBox < 0)
                {
                    selectedBox = CreatureBox.boxStrings.Count - 1;
                }

                //selectedCategory = Mathf.Clamp(selectedCategory, 0, Inventory.ItemCategories.Count-1);
                selectedCreature = Mathf.Clamp(selectedCreature, 0, creatureBox.GetCreaturesByBox(selectedBox).Count - 1);
                //todo cambiar para no hardcodear list


                if (prevBox != selectedBox)
                {
                    ResetSelection();
                    itemCategoryText.text = CreatureBox.boxStrings[selectedBox];
                    UpdateBoxList();
                    UpdateCreatureSelection();
                }
                else if (prevSelection != selectedCreature)
                {
                    UpdateCreatureSelection();
                }


                if (Input.GetKeyDown(KeyCode.Z))
                {
                    //Open party screen
                    CreatureSelected();
                }
                else if (Input.GetKeyDown(KeyCode.X))
                {
                    Debug.Log("Back en bolsa");
                    OnBack?.Invoke();
                }
            }
            
        }
        
        
    }

    void UpdateBoxList()
    {
        //Lipiar todos los objetos existentes de la lista e introducir un objeto por cada objeto del inventario del jugador
        foreach (Transform item in boxList.transform)
        {//Destruir todos los objetos vacios
            Destroy(item.gameObject);
        }
        creatureSlotUIList = new List<CreatureBoxItemUI>();
        var selectedSlots = creatureBox.GetCreaturesByBox(selectedBox);
        

        for (int i = 0; i < CreatureBox.BOX_MAX_CREATURE_CAPACITY; i++)
        {
            var instatiatedObject = Instantiate(boxSlotUIPrefab, boxList.transform);
            if(i < selectedSlots.Count)
            {
                instatiatedObject.SetData(selectedSlots[i]);
                creatureSlotUIList.Add(instatiatedObject);
            }
            else
            {
                instatiatedObject.SetEmpty();
                creatureSlotUIList.Add(instatiatedObject);
            }
            
        }
    }

    void CreatureSelected()
    {
        //hacer cosas dependiendo de la operación
        Debug.Log("CreatureSelected");
        busy = true;
        if (operation == CreatureBoxStateEnum.RETRIEVE)
        {
            CreatureParty party = CreatureParty.GetPlayerParty();
            if (party.Creatures.Count < 6)
            {
                //inicar proceso de transferencia
                Creature retrievedCreature = creatureBox.RetrieveCreature(selectedBox, selectedCreature);
                party.AddCreature(retrievedCreature);
                StartCoroutine(ShowBoxMessage($"{retrievedCreature.Base.Name} ha sido transferido al equipo"));
            }
            else
            {
                StartCoroutine(ShowBoxMessage("No se puede transferir porque el equipo está lleno"));
            }
        }else if (operation == CreatureBoxStateEnum.DELETE)
        {
            Debug.Log("A creature is going to be deleted");
            //poner un menú secundario para protección extra de los datos
            StartCoroutine(ShowBoxMessage("El prog-mon va a ser borrado permanentemente. ¿Está seguro de esta operación?",false));

            SecondaryDeleteConfirmUI.gameObject.SetActive(true);
            deleteConfirmOpen = true;
            busy = false;
        }
    }
    
    IEnumerator ShowBoxMessage(string message,bool waitforInput=true)
    {
        yield return DialogueManager.Instance.MostrarTextoDialogoSimple(message, waitforInput, false);
        busy = false;
    }

    public void UpdateCreatureSelection()
    {
        var selectedSlots = creatureBox.GetCreaturesByBox(selectedBox);
        selectedCreature = Mathf.Clamp(selectedCreature, 0, selectedSlots.Count - 1);

        for (int i = 0; i < creatureSlotUIList.Count; i++)
        {
            if (i == selectedCreature)
            {
                creatureSlotUIList[i].SetColor(GlobalSettings.i.HighlightedColor);

            }
            else
            {
                creatureSlotUIList[i].SetColor(Color.black);
            }
        }


        if (selectedSlots.Count > 0)
        {
            var selectedSlot = selectedSlots[selectedCreature];
            itemIcon.sprite = selectedSlot.Base.CreatureBattleSprite;
        }

        HandleScrolling();
    }

    void HandleScrolling()
    {
        float scrollPos = Mathf.Clamp(selectedCreature - itemsInViewPort / 2, 0, selectedCreature) * creatureSlotUIList[0].Height;//scroll position

        boxListRect.localPosition = new Vector2(boxListRect.localPosition.x, scrollPos);
    }

    void ResetSelection()
    {
        selectedCreature = 0;
        itemIcon.sprite = null;
    }

}
