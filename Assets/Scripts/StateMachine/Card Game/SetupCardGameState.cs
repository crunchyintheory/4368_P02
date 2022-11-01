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
        
        this.StateMachine.Deck = Instantiate(this._deckPrefab, new Vector3(0.1f, -0.18f, 0.6f), Quaternion.identity);
        this.StateMachine.Deck.ResetDeck(); 
        
        this.StateMachine.PlayerHand = Instantiate(this._handPrefab, new Vector3(0, -0.0413f, 0.4004f), Quaternion.Euler(68.379f, 0, 0));
        this.StateMachine.PlayerHand.Draw(this.StateMachine.Deck, 4);
        
        this.StateMachine.EnemyHand = Instantiate(this._handPrefab, new Vector3(0, -0.1617f, 0.7678f), Quaternion.Euler(-90f, 0, 0));
        this.StateMachine.EnemyHand.Draw(this.StateMachine.Deck, 4);
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
