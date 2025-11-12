using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ChoiceAreaUIItem : MonoBehaviour
{
    public TextMeshProUGUI textoEleccion;

    public DialogueElement element;

    public Action OnFinished;

    public void OnSelected()
    {
        element.TriggerElement(this.OnFinished);
    }

    public void SetData(string texto,DialogueElement element,Action onFinished)
    {
        this.textoEleccion.text = texto;
            this.element = element;
        this.OnFinished = onFinished;
    }

    public void SetColor(Color color)
    {
        textoEleccion.color = color;
    }
}
