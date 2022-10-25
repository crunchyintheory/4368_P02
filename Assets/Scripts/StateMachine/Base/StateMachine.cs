using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class StateMachine : MonoBehaviour
{
    public State CurrentState => this._currentState;
    protected bool InTransition { get; private set; }

    private State _currentState;
    protected State previousState;

    public void ChangeState<T>() where T : State
    {
        T targetState = GetComponent<T>();
        if (targetState == null)
        {
            Debug.LogWarning(@"Cannot change to state, as it 
does not exist on the State Machine Object. 
Make sure you have the desired State attached 
to the State Machine!");
            return;
        }

        InitiateStateChange(targetState);
    }

    public void RevertState()
    {
        if (this.previousState != null)
        {
            InitiateStateChange(this.previousState);
        }
    }

    protected void InitiateStateChange(State targetState)
    {
        if (this._currentState != targetState && !this.InTransition)
        {
            Transition(targetState);
        }
    }

    protected void Transition(State newState)
    {
        // start transition
        this.InTransition = true;
        // transitioning
        this._currentState?.Exit();
        this._currentState = newState;
        this._currentState?.Enter();
        // end transition
        this.InTransition = false;
    }

    private void Update()
    {
        // simulate Update States with 'tick'
        if (this.CurrentState != null && !this.InTransition)
        {
            this.CurrentState.Tick();
        }
    }
}
