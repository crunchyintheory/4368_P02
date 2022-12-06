using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTurnUIPlayState : PlayerTurnUIState
{
    public static PlayerTurnUIPlayState Instance;

    protected override void Awake()
    {
        base.Awake();
        if (Instance != null)
        {
            Debug.Log(Instance);
            //Destroy(this.gameObject);
            return;
        }

        Instance = this;
    }
    
    public override void Enter()
    {
        this.StateMachine.PlayerUI.gameObject.SetActive(true);
    }

    public override void Exit()
    {
        this.StateMachine.PlayerUI.gameObject.SetActive(false);
    }

    public void SelectColor()
    {
        this.StateMachine.ChangeState<PlayerTurnUISelectColorState>();
    }
}
