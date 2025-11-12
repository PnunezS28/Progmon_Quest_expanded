using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleTransitionController : MonoBehaviour
{
    [SerializeField] Animator wildBattleTransition;
    [SerializeField] Animator opponentBattleTransition;
    [SerializeField] Animator bossBattleTransition;
    [SerializeField] Animator battleLeaveTransition;
    public void WildBattleFadeIn()
    {
        wildBattleTransition.SetTrigger("FadeIn");
    }

    public void WildBattleFadeOut()
    {
        wildBattleTransition.SetTrigger("FadeOut");
    }

    public void OpponentBattleFadeIn()
    {
        opponentBattleTransition.SetTrigger("FadeIn");
    }

    public void OpponentBattleFadeOut()
    {
        opponentBattleTransition.SetTrigger("FadeOut");
    }

    public void BossBattleFadeIn()
    {
        bossBattleTransition.SetTrigger("FadeIn");
    }

    public void BossBattleFadeOut()
    {
        bossBattleTransition.SetTrigger("FadeOut");
    }

    public void TransitionReset()
    {
        wildBattleTransition.SetTrigger("FadeReset");
        opponentBattleTransition.SetTrigger("FadeReset");
        bossBattleTransition.SetTrigger("FadeReset");
        opponentBattleTransition.SetTrigger("FadeReset");
    }

    public void BattleLeaveFadeIn()
    {
        battleLeaveTransition.SetTrigger("FadeIn");
    }

    public void BattleLeaveFadeOut()
    {
        battleLeaveTransition.SetTrigger("FadeOut");
    }

    public void BattleLeaveReset()
    {
        battleLeaveTransition.SetTrigger("FadeReset");
    }
}
