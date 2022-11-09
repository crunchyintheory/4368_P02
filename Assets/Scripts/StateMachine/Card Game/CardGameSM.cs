using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardGameSM : StateMachine
{
    [SerializeField] private InputController _input;
    public InputController Input => this._input;

    public Deck Deck;
    public Hand PlayerHand;
    public Hand EnemyHand;
    public DiscardPile Discard;

    public static ICardGameTurnState CurrentTurn;
    
    // Start is called before the first frame update
    void Start()
    {
        ChangeState<MainMenuCardGameState>();
    }
}
