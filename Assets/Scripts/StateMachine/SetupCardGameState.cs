using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetupCardGameState : CardGameState
{
    [SerializeField] private int _startingCardNumber = 104;
    [SerializeField] private int _numPlayers = 2;

    private bool _activated = false;

    public override void Enter()
    {
        Debug.Log("Setup: ...Entering");
        Debug.Log($"Creating {this._numPlayers} players.");
        Debug.Log($"Creating deck with {this._startingCardNumber} cards");

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
