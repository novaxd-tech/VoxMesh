using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class voiceTypeSwitch : MonoBehaviour
{
    public enum Genero
    {
        masculino,
        feminino
    }

    public Genero VoiceGender;

    public TextMeshProUGUI text_top;
    public TextMeshProUGUI text_bottom;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Switch()
    {
        VoiceGender = VoiceGender == Genero.masculino ? Genero.feminino : Genero.masculino;
        string temp_text;
        temp_text = text_bottom.text;
        text_bottom.text = text_top.text;
        text_top.text = temp_text;

    }
}
