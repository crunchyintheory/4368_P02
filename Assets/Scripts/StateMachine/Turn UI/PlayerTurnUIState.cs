using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerTurnUISM))]
public class PlayerTurnUIState : State
{
    protected PlayerTurnUISM StateMachine { get; private set; }

    void Awake()
    {
        this.StateMachine = GetComponent<PlayerTurnUISM>();
    }
}