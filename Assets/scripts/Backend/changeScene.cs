using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class changeScene : MonoBehaviour
{

    public string scene;

    public void change()
    {
        
         SceneManager.LoadScene(scene); 
    }
}
