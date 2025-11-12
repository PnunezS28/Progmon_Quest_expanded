using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PartyMemberUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI nameText;
    [SerializeField] TextMeshProUGUI levelText;
    [SerializeField] TextMeshProUGUI healthPointText;
    [SerializeField] HPBarHandler hPBar;
    [SerializeField] GameObject equipedIcon;


    Creature _creature;
    // inserta los datos de la criatura en su HUD asociado
    public void Init(Creature creature)
    {
        _creature = creature;
        UpdateData();
        _creature.OnHpChanged += UpdateData;
        _creature.OnLevelUp += UpdateData;
    }

    void UpdateData()
    {
        nameText.text = _creature.Base.Name;
        levelText.text = "Lv. " + _creature.Level;
        healthPointText.text = $"{_creature.HP}/{_creature.MaxHP}";
        hPBar.SetHP((float)_creature.HP / _creature.MaxHP);
        if (_creature.EquipedItemId != -1)
        {
            equipedIcon.SetActive(true);
        }
        else
        {
            equipedIcon.SetActive(false);
        }
    }

    public void SetSelected(bool selected)
    {
        if (selected)
        {
            GetComponent<Image>().color = GlobalSettings.i.HighlightedColor;
        }
        else
        {
            GetComponent<Image>().color = Color.white;
        }
    }

}
