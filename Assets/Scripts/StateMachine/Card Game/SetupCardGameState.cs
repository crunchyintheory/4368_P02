using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetupCardGameState : CardGameState
{
    public static event Action GameBegan;
    public static event Action GameEnded;
    [SerializeField] private int _startingCardNumber = 104;
    [SerializeField] private int _startingHandSize = 7;
    [SerializeField] private int _numPlayers = 2;

    private bool _activated = false;

    public override void Enter()
    {
        SceneLoader.LoadScene("GameScene");
        
        Debug.Log("Setup: ...Entering");
        Debug.Log($"Creating {this._numPlayers} players.");
        Debug.Log($"Creating deck with {this._startingCardNumber} cards");
        GameBegan?.Invoke();

        this._activated = false;
    }

    public override void Tick()
    {
        if (this._activated == false)
        {
            this._activated = true;
            this.StateMachine.ChangeState<PlayerTurnCardGameState>();
        }
    }

    public override void Exit()
    {
        this._activated = false;
        Debug.Log("Setup: Exiting...");
    }
}
