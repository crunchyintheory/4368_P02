using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public abstract class GameOverCardGameState : CardGameState
{
    protected abstract string text { get; }
    public static Action OnGameOverEntered;
    public static Action OnGameOverExited;
    
    [SerializeField] private TextMeshProUGUI _textElement;

    public override void Enter()
    {
        if (SceneLoader.CurrentSceneName != "GameOverScene")
            SceneLoader.LoadScene("GameOverScene");
        this._textElement.text = this.text;
        this.StateMachine.Input.PressedCancel += OnCancel;
        OnGameOverEntered?.Invoke();
    }

    public override void Exit()
    {
        this.StateMachine.Input.PressedCancel -= OnCancel;
        OnGameOverExited?.Invoke();
    }

    private void OnCancel()
    {
        Application.Quit();
    }
}
