using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class PartyScreen : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI messageText;
    [SerializeField]
    TextMeshProUGUI ControlsText;

    PartyMemberUI[] memberSlots;
    List<Creature> creatureParty;
    CreatureParty party;

    //Si es null no estamos en combate
    public BattleStateEnum? CalledFrom { get; set; }

    /// <summary>
    /// Party screen puede ser llamado desde varios lugares, como ActionSelection, runningTurn o aboutToUse
    /// </summary>
    
    int selection=0;

    public Creature SelectedMember=>creatureParty[selection];

    bool cambiandoOrden = false;
    int criaturaACambiar = 0;

    bool initialized = false;

    public void Init()
    {
        if (initialized)
        {
            return;
        }
        memberSlots = GetComponentsInChildren<PartyMemberUI>(true);
        //fallo a la hora de load file boot
        party = CreatureParty.GetPlayerParty();
        SetPartyData();

        //Actualiza el equipo en caso de que se actualizen los datos del equipo del jugador
        party.OnUpdated += SetPartyData;
        initialized = true;
        Debug.Log("Initialized PartyScreen");
    }

    //Se pueden pasar diferentes acciones dependiendo de donde se llame
    public void HandleUpdate(Action onSelected,Action onBack)
    {
        var previousSelection = selection;
        if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
        {
            selection++;
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
        {
            selection--;
        }
        if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
        {
            selection += 2;
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
        {
            selection -= 2;
        }
        //
        selection = Mathf.Clamp(selection, 0, creatureParty.Count - 1);
        //actualiza el UI con el selector de teclado

        if(selection!=previousSelection) UpdateMemberSelection(selection);


        if (Input.GetKeyDown(KeyCode.Z) &&creatureParty.Count>0)
        {
            if (cambiandoOrden == true)
            {
                //realizar cambio
                party.swapOrder(criaturaACambiar, selection);
                cambiandoOrden = false;
                criaturaACambiar = 0;
            }
            else
            {
                onSelected?.Invoke();
            }
        }
        else if (Input.GetKeyDown(KeyCode.X))
        {//No se puede salir de la pantalla si se realiza el cambio porque la criatura estaba debilitada
            if (cambiandoOrden == true) {
                cambiandoOrden = false;
                criaturaACambiar = 0;
                SetText("Selecciona criatura");
            }
            else
            {
                onBack?.Invoke();
            }
        }
        else if (Input.GetKeyDown(KeyCode.C) && creatureParty.Count>0 && CalledFrom == null)
        {
            cambiandoOrden = true;
            criaturaACambiar = selection;
            SetText($"Cambiando {creatureParty[selection].Base.Name} por...");
        }
        //dialogueBox.UpdateActionSelection(currentAction);
    }

    public void SetPartyData()
    {
        var creatures = party.Creatures;

        creatureParty = creatures;
        for(int i = 0; i < memberSlots.Length; i++)
        {
            if (i < creatures.Count)
            {//Introduce los datos de la criatura en el slot para ser empleados en combate
                memberSlots[i].gameObject.SetActive(true);
                memberSlots[i].Init(creatures[i]);
            }
            else
            {//Deshabilita el slot si no hay datos para ello
                memberSlots[i].gameObject.SetActive(false);
            }
            
        }
        messageText.text = "Select a creature";
        UpdateMemberSelection(selection);
    }

    public void UpdateMemberSelection(int index)
    {
        for (int i = 0; i < creatureParty.Count; i++)
        {
            if (i == index)
            {
                memberSlots[i].SetSelected(true);
            }
            else
            {
                memberSlots[i].SetSelected(false);
            }
        }
    }

    public bool TransferSelectedCreatureToBox()
    {
        //Se debe comprobar que el jugador tiene al menos una criatura sana al final
        if (creatureParty.Count - 1 == 0)
        {
            //no se puede quedar con un equipo vacío
            return false;
        }
        Creature selectedCreature = this.SelectedMember;
        if(party.getCountOfHealthyMembers()==1 && party.GetFirstHealthyCreature() == selectedCreature)
        {
            //no puedes transferir tu última criatura sana
            return false;
        }

        party.RemoveCreature(selectedCreature);
        CreatureBox.i.saveCreature(selectedCreature);
        return true;
    }

    public void SetText(string text)
    {
        messageText.text = text;
    }

    public void setControlsText(string text)
    {
        ControlsText.text = text;
    }
}
