using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Data;

public class checkIfDeviceFieldsAreFilledIn : MonoBehaviour
{
    public TMP_InputField name;
    public TMP_InputField room;

    private bool nameFilledIn;
    private bool roomFilledIn;
    

    public void filledName()
    {

        string name_ = name.text;
        
        // database.executeCommand($"UPDATE devices SET device_name = '{name_}' WHERE id = {GetComponent<device>().database_id}");
        if (string.IsNullOrEmpty(name.text))
        {
            nameFilledIn = false;
        }
        nameFilledIn = true;
    }

    public void filledRoom()
    {
        if (string.IsNullOrEmpty(room.text))
        {
            roomFilledIn = false;
        }
        string room_ = room.text;
        //database.executeCommand($"UPDATE devices SET room_id = '{room_}' WHERE id = {GetComponent<device>().database_id}");

        roomFilledIn = true;
    }

    public bool bothFieldsFilledIn()
    {
        if(nameFilledIn && roomFilledIn)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
