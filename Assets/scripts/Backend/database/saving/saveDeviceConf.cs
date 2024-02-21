using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Data;
using TMPro;
using UnityEngine.SceneManagement;

public class saveDeviceConf : MonoBehaviour
{
    public TMP_InputField ip_inputField;
    public GameObject commands_vlg;
    public GameObject devices_vlg;
    public devices_configuration_panel device_configuration;

    [Header("toggle panel")]
    public GameObject device_panel;
    public GameObject device_command_panel;

    [HideInInspector]
    public bool ip_has_changed = false;
   
    public void saveDeviceConfigs()
    {
        if (!device_configuration.gameObject.activeSelf) return;

        //se o ip tiver mudado 
        if (ip_has_changed)
        {

            //define o novo ip para o device que está sendo configurado
            string ip = ip_inputField.text;
            device[] devices = devices_vlg.GetComponentsInChildren<device>();

            foreach(device device in devices)
            {
                if(device.database_id == device_configuration.device_id)
                {
                    device.ip = ip;
                }
            }

            //atualiza (deleta e cria denovo) o device pelo id
            database.executeCommand($"UPDATE devices SET ip = '{ip}' WHERE id = {device_configuration.device_id}", true);
        }

        
        database.executeCommand($"DELETE FROM device_commands WHERE device_id = {device_configuration.device_id}", true);
        //pega os comandos
        for(int i=0; i < commands_vlg.transform.childCount; i++)
        {
            //pega os campos do comando iterado
            Transform command_grid = commands_vlg.transform.GetChild(i).GetChild(0);
            string voice_command = command_grid.GetChild(0).GetComponentInChildren<TMP_InputField>().text.ToLower();
            string server_command = command_grid.GetChild(1).GetComponentInChildren<TMP_InputField>().text.ToLower();
            string port = command_grid.GetChild(2).GetComponentInChildren<TMP_InputField>().text.ToLower();
            string ai_response = command_grid.GetChild(3).GetComponentInChildren<TMP_InputField>().text.ToLower();
            int id = device_configuration.device_id;
            database.executeCommand($"INSERT INTO device_commands (device_id, voice_command, server_command, port, ai_response) VALUES ('{id}', '{voice_command}', '{server_command}', '{port}', '{ai_response}')", true);
        }

        device_command_panel.SetActive(false);
        device_panel.SetActive(true);

    }

    public void IPChanged()
    {
        ip_has_changed = true;
    }
}
