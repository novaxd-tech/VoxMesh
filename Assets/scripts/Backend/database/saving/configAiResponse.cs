using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using TMPro;
using System.Data;
public class configAiResponse : MonoBehaviour
{

    public GameObject VLG;
    public GameObject prefab_resposta;

    public void Start()
    {
        int numero_respostas = database.checkIfTableHasContents("responses");
        if (numero_respostas > 0)
        {
            Destroy(VLG.transform.GetChild(0).gameObject);
            //recupera as respostas do banco de dados
            for(int i = 1; i <= numero_respostas; i++)
            {
                IDataReader reader = (IDataReader)database.executeCommand($"SELECT response FROM responses WHERE id = {i}",false, true);
                //Debug.Log(reader[0].ToString());
                GameObject r = Instantiate(prefab_resposta);
                r.transform.parent = VLG.transform;
                r.GetComponentInChildren<TMP_InputField>().text = reader[0].ToString();
                speechRecognition.respostas.Add(reader[0].ToString());

            }
        }
    }

    public void registerResponse()
    {
        database.executeCommand("DELETE FROM responses");
        for(int i = 0; i < VLG.transform.childCount; i++)
        {
            TMP_InputField i_inputField = VLG.transform.GetChild(i).GetComponentInChildren<TMP_InputField>();
            database.executeCommand($"INSERT INTO responses (response) VALUES ('{i_inputField.text.ToLower()}')");
        }
    }
}
