using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Data;


public class configAi : MonoBehaviour
{

    public TMP_InputField name;
    public speechRecognition speechR;

    private void Start()
    {        
        IDataReader reader = (IDataReader)database.executeCommand("SELECT name FROM ai_name", false, true);
        name.text = reader[0].ToString();
        speechR.addNewName(reader[0].ToString());
    }

    public void register_name()
    {
        database.executeCommand("DELETE FROM ai_name");
        database.executeCommand($"INSERT INTO ai_name (name) VALUES ('{name.text}')");
        //database.executeCommand($"UPDATE ai_name SET name = '{name.text}'");   
    }
}
