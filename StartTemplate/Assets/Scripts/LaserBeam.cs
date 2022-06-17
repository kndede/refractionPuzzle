using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserBeam 
{
    Vector3 pos, dir;

    public Gradient gradient;

    public GameObject laserObj;
    LineRenderer laser;
    List<Vector3> laserIndices = new List<Vector3>();
    EndObject endObject;

    private GameObject flash;
    private GameObject Hit;

    public float nozzlePoint=0.5f;

    public ShootLaser shootLaser;
    public bool seperated;

    
    Dictionary<string, float> refractiveMaterials = new Dictionary<string, float>()
    {
        {"Air",1.0f },
        {"Glass",1.5f }
    };
    public LaserBeam(Vector3 pos, Vector3 dir, Material mat,Gradient gradient,GameObject hit,AnimationCurve widthCurve,EndObject thisEndObject,bool isSeperated)
    {
        this.laser = new LineRenderer();
        this.laserObj = new GameObject();
        this.laserObj.name = "Laser Beam";
        this.pos = pos;
        this.dir = dir;
        this.Hit = hit;

        this.laser = this.laserObj.AddComponent(typeof(LineRenderer)) as LineRenderer;
        this.laser.widthCurve = widthCurve;
        this.laser.material = mat;
        this.laser.colorGradient = gradient;
        this.laser.textureMode = LineTextureMode.Stretch;
        this.seperated = isSeperated;
        endObject = thisEndObject;
        //this.laser.startColor = Color.green;
        //this.laser.endColor = Color.green;
        string myTag = "Laserbeam";
        laserObj.tag = myTag;
        CastRay(pos, dir, laser);
    }
   
    void CastRay(Vector3 pos, Vector3 dir, LineRenderer laser)
    {
        
        laserIndices.Add(pos);
        Ray ray = new Ray(pos, dir);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 30, 1))
        {
            CheckHit(hit, dir, laser);
        }
        else
        {
            laserIndices.Add(ray.GetPoint(30));
            UpdateLaser();
        }
    }
    private bool isEnded=false;
    public EndObject lastHitObject;
    void TurnEndObjectFalse()
    {
        if (lastHitObject != null)
        {

            lastHitObject.endHit = false;
        }
        
        
    }
    private void CheckHit(RaycastHit hitInfo, Vector3 direction, LineRenderer laser)
    {
        if (hitInfo.collider.gameObject.tag == "Mirror")
        {
                TurnEndObjectFalse();
            
            Vector3 pos = hitInfo.point;
            Vector3 dir = Vector3.Reflect(direction, hitInfo.normal);
            GameObject hitEffect=GameObject.Instantiate(Hit, hitInfo.point, Quaternion.identity, this.laserObj.transform);
            hitEffect.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
            CastRay(pos, dir, laser);
        }
        else if (hitInfo.collider.gameObject.tag == "Refract")
        {
            

                TurnEndObjectFalse();
            
            Vector3 pos = hitInfo.point;
            laserIndices.Add(pos);

            Vector3 newPos1 = new Vector3(Mathf.Abs(direction.x) / (direction.x + 0.0001f) * 0.001f + pos.x, Mathf.Abs(direction.y) / (direction.y + 0.0001f) * 0.001f + pos.y, Mathf.Abs(direction.z) / (direction.z + 0.0001f) * 0.001f + pos.z);

            float n1 = refractiveMaterials["Air"];
            float n2 = refractiveMaterials["Glass"];

            Vector3 normal = hitInfo.normal;
            Vector3 incident = direction;

            Vector3 refractedVector = Refract(n1, n2, normal, incident);

            Ray ray1 = new Ray(newPos1, refractedVector);
            Vector3 newRayStartPosition = ray1.GetPoint(1.5f);

            Ray ray2 = new Ray(newRayStartPosition, -refractedVector);
            RaycastHit hit2;

            if (Physics.Raycast(ray2, out hit2, 1.6f, 1))
            {
                laserIndices.Add(hit2.point);
            }
            UpdateLaser();

            Vector3 refractedVector2 = Refract(n2, n1, -hit2.normal, refractedVector);
            CastRay(hit2.point, refractedVector2, laser);
        }
        else if (hitInfo.collider.gameObject.GetComponent<EndObject>()!=null)
        {
            
            GameObject hitEffect = GameObject.Instantiate(Hit, hitInfo.point, Quaternion.identity, this.laserObj.transform);
            EndObject endObject1 = hitInfo.collider.gameObject.GetComponent<EndObject>();
            lastHitObject = endObject1;
            hitEffect.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
            endObject1.endHit = true;
           
            //isEnded = true;
            laserIndices.Add(hitInfo.point);
            UpdateLaser();

        }
        else if (hitInfo.collider.gameObject.tag == "Gem")
        {
            GameManager.instance.CollectGem(hitInfo.transform.position, 100);
            hitInfo.collider.gameObject.SetActive(false);
           // laserIndices.Add(hitInfo.point);
           // UpdateLaser();

            CastRay(hitInfo.point, direction, laser);
        }
        else if (hitInfo.collider.gameObject.tag == "Seperator")
        {
            Component[] seperators=hitInfo.collider.gameObject.GetComponentsInChildren(typeof(Seperator));
            foreach (Component item in seperators)
            {
                Seperator sep = item.GetComponent<Seperator>();

               ShootLaser _laser = sep.gameObject.AddComponent<ShootLaser>();
                _laser.seperated = true;
                    _laser.gameObject.transform.SetParent(sep.transform);
                  _laser.endObject = endObject;
                 _laser.Hit = Hit;
                  _laser.material = laser.material;
                   _laser.gradient = laser.colorGradient;
                  _laser.laserWidth = laser.widthCurve;
                _laser.CastLaser();

                
                
            }
            GameObject.Instantiate(Hit, hitInfo.point, Quaternion.identity, this.laserObj.transform);
            

                TurnEndObjectFalse();
            
            laserIndices.Add(hitInfo.point);
            UpdateLaser();
        }
        else
        {
            GameObject.Instantiate(Hit, hitInfo.point, Quaternion.identity, this.laserObj.transform);
            

                TurnEndObjectFalse();
            
            laserIndices.Add(hitInfo.point);
            UpdateLaser();
        }
    }
    Vector3 Refract(float n1, float n2, Vector3 normal, Vector3 incident)
    {
        incident.Normalize();
        Vector3 refractedVector = (n1 / n2 * Vector3.Cross(normal, Vector3.Cross(-normal, incident)) - normal * Mathf.Sqrt(1 - Vector3.Dot(Vector3.Cross(normal, incident) * (n1 / n2 * n1 / n2), Vector3.Cross(normal, incident)))).normalized;

        return refractedVector;
    }
    void UpdateLaser()
    {
        int count = 0;
        laser.positionCount = laserIndices.Count;
             foreach (Vector3 idx in laserIndices)
                 {
                     laser.SetPosition(count, idx);
                    // if (count!=0)
                     //   {
                
                          //   Vector3 hitPos = idx - laserIndices[count].normalized * 0.05f;
                          //  if (laserIndices.Count==(count-1))
                            //     {
                               //      GameObject.Instantiate(Hit, hitPos, Quaternion.identity, this.laserObj.transform);
                             //        }
                           //  else
                           //     {
                           //             GameObject.Instantiate(Hit, hitPos, Quaternion.identity, this.laserObj.transform);
                           //               Hit.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
                        //  }
                
                   // }
           
            count++;
        }
    }
    void EndGame()
    {

    }
}
