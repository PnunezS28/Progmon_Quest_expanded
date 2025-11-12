using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCController : MonoBehaviour, Interactable
{
    //Clase Monobehaviour que permite al juego controlar un personaje
    [SerializeField] DialogueElement dialogueElement;
    [SerializeField] List<Vector2> movementPattern;
    [SerializeField] float timeBetweenPattern;

    [SerializeField] NPCStateEnum state;
    float idleTimer=0f;
    [SerializeField]int currentPattern = 0;

    Character character;
    NPCShop nPCShop;

    private void Awake()
    {
        character = GetComponent<Character>();
        nPCShop = GetComponent<NPCShop>();
        //ResetState();
    }

    //Método que sirve para interactuar con el npc, recive la posición del iniciador de la interacción
    public void Interact(Transform initiatior)
    {
        
        //Solo interactuable si está en idle
        if (dialogueElement != null && state == NPCStateEnum.IDLE && (GameController.Instance.GameState==GameStateEnum.FREE_ROAM))
        {Debug.Log("Interacting with NPC");
            character.LookTowards(initiatior.position);
            if (nPCShop != null)
            {
                Debug.Log("Starting shop");
                nPCShop.Trade(this);
            }
            else
            {
            state = NPCStateEnum.DIALOGUE;
                dialogueElement.TriggerElement(OnEndDialogue);
            }
            
        }
        //StartCoroutine());
    }

    private void Update()
    {
        if (state == NPCStateEnum.IDLE)
        {
            idleTimer += Time.deltaTime;
            //El controlador de NPC perimte al juego mover un personaje por un patrón cada tanto tiempo
            if (idleTimer > timeBetweenPattern)
            {
                idleTimer = 0f;
                if(movementPattern.Count>0)
                    StartCoroutine(Walk());

            }
        }
        character.HandleUpdate();
    }

    IEnumerator Walk()
    {
        state = NPCStateEnum.WALKING;
        var oldPosition = transform.position;

        yield return character.MoveToPosition(movementPattern[currentPattern]);
        
        //Controla si ha posidio seguir con su camino, sino, espera a que pueda caminar
        if (transform.position != oldPosition)
        {
            //modificador que permite hacer un loop con solo el cálculo
            currentPattern = (currentPattern + 1)%movementPattern.Count;
        }
        state = NPCStateEnum.IDLE;

    }

    public void OnEndDialogue()
    {
        idleTimer = 0;
        this.state = NPCStateEnum.IDLE;
    }

    //Menú contextual sobre el componente para mostrar el camino
    [ContextMenu("Show Path")]
    void ShowPath()
    {
        var pos = new Vector2(transform.position.x, transform.position.y);
        var index = 0;

        var colours = new List<Color>()
        {
            Color.red,
            Color.green,
            Color.blue
        };

        foreach (Vector2 path in movementPattern)
        {
            Vector2 newPosRef = movementPattern[index];

            if (newPosRef.x == 0)
                newPosRef.y *= 1f;

            else if (newPosRef.y == 0)
                newPosRef.x *= 1f;

            Debug.DrawLine(pos, pos + newPosRef, colours[index % 3], 2f);

            index += 1;
            pos += newPosRef;
        }
    }

    public void ResetState()
    {
        state = NPCStateEnum.IDLE;
        idleTimer = 0;
        currentPattern = 0;
        StopAllCoroutines();
    }

    public NPCShop NPCShop => nPCShop;
}
