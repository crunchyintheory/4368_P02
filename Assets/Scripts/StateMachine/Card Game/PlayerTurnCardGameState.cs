using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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
        this.StateMachine.Input.PressedConfirm += OnPressedConfirm;

        // Temporary for demo purposes until an actual hand is implemented.
        this.StateMachine.Input.PressedLeft += OnCardButtonPressed;
        this.StateMachine.Input.PressedCancel += OnColorSelected;
    }

    public override void Exit()
    {
        this._playerTurnTextUI.gameObject.SetActive(false);
        PlayerTurnEnded?.Invoke();
        
        this.StateMachine.Input.PressedConfirm -= OnPressedConfirm;
        
        Debug.Log("Player Turn: Exiting...");
        
        // Temporary for demo purposes until an actual hand is implemented.
        this.StateMachine.Input.PressedLeft -= OnCardButtonPressed;
        this.StateMachine.Input.PressedCancel -= OnColorSelected;
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
            this.StateMachine.ChangeState<WinCardGameState>();
        }
    }

    public void OnColorSelected()
    {
        this._playerHandUI.text = $"Player Hand: {--this._playerCardCount}";
        if (this._playerCardCount == 0)
        {
            this.StateMachine.ChangeState<WinCardGameState>();
        }
    }
}
