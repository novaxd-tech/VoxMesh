using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Data;
public class deviceConfiguration : MonoBehaviour
{

    Transform device_conf_panel;
    Transform thisPanel;
    

    public GameObject command_prefab;

    // Start is called before the first frame update
    void Start()
    {
        Transform devices_content = GameObject.Find("DEVICES_CONTENT").transform;
        device_conf_panel = devices_content.GetChild(1);
        thisPanel = devices_content;
        
    }

    public void loadDeviceConfiguration()
    {

        if (!GetComponent<checkIfDeviceFieldsAreFilledIn>().bothFieldsFilledIn())
        {
            return;
        }

        GameObject save = GameObject.Find("save");
        save.GetComponent<configDevice>().AddDevice();
       
        //troca a tela
        thisPanel.GetChild(0).gameObject.SetActive(false);
        device_conf_panel.gameObject.SetActive(true);
        device_conf_panel.GetComponent<devices_configuration_panel>().device_id = GetComponent<device>().database_id;

        //configura o nome 
        device_conf_panel.GetChild(0).GetComponent<TextMeshProUGUI>().text = GetComponent<device>().name;

        //configura o ip
        device_conf_panel.GetChild(1).GetChild(0).GetComponent<TMP_InputField>().text = GetComponent<device>().ip;

        //referencia o vlg dos comandos
        Transform vlg = device_conf_panel.GetChild(1).GetChild(2).GetChild(0).GetChild(1).GetChild(0);

        //verifica se a tabela tem registro
        IDataReader reader = (IDataReader)database.executeCommand($"SELECT COUNT(*) FROM device_commands WHERE device_id = {GetComponent<device>().database_id}", false, true);

        int command_count = 0;
        while (reader.Read())
        {
            command_count = reader.GetInt32(reader.GetOrdinal("COUNT(*)"));
        }

        //remove o comando ou comandos que estão no vlg
        for(int i = 0; i < vlg.childCount; i++)
        {
            Destroy(vlg.GetChild(i).gameObject);
        }

        if(command_count > 0)
        {

            //puxa os comandos que pertencem ao dispositivo atual
            IDataReader conf = (IDataReader)database.executeCommand($"SELECT voice_command, server_command, port, ai_response FROM device_commands WHERE device_id = {GetComponent<device>().database_id}", false, true);

            bool configuredIP = true;
            //instancia os comandos
            while (conf.Read())
            {
                GameObject comando = Instantiate(command_prefab, vlg);

                //configura o ip
                if (!configuredIP)
                {
                    IDataReader ip = (IDataReader)database.executeCommand($"SELECT ip FROM devices WHERE id = {device_conf_panel.GetComponent<devices_configuration_panel>().device_id}",false,true);
                    Debug.Log(ip.GetString(ip.GetOrdinal("ip")));
                    device_conf_panel.GetChild(1).GetChild(0).GetComponent<TMP_InputField>().text = ip.GetString(ip.GetOrdinal("ip"));
                    configuredIP = true;
                }

                //configura os campos
                comando.transform.GetChild(0).GetChild(0).GetComponentInChildren<TMP_InputField>().text = conf.GetString(conf.GetOrdinal("voice_command"));
                comando.transform.GetChild(0).GetChild(1).GetComponentInChildren<TMP_InputField>().text = conf.GetString(conf.GetOrdinal("server_command"));
                comando.transform.GetChild(0).GetChild(2).GetComponentInChildren<TMP_InputField>().text = conf.GetString(conf.GetOrdinal("port"));
                comando.transform.GetChild(0).GetChild(3).GetComponentInChildren<TMP_InputField>().text = conf.GetString(conf.GetOrdinal("ai_response"));
            }
        }
        else
        {
            Instantiate(command_prefab, vlg);
        }
    }
}
