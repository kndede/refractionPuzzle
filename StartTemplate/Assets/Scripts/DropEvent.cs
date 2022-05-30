using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropEvent : MonoBehaviour
{
    public static DropEvent dropEvent;

    public Action Drop;

    private void Start()
    {
         dropEvent=this;
    }

    public void Dropped()
    {
        if (Drop!=null)
        {
            Drop();
        }
    }
}
