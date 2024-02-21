using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class textChanger : MonoBehaviour
{

    string text1;
    public string text2;
    string text3;
    
    private void Start()
    {
    }

    public void changeText()
    {
        if(GetComponent<TextMeshProUGUI>() != null)
        {
            text3 = GetComponent<TextMeshProUGUI>().text;
            GetComponent<TextMeshProUGUI>().text = text2;
            text2 = text3;
        }else if(GetComponent<Text>() != null)
        {
            text3 = GetComponent<Text>().text;
            GetComponent<Text>().text = text2;
            text2 = text3;
        }

    }
}
