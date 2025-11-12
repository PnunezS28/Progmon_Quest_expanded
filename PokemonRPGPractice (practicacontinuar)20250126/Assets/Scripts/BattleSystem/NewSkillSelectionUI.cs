using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class NewSkillSelectionUI : MonoBehaviour
{
    [SerializeField] List<TextMeshProUGUI> skillTexts;
    int currentSelection = 0;

    [SerializeField] Color HighlightedColor;

    public void SetSkillData(List<SkillBase> currentSkills,SkillBase newSkill)
    {
        for(int i = 0; i < currentSkills.Count; i++)
        {
            skillTexts[i].text = currentSkills[i].SkillName;
        }

        skillTexts[currentSkills.Count].text = newSkill.SkillName;
    }
    public void HandleSkillSelection(Action<int> onSelected)
    {//Input en el UI via teclado con flechas direccionales y WASD
         if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
        {
            currentSelection ++;
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
        {
            currentSelection--;
        }

        currentSelection = Mathf.Clamp(currentSelection, 0, CreatureBase.MaxNumOfSkills);
        updateMoveSelection(currentSelection);
        
        if (Input.GetKeyDown(KeyCode.Z))
        {
            onSelected?.Invoke(currentSelection);
        }
    }

    public void updateMoveSelection(int selection)
    {
        for (int i = 0; i < CreatureBase.MaxNumOfSkills+1; i++)
        {
            if (i == selection)
            {
                skillTexts[i].color = HighlightedColor;
            }
            else
            {
                skillTexts[i].color = Color.black;
            }
        }
    }

}
