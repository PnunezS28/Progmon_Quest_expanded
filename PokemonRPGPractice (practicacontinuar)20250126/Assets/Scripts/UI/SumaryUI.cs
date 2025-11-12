using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SumaryUI : MonoBehaviour
{
    Creature _creature;

    [SerializeField] TextMeshProUGUI creatureNameText;
    [SerializeField] Image creatureSprite;
    [SerializeField] TextMeshProUGUI categoryDataText;

    [Header("Status data")]
    [SerializeField] GameObject statusUI;
    [SerializeField] TextMeshProUGUI healthText;
    [SerializeField] HPBarHandler hpBar;
    [SerializeField] TextMeshProUGUI statusText;
    [SerializeField] TextMeshProUGUI levelText;
    [SerializeField] TextMeshProUGUI EXPText;
    [SerializeField] TextMeshProUGUI EXPNextLevelText;
    [SerializeField] GameObject expBar;
    [SerializeField] Image type1Sprite;
    [SerializeField] Image type2Sprite;

    [Header("Skill data")]
    [SerializeField] GameObject skillUI;
    [SerializeField] SumarySkillUIHandler[] sumarySkills;
    [SerializeField] TextMeshProUGUI SkillPowerText;
    [SerializeField] TextMeshProUGUI SkillAccuracyText;
    [SerializeField] TextMeshProUGUI SkillTypeText;
    [SerializeField] TextMeshProUGUI SkillDetailsText;

    [Header("Statistics data")]
    [SerializeField] GameObject statsUI;
    [SerializeField] TextMeshProUGUI statHPText;
    [SerializeField] TextMeshProUGUI statAtqText;
    [SerializeField] TextMeshProUGUI statDefText;
    [SerializeField] TextMeshProUGUI statAtqSpText;
    [SerializeField] TextMeshProUGUI statDefSpText;
    [SerializeField] TextMeshProUGUI statVelText;
    [SerializeField] TextMeshProUGUI statNameEquipItem;
    [SerializeField] TextMeshProUGUI statDescriptionEquipItem;

    [Header("Evolution List")]
    [SerializeField] GameObject evolutionUI;
    //controla el UI de la pantalla de inventario y lista los objetos en posesión del jugador
    [SerializeField] GameObject evoList;
    //para hacer el scroll automático se va a mover el transform y para subir o bajar la lista
    [SerializeField] EvoSlotUI evoSlotUIPrefab;
    [SerializeField] TextMeshProUGUI textoCondicionEvo;

    List<EvoSlotUI> evoSlotUIList;
    [SerializeField] EvoConfirmUI evoConfirmUI;
    [SerializeField] GameObject evoEffectsCenter;
    [SerializeField] ParticleFXContainer evoParticleFX;
    [SerializeField] AudioClip evochangeSFX;
    [SerializeField] AudioClip evoCompleteSFX;


    bool confirmingEvo = false;


    bool busy=false;
    Dictionary<ConditionID, Color> statusColors;

    public static List<string> DataCategories { get; set; } = new List<string>()
    { "ESTADO","HABILIDADES","ESTADÍSTICAS","EVOLUCIÓN"};

    int selectedCategory = 0;
    int selection = 0;

    // Update is called once per frame
    public void HandleUpdate(Action OnBack)
    {
        if (busy)
        {
            return;
        }
        //if confirming evo handleupdate evoconfirmui return
        if(confirmingEvo)
        {
            Action onAcceptedEvo = () =>
            {
                confirmedEvolution();
            };
            Action onCancelEvo = () =>
            {
                confirmingEvo = false;
                evoConfirmUI.HideUI();
            };
            evoConfirmUI.HandleUpdate(onAcceptedEvo,onCancelEvo);
            return;
        }

        int prevCategory = selectedCategory;
        int prevSelection = selection;
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            selectedCategory++;
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            selectedCategory--;
        }

        if (selectedCategory == 1)
        {//skills
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                selection--;
            }else if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                selection++;
            }
        }

        if (selectedCategory == 2)
        {//stats and equippedItem
            if (Input.GetKeyDown(KeyCode.C))
            {
                //desequipar objeto
                Debug.Log("Desequipando objeto");
                if (_creature.EquipedItemId != -1)
                {
                    //está equipado
                    DesequiparObjetoDeCriatura(); 
                }
                else
                {
                    //no está equipado
                    StartCoroutine( MostrarMensaje("No hay ningún objeto equipado"));
                }
            }
        }

        if(selectedCategory == 3)
        {
            //TODO: testear UI evolución
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                selection--;
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                selection++;
            }

            if (Input.GetKeyDown(KeyCode.Z))
            {
                Debug.Log("Aceptar evo status");
                OpenEvoConfirm();
            }
        }


        if (selectedCategory > DataCategories.Count - 1)
        {
            selectedCategory = 0;
        }
        else if (selectedCategory < 0)
        {
            selectedCategory = DataCategories.Count - 1;
        }


        //selectedCategory = Mathf.Clamp(selectedCategory, 0, Inventory.ItemCategories.Count-1);
        if (selectedCategory == 1)
        {
            selection = Mathf.Clamp(selection, 0, _creature.Skills.Count - 1);

        }else if (selectedCategory == 3)
        {
            selection = Mathf.Clamp(selection, 0, _creature.Base.Evolutions.Count - 1);

        }
        //todo cambiar para no hardcodear list


        if (prevCategory != selectedCategory)
        {
            ResetSelection();
            categoryDataText.text = DataCategories[selectedCategory];
            UpdateDataUI();
            UpdateSkillSelection();
        }
        else if (prevSelection != selection)
        {
            if (selectedCategory == 1)
            {//Update skill UI
                UpdateSkillSelection();
            }else if (selectedCategory == 3)
            {
                UpdateEvoSelection();
            }
        }

        if (Input.GetKeyDown(KeyCode.X))
        {
            statusUI.SetActive(true);
            evolutionUI.SetActive(false);
            statsUI.SetActive(false);
            skillUI.SetActive(false);
            selectedCategory = 0;

            OnBack?.Invoke();
        }
    }

    public void SetData(Creature creature)
    {
        this._creature = creature;
        creatureNameText.text = creature.Base.Name;
        creatureSprite.sprite = creature.Base.CreatureBattleSprite;

        healthText.text = $"{creature.HP}/{creature.MaxHP}";
        hpBar.SetHP((float)creature.HP / creature.MaxHP);

        levelText.text = "Lv. " + _creature.Level;

        //Solo el hud del jugador tiene EXP
        float normalizedExp = GetNormalizedExp();

        expBar.transform.localScale = new Vector3(normalizedExp, 1, 1);

        var nextLevelExp = _creature.Base.GetExpForLevel(_creature.Level + 1);
        var expNedded = nextLevelExp - creature.Exp;
        if(_creature.Level == CreatureBase.MaxCreatureLevel) {
            //nextLevelExp = 0;
            expNedded = 0;
        }
        EXPText.text = $"Total exp.: {creature.Exp}";
        EXPNextLevelText.text = $"Exp for next level: {expNedded}";

        statusColors = GlobalSettings.i.StatusColors;

        SetStatusText();

        type1Sprite.sprite = GlobalSettings.i.CreatureTypeUISprites[creature.Base.Type1];
        type2Sprite.sprite = GlobalSettings.i.CreatureTypeUISprites[creature.Base.Type2];


        //set skills
        var creatureSkill = creature.Skills;
        for(int i=0;i< sumarySkills.Length;i++)
        {
            if(i< creatureSkill.Count)
            {
                sumarySkills[i].SetData(creatureSkill[i]);
            }
            else
            {
                sumarySkills[i].SetEmpty();
            }
        }

        //set stats

        statHPText.text = $"{creature.MaxHP}";
        statAtqText.text = $"{creature.Attack}";
        statDefText.text = $"{creature.Defense}";
        statAtqSpText.text = $"{creature.SpAttack}";
        statDefSpText.text = $"{creature.SpDefense}";
        statVelText.text = $"{creature.Speed}";

        if (creature.EquipedItemId != -1)
        {
            statNameEquipItem.text = creature.EquipAbility.Name;
            statDescriptionEquipItem.text = creature.EquipAbility.Description;
        }
        else
        {
            statNameEquipItem.text = "NO ITEM";
            statDescriptionEquipItem.text = "---";
        }
    }

    void ResetSelection()
    {
        //selected skill=0
        selection = 0;
    }
    void UpdateDataUI()
    {
        statusUI.SetActive(false);
        skillUI.SetActive(false);
        statsUI.SetActive(false);
        evolutionUI.SetActive(false);
        if (selectedCategory == 0)
        {
            //status
            statusUI.SetActive(true);
        }else if (selectedCategory == 1)
        {
            //skills
            skillUI.SetActive(true);
        }else if (selectedCategory == 2)
        {
            //stats
            statsUI.SetActive(true);
        }else if(selectedCategory == 3)
        {
            evolutionUI.SetActive(true);
            UpdateEvoList();
        }

    }

    void UpdateSkillSelection()
    {
        for (int i = 0; i < sumarySkills.Length; i++)
        {
            if (i ==selection)
            {
                sumarySkills[i].SetColor(GlobalSettings.i.HighlightedColor);
            }
            else
            {
                sumarySkills[i].SetColor(Color.black);
            }
        }
        if (sumarySkills[selection].Skill != null)
        {
            SkillPowerText.text =$"Potencia: {sumarySkills[selection].Skill.SkillBase.Power}" ;
            SkillDetailsText.text =sumarySkills[selection].Skill.SkillBase.Description ;
            SkillTypeText.text = $"Tipo: {sumarySkills[selection].Skill.SkillBase.Type.ToString().ToUpper()}";
            SkillAccuracyText.text =$"Precision: {sumarySkills[selection].Skill.SkillBase.Accuracy}";
        }
        else
        {
            SkillPowerText.text = "Potencia: ---";
            SkillDetailsText.text = "";
            SkillAccuracyText.text = "Precision: ---";
        }
    }

    void UpdateEvoList()
    {
        //evoList
        //Lipiar todos los objetos existentes de la lista e introducir un objeto por cada objeto del inventario del jugador
        foreach (Transform item in evoList.transform)
        {//Destruir todos los objetos vacios
            Destroy(item.gameObject);
        }
        Debug.Log("Updating item list");
        evoSlotUIList = new List<EvoSlotUI>();
        foreach (var evoSlot in this._creature.Base.Evolutions)
        {//instaciar los objetos como hijos de la lista
            var instatiatedObject = Instantiate(evoSlotUIPrefab, evoList.transform);
            instatiatedObject.SetData(this._creature, evoSlot);
            evoSlotUIList.Add(instatiatedObject);
        }

        if(evoSlotUIList.Count > 0)
        {
            selection = 0;
            UpdateEvoSelection();
        }
        else
        {
            textoCondicionEvo.text = "";
        }
    }


    void UpdateEvoSelection()
    {
        for (int i = 0; i < evoSlotUIList.Count; i++)
        {
            if (i == selection)
            {
                evoSlotUIList[i].SetSelected(true);
            }
            else
            {
                evoSlotUIList[i].SetSelected(false);
            }
        }
        EvoSlotUI selectedSlot = evoSlotUIList[selection];
        textoCondicionEvo.text = selectedSlot.evolution.GetPrettyConditionString(_creature,selectedSlot.discoveredEvolution);
    }

    void OpenEvoConfirm()
    {
        Debug.Log("SumaryUI: OpenEvoConfirm");
        if(evoSlotUIList.Count == 0)
        {
            return;
        }
        EvoSlotUI selectedEvo = evoSlotUIList[selection];
        if (evoConfirmUI!=null)
        {
            Debug.Log("SumaryUI: OpenEvoConfirm Evolución encontrada comprobando evolución");

            if (selectedEvo.evolution.CanEvolve(_creature) )
            {
                Debug.Log("SumaryUI: OpenEvoConfirm Evolución encontrada, preparando evolución");
                evoConfirmUI.ShowUI();
                evoConfirmUI.SetData(selectedEvo.evolution);
                confirmingEvo = true;
            }
        }
    }

    private void confirmedEvolution()
    {
        this.busy = true;
        confirmingEvo = false;

        //evoConfirmUI.gameObject.SetActive(false);
        Debug.Log("SumaryUI: evolution was Confirmed");

        StartCoroutine(evolutionSequence(selection));
    }

    IEnumerator evolutionSequence(int evoselected)
    {
        Debug.Log("SemaryUI: corroutineStart");
        Sprite secondSprite = _creature.Base.Evolutions[evoselected].Creature.CreatureBattleSprite;
        var sequence = DOTween.Sequence();

        //sequence.Append(creatureSprite.DOColor(000000, .5f));
        sequence.Append(creatureSprite.DOColor(Color.green, .5f));
        sequence.Append(creatureSprite.DOColor(Color.black, .5f));
        yield return sequence.WaitForCompletion();

        //Instanciar sfx

        Instantiate(evoParticleFX, evoEffectsCenter.transform);

        AudioManager.i.PlaySfx(evochangeSFX);

        creatureSprite.sprite = secondSprite;
        creatureSprite.color= Color.black;
        var sequence2 = DOTween.Sequence();
        sequence2.Append(creatureSprite.DOColor(Color.green, .5f));
        sequence2.Append(creatureSprite.DOColor(Color.white, .5f));

        yield return sequence2.WaitForCompletion();

        int selectedEvo = selection;
        _creature.EvolveCreature(selectedEvo);
        SetData(_creature);
        UpdateDataUI();

        AudioManager.i.PlaySfx(evoCompleteSFX);
        yield return MostrarMensaje("¡Tu progmon se convirtió en {evo}!".Replace("{evo}",_creature.Base.Name));

        Debug.Log("SemaryUI: corroutine End");
        this.busy = false;
        yield return null;
    }

    float GetNormalizedExp()
    {
        if(_creature.Level==CreatureBase.MaxCreatureLevel)
        {
            return 1;
        }
        int currentLevelExp = _creature.Base.GetExpForLevel(_creature.Level);
        int nextLevelExp = _creature.Base.GetExpForLevel(_creature.Level + 1);

        //calcular cantidad de exp en el nivel actual / calcular cantidad de exp adicional para este nivel
        float normalizedExp = (float)(_creature.Exp - currentLevelExp) / (nextLevelExp - currentLevelExp);

        return Mathf.Clamp01(normalizedExp);
    }

    void SetStatusText()
    {
        if (_creature.Status == null)
        {
            statusText.text = "";
        }
        else
        {
            statusText.text = _creature.Status.Id.ToString().ToUpper();
            statusText.color = statusColors[_creature.Status.Id];
        }
    }

    void DesequiparObjetoDeCriatura()
    {
        int itemDexId = _creature.UnsetEquipedAbility();
        Debug.Log($"itemDexId de objeto equipado= {itemDexId}");
        ItemBase equipedItem = CreatureDexHandler.i.GetItemBase(itemDexId);
        Inventory.GetInventory().AddItem(equipedItem);
        statNameEquipItem.text = "NO ITEM";
        statDescriptionEquipItem.text = "---";
        StartCoroutine(MostrarMensaje("Objeto desequipado."));
    }

    IEnumerator MostrarMensaje(string mensaje)
    {
        busy = true;
        yield return DialogueManager.Instance.MostrarTextoDialogoSimple(mensaje, returnToFreeRoam: false);
        busy=false;
    }
}
