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
    public List<Seperator> mySeperators;
    public Transform parentPos;

    Vector3 initialPos;
    private bool isSeperator = false;

    AnimationScript animationS;
    Vector3 myScale;
    private void Start()
    {
        shooters=FindShooters();
        initialPos = parentPos.position;
        myScale=this.transform.localScale;
        animationS = GetComponent<AnimationScript>();
        //if (this.gameObject.GetComponent<Seperator>()!=null)
        //{
        //    isSeperator = true;
        //}
        //if (this.gameObject.GetComponentsInChildren<Seperator>()!=null)
        //{
        //    foreach (Seperator item in this.gameObject.GetComponentsInChildren<Seperator>())
        //    {
        //        mySeperators.Add(item);
        //    }
        //    isSeperator = true;
        //}
       
    }

    //public UnityEngine.Object[] GetLasers()
    //{
    //    UnityEngine.Object[] lasers = FindObjectsOfType(typeof(LaserBeam));
    //    return lasers;
    //}
    
    public List<ShootLaser> FindShooters()
    {
        List<ShootLaser> shooters = new List<ShootLaser>();
        foreach (ShootLaser item in FindObjectsOfType(typeof(ShootLaser)))
        {
            shooters.Add( item);
        }
        return shooters;
    }
    private void OnMouseDown()
    {
        
        transform.GetComponent<Collider>().enabled = false;
        animationS.isScaling = false;
        animationS.isFloating = false;
        transform.localScale = myScale;
        foreach (var item in transform.GetComponentsInChildren<Collider>())
        {
            item.enabled = false;
        }
        offset = transform.parent.position - MouseWorldPosition();
        //lastPos = transform.position;
        foreach (ShootLaser item in FindShooters())
        {
            if (item.beam.seperated)
            {
                Destroy(item.beam.laserObj);
                Destroy(item);
            }
            else
            {
                item.CastLaser();
            }
            
        }

        //if (isSeperator)
        //{
        //    UnityEngine.Object[] myLasers=GetLasers();
        //    foreach (GameObject item in myLasers)
        //    {
        //        item.gameObject.SetActive(false);
        //    }
        //}
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
                    foreach (ShootLaser item in FindShooters())
                    {
                        if (item.beam.seperated)
                        {
                            Destroy(item.beam.laserObj);
                            Destroy(item);
                        }
                        else
                        {
                            item.CastLaser();
                        }

                    }
                }
                else
                {

                    parentPos.position = initialPos;
                    animationS.isScaling = true;
                    animationS.isFloating = true;
                    foreach (ShootLaser item in FindShooters())
                    {
                        if (item.beam.seperated)
                        {
                            Destroy(item.beam.laserObj);
                            Destroy(item);
                        }
                        else
                        {
                            item.CastLaser();
                        }

                    }
                }
            }
            else
            {

                parentPos.position = initialPos;
                animationS.isScaling = true;
                animationS.isFloating = true;
                if (lastDropArea != null)
                {
                    lastDropArea.Emptying();
                }
                foreach (ShootLaser item in FindShooters())
                {
                    if (item.beam.seperated)
                    {
                        Destroy(item.beam.laserObj);
                        Destroy(item);
                    }
                    else
                    {
                        item.CastLaser();
                    }

                }
            }
        }
        transform.GetComponent<Collider>().enabled = true;
        foreach (var item in transform.GetComponentsInChildren<Collider>())
        {
            item.enabled = true;
        }
        foreach (ShootLaser item in FindShooters())
        {
            if (item.beam.seperated)
            {
                Destroy(item.beam.laserObj);
                Destroy(item);
            }
            else
            {
                item.CastLaser();
            }

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
