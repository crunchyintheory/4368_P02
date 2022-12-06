using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class EnemyTurnCardGameState : CardGameState, ICardGameTurnState
{
    private const int DRAW_WGT = 1;
    private const int DRAW_WGT_EMERGENCY = 999;
    private const int WILD_WGT = -2;
    private const int SKIP_WGT = 3;

    public static event Action EnemyTurnBegan;
    public static event Action EnemyTurnEnded;

    [SerializeField] private float _pauseDuration = 1.5f;
    [SerializeField] private Text _enemyHandUI = null;
    [SerializeField] private int _playerHandCriticalSize = 3;

    private int _enemyCardCount = 7;

    private bool HasPlayed;

    public override void Enter()
    {
        Debug.Log("Enemy Turn: ...Enter");
        CardGameSM.CurrentTurn = this;
        EnemyTurnBegan?.Invoke();
        this.HasPlayed = false;
        this._enemyHandUI.text = this.StateMachine.EnemyHand.Size.ToString();

        StartCoroutine(EnemyThinkingRoutine(this._pauseDuration));
    }

    public override void Exit()
    {
        Debug.Log("Enemy Turn: Exit...");
    }

    private Card.UnoColor FindOptimalWildColor()
    {
        Dictionary<Card.UnoColor, int> weights = new();

        foreach (Card card in this.StateMachine.EnemyHand.Cards)
        {
            if (card.IsWild) continue;

            if (!weights.ContainsKey(card.Color))
                weights.Add(card.Color, 1);
            else
                weights[card.Color]++;
        }

        if (weights.Count == 0) 
            return Card.UnoColor.Red;
        
        return weights.OrderBy(x => x.Value).Last().Key;
    }

    private IEnumerator EnemyThinkingRoutine(float pauseDuration)
    {
        while (!this.HasPlayed && this.StateMachine.EnemyHand.Size > 0)
        {
            this.HasPlayed = true;
            Debug.Log("Enemy thinking...");
            yield return new WaitForSeconds(pauseDuration);
        
            Debug.Log("Enemy performs action");

            Card topCard = DiscardPile.TopCard;
            IEnumerable validCards = this.StateMachine.EnemyHand.Cards.Where(card => card.CanBePlayedOn(topCard));

            Card candidate = null;
            int candidateWeight = int.MinValue;

            foreach (Card card in validCards)
            {
                int weight = 0;

                if (card.FlagIs(Card.UnoFlag.Draw))
                {
                    if (this.StateMachine.PlayerHand.Size <= this._playerHandCriticalSize)
                        weight += DRAW_WGT_EMERGENCY * card.DrawAmount;
                    else
                        weight += DRAW_WGT * card.DrawAmount;
                }
            
                else if (card.FlagIs(Card.UnoFlag.Reverse) || card.FlagIs(Card.UnoFlag.Skip))
                    weight += SKIP_WGT;

                if (card.IsWild)
                    weight += WILD_WGT;

                if (weight > candidateWeight)
                {
                    candidate = card;
                    candidateWeight = weight;
                }
            }
            
            if (candidate == null)
            {
                candidate = this.StateMachine.EnemyHand.Draw(this.StateMachine.Deck, 1)[0];
                this._enemyHandUI.text = this.StateMachine.EnemyHand.Size.ToString();
                if (!candidate.CanBePlayedOn(topCard))
                    continue;
                else
                    yield return new WaitForSeconds(pauseDuration);
            }

            if (candidate.IsWild)
            {
                (candidate as WildCard)?.SetColor(FindOptimalWildColor());
            }   
            candidate.Play();

            this._enemyHandUI.text = this.StateMachine.EnemyHand.Size.ToString();
        }
        
        this._enemyHandUI.text = this.StateMachine.EnemyHand.Size.ToString();
        
        // After turn actions
        if (this.StateMachine.EnemyHand.Size == 0)
        {
            yield return new WaitForSeconds(pauseDuration);
            this.StateMachine.ChangeState<LoseCardGameState>();
        }
        else
        {
            EnemyTurnEnded?.Invoke();
            this.StateMachine.ChangeState<PlayerTurnCardGameState>();            
        }
    }

    public void DrawEnemy(int count)
    {
        this.StateMachine.PlayerHand.Draw(this.StateMachine.Deck, count);
    }

    public void SkipEnemyTurn()
    {
        this.HasPlayed = false;
    }
}
