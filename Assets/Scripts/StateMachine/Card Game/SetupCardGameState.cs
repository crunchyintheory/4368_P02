using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetupCardGameState : CardGameState
{
    public static event Action GameBegan;
    public static event Action GameEnded;

    [SerializeField] private Deck _deckPrefab;
    [SerializeField] private Hand _handPrefab;
    
    [SerializeField] private int _startingCardNumber = 104;
    [SerializeField] private int _startingHandSize = 7;
    [SerializeField] private int _numPlayers = 2;

    [SerializeField] private Vector3 _deckPosition = new Vector3(0.1f, -0.1586f, 0.6f);
    [SerializeField] private Vector3 _playerHandPosition = new Vector3(0, -0.0413f, 0.4004f);
    [SerializeField] private Vector3 _playerHandRotation = new Vector3(68.379f, 0, 0);
    [SerializeField] private Vector3 _enemyHandPosition = new Vector3(0, -0.1617f, 0.7678f);
    [SerializeField] private Vector3 _enemyHandRotation = new Vector3(-90f, 0, 0);

    private bool _activated;

    public override void Enter()
    {
        StartCoroutine(LoadGameSceneCoroutine());
        
        Debug.Log("Setup: ...Entering");
        Debug.Log($"Creating {this._numPlayers} players.");
        Debug.Log($"Creating deck with {this._startingCardNumber} cards");

        GameBegan?.Invoke();

        this._activated = false;
    }

    private IEnumerator LoadGameSceneCoroutine()
    {
        SceneLoader.LoadScene("GameScene");

        yield return new WaitForFixedUpdate();
        
        this.StateMachine.Deck = Instantiate(this._deckPrefab, this._deckPosition, Quaternion.identity);
        this.StateMachine.Deck.ResetDeck(); 
        
        this.StateMachine.PlayerHand = Instantiate(this._handPrefab, this._playerHandPosition, Quaternion.Euler(this._playerHandRotation));
        this.StateMachine.PlayerHand.isPlayerHand = true;
        this.StateMachine.Deck.PlayerHand = this.StateMachine.PlayerHand;
        this.StateMachine.Deck.CanPlayerDraw = true;
        this.StateMachine.PlayerHand.Draw(this.StateMachine.Deck, 4, 1f);
        
        this.StateMachine.EnemyHand = Instantiate(this._handPrefab, this._enemyHandPosition, Quaternion.Euler(this._enemyHandRotation));
        this.StateMachine.EnemyHand.Draw(this.StateMachine.Deck, 4, 1f);
    }

    public override void Tick()
    {
        if (this._activated == false)
        {
            this._activated = true;
            this.StateMachine.ChangeState<PlayerTurnCardGameState>();
        }
    }

    public override void Exit()
    {
        this._activated = false;
        Debug.Log("Setup: Exiting...");
    }
}
