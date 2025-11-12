using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(NPCController))]
public class NPCShop : MonoBehaviour
{
    //dialogo
    [TextArea(1,3)]
    [SerializeField]public  string  dialogoInicio;
    //choices convertir en elemento de UI del UICanvas y GameController
    [TextArea(1,3)]
    [SerializeField]public string dialogoCierre;

    //shoplist

    [SerializeField] List<ItemBase> avalableItems;

    public void Trade(NPCController npc)
    {
        //Activa el diálogo y luego el choices de tienda, comnprueba en memoria si el valor de open sop es 0 para abrir, 1 para vender y 2 para cerrar en el action onFinished
        
         StartCoroutine(ShopController.i.OpenShop(npc));

    }

    public List<ItemBase> AvalableItems => avalableItems;
}
