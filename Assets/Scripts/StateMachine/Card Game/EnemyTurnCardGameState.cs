using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyTurnCardGameState : CardGameState
{
    public static event Action EnemyTurnBegan;
    public static event Action EnemyTurnEnded;

    [SerializeField] private float _pauseDuration = 1.5f;
    [SerializeField] private Text _enemyHandUI = null;

    private int _enemyCardCount = 7;

    public override void Enter()
    {
        Debug.Log("Enemy Turn: ...Enter");
        EnemyTurnBegan?.Invoke();
        this._enemyHandUI.text = $"Enemy Hand: {this.StateMachine.EnemyHand.Size}";

        StartCoroutine(EnemyThinkingRoutine(this._pauseDuration));
    }

    public override void Exit()
    {
        Debug.Log("Enemy Turn: Exit...");
    }

    private IEnumerator EnemyThinkingRoutine(float pauseDuration)
    {
        Debug.Log("Enemy thinking...");
        yield return new WaitForSeconds(pauseDuration);
        
        Debug.Log("Enemy performs action");
        
        this.StateMachine.EnemyHand.Cards[0].Play();
        this._enemyHandUI.text = $"Enemy Hand: {this.StateMachine.EnemyHand.Size}";
        if (this._enemyCardCount == 0)
        {
            this.StateMachine.ChangeState<LoseCardGameState>();
        }
        else
        {
            EnemyTurnEnded?.Invoke();
            this.StateMachine.ChangeState<PlayerTurnCardGameState>();            
        }
    }
}
