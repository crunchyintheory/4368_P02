using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// ReSharper disable once IdentifierTypo
public class PlayerTurnUISM : StateMachine
{
    [SerializeField] public RectTransform PlayerUI = null;
    [SerializeField] public RectTransform SelectColorUI = null;
    
    [SerializeField] private InputController _input;
    public InputController Input => this._input;
    
    // Start is called before the first frame update
    void Start()
    {
        ChangeState<PlayerTurnUIIdleState>();
    }

    private void OnEnable()
    {
        PlayerTurnCardGameState.PlayerTurnBegan += OnPlayerTurnBegan;
        PlayerTurnCardGameState.PlayerTurnEnded += OnPlayerTurnEnded;
    }

    private void OnDisable()
    {
        PlayerTurnCardGameState.PlayerTurnBegan -= OnPlayerTurnBegan;
        PlayerTurnCardGameState.PlayerTurnEnded -= OnPlayerTurnEnded;
    }

    void OnPlayerTurnBegan()
    {
        ChangeState<PlayerTurnUIPlayState>();
    }

    void OnPlayerTurnEnded()
    {
        ChangeState<PlayerTurnUIIdleState>();
    }
}