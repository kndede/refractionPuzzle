using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndGameEvents : MonoBehaviour
{
    public static EndGameEvents endGame;
    public List<EndObject> endObjects;

    public event Action endIt;
    public bool ended = false;

    public GameObject successCanvas;
    
    private void Awake()
    {
        endGame = this;
        successCanvas.SetActive(false);
    }

    int objectCount = 0;
    private void Start()
    {
        endIt += End;
        foreach (EndObject item in GameObject.FindObjectsOfType(typeof(EndObject)))
        {
            objectCount++;
            endObjects.Add(item);
        }
    }
    public void EndGameTrigger()
    {
        if (endIt != null)
        {
            endIt();
        }
    }
    public void CheckAll()
    {
        int trueCount=0;
        foreach (EndObject item in endObjects)
        {
            if (item.scalingOver)
            {
                trueCount++;
            }
        }
        if (trueCount==objectCount)
        {
            EndGameTrigger();
        }
    }
    void End()
    {
        ended = true;
        GameManager.instance.LevelComplete();
    }
}
