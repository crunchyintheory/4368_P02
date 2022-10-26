using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTurnUIPlayState : PlayerTurnUIState
{
    
    public override void Enter()
    {
        this.StateMachine.PlayerUI.gameObject.SetActive(true);
        this.StateMachine.PlayerTurnTextUI.gameObject.SetActive(true);
        
        // Temporary for demo purposes until an actual hand is implemented.
        this.StateMachine.Input.PressedRight += OnSelectColorButtonClicked;
    }

    public override void Exit()
    {
        this.StateMachine.PlayerUI.gameObject.SetActive(false);
        this.StateMachine.PlayerTurnTextUI.gameObject.SetActive(false);
        
        // Temporary for demo purposes until an actual hand is implemented.
        this.StateMachine.Input.PressedRight -= OnSelectColorButtonClicked;
    }

    public void OnSelectColorButtonClicked()
    {
        this.StateMachine.ChangeState<PlayerTurnUISelectColorState>();
    }
}
