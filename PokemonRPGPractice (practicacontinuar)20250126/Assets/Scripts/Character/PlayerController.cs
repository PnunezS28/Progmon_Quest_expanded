using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class PlayerController : MonoBehaviour
{
    [SerializeField] string trainerName;
    [SerializeField] Sprite trainerSprite;
    [SerializeField] float runningMutiplier=1.5f;

    //Monobehaviour para controlar un objeto Character con CharacterAnimator
    //Patrón observador
    //public event Action OnEncounteredWild;
    //public event Action<Collider2D> OnEnterTrainerView;

    //[SerializeField]
    //private bool isMoving;//La variable protege de que se duplique el input de movimiento

    private Vector2 input;
    private Character character;

    public static PlayerController instance { get; private set; }


    private void Awake()
    {
        this.character = GetComponent<Character>();

        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
        Debug.Log("PlayerController Awake!!!");

    }


    // Update is called once per frame
    public void HandleUpdate()
    {
        if (!character.IsMoving)
        {
            //El script va a reicivir el input para mover el jugador si este no se está moviendo
            //EL jugador se mueve por casillas, o sea, añadiendo un unidad al eje x o el eje y.
            input.x = Input.GetAxisRaw("Horizontal");//izquierda y derecha
            input.y = Input.GetAxisRaw("Vertical");//arriva y abajo

            //Elimina el movimiento diagonal, dando prioridad al movimiento horizontal
            if (input.x != 0) input.y = 0;

            //Cuando detecte que el input no es 0, se moverá
            if(input != Vector2.zero)
            {
                //Inicia la corrutina del personaje que controla y cuando acabe permite la invocación de check encounter
                if (Input.GetKey(KeyCode.X))
                {
                    //x sirve para correr
                    StartCoroutine(character.MoveToPosition(input, OnMoveOver,(character.moveSpeed* runningMutiplier)));
                }
                else
                {
                    StartCoroutine(character.MoveToPosition(input,OnMoveOver));
                }

            }
        }

        character.HandleUpdate();

        if (Input.GetKeyDown(KeyCode.Z) && GameController.Instance.GameState!=GameStateEnum.DIALOGUE && GameController.Instance.GameState != GameStateEnum.SHORT_DIALOGUE)
        {
            Interact();
        }
        
    }
    void Interact()
    {
        var faceingDir = new Vector3(character.Animator.MoveX, character.Animator.MoveY);
        Vector3 playerCenter;

        if (character.Animator.MoveX != 0 || character.Animator.MoveY > 0)
        {
            playerCenter = new Vector3(transform.position.x,transform.position.y-.3f);
        }
        else
        {
            playerCenter = transform.position;
        }
        
        var interactPos = playerCenter + faceingDir;

        //Debug.DrawLine(transform.position,interactPos,Color.red,0.5f);
        var colider = Physics2D.OverlapCircle(interactPos, 0.3f, GameLayers.i.InteractableLayer);

        if (colider != null)
        {
            //interactuar con algo si es posible
            AudioManager.i.PlaySfx(AudioId.Interact);
            colider.GetComponent<Interactable>()?.Interact(transform);
        }

    }

    private void OnDrawGizmosSelected()
    {
        Vector3 playerCenter = transform.position;
        //var faceingDir = new Vector3(character.Animator.MoveX, character.Animator.MoveY);
        var interactPos = playerCenter;

        Vector3 perspectiveOffset = new Vector3(0, -.3f, 0);

        Gizmos.DrawWireSphere(interactPos + Vector3.down, 0.3f);
        Gizmos.DrawWireSphere(interactPos + Vector3.up + perspectiveOffset, 0.3f);
        Gizmos.DrawWireSphere(interactPos + Vector3.right + perspectiveOffset, 0.3f);
        Gizmos.DrawWireSphere(interactPos + Vector3.left + perspectiveOffset, 0.3f);
    }
    /*
    //Corrutina para movimiento 
    IEnumerator MoveToPosition(Vector3 targetPosition)
    {
        isMoving = true;
        //Comprueba si la distancia desde la posición actual a la objetivo es mayor que un valor float muy pequeño
        while ((targetPosition - transform.position).sqrMagnitude > Mathf.Epsilon)
        {
            //Mueve el sprite desde su posición actual a la posición objetivo.
            transform.position=Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
            yield return null;//Pequeña pausa
        }
        //Cuando acaba el while fija la posición actual a la posición objetivo
        transform.position = targetPosition;
        isMoving = false;
        CheckEncounter();//Comprueba si activa una batalla tras moverse a una zona de TallGrass.
    }

    //Comprueba que la casilla a la que va a pisar es caminable
    private bool IsWalkableTile(Vector3 taregtPosition)
    {
        //taregtPosition.y -= -.3f;
        //Comprueba si ahy colisión en la siguiente casilla en la capa indicada.
        if (Physics2D.OverlapCircle(taregtPosition, .1f, solidObjectsLayer | interactablesLayer) != null)
        {
            return false;
        }
        return true;
    }
    */
    private void OnMoveOver()
    {
        Vector2 playerCenterOffsetted = new Vector2();
        playerCenterOffsetted.x = transform.position.x;
        playerCenterOffsetted.y = transform.position.y - .3f;
        //CheckEncounter();
        //CheckIfInTrainersView();
        //Check if In trigerable portal
        var colliders = Physics2D.OverlapCircleAll(playerCenterOffsetted, .485f, GameLayers.i.TriggerableLayer);

        foreach (var collider in colliders)
        {
            var triggerable= collider.GetComponent<IPlayerTrigerable>();
            if (triggerable != null)
            {
                //can trigger
                character.Animator.IsMoving = false;
                triggerable.OnPlayerTriggered(this);
                break;
            }
        }
    }
    /*
    private void CheckEncounter()
    {

        Vector2 playerCenterOffsetted = new Vector2();
        playerCenterOffsetted.x = transform.position.x;
        playerCenterOffsetted.y = transform.position.y - .3f;
        //Debug.Log("CheckEncounter");
        //Comprueba si ahy colisión en la siguiente casilla en la capa indicada.
        if (Physics2D.OverlapCircle(playerCenterOffsetted, .485f, GameLayers.i.GrassLayer) != null)
        {
            Debug.Log("CheckingEncounter");
            //Encounter
            //Si el númeroaleatorio entre 1 y 100 es menor o igual a 10 inicia batalla
            if (Random.Range(1, 101) <= 10)
            {
                Debug.Log("Encountered a wild monster");
                character.Animator.IsMoving=false;
                OnEncounteredWild();
            }
        }
        //no encounter
    }*/
    /*
    private void CheckIfInTrainersView()
    {
        Vector2 playerCenterOffsetted = new Vector2();
        playerCenterOffsetted.x = transform.position.x;
        playerCenterOffsetted.y = transform.position.y - .3f;
        //Debug.Log("CheckEncounter");
        //Comprueba si ahy colisión en la siguiente casilla en la capa indicada.

        //Guarda el colider2D asociado al entrenador posiblemente
        var collider = Physics2D.OverlapCircle(playerCenterOffsetted, .485f, GameLayers.i.FOVLayer);
        if (collider != null)
        {
            Debug.Log("Encountered a trainer's challenge");
            character.Animator.IsMoving = false;
            OnEnterTrainerView?.Invoke(collider);

        }
    }*/


    public string TrainerName
    {
        get => trainerName;
    }

    public Sprite TrainerSprite
    {
        get => trainerSprite;
    }
}
