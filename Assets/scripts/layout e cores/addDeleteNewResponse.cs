
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class addDeleteNewResponse : MonoBehaviour
{

    public GameObject prefab;
    VerticalLayoutGroup VLG;
    private void Start()
    {
        VLG = transform.parent.GetComponent<VerticalLayoutGroup>();
    }
    public void addItem()
    {
        GameObject newItem = Instantiate(prefab);
        newItem.transform.SetParent(VLG.transform);
        int index = transform.GetSiblingIndex();
        newItem.transform.SetSiblingIndex(index+1);


        foreach (TMP_InputField if_ in newItem.GetComponentsInChildren<TMP_InputField>())
        {
            if_.text = "";
        }
    }

    public void deleteItem()
    {
        Destroy(this.gameObject);
    }
}
