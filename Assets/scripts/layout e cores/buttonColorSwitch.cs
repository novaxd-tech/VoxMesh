using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class buttonColorSwitch : MonoBehaviour
{



    Button me_button;
    TextMeshProUGUI me_text;
    // Start is called before the first frame update
    void Start()
    {
        me_text = GetComponentInChildren<TextMeshProUGUI>();
        me_button = GetComponent<Button>();
    }


    
}
