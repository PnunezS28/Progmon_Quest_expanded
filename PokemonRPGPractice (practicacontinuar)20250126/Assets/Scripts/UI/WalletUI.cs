using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WalletUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI moneyText;

    private void Start()
    {
        MoneyWallet.i.OnMoneyUpdated += SetMoneyText;
    }

    public void Show()
    {
        gameObject.SetActive(true);
        SetMoneyText();
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }

    void SetMoneyText()
    {
        moneyText.text =MoneyWallet.i.MoneyG +" G";
    }
}
