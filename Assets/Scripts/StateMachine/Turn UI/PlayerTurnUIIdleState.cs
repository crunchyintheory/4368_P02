using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTurnUIIdleState : PlayerTurnUIState
{
    public override void Enter()
    {
        this.StateMachine.PlayerUI.gameObject.SetActive(false);
    }
    
}