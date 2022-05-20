using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class EndObject : MonoBehaviour
{
    public ParticleSystem light;
    public AnimationCurve lightScaling;
    public float scalingRate=0.1f;
    public Vector3 maxScale;
    private void Start()
    {
        light.gameObject.transform.localScale = new Vector3(0f, 0f, 0f);
    }
    private void Update()
    {
        if (EndGameEvents.endGame.ended)
        {
            ScaleTheLight();
        }
    }
    void ScaleTheLight()
    {
        if (light.gameObject.transform.localScale.x>=maxScale.x)
        {
            return;
        }
        Vector3 scaling = new Vector3(1f, 1f, 1f);
        
        light.gameObject.transform.localScale= light.gameObject.transform.localScale+ new Vector3(1f,1f,1f)*scalingRate;
    }

    public void Ending()
    {

        EndGameEvents.endGame.EndGameTrigger();
    }
}
