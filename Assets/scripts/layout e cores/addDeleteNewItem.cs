
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class addDeleteNewItem : MonoBehaviour
{
    

    public void addItem()
    {        
        int index = transform.GetSiblingIndex();
        transform.parent.GetComponent<addNewItem>().addItem(index);
    }

    public void deleteItem()
    {
        Destroy(this.gameObject);
    }
}
