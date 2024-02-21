using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Data;

public class changeLanguage : MonoBehaviour
{

    public TextMeshProUGUI texto_top;
    public TextMeshProUGUI texto_bottom;
    string x;

    public enum Lang
    {
        en,
        es
    }
    Lang lang = Lang.en;

    private void Start()
    {
        IDataReader reader = (IDataReader)database.executeCommand("SELECT language FROM lang WHERE id = 1");
        if (reader[0].ToString() == "es") onclick();
    }


    public void onclick()
    {

        //muda os textos do dropdown
        x = texto_top.text;
        texto_top.text = texto_bottom.text;
        texto_bottom.text = x;

        if (lang == Lang.en) lang = Lang.es;
        if (lang == Lang.es) lang = Lang.en;


        //salva no banco de dados a linguagem 
        database.createTable("lang", "language TEXT");
        database.executeCommand("DELETE FROM lang");
        database.executeCommand($"INSERT INTO lang (language) VALUES ('{lang}')");

    }


    public void atualizaTextos()
    {
        //pega todos os objetos que tenham o script 'textChanger'
        textChanger[] changers = GameObject.FindObjectsOfType<textChanger>(true);

        foreach(textChanger changer in changers)
        {
            changer.changeText();
        }
    }
}
