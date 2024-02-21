using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Data;
using TMPro;
using UnityEngine.UI;

public class Group : MonoBehaviour
{
    public List<int> devices_id = new List<int>();

    //[HideInInspector]
    public TMP_InputField inputField;

    GameObject Add_groups;
    AddToGroup addToGroup;
    RemoveFromGroup removeFromGroup;
    public GameObject device_prefab;
    public GameObject meshes_vlg;
    public GameObject mesh_id_prefab;

    [HideInInspector]
    public List<string> allowed_IDs = new List<string>();
    
    [HideInInspector]
    public int id;
    
    [HideInInspector]
    public string name;


    private void Start()
    {
        
       // inputField = transform.GetChild(0).GetChild(0).GetComponent<TMP_InputField>();
        addToGroup = FindObjectOfType<AddToGroup>(true);
        removeFromGroup = FindObjectOfType<RemoveFromGroup>(true);
        Add_groups = GameObject.Find("Add Groups");

        inputField.onEndEdit.AddListener(delegate { name = inputField.text; });

        List<string> meshes = RemoveDuplicatesFromList(MyListener.meshes);


        //popula o vlg de meshes
        foreach(string mesh_id in meshes)
        {
            addToAccessVlg(mesh_id);
        }
    }

    public string getAllowedDevices(){
        // PEGA OS IDS QUE ESTÃO ATIVOS NO TOGGLE GROUP 
        for(int i = 0; i < meshes_vlg.transform.childCount; i++){
            Toggle toggle = meshes_vlg.transform.GetChild(i).GetComponent<Toggle>();
            if(toggle.isOn){
                allowed_IDs.Add(toggle.GetComponentInChildren<TextMeshProUGUI>().text);
            }
        }

        // JUNTA TUDO EM UMA STRING SEPARADA POR ','
        return string.Join(",", allowed_IDs);
    }

    private List<T> RemoveDuplicatesFromList<T>(List<T> list)
    {
        list = list.Distinct().ToList();
        return list;
    }

    public void addToAccessVlg(string mesh_id)
    {
        for(int i = 0; i < meshes_vlg.transform.childCount; i++)
        {
            TextMeshProUGUI id = meshes_vlg.transform.GetChild(i).GetComponentInChildren<TextMeshProUGUI>();
            if (id.text == mesh_id) return;
        }
        //ADICIONA UM PREFAB MESH_ID AO VLG DE ACESSO DO GRUPO
        GameObject mesh_obj = Instantiate(mesh_id_prefab, meshes_vlg.transform);


        //MUDA O ID DO PREFAB
        mesh_obj.GetComponentInChildren<TextMeshProUGUI>().text = mesh_id;


        //TOGGLE GROUP
        //mesh_obj.GetComponent<Toggle>().group = meshes_vlg.GetComponent<ToggleGroup>();
    }

    public string format()
    {
        return string.Join(";", devices_id);
    }

    public void config()
    {
        addToGroup.GetComponent<AddToGroup>().grupo = this;
        removeFromGroup.GetComponent<RemoveFromGroup>().grupo = this;
        GameObject General = Add_groups.transform.GetChild(0).gameObject;
        GameObject config = Add_groups.transform.GetChild(1).gameObject;

        General.SetActive(false);
        config.SetActive(true);

        //popula grupo
        config.transform.GetChild(3).GetChild(0).GetComponent<TextMeshProUGUI>().text = name;
        GameObject vlg_group = config.transform.GetChild(3).GetChild(1).GetChild(0).GetChild(0).gameObject;
        //remove todos os filhos de vlg_group
        for (int i = 0; i < vlg_group.transform.childCount; i++)
        {
            Destroy(vlg_group.transform.GetChild(i).gameObject);
        }


        if(devices_id.Count != 0)
        {
            foreach(int i in devices_id)
            {
                IDataReader reader_devices_group = (IDataReader)database.executeCommand($"SELECT device_name FROM devices WHERE id = {i}",false, true);
                GameObject device = Instantiate(device_prefab);
                device.transform.SetParent(vlg_group.transform, false);
                device.GetComponentInChildren<Toggle>().group = vlg_group.GetComponent<ToggleGroup>();
                device.GetComponent<toggle_>().id = i;
                device.GetComponent<toggle_>().name = reader_devices_group[0].ToString();
                //configurando o nome
                device.GetComponentInChildren<TextMeshProUGUI>().text = reader_devices_group[0].ToString();

            }
        }

        //popula devices
        int devices_count = database.checkIfTableHasContents("devices");
        GameObject vlg_devices = config.transform.GetChild(0).GetChild(1).GetChild(0).GetChild(0).gameObject;

        for(int i = 0; i < vlg_devices.transform.childCount; i++)
        {
            Destroy(vlg_devices.transform.GetChild(i).gameObject);
        }
        for (int id = 1; id <= devices_count; id++)
        {
            IDataReader reader_devices = (IDataReader)database.executeCommand($"SELECT device_name FROM devices WHERE id = {id}",false, true);
            GameObject device = Instantiate(device_prefab);
            device.transform.SetParent(vlg_devices.transform, false);
            device.GetComponent<toggle_>().id = id;
            device.GetComponent<toggle_>().name = reader_devices[0].ToString();
            device.GetComponentInChildren<Toggle>().group = vlg_devices.GetComponent<ToggleGroup>();
            device.GetComponentInChildren<TextMeshProUGUI>().text = reader_devices[0].ToString();
            //config.transform.GetChild(3).GetChild(0).GetComponent<TextMeshProUGUI>().text = name;
        }
    }


    public void animation()
    {
        if (!GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("spin")) GetComponent<Animator>().SetTrigger("spin");
    }


}
