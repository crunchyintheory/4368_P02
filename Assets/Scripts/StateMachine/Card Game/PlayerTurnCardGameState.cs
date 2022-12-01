using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PlayerTurnCardGameState : CardGameState, ICardGameTurnState
{
    public static event Action PlayerTurnBegan;
    public static event Action PlayerTurnEnded;
    [SerializeField] private Text _playerTurnTextUI = null;
    [SerializeField] private Text _playerHandUI = null;

    private int _playerTurnCount = 0;

    private int _playerCardCount = 7;

    public static bool DisableDrawing = false;
    public static bool HasDrawn = false;

    public static bool CanPlayerPlay = false;

    public static bool CanPlayerDraw
    {
        get => !DisableDrawing && !HasDrawn;
        set => DisableDrawing = !value;
    }

    public static Deck Deck => Instance.StateMachine.Deck;

    public static PlayerTurnCardGameState Instance;

    protected override void Awake()
    {
        base.Awake();
        if (Instance != null)
        {
            Destroy(this.gameObject);
            return;
        }

        Instance = this;
    }

    public override void Enter()
    {
        Debug.Log("Player Turn: ...Entering");
        CardGameSM.CurrentTurn = this;
        HasDrawn = false;
        CanPlayerDraw = true;
        CanPlayerPlay = true;
        PlayerTurnBegan?.Invoke();
        
        this._playerTurnCount++;
        this._playerTurnTextUI.text = $"Player Turn: {this._playerTurnCount}";
        
        //Solve race condition
        if(this.StateMachine.PlayerHand) this._playerHandUI.text = $"Player Hand: {this.StateMachine.PlayerHand.Size}";
        //this.StateMachine.Input.PressedConfirm += OnPressedConfirm;

        // Temporary for demo purposes until an actual hand is implemented.
        //this.StateMachine.Input.PressedLeft += OnCardButtonPressed;
        //this.StateMachine.Input.PressedCancel += OnColorSelected;
    }

    public override void Exit()
    {
        CanPlayerPlay = false;
        CanPlayerDraw = false;
        this._playerTurnTextUI.gameObject.SetActive(false);
        PlayerTurnEnded?.Invoke();
        
        //this.StateMachine.Input.PressedConfirm -= OnPressedConfirm;
        
        Debug.Log("Player Turn: Exiting...");
        
        // Temporary for demo purposes until an actual hand is implemented.
        //this.StateMachine.Input.PressedLeft -= OnCardButtonPressed;
        //this.StateMachine.Input.PressedCancel -= OnColorSelected;
    }

    private void OnPressedConfirm()
    {
        Debug.Log("Attempt to enter Enemy State!");
        this.StateMachine.ChangeState<EnemyTurnCardGameState>();
    }

    public void OnCardPlayed()
    {
        this._playerHandUI.text = $"Player Hand: {this.StateMachine.PlayerHand.Size}";
        if (this.StateMachine.PlayerHand.Size == 0)
        {
            this.StateMachine.ChangeState<WinCardGameState>();
            return;
        }

        if (!CanPlayerPlay)
            this.StateMachine.ChangeState<EnemyTurnCardGameState>();
    }

    public void OnCardButtonPressed()
    {
        this._playerHandUI.text = $"Player Hand: {this.StateMachine.PlayerHand.Size}";
        if (this._playerCardCount == 0)
        {
            this.StateMachine.ChangeState<WinCardGameState>();
        }
    }

    public void OnColorSelected()
    {
        this._playerHandUI.text = $"Player Hand: {this.StateMachine.PlayerHand.Size}";
        if (this._playerCardCount == 0)
        {
            this.StateMachine.ChangeState<WinCardGameState>();
        }
    }

    public void DrawEnemy(int count)
    {
        this.StateMachine.EnemyHand.Draw(this.StateMachine.Deck, count);
    }

    public void SkipEnemyTurn()
    {
        HasDrawn = false;
        CanPlayerPlay = true;
    }
}
