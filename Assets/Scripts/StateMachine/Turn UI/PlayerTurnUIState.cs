using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerTurnUISM))]
public class PlayerTurnUIState : State
{
    protected PlayerTurnUISM StateMachine { get; set; }

    protected virtual void Awake()
    {
        this.StateMachine = GetComponent<PlayerTurnUISM>();
    }
}