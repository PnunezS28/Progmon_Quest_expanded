using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatureBox : MonoBehaviour
{
    //creature lists de 0 a 9, como un array

    [SerializeField] List<Creature> creatureBox0;
    [SerializeField] List<Creature> creatureBox1;
    [SerializeField] List<Creature> creatureBox2;
    [SerializeField] List<Creature> creatureBox3;
    [SerializeField] List<Creature> creatureBox4;
    [SerializeField] List<Creature> creatureBox5;
    [SerializeField] List<Creature> creatureBox6;
    [SerializeField] List<Creature> creatureBox7;
    [SerializeField] List<Creature> creatureBox8;
    [SerializeField] List<Creature> creatureBox9;

    List<List<Creature>> allBoxes;

    public static CreatureBox i;

    bool initialized = false;
    //slot allLists, iniciar en awake

    //Serializa los datos de las cajas y las devuelve en una lista
    public List<List<SerializedCreatureData>> GetBoxSerializedData()
    {
        List<List<SerializedCreatureData>> boxData = new List<List<SerializedCreatureData>>();

        for(int i = 0; i < allBoxes.Count; i++)
        {
            List<SerializedCreatureData> data=new List<SerializedCreatureData>();
            foreach (Creature c in allBoxes[i])
            {
                var serializedCreature = new SerializedCreatureData(c);
                data.Add(serializedCreature);
            }
            boxData.Add(data);
        }

        return boxData;
    }

    public void SetBoxData(List<List<SerializedCreatureData>> serializedData)
    {
        //deserializa los datos de las criaturas y las reinstacia en las cajas
        for (int i = 0; i < allBoxes.Count; i++)
        {
            List<Creature> data = new List<Creature>();
            foreach (SerializedCreatureData c in serializedData[i])
            {
                var deserializedCreature = new Creature(c);
                data.Add(deserializedCreature);
            }
            allBoxes[i]=data;
        }
    }

    public static CreatureBox GetCreatureBox()
    {
        return FindObjectOfType<CreatureBox>();
    }

    public Action OnUpdated;

    public static int BOX_MAX_CREATURE_CAPACITY = 20;

    //slot list strings
    public static List<string> boxStrings {get;set;}= new List<string>()
    { "PROG_CAJA[0]","PROG_CAJA[1]","PROG_CAJA[2]","PROG_CAJA[3]","PROG_CAJA[4]",
    "PROG_CAJA[5]","PROG_CAJA[6]","PROG_CAJA[7]","PROG_CAJA[8]","PROG_CAJA[9]",};

    private void Awake()
    {
        allBoxes = new List<List<Creature>> { creatureBox0, creatureBox1, creatureBox2, creatureBox3 ,
        creatureBox4, creatureBox5, creatureBox6, creatureBox7, creatureBox8, creatureBox9};
        //guardar instacia singleton
        if (i == null)
        {
            i = this;
        }
    }

    public void Init()
    {
        if (initialized == false)
        {
            foreach(List<Creature> box in allBoxes)
            {
                foreach(Creature c in box)
                {
                    c.Init();
                }
            }
            initialized = true;
        }
        Debug.Log("Initialized CreatureBox");
    }

    //Set creature boxes para cuando se carga la partida

    public List<Creature> GetCreaturesByBox(int boxIndex)
    {
        return allBoxes[boxIndex];
    }

    public void saveCreature(Creature newCreature)
    {
        Debug.Log("Saving creature in creature box");
        bool saved = false;
        for(int i = 0; i < allBoxes.Count && saved==false; i++)
        {
            if (allBoxes[i].Count < BOX_MAX_CREATURE_CAPACITY)
            {
                allBoxes[i].Add(newCreature);
                Debug.Log("Creature saved in box i= " + i);
                saved = true;
                OnUpdated?.Invoke();
            }
        }

        //si no se pudo guardar mostrar mensaje de error
        if (saved == false)
        {
            Debug.LogError("ERROR CREATURE_BOX: boxes are full! creature was not saved");
        }
    }

    public Creature RetrieveCreature(int boxIndex, int positionIndex)
    {
        Debug.Log($"Retrieving creature from creature box {boxIndex}, position {positionIndex}");

        var slot = GetCreaturesByBox(boxIndex);
        Creature selectedCreature = slot[positionIndex];
        RemoveCreature(boxIndex, positionIndex);
        Debug.Log("Retrieving complete");
        OnUpdated?.Invoke();
        return selectedCreature;
    }

    //borra una criatura de la lista
    public void RemoveCreature(int boxIndex,int positionIndex)
    {
        Debug.Log($"Retrieving creature from creature box {boxIndex}, position {positionIndex}");

        var slot = GetCreaturesByBox(boxIndex);
        var selectedCreature = slot[positionIndex];
        allBoxes[boxIndex].Remove(selectedCreature);
        OnUpdated?.Invoke();
    }

}
