using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CountSelectorUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI countText;
    [SerializeField] TextMeshProUGUI priceText;

    bool selected;
    int currentCount;

    int maxCount;
    float pricePerUnit;
    bool cancel = false;

    public IEnumerator ShowSelector(int maxCount,float pricePerUnit,
        Action<int,bool> onCountSelected)
    {
        //inicia el selector de cantidad para vender

        this.maxCount = maxCount;
        this.pricePerUnit = pricePerUnit;

        selected = false;
        currentCount = 1;
        gameObject.SetActive(true);
        SetValues();
        //para hasta que haga una selección
        yield return new WaitUntil(() => selected == true);
        //cierra la ventana
        onCountSelected?.Invoke(currentCount,cancel);
        gameObject.SetActive(false);
    }

    private void Update()
    {
        int prevCount = currentCount;

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            currentCount++;
        }else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            currentCount--;
        }

        currentCount = Mathf.Clamp(currentCount, 1, maxCount);

        if (prevCount != currentCount)
        {
            //update values
            SetValues();
        }
        
        if (Input.GetKeyDown(KeyCode.Z))
        {
            selected = true;
            cancel = false;
        }else if (Input.GetKeyDown(KeyCode.X))
        {
            selected = true;
            cancel = true;
        }
    }

    void SetValues()
    {
        countText.text = $"x {currentCount}";
        priceText.text = pricePerUnit * currentCount+" G";
    }
}
