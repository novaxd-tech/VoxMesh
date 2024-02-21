using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Data;

public class configVT : MonoBehaviour
{

    public TextMeshProUGUI text_top;
    public TextMeshProUGUI text_bottom;

    public voiceTypeSwitch voiceType;
    private void Start()
    {
        database.createTable("voice_type", "genre TEXT");
        int voz = database.checkIfTableHasContents("voice_type");
        IDataReader reader = (IDataReader)database.executeCommand("SELECT genre FROM voice_type", false, true);
        if (voz == 0) voiceTypeConfig();
        if(reader[0].ToString() == "Male")
        {
            text_top.text = "Male";
            text_bottom.text = "Female";
            voiceType.VoiceGender = voiceTypeSwitch.Genero.masculino;
        }
        else
        {
            voiceType.VoiceGender = voiceTypeSwitch.Genero.feminino;
            text_top.text = "Female";
            text_bottom.text = "Male";
        }
        
    }

    public void voiceTypeConfig()
    {
        database.executeCommand("DELETE FROM voice_type");
        database.executeCommand($"INSERT INTO voice_type (genre) VALUES ('{text_top.text}')");
    }

    
}
