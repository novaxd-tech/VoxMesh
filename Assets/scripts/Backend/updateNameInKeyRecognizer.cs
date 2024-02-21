using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class updateNameInKeyRecognizer : MonoBehaviour
{

    speechRecognition speechR;
    public void updateName()
    {
        speechR.addNewName(GetComponent<TMP_InputField>().text);
    }
}
