using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndGameEvents : MonoBehaviour
{
    public static EndGameEvents endGame;

    public event Action endIt;
    public bool ended = false;

    public GameObject successCanvas;
    
    private void Awake()
    {
        endGame = this;
        successCanvas.SetActive(false);
    }
    private void Start()
    {
        endIt += End;
    }
    public void EndGameTrigger()
    {
        if (endIt != null)
        {
            endIt();
        }
    }

    void End()
    {
        ended = true;
        successCanvas.SetActive(true);
    }
}
