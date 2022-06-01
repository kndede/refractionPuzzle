using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropObject : MonoBehaviour
{
    [SerializeField]
    private Transform pos;
    public int dropIndex;
    private void Start()
    {
        dropIndex = 0;
    }
    public void Fill() { dropIndex++; }
    public void Emptying() { dropIndex--; }
    public Vector3 GetDropPosition()
    {
        
        return this.pos.position;
    }

    
}
