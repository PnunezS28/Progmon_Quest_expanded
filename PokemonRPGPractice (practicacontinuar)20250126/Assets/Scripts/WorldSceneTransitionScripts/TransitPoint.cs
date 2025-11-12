using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransitPoint
{
    //Clase plana C# que contiene la información de destino de una transición de una escena a otra
    //para posicionar al jugador en un punto determinado al cargar la escena
    public string TransitDestinationName { get; set; }
    public int TransitId { get; set; }
    public int DestinationSceneId { get; set; }
    public string destinationTransformTag;


}
