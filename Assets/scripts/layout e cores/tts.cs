using ACTA;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityText2Speech;
using TMPro;

public class tts : MonoBehaviour
{

    public voiceTypeSwitch voiceType;
    public USgs SGS;

    [Header("Male and Female Voice Models")]
    public string maleVoiceModelPath;
    public string maleVoiceModelPathJson;

    public string femaleVoiceModelPath;
    public string femaleVoiceModelPathJson;

    


    public void fala(string text)
    {
        /*Debug.Log(text);
        // se vt = masculino, usa plugin, se vt = feminino, usa dll windows
        if(voiceType.VoiceGender == voiceTypeSwitch.Genero.masculino)
        {
            SGS.voiceModelFilePath = maleVoiceModelPath;
            SGS.voiceConfigFilePath = maleVoiceModelPathJson;
            SGS.ReceiveTextToSpeech(text);
        }
        else
        {
            SGS.voiceModelFilePath = femaleVoiceModelPath;
            SGS.voiceConfigFilePath = femaleVoiceModelPathJson;
            SGS.ReceiveTextToSpeech(text);
        }*/


        SGS.ReceiveTextToSpeech(text);
    }

    public void sendFala(TMP_InputField t)
    {
        fala(t.text);
    }
}
