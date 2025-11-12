using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class BattleHudHandler : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI nameText;
    [SerializeField] TextMeshProUGUI levelText;
    [SerializeField] HPBarHandler hPBar;
    [SerializeField] TextMeshProUGUI statusText;
    [SerializeField] TextMeshProUGUI healthText;
    [SerializeField] GameObject expBar;

    Creature _creature;
    Dictionary<ConditionID, Color> statusColors;
    // inserta los datos de la criatura en su HUD asociado
    public void Setdata(Creature creature)
    {
        if (_creature != null)
        {
            //quitar subscripción de criatura anterior
            _creature.OnHpChanged -= UpdateHP;
            _creature.OnStatusChanged -= SetStatusText;

        }
        _creature = creature;
        nameText.text = creature.Base.Name;
        SetLevel();
        healthText.text = $"{creature.HP}/{creature.MaxHP}";
        hPBar.SetHP((float) creature.HP / creature.MaxHP);
        SetExp();

        statusColors = GlobalSettings.i.StatusColors;

        SetStatusText();
        _creature.OnStatusChanged += SetStatusText;
        _creature.OnHpChanged += UpdateHP;
    }

    public void UpdateHP()
    {
        StartCoroutine(UpdateHPAsync());
    }

    public IEnumerator UpdateHPAsync()
    {
        yield return hPBar.SetHPSmooth((float)_creature.HP / _creature.MaxHP);
        healthText.text = $"{_creature.HP}/{_creature.MaxHP}";
    }

    public void SetLevel()
    {
        levelText.text = "Lv. " + _creature.Level;
    }

    public void SetExp()
    {
        if (expBar == null) return;
        //Solo el hud del jugador tiene EXP
        float normalizedExp= GetNormalizedExp();

        expBar.transform.localScale = new Vector3(normalizedExp, 1, 1);
    }

    public IEnumerator SetExpSmooth(bool reset=false)
    {
        if (expBar == null) yield break;
        //Solo el hud del jugador tiene EXP
        if (reset)
        {
            expBar.transform.localScale = new Vector3(0, 1, 1);
        }

        float normalizedExp = GetNormalizedExp();

        yield return expBar.transform.DOScaleX(normalizedExp, 1.5f).WaitForCompletion();
    }

    float GetNormalizedExp()
    {
        if (_creature.Level == CreatureBase.MaxCreatureLevel)
        {
            return 1;
        }
        int currentLevelExp=_creature.Base.GetExpForLevel(_creature.Level);
        int nextLevelExp=_creature.Base.GetExpForLevel(_creature.Level+1);

        //calcular cantidad de exp en el nivel actual / calcular cantidad de exp adicional para este nivel
        float normalizedExp= (float) (_creature.Exp - currentLevelExp) / (nextLevelExp - currentLevelExp);

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

    public IEnumerator WaitForHPUpdate()
    {
        yield return new WaitUntil(() => hPBar.IsUpdating == false);
    }

    public void ClearData()
    {
        if (_creature != null)
        {
            //quitar subscripción de criatura anterior
            _creature.OnHpChanged -= UpdateHP;
            _creature.OnStatusChanged -= SetStatusText;
        }
    }
}
