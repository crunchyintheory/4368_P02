using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTurnUISelectColorState : PlayerTurnUIPlayState
{
    public new static PlayerTurnUISelectColorState Instance;
    
    public delegate void OnColorSelectedEventHandler(Card.UnoColor color);
    public static event OnColorSelectedEventHandler OnColorSelected;

    private GameObject _selectColorUI;

    protected override void Awake()
    {
        this.StateMachine = GetComponent<PlayerTurnUISM>();
        if (Instance != null)
        {
            Destroy(this.gameObject);
            return;
        }

        Instance = this;
    }
    
    public override void Enter()
    {
        this.StateMachine.PlayerUI.gameObject.SetActive(false);
        this.StateMachine.PlayerTurnTextUI.gameObject.SetActive(true);
        this._selectColorUI = Instantiate(this.StateMachine.SelectColorUI.gameObject, this.transform);
    }

    public override void Exit()
    {
        this.StateMachine.PlayerUI.gameObject.SetActive(true);
        this.StateMachine.PlayerTurnTextUI.gameObject.SetActive(false);
        Destroy(this._selectColorUI);
    }

    public void ColorSelected(Card.UnoColor color)
    {
        OnColorSelected?.Invoke(color);
        this.StateMachine.ChangeState<PlayerTurnUIPlayState>();
    }
}