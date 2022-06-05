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
    public List<ShootLaser> shooters;
    public Transform parentPos;

    Vector3 initialPos;
    private void Start()
    {
        foreach (ShootLaser item in FindObjectsOfType(typeof(ShootLaser)))
        {
            shooters.Add(item);
        }
        initialPos = parentPos.position;
    }
    private void OnMouseDown()
    {

        transform.GetComponent<Collider>().enabled = false;
        offset = transform.position - MouseWorldPosition();
        //lastPos = transform.position;
        foreach (ShootLaser item in shooters)
        {

            item.CastLaser();
        }
    }
    private void OnMouseDrag()
    {
        parentPos.position = MouseWorldPosition() + offset;
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

                    parentPos.position = dropArea.GetDropPosition();

                    dropArea.Fill();
                    if (lastDropArea!=null&&lastDropArea!=dropArea)
                    {
                        lastDropArea.Emptying();
                    }
                    lastDropArea = dropArea; 
                    foreach (ShootLaser item in shooters)
                    {

                        item.CastLaser();
                    }
                }
                else
                {

                    parentPos.position = initialPos;
                    foreach (ShootLaser item in shooters)
                    {

                        item.CastLaser();
                    }
                }
            }
            else
            {

                parentPos.position = initialPos;
                dropArea.dropIndex--;
                foreach (ShootLaser item in shooters)
                {

                    item.CastLaser();
                }
            }
        }
        transform.GetComponent<Collider>().enabled = true;
        foreach (ShootLaser item in shooters)
        {

            item.CastLaser();
        }
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
