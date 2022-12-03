using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FiniteStateMachine
{
    public State currState { get; private set; }

    public void Initialize(State startingState)
    {
        currState = startingState;
        currState.Enter();
    }
    public void ChangeState(State newState)
    {
        currState.Exit();
        currState = newState;
        currState.Enter();
    }
}
