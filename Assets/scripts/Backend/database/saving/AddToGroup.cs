using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Data;


public class AddToGroup : MonoBehaviour
{

    public ToggleGroup tg_devices;

    [HideInInspector]
    public Group grupo;

    public ToggleGroup tg_group;
    public GameObject device_prefab;




    public void addToGroup()
    {
        //pega o device selecionado
        if (tg_devices.AnyTogglesOn())
        {
            for (int i = 1; i <= tg_devices.transform.childCount; i++)
            {
                if (tg_devices.transform.GetChild(i-1).GetComponentInChildren<Toggle>().isOn)
                {
                    IDataReader reader = (IDataReader)database.executeCommand($"SELECT device_name FROM devices WHERE id = {i}",false, true);
                    //adiciona o id do device selecionado ao grupo
                    int id = tg_devices.transform.GetChild(i-1).GetComponent<toggle_>().id;
                    if (grupo.devices_id.Contains(id)) return;
                    grupo.devices_id.Add(id);

                    GameObject device = Instantiate(device_prefab, tg_group.transform);
                    device.GetComponentInChildren<TextMeshProUGUI>().text = reader[0].ToString();
                    device.GetComponentInChildren<Toggle>().isOn = false;
                    device.GetComponentInChildren<Toggle>().group = device.transform.parent.GetComponent<ToggleGroup>();
                    device.GetComponent<toggle_>().id = i;
                    
                }
            }
        }
    }
}
