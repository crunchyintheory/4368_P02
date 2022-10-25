using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerTurnCardGameState : CardGameState
{
    [SerializeField] private Text _playerTurnTextUI = null;

    private int _playerTurnCount = 0;

    public override void Enter()
    {
        Debug.Log("Player Turn: ...Entering");
        this._playerTurnTextUI.gameObject.SetActive(true);
        
        this._playerTurnCount++;
        this._playerTurnTextUI.text = $"Player Turn: {this._playerTurnCount}";
        StateMachine.Input.PressedConfirm += OnPressedConfirm;
    }

    public override void Exit()
    {
        this._playerTurnTextUI.gameObject.SetActive(false);
        
        StateMachine.Input.PressedConfirm -= OnPressedConfirm;
        
        Debug.Log("Player Turn: Exiting...");
    }

    private void OnPressedConfirm()
    {
        Debug.Log("Attempt to enter Enemy State!");
        this.StateMachine.ChangeState<EnemyTurnCardGameState>();
    }
}
