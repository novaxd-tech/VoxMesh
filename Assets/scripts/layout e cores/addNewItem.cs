using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class addNewItem : MonoBehaviour
{
    //ESSE SCRIPT DEVE SER ATRELADO AO VLG
    public GameObject prefab;
    int childCount 
    {
        get
        {
            return transform.childCount;
        }
    }

    public void addItem(int siblingIndex)
    {
        GameObject newItem = Instantiate(prefab, transform);
        newItem.transform.SetSiblingIndex(siblingIndex+1);
    }

    private void Update(){
        //pega o ultimo filho do meu primeiro filho
        Transform primeiroFilho = transform.GetChild(0);

        //ultimo filho do primeiro filho 
        int primeiroFilhoChildCount = primeiroFilho.childCount;

        // se o numero de filhos for 1 
        if(childCount == 1){
            //desativa o ultimo filho do primeiro filho
            primeiroFilho.GetChild(primeiroFilhoChildCount - 1).GetComponent<Button>().interactable = false;
        }else{
            primeiroFilho.GetChild(primeiroFilhoChildCount - 1).GetComponent<Button>().interactable = true;
        }
    }
}
