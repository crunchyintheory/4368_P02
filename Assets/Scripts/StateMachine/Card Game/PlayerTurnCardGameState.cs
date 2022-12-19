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
    [SerializeField] private Text _playerHandUI = null;
    [SerializeField] private Vector3 _speechBubblePosition;

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
    
    private ParticleSystem _speechBubble;

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
        
        //Solve race condition
        if(this.StateMachine.PlayerHand) this._playerHandUI.text = this.StateMachine.PlayerHand.Size.ToString();
        //this.StateMachine.Input.PressedConfirm += OnPressedConfirm;

        // Temporary for demo purposes until an actual hand is implemented.
        //this.StateMachine.Input.PressedLeft += OnCardButtonPressed;
        //this.StateMachine.Input.PressedCancel += OnColorSelected;
    }

    public override void Exit()
    {
        CanPlayerPlay = false;
        CanPlayerDraw = false;
        PlayerTurnEnded?.Invoke();
        
        //this.StateMachine.Input.PressedConfirm -= OnPressedConfirm;
        
        Debug.Log("Player Turn: Exiting...");
        
        // Temporary for demo purposes until an actual hand is implemented.
        //this.StateMachine.Input.PressedLeft -= OnCardButtonPressed;
        //this.StateMachine.Input.PressedCancel -= OnColorSelected;
    }

    public void CreateSpeechBubble(ParticleSystem _prefab)
    {
        this._speechBubble = Instantiate(_prefab, this._speechBubblePosition, Quaternion.identity);
    }

    private void OnPressedConfirm()
    {
        Debug.Log("Attempt to enter Enemy State!");
        this.StateMachine.ChangeState<EnemyTurnCardGameState>();
    }

    public void OnCardPlayed()
    {
        this._playerHandUI.text = this.StateMachine.PlayerHand.Size.ToString();
        if (this.StateMachine.PlayerHand.Size == 0)
        {
            StartCoroutine(WinCoroutine());
            return;
        }
        else if (this.StateMachine.PlayerHand.Size == 1)
        {
            this._speechBubble.Play();
        }

        if (!CanPlayerPlay)
            this.StateMachine.ChangeState<EnemyTurnCardGameState>();
    }

    private IEnumerator WinCoroutine()
    {
        yield return new WaitForSeconds(1);
        this.StateMachine.ChangeState<WinCardGameState>();
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
