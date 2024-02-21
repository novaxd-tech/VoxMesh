            
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class addDeleteNewGroup : MonoBehaviour
{

    public GameObject prefab;
    public Group group;

    private void Start()
    {
        group = GetComponent<Group>();
    }
    public void addItem(){
        GameObject newItem = Instantiate(prefab);
        newItem.transform.parent = transform.parent;
        int index = group.transform.GetSiblingIndex();
        newItem.transform.SetSiblingIndex(index);

        GetComponentInChildren<TMP_InputField>().text = "";
        group.name = "";
        group.devices_id.Clear();
    }

    public void deleteItem()
    {
        Destroy(this.gameObject);
    }
}
