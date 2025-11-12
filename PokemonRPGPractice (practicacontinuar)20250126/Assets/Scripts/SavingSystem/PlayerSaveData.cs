using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerSaveData
{
    // datos del jugador y su guardado
    public float[] playerPosition;
    public int playerSceneId;
    public List<SerializedCreatureData> creaturesInParty;
    //guardar objetos
    public float walletMoney;
    public List<SerializedItemData> recoveryItems;
    public List<SerializedItemData> catcherItems;
    public List<SerializedItemData> equipItems;
    public List<SerializedItemData> skillTeachingItems;
    public List <SerializedItemData> keyItems;
    //TODO:Añadir slots de equipamiento
    public Dictionary<string, string> playerSavedFlags;

    public Dictionary<int, bool> creatureDexCompletion;

    public List<List<SerializedCreatureData>> serializedBoxData;

    public PlayerSaveData(PlayerController player)
    {
        playerPosition = new float[3];
        playerPosition[0] = player.gameObject.transform.position.x;
        playerPosition[1] = player.gameObject.transform.position.y;
        playerPosition[2] = player.gameObject.transform.position.z;

        List<Creature> creatureList = player.gameObject.GetComponent<CreatureParty>().Creatures;
        creaturesInParty = new List<SerializedCreatureData>();

        foreach(Creature c in creatureList)
        {
            creaturesInParty.Add(new SerializedCreatureData(c));
        }

        walletMoney = MoneyWallet.i.MoneyG;

        List<ItemSlot> recoveryItems = Inventory.GetInventory().GetSlotsByCategory((int)ItemCategoryEnum.RECOVERY_ITEMS);
        this.recoveryItems = new List<SerializedItemData>();
        foreach (ItemSlot item in recoveryItems)
        {
            this.recoveryItems.Add(new SerializedItemData(item));
        }

        List<ItemSlot> catcherItems = Inventory.GetInventory().GetSlotsByCategory((int)ItemCategoryEnum.CATCHER_ITEMS);
        this.catcherItems = new List<SerializedItemData>();
        foreach (ItemSlot item in catcherItems)
        {
            this.catcherItems.Add(new SerializedItemData(item));
        }

        List<ItemSlot> equipItems= Inventory.GetInventory().GetSlotsByCategory((int)ItemCategoryEnum.EQUIP_ITEMS);
        this.equipItems = new List<SerializedItemData>();
        foreach (ItemSlot item in equipItems)
        {
            this.equipItems.Add(new SerializedItemData(item));
        }

        List<ItemSlot> skillTeachingItems = Inventory.GetInventory().GetSlotsByCategory((int)ItemCategoryEnum.SKILL_TEACHING_ITEMS);
        this.skillTeachingItems = new List<SerializedItemData>();
        foreach (ItemSlot item in skillTeachingItems)
        {
            this.skillTeachingItems.Add(new SerializedItemData(item));
        }

        List<ItemSlot> keyItems = Inventory.GetInventory().GetSlotsByCategory((int)ItemCategoryEnum.KEY_ITEMS);
        this.keyItems = new List<SerializedItemData>();
        foreach (ItemSlot item in keyItems)
        {
            this.keyItems.Add(new SerializedItemData(item));
        }

        serializedBoxData = CreatureBox.i.GetBoxSerializedData();

        playerSceneId = LevelLoader.instance.GetCurrentSceneId();

        playerSavedFlags=FlagManager.instance.GetFlagDictionary();

        creatureDexCompletion = CreatureDexHandler.i.CreatureDexCompletion;
    }
}
