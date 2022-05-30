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

    private GameObject flash;
    private GameObject Hit;

    public float nozzlePoint=0.5f;

    Dictionary<string, float> refractiveMaterials = new Dictionary<string, float>()
    {
        {"Air",1.0f },
        {"Glass",1.5f }
    };
    public LaserBeam(Vector3 pos, Vector3 dir, Material mat,Gradient gradient,GameObject hit,AnimationCurve widthCurve)
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
        //this.laser.startColor = Color.green;
        //this.laser.endColor = Color.green;

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
    private void CheckHit(RaycastHit hitInfo, Vector3 direction, LineRenderer laser)
    {
        if (hitInfo.collider.gameObject.tag == "Mirror")
        {
            EndObject.endHit = false;
            Vector3 pos = hitInfo.point;
            Vector3 dir = Vector3.Reflect(direction, hitInfo.normal);

            CastRay(pos, dir, laser);
        }
        else if (hitInfo.collider.gameObject.tag == "Refract")
        {
            EndObject.endHit = false;
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
            // Vector3 pos = hitInfo.point;
            EndObject eo = hitInfo.collider.gameObject.GetComponent<EndObject>();
            EndObject.endHit = true;
           
            isEnded = true;

            laserIndices.Add(hitInfo.point);
            UpdateLaser();

        }
        else
        {
            EndObject.endHit = false;
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
            if (count!=0)
            {
                
                Vector3 hitPos = idx - laserIndices[count].normalized * 0.05f;
                if (laserIndices.Count==(count-1))
                {
                    GameObject.Instantiate(Hit, hitPos, Quaternion.identity, this.laserObj.transform);
                }
                else
                {
                    GameObject.Instantiate(Hit, hitPos, Quaternion.identity, this.laserObj.transform);
                    Hit.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
                }
                
            }
           
            count++;
        }
    }
    void EndGame()
    {

    }
}
