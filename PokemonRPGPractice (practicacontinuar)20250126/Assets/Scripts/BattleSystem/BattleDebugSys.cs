using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleDebugSys : MonoBehaviour
{
    [SerializeField] ItemBase debug_masterCatcherItemBase;

    public event Action OnBattleOverDebug;
    public event Action OnBattleOverLoseDebug;
    public event Action<ItemBase> OnMasterCatcherdebug;


    public void EndBattleDebug()
    {
        OnBattleOverDebug?.Invoke();
    }

    public void EndBattleLoseDebug()
    {
        OnBattleOverLoseDebug?.Invoke();
    }

    public void UseMasterCatcher()
    {
        OnMasterCatcherdebug?.Invoke(debug_masterCatcherItemBase);
    }
}
