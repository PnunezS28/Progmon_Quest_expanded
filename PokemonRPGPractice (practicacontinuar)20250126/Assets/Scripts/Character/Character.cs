using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{//Monobehaviour que controla los comportamientos comunes de un personaje, IE movimiento
    CharacterAnimator animator;
    public float moveSpeed=1;

    Vector3 originalPosition;
    public bool IsMoving { get; private set; }
    [SerializeField] bool controlReset = true;

    private void Awake()
    {
        animator = GetComponent<CharacterAnimator>();
        originalPosition = transform.localPosition;

    }

    private void Start()
    {
        if (controlReset == true)
        {
            LevelLoader.instance.OnTransitionStart += ResetPosition;
            //Suscribir a level loader para preparar el reset de posición
        }
    }

    //Corrutina para movimiento 
    public IEnumerator MoveToPosition(Vector2 MoveVector, Action OnMoveOver=null,float speed=0)
    {
        //En el caso del NPC, el movimiento puede ser mayor que 1
        animator.MoveX = Mathf.Clamp(MoveVector.x,-1,1);
        animator.MoveY = Mathf.Clamp(MoveVector.y,-1,1);
        var targetPosition = transform.position;

        targetPosition.x += MoveVector.x;
        targetPosition.y += MoveVector.y;

        if (!IsPathClear(targetPosition))
            yield break;//Parar corrutina si no se puede cmaniar
        

        IsMoving = true;

        float usedMoveSpeed = moveSpeed;

        if (speed != 0)
        {
            usedMoveSpeed=speed;
        }


        //Comprueba si la distancia desde la posición actual a la objetivo es mayor que un valor float muy pequeño
        while ((targetPosition - transform.position).sqrMagnitude > Mathf.Epsilon)
        {
            //Mueve el sprite desde su posición actual a la posición objetivo.
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, usedMoveSpeed * Time.deltaTime);
            yield return null;//Pequeña pausa
        }
        //Cuando acaba el while fija la posición actual a la posición objetivo
        transform.position = targetPosition;
        IsMoving = false;

        //Esta invocación de una acción sirve para compativilizarlo con el 
        OnMoveOver?.Invoke();
    }

    public void HandleUpdate()
    {
        animator.IsMoving = IsMoving;
    }

    private bool IsPathClear(Vector3 targetPosition)
    {
        var diff = targetPosition - transform.position;
        var direction = diff.normalized;//Vector con misma dirección longitud 1

        if( Physics2D.BoxCast(transform.position+direction, new Vector2(.2f, .2f), 0,direction,diff.magnitude-1,
            GameLayers.i.SolidLayer | GameLayers.i.InteractableLayer | GameLayers.i.PlayerLayer) == true)
        {
            //Hay un colider
            return false;
        }
         //Camino libre
        return true;
    }

    //Método para mirar en una dirección determinada, usada por ejemplo al hablar para que el personaje mire hacia el jugador
    public void LookTowards(Vector3 targetPosition)
    {//Mira hacia la posición indicada
        var xdiff = Mathf.Floor(targetPosition.x) - Mathf.Floor(transform.position.x);
        //Calcular diferencia en casillas
        var ydiff = Mathf.Floor(targetPosition.y) - Mathf.Floor(transform.position.y);

        if (xdiff == 0 || ydiff == 0)
        {//Solo funciona si uno de los dos es 0, osea, no pudede hacerse en diagonal porque no hay semejante sprite
            animator.MoveX = Mathf.Clamp(xdiff, -1f, 1f);
            animator.MoveY = Mathf.Clamp(ydiff, -1f, 1f);
        }
        else Debug.LogError("Error in CharacterScript LookTowards: You cannot ask a character to look diagonaly!");
    }

    void ResetPosition()
    {
        Debug.Log("Character positionReset");
        this.gameObject.transform.localPosition = originalPosition;
        this.gameObject.SetActive(true);
        this.animator.ResetFacingPosition();
        NPCController npc = this.gameObject.GetComponent<NPCController>();
        if (npc != null)
        {
            npc.ResetState();
        }
        LevelLoader.instance.OnTransitionStart -= ResetPosition;
    }

    public CharacterAnimator Animator
    {
        get
        {
            return animator;
        }
    }
}
