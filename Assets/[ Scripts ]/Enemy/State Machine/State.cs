using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class State
{
    protected FiniteStateMachine stateMachine;
    protected Entity entity;

    protected float startTime;

    protected string animBoolName;

    public State(FiniteStateMachine stateMachine, Entity entity, string animBoolName)
    {
        this.stateMachine = stateMachine;
        this.entity = entity;
        this.animBoolName = animBoolName;
    }
    
    public virtual void Enter()
    {
        startTime = Time.time;
        entity.ANIM.SetBool(animBoolName, true);

        Debug.Log($"Entering State => {animBoolName}");
    }
    public virtual void LogicUpdate()
    {

    }
    public virtual void PhysicsUpdate()
    {

    }
    public virtual void Exit()
    {
        entity.ANIM.SetBool(animBoolName, false);
    }
}
