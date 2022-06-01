using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropAreas : MonoBehaviour
{
    public static DropAreas dropAreas;
    public List<DropObject> dropObjects;
    void Start()
    {
        if (this!=dropAreas)
        {
            dropAreas = this;
        }

    }


    void Update()
    {
        
    }
}
