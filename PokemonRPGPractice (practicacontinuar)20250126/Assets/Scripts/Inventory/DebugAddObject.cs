using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugAddObject : MonoBehaviour
{
    public void AddNewCreatureToPlayer(int id, int level)
    {
        CreatureBase selectedBase= CreatureDexHandler.i.GetCreatureBase(id);
        if (selectedBase == null)
        {
            Debug.LogError($"ERROR: DebugAddObject, selected creature id {id} not found");
            return;
        }
        Creature newCreature = new Creature(selectedBase, level);
        Debug.Log($"DebugAddObject, CreatureBase found generating creature id= {id}, name= {selectedBase.Name} at level= {level}");

        PlayerController.instance.GetComponent<CreatureParty>().AddCreature(newCreature);
    }

    public void AddNewObjectToPlayer(int id, int amount)
    {
        ItemBase selectedBase=CreatureDexHandler.i.GetItemBase(id);
        if (selectedBase == null)
        {
            Debug.LogError("ERROR: DebugAddObject, selected item id {id} not found");
            return;
        }
        Inventory inventory = Inventory.GetInventory();
        Debug.Log($"DebugAddObject, ItemBase found id= {id}, name= {selectedBase.ItemName}, amount= {amount}");
        inventory.AddItem(selectedBase, amount);
    }

    public void AddMoneyToPlayer(int amount)
    {
        MoneyWallet wallet = MoneyWallet.i;
        Debug.Log($"DebugAddObject, adding money {amount} G to player wallet");
        wallet.Addmoney(amount);
    }
}
