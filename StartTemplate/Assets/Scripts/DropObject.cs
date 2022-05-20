using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropObject : MonoBehaviour
{
    [SerializeField]
    private Transform pos;
    private void Start()
    {
        
    }

    public Vector3 GetDropPosition()
    {
        return this.pos.position;
    }
}
