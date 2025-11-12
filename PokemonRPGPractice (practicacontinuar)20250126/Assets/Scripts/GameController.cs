using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    [SerializeField] PlayerController playerController;
    [SerializeField] BattleSystem battleSystem;
    [SerializeField] DialogueManager dialogueManager;

    [Header("Debug")]
    [SerializeField] FieldDebugUI debugUI;
    [SerializeField] bool allowDebug = false;

    [Header("Player")]
    [SerializeField] Camera worldCamera;
    [SerializeField] PlayerCameraFollowScript followScript;
    [SerializeField] GameObject PlayerPrefab;

    [Header("Player UI")]
    [SerializeField] PartyScreen partyScreen;
    [SerializeField] InventoryUI inventoryUI;
    [SerializeField] GameObject menuIcon;
    [SerializeField] SumaryUI sumaryUI;
    [SerializeField] OptionsUI optionsUI;
    [SerializeField] MultiPickUpUI multiPickUpUI;
    [SerializeField] BattleTransitionController battleTransitionController;
    [SerializeField] float transitionAnimationWait = 1f;
    [SerializeField] float exitTransitionAnimationWait = .5f;

    [Header("Creature Handling")]
    [SerializeField] CreatureDexHandler creatureDexHandler;
    [SerializeField] CreatureDexUI creatureDexUI;
    [SerializeField] CreatureBoxMenuController creatureBoxMenu;

    GameObject ActivePlayer;

    MenuController menuController;
    ShopController shopController;

    static PlayerSaveData saveData;

    public static GameController Instance { get; private set; }
    GameStateEnum gameState;
    public GameStateEnum GameState => gameState;


    private void Awake()
    {//Inicialización de base de datos de condiciones de estado
        
        Debug.Log("GameController awake");
        
    }
    private void OnEnable()
    {
        Debug.Log("GameController OnEnabled");

        ConditionsDB.Init();
        creatureDexHandler.Init();
        TransitPointDB.Init();
        BattlePassiveDB.Init();
        if(Instance == null)
        {
            Instance = this;
        }
        else if(Instance != this){
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);

        CreatureBox.i.Init();

        dialogueManager = FindObjectOfType<DialogueManager>();
        

        SpawnPlayer();

        playerController = FindObjectOfType<PlayerController>();
        

        menuController = GetComponent<MenuController>();
        shopController = GetComponent<ShopController>();
        //falla cuando se hace carga de juego desde boot
        FlagManager flagManager = FindObjectOfType<FlagManager>();
        if (flagManager.GetFlagAsBool(FlagManager.BOOT_LOAD_GAME)==false)
        {
            partyScreen.Init();
            ShopController.i.Init();
            FindObjectOfType<SceneLoader>().EndLoadingFade();
            gameState = GameStateEnum.FREE_ROAM;

        }
        else
        {
            flagManager.SetFlag(FlagManager.BOOT_LOAD_GAME, false.ToString());
        }
        //init shopcontroler
        //ShopController.i.Init();
        //battleSystem = FindObjectOfType<BattleSystem>(true);
        //creatureBoxMenu = GetComponent<CreatureBoxMenuController>();
    }
    

    public void SpawnPlayer()
    {
        Debug.Log("GameController SpawningPlayer");
        gameState = GameStateEnum.BUSY;
        //Comporbar si el jugador debe spanear en un sitio concreto por haber pasado por un punto de transición de escena
        FlagManager flagManager = FindObjectOfType<FlagManager>();
        if (flagManager.GetFlagAsBool(FlagManager.SPAWN_ON_TRANSIT_POINT))
        {
            Debug.Log("GameController Detectada inicialización en punto de transición estableciendo transform");
            //Hay que spawnear en un punto concreto tras la transición
            Debug.Log("GameController Obteniendo punto");
            int transitPointId = (int)flagManager.GetFlagAsFloat(FlagManager.DESTINATION_TRANSIT_ID);
            TransitPoint point = TransitPointDB.transitPointDB[transitPointId];

            TransitPointTag[] transitPoints = GameObject.FindObjectsOfType<TransitPointTag>();

            Transform targetTransform = null;
            FacingDirectionEnum targetFacingDirection=FacingDirectionEnum.DOWN;

            //buscar punto de transición
            foreach (TransitPointTag pointTags in transitPoints)
            {
                if (pointTags.getTransitTag().Equals(point.destinationTransformTag))
                {
                    targetTransform = pointTags.gameObject.transform;
                    targetFacingDirection = pointTags.FacingDirection;
                }
            }

            Debug.Log($"GameController Instanciando en {targetTransform.position} ");
            //si el jugador es null lo reinstancia
            if (ActivePlayer == null)
            {//Se ha perdido el jugador
                GameObject instatiatedPlayer = Instantiate(PlayerPrefab);

                //ActivePlayer = Instantiate(PlayerPrefab, pointTransform, pointQuaternion);
                Debug.Log("Setting transform");
                instatiatedPlayer.gameObject.transform.position = targetTransform.position;
                ActivePlayer = instatiatedPlayer;
            }
            //coloca al jugador en la posición apropiada
            ActivePlayer = GameObject.FindGameObjectWithTag("Player");
            //worldCamera = ActivePlayer.GetComponentInChildren<Camera>();
            ActivePlayer.transform.position = targetTransform.position;
            ActivePlayer.GetComponent<Character>().Animator.SetFacingDirection(targetFacingDirection);

            Physics.SyncTransforms();
            //refrescar memoria
            flagManager.SetFlag(FlagManager.SPAWN_ON_TRANSIT_POINT, false.ToString());
            followScript.SetPlayer(ActivePlayer);

            //worldCamera.GetComponent<PlayerCameraFollowScript>().StartPlayerCamera();
        }
        else if (flagManager.GetFlagAsBool(FlagManager.SPAWN_ON_SAVE_FILE_LOADING))
        {
            LoadPlayerData(flagManager);
            
        }else if (flagManager.GetFlagAsBool(FlagManager.BOOT_LOAD_GAME))
        {
            Debug.Log("GameController begining load game process");
            //flagManager.SetFlag(FlagManager.BOOT_LOAD_GAME, false.ToString());

            LoadPlayer();
        }
        else if (flagManager.GetFlagAsBool(FlagManager.TRANSIT_TO_HEAL_CENTER_ON_DEFEAT))
        {
            SpawnPlayerOnHealCenter(flagManager);
        }
        else
        {
            Debug.Log("GameController Spawning new player at default position");
            Transform defaultTransform = GameObject.FindGameObjectWithTag("DefaultSpawnPoint").transform;
            ActivePlayer = Instantiate(PlayerPrefab, defaultTransform.position,Quaternion.identity);
            followScript.SetPlayer(ActivePlayer);

            //worldCamera.GetComponent<PlayerCameraFollowScript>().StartPlayerCamera();
        }

        //Cuando despierte debe llamar al script de la cámara para que lo encuentre
    }

    void LoadPlayerData(FlagManager flagManager)
    {
        //Cargar jugador desde partida guardada
        //cargar jugador en posición guardada
        Vector3 savedPosition = new Vector3(saveData.playerPosition[0], saveData.playerPosition[1], saveData.playerPosition[2]);
        Debug.Log($"Instanciando en {savedPosition} ");

        if (ActivePlayer == null)
        {//Se ha perdido el jugador
            GameObject instatiatedPlayer = Instantiate(PlayerPrefab);

            //ActivePlayer = Instantiate(PlayerPrefab, pointTransform, pointQuaternion);
            Debug.Log("GameController Setting transform");
            instatiatedPlayer.gameObject.transform.position = savedPosition;
            ActivePlayer = instatiatedPlayer;
        }

        ActivePlayer = GameObject.FindGameObjectWithTag("Player");
        playerController = FindObjectOfType<PlayerController>();

        //worldCamera = ActivePlayer.GetComponentInChildren<Camera>();
        ActivePlayer.transform.position = savedPosition;
        Debug.Log("GameController Instanciado");
        //Introducir datos de criaturas
        List<Creature> criaturas = new List<Creature>();

        foreach(SerializedCreatureData d in saveData.creaturesInParty)
        {
            criaturas.Add(new Creature(d));
        }

        ActivePlayer.gameObject.GetComponent<CreatureParty>().Creatures = criaturas;
        Debug.Log("GameController Criaturas en equipo restauradas");

        MoneyWallet.i.SetMoney(saveData.walletMoney);
        //cargar objetos
        ActivePlayer.gameObject.GetComponent<Inventory>().SetInventory(saveData.recoveryItems,saveData.catcherItems,saveData.equipItems,saveData.skillTeachingItems,saveData.keyItems);
        Debug.Log("GameController Inventario restaurado");

        CreatureDexHandler.i.SetCompletionFlags(saveData.creatureDexCompletion);
        Debug.Log("GameController Progreso de CreatureDex restaurado");

        CreatureBox.i.SetBoxData(saveData.serializedBoxData);

        Physics.SyncTransforms();
        flagManager.SetFlagDictionary(saveData.playerSavedFlags);
        Debug.Log("GameController Banderas de flagDictionary restauradas");
        //refrescar memoria
        saveData = null;
        flagManager.SetFlag(FlagManager.SPAWN_ON_SAVE_FILE_LOADING, false.ToString());

        //Cuando despierte debe llamar al script de la cámara para que lo encuentre
        followScript.SetPlayer(ActivePlayer);

        partyScreen.Init();
        ShopController.i.Init();
        FindObjectOfType<SceneLoader>().EndLoadingFade();

        gameState = GameStateEnum.FREE_ROAM;

        //TransitionEnd

        //worldCamera.GetComponent<PlayerCameraFollowScript>().StartPlayerCamera();
    }

    void SpawnPlayerOnHealCenter(FlagManager flagManager)
    {
        Debug.Log("GameController Detectada inicialización en punto de destino HealCenter");
        //Hay que spawnear en un punto concreto tras la transición
        Debug.Log("GameController Obteniendo punto");
        int transitPointId = (int)flagManager.GetFlagAsFloat(FlagManager.DESTINATION_TRANSIT_ID);
        TransitPoint point = TransitPointDB.transitPointDB[transitPointId];

        TransitPointTag[] transitPoints = GameObject.FindObjectsOfType<TransitPointTag>();

        Transform targetTransform = null;
        FacingDirectionEnum targetFacingDirection = FacingDirectionEnum.DOWN;

        foreach (TransitPointTag pointTags in transitPoints)
        {
            if (pointTags.getTransitTag().Equals(point.destinationTransformTag))
            {
                targetTransform = pointTags.gameObject.transform;
                targetFacingDirection = pointTags.FacingDirection;
            }
        }

        Debug.Log($"GameController Instanciando en {targetTransform.position} ");

        if (ActivePlayer == null)
        {//Se ha perdido el jugador
            GameObject instatiatedPlayer = Instantiate(PlayerPrefab);

            //ActivePlayer = Instantiate(PlayerPrefab, pointTransform, pointQuaternion);
            Debug.Log("Setting transform");
            instatiatedPlayer.gameObject.transform.position = targetTransform.position;
            ActivePlayer = instatiatedPlayer;
        }

        ActivePlayer = GameObject.FindGameObjectWithTag("Player");
        //worldCamera = ActivePlayer.GetComponentInChildren<Camera>();
        ActivePlayer.transform.position = targetTransform.position;
        ActivePlayer.GetComponent<Character>().Animator.SetFacingDirection(targetFacingDirection);

        Physics.SyncTransforms();
        //refrescar memoria
        flagManager.SetFlag(FlagManager.TRANSIT_TO_HEAL_CENTER_ON_DEFEAT, false.ToString());
        followScript.SetPlayer(ActivePlayer);
        //worldCamera.GetComponent<PlayerCameraFollowScript>().StartPlayerCamera();

        //llega pero no conigue acabar

    }

    public IEnumerator OnDefeatMessage()
    {
        gameState = GameStateEnum.BUSY;
        CreatureParty.GetPlayerParty().FullCureAllCreatures();
        yield return DialogueManager.Instance.MostrarTextoDialogoSimple("Huiste hasta el centro de curación para que tus criaturas no fueran borradas.");

        LevelLoader.instance.TransitToHealCenter();
        gameState = GameStateEnum.FREE_ROAM;
    }

    private void Start()
    {
        Debug.Log("GameController Start");//Se ejecuta, lo siguiente no? o salta awake y OnEnabled
        //playerController.OnEncounteredWild += StartWildBattle;
        battleSystem.OnBattleOver += EndBattle;
        battleSystem.TrySetupDebug();
        optionsUI.OnClose += CloseOptionsWindow;
        menuController.OnBack += () =>
        {
            gameState = GameStateEnum.FREE_ROAM;
        };
        menuController.OnMenuSelected += OnMenuSelected;
        creatureBoxMenu.OnBack += () =>
        {
            StartCoroutine(CloseCreatureBox());
        };

        ShopController.i.OnStartShopping += () => StartShop();
        ShopController.i.OnFinishShopping += () => CloseShop();
    }
    private void Update()
    {
        if (gameState == GameStateEnum.FREE_ROAM)
        {
            playerController.HandleUpdate();

            if (Input.GetKeyDown(KeyCode.C)==true && Input.GetKey(KeyCode.Z)==false)
            {
                Debug.Log("GameController Player pressed menu button");
                //botón menú
                menuController.OpenMenu();
                gameState = GameStateEnum.MENU;
            }

            if(Input.GetKey(KeyCode.F) && Input.GetKeyDown(KeyCode.G) && allowDebug == true)
            {
                debugUI.OpenDebug();
                gameState = GameStateEnum.FieldDebug;
            }

        }
        else if (gameState == GameStateEnum.BATTLE)
        {
            battleSystem.HandleUpdate();
        }else if (gameState == GameStateEnum.DIALOGUE)
        {
            dialogueManager.HandleUpdate();
        }
        else if (gameState == GameStateEnum.MENU)
        {
            menuController.HandleUpdate();
        }else if (gameState == GameStateEnum.PARTY_SCREEN)
        {
            Action onSelected = () =>
            {
                Debug.Log("GameController Abriendo sumario de Criatura");
                OpenCreatureSumary();
            };
            Action onBack = () =>
            {
                //cerrar ventana
                partyScreen.gameObject.SetActive(false);
                gameState = GameStateEnum.MENU;
            };
            partyScreen.HandleUpdate(onSelected,onBack);
        }
        else if (gameState == GameStateEnum.BAG)
        {
            //control para bolsa
            Action onBack = () =>
            {
                //cerrar ventana
                inventoryUI.gameObject.SetActive(false);
                gameState = GameStateEnum.MENU;
            };
            inventoryUI.HandleUpdate(onBack);
        }
        else if (gameState == GameStateEnum.SUMARY)
        {
            Action onBack = () =>
            {
                CloseCreatureSummary();
            };
            sumaryUI.HandleUpdate(onBack);
        }
        else if (gameState == GameStateEnum.CREATURE_DEX)
        {
            Action onBack = () =>
            {
                CloseCreatureDexUI();
                gameState = GameStateEnum.MENU;
            };
            creatureDexUI.HandleUpdate(onBack);
        }else if (gameState == GameStateEnum.CREATURE_BOX)
        {
            
            creatureBoxMenu.HandleUpdate();
        }
        else if (gameState == GameStateEnum.OPTIONS_MENU)
        {
            optionsUI.HandleUpdate();
        }else if (gameState == GameStateEnum.SHOP)
        {
            shopController.HandleUpdate();
        }else if (gameState == GameStateEnum.MULTIPICKUP_MENU)
        {
            multiPickUpUI.HandleUpdate();
        }else if (gameState == GameStateEnum.FieldDebug)
        {
            debugUI.handleUpdate( () =>
            {
                gameState= GameStateEnum.FREE_ROAM;
                debugUI.CloseDebug();
            });
        }
    }

    public void StartDialogue()
    {
        gameState = GameStateEnum.DIALOGUE;
        menuIcon.SetActive(false);
    }

    public void StartShortDialogue()
    {
        gameState = GameStateEnum.SHORT_DIALOGUE;
        menuIcon.SetActive(false);
    }


    public void EndDialogue(bool backToFreeroam=true)
    {
        if (backToFreeroam == false)
        {
            return;
        }
        gameState = GameStateEnum.FREE_ROAM;
        menuIcon.SetActive(true);
    }

    #region Battle transition
    public void StartWildBattle()
    {
        if(FlagManager.instance.GetFlagAsBool("DEBUG_encountersOn", true)==false)
        {
            Debug.Log("DEBUG: random encounters are turned OFF !!!");
            return;
        }


        StartCoroutine(StartWildBattleTransition());
    }

    IEnumerator StartWildBattleTransition()
    {
        //TODO:iniciar transición y esperar a que acabe
        gameState = GameStateEnum.BUSY;
        AudioManager.i.PlaySfx(AudioId.TrainerExclamation);

        battleTransitionController.WildBattleFadeIn();

        yield return new WaitForSeconds(transitionAnimationWait);

        gameState = GameStateEnum.BATTLE;
        battleSystem.gameObject.SetActive(true);
        worldCamera.gameObject.SetActive(false);
        menuIcon.SetActive(false);
        battleTransitionController.WildBattleFadeOut();
        var playerParty = playerController.GetComponent<CreatureParty>();
        var wildCreature = FindObjectOfType<MapArea>().GetRandomWildCreature();

        var wildCreatureInstance = new Creature(wildCreature.Base, wildCreature.Level);

        battleSystem.StartWildBattle(playerParty, wildCreatureInstance);
        yield return null;
    }

    Action onForcedEncounterVictory;

    public void StartForcedEncounterWildBattle(Creature encounter,Action onVictory)
    {
        onForcedEncounterVictory = onVictory;
        /*
        gameState = GameStateEnum.BATTLE;
        battleSystem.gameObject.SetActive(true);
        worldCamera.gameObject.SetActive(false);
        menuIcon.SetActive(false);

        var playerParty = playerController.GetComponent<CreatureParty>();
        var wildCreature = encounter;

        var wildCreatureInstance = new Creature(wildCreature.Base, wildCreature.Level);

        battleSystem.StartWildBattle(playerParty, wildCreatureInstance);
        */
        StartCoroutine(StartForcedBattleTransition(encounter));

    }

    IEnumerator StartForcedBattleTransition(Creature encounter)
    {
        //TODO:iniciar transición y esperar a que acabe
        gameState = GameStateEnum.BUSY;
        AudioManager.i.PlaySfx(AudioId.TrainerExclamation);

        battleTransitionController.WildBattleFadeIn();

        yield return new WaitForSeconds(transitionAnimationWait);

        gameState = GameStateEnum.BATTLE;
        battleSystem.gameObject.SetActive(true);
        worldCamera.gameObject.SetActive(false);
        menuIcon.SetActive(false);
        battleTransitionController.WildBattleFadeOut();
        var playerParty = playerController.GetComponent<CreatureParty>();
        var wildCreature = encounter;

        var wildCreatureInstance = new Creature(wildCreature.Base, wildCreature.Level);

        battleSystem.StartWildBattle(playerParty, wildCreatureInstance);
        yield return null;
    }

    public void onEnterTrainerView(string id)
        {
        this.trainer = FindTrainerById(id);
        //Por seguridad
            if (trainer.Defeated == false)
            {
                gameState = GameStateEnum.CUTSCENE;
                StartCoroutine(trainer.TriggerTrainerBattle(playerController));
            }
        }

    TrainerController trainer;

    TrainerController FindTrainerById(string id)
    {
        TrainerController controller = null;
        TrainerController[] trainersInScene = FindObjectsOfType<TrainerController>();

        foreach(TrainerController search in trainersInScene)
        {
            if (search.TrainerId.Equals(id))
            {
                return search;
            }
        }

        return controller;
    }

    public void StartTrainerBattle(string trainerId, bool isBossbattle)
    {
        //Paso 1 buscar entrenador con el id correcto
        if (this.trainer == null)
        {
            TrainerController[] trainers = FindObjectsOfType<TrainerController>();
            foreach (TrainerController searchedTrainer in trainers)
            {
                if (searchedTrainer.TrainerId.Equals(trainerId))
                {
                    this.trainer = searchedTrainer;
                }
            }
        }

        StartCoroutine(StartOponentBattleTransition(this.trainer, isBossbattle));

        /*
        battleSystem.gameObject.SetActive(true);
        worldCamera.gameObject.SetActive(false);
        menuIcon.SetActive(false);
        var oponentTrainer = trainer;

        var playerParty = playerController.GetComponent<CreatureParty>();
        var oponentParty = oponentTrainer.GetComponent<CreatureParty>();//Segunda batalla llega duplicado erroneo
        gameState = GameStateEnum.BATTLE;
        battleSystem.StartTrainerBattle(playerParty, oponentParty,isBossbattle);
        */
    }

    IEnumerator StartOponentBattleTransition(TrainerController trainer, bool isBossbattle)
    {
        gameState = GameStateEnum.BUSY;

        AudioManager.i.PlaySfx(AudioId.TrainerExclamation);

        if (isBossbattle)
        {
            battleTransitionController.BossBattleFadeIn();
        }
        else
        {
            battleTransitionController.OpponentBattleFadeIn();
        }

        yield return new WaitForSeconds(transitionAnimationWait);

        if (isBossbattle)
        {
            battleTransitionController.BossBattleFadeOut();
        }
        else
        {
            battleTransitionController.OpponentBattleFadeOut();
        }

        battleSystem.gameObject.SetActive(true);
        worldCamera.gameObject.SetActive(false);
        menuIcon.SetActive(false);
        var oponentTrainer = trainer;

        var playerParty = playerController.GetComponent<CreatureParty>();
        var oponentParty = oponentTrainer.GetComponent<CreatureParty>();//Segunda batalla llega duplicado erroneo
        gameState = GameStateEnum.BATTLE;
        battleSystem.StartTrainerBattle(playerParty, oponentParty, isBossbattle);
        yield return null;
    }

    void EndBattle(bool playerWon)
    {
        gameState = GameStateEnum.BUSY;
        StartCoroutine(TransitionEndBattle(playerWon));
    }

    IEnumerator TransitionEndBattle(bool playerWon)
    {
        if (trainer != null && playerWon == true)
        {
            trainer.BattleLost();
        }

        battleSystem.gameObject.SetActive(false);
        worldCamera.gameObject.SetActive(true);
        menuIcon.SetActive(true);

        battleTransitionController.BattleLeaveFadeOut();
        yield return new WaitForSeconds(exitTransitionAnimationWait);
        //volver a reproducir la música de la escena
        AudioManager.i.PlayMusic(GameObject.FindObjectOfType<MapArea>().SceneBGM);
        battleTransitionController.BattleLeaveReset();

        if (playerWon == false)
        {
            //GetComponentInChildren<Canvas>().worldCamera = worldCamera;
            if(trainer!=null)
            {
                trainer.BattleWon();
            }
            StartCoroutine(OnDefeatMessage());

        }
        else if (trainer != null)
        {
            if (trainer.hasOnEndDialogue() == false)
            {
                gameState = GameStateEnum.FREE_ROAM;
            }
        }
        else
        {
            //Cuando acaba el encuentro forzado se queda con el texto mostrado pero puedes moverte
            gameState = GameStateEnum.FREE_ROAM;
            onForcedEncounterVictory?.Invoke();
        }
        this.trainer = null;

        yield return null;
    }
    #endregion

    public void OnTransitionStart()
    {
        gameState = GameStateEnum.BUSY;
    }

    public void OnTransitionEnd()
    {
        gameState = GameStateEnum.FREE_ROAM;
        //GetComponentInChildren<Canvas>().worldCamera = worldCamera;
    }

    void OnMenuSelected(int selection)
    {
        AudioManager.i.PlaySfx(AudioId.Interact);
        if (selection == 0)
        {
            //creatures
            partyScreen.gameObject.SetActive(true);
            gameState = GameStateEnum.PARTY_SCREEN;
            partyScreen.SetText("Selecciona criatura");
            partyScreen.setControlsText("[Z]:Seleccionar\n[X]:Salir\n[C]:Cambiar orden");
        }
        else if (selection == 1)
        {
            inventoryUI.gameObject.SetActive(true);
            gameState = GameStateEnum.BAG;
            //bag
        }
        else if(selection==2)
        {
            //creatureDex
            Debug.Log("GameController Abriendo CreatureDex");
            gameState = GameStateEnum.CREATURE_DEX;
            OpenCreatureDexUI();
        }
        else if (selection == 3)
        {
            //abrir opciones
            gameState = GameStateEnum.OPTIONS_MENU;
            optionsUI.gameObject.SetActive(true);
        }
        else if (selection == 4)
        {
            //salir del juego
            Debug.Log("GameController Saliendo del juego");
            Application.Quit();
        }else if (selection == 5)
        {
            SavePlayer();
            //TODO: añadir sonido de guardado
            menuController.CloseMenu();
            //save
        }
        else if (selection == 6)
        {
            LoadPlayer();
            //que salte un aviso
            menuController.CloseMenu();
            gameState = GameStateEnum.FREE_ROAM;
            //load
        }
    }

    void OpenCreatureSumary()
    {
        sumaryUI.gameObject.SetActive(true);
        sumaryUI.SetData(partyScreen.SelectedMember);
        gameState = GameStateEnum.SUMARY;
    }
    void CloseCreatureSummary()
    {
        sumaryUI.gameObject.SetActive(false);
        gameState = GameStateEnum.PARTY_SCREEN;
    }

    void OpenCreatureDexUI()
    {
        creatureDexUI.gameObject.SetActive(true);
        creatureDexUI.state = CreatureDexUIState.LIST;
    }

    void CloseCreatureDexUI()
    {
        creatureDexUI.gameObject.SetActive(false);
    }

    //abrir menu de operaciones para creature box, hacer un menu de seleccion y otro ui

    public IEnumerator OpenCreatureBoxMenu()
    {
        Debug.Log("GameController Opening creature box menu");
        AudioManager.i.PlaySfx(AudioId.TrainerExclamation);
        StartShortDialogue();
        yield return DialogueManager.Instance.MostrarTextoDialogoSimple("Sistema de almacenaje de criaturas activado. Elija operación.", returnToFreeRoam: false);
        creatureBoxMenu.OpenMenu();
        gameState = GameStateEnum.CREATURE_BOX;
    }

    public IEnumerator CloseCreatureBox()
    {
        Debug.Log("GameController Closing creature box menu");
        StartShortDialogue();
        yield return DialogueManager.Instance.MostrarTextoDialogoSimple("Cerrando sistema.");
        EndDialogue();
    }

    void SavePlayer()
    {
        SaveSystem.SaveData(playerController);
        AudioManager.i.PlaySfx(AudioId.SaveCompleted);
        StartCoroutine(SaveSystemMessage("Se ha guardado el juego"));
    }

    void LoadPlayer()
    {
        if (SaveSystem.SaveFileExists()==false)
        {
            Debug.Log("Aviso, no hay juego guardado");
            StartCoroutine(SaveSystemMessage("ATENCIÓN: no hay juego guardado."));
            return;
        }
        FindObjectOfType<SceneLoader>().StartLoadingFade();
        saveData = SaveSystem.LoadData();

        FlagManager.instance.SetFlag(FlagManager.SPAWN_ON_SAVE_FILE_LOADING, "true");
        //a la hora de cargar desde boot level loader es null
        StartCoroutine( LevelLoader.instance.LoadLevel(saveData.playerSceneId));

    }
    IEnumerator SaveSystemMessage(string message)
    {
        //AVISO: si guardas enfrente de un npc y pulsas rápido interactuar se causa un glich en el sistema de diálogo
        //gameState = GameStateEnum.DIALOGUE;
        StartShortDialogue();
        yield return DialogueManager.Instance.MostrarTextoDialogoSimple(message, returnToFreeRoam: true);
        //EndDialogue();
    }

    public void OnDialogueDefeatDialogue()
    {
        gameState = GameStateEnum.FREE_ROAM;
    }

    public void CloseOptionsWindow()
    {
        gameState = GameStateEnum.MENU;
        optionsUI.gameObject.SetActive(false);
    }

    public void StartShop()
    {
        gameState = GameStateEnum.SHOP;
    }

    public void CloseShop()
    {
        gameState = GameStateEnum.FREE_ROAM;
    }

    public void StartMultiPickupMenu(ItemBase[] items, Action OnClose)
    {
        multiPickUpUI.ShowUI(items, OnClose);
        gameState = GameStateEnum.MULTIPICKUP_MENU;
    }

    public void CloseMultiPickupMenu()
    {
        multiPickUpUI.CloseUI();
        gameState = GameStateEnum.FREE_ROAM;
    }
}
