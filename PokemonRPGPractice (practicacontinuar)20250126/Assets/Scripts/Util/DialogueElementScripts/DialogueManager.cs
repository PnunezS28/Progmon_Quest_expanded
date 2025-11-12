using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    //Control de la velocidad del texto haciendo una pausa breve
    public float velocidadDeEscritura=.1f;
    public TextMeshProUGUI nameText;//Elemento del layout para mostrar nombre dle hablante
    //si se usa textMeshPro, si no, se puede usar Text
    public TextMeshProUGUI textoDialogo;//elemento del layout donde se muestra el texto citado
    //Controla las frases en el diálogo actual
    private Queue<string> frases;

    Action onDialogueFinished;

    public KeyCode teclaContinuar = KeyCode.Space;

    //Prompts y botones para el diálogo y su interacción en el UI
    public GameObject botonContinuarDialogo;

    public GameObject dialogueBox;

    public GameObject areaElecciones;//Area de layout para mostrar los botones de opcion
    public ChoiceAreaUIItem prefabBoton;//Prefab para construir los botones

    public AudioClip dialogueTypeWritterSound;
    //public float volume = 1f;

    public bool EnDialogo { get; private set; } = false;

    private bool enEleccion = false;
    int eleccionSeleccionada = 0;
    List<ChoiceAreaUIItem> listSelecciones;

    private bool escribiendo = false;
    private string frase;

    private DialogueElement dialogueNextTrigger;//Siguiente elemento de diálogo a activar si lo hubiera

    public static DialogueManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        /*
        else if (Instance != this)
        {
            Destroy(this.gameObject);
        }*/
        
    }

    // Start is called before the first frame update
    void Start()
    {
        frases = new Queue<string>();
    }

    public void HandleUpdate()
    {
        //cuando se pulse el botón asignado también funcionará el botón de continuar si está en diálogo
        if (Input.GetKeyDown(teclaContinuar) && !enEleccion)
        {
            ContinuarDialogo();
        }else if (enEleccion)
        {
            int prevSelection = eleccionSeleccionada;

            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                ++eleccionSeleccionada;
            }
            else if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                --eleccionSeleccionada;
            }

            eleccionSeleccionada = Mathf.Clamp(eleccionSeleccionada, 0, listSelecciones.Count - 1);

            if (eleccionSeleccionada != prevSelection)
            {
                ActualizarSeleccion();
            }

            if (Input.GetKeyDown(teclaContinuar))
            {
                listSelecciones[eleccionSeleccionada].OnSelected();
                CerrarAreaElecciones();
            }
        }
    }

    public void ContinuarDialogo()
    {
        //añadir saltar diálogo
        if (escribiendo)
        {//Solo saltar en mensajes que no sean cortos
            StopAllCoroutines();
            //excepción null
            Debug.Log("length frase" + frase.Length);
            if(textoDialogo!=null)//excepción null
                textoDialogo.maxVisibleCharacters = frase.Length;

            escribiendo = false;
        }
        else
        {//creo que esto hace conflicto con el mostrar mensaje corto. Un edje case al tener un npc enfrente con el que poder interactuar y pulsando rápido interactuar
            /*
            if (inShortDialogue==true && EnDialogo)
            {
                
                EnDialogo = false;
                dialogueBox.SetActive(false);
                GameController.Instance.EndDialogue();
            }
            else
            {*/
            StartCoroutine(MostrarSiguienteFrase());
            //}
        }
    }

    public void PromptExaminable(string name)
    {
        textoDialogo.text = name;
    }

    public void ClearBox()
    {
        textoDialogo.text = "";
    }

    //Inicia un diálogo indicado
    public void IniciarDialogo(Dialogue dialogue, Action onFinished = null)
    {
        GameController.Instance.StartDialogue();
        dialogueBox.SetActive(true);
        //Cuando se inicie el diálogo hay que bloquear la cámara para que
        //se pueda concentrar en la conversación
        enEleccion = false;
        Debug.Log("Start conversation: " + dialogue.name);
        //Establece que está en diálogo y habilita el botón de continuar
        if (botonContinuarDialogo != null)
        {
            botonContinuarDialogo.gameObject.SetActive(true);
        }

        EnDialogo = true;
        //Convinando los valores ppor defecto, acciones y modificador nulable para el método Invoke se pueden incorporar otros trozos de código
        // sin cambiar demasiado la estructura del código
        if (onFinished != null)
        {
            onDialogueFinished = onFinished;
        }
        

        if (dialogue.name.Length != 0 && nameText!=null)
        {
            nameText.text = dialogue.name;
        }
        else
        {
            if(nameText!=null)
                nameText.text = "";
        }
        //Comprobar si tiene elección
        if (dialogue.nextTrigger != null)
        {
            Debug.Log("Siguiente Trigger detectado");
            dialogueNextTrigger = dialogue.nextTrigger;
        }
        else
        {
            Debug.Log("Siguiente Trigger no detectado");
        }


        //puedes colocar el nombre aquí
        frases.Clear();//limpiar la cola e introducir el diálogo nuevoen la cola
        foreach(string sentence in dialogue.sentences)
        {
            frases.Enqueue(sentence);
        }

        //método para mostrar la siguiente frase
        StartCoroutine(MostrarSiguienteFrase());
    }

    public IEnumerator MostrarTextoDialogoSimple(string text,bool waitForInput=true,bool returnToFreeRoam=true)
    {
        //GameController.Instance.StartShortDialogue();
        EnDialogo = true;
        frase = text;
        dialogueBox.SetActive(true);
        //no pasa de aquí
        yield return TypeSentence(text);

        if (waitForInput)
        {//espera a que el usuario puslse la tecla continuar
            yield return new WaitUntil(() => Input.GetKeyDown(teclaContinuar));
        }

        EnDialogo = false;
        dialogueBox.SetActive(false);
        GameController.Instance.EndDialogue(returnToFreeRoam);
    }

    //Método para mostrar la siguiente frase de un diálogo,
    //público para que lo use el botón continuar
    IEnumerator MostrarSiguienteFrase()
    {
        yield return new WaitForEndOfFrame();
        //Seguro para que no se inicien más corrutinas de las necesarias.
        StopAllCoroutines();
        if (frases.Count == 0)
        {
            //fin de cola
            AcabarDialogo();
            yield break;
        }
        //hay más
        //saca de la cola la frase y la muestra
        frase = frases.Dequeue();
        Debug.Log(frase);
        //dialogueText.text = sentence;
        if (frase.Length > 0)
        {
            StartCoroutine(TypeSentence(frase));//inicio corrutina para animar dialogo
        }
        else
        {
            //saltar si no tiene texto
            AcabarDialogo();
        }
        
    }

    void AcabarDialogo()
    {
        Debug.Log("Dialogue Manager: Acabar dialogo");
        //por alguna razón se pierde el nextTrigger cuando usa itemGiveTrigger hacia otro elemento
        
        if (dialogueNextTrigger != null && dialogueNextTrigger is WildBattleStartTrigger==false)
        {
            Debug.Log("Entrando en siguiente trigger");
            DialogueElement next = dialogueNextTrigger;
            dialogueNextTrigger = null;//vacia el cache que guarda la elección por si la próxima conversación no tiene choiceTrigger u otro elemento
            next.TriggerElement(onDialogueFinished);
            return;//entra en las elecciones y continua en el modo conversación
        }

        if(nameText!=null)
            nameText.text = "";

        textoDialogo.text = "";
        dialogueBox.SetActive(false);
        GameController.Instance.EndDialogue();
        Debug.Log("Dialogue Manager: Fin de conversacion");

        EnDialogo = false;
        onDialogueFinished?.Invoke();
        onDialogueFinished = null;

        if (botonContinuarDialogo != null)
        {
            botonContinuarDialogo.gameObject.SetActive(false);
        }
        if(dialogueNextTrigger is WildBattleStartTrigger)
        {
            dialogueNextTrigger.TriggerElement();
            dialogueNextTrigger = null;
            EnDialogo = false;
            onDialogueFinished?.Invoke();
            onDialogueFinished = null;
        }
    }

    //corrutina que anima las letras introducidas
    IEnumerator TypeSentence(string sentence)
    {
        textoDialogo.text = sentence;
        yield return new WaitForEndOfFrame();
        escribiendo = true;

        
        textoDialogo.maxVisibleCharacters = 0;

        foreach(char letter in sentence.ToCharArray())
        {
            textoDialogo.maxVisibleCharacters++;
            if(char.IsWhiteSpace(letter)==false &&dialogueTypeWritterSound!=null)
            {
                AudioManager.i.PlaySfx(dialogueTypeWritterSound);
            }
            
            yield return new WaitForSeconds(velocidadDeEscritura);
        }
        escribiendo = false;
    }
     /*AVISO: TODO REIMPLEMENTAR PARA CONTROL DE TECLADO EXCLUSIVO*/
    public void LimpiarAreaElecciones()
    {
        foreach(Transform item in areaElecciones.transform)
        {
            Destroy(item.gameObject);//limpia las otras elecciones que no se usan
        }
        listSelecciones = null;
        eleccionSeleccionada = 0;
    }

    void CerrarAreaElecciones()
    {
        areaElecciones.gameObject.SetActive(false);
        enEleccion = false;
    }

    //prepara el layout de los botones para realizar elecciones
    public void EscribirAreaElecciones(Choice[] choices,Action onFinished)
    {
        onDialogueFinished = onFinished;//No se activa bien
        Debug.Log("Elección iniciada");
        enEleccion = true;
        LimpiarAreaElecciones();
        areaElecciones.gameObject.SetActive(true);
        listSelecciones = new List<ChoiceAreaUIItem>();

        for (int i = 0; i < choices.Length; i++)
        {
            Choice choice = choices[i];

            Debug.Log("Elección :"+choice.name);
            //Crear el nuevo botón para la elección
            ChoiceAreaUIItem newButton = Instantiate(prefabBoton, areaElecciones.transform);
            //obtener el componente texto del hijo del botón e indicar elección

            newButton.SetData(choice.name, choice.dialogueTrigger,onFinished);

            listSelecciones.Add(newButton);//añadir el onfinished

            //asignar botón para limpiar el panel de elecciones
            //Asignar el evento que activa el diálogo al botón
            //asignar botón como hijo del area de elección
            //newButton.transform.SetParent(areaElecciones.transform, false);
        }

        ActualizarSeleccion();

    }

    //update item selection
    void ActualizarSeleccion()
    {
        for(int i=0;i<listSelecciones.Count;i++)
        {
            if (eleccionSeleccionada == i)
            {
                listSelecciones[i].SetColor(GlobalSettings.i.HighlightedColor);
            }
            else
            {
                listSelecciones[i].SetColor(Color.black);
            }
        }
    }

}
