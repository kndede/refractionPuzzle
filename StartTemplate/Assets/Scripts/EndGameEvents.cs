using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndGameEvents : MonoBehaviour
{
    public static EndGameEvents endGame;

    public event Action endIt;
    public bool ended = false;
    private void Awake()
    {
        endGame = this;
       
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
    }
}