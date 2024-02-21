using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class RemoveFromGroup : MonoBehaviour
{

    public ToggleGroup tg_group;
    
    [HideInInspector]
    public Group grupo;
    public void removeFromGroup()
    {
        if (tg_group.AnyTogglesOn())
        {
            for (int i = 0; i < tg_group.transform.childCount; i++)
            {
                if (tg_group.transform.GetChild(i).GetComponentInChildren<Toggle>().isOn)
                {
                    grupo.devices_id.Remove(tg_group.transform.GetChild(i).GetComponent<toggle_>().id);
                    Destroy(tg_group.transform.GetChild(i).gameObject);
                }
            }
        }
    }
}
