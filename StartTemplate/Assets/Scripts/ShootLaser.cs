using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootLaser : MonoBehaviour
{
    public Material material;
    public Gradient gradient;
    public EndObject endObject;
    public LaserBeam beam;


    public float HitOffset = 0;


    public GameObject flash;
    public GameObject Hit;

    private Vector4 Length = new Vector4(1, 1, 1, 1);

    public AnimationCurve laserWidth;
    int counter = 0;

    public bool seperated = false;
    private void Start()
    {
        if (!seperated)
        {
            CastLaser();
        }

    }
    private void Update()
    {
        //CastLaser();
       
    }

    public void CastLaser()
    {
        if (!EndGameEvents.endGame.ended)
        {

            if (beam != null)
            {


                Destroy(beam.laserObj);




            }

            if (material!=null)
            {

                material.SetTextureScale("_MainTex", new Vector2(Length[0], Length[1]));
                material.SetTextureScale("_Noise", new Vector2(Length[2], Length[3]));

                beam = new LaserBeam(gameObject.transform.position, gameObject.transform.forward, material, gradient, Hit, laserWidth, endObject, seperated);
            }
                //beam.gradient = gradient;
            



        }
        else
        {
            Debug.Log("Congratz!");
        }

        //if (counter < 1)
        //{
        //    Vector3 flashPos = new Vector3(transform.position.x, transform.position.y, transform.position.z + 0.5f);

        //    GameObject.Instantiate(flash, flashPos, Quaternion.identity, this.gameObject.transform);
        //    //flash.transform.Rotate(0.0f, 0.0f, 180f, Space.Self);
            
        //    counter++;
        //}
    }

    public void DestroyLaser()
    {
        Destroy(this.beam.laserObj);
    }





}
