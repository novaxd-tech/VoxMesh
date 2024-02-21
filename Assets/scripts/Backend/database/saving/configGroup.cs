using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Data;
using TMPro;
using UnityEngine.UI;

public class configGroup : MonoBehaviour
{
    public GameObject vlg;
    public GameObject prefab_group;

    //config group 
    public GameObject general;
    public GameObject conf;

    //MESH ID prefab
    public GameObject MESH_ID_PREFAB;



    private void Start()
    {
        getGroups();

        IDataReader teste = (IDataReader)database.executeCommand("SELECT * FROM groups", false, true);
        
    }

    public void getGroups()
    {
        int numero_groups = database.checkIfTableHasContents("groups");
        
        if (numero_groups > 0)
        {
            Destroy(vlg.transform.GetChild(0).gameObject);
            for (int id = 1; id <= numero_groups; id++)
            {
                IDataReader reader = (IDataReader)database.executeCommand($"SELECT nome, devices, access, activate_voice_command, " +
                    $"deactivate_voice_command, activate_command, deactivate_command, activate_command_port, deactivate_command_port FROM groups WHERE id = {id}", false, true);
                GameObject group_obj = Instantiate(prefab_group, vlg.transform);
                Group group = group_obj.GetComponent<Group>();
                
                //FIXED SELF-DEACTIVATING COMPONENTS BUG...
                group.enabled = true;
                group_obj.GetComponent<addDeleteNewItem>().enabled = true;
                Image[] image_components = group.GetComponentsInChildren<Image>();
                foreach(Image img_cmp in image_components)
                {
                    img_cmp.enabled = true;
                }
                
                Button[] button_components = group.GetComponentsInChildren<Button>();
                foreach(Button btn_cmp in button_components)
                {
                    btn_cmp.enabled = true; 
                }
                //FIXED SELF-DEACTIVATING COMPONENTS BUG...

                //setting up group voice commands 
                group_obj.transform.GetChild(0).GetChild(3).GetChild(0).GetComponent<TMP_InputField>().text = reader[3].ToString(); //activate voice command
                group_obj.transform.GetChild(0).GetChild(3).GetChild(1).GetComponent<TMP_InputField>().text = reader[4].ToString(); //deactivate voice command

                //setting up group commands
                group_obj.transform.GetChild(0).GetChild(3).GetChild(2).GetComponent<TMP_InputField>().text = reader[5].ToString(); //activate voice command
                group_obj.transform.GetChild(0).GetChild(3).GetChild(3).GetComponent<TMP_InputField>().text = reader[6].ToString(); //deactivate voice command

                //setting up group command ports
                group_obj.transform.GetChild(0).GetChild(4).GetComponent<TMP_InputField>().text = reader[7].ToString(); //activate command port
                group_obj.transform.GetChild(0).GetChild(5).GetComponent<TMP_InputField>().text = reader[8].ToString(); //deactivate command port

                //setting up access list
                Transform access_vlg = group_obj.transform.GetChild(0).GetChild(2).GetChild(3).GetChild(0);
                string[] IDs = reader[2].ToString().Split(',');
                foreach(string mesh_id in IDs)
                {
                    if (string.IsNullOrEmpty(mesh_id)) continue;
                    GameObject prefab = Instantiate(MESH_ID_PREFAB, access_vlg);
                    prefab.GetComponentInChildren<TextMeshProUGUI>().text = mesh_id;
                }

                //setting up group id and name
                group.id = id;
                group.name = reader[0].ToString();
                group.GetComponentInChildren<TMP_InputField>().text = group.name;


                //setting up group devices
                string[] devices = reader[1].ToString().Split(';');
                for (int i = 0; i < devices.Length; i++)
                {
                    if (devices[i] == "")
                    {
                        break;
                    }
                    group.GetComponent<Group>().devices_id.Add(int.Parse(devices[i]));
                }
            }
        }
    }

    public void register_group()
    {
        string command_clear_table = "DELETE FROM groups";
        database.executeCommand(command_clear_table, true);
        
        for(int i = 0; i < vlg.transform.childCount; i++)
        {
            Group group = vlg.transform.GetChild(i).GetComponent<Group>();

            string group_name = group.inputField.text;
            string group_devices = group.format().ToLower();
            string access = group.getAllowedDevices().ToLower();
            string activate_voice_command = group.transform.GetChild(0).GetChild(3).GetChild(0).GetComponent<TMP_InputField>().text.ToLower();
            string deactivate_voice_command = group.transform.GetChild(0).GetChild(3).GetChild(1).GetComponent<TMP_InputField>().text.ToLower();
            string activate_command = group.transform.GetChild(0).GetChild(3).GetChild(2).GetComponent<TMP_InputField>().text.ToLower();
            string deactivate_command = group.transform.GetChild(0).GetChild(3).GetChild(3).GetComponent<TMP_InputField>().text.ToLower();
            string activate_command_port = group.transform.GetChild(0).GetChild(4).GetComponent<TMP_InputField>().text.ToLower();
            string deactivate_command_port = group.transform.GetChild(0).GetChild(5).GetComponent<TMP_InputField>().text.ToLower();





            //registra no banco 
            string command_insert_group = $"INSERT INTO groups (nome, devices, access, activate_voice_command, deactivate_voice_command, activate_command, deactivate_command, activate_command_port, deactivate_command_port)" +
                $" VALUES ('{group_name}', '{group_devices}', '{access}', '{activate_voice_command}', '{deactivate_voice_command}', '{activate_command}', '{deactivate_command}', '{activate_command_port}', '{deactivate_command_port}')";
            database.executeCommand(command_insert_group, true);
            
            
        }

        general.SetActive(true);
        conf.SetActive(false);
        
    }
}
