using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mouseDetection : MonoBehaviour
{
    public void MouseOver()
    {
        triangulo.SetTrigger("fadeout");
        GetComponent<Animator>().SetTrigger("open");
    }

    public void MouseExit()
    {
        triangulo.SetTrigger("fadein");
        GetComponent<Animator>().SetTrigger("close");
    }

    bool mouseOn;
    public Animator triangulo;
    private void Update()
    {
        Vector2 mousePos = Input.mousePosition;
       
        RaycastHit2D raycast = Physics2D.Raycast(mousePos, Vector2.zero);

        if(raycast.collider != null)
        {
            
            if(raycast.collider == GetComponent<Collider2D>() && mouseOn == false)
            {
            
                mouseOn = true;
                MouseOver();
            }
        }
        else
        {
            if(mouseOn == true)
            {
                mouseOn = false;    
                MouseExit();
            }
        }

    }
}
