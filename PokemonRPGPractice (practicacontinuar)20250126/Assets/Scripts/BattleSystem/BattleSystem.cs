using DG.Tweening;
using Newtonsoft.Json.Bson;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class BattleSystem : MonoBehaviour
{
    [SerializeField] BattleUnit playerUnit;

    [SerializeField] BattleUnit enemyUnit;

    [SerializeField] BattleDialogueBox dialogueBox;
    [SerializeField] PartyScreen partyScreen;
    [SerializeField] InventoryUI inventoryUI;

    [SerializeField] Image playerImage;
    [SerializeField] Image rivalImage;

    [SerializeField] GameObject catcherSprite;
    [SerializeField] NewSkillSelectionUI skillSelectionUI;

    [Header("BGM and clips")]
    [SerializeField] AudioClip wildBattleMusic;
    [SerializeField] AudioClip trainerBattleMusic;
    [SerializeField] AudioClip bossBattleMusic;
    [SerializeField] AudioClip battleVictoryMusic;
    [SerializeField] float audioOffsetVictoryMusic = 0;
    [SerializeField] AudioClip levelUpClip;

    [Header("Catcher Clips")]
    [SerializeField] AudioClip catcherThrowClip;
    [SerializeField] AudioClip catcherAbsorbClip;
    [SerializeField] AudioClip catcherShakeClip;
    [SerializeField] AudioClip catcherSuccessClip;

    [Header("Screen Transition")]
    [SerializeField] BattleTransitionController transitionController;
    [SerializeField] float transitionWait = .5f;

    [Header("Skill VFX")]
    [SerializeField] List<BattleParticleFX> particleGenericFXs;


    public event Action<bool> OnBattleOver;

    BattleStateEnum battleState;

    int currentAction; //hace referencia l número de elemento en la lista de acciones
    int currentSkill;
    bool aboutToUseChoice = true;

    //datos de criaturas
    CreatureParty playerParty;
    CreatureParty oponentParty;

    Creature wildCreature;

    //variables para batalle de entrenador
    bool isTrainerBattle=false;
    //para mostraar los datos de los jugadores
    PlayerController player;
    TrainerController oponent;

    int escapeAttempts;
    SkillBase skillToLearn;


    [Header("BattleDebugSystem")]
    [SerializeField]
    BattleDebugSys battleDebugSys;
    [SerializeField]
    BattleDebugSysUI debugUI;
    [SerializeField] bool enableDebugingMenu;

    public void StartWildBattle(CreatureParty playerParty, Creature wildCreature)
    {
        this.playerParty = playerParty;
        this.wildCreature = wildCreature;
        isTrainerBattle = false;
        player = playerParty.GetComponent<PlayerController>();

        AudioManager.i.PlayMusic(wildBattleMusic);

        StartCoroutine(SetUpBattle());
    }

    public void StartTrainerBattle(CreatureParty playerParty, CreatureParty trainerParty,bool bossBattle=false)
    {
        this.playerParty = playerParty;
        this.oponentParty = trainerParty;//llega un equipo erronea la segunda vez
        isTrainerBattle = true;
        player=playerParty.GetComponent<PlayerController>();
        oponent=trainerParty.GetComponent<TrainerController>();

        if (bossBattle)
        {
            AudioManager.i.PlayMusic(bossBattleMusic);

        }
        else
        {
            AudioManager.i.PlayMusic(trainerBattleMusic);
        }

        StartCoroutine(SetUpBattle());
    }

    public void HandleUpdate()
    {
        if (battleState == BattleStateEnum.ACTION_SELECTION)
        {
            HandleActionSelection();
        }
        else if(battleState== BattleStateEnum.PLAYER_MOVE_SELECTION)
        {
            HandleSkillSelection();
        }
        else if (battleState == BattleStateEnum.PARTY_SCREEN)
        {
            HandlePartyScreenSelection();
        }
        else if (battleState == BattleStateEnum.BAG)
        {
            Action OnBack = () =>
            {
                inventoryUI.gameObject.SetActive(false);
                battleState = BattleStateEnum.ACTION_SELECTION;
            };
            Action<ItemBase> onItemUsed = (ItemBase usedItem) =>
            {
                //La corrutina controla que se use el obeto correctamente, si el objeto es un objeto de captura de criatura ejecutará una corrtuina adicional antes de continuar
                // con los turnos
                StartCoroutine(OnItemUsed(usedItem));
            };
            inventoryUI.HandleUpdate(OnBack,onItemUsed);
        }
        else if (battleState == BattleStateEnum.ABOUT_TO_USE)
        {
            HandleAboutToUse();
        }
        else if (battleState == BattleStateEnum.SKILL_FORGET)
        {
            //fforget move Handle
            Action<int> onMoveSelected = (skillIndex) =>
            {
                skillSelectionUI.gameObject.SetActive(false);
                //olvidar habilidad elegida

                if (skillIndex == CreatureBase.MaxNumOfSkills)
                {
                    //no aprender habilidad nueva
                    StartCoroutine(dialogueBox.TypeDialogue($"{playerUnit.Creature.Base.Name} no aprendió {skillToLearn.SkillName}"));
                }
                else
                {
                    //olvidar habilidad seleccionada y aparender nueva
                    var selectedSkill = playerUnit.Creature.Skills[skillIndex].SkillBase;
                    string skillForgetName = selectedSkill.SkillName;
                    string skillLearnName = skillToLearn.SkillName;

                    playerUnit.Creature.Skills[skillIndex] = new Skill(skillToLearn);
                    StartCoroutine(mensajeOlvidar(skillForgetName,skillLearnName, playerUnit.Creature.Base.Name));

                }
                skillToLearn = null;//resetear a null
                battleState = BattleStateEnum.RUNNING_TURN;
            };
            skillSelectionUI.HandleSkillSelection(onMoveSelected);
        }
        else if (battleState == BattleStateEnum.debug)
        {
            Action onBackDebug = () =>
            {
                battleState = BattleStateEnum.ACTION_SELECTION;
                debugUI.CloseDebug();
            };
            debugUI.handleUpdate(onBackDebug);
        }
    }
    IEnumerator mensajeOlvidar(string skillOlvidar,string skillLearn,string name)
    {
        yield return dialogueBox.TypeDialogue($"{name} olvidó {skillOlvidar}");
        yield return dialogueBox.TypeDialogue($"{name} aprendió {skillLearn}");
    }
    
    public void TrySetupDebug()
    {
        Debug.Log("Battle System, TrySetupDebug");
        if (battleDebugSys != null)
        {//sistema para debug de batalla, forzar cierre
            battleDebugSys.OnBattleOverDebug += () => {
                Debug.Log("BattleSystem: OnBattleOverDebug Invoke");
                StartCoroutine( BattleOver(true));
            };
            battleDebugSys.OnBattleOverLoseDebug += () => {
                Debug.Log("BattleSystem: OnBattleOverLoseDebug Invoke");
                StartCoroutine(BattleOver(false,false));
            };
            battleDebugSys.OnMasterCatcherdebug += (itemBase) => {
                if (isTrainerBattle == false)
                {
                    Debug.Log("BattleSystem: OnMasterCatcherdebug Invoke");
                    StartCoroutine(OnItemUsed(itemBase));
                }
                else
                {
                    Debug.Log("Acción debug Master catcher prohibida");
                    ActionSelection();
                }
            };
            battleDebugSys.OnBattleOverLoseDebug += () => { BattleOver(false,false); };
        }
    }


    //Inicializa las unidades de combate y sus HUDs
    public IEnumerator SetUpBattle()
    {

        escapeAttempts = 0;
        playerUnit.Clear();
        enemyUnit.Clear();
        if (!isTrainerBattle)
        {
            rivalImage.gameObject.SetActive(false);
            playerUnit.gameObject.SetActive(false);

            playerImage.gameObject.SetActive(true);

            playerImage.sprite = player.TrainerSprite;

            enemyUnit.Setup(wildCreature);
            enemyUnit.BattleHud.Setdata(enemyUnit.Creature);

            yield return dialogueBox.TypeDialogue($"¡Un {enemyUnit.Creature.Base.Name} salvaje ataca!");

            playerImage.gameObject.SetActive(false);
            playerUnit.gameObject.SetActive(true);

            //wild creature battle
            playerUnit.Setup(playerParty.GetFirstHealthyCreature());
            playerUnit.BattleHud.Setdata(playerUnit.Creature);


            dialogueBox.SetSkillNames(playerUnit.Creature.Skills);//Escribe los nombres de los ataques de la criatura

            // establecer diálogo, usando string interpolation
            //yield return pausa esta corrutina para ejecutar otra y continú esta cuando la otra acaba
        }
        else
        {
            //is trainer battle

            //Show trainer sprites
            playerUnit.gameObject.SetActive(false);
            enemyUnit.gameObject.SetActive(false);

            playerImage.gameObject.SetActive(true);
            rivalImage.gameObject.SetActive(true);

            playerImage.sprite = player.TrainerSprite;
            rivalImage.sprite = oponent.TrainerSprite;

            yield return dialogueBox.TypeDialogue($"{oponent.TrainerName} te reta a luchar");

            //send oponent's first creature
            rivalImage.gameObject.SetActive(false);
            enemyUnit.gameObject.SetActive(true);

            var firstEnemyCreature = oponentParty.GetFirstHealthyCreature();
            enemyUnit.Setup(firstEnemyCreature);
            enemyUnit.BattleHud.Setdata(enemyUnit.Creature);

            yield return dialogueBox.TypeDialogue($"{oponent.TrainerName} envió a {firstEnemyCreature.Base.Name}");

            //send player's first creature
            playerImage.gameObject.SetActive(false);
            playerUnit.gameObject.SetActive(true);

            var firstPlayerCreature = playerParty.GetFirstHealthyCreature();
            playerUnit.Setup(firstPlayerCreature);
            playerUnit.BattleHud.Setdata(playerUnit.Creature);

            yield return dialogueBox.TypeDialogue($"Enviaste a {firstPlayerCreature.Base.Name}");
            dialogueBox.SetSkillNames(playerUnit.Creature.Skills);//Escribe los nombres de los ataques de la criaturas

        }

        partyScreen.Init();

        //Activación de habilidades equipadas al inicio del combate
        bool playerGoesFirst = playerUnit.Creature.Speed >= enemyUnit.Creature.Speed;
    

        //Si playerGoesFirst=true, firstUnit=player unit; sino firstunit=enemyUnit
        var firstUnit = (playerGoesFirst) ? playerUnit : enemyUnit;
        var secondUnit = (playerGoesFirst) ? enemyUnit : playerUnit;

        var secondCreature = secondUnit.Creature;

        if (firstUnit.Creature.EquipedItemId != -1)
        {
            if (firstUnit.Creature.EquipAbility.OnEnter != null)
            {
                firstUnit.Creature.EquipAbility.OnEnter(new BattleArgs(firstUnit.Creature, Oponent: secondUnit.Creature));
            }
        }

        if (secondUnit.Creature.EquipedItemId != -1)
        {
            if (secondUnit.Creature.EquipAbility.OnEnter != null)
            {
                secondUnit.Creature.EquipAbility.OnEnter(new BattleArgs(secondUnit.Creature, Oponent: firstUnit.Creature));
            }
        }
        yield return ShowStatusChanges(firstUnit.Creature);
        yield return ShowStatusChanges(secondUnit.Creature);

        ActionSelection();

    }


    void ActionSelection()
    {
        Debug.Log("Entrando player action");
        battleState = BattleStateEnum.ACTION_SELECTION;
        dialogueBox.SetDialogue("Elije una acción");

        dialogueBox.EnableActionSelector(true);
    }

    void OpenPartyScreen()
    {
        partyScreen.CalledFrom = battleState;
        battleState = BattleStateEnum.PARTY_SCREEN;
        partyScreen.gameObject.SetActive(true);
        partyScreen.SetText("Selecciona criatura para combatir");
        partyScreen.setControlsText("[Z]:Cambiar criatura\n[X]:Salir");
    }

    void PlayerMoveSelection()
    {
        Debug.Log("Entrando player move");
        battleState = BattleStateEnum.PLAYER_MOVE_SELECTION;
        dialogueBox.EnableActionSelector(false);
        dialogueBox.EnableDialogueText(false);
        dialogueBox.EnableSkillSelector(true);
    }

    void OpenBag()
    {
        battleState = BattleStateEnum.BAG;
        inventoryUI.gameObject.SetActive(true);
    }

    IEnumerator AboutToUse(Creature newCreture)
    {
        battleState = BattleStateEnum.BUSY;
        if (playerParty.getCountOfHealthyMembers() > 1)
        {
            yield return dialogueBox.TypeDialogue($"{oponent.TrainerName} va a sacar a {newCreture.Base.Name}. ¿Quieres cambiar de prog-mon?");

            battleState = BattleStateEnum.ABOUT_TO_USE;
            dialogueBox.EnableChoiceBox(true);
        }
        else
        {
            //solo tienes una criatura con la que luchar5
            yield return SendNextOponentCreature();
        }
    }

    IEnumerator ChoseSkillToForget(Creature creature,SkillBase newSkill)
    {
        battleState = BattleStateEnum.BUSY;
        yield return dialogueBox.TypeDialogue($"Elige la habilidad que quieres olvidar");

        skillSelectionUI.gameObject.SetActive(true);
        //Linq que permite devolver una lista con los datos elegidos
        skillSelectionUI.SetSkillData(creature.Skills.Select(x=>x.SkillBase).ToList(), newSkill);
        skillToLearn = newSkill;
        battleState = BattleStateEnum.SKILL_FORGET;
    }

    IEnumerator BattleOver(bool playerWon,bool additionalMessages=true)
    {
        battleState = BattleStateEnum.BATTLE_OVER;

        if (additionalMessages)
        {
            AudioManager.i.PlayMusic(battleVictoryMusic,true,false,audioOffsetVictoryMusic);

            yield return dialogueBox.TypeDialogue($"¡Ganaste el combate!");

            if (isTrainerBattle && oponent.MoneyReward>0)
            {
                //Ganar dinero
                yield return dialogueBox.TypeDialogue($"Ganaste {oponent.MoneyReward} G");
                MoneyWallet.i.Addmoney(oponent.MoneyReward);
            }
        }
        if (playerWon == false)
        {
            yield return dialogueBox.TypeDialogue($"Pierdes el combate");
        }

        transitionController.BattleLeaveFadeIn();
        yield return new WaitForSeconds(transitionWait);

        //lambda de una lista para ejecutar un método
        playerParty.Creatures.ForEach(p => p.OnBattleOver());//Resetea aumento de carcaterísticas
        //Lipiar datos para que no dé error por estar subscrito fuera de batalla estando inactivo
        playerUnit.BattleHud.ClearData();
        enemyUnit.BattleHud.ClearData();
        transitionController.BattleLeaveFadeOut();

        OnBattleOver(playerWon);
    }

    #region HandleSelection

    //Se encarga de recivir la acción que el usuario quiere tomar
    void HandleActionSelection()
    {//Input en el UI via teclado con flechas direccionales y WASD
        if(Input.GetKeyDown(KeyCode.RightArrow)|| Input.GetKeyDown(KeyCode.D))
        {
            currentAction++;
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
        {
            currentAction--;
        }
        if (Input.GetKeyDown(KeyCode.DownArrow)|| Input.GetKeyDown(KeyCode.S))
        {
            currentAction+=2;
        }
        else if(Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
        {
            currentAction -= 2;
        }
        //
        currentAction = Mathf.Clamp(currentAction, 0, 3);

        dialogueBox.UpdateActionSelection(currentAction);
        //Confirmar acción
        if (Input.GetKeyDown(KeyCode.Z))
        {
            switch (currentAction)
            {
                case 0:
                    Debug.Log("Se seleccionó la acción luchar");
                    PlayerMoveSelection();
                    break;
                case 1:
                    Debug.Log("Se seleccionó la acción Bolsa");

                    //StartCoroutine(RunTurns(BattleActionEnum.UseItem));
                    //bag

                    OpenBag();
                    break;
                case 2:
                    Debug.Log("Se seleccionó la acción Criaturas");
                    OpenPartyScreen();
                    break;
                case 3:
                    Debug.Log("Se seleccionó la acción Huir");
                    //run
                    if (!isTrainerBattle)
                    {
                        StartCoroutine(RunTurns(BattleActionEnum.Escape));
                    }
                    else
                    {
                        Debug.Log("Escaping is forbidden while in a trainer battle");
                        StartCoroutine(WriteFleeingMessageForbidden());
                    }
                    break;
            }
        }

        if (Input.GetKey(KeyCode.F) && Input.GetKeyDown(KeyCode.G) && enableDebugingMenu==true)
        {
            //iniciar modo debug
            battleState = BattleStateEnum.debug;
            debugUI.OpenDebug();
        }
    }
    
    private void HandleSkillSelection()
    {//Input en el UI via teclado con flechas direccionales y WASD
        if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
        {
            ++currentSkill;
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
        {
            --currentSkill;
        }//permite seleccionar movimientos con las cuatro teclas direccionales
        else if(Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
        {
            currentSkill += 2;
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
        {
            currentSkill -= 2;
        }

        currentSkill = Mathf.Clamp(currentSkill, 0, playerUnit.Creature.Skills.Count-1);
        dialogueBox.UpdateSkillSelection(currentSkill, playerUnit.Creature.Skills[currentSkill]);

        if (Input.GetKeyDown(KeyCode.Z))
        {
            var move = playerUnit.Creature.Skills[currentSkill];
            if (move.Uses == 0)
            {
                return;//prohbido usar movimientos sin usos disponibles
            }

            dialogueBox.EnableSkillSelector(false);
            dialogueBox.EnableDialogueText(true);
            StartCoroutine(RunTurns(BattleActionEnum.UseMove));
        }
        else if (Input.GetKeyDown(KeyCode.X))
        {//Cancela la selección de ataque para bolver a la selección de acción
            dialogueBox.EnableSkillSelector(false);
            dialogueBox.EnableDialogueText(true);
            dialogueBox.EnableActionSelector(true);
            ActionSelection();
        }
    }

    void HandlePartyScreenSelection()
    {
        Action onSelected = () =>
         {
             //Realizar cambio
             var selectedMember = partyScreen.SelectedMember;
             if (selectedMember.HP <= 0)
             {//No se puede sacar una criatura debilitada
                 partyScreen.SetText("¡Debes enviar un prog-mon para continuar!");
                 return;
             }
             if (selectedMember == playerUnit.Creature)
             {
                 partyScreen.SetText("Es prog-mon ya está luchando");
                 return;
             }

             partyScreen.gameObject.SetActive(false);
             dialogueBox.EnableActionSelector(false);

             if (partyScreen.CalledFrom == BattleStateEnum.ACTION_SELECTION)
             {//Cambiando durante turno propio
                 StartCoroutine(RunTurns(BattleActionEnum.SwitchCreature));
             }
             else
             {//Cambio porque su criatura fué derrotada
                 battleState = BattleStateEnum.BUSY;
                 bool isTrainerAboutToUse = partyScreen.CalledFrom == BattleStateEnum.ABOUT_TO_USE;

                 StartCoroutine(SwitchPlayerCreature(selectedMember, isTrainerAboutToUse));

             }
             partyScreen.CalledFrom = null;
         };
        Action onBack = () =>
        {
            if (playerUnit.Creature.HP <= 0)
            {
                //entraste porque tu criatura se debilitó
                partyScreen.SetText("¡Debes enviar un prog-mon para continuar!");
                return;
            }

            //Cancelar acción de cambiar
            partyScreen.gameObject.SetActive(false);
            if (partyScreen.CalledFrom == BattleStateEnum.ABOUT_TO_USE)
            {
                StartCoroutine(SendNextOponentCreature());
            }
            else
            {
                ActionSelection();
            }
            partyScreen.CalledFrom = null;
        };
        partyScreen.HandleUpdate(onSelected,onBack);

    }
    #endregion

    IEnumerator RunTurns(BattleActionEnum playerAction)
    {
        battleState = BattleStateEnum.RUNNING_TURN;

        if (playerAction == BattleActionEnum.UseMove)
        {
            //Ejecuta movimiento
            //guardar el movimiento seleccionado en variabkes de las creaturas en BattleUnit
            playerUnit.Creature.currentMove = playerUnit.Creature.Skills[currentSkill];
            enemyUnit.Creature.currentMove = enemyUnit.Creature.GetRandomSkill();

            int playerMovePriority = playerUnit.Creature.currentMove.SkillBase.Priority;
            int enemyMovePriority = enemyUnit.Creature.currentMove.SkillBase.Priority;

            //comprobar vorden por prioridad
            bool playerGoesFirst = true;
            if (enemyMovePriority > playerMovePriority)
            {
                playerGoesFirst = false;
            }else if(enemyMovePriority == playerMovePriority)
            {
                //Comprobar qué critura puede moverse primero por orden de velocidad descendente
                playerGoesFirst = playerUnit.Creature.Speed >= enemyUnit.Creature.Speed;
            }


            //Si playerGoesFirst=true, firstUnit=player unit; sino firstunit=enemyUnit
            var firstUnit=(playerGoesFirst)? playerUnit : enemyUnit;
            var secondUnit=(playerGoesFirst)? enemyUnit : playerUnit;

            var secondCreature = secondUnit.Creature;

            //First turn
            yield return RunMove(firstUnit, secondUnit, firstUnit.Creature.currentMove);
            yield return RunAfterTurn(firstUnit);

            //no ejecutar segundo turno si la batalla ha acabado
            if (battleState == BattleStateEnum.BATTLE_OVER) yield break;

            if (secondCreature.HP > 0)
            {//second turn
                yield return RunMove(secondUnit, firstUnit, secondUnit.Creature.currentMove);
                yield return RunAfterTurn(secondUnit);
            }
            

        }
        else
        {
            //Accion de entrenador
            if (playerAction == BattleActionEnum.SwitchCreature)
            {
                var selectedMember = partyScreen.SelectedMember;

                battleState = BattleStateEnum.BUSY;
                yield return SwitchPlayerCreature(selectedMember);

            }
            else if (playerAction == BattleActionEnum.UseItem)
            {//salta el turno del jugador cuando se usa objeto, se amenja desde 
                dialogueBox.EnableActionSelector(false);
                //yield return ThrowCatcher();
            }
            else if (playerAction == BattleActionEnum.Escape)
            {
                dialogueBox.EnableActionSelector(false);

                yield return TryToEscape();
            }


            //enemy turn

            //TODO enemy turn: implementar inteligencia en las criaturas, metodo GetSmartMove(estado batalla)

            var enemyMove= enemyUnit.Creature.currentMove = enemyUnit.Creature.GetRandomSkill();

            //First turn
            yield return RunMove(enemyUnit, playerUnit, enemyMove);
            yield return RunAfterTurn(enemyUnit);

            //no ejecutar segundo turno si la batalla ha acabado
        }

        if (battleState != BattleStateEnum.BATTLE_OVER)
        {
            ActionSelection();
        }
    }

    IEnumerator SwitchPlayerCreature(Creature newCreature,bool isTrainerAboutToUse=false)
    {
        if (playerUnit.Creature.HP > 0)
        {//Solo realiza esta región si la criatura no está debilitada
            yield return dialogueBox.TypeDialogue($"Cambiando a {playerUnit.Creature.Base.Name}");
            playerUnit.PlayFaintAnimation();//Podríamos hacer otra animación aquí
            yield return new WaitForSeconds(2f);
        }
        

        playerUnit.Setup(newCreature);
        playerUnit.BattleHud.Setdata(newCreature);

        dialogueBox.SetSkillNames(newCreature.Skills);//Escribe los nombres de los ataques de la criatura

        // establecer diálogo, usando string interpolation
        //yield return pausa esta corrutina para ejecutar otra y continú esta cuando la otra acaba
        yield return dialogueBox.TypeDialogue($"Se envió a {newCreature.Base.Name}");
        //TODO: Aquí puede dar problemas si el ponente está devilitado
        if (playerUnit.Creature.EquipedItemId != -1 && enemyUnit.Creature.HP<0)
            {
                if (playerUnit.Creature.EquipAbility.OnEnter != null)
                {
                    playerUnit.Creature.EquipAbility.OnEnter(new BattleArgs(enemyUnit.Creature, Oponent: playerUnit.Creature));
                }
            }
        if (isTrainerAboutToUse)
        {//en este punto la criatura del oponenete está devilitada, de forma que es posible que la habilidad del usuario no pueda usarse
            StartCoroutine(SendNextOponentCreature());
        }
        else
        {
            
            battleState = BattleStateEnum.RUNNING_TURN;
        }
        
    }

    void HandleAboutToUse()
    {
        if(Input.GetKeyDown(KeyCode.UpArrow)|| Input.GetKeyDown(KeyCode.DownArrow))
        {
            aboutToUseChoice = !aboutToUseChoice;
        }

        dialogueBox.UpdateChoiceSelection(aboutToUseChoice);

        if (Input.GetKeyDown(KeyCode.Z))
        {
            dialogueBox.EnableChoiceBox(false);
            if (aboutToUseChoice == true)
            {
                //va a cambiar
                OpenPartyScreen();
            }
            else
            {
                //continua
                StartCoroutine(SendNextOponentCreature());
            }
        }
        if (Input.GetKeyDown(KeyCode.X))
        {
            //cancelar cambio, continuar
            dialogueBox.EnableChoiceBox(false);
            StartCoroutine(SendNextOponentCreature());
        }
    }

    IEnumerator SendNextOponentCreature()
    {
        battleState = BattleStateEnum.BUSY;

        var nextCreature = oponentParty.GetFirstHealthyCreature();

        enemyUnit.Setup(nextCreature);
        enemyUnit.BattleHud.Setdata(nextCreature);

        yield return dialogueBox.TypeDialogue($"{oponent.TrainerName} envió a {nextCreature.Base.Name}");
        if(enemyUnit.Creature.EquipedItemId != -1)
        {
            if(enemyUnit.Creature.EquipAbility.OnEnter != null)
            {
                enemyUnit.Creature.EquipAbility.OnEnter(new BattleArgs(enemyUnit.Creature, Oponent: playerUnit.Creature));
            }
        }
        //OnEnter
        battleState = BattleStateEnum.RUNNING_TURN;
    }

    #region RunBattleSkill
    //Código encapsulado para ejecutar movimientos
    IEnumerator RunMove(BattleUnit sourceUnit,BattleUnit targetUnit,Skill move)
    {
        bool canRunMove= sourceUnit.Creature.OnBeforeSkill();
        if (canRunMove==false)
        {//Si no puede ejecutar movimiento saltará su corrutina de movimiento
            yield return ShowStatusChanges(sourceUnit.Creature);
            yield return sourceUnit.BattleHud.WaitForHPUpdate();
            yield break;
        }
        yield return ShowStatusChanges(sourceUnit.Creature);

        move.Uses--;
        yield return dialogueBox.TypeDialogue($"{sourceUnit.Creature.Base.Name} usó {move.SkillBase.SkillName}");

        //Comprobar si el movimiento acierta
        if (checkIfMoveHits(move, sourceUnit.Creature, targetUnit.Creature))
        {
            //2. aplicar daño a enemigo
            sourceUnit.PlayAttackAnimation();
            //Si no hace efecto no reproducir animación
            AudioManager.i.PlaySfx(AudioId.CreatureAttack);

            yield return new WaitForSeconds(1f);
            //TODO: buscar array objeto BattleParticleFX (ParticleFXID,GameObject) apropiado para el tipo de skill y pasarlo a HitAnimation
            /*
             paso1. comprobar si la skill tiene un efecto de daño asociado
            paso2. si lo tiene guardar en variable y pasar a PlayHitAnimation
            Sino, intentar sacar animación de lista de genéricas
             */

            ParticleFXContainer damageParticleFX=move.SkillBase.SkillParticleFXPrefab;

            if (damageParticleFX == null)
            {
                damageParticleFX= GetGenericDamageEffect(move.SkillBase);
            }

            if (move.SkillBase.Category == SkillCategory.STATUS)
            {// le ejecución de un movimiento es diferente si el movimiento es de estado
                yield return RunSkillEffects(move.SkillBase.Effects, sourceUnit, targetUnit,move.SkillBase.Target);
                AudioManager.i.PlaySfx(move.SkillBase.Sound);
                if (statusEffective)
                {
                    if (move.SkillBase.Target == SkillTargetEnum.SELF)
                    {
                        sourceUnit.PlayHitAnimation(damageParticleFX);
                    }
                    else
                    {
                        targetUnit.PlayHitAnimation(damageParticleFX);

                    }
                }
                statusEffective = true;
                //Listar cambios en estadísticas
                yield return ShowStatusChanges(sourceUnit.Creature);
                yield return ShowStatusChanges(targetUnit.Creature);

            }
            else
            {
                if (move.SkillBase.Target == SkillTargetEnum.SELF)
                {
                    sourceUnit.PlayHitAnimation(damageParticleFX);
                }
                else
                {
                    targetUnit.PlayHitAnimation(damageParticleFX);

                }

                AudioManager.i.PlaySfx(move.SkillBase.Sound);
                DamageDetails details = targetUnit.Creature.TakeDamage(move, sourceUnit.Creature);
                yield return targetUnit.BattleHud.WaitForHPUpdate();

                //informar jugador después de actualizar HUD
                yield return ShowDamageDetails(details);
            }
            
            if(move.SkillBase.Secondaries!=null && move.SkillBase.Secondaries.Count > 0)
            {//Intenta activar efectos secundarios
                foreach(var secondary in move.SkillBase.Secondaries)
                {
                    var rnd = UnityEngine.Random.Range(1, 101);
                    if (rnd <= secondary.TriggerChance)
                    {//efectos secundarios activado
                        yield return RunSkillEffects(secondary, sourceUnit, targetUnit,secondary.Target);
                    }
                }
                statusEffective = true;
                //Listar cambios en estadísticas
                yield return ShowStatusChanges(sourceUnit.Creature);
                yield return ShowStatusChanges(targetUnit.Creature);
            }
            //manejar criatura debilitada
            if (targetUnit.Creature.HP<=0)
            {
                yield return HandleCreatureFainted(targetUnit);
            }
        }
        else
        {
            yield return dialogueBox.TypeDialogue($"El ataque de {sourceUnit.Creature.Base.Name} falló");
        }
        
    }
    bool statusEffective = true;
    //Encapsulación de los efectos de skill, ya que luego también habrá inflicción de estados
    IEnumerator RunSkillEffects(SkillEffects effects, BattleUnit source,BattleUnit target,SkillTargetEnum SkillTarget)
    {
        Creature sourceCreature= source.Creature;
        Creature targetCreature = target.Creature;


        if (effects.Boosts != null)
        {//si el movimiento tiene contenido va a realizar incremento de estadísticas
            if (SkillTarget == SkillTargetEnum.SELF)
            {//Mejoras se pueden aplicar en uno mismo aun si el oponente se debilita
                statusEffective=sourceCreature.ApplyBoosts(effects.Boosts);
            }
            else if (SkillTarget == SkillTargetEnum.SINGLE_FOE && targetCreature.HP>0)
            {
                statusEffective=targetCreature.ApplyBoosts(effects.Boosts);
            }
                
        }

        if (effects.Status != ConditionID.none)
        {
            //El movimiento inflinjirá un estado si su id de condición no es none
            statusEffective = targetCreature.SetStatus(effects.Status);
            
        }

        if (effects.VolatileStatus != ConditionID.none)
        {
            //El movimiento inflinjirá un estado volatil si su id de condición no es none
            statusEffective = targetCreature.SetVolatileStatus(effects.VolatileStatus);
        }

        //Listar cambios en estadísticas
        //yield return ShowStatusChanges(sourceCreature);
        //yield return ShowStatusChanges(targetCreature);
        yield return null;
    }

    bool checkIfMoveHits(Skill move,Creature source, Creature target)
    {
        if (move.SkillBase.AlwaysHit)
        {// devuelve true si la habilidad está establicida a acertar siempre
            return true;
        }
        float moveAccuracy = move.SkillBase.Accuracy;

        int accuracy = source.StatsBoosts[CreatureStat.Accuracy];
        int evasion = target.StatsBoosts[CreatureStat.Evasion];

        var boostValues = new float[] { 1f, 4f / 3f, 5f / 3f, 2f, 7f / 3f, 8f / 3f, 3f };

        //aumento de precisión, incremento de prescisión total
        if (accuracy > 0)
        {//impulsada en positivo
            moveAccuracy *= boostValues[accuracy];
        }else if (accuracy < 0)
        {
            moveAccuracy /= boostValues[-accuracy];
        }

        //incremento de evasión, penalización de precisión totatl
        if (evasion > 0)
        {//impulsada en positivo
            moveAccuracy /= boostValues[evasion];
        }
        else if (accuracy < 0)
        {
            moveAccuracy *= boostValues[-evasion];
        }

        //Si accuracy es igual o mayor que el numero aleatorio acierta, devolviendo true
        return UnityEngine.Random.Range(1, 101) <= moveAccuracy;
    }


    //Lista los cambios de estadísticas que una criatura ha recivido durante el combate como corrutina
    IEnumerator ShowStatusChanges(Creature creature)
    {
        while (creature.StatusChanges.Count > 0)
        {
            var message = creature.StatusChanges.Dequeue();
            yield return dialogueBox.TypeDialogue(message);
        }
    }

    //Codigo encapsulado para cuando una criatura se debilita, ganar experiencia y demás
    IEnumerator HandleCreatureFainted(BattleUnit faintedUnit)
    {
        if (faintedUnit.Creature.HP <= 0)
        {
            if (faintedUnit.IsPlayerUnit)
            {
                yield return dialogueBox.TypeDialogue($"¡Tu {faintedUnit.Creature.Base.Name} se debilitó!");
            }
            else
            {
                yield return dialogueBox.TypeDialogue($"¡El {faintedUnit.Creature.Base.Name} rival se debilitó!");
            }
            
            faintedUnit.PlayFaintAnimation();
            yield return new WaitForSeconds(2f);
            //OnFainted; Activa cuando es derrotado
            if (!faintedUnit.IsPlayerUnit)
            {
                //comprueba que si la batalla se ha ganado recproduce la música de victoria,
                //si es una batalla de entrenador antes comrpueba si aún quedan criaturas del oponenete disponibles
                bool battleWon = true;
                if (isTrainerBattle)
                {
                    battleWon = oponentParty.GetFirstHealthyCreature() == null;
                }
                if (battleWon)
                {
                    AudioManager.i.PlayMusic(battleVictoryMusic);
                }
                //Ganar experiencia or el expYield de la unidad derrotada
                int expYield= faintedUnit.Creature.Base.ExpYield;
                int enemyLevel = faintedUnit.Creature.Level;

                //la criatura dá más exp si era de entrenado
                float trainerBonus = (isTrainerBattle) ? 1.5f : 1;

                //experiencia total
                int expGain = Mathf.FloorToInt((expYield * enemyLevel * trainerBonus)/7);

                Debug.Log($"Player's {playerUnit.Creature.Base.Name} had {playerUnit.Creature.Exp} exp");
                Debug.Log($"Player's {playerUnit.Creature.Base.Name} gained {expGain} exp");
                if (playerUnit.Creature.Level < CreatureBase.MaxCreatureLevel)
                {
                    //Las criaturas ganan experiencia solo si no han llegado al nivel máximo
                    playerUnit.Creature.Exp += expGain;
                    yield return dialogueBox.TypeDialogue($"Tu {playerUnit.Creature.Base.Name} ganó {expGain} exp");
                    Debug.Log($"Player's {playerUnit.Creature.Base.Name} has now {playerUnit.Creature.Exp} exp");
                }
                StartCoroutine(playerUnit.BattleHud.SetExpSmooth());

                //comprobar si ha subido de nivel las veces que haga falta

                while (playerUnit.Creature.CheckForLevelUp())
                {
                    Debug.Log($"Player's {playerUnit.Creature.Base.Name} grew to level {playerUnit.Creature.Level}");
                    //creature leveled up
                    playerUnit.BattleHud.SetLevel();
                    AudioManager.i.PlaySfx(levelUpClip);
                    yield return dialogueBox.TypeDialogue($"Tu {playerUnit.Creature.Base.Name} creció al nivel {playerUnit.Creature.Level}");

                    //attempt to learn a new skill if able, otherwise it is null
                    //TODO: saltar si ya conoce la habilidad
                    var newSkill = playerUnit.Creature.getLearnableSkillAtCurrentLevel();
                    List<SkillBase> learnedSkills=new List<SkillBase>();
                    foreach ( var skill in playerUnit.Creature.Skills)
                    {
                        learnedSkills.Add(skill.SkillBase);
                    }

                    
                    if (newSkill != null && learnedSkills.Contains(newSkill.SkillBase)==false)
                    {
                        //learn skill
                        if (playerUnit.Creature.Skills.Count < CreatureBase.MaxNumOfSkills)
                        {
                            //añadir skill
                            playerUnit.Creature.LearnSkill(newSkill.SkillBase);

                            yield return dialogueBox.TypeDialogue($"Tu {playerUnit.Creature.Base.Name} aprendió {newSkill.SkillBase.SkillName}");

                            Debug.Log($"{playerUnit.Creature.Base.Name} acquired the {newSkill.SkillBase.SkillName} skill");
                            //Reintroducir nombres de habilidad disponibles
                            dialogueBox.SetSkillNames(playerUnit.Creature.Skills);
                        }
                        else
                        {
                            yield return dialogueBox.TypeDialogue($"Tu {playerUnit.Creature.Base.Name} está intentando aprender {newSkill.SkillBase.SkillName}");
                            yield return dialogueBox.TypeDialogue($"Pero no puede tener más de {CreatureBase.MaxNumOfSkills} habilidades");
                            Debug.Log($"The player's {playerUnit.Creature.Base.Name} has reached maximum number of skills and must forget one to acquire the {newSkill.SkillBase.SkillName} skill");
                            yield return ChoseSkillToForget(playerUnit.Creature,newSkill.SkillBase);
                            //continuar cuando el jugador elija su movimiento
                            yield return new WaitUntil(() => battleState != BattleStateEnum.SKILL_FORGET);
                            dialogueBox.SetSkillNames(playerUnit.Creature.Skills);//Escribe los nombres de los ataques de la criatura
                            yield return new WaitForSeconds(2f);
                        }
                    }

                    //reupdate exp bar y hud de equipo
                    StartCoroutine(playerUnit.BattleHud.SetExpSmooth(true));

                }

                yield return new WaitForSeconds(1.6f);
            }

            CheckForBattleOver(faintedUnit);
        }
    }

    ParticleFXContainer GetGenericDamageEffect(SkillBase skill)
    {
        BattleParticleFX battleParticleFX=null;
        if (skill.Category == SkillCategory.STATUS)
        {
            if(skill.Effects.VolatileStatus!=ConditionID.none|| skill.Effects.Status != ConditionID.none)
            {
                battleParticleFX= (BattleParticleFX)particleGenericFXs.Where(x => x.id == BattleParticleFXID.APPLY_EFFECT).FirstOrDefault();

            }
            else
            {
                //bufo o debufo, basado en primer boost
                if (skill.Effects.Boosts[0].BoostLevel > 0)
                {
                    battleParticleFX = (BattleParticleFX)particleGenericFXs.Where(x => x.id == BattleParticleFXID.BUFF).FirstOrDefault();

                }
                else if(skill.Effects.Boosts[0].BoostLevel < 0)
                {
                    battleParticleFX = (BattleParticleFX)particleGenericFXs.Where(x => x.id == BattleParticleFXID.DEBUFF).FirstOrDefault();

                }

            }

            return battleParticleFX.ParticleFXPrefab;
        }

        switch (skill.Type)
        {
            case CreatureTypeEnum.NONE:
                battleParticleFX = (BattleParticleFX)particleGenericFXs.Where(x => x.id == BattleParticleFXID.NONE).FirstOrDefault();
                break;
            case CreatureTypeEnum.NEUTRAL:
                battleParticleFX = (BattleParticleFX)particleGenericFXs.Where(x => x.id == BattleParticleFXID.NEUTRAL).FirstOrDefault();
                break;
            case CreatureTypeEnum.FIRE:
                battleParticleFX = (BattleParticleFX)particleGenericFXs.Where(x => x.id == BattleParticleFXID.FIRE).FirstOrDefault();
                break;
            case CreatureTypeEnum.WATER:
                battleParticleFX = (BattleParticleFX)particleGenericFXs.Where(x => x.id == BattleParticleFXID.WATER).FirstOrDefault();
                break;
            case CreatureTypeEnum.GRASS:
                battleParticleFX = (BattleParticleFX)particleGenericFXs.Where(x => x.id == BattleParticleFXID.GRASS).FirstOrDefault();
                break;
            case CreatureTypeEnum.EARTH:
                battleParticleFX = (BattleParticleFX)particleGenericFXs.Where(x => x.id == BattleParticleFXID.EARTH).FirstOrDefault();
                break;
            case CreatureTypeEnum.ELECTRIC:
                battleParticleFX = (BattleParticleFX)particleGenericFXs.Where(x => x.id == BattleParticleFXID.ELECTRIC).FirstOrDefault();
                break;
            case CreatureTypeEnum.WIND:
                battleParticleFX = (BattleParticleFX)particleGenericFXs.Where(x => x.id == BattleParticleFXID.WIND).FirstOrDefault();
                break;
            case CreatureTypeEnum.METAL:
                battleParticleFX = (BattleParticleFX)particleGenericFXs.Where(x => x.id == BattleParticleFXID.METAL).FirstOrDefault();
                break;
            case CreatureTypeEnum.PSYCHIC:
                battleParticleFX = (BattleParticleFX)particleGenericFXs.Where(x => x.id == BattleParticleFXID.PSYCHIC).FirstOrDefault();
                break;
            case CreatureTypeEnum.GHOST:
                battleParticleFX = (BattleParticleFX)particleGenericFXs.Where(x => x.id == BattleParticleFXID.GHOST).FirstOrDefault();
                break;
            case CreatureTypeEnum.DRAGON:
                battleParticleFX = (BattleParticleFX)particleGenericFXs.Where(x => x.id == BattleParticleFXID.DRAGON).FirstOrDefault();
                break;

        }

        return battleParticleFX.ParticleFXPrefab; 
    }

    #endregion

    IEnumerator RunAfterTurn(BattleUnit sourceUnit)
    {
        //No ejecutar si la batalla acaba
        if (battleState == BattleStateEnum.BATTLE_OVER) yield break;
        //La ejecucuión del turno pausará hasta que se continue el estado running turns
        //cubre el caso de que el usuario use un movimiento que retire o debilite su criatura antes de
        //que acabe su turno
        yield return new WaitUntil(()=>battleState==BattleStateEnum.RUNNING_TURN);

        //After turn
        sourceUnit.Creature.OnAfterTurn();
        if (sourceUnit.Creature.EquipedItemId != -1)
        {
            if(sourceUnit.Creature.EquipAbility.OnTurnEnd != null)
            {
                if (sourceUnit.IsPlayerUnit)
                {
                    sourceUnit.Creature.EquipAbility.OnTurnEnd(new BattleArgs(sourceUnit.Creature, usedSkill: sourceUnit.Creature.currentMove, Oponent: enemyUnit.Creature));
                }
                else
                {
                    sourceUnit.Creature.EquipAbility.OnTurnEnd(new BattleArgs(sourceUnit.Creature, usedSkill: sourceUnit.Creature.currentMove, Oponent: playerUnit.Creature));
                }
            }
        }

        yield return ShowStatusChanges(sourceUnit.Creature);
        //Actualización en caso de que el estado produzca daño
        yield return sourceUnit.BattleHud.WaitForHPUpdate();

        //código para cubrir el caso de que la criatura se debilite por una condición de estado, ie. poison o burn
        if (sourceUnit.Creature.HP <= 0)
        {
            yield return HandleCreatureFainted(sourceUnit);
            yield return new WaitUntil(() => battleState == BattleStateEnum.RUNNING_TURN);
        }
    }

    //Código encapsulado para cuando una unidad de batalla se debilita

    void CheckForBattleOver(BattleUnit faintedUnit)
    {
        if (faintedUnit.IsPlayerUnit)
        {
            var nextPlayerCreature = playerParty.GetFirstHealthyCreature();
            if (nextPlayerCreature != null)
            {
                //El jugador puede continuar porque tiene otra criatura disponible
                //Activar pantalla de party para elegir siguiente criatura
                OpenPartyScreen();
            }
            else
            {
                //El jugador no tiene criaturas saludables
                StartCoroutine(BattleOver(false,false));
            }
        }
        else
        {
            if (!isTrainerBattle)
            {
                StartCoroutine(BattleOver(true));
            }
            else
            {
                //trainer battle
                var nextOponentCreature=oponentParty.GetFirstHealthyCreature();
                if (nextOponentCreature != null)
                {
                    //send out next creature
                    StartCoroutine(AboutToUse(nextOponentCreature));
                }
                else
                {
                    StartCoroutine(BattleOver(true));
                }
            }
        }
    }
    IEnumerator ShowDamageDetails(DamageDetails details)
    {

        if (details.TypeEffectiveness == 0)
        {
            yield return dialogueBox.TypeDialogue("No tubo efecto");
        }
        if (details.Critical > 1f)
        {//yield return con una corrutina te permite pausar esta corrutina hasta que finalice la que se llama
            yield return dialogueBox.TypeDialogue("¡Es un golpe crítico!");
        }

        if (details.TypeEffectiveness > 1.5f)
        {
            yield return dialogueBox.TypeDialogue("¡Es superefectivo!");
        }
        if (details.TypeEffectiveness == 1.5f)
        {// DRA es un tipo que la mayoría de las veces aplica y recive 1.5 veces más daño
            yield return dialogueBox.TypeDialogue("Es muy potente");
        }
        if (details.TypeEffectiveness < 1f)
        {
            yield return dialogueBox.TypeDialogue("No es muy efectivo");

        }
    }

    IEnumerator ThrowCatcher(CatcherItem item)
    {
        battleState = BattleStateEnum.BUSY;
        dialogueBox.EnableActionSelector(false);
        //no permitido si es una batalla de entrenador
        /*
        if (isTrainerBattle)
        {
            yield return dialogueBox.TypeDialogue("You cannot attempt to catch other trainer's creatures");
            state = BattleStateEnum.RUNNING_TURN;
            yield break;
        }*/

        Debug.Log("The player initiated creature catching sequence");

        yield return dialogueBox.TypeDialogue($"¡{player.TrainerName} utilizó {item.ItemName}!");

        var catcherObject = Instantiate(catcherSprite, playerUnit.transform.position-new Vector3(2,0),Quaternion.identity);

        //Animar objeto
        var catcher = catcherObject.GetComponent<SpriteRenderer>();
        //lanzar
        AudioManager.i.PlaySfx(catcherThrowClip);
        yield return catcher.transform.DOJump(enemyUnit.transform.position + new Vector3(0, 2), 2f, 1, 1f).WaitForCompletion();
        //enemigo es atrapado
        AudioManager.i.PlaySfx(catcherAbsorbClip);

        yield return enemyUnit.PlayCaptureAnimation();
        //bajar objeto
        yield return catcher.transform.DOMoveY(enemyUnit.transform.position.y - 1, .5f).WaitForCompletion();

        int shakeCount = TryToCatchCreture(enemyUnit.Creature, item);

        //loop hasta 3 veces o menos
        for (int i = 0; i < Mathf.Min(shakeCount,3); i++)
        {
            yield return new WaitForSeconds(.5f);
            AudioManager.i.PlaySfx(catcherShakeClip);

            yield return catcher.transform.DOPunchRotation(new Vector3(0,0,10f),.8f).WaitForCompletion();
        }

        if (shakeCount == 4)
        {
            //creature caught
            Debug.Log("Creature catching successful");
            AudioManager.i.PlaySfx(catcherSuccessClip);

            AudioManager.i.PlayMusic(battleVictoryMusic);
            yield return dialogueBox.TypeDialogue($"¡{enemyUnit.Creature.Base.Name} fué atrapado!");
            yield return catcher.DOFade(0, 1.5f).WaitForCompletion();

            Destroy(catcher.gameObject);
            bool willBeSavedToBox = false;
            if (playerParty.Creatures.Count == 6)
            {
                willBeSavedToBox = true;
            }
            playerParty.AddCreature(enemyUnit.Creature);

            if (willBeSavedToBox)
            {
                yield return dialogueBox.TypeDialogue($"{enemyUnit.Creature.Base.Name} se envió a las caja de almacenamiento");
            }
            else
            {
                yield return dialogueBox.TypeDialogue($"{enemyUnit.Creature.Base.Name} se añadió a tu equipo");
            }


            yield return BattleOver(true,false);
        }
        else
        {
            //creature broke out
            yield return new WaitForSeconds(1f);
            Debug.Log("Creature catching failed");
            catcher.DOFade(0, .2f);
            //break out animation
            yield return enemyUnit.PlayBreakOutAnimation();
            Destroy(catcher.gameObject);
            if(shakeCount<=2)
                yield return dialogueBox.TypeDialogue($"¡{enemyUnit.Creature.Base.Name} se escapó!");

            if (shakeCount == 2)
                yield return dialogueBox.TypeDialogue("¡Casi lo tenías!");


            battleState = BattleStateEnum.RUNNING_TURN;
        }
    }

    IEnumerator OnItemUsed(ItemBase usedItem)
    {
        battleState = BattleStateEnum.BUSY;
        inventoryUI.gameObject.SetActive(false);

        if (usedItem is CatcherItem)
        {
            if (isTrainerBattle == true)
            {
                yield return WriteCaptureMessageForbidden();
                //prohibe el uso de un dispositivo de captura y lo devuelve
                Inventory.GetInventory().AddItem(usedItem);
                yield break;
            }
            yield return ThrowCatcher((CatcherItem)usedItem);
        }

        StartCoroutine(RunTurns(BattleActionEnum.UseItem));
    }

    int TryToCatchCreture(Creature catchTarget,CatcherItem catcherItem)
    {
        float statusBonus = ConditionsDB.GetCatchStatusBonus(catchTarget.Status);
        //Devuelve cuenta de movimientos
        Debug.Log($"Catch data : creature HP= {catchTarget.HP}, cretureMaxHP= {catchTarget.MaxHP}, catch rate= {catchTarget.Base.CatchRate}, catcher item modifier= {catcherItem.CatchRateModifier}, status bonus= {statusBonus}");
        float a = (3 * catchTarget.MaxHP - 2 * catchTarget.HP) * catchTarget.Base.CatchRate *catcherItem.CatchRateModifier * statusBonus / (3 * catchTarget.MaxHP);

        if (a >= 255)
        {
            return 4;//captura exitosa
        }
        //Calcular b para determinar cuantas veces el objeto se agita.
        //No preguntar más
        float b = 1048560 / Mathf.Sqrt(Mathf.Sqrt(16711680 / a));

        int shakeCount = 0;
        while (shakeCount < 4)
        {
            if(UnityEngine.Random.Range(0, 65535) >= b)
            {
                break;
            }

            ++shakeCount;
        }

        return shakeCount;
    }
    IEnumerator TryToEscape()
    {
        Debug.Log("Player tried to escape");
        battleState = BattleStateEnum.BUSY;
        /*
        if (isTrainerBattle)
        {//No se permite huir de batallas de entrenador
            Debug.Log("Escaping is forbidden while in a trainer battle");
            yield return dialogueBox.TypeDialogue("You cannot run from trainer battles!");
            state = BattleStateEnum.RUNNING_TURN;
            yield break;
        }*/

        int playerSpeed = playerUnit.Creature.Speed;
        int enemySpeed = enemyUnit.Creature.Speed;
        Debug.Log("Attempting to escape");
        ++escapeAttempts;
        if (enemySpeed < playerSpeed)
        {
            //can escape
            Debug.Log("Escape successful due to higher speed");
            yield return dialogueBox.TypeDialogue("¡Escapaste sin problemas!");
            yield return BattleOver(true,false);
        }
        else
        {
            //intento de escapar
            float f = (playerSpeed * 128) / enemySpeed + 30 * escapeAttempts;
            f = f % 256;
            if (UnityEngine.Random.Range(0, 256) < f)
            {
                Debug.Log("Escape successful");
                //successful escape
                yield return dialogueBox.TypeDialogue("¡Escapaste sin problemas!");
                yield return BattleOver(true,false);
            }
            else
            {
                Debug.Log("Escape failed");
                yield return dialogueBox.TypeDialogue("¡No pudiste escapar!");
                //Escape failled
                battleState = BattleStateEnum.RUNNING_TURN;
            }
        }
    }


    IEnumerator WriteCaptureMessageForbidden()
    {
        battleState = BattleStateEnum.BUSY;
        dialogueBox.EnableActionSelector(false);

        yield return dialogueBox.TypeDialogue("¡No puedes atrapar prog-mon que pertenecen a otras personas!");
        ActionSelection();
    }


    IEnumerator WriteFleeingMessageForbidden()
    {
        battleState = BattleStateEnum.BUSY;
        dialogueBox.EnableActionSelector(false);

        yield return dialogueBox.TypeDialogue("¡No puedes huir de batallas contra entrenador!");
        ActionSelection();
    }

}
