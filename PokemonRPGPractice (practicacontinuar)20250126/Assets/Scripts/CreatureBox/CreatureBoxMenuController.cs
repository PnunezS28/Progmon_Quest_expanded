using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class CreatureBoxMenuController : MonoBehaviour
{
    [SerializeField] GameObject menu;
    [SerializeField] CreatureBoxUI creatureBoxUI;
    [SerializeField] PartyScreen partyScreen;
    List<TextMeshProUGUI> menuItems;

    bool creatureBoxOpen = false;
    bool partyScreenOpen = false;
    bool busy = false;
    //party screen para guardar criatura

    public event Action OnBack;

    int selectedItem = 0;

    private void Awake()
    {
        menuItems = menu.GetComponentsInChildren<TextMeshProUGUI>().ToList();
    }

    public void OpenMenu()
    {
        menu.SetActive(true);
        UpdateItemSelection();
    }

    public void CloseMenu()
    {
        menu.SetActive(false);
        OnBack?.Invoke();
    }

    public void HandleUpdate()
    {
        if (busy == false)
        {
            if (creatureBoxOpen == false && partyScreenOpen==false)
            {
                int prevSelection = selectedItem;
                if (Input.GetKeyDown(KeyCode.DownArrow))
                {
                    ++selectedItem;
                }
                else if (Input.GetKeyDown(KeyCode.UpArrow))
                {
                    --selectedItem;
                }

                selectedItem = Mathf.Clamp(selectedItem, 0, menuItems.Count - 1);

                if (prevSelection != selectedItem)
                {
                    UpdateItemSelection();
                }

                if (Input.GetKeyDown(KeyCode.Z))
                {
                    //realizar acción
                    Debug.Log("Selected menu action");
                    OnOperationSelected();
                }
                else if (Input.GetKeyDown(KeyCode.X))
                {
                    Debug.Log("Selected menu back");
                    CloseMenu();
                }
            }
            else if(creatureBoxOpen==true)
            {//criatureBoxOpen
                Action boxOnBack = () =>
                {
                    CloseCreatureBoxUI();
                };
                creatureBoxUI.HandleUpdate(boxOnBack);
            }else if (partyScreenOpen == true)
            {
                Action partyOnback = () =>
                {
                    ClosePartyScreen();
                };
                Action onCreatureSelected = () =>
                {
                    bool operationSuccess= partyScreen.TransferSelectedCreatureToBox();
                    if (operationSuccess)
                    {
                        ClosePartyScreen();
                        //aviso de éxito
                        StartCoroutine(TransferSuccessMessasge());
                    }
                    else
                    {
                        StartCoroutine(TransferToBoxWarning());
                    }
                };
                partyScreen.HandleUpdate(onCreatureSelected,partyOnback);
            }
        }
        
        
    }

    void OnOperationSelected()
    {
        Debug.Log("OperationSelected");
        //usar selectedItem
        switch (selectedItem)
        {
            case 0:
                //guardar criatura
                partyScreen.gameObject.SetActive(true);
                partyScreen.setControlsText("[Z]: Guardar Criatura\n[X]: Cancelar");
                partyScreenOpen = true;
                break;
            case 1:
                //retirar criatura
                OpenCreatureBoxUI();
                creatureBoxUI.SetOperation(CreatureBoxStateEnum.RETRIEVE);
                break;
            case 2:
                //borrar criatura
                OpenCreatureBoxUI();
                creatureBoxUI.SetOperation(CreatureBoxStateEnum.DELETE);
                break;
            case 3:
                //cerrar menu
                CloseMenu();
                break;
        }
    }

    void OpenCreatureBoxUI()
    {
        creatureBoxUI.gameObject.SetActive(true);
        creatureBoxOpen=true;
    }

    void CloseCreatureBoxUI()
    {
        creatureBoxUI.gameObject.SetActive(false);
        creatureBoxOpen = false;
    }

    void ClosePartyScreen()
    {
        partyScreen.gameObject.SetActive(false);
        partyScreenOpen = false;
    }

   IEnumerator TransferSuccessMessasge()
   {
        busy = true;
        yield return DialogueManager.Instance.MostrarTextoDialogoSimple("Se ha transferido la criatura a la caja", returnToFreeRoam: false);
        busy = false;
   }

    IEnumerator TransferToBoxWarning()
    {
        busy=true;
        yield return DialogueManager.Instance.MostrarTextoDialogoSimple("AVISO: debes tener al menos una criatura sana en tu equipo", returnToFreeRoam: false);
        busy = false;

    }

    public void UpdateItemSelection()
    {
        for (int i = 0; i < menuItems.Count; i++)
        {
            if (i == selectedItem)
            {
                menuItems[i].color = GlobalSettings.i.HighlightedColor;
            }
            else
            {
                menuItems[i].color = Color.black;
            }
        }
    }
}
