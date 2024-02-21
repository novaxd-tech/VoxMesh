using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System;
using System.Security.Cryptography;
using TMPro;
using System.Data;
using UnityEngine.SceneManagement;
using Mono.Data.Sqlite;


public class setUpLoginConfig : MonoBehaviour
{
    public TMP_InputField login;
    public TMP_InputField password;

    

    public void SetLogin()
    {

        string login_data = sha256(login.text + "s4l7_4b$urd0B");
        string password_data = sha256(password.text + "s4l7_4b$urd0A");
        
        if(database.checkIfTableHasContents("credentials") == 1)
        {
            IDataReader reader = (IDataReader)database.executeCommand("SELECT * FROM credentials", true);
            if (login_data == reader[1].ToString() && password_data == reader[2].ToString())
            {
                SceneManager.LoadScene("Painel");
            }
            else
            {
                Debug.Log("CREDENCIAIS NÃO CONDIZEM!!");
            }
        }
        else
        {
            //se a tabela não existir...
            database.executeCommand($"INSERT INTO credentials (login, password) VALUES ('{login_data}','{password_data}')");
            SceneManager.LoadScene("Painel");

        }

       

        
    }

    private string sha256(string text)
    {
        byte[] bytes = Encoding.UTF8.GetBytes(text);
        SHA256Managed hashstring = new SHA256Managed();
        byte[] hash = hashstring.ComputeHash(bytes);
        string hashString = string.Empty;
        foreach (byte x in hash)
        {
            hashString += String.Format("{0:x2}", x);
        }
        return hashString;
    }
}