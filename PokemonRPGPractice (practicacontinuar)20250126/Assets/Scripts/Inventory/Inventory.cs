using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    // lista de objetos en posesión del jugador
    [SerializeField] List<ItemSlot> recoveryItemSlots;
    [SerializeField] List<ItemSlot> catcherItemSlots;
    [SerializeField] List<ItemSlot> equipItemSlots;
    [SerializeField] List<ItemSlot> skillTeachingItemSlots;
    [SerializeField] List<ItemSlot> keyItemSlots;

    List<List<ItemSlot>> allSlots;

    //patrón observador
    public event Action OnUpdated;

    public static Inventory GetInventory()
    {
        return FindObjectOfType<PlayerController>().GetComponent<Inventory>();
    }


    private void Awake()
    {
        allSlots = new List<List<ItemSlot>>() { recoveryItemSlots, catcherItemSlots, equipItemSlots, skillTeachingItemSlots, keyItemSlots };
    }

    public static List<string> ItemCategories { get; set; } = new List<string>()
    { "RECOVERY_ITEMS","CATCHER_ITEMS","EQUIP_ITEMS","SKILL_TEACHING_ITEMS", "KEY_ITEMS"};

    public List<ItemSlot> GetSlotsByCategory(int categoryIndex)
    {
        return allSlots[categoryIndex];
    }

    public void SetInventory(List<SerializedItemData> recoveryItems,List<SerializedItemData> catcherItems, List<SerializedItemData> equipItems,List<SerializedItemData> skillTeachingItems,List<SerializedItemData> keyItems)
    {
        List<ItemSlot> newRecover = new List<ItemSlot>();
        foreach (SerializedItemData item in recoveryItems)
        {
            newRecover.Add(new ItemSlot(item));
        }
        recoveryItemSlots = newRecover;
        List<ItemSlot> newCatcherItemSlots = new List<ItemSlot>();
        foreach (SerializedItemData item in catcherItems)
        {
            newCatcherItemSlots.Add(new ItemSlot(item));

        }
        catcherItemSlots = newCatcherItemSlots;
        //TODO: añadir a los objetos equipables

        List<ItemSlot> newEquipItemSlots = new List<ItemSlot>();
        foreach (SerializedItemData item in equipItems)
        {
            newEquipItemSlots.Add(new ItemSlot(item));

        }
        equipItemSlots = newEquipItemSlots;
        //TODO add serialized skill teaching item slots

        List<ItemSlot> newSkilTeachingItemSlots = new List<ItemSlot>();
        foreach (SerializedItemData item in skillTeachingItems)
        {
            newSkilTeachingItemSlots.Add(new ItemSlot(item));

        }
        skillTeachingItemSlots = newSkilTeachingItemSlots;

        List<ItemSlot> newKeyItemSlots = new List<ItemSlot>();
        foreach (SerializedItemData item in keyItems)
        {
            newKeyItemSlots.Add(new ItemSlot(item));

        }
        keyItemSlots = newKeyItemSlots;
        allSlots = new List<List<ItemSlot>>() { recoveryItemSlots, catcherItemSlots, equipItemSlots, skillTeachingItemSlots, keyItemSlots };

        OnUpdated?.Invoke();
    }

    public ItemBase UseItem(int indexSelectedItem,Creature selectedCreature,int selectedCategory,bool stopTmUse=false)
    {
        //devuelve el objeto para poder mostrar diálogo personalizado
        var itemToUse= GetItem(indexSelectedItem,selectedCategory);
        bool itemUsed=itemToUse.Use(selectedCreature);
        if (itemUsed && stopTmUse==false)
        {
            //reducir su cantidad en inventario
            RemoveItem(itemToUse,selectedCategory,1);
            
            return itemToUse;
        }

        return null;
    }

    public void AddItem(ItemBase item, int count=1)
    {
        //buscar categoría de objeto
        var category= (int) GetCategoryOfItem(item);
        var currentSlots = GetSlotsByCategory(category);
        //comprobar si ya tiene slot
        var itemSlot = currentSlots.Find(slot => slot.Item == item);

        if (itemSlot != null)
        {
            //ya existe en el inventario
            itemSlot.CountInPossesion += count;
        }
        else
        {
            //añadir nuevo slot
            currentSlots.Add(new ItemSlot()
            {
                Item = item,
                CountInPossesion = count
            });
        }
        OnUpdated?.Invoke();
    }

    ItemCategoryEnum GetCategoryOfItem(ItemBase item)
    {
        if(item is RecoveryItem)
        {
            return ItemCategoryEnum.RECOVERY_ITEMS;
        }
        else if(item is CatcherItem)
        {
            return ItemCategoryEnum.CATCHER_ITEMS;
        }else if(item is EquipBattleItem)
        {
            return ItemCategoryEnum.EQUIP_ITEMS;
        }else if(item is SkillTeachingItem)
        {
            return ItemCategoryEnum.SKILL_TEACHING_ITEMS;
        }else if(item is KeyItem)
        {
            return ItemCategoryEnum.KEY_ITEMS;
        }
        //else if is TMs return tms

        //por defecto recovery items, TODO crear página objetos clave
        return ItemCategoryEnum.RECOVERY_ITEMS;
    }

    public void RemoveItem(ItemBase item, int selectedCategory,int amount=1)
    {
        var currentSlots = GetSlotsByCategory(selectedCategory);
        var itemSlot = currentSlots.Find(slot => slot.Item == item);
        //item a reducir
        itemSlot.CountInPossesion-=amount;
        Debug.Log($"{item.ItemName} Item was used, now left {itemSlot.CountInPossesion}");
        if (itemSlot.CountInPossesion == 0)
        {
            currentSlots.Remove(itemSlot);
        }
        OnUpdated?.Invoke();
    }
    public void RemoveItem(ItemBase item, int amount = 1)
    {
        var category = (int)GetCategoryOfItem(item);
        var currentSlots = GetSlotsByCategory(category);
        //comprobar si ya tiene slot
        var itemSlot = currentSlots.Find(slot => slot.Item == item);



        //item a reducir
        itemSlot.CountInPossesion-=amount;
        Debug.Log($"{item.ItemName} Item was used, now left {itemSlot.CountInPossesion}");
        if (itemSlot.CountInPossesion == 0)
        {
            currentSlots.Remove(itemSlot);
        }
        OnUpdated?.Invoke();
    }

    public int getItemCount(ItemBase item)
    {
        var category = (int)GetCategoryOfItem(item);
        var currentSlots = GetSlotsByCategory(category);
        //comprobar si ya tiene slot
        var itemSlot = currentSlots.Find(slot => slot.Item == item);
        if (itemSlot != null)
        {
            return itemSlot.CountInPossesion;
        }
        else
        {
            return 0;
        }
    }
    public ItemBase GetItem(int indexSelectedItem, int selectedCategory)
    {
        List<ItemSlot> currentSlots = GetSlotsByCategory(selectedCategory);
        if (currentSlots.Count == 0)
        {
            Debug.Log("Inventory: current slot is empty");
            return null;
        }
        var itemToUse = currentSlots[indexSelectedItem].Item;

        return itemToUse;
    }
}
