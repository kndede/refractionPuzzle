using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropEvent : MonoBehaviour
{
    public static DropEvent dropEvent;

    public Action Drop;

    public List<DropAreas> dropAreas;
    public List<DropObject> dropObjects;
    private void Start()
    {
         dropEvent=this;

        
        // foreach (DropAreas item in GameObject.FindObjectsOfType(typeof(DropAreas)))
        //{
        //    dropAreas.Add(item);
        //}
    }

    public void Dropped()
    {
        if (Drop!=null)
        {
            Drop();
        }
    }
}
