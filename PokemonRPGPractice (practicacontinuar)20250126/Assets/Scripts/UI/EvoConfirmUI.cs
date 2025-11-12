using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class EvoConfirmUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI creatureNameText;
    [SerializeField] string confirmText;
    [SerializeField] string actiontag;
    [SerializeField] string creaturetag;

    [SerializeField] TextMeshProUGUI levelResetWarning;


    // Start is called before the first frame update
    public void SetData(CreatureEvolution evo)
    {
        string setText = confirmText;
        string accionTexto;
        switch (evo.EvoType)
        {
            case EvoTypeEnum.EVOLUTION:
                accionTexto = "evolucionar";
                break;
            case EvoTypeEnum.REGRESSION:
                accionTexto = "regresionar";
                break;
            case EvoTypeEnum.MODECHANGE:
                accionTexto = "cambiar de modo";
                break;
            default:
                accionTexto = "???";
                break;
        }

        setText=setText.Replace(actiontag, accionTexto);
        setText=setText.Replace(creaturetag,evo.Creature.Name);
        creatureNameText.SetText(setText);

        if(evo.EvoType == EvoTypeEnum.EVOLUTION || evo.EvoType==EvoTypeEnum.REGRESSION)
        {
            levelResetWarning.gameObject.SetActive(true);
        }
        else
        {
            levelResetWarning.gameObject.SetActive(false);
        }
    }

    // Update is called once per frame
    public void HandleUpdate(Action onAcceptEvo,Action onCancelEvo)
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            Debug.Log("EvoConfirmUI: aceptar");
            HideUI();
            onAcceptEvo?.Invoke();
        }
        else if (Input.GetKeyDown(KeyCode.X))
        {
            Debug.Log("EvoConfirmUI: cancelar");
            HideUI();
            onCancelEvo?.Invoke();

        }
    }

    public void ShowUI()
    {
        this.gameObject.SetActive(true);
    }

    public void HideUI()
    {
        this.gameObject.SetActive(false);
    }
}
