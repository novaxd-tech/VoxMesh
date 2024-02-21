using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class searchHandler : MonoBehaviour
{
    
    public void onclick(string message)
    {
        if(message == "make_myself_king")
        {
            netHandler.setKingRoleByDefault();
            return;
        }
        netHandler.sendMessage(message);        
    }
}
