using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainerController : MonoBehaviour, Interactable
{
    [SerializeField] string trainerId;

    [SerializeField] string trainerName;
    [SerializeField] Sprite trainerSprite;
    [Min(0)]
    [SerializeField] float moneyReward = 0;
    [SerializeField] bool isBossBattle=false;
    // Script controlador de jugadores oponentes que pueden iniciar una batalla jugador contra jugador
    [Header("Dialogues")]
    [SerializeField] DialogueTrigger dialogue;
    [SerializeField] DialogueTrigger onDefeatDialogue;
    [SerializeField] DialogueTrigger afterBattleDialogue;

    [SerializeField] GameObject exclamation;
    [SerializeField] GameObject fov;

    [SerializeField] bool defeated = false;

    [SerializeField] TrainerController thisInstance;

    //[SerializeField] bool EMERGENCYNotInteractable = false;
    [SerializeField] bool canInteract = true;

    Character character;

    private void Awake()
    {
        character = GetComponent<Character>();
    }

    private void Start()
    {
        SetFovRotation(character.Animator.DefaultDirection);
        var defeatedThis=FlagManager.instance.GetFlagAsBool(FlagManager.TRAINER_DEFEATED_BASE+trainerId);
        defeated = defeatedThis;
    }
    private void Update()
    {
        character.HandleUpdate();
    }

    public void Interact(Transform initiator)
    {//tODO DEBUGUEAR INTERCACCIÓN DESPUÉS DE VARIAS BATALLAS
        if (canInteract == false)
        {
            return;
        }
        character.LookTowards(initiator.position);

        //Show dialogue
        if (!defeated)
        {
            //if (EMERGENCYNotInteractable == true) return;
            dialogue.TriggerElement(() =>
            {
                //Iniciar batalla de entrenador
                Debug.Log("A trainer battle has started!");
                GameController.Instance.StartTrainerBattle(TrainerId, isBossBattle);
            });
        }
        else
        {
            afterBattleDialogue.TriggerElement();
        }
        
    }

    public IEnumerator TriggerTrainerBattle(PlayerController player)
    {
        //Inicia la batalla con entrenador
        //mostrar excalmacion
        AudioManager.i.PlaySfx(AudioId.TrainerExclamation);
        exclamation.SetActive(true);
        yield return new WaitForSeconds(.5f);
        exclamation.SetActive(false);

        var diff = player.transform.position - transform.position;
        //diferencia desde jugador a entrenador
        var moveVector= diff - diff.normalized;
        //redondear a casillas
        moveVector = new Vector2(Mathf.Round(moveVector.x), Mathf.Round(moveVector.y));
        //moverse a encontrarse con el jugador
        yield return character.MoveToPosition(moveVector);

        //Show dialogue

        dialogue.TriggerElement(() =>
        {
            //Iniciar batalla de entrenador
            Debug.Log("A trainer battle has started!");
            GameController.Instance.StartTrainerBattle(TrainerId,isBossBattle);
        });
    }

    public void BattleWon()
    {
        character.GetComponent<CreatureParty>().FullCureAllCreatures();
    }

    public void BattleLost()
    {
        if (onDefeatDialogue != null)
        {
            onDefeatDialogue.TriggerElement(()=>
            {
                fov.gameObject.SetActive(false);
                defeated = true;
                FlagManager.instance.SetFlag(FlagManager.TRAINER_DEFEATED_BASE + trainerId, true.ToString());
                GameController.Instance.OnDialogueDefeatDialogue();
            });

        }
        else
        {
            fov.gameObject.SetActive(false);
            defeated = true;
            FlagManager.instance.SetFlag(FlagManager.TRAINER_DEFEATED_BASE + trainerId, true.ToString());
        }
        
    }

    public void SetFovRotation(FacingDirectionEnum dir)
    {
        float angle = 0;
        switch (dir)
        {
            case FacingDirectionEnum.RIGHT:
                fov.GetComponent<Collider2D>().offset=new Vector2(fov.GetComponent<TrainerFovTrigger>().xOffsetLateral*-1, fov.GetComponent<TrainerFovTrigger>().ylatteralOffset);
                angle = 90;
                break;
            case FacingDirectionEnum.UP:
                fov.GetComponent<Collider2D>().offset = new Vector2(0, fov.GetComponent<TrainerFovTrigger>().yOffset);
                angle = 180;
                break;
            case FacingDirectionEnum.LEFT:
                fov.GetComponent<Collider2D>().offset = new Vector2(fov.GetComponent<TrainerFovTrigger>().xOffsetLateral, fov.GetComponent<TrainerFovTrigger>().ylatteralOffset);
                angle = 270;
                break;
            default:
                fov.GetComponent<Collider2D>().offset = new Vector2(0, fov.GetComponent<TrainerFovTrigger>().yOffset);
                //mirando abajo
                break;
        }
        
        fov.transform.eulerAngles=new Vector3(0f,0f,angle);
    }

    public string TrainerName
    {
        get => trainerName;
    }
    public string TrainerId
    {
        get => trainerId;
    }
    public Sprite TrainerSprite
    {
        get => trainerSprite;
    }

    public bool Defeated
    {
        get => defeated;
    }

    public float MoneyReward => moneyReward;

    public bool hasOnEndDialogue()
    {
        if (onDefeatDialogue != null)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
