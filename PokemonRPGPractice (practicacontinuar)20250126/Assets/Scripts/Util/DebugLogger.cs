using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class DebugLogger : MonoBehaviour
{
    string filename = Application.streamingAssetsPath + "/" + "gamelog.txt";

    private void OnEnable()
    {
        Debug.Log("DebugLogger OnEnable");

    }

    private void OnDisable()
    {
        Application.logMessageReceived -= Log;
    }

    void Awake()
    {
        Application.logMessageReceived += Log;
        Debug.Log("Saving on:"+ filename);
        Debug.Log("DebugLogger awake");

    }

    public void Log(string logString,string stackTrace,LogType type)
    {
        TextWriter tw = new StreamWriter(filename, true);

        tw.WriteLine("["+System.DateTime.Now+"]"+logString+": "+stackTrace);

        tw.Close();
    }
}
