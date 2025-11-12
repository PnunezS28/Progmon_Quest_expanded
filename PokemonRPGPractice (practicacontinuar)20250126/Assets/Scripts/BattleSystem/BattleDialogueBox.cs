using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BattleDialogueBox : MonoBehaviour
{
    //Variables para la escritura de texto del diálogo
    [SerializeField] TextMeshProUGUI dialogueText;
    public float velocidadEscritura=30;
    string currentDialogue;
    public bool isWritting = false;

    //Elementos del UI en la caja de diálogo
    [SerializeField] GameObject actionSelector;
    [SerializeField] GameObject skillSelector;
    [SerializeField] GameObject detailBox;
    [SerializeField] GameObject choiceBox;

    [SerializeField] List<TextMeshProUGUI> actionTexts;
    [SerializeField] List<TextMeshProUGUI> skillTexts;

    [Header("Skill details")]
    [SerializeField] TextMeshProUGUI usesText;
    [SerializeField] TextMeshProUGUI typeText;
    [SerializeField] TextMeshProUGUI powAccText;
    [SerializeField] TextMeshProUGUI DescriptionText;

    [Header("Change creature")]
    [SerializeField] TextMeshProUGUI changeText;
    [SerializeField] TextMeshProUGUI continueText;


    public void SetDialogue(string dialog)
    {
        dialogueText.text = dialog;
    }

    //Función para salar el diálogo
    public void skipDialogue()
    {
        StopAllCoroutines();
        dialogueText.text = currentDialogue;
    }

    

    //Corrutina para escribir dialogo caracter a caracter
    public IEnumerator TypeDialogue(string dialogue)
    {
        currentDialogue = dialogue;
        isWritting = true;
        dialogueText.text = "";
        foreach(var letter in dialogue.ToCharArray())
        {//Escribe las letras una a una, esperando el periodo indicado en velocidad de escritura
            dialogueText.text += letter;
            yield return new WaitForSeconds(1f/velocidadEscritura);
        }
        yield return new WaitForSeconds(1f); //Pequeño espacio para que el jugador lea la notificación
        isWritting = false;
    }

    public void EnableDialogueText(bool enable)
    {
        dialogueText.enabled=enable;
    }

    public void EnableActionSelector(bool enable)
    {
        actionSelector.SetActive(enable);
    }

    public void EnableSkillSelector(bool enable)
    {
        skillSelector.SetActive(enable);
        detailBox.SetActive(enable);
    }

    public void EnableChoiceBox(bool enable)
    {
        choiceBox.SetActive(enable);
    }

    //Actualiza el UI para indicar qué acción se ha seleccionado via teclado
    public void UpdateActionSelection(int selectedAction)
    {
        //recorre las acciones y pinta la acción seleccionada de azul y las otras de negro
        for (int i = 0; i < actionTexts.Count; i++)
        {
            if (i == selectedAction)
            {
                actionTexts[i].color = GlobalSettings.i.HighlightedColor;
            }
            else
            {
                actionTexts[i].color = Color.black;
            }
        }
    }

    public void SetSkillNames(List<Skill> skills)
    {
        for(int i = 0;i<skillTexts.Count;i++)
        {
            if (i < skills.Count)
            {
                skillTexts[i].text = skills[i].SkillBase.SkillName;//Listar movimiento
            }
            else
            {
                skillTexts[i].text = "---";//Listar vacio
            }
        }
    }

    public void UpdateSkillSelection(int selectedSkill,Skill skill)
    {
        //recorre los movimientos y pinta el movimiento seleccionado de azul y las otras de negro
        for (int i = 0; i < skillTexts.Count; i++)
        {
            if (i == selectedSkill)
            {
                skillTexts[i].color = GlobalSettings.i.HighlightedColor;
            }
            else
            {
                skillTexts[i].color = Color.black;
            }
        }

        usesText.text = $"Uses: {skill.Uses}/{skill.SkillBase.MaxUses}";
        typeText.text = $"TYPE: {skill.SkillBase.Type.ToString()}" ;
        powAccText.text = $"Power {skill.SkillBase.Power}\nAcc. {skill.SkillBase.Accuracy}";
        DescriptionText.text = skill.SkillBase.Description;

        if (skill.Uses == 0)
        {
            usesText.color = Color.red;
        }
        else
        {
            usesText.color = Color.black;
        }
    }

    public void UpdateChoiceSelection(bool yesSelected)
    {
        //recorre las acciones y pinta la acción seleccionada de azul y las otras de negro
        if (yesSelected)
        {
            changeText.color = GlobalSettings.i.HighlightedColor;
            continueText.color = Color.black;
        }
        else
        {
            changeText.color = Color.black;
            continueText.color = GlobalSettings.i.HighlightedColor;
        }
    }

}
