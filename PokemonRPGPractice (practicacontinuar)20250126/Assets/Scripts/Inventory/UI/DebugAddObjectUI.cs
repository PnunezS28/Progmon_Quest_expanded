using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class DebugAddObjectUI : MonoBehaviour
{
    [SerializeField] GameObject menuUI;

    [SerializeField] GameObject menu;
    List<TextMeshProUGUI> menuItems;

    [SerializeField]
    DebugAddObject debugSys;
    [SerializeField] int moneyAmountIncrement = 10;

    int selectedItem = 0;

    //Create creature
    int selectedCreatureId = 1;
    int selectedCreatureLevel = 1;

    //create item
    int selectedItemIndex = 1;
    int selectedItemAmount = 1;

    List<ItemBase> itemList = new List<ItemBase>();

    //create money
    int selectedMoney = 10;

    int maxCreatureIdCount;
    int maxItemIdCount;

    Action debugBack;
    
    public void OpenDebugUI()
    {
        Debug.Log("Opening debug");

        menuUI.SetActive(true);
        menuItems = menu.GetComponentsInChildren<TextMeshProUGUI>().ToList();
        if (menuItems == null)
        {
            Debug.LogError("ERROR: DebugAddObjectUI, menuItems is null");
            return;
        }
        maxCreatureIdCount = CreatureDexHandler.i.getAllCretaureDexEntries().Count();
        Debug.Log($"DebugAddObjectUI, maxCreatureIdCount= {maxCreatureIdCount}");


        itemList=CreatureDexHandler.i.GetAllItemBase();
        maxItemIdCount = itemList.Count();
        Debug.Log($"DebugAddObjectUI, maxItemIdCount= {maxItemIdCount}");
        //string selectedCreatureName = CreatureDexHandler.i.GetCreatureBase(selectedCreatureId).Name;

        menuItems[0].text = $"Select Creature ID <{selectedCreatureId}>";
        menuItems[1].text = $"Select Creature level <{selectedCreatureLevel}>";
        menuItems[6].text = $"Select money Amount < {selectedMoney} > G";
    }

    public void CloseDebugUI()
    {
        Debug.Log("Closign Debug");

        menuUI.SetActive(false);
        selectedItem = 0;
        selectedCreatureId = 1;
        selectedCreatureLevel = 1;
        selectedItemIndex = 1;
        selectedItemAmount = 1;
        selectedMoney = 10;

    }


    // Update is called once per frame
    public void HandleUpdate(Action OnBack)
    {
        debugBack = OnBack;
        int prevSelection = selectedItem;

        int lastCreatureId = selectedCreatureId;
        int lastItemId = selectedItemIndex;

        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            ++selectedItem;
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            --selectedItem;
        }

        //selectedCategory = Mathf.Clamp(selectedCategory, 0, Inventory.ItemCategories.Count-1);
        //Algo es null
        selectedItem = Mathf.Clamp(selectedItem, 0, menuItems.Count - 1);
        //todo cambiar para no hardcodear list


        if (prevSelection != selectedItem)
        {
            UpdateItemSelection();
        }

        //Seleccionar Id criatura
        if (selectedItem == 0)
        {
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                --selectedCreatureId;
            }
            else if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                ++selectedCreatureId;
            }
            selectedCreatureId = Mathf.Clamp(selectedCreatureId, 1, maxCreatureIdCount);
            string selectedCreatureName = "";
            if (lastCreatureId != selectedCreatureId)
            {
                selectedCreatureName = CreatureDexHandler.i.GetCreatureBase(selectedCreatureId).Name;
            }
            if (selectedCreatureName.Equals("") == false)
                menuItems[0].text = $"Select Creature ID < {selectedCreatureId} > = {selectedCreatureName}";
        }
        //Seleccionar nivel criatura
        if (selectedItem == 1)
        {
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                --selectedCreatureLevel;
            }
            else if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                ++selectedCreatureLevel;
            }
            selectedCreatureLevel = Mathf.Clamp(selectedCreatureLevel, 1, CreatureBase.MaxCreatureLevel);
            menuItems[1].text = $"Select Creature level < {selectedCreatureLevel} >";
        }

        if (selectedItem == 3)
        {
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                --selectedItemIndex;
            }
            else if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                ++selectedItemIndex;
            }
            selectedItemIndex = Mathf.Clamp(selectedItemIndex, 1, maxItemIdCount-1);
            string selectedItemName = "";
            if (lastItemId != selectedItemIndex)
            {
                //OutofRange
                selectedItemName = itemList[selectedItemIndex].ItemName;
            }
            if (selectedItemName.Equals("") == false)
                menuItems[3].text = $"Select item ID < {selectedItemIndex} > = {selectedItemName}";
        }

        if (selectedItem == 4)
        {
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                --selectedItemAmount;
            }
            else if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                ++selectedItemAmount;
            }
            selectedItemAmount = Mathf.Clamp(selectedItemAmount, 1, 99);

            menuItems[4].text = $"Select Item Amount < {selectedItemAmount} >";
        }

        if (selectedItem == 6)
        {
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                selectedMoney -= moneyAmountIncrement;
            }
            else if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                selectedMoney += moneyAmountIncrement;
            }
            selectedMoney = Mathf.Clamp(selectedMoney, 10, 999990);

            menuItems[6].text = $"Select money Amount < {selectedMoney} > G";
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

    void ItemSelected()
    {
        switch (selectedItem)
        {
            case 2:
                //Generate creature
                debugSys.AddNewCreatureToPlayer(selectedCreatureId, selectedCreatureLevel);
                break;
            case 5:
                //generate item
                debugSys.AddNewObjectToPlayer(itemList[selectedItemIndex].ItemDexId, selectedItemAmount);
                break;
            case 6:
                //Generate Money
                debugSys.AddMoneyToPlayer(selectedMoney);
                break;
            case 7:
                //Close
                debugBack?.Invoke();
                break;
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
}
