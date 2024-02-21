using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class buttonToggle : MonoBehaviour
{
    public enum State
    {
        on,
        off
    }

    public State state;
    public Animator animator;
   

    public void changeState()
    {
        if (state == State.on)
        {
            state = State.off;
            animator.SetTrigger("off");
        }
        else if (state == State.off)
        {
            state = State.on;
            animator.SetTrigger("on");
        }
    }

    public void setOn()
    {
        state = State.off;
        animator.SetTrigger("off");
    }

    public void setOff()
    {
        state = State.on;
        animator.SetTrigger("on");
    }
}
