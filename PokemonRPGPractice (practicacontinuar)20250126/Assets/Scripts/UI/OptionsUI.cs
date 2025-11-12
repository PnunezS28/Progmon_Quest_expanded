using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OptionsUI : MonoBehaviour
{
    [SerializeField] List<GameObject> options;
    [SerializeField] TextMeshProUGUI labelVolumenBGM;
    [SerializeField] TextMeshProUGUI labelVolumenSFX;

    float previousSetBGMVolumen;
    float previousSetSFXVolumen;

    [SerializeField] float indiceDeIncremento = 0.05f;

    int seleccion = 0;

    public Action OnClose;

    void Start()
    {
        if (options != null)
        {
            previousSetBGMVolumen = options[0].GetComponentInParent<Slider>().value;
            previousSetSFXVolumen = options[1].GetComponentInParent<Slider>().value;
        }
        UpdateSliderLabels();
        UpdateSelection();
    }

    // Update is called once per frame
    public void HandleUpdate()
    {
        int prevSelection = seleccion;
        //num0= slider bgm
        //num1= slider sfx
        //num2= boton aplicar y cerrar
        //num3= boton cerrar sin aplicar
        //num4 reeestablecer a defecto
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            seleccion++;
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            seleccion--;
        }

        if (seleccion != prevSelection)
        {
            UpdateSelection();
        }

        //clampear la selección
        seleccion = Mathf.Clamp(seleccion, 0, options.Count - 1);

        if (Input.GetKeyDown(KeyCode.Z))
        {
            //pulsar botones
            if (seleccion == 2)
            {
                ToggleWindowsFullScreen();
            }
            else if (seleccion == 3)
            {
                AudioManager.i.SetBGMVolume(options[0].GetComponentInParent<Slider>().value);
                AudioManager.i.SetSFXVolume(options[1].GetComponentInParent<Slider>().value);
                CerrarOpciones();
            }
            else if (seleccion == 4)
            {
                CerrarOpciones();
            }
            else if (seleccion == 5)
            {
                options[0].GetComponentInParent<Slider>().value = GlobalSettings.i.DefaultBGMVolumen;
                options[1].GetComponentInParent<Slider>().value = GlobalSettings.i.DefaultSFXVolumen;
                UpdateSliderLabels();
            }

        }
        else if (Input.GetKeyDown(KeyCode.X))
        {
            //cerrar
            options[0].GetComponentInParent<Slider>().value = previousSetBGMVolumen;
            options[1].GetComponentInParent<Slider>().value = previousSetSFXVolumen;
            CerrarOpciones();
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow)&& (seleccion==0||seleccion==1))
        {
            options[seleccion].GetComponentInParent<Slider>().value =Mathf.Round((options[seleccion].GetComponentInParent<Slider>().value + indiceDeIncremento)*100)/100  ;
            UpdateSliderLabels();
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow) && (seleccion == 0 || seleccion == 1))
        {
            options[seleccion].GetComponentInParent<Slider>().value = Mathf.Round((options[seleccion].GetComponentInParent<Slider>().value - indiceDeIncremento) * 100) / 100;
            UpdateSliderLabels();
        }
    }

    void CerrarOpciones()
    {
        OnClose?.Invoke();
    }

    void UpdateSliderLabels()
    {
        labelVolumenBGM.text = "" + options[0].GetComponentInParent<Slider>().value;
        labelVolumenSFX.text = "" + options[1].GetComponentInParent<Slider>().value;
    }

    void UpdateSelection()
    {
        for (int i = 0; i < options.Count; i++)
        {
            if (i == seleccion)
            {
                if (i == 0 || i == 1)
                {
                    options[i].GetComponentInChildren<Image>().color = GlobalSettings.i.HighlightedColor;
                }
                else
                {
                    options[i].GetComponent<Image>().color = GlobalSettings.i.HighlightedColor;
                }
                
            }
            else
            {
                if (i == 0 || i == 1)
                {
                    options[i].GetComponentInChildren<Image>().color = Color.white;
                }
                else
                {
                    options[i].GetComponent<Image>().color = Color.white;
                }
            }
        }
    }

    void ToggleWindowsFullScreen()
    {
        if (Screen.fullScreen == true)
        {
            Debug.Log("OptionsUI: Toggle Fullscreen false");
            Screen.fullScreen = false;
        }
        else
        {
            Debug.Log("OptionsUI: Toggle Fullscreen true");
            Screen.fullScreen = true;
        }
    }
}
