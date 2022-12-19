using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuCardGameState : CardGameState
{
    public override void Enter()
    {
        Debug.Log("Main Menu: ...Entering");
        
        if(SceneLoader.CurrentSceneName != "MainMenu")
            SceneLoader.LoadScene("MainMenu");
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void Play()
    {
        this.StateMachine.ChangeState<SetupCardGameState>();
    }
}