using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class BattleDebugSysUI : MonoBehaviour
{
    [SerializeField] GameObject menuUI;

    [SerializeField] GameObject menu;
    List<TextMeshProUGUI> menuItems;

    [SerializeField]
    BattleDebugSys battleDebugSys;

    int selectedItem = 0;
    Action debugBack;

    public void OpenDebug()
    {
        Debug.Log("Opening debug");
        menuUI.SetActive(true);
        menuItems = menu.GetComponentsInChildren<TextMeshProUGUI>().ToList();
    }

    public void CloseDebug()
    {
        Debug.Log("Closign Debug");

        menuUI.SetActive(false);

    }
    // Update is called once per frame
    public void handleUpdate(Action OnBack)
    {
        int prevSelection = selectedItem;
        debugBack = OnBack;

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


        if (Input.GetKeyDown(KeyCode.Z))
        {
            //Open party screen
            ItemSelected();
        }
        else if (Input.GetKeyDown(KeyCode.X))
        {
            Debug.Log("Back en debug batalla");

            debugBack?.Invoke();
        }
    }

    public void UpdateItemSelection()
    {

        for (int i = 0; i < menuItems.Count; i++)
        {
            if (i == selectedItem)
            {
                menuItems[i].color=GlobalSettings.i.HighlightedColor;

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
                Debug.Log("Activado comando saltar batalla ganar desde debug");
                CloseDebug();
                battleDebugSys.EndBattleDebug();
                break;

            case 1:
                Debug.Log("Activado comando saltar batalla perder desde debug");
                CloseDebug();
                battleDebugSys.EndBattleLoseDebug();
                break;
            case 2:
                Debug.Log("Activado comando usar masterCatcher desde debug");
                CloseDebug();
                battleDebugSys.UseMasterCatcher();
                break;
            default:
                Debug.Log("Cerrando menú debug");
                debugBack?.Invoke();
                break;
        }
    }
}
