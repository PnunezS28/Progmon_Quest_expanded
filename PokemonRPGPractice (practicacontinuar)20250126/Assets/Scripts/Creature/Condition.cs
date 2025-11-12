using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Condition
{//Clase de C# plano para condiciones de estado y clima

    public ConditionID Id { get; set; }
    public string Name { get; set; }

    public string Description { get; set; }

    public string StartMessage { get; set; }

    //Acción para aplicar efectos al inicio del estado
    public Action<Creature> OnStart { get; set; }

    //Función que perite devolver datos de una acción para aplicar efectos antes de realizar un movimiento
    public Func<Creature, bool> OnBeforeSkill { get; set; }

    //Acción para aplicar efecto al final del turno
    public Action<Creature> OnAfterTurn { get; set; }
}
