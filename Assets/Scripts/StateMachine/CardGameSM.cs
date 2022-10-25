using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardGameSM : StateMachine
{
    [SerializeField] private InputController _input;
    public InputController Input => this._input;
    
    // Start is called before the first frame update
    void Start()
    {
        ChangeState<SetupCardGameState>();
    }
}
