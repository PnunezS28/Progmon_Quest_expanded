using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiPickUpUI : MonoBehaviour
{

    //controla el UI de la pantalla de inventario y lista los objetos en posesión del jugador
    [SerializeField] GameObject itemList;
    //para hacer el scroll automático se va a mover el transform y para suvir o bajar la lista
    [SerializeField] MultiItemSlotUI itemSlotUIPrefab;

    Action OnClose;

    // Start is called before the first frame update
    public void ShowUI(ItemBase[] items, Action OnClose)
    {
        this.gameObject.SetActive(true);
        clearList();

        this.OnClose = OnClose;

        foreach(var itemSlot in items) 
        {
            var instanciatedObject = Instantiate(itemSlotUIPrefab, itemList.transform);
            instanciatedObject.SetData(itemSlot);
        }
    }

    public void CloseUI()
    {
        this.gameObject.SetActive(false);
    }

    void clearList()
    {
        foreach (Transform item in itemList.transform)
        {//Destruir todos los objetos vacios
            Destroy(item.gameObject);
        }
    }

    // Update is called once per frame
    public void HandleUpdate()
    {
        //Cuando se pulse Z o X se ciuerra y se añaden los objetos
        if(Input.GetKeyDown(KeyCode.Z)|| Input.GetKeyDown(KeyCode.X)) 
        {
            OnClose?.Invoke();
        }
        
    }
}
