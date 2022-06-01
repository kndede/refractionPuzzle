using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragDrop : MonoBehaviour
{
    Vector3 offset;
    public DropObject dropArea;
    public DropObject lastDropArea;
     Vector3 lastPos;
    public ShootLaser shooter;

    Vector3 initialPos;
    private void Start()
    {
        shooter = (ShootLaser)FindObjectOfType(typeof(ShootLaser));
        initialPos = transform.position;
    }
    private void OnMouseDown()
    {

        transform.GetComponent<Collider>().enabled = false;
        offset = transform.position - MouseWorldPosition();
        //lastPos = transform.position;
        shooter.CastLaser();
    }
    private void OnMouseDrag()
    {
        transform.position = MouseWorldPosition() + offset;
    }

    private void OnMouseUp()
    {
        var rayOrigin = Camera.main.transform.position;
        var rayDir = MouseWorldPosition() - Camera.main.transform.position;
        RaycastHit hitInfo;
        if (Physics.Raycast(rayOrigin,rayDir,out hitInfo))
        {
            if (hitInfo.transform.gameObject.GetComponent<DropObject>())
            {
                
                dropArea = hitInfo.transform.gameObject.GetComponent<DropObject>();
                if (dropArea.dropIndex==0)
                {

                    transform.position = dropArea.GetDropPosition();

                    dropArea.Fill();
                    if (lastDropArea!=null)
                    {
                        lastDropArea.Emptying();
                    }
                    lastDropArea = dropArea;
                    shooter.CastLaser();
                }
                else
                {
                   
                    transform.position = initialPos;
                    shooter.CastLaser();
                }
            }
            else
            {
               
                transform.position = initialPos;
                shooter.CastLaser();
            }
        }
        transform.GetComponent<Collider>().enabled = true;
        shooter.CastLaser();
    }
    private Vector3 MouseWorldPosition()
    {

        var mouseScreenPos = Input.mousePosition;
        mouseScreenPos.z = Camera.main.WorldToScreenPoint(transform.position).z;
        return Camera.main.ScreenToWorldPoint(mouseScreenPos);
    }

    void ReturnToLastDrop()
    {

    }
}
