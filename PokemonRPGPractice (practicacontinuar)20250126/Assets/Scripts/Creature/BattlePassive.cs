using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattlePassive
{//Clase de C# plano para efectos únicos de batalla pasivos de cada criatura
    public BattlePassiveID Id { get; set; } //Id del efecto de batalla pasivo
    public string Name { get; set; } //nombre del efecto pasivo
    public string Description { get; set; }//descripción que describe al jugador como funciona
    public bool IsOneUse { get; set; }//indica si es de un solo uso
    //acciones y funciones

    public Action<BattleArgs> OnEnter { get; set; }//Activado cuando la criatura entra en batalla
    public Func<BattleArgs,float> OnSkillUsed { get; set; }//Activado cuando el atacante usa un ataque,
                                                           //devuelve un float que aumenta o reduce el daño realizado cambiando el modificador o devolviendo el mismo
    public Func<BattleArgs,int,int> OnDamagetaken { get; set; }//activado cuando el usuario recive daño, devuelve int que aumenta o reduce el daño recivido.
    //recive battleArgs y el daño que ha recivido, devuelve el nuevo valor que debiera usarse o el mismo si no cambia
    //public Action<BattleArgs> OnOpponentHit { get; set; }//Activado cuando el usuario hace daño al oponente
    public Action<BattleArgs> AfterGetHit { get; set; }//activado después de que el usuario reciva daño
    public Func<BattleArgs,bool> OnStatusRecieved { get; set; }//Activado después de que el estado cambie, devuelve un bool para determinar si finalmente el estado se cancela o no
    public Action<BattleArgs> OnFainted { get; set; }//Activado cuando el usuario se debilita en combate
    public Action<BattleArgs> OnTurnEnd { get; set; }//Activado en el usuario al final del turno
    public Func<Creature, CreatureStat,int, int> OnStatGet { get; set; }//Activado cuando la estadística se obtiene en casos como potencia bruta que duplican el ataque físico.
    //Recive la criatura, la estadística y el valor original de la estadística. Devuelve int que indica la estadística a devolver o el mismo valor si no cambia.
    //No olvidar que se puede comprbar battlePassive?.OnStatGet!=null.
}
