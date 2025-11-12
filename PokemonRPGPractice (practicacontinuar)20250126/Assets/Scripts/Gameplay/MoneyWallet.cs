using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoneyWallet : MonoBehaviour
{
    [SerializeField] float moneyG;

    public event Action OnMoneyUpdated;

    public static MoneyWallet i { get; private set; }

    private void Awake()
    {
        i = this;
    }

    public float MoneyG => moneyG;

    public void Addmoney(float Amount)
    {
        moneyG += Amount;
        OnMoneyUpdated?.Invoke();
    }

    public void Takemoney(float Amount)
    {
        moneyG -= Amount;
        OnMoneyUpdated?.Invoke();
    }

    public void SetMoney(float Amount)
    {
        moneyG = Amount;
    }

    public bool HasMoney(float amount)
    {
        return amount <= moneyG;
    }
}
