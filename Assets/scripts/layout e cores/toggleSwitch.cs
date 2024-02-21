using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class toggleSwitch : MonoBehaviour
{

    Toggle toggle_me;
    TextMeshProUGUI label_toggle;

    public GameObject objectToDeactivate;
    public GameObject objectToActivate;

    // Start is called before the first frame update
    void Start()
    {
        toggle_me = GetComponent<Toggle>();
        label_toggle = GetComponentInChildren<TextMeshProUGUI>();
        changeTextColor();
    }


    public void changeTextColor()
    {
        if(toggle_me.isOn == true)
        {
            label_toggle.color = Color.white;
        }
        else
        {
            label_toggle.color = Color.blue;
        }
    }

    public void gameObjectSwitch()
    {
        objectToActivate.SetActive(true);
        objectToDeactivate.SetActive(false);
    }

    public static void update()
    {
        //PASSO 6
        //ATUALIZA OS DEVICES E GRUPOS
        
        if (MyListener.role != "king") return;
        GameObject save = GameObject.Find("save");
        
    }
}
