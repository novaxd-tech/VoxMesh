using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Data;
using UnityEngine.UI;

public class configDevice : MonoBehaviour
{
    public GameObject vlg;
    public GameObject prefab_device;


    //pega o registro da tabela no banco de dados assim que for iniciado
    private void Start()
    {
        getDevices();
    }

    public void getDevices()
    {
        //pega a quantidade de registros na tabela "devices"
        int numero_devices = database.checkIfTableHasContents("devices");
        

        if (numero_devices > 0)
        {
            
            for(int i=0; i < vlg.transform.childCount; i++)
            {
                Destroy(vlg.transform.GetChild(i).gameObject);
            }
            
            //itera sobre o numero de devices no banco
            for (int id = 1; id <= numero_devices; id++)
            {
                IDataReader reader = (IDataReader)database.executeCommand($"SELECT device_name, room_id, ip FROM devices WHERE id = {id}", false, true);
                GameObject device = Instantiate(prefab_device);
                device.transform.parent = vlg.transform;
                
                device.GetComponent<device>().database_id = id;
                device.GetComponent<device>().ip = reader[2].ToString();
                device.GetComponent<device>().name = reader[0].ToString();
                device.GetComponent<device>().room_id = int.Parse(reader[1].ToString());
                
                //configura os campos do device
                device.transform.GetChild(0).GetComponentInChildren<TMP_InputField>().text = reader[0].ToString();
                device.transform.GetChild(1).GetComponentInChildren<TMP_InputField>().text = reader[1].ToString();
                
            }
        }
    }

   

    //SALVA OS DEVICES NO BANCO
    public void AddDevice()
    {
        database.executeCommand("DELETE FROM devices", true);
        
        
        //itera sobre os dispositivos definidos
        for(int vlg_child = 0; vlg_child < vlg.transform.childCount; vlg_child++)
        {
            //componentes
            Transform device_T = vlg.transform.GetChild(vlg_child);
            device device = device_T.GetComponent<device>();
             
            //pega o nome e o room_id
            string device_name = device_T.GetChild(0).GetComponentInChildren<TMP_InputField>().text;
            string room_id = device_T.GetChild(1).GetComponentInChildren<TMP_InputField>().text;

            //configura o componente do device
            if (string.IsNullOrEmpty(device_name) || string.IsNullOrEmpty(room_id)) continue;
            device.name = device_name;
            device.room_id = int.Parse(room_id);
            
            

            // SALVA NO BANCO DE DADOS 
            string command_insert_table = $"INSERT INTO devices (ip, device_name, room_id) VALUES ('{device.ip.ToLower()}', '{device_name}', '{room_id}')";
            //string update_table = $"UPDATE devices SET device_name = '{device_name}', room_id = '{room_id}' WHERE id = {vlg_child + 1}";
            database.executeCommand(command_insert_table, true);

            //pega o id do ultimo registro do banco de dados

            
        }
    }
}
