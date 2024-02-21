using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class addNewResponse : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void add()
    {
        
        speechRecognition.respostas.Add(GetComponent<TMP_InputField>().text);
    }

    public void delete()
    {
        speechRecognition.respostas.Remove(GetComponent<TMP_InputField>().text);
    }
}
