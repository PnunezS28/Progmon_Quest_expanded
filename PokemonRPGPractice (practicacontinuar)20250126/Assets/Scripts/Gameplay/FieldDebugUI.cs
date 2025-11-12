using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

enum FieldDebugMenuModeEnum { FIELD, CREATE_OBJECT }

public class FieldDebugUI : MonoBehaviour
{
    [SerializeField] GameObject menuUI;

    [SerializeField] GameObject menu;
    List<TextMeshProUGUI> menuItems;

    [SerializeField]
    FieldDebugSys debugSys;

    [SerializeField] DebugAddObjectUI debugAddObjectUI;

    int selectedItem = 0;

    int selectedTransit = 0;
    FieldDebugMenuModeEnum menuMode;

    Action debugBack;

    public void OpenDebug()
    {
        Debug.Log("Opening debug");
        menuUI.SetActive(true);
        menuItems = menu.GetComponentsInChildren<TextMeshProUGUI>().ToList();
        menuItems[2].text = "Go to transit: < 0 >";
        menuMode = FieldDebugMenuModeEnum.FIELD;

    }

    public void CloseDebug()
    {
        Debug.Log("Closign Debug");

        menuUI.SetActive(false);

    }

    // Update is called once per frame
    public void handleUpdate(Action OnBack)
    {

        if (menuMode == FieldDebugMenuModeEnum.CREATE_OBJECT)
        {
            debugAddObjectUI.HandleUpdate(() =>
            {
                debugAddObjectUI.CloseDebugUI();
                menuMode = FieldDebugMenuModeEnum.FIELD;
            });
        }
        else
        {
            debugBack = OnBack;
            int prevSelection = selectedItem;

            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                ++selectedItem;
            }
            else if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                --selectedItem;
            }

            //selectedCategory = Mathf.Clamp(selectedCategory, 0, Inventory.ItemCategories.Count-1);
            selectedItem = Mathf.Clamp(selectedItem, 0, menuItems.Count - 1);
            //todo cambiar para no hardcodear list


            if (prevSelection != selectedItem)
            {
                UpdateItemSelection();
            }

            if (selectedItem == 2)
            {
                int prevTransit = selectedTransit;
                if (Input.GetKeyDown(KeyCode.LeftArrow))
                {
                    selectedTransit -= 1;
                }
                else if (Input.GetKeyDown(KeyCode.RightArrow))
                {
                    selectedTransit += 1;

                }
                selectedTransit = Mathf.Clamp(selectedTransit, 0, TransitPointDB.transitPointDB.Count - 1);

                if (selectedTransit != prevTransit)
                {
                    //Actualizar texto selección
                    menuItems[2].text = $"Go to transit: < {selectedTransit} > \n {TransitPointDB.transitPointDB[selectedTransit].TransitDestinationName}";
                }
            }


            if (Input.GetKeyDown(KeyCode.Z))
            {
                //Open party screen
                ItemSelected();
            }
            else if (Input.GetKeyDown(KeyCode.X))
            {
                Debug.Log("Back en debug campo");

                debugBack?.Invoke();
            }

        }

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

    void ItemSelected()
    {
        switch (selectedItem)
        {
            case 0:
                debugSys.OpenCloseWorld2();
                break;
            case 1:
                debugSys.OpenCloseWorld3();
                break;
            case 2:
                CloseDebug();
                debugSys.DebugTargetTransit(selectedTransit);
                break;
            case 3:
                Debug.Log("SwitchOnOff randomEncounters");
                debugSys.DebugSwitchOnOffRandomEncounters();
                break;
            case 4:
                OpenDebugAddObject();
                break;
            case 5:
                //Cerrar
                Debug.Log("Back en debug campo");

                debugBack?.Invoke();
                break;
        }
    }


    public void OpenDebugAddObject()
    {
        Debug.Log("DEBUG: Opening add object debug menu");
        debugAddObjectUI.OpenDebugUI();
        menuMode = FieldDebugMenuModeEnum.CREATE_OBJECT;
    }
}
