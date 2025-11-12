using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SumarySkillUIHandler : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI skillNameText;
    [SerializeField] TextMeshProUGUI skillUsesText;

    Skill skill;

    public Skill Skill => skill;
    public void SetData(Skill skill)
    {
        this.skill = skill;
        skillNameText.text = skill.SkillBase.SkillName;
        skillUsesText.text = $"{skill.Uses}/{skill.SkillBase.MaxUses}";
    }

    public void SetEmpty()
    {
        skillNameText.text = "---";
        skillUsesText.text = "00/00";
    }

    public void SetColor(Color color)
    {
        skillNameText.color = color;
        skillUsesText.color = color;
    }
}
