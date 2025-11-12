using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public enum ShopState { Menu,Buying,Selling,Busy,SelectingToSell,SelectingToBuy}

public class ShopController : MonoBehaviour
{
    [SerializeField] GameObject ShopUiGroup;
    [SerializeField] GameObject SelectionChooseActionUI;
    [SerializeField] InventoryUI inventorySell;
    [SerializeField] GameObject chooseToSellArea;
    [SerializeField] CountSelectorUI countSelectorUI;

    [SerializeField] WalletUI walletUI;

    [SerializeField] ShopUI shopUI;

    public event Action OnStartShopping;
    public event Action OnFinishShopping;


    public static ShopController i { get; private set; }
    NPCController openShop;
    TextMeshProUGUI[] optionsInAction;
    TextMeshProUGUI[] optionsInChoosingToSell;


    ShopState shopState;
    int selectionAction = 0;
    int selectionChooseToSell = 0;

    private void Awake()
    {
        i = this;
    }
    Inventory inventory;
    public void Init()
    {
        //falla cuando se hace carga de juego desde Boot
        inventory = Inventory.GetInventory();
    }
    public void HandleUpdate()
    {
        if (shopState == ShopState.Menu)
        {
            int prevSelection = selectionAction;

            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                ++selectionAction;
            }
            else if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                --selectionAction;
            }

            selectionAction = Mathf.Clamp(selectionAction, 0, optionsInAction.Length - 1);

            if (prevSelection != selectionAction)
            {
                UpdateSelectorSelection(optionsInAction, selectionAction);
            }

            if (Input.GetKeyDown(KeyCode.Z))
            {
                //Open party screen
                StartCoroutine( OnActionSelected());
            }
        }else if (shopState == ShopState.Selling)
        {
            inventorySell.HandleUpdate(OnBackFromSelling,(selectedItem)=> 
            {
                Debug.Log("SHOPController: OnselectedItem Shop");
                if(selectedItem == null)
                {
                    Debug.Log("SHOP: Selected item is empty");
                    shopState = ShopState.Selling;
                }
                else
                {
                    StartCoroutine( SellItem(selectedItem));
                }
            });
        }else if (shopState == ShopState.SelectingToSell||shopState==ShopState.SelectingToBuy)
        {
            int prevSelection = selectionChooseToSell;

            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                ++selectionChooseToSell;
            }
            else if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                --selectionChooseToSell;
            }

            selectionChooseToSell = Mathf.Clamp(selectionChooseToSell, 0, optionsInChoosingToSell.Length-1);

            if (prevSelection != selectionChooseToSell)
            {
                UpdateSelectorSelection(optionsInChoosingToSell, selectionChooseToSell);
            }

            if (Input.GetKeyDown(KeyCode.Z))
            {
                Debug.Log("ShopControler: accept select shop");
                //Open party screen
                //StartCoroutine(OnActionSelected());
                if (shopState == ShopState.SelectingToSell)
                {
                    StartCoroutine( SellItemDecided());
                }else if(shopState == ShopState.SelectingToBuy)
                {
                    StartCoroutine(BuyItemDecided());
                }
            }
            if (Input.GetKeyDown(KeyCode.X))
            {
                Debug.Log("ShopControler: cancel selecting shop");
                //cancelar
                if (shopState == ShopState.SelectingToSell)
                {
                    chooseToSellArea.SetActive(false);
                    selectedItem = null;
                    walletUI.Close();
                    shopState = ShopState.Selling;
                }
                else if (shopState == ShopState.SelectingToBuy)
                {
                    chooseToSellArea.SetActive(false);
                    selectedItem = null;
                    walletUI.Close();
                    shopState = ShopState.Buying;
                }
            }
        }else if (shopState == ShopState.Buying)
        {
            shopUI.HandleUpdate();
        }
        
        
    }

    public IEnumerator OpenShop(NPCController shop)
    {
        GameController.Instance.StartShortDialogue();
        yield return DialogueManager.Instance.MostrarTextoDialogoSimple(shop.NPCShop.dialogoInicio, returnToFreeRoam: false);
        this.openShop = shop;
        OnStartShopping?.Invoke();
        StartMenuState();
    }

    void StartMenuState()
    {
        ShopUiGroup.gameObject.SetActive(true);
        SelectionChooseActionUI.gameObject.SetActive(true);
        optionsInAction = transform.GetComponentsInChildren<TextMeshProUGUI>();
        UpdateSelectorSelection(optionsInAction, selectionAction);
        shopState = ShopState.Menu;
    }

    public void UpdateSelectorSelection(TextMeshProUGUI[] options,int selected)
    {
        for (int i = 0; i < options.Length; i++)
        {
            if (i == selected)
            {
                options[i].color=GlobalSettings.i.HighlightedColor;

            }
            else
            {
                options[i].color = Color.black;
            }
        }
    }

    public IEnumerator OnActionSelected()
    {
        switch (selectionAction)
        {
            case 0:
                Debug.Log("Abriendo tienda");
                shopState = ShopState.Buying;
                walletUI.Show();
                SelectionChooseActionUI.gameObject.SetActive(false);
                shopUI.Show(openShop.NPCShop.AvalableItems,(selectedItem)=> StartCoroutine(BuyItem(selectedItem)),OnBackFromBuying);

                break;
            case 1:
                Debug.Log("Abrir vender");
                inventorySell.gameObject.SetActive(true);
                shopState = ShopState.Selling;
                SelectionChooseActionUI.SetActive(false);

                break;
            case 2:
                Debug.Log("Cerrando dialogo tienda");
                GameController.Instance.StartShortDialogue();
                yield return DialogueManager.Instance.MostrarTextoDialogoSimple(openShop.NPCShop.dialogoCierre);
                SelectionChooseActionUI.SetActive(false);
                ShopUiGroup.gameObject.SetActive(false);

                //GameController.Instance.CloseShop();
                OnFinishShopping?.Invoke();
                break;
        }
        
    }

    void OnBackFromSelling()
    {
        inventorySell.gameObject.SetActive(false);
        StartMenuState();
    }

    ItemBase selectedItem;
    int amountTosell;
    IEnumerator SellItem(ItemBase itemToSell)
    {
        shopState = ShopState.Busy;

        Debug.Log("SHOPCONTROLLER: Vendiendo objeto: "+itemToSell.ItemName);
        if (itemToSell.IsSellable==false)
        {
            Debug.Log("SHOPCONTROLLER: este objeto no puede ser vendido");

            yield return DialogueManager.Instance.MostrarTextoDialogoSimple("No puedes vender este objeto",returnToFreeRoam:false);
            shopState = ShopState.Selling;
            yield break;
        }

        //se puede vender

        walletUI.Show();
        float sellingPrice = Mathf.Round(itemToSell.ShopPrice / 2);
        int itemCount=inventory.getItemCount(itemToSell);
        int countToSell = 1;
        bool cancelOperation=false;

        if (itemCount > 1)
        {
            yield return DialogueManager.Instance.MostrarTextoDialogoSimple($"¿Cuantos quieres vender?",false, returnToFreeRoam: false);

            yield return countSelectorUI.ShowSelector(itemCount,sellingPrice,
                (selectedCount,cancel)=> {
                    countToSell = selectedCount;
                    cancelOperation = cancel;
                    });
        }

        if(cancelOperation)
        {
            shopState = ShopState.Selling;
            Debug.Log("ShopControler: Canceling operation");
            yield break;
        }

        sellingPrice = sellingPrice * countToSell;

        Debug.Log("SHOPCONTROLLER: vendiendo por valor de: "+sellingPrice);
        if (countToSell > 1)
        {
            yield return DialogueManager.Instance.MostrarTextoDialogoSimple($"Estos objetos pueden ser vendidos por un total de: {sellingPrice} G.", returnToFreeRoam: false);
        }
        else
        {
            yield return DialogueManager.Instance.MostrarTextoDialogoSimple($"Este objeto puede ser vendido por: {sellingPrice} G.", returnToFreeRoam: false);
        }
        
        chooseToSellArea.SetActive(true);
        optionsInChoosingToSell= chooseToSellArea.transform.GetComponentsInChildren<TextMeshProUGUI>();

        optionsInChoosingToSell[0].text = $"Vender({sellingPrice} G)";
        shopState = ShopState.SelectingToSell;
        //pasar información a seleccion a decidirse si vender o no
        selectedItem = itemToSell;
        amountTosell = countToSell;
    }

    IEnumerator SellItemDecided()
    {
        //Se ejecuta esta parte si aceptó, si no, la salta
        if (selectionChooseToSell == 0)
        {
            shopState = ShopState.Busy;
            if (selectedItem == null)
            {
                Debug.Log("SHOP: slected item is empty, sellItemDecided");
                shopState = ShopState.Selling;
                yield break;
            }
            inventory.RemoveItem(selectedItem,amount:amountTosell);
            //TODO: añadir dinero
            MoneyWallet.i.Addmoney(Mathf.Round(selectedItem.ShopPrice / 2)*amountTosell);
            yield return DialogueManager.Instance.MostrarTextoDialogoSimple($"Vendiste {amountTosell} {selectedItem.ItemName} por {Mathf.Round(selectedItem.ShopPrice / 2)*amountTosell} G", returnToFreeRoam: false);
            Debug.Log($"Item was sold for {Mathf.Round(selectedItem.ShopPrice / 2)*amountTosell} G");

        }
        chooseToSellArea.SetActive(false);
        selectedItem = null;
        walletUI.Close();
        shopState = ShopState.Selling;

    }

    IEnumerator BuyItem(ItemBase itemToBuy)
    {
        shopState = ShopState.Busy;
        Debug.Log("SHOPCONTROLLER: Comprando objeto: " + itemToBuy.ItemName);

        
        float buyingPrice =itemToBuy.ShopPrice;
        int countToBuy = 1;
        bool canceloperation = false;

        if (MoneyWallet.i.HasMoney(buyingPrice))
        {
            yield return DialogueManager.Instance.MostrarTextoDialogoSimple($"¿Cuantos quieres comprar?", false, returnToFreeRoam: false);
            int maxBuyCount = Mathf.FloorToInt(MoneyWallet.i.MoneyG/buyingPrice);

            yield return countSelectorUI.ShowSelector(maxBuyCount, buyingPrice,
                (selectedCount,cancel) => {
                    countToBuy = selectedCount;
                    canceloperation = cancel;
                    });


            if (canceloperation)
            {

                shopState = ShopState.Buying;
                yield break;
            }
            buyingPrice = buyingPrice * countToBuy;

            Debug.Log("SHOPCONTROLLER: comprando por valor de: " + buyingPrice);


            if (countToBuy > 1)
            {
                yield return DialogueManager.Instance.MostrarTextoDialogoSimple($"Estos objetos pueden ser comprados por un total de: {buyingPrice} G.", returnToFreeRoam: false);
            }
            else
            {
                yield return DialogueManager.Instance.MostrarTextoDialogoSimple($"Este objeto puede ser comprado por: {buyingPrice} G.", returnToFreeRoam: false);
            }

            //comprar

            chooseToSellArea.SetActive(true);
            optionsInChoosingToSell = chooseToSellArea.transform.GetComponentsInChildren<TextMeshProUGUI>();

            optionsInChoosingToSell[0].text = $"Comprar({buyingPrice} G)";
            shopState = ShopState.SelectingToBuy;
            //pasar información a seleccion a decidirse si vender o no
            selectedItem = itemToBuy;
            amountTosell = countToBuy;

        }
        else
        {
            yield return DialogueManager.Instance.MostrarTextoDialogoSimple($"¡No tienes suficiente dinero para comprarlo!", returnToFreeRoam: false);
            shopState = ShopState.Buying;
        }

    }

    IEnumerator BuyItemDecided()
    {
        //Modificar para comprar
        if (selectionChooseToSell == 0)
        {
            shopState = ShopState.Busy;
            inventory.AddItem(selectedItem, amountTosell);
            //TODO: añadir dinero
            MoneyWallet.i.Takemoney(selectedItem.ShopPrice* amountTosell);
            yield return DialogueManager.Instance.MostrarTextoDialogoSimple($"Compraste {amountTosell} {selectedItem.ItemName} por {selectedItem.ShopPrice*amountTosell} G", returnToFreeRoam: false);
            Debug.Log($"Item was bought for {selectedItem.ShopPrice} G");

        }
        chooseToSellArea.SetActive(false);
        selectedItem = null;
        walletUI.Close();
        shopState = ShopState.Buying;
    }

    void OnBackFromBuying()
    {
        shopUI.Close();
        walletUI.Close();
        StartMenuState();
    }
}
