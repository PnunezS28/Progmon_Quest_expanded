using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;

public class BootMenuUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI[] menuUI;
    [SerializeField] Button loadButton;
    List<TextMeshProUGUI> menuItems;

    int selectedItem = 0;

    private void Start()
    {
        Debug.Log("Opening debug menu to start");
        menuItems = menuUI.ToList();
        selectedItem = 0;

        UpdateItemSelection();

        if (SaveSystem.SaveFileExists() == false)
        {
            loadButton.interactable = false;
        }
    }

    private void Update()
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
            OnMenuSelected();
        }
    }

    void OnMenuSelected()
    {
        switch (selectedItem)
        {
            case 0:
                OnPressedStartNewGame(); break;
            case 1: OnPressedLoadGame(); break;
            case 2: OnPressedToggleFullScreen(); break;
            case 3: OnPressedExitGame(); break;
            default:
                Debug.Log("BootMenuUI ERROR: swlectedItem index not implemented");
                break;
        }
    }

    public void UpdateItemSelection()
    {

        for (int i = 0; i < menuItems.Count; i++)
        {
            if (i == selectedItem)
            {
                menuItems[i].color = Color.blue;

            }
            else
            {
                menuItems[i].color = Color.black;
            }
        }

    }

    #region button hook
    public void OnPressedStartNewGame()
    {
        BootMenuManager.instance.StartNewGame();
    }

    public void OnPressedLoadGame()
    {
        BootMenuManager.instance.LoadGame();
    }

    public void OnPressedToggleFullScreen()
    {
        BootMenuManager.instance.ToggleFullScreen();
    }

    public void OnPressedExitGame()
    {
        BootMenuManager.instance.ExitGame();
    }
    #endregion
}
