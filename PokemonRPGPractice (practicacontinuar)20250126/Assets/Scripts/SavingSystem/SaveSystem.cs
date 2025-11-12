using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public static class SaveSystem
{
    static string saveFileName = "playerSave.agl";
    public static void SaveData(PlayerController player)
    {
        Debug.Log("A data save process has begun");
        BinaryFormatter formatter=new BinaryFormatter();
        string path = Application.persistentDataPath+"/"+ saveFileName;
        Debug.Log($"Saving file in {path}");

        FileStream stream=new FileStream(path, FileMode.Create);
        Debug.Log("FileStream opened");
        PlayerSaveData saveData=new PlayerSaveData(player);
        Debug.Log("Player data converted");

        formatter.Serialize(stream, saveData);
        Debug.Log("Data serilized");
        stream.Close();
        Debug.Log("FileStream closed and process completed");
    }

    public static PlayerSaveData LoadData()
    {
        Debug.Log("A data load process has begun");
        string path = Application.persistentDataPath + "/" + saveFileName;
        Debug.Log($"Looking for save file in {path}");
        if (File.Exists(path))
        {
            Debug.Log($"Save file was found, retrieving data");
            BinaryFormatter formatter = new BinaryFormatter();

            FileStream stream = new FileStream(path, FileMode.Open);
            Debug.Log("FileStream opened");

            Debug.Log("Deserializing player save data");
            PlayerSaveData data= formatter.Deserialize(stream) as PlayerSaveData;
            Debug.Log("Data deserialized");
            stream.Close();

            return data;
        }
        else
        {
            Debug.LogError("ERROR: Save file not found");
            return null;
        }
    }

    public static bool SaveFileExists()
    {
        return File.Exists(Application.persistentDataPath + "/" + saveFileName);
    }
}
