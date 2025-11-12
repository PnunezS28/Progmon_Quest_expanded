using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopUI : MonoBehaviour
{
    //TODO: Añadir cuadro que explique los controles, aun si es redundante

    //controla el UI de la pantalla de inventario y lista los objetos en posesión del jugador
    [SerializeField] GameObject itemList;
    //para hacer el scroll automático se va a mover el transform y para suvir o bajar la lista
    [SerializeField] ItemSlotUI itemSlotUIPrefab;

    [SerializeField] Image itemIcon;
    [SerializeField] TextMeshProUGUI itemDescription;


    List<ItemSlotUI> shopUIList;
    int selectedItem = 0;

    RectTransform itemListRect;

    const int itemsInViewPort = 8;

    List<ItemBase> avalableItems;

    Action<ItemBase> onItemSelected;
    Action onBack;

    private void Awake()
    {
        Debug.Log("ShopUI OnAwake");
        itemListRect = itemList.GetComponent<RectTransform>();
    }

    public void Show(List<ItemBase> avalableItems,Action<ItemBase> onItemSelected,Action onBack)
    {
        this.avalableItems = avalableItems;
        this.onItemSelected = onItemSelected;
        this.onBack = onBack;

        gameObject.SetActive(true);

        UpdateItemList();
        UpdateItemSelection();
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }

    public void HandleUpdate()
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

        selectedItem = Mathf.Clamp(selectedItem, 0, avalableItems.Count - 1);
        //todo cambiar para no hardcodear list

        if (prevSelection != selectedItem)
        {
            UpdateItemSelection();
        }

        if (Input.GetKeyDown(KeyCode.Z))
        {
            onItemSelected?.Invoke(avalableItems[selectedItem]);
        }else if (Input.GetKeyDown(KeyCode.X))
        {
            onBack?.Invoke();
        }
    }

    void UpdateItemList()
    {
        //Lipiar todos los objetos existentes de la lista e introducir un objeto por cada objeto del inventario del jugador
        foreach (Transform item in itemList.transform)
        {//Destruir todos los objetos vacios
            Destroy(item.gameObject);
        }
        Debug.Log("Updating item list");

        shopUIList = new List<ItemSlotUI>();
        foreach (var item in avalableItems)
        {//instaciar los objetos como hijos de la lista
            var instatiatedObject = Instantiate(itemSlotUIPrefab, itemList.transform);
            instatiatedObject.SetShopData(item);
            shopUIList.Add(instatiatedObject);
        }

    }

    public void UpdateItemSelection()
    {
        selectedItem = Mathf.Clamp(selectedItem, 0, avalableItems.Count - 1);

        for (int i = 0; i < avalableItems.Count; i++)
        {
            if (i == selectedItem)
            {
                shopUIList[i].SetColor(GlobalSettings.i.HighlightedColor);

            }
            else
            {
                shopUIList[i].SetColor(Color.black);
            }
        }


        if (shopUIList.Count > 0)
        {
            var selectedSlot = avalableItems[selectedItem];
            itemIcon.sprite = selectedSlot.ItemIcon;
            itemDescription.text = selectedSlot.ItemDescription;
        }
        if (shopUIList.Count > 0)
        {
            HandleScrolling();
        }
    }

    void HandleScrolling()
    {
        float scrollPos = Mathf.Clamp(selectedItem - itemsInViewPort / 2, 0, selectedItem) * shopUIList[0].Height;//scroll position

        itemListRect.localPosition = new Vector2(itemListRect.localPosition.x, scrollPos);
    }
}
