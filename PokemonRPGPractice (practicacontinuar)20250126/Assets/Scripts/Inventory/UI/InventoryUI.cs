using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    

    //controla el UI de la pantalla de inventario y lista los objetos en posesión del jugador
    [SerializeField] GameObject itemList;
    //para hacer el scroll automático se va a mover el transform y para subir o bajar la lista
    [SerializeField] ItemSlotUI itemSlotUIPrefab;

    [SerializeField] Image itemIcon;
    [SerializeField] TextMeshProUGUI itemDescription;

    [SerializeField] TextMeshProUGUI itemCategoryText;

    [SerializeField] PartyScreen partyScreen;
    [SerializeField] NewSkillSelectionUI newSkillSelection;

    Action<ItemBase> onItemUsed;

    Inventory inventory;
    List<ItemSlotUI> itemSlotUIList;
    int selectedItem = 0;
    int selectedCategory = 0;

    RectTransform itemListRect;

    const int itemsInViewPort = 8;
    InventoryUIState state;

    SkillBase skillToLearn;

    private void Awake()
    {
        inventory = Inventory.GetInventory();
        itemListRect = itemList.GetComponent<RectTransform>();
    }

    private void Start()
    {
        UpdateItemList();
        UpdateItemSelection();

        inventory.OnUpdated += UpdateItemList;
    }

    void UpdateItemList()
    {
        //itemList
        //Lipiar todos los objetos existentes de la lista e introducir un objeto por cada objeto del inventario del jugador
        foreach (Transform item in itemList.transform)
        {//Destruir todos los objetos vacios
            Destroy(item.gameObject);
        }
        Debug.Log("Updating item list");
        itemSlotUIList = new List<ItemSlotUI>();
        foreach (var itemSlot in inventory.GetSlotsByCategory(selectedCategory))
        {//instaciar los objetos como hijos de la lista
            var instatiatedObject = Instantiate(itemSlotUIPrefab, itemList.transform);
            instatiatedObject.SetData(itemSlot);
            itemSlotUIList.Add(instatiatedObject);
        }
    }

    public void HandleUpdate(Action OnBack, Action<ItemBase> onItemUsed = null)
    {
        this.onItemUsed = onItemUsed;
        if (state == InventoryUIState.ItemSelection)
        {
            int prevSelection = selectedItem;
            int prevCategory = selectedCategory;

            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                ++selectedItem;
            }
            else if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                --selectedItem;
            }
            else if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                selectedCategory++;
            }
            else if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                selectedCategory--;
            }

            if (selectedCategory > Inventory.ItemCategories.Count - 1)
            {
                selectedCategory = 0;
            }
            else if (selectedCategory < 0)
            {
                selectedCategory = Inventory.ItemCategories.Count - 1;
            }

            //selectedCategory = Mathf.Clamp(selectedCategory, 0, Inventory.ItemCategories.Count-1);
            selectedItem = Mathf.Clamp(selectedItem, 0, inventory.GetSlotsByCategory(selectedCategory).Count - 1);
            //todo cambiar para no hardcodear list


            if (prevCategory != selectedCategory)
            {
                ResetSelection();
                itemCategoryText.text = Inventory.ItemCategories[selectedCategory];
                UpdateItemList();
                UpdateItemSelection();
            }
            else if (prevSelection != selectedItem)
            {
                UpdateItemSelection();
            }


            if (Input.GetKeyDown(KeyCode.Z))
            {
                //Open party screen
                //TODO: NullreferenceException
                StartCoroutine(ItemSelected());
            }
            else if (Input.GetKeyDown(KeyCode.X))
            {
                Debug.Log("Back en bolsa");
                OnBack?.Invoke();
            }
        }
        else if (state == InventoryUIState.PartySelection)
        {
            //Handle party selection
            Action OnSelected = () =>
            {
                StartCoroutine(UseItem());
            };
            Action OnPartyScreenBack = () =>
            {
                Debug.Log("Volviendo a inventario desde party screen");
                ClosePartyScreen();
            };
            partyScreen.HandleUpdate(OnSelected, OnPartyScreenBack);
        }else if (state == InventoryUIState.SKILL_FORGET)
        {
            Action<int> onSkillSelected = (int selectedMove) =>
            {//sevuelve index del movimiento elegido
                StartCoroutine(OnMoveForgetSelected(selectedMove));
            };
            newSkillSelection.HandleSkillSelection(onSkillSelected);
        }

    }

    IEnumerator ItemSelected()
    {
        state = InventoryUIState.Busy;

        var usedItem = inventory.GetItem(selectedItem, selectedCategory);
        if (usedItem == null)
        {
            Debug.Log("InventoryUI: selected item is null");
            //se congela
            state = InventoryUIState.ItemSelection;
            onItemUsed?.Invoke(null);
            yield break;
        }

        if (GameController.Instance.GameState == GameStateEnum.BATTLE)
        {
            //impedir usar objetos que no puedan usarse en combate
            if (usedItem.CanUseInBattle == false)
            {
                //no permitir
                yield return DialogueManager.Instance.MostrarTextoDialogoSimple($"No puedes usar ese objeto en combate", returnToFreeRoam: false);
                state = InventoryUIState.ItemSelection;

                yield break;
            }
        }
        else
        {
            if (usedItem.CanUseOutsideBattle == false && GameController.Instance.GameState != GameStateEnum.SHOP)
            {
                //no permitir
                yield return DialogueManager.Instance.MostrarTextoDialogoSimple($"No puedes usar ese objeto fuera de combate", returnToFreeRoam: false);
                state = InventoryUIState.ItemSelection;

                yield break;
            }
            
        }
            //fuera de combate
            if (GameController.Instance.GameState == GameStateEnum.SHOP)
            {
                onItemUsed?.Invoke(usedItem);
                state = InventoryUIState.ItemSelection;
                yield break;
            }
            //Si el objeto es de recuperación abre la pantalla de equipo, si es un objeto de captura, lanza el atrapador directamente
            if (selectedCategory == (int)ItemCategoryEnum.CATCHER_ITEMS)
            {
                StartCoroutine(UseItem());
                state = InventoryUIState.ItemSelection;
                //catcher
            }
            else
            {
                //otra cosa
                OpenPartyScreen();
            }
    }
    bool stopTmUse = false;
    IEnumerator UseItem()
    {
        state = InventoryUIState.Busy;
        //utilizar objeto elegido en la criatura
        Debug.Log("Intentando usar objeto");
        yield return HandleSkillTeachingItem();

        var usedItem = inventory.UseItem(selectedItem, partyScreen.SelectedMember, selectedCategory,stopTmUse);//Utilizar objeto A en criatura B
        stopTmUse = false;


        if (usedItem != null)
        {
            //TODO: ajustar cuando suena el sfx de item use

            if (usedItem is RecoveryItem||usedItem is EquipBattleItem)
            {
                AudioManager.i.PlaySfx(AudioId.itemUse);
                yield return DialogueManager.Instance.MostrarTextoDialogoSimple($"{usedItem.ItemUsedText}", returnToFreeRoam: false);
            }
            ClosePartyScreen();
            onItemUsed?.Invoke(usedItem);
        }
        else
        {
            //no ha podido usarse
            if(usedItem is RecoveryItem)
                yield return DialogueManager.Instance.MostrarTextoDialogoSimple($"No tendría efecto", returnToFreeRoam: false);

            if (usedItem is CatcherItem)
            {
                state = InventoryUIState.ItemSelection;
            }
            else
            {
                state = InventoryUIState.PartySelection;
            }
        }
    }

    IEnumerator HandleSkillTeachingItem()
    {
        //con casting usando ass. se volverá nulo si la variable que intentas castear no es correcto
        var item = inventory.GetItem(selectedItem,selectedCategory) as SkillTeachingItem;
        //Aquí falla un poco

        if (item == null)
        {//No es objeto de enseñanza
            yield break;
        }
        //es objeto de enseñanza
        AudioManager.i.PlaySfx(AudioId.itemUse);
        Creature selectedCreature = partyScreen.SelectedMember;

        if (selectedCreature.HasSkill(item.Skill))
        {
            yield return DialogueManager.Instance.MostrarTextoDialogoSimple($"{selectedCreature.Base.Name} ya posee habilidad {item.Skill.SkillName}", returnToFreeRoam: false);
            stopTmUse = true;
            yield break;
        }

        if (item.CanBeTaught(selectedCreature)==false)
        {
            yield return DialogueManager.Instance.MostrarTextoDialogoSimple($"{selectedCreature.Base.Name} no puede aprender {item.Skill.SkillName}", returnToFreeRoam: false);
            stopTmUse = true;

            yield break;
        }

        if (selectedCreature.Skills.Count < CreatureBase.MaxNumOfSkills)
        {//se puede añadir sin problemas
            selectedCreature.LearnSkill(item.Skill);
            yield return DialogueManager.Instance.MostrarTextoDialogoSimple($"¡{selectedCreature.Base.Name} aprendió la habilidad {item.Skill.SkillName}!", returnToFreeRoam: false);
        }
        else
        {
            //hacer espacio para movimiento nuevo

            yield return DialogueManager.Instance.MostrarTextoDialogoSimple($"Tu {selectedCreature.Base.Name} está intentando aprender {item.Skill.SkillName}", returnToFreeRoam: false);
            yield return DialogueManager.Instance.MostrarTextoDialogoSimple($"Pero no puede tener más de {CreatureBase.MaxNumOfSkills} habilidades", returnToFreeRoam: false);
            Debug.Log($"The player's {selectedCreature.Base.Name} has reached maximum number of skills and must forget one to acquire the {item.Skill.SkillName} skill");
            yield return ChoseSkillToForget(selectedCreature, item.Skill);

            yield return new WaitUntil(()=>state != InventoryUIState.SKILL_FORGET);
        }
    }

    IEnumerator ChoseSkillToForget(Creature creature, SkillBase newSkill)
    {
        state = InventoryUIState.Busy;
        yield return DialogueManager.Instance.MostrarTextoDialogoSimple($"Elige la habilidad que quieres olvidar", returnToFreeRoam: false);

        newSkillSelection.gameObject.SetActive(true);
        //Linq que permite devolver una lista con los datos elegidos
        newSkillSelection.SetSkillData(creature.Skills.Select(x => x.SkillBase).ToList(), newSkill);
        skillToLearn = newSkill;
        state = InventoryUIState.SKILL_FORGET;
    }

    public void UpdateItemSelection()
    {
        var selectedSlots = inventory.GetSlotsByCategory(selectedCategory);
        selectedItem = Mathf.Clamp(selectedItem, 0, selectedSlots.Count - 1);

        for (int i = 0; i < itemSlotUIList.Count; i++)
        {
            if (i == selectedItem)
            {
                itemSlotUIList[i].SetColor(GlobalSettings.i.HighlightedColor);

            }
            else
            {
                itemSlotUIList[i].SetColor(Color.black);
            }
        }


        if (selectedSlots.Count > 0)
        {
            var selectedSlot = selectedSlots[selectedItem];
            itemIcon.sprite = selectedSlot.Item.ItemIcon;
            itemDescription.text = selectedSlot.Item.ItemDescription;
        }
        if (selectedSlots.Count > 0)
        {
            HandleScrolling();
        }
    }

    void HandleScrolling()
    {
        float scrollPos = Mathf.Clamp(selectedItem - itemsInViewPort / 2, 0, selectedItem) * itemSlotUIList[0].Height;//scroll position

        itemListRect.localPosition = new Vector2(itemListRect.localPosition.x, scrollPos);
    }

    void OpenPartyScreen()
    {
        state = InventoryUIState.PartySelection;
        partyScreen.gameObject.SetActive(true);


    }

    void ClosePartyScreen()
    {
        state = InventoryUIState.ItemSelection;
        partyScreen.gameObject.SetActive(false);
    }

    void ResetSelection()
    {
        selectedItem = 0;
        itemIcon.sprite = null;
        itemDescription.text = "";
    }


    IEnumerator mensajeOlvidar(string skillOlvidar, string skillLearn, string name)
    {
        yield return DialogueManager.Instance.MostrarTextoDialogoSimple($"{name} olvidó {skillOlvidar}", returnToFreeRoam: false);
        yield return DialogueManager.Instance.MostrarTextoDialogoSimple($"{name} aprendió {skillLearn}", returnToFreeRoam: false);
    }

    IEnumerator OnMoveForgetSelected(int selectedMove)
    {
        var creature = partyScreen.SelectedMember;
        newSkillSelection.gameObject.SetActive(false);
        //olvidar habilidad elegida

        if (selectedMove == CreatureBase.MaxNumOfSkills)
        {
            //no aprender habilidad nueva
            yield return (DialogueManager.Instance.MostrarTextoDialogoSimple($"{creature.Base.Name} no aprendió {skillToLearn.SkillName}", returnToFreeRoam: false));
        }
        else
        {
            //olvidar habilidad seleccionada y aparender nueva
            var selectedSkill = creature.Skills[selectedMove].SkillBase;
            string skillForgetName = selectedSkill.SkillName;
            string skillLearnName = skillToLearn.SkillName;

            creature.Skills[selectedMove] = new Skill(skillToLearn);
            yield return (mensajeOlvidar(skillForgetName, skillLearnName, creature.Base.Name));

        }
        skillToLearn = null;//resetear a null
        state = InventoryUIState.ItemSelection;
    }
}
