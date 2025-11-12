using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class MenuController : MonoBehaviour
{
    [SerializeField] GameObject menuGroup;
    [SerializeField] GameObject menu;
    [SerializeField] GameObject menuIcon;
    [SerializeField] WalletUI walletUI;
    List<TextMeshProUGUI> menuItems;

    public event Action<int> OnMenuSelected;
    public event Action OnBack;

    int selectedItem = 0;

    private void Awake()
    {
        menuItems= menu.GetComponentsInChildren<TextMeshProUGUI>().ToList();
    }

    public void OpenMenu()
    {
        AudioManager.i.PlaySfx(AudioId.OpenMenu);
        menuGroup.SetActive(true);
        menuIcon.SetActive(false);
        walletUI.Show();
        UpdateItemSelection();
    }

    public void CloseMenu()
    {
        menuIcon.SetActive(true);
        AudioManager.i.PlaySfx(AudioId.CloseMenu);
        walletUI.Close();

        menuGroup.SetActive(false);
    }

    public void HandleUpdate()
    {
        int prevSelection = selectedItem;
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            ++selectedItem;
        }else if (Input.GetKeyDown(KeyCode.UpArrow))
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
            OnMenuSelected?.Invoke(selectedItem);
        }else if(Input.GetKeyDown(KeyCode.X))
        {
            Debug.Log("Selected menu back");
            OnBack?.Invoke();
            CloseMenu();
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
