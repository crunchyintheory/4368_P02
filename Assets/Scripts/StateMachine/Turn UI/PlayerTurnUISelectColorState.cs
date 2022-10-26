using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTurnUISelectColorState : PlayerTurnUIPlayState
{
    public override void Enter()
    {
        this.StateMachine.PlayerUI.gameObject.SetActive(false);
        this.StateMachine.PlayerTurnTextUI.gameObject.SetActive(true);
        this.StateMachine.SelectColorUI.gameObject.SetActive(true);

        this.StateMachine.Input.PressedCancel += OnColorSelected;
    }

    public override void Exit()
    {
        this.StateMachine.PlayerUI.gameObject.SetActive(true);
        this.StateMachine.PlayerTurnTextUI.gameObject.SetActive(false);
        this.StateMachine.SelectColorUI.gameObject.SetActive(false);
        
        this.StateMachine.Input.PressedCancel -= OnColorSelected;
    }

    public void OnColorSelected()
    {
        this.StateMachine.ChangeState<PlayerTurnUIPlayState>();
    }
}