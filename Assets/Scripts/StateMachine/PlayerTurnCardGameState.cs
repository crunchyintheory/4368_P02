using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerTurnCardGameState : CardGameState
{
    public static event Action PlayerTurnBegan;
    public static event Action PlayerTurnEnded;
    [SerializeField] private Text _playerTurnTextUI = null;
    [SerializeField] private Text _playerHandUI = null;

    private int _playerTurnCount = 0;

    private int _playerCardCount = 7;

    public override void Enter()
    {
        Debug.Log("Player Turn: ...Entering");
        PlayerTurnBegan?.Invoke();
        
        this._playerTurnCount++;
        this._playerTurnTextUI.text = $"Player Turn: {this._playerTurnCount}";
        this._playerHandUI.text = $"Player Hand: {this._playerCardCount}";
        StateMachine.Input.PressedConfirm += OnPressedConfirm;
    }

    public override void Exit()
    {
        this._playerTurnTextUI.gameObject.SetActive(false);
        PlayerTurnEnded?.Invoke();
        
        StateMachine.Input.PressedConfirm -= OnPressedConfirm;
        
        Debug.Log("Player Turn: Exiting...");
    }

    private void OnPressedConfirm()
    {
        Debug.Log("Attempt to enter Enemy State!");
        this.StateMachine.ChangeState<EnemyTurnCardGameState>();
    }

    public void OnCardButtonPressed()
    {
        this._playerHandUI.text = $"Player Hand: {--this._playerCardCount}";
        if (this._playerCardCount == 0)
        {
            //this.StateMachine.ChangeState<WinState>();
        }
    }
}
