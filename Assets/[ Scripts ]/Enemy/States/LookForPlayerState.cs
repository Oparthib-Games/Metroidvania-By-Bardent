using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookForPlayerState : State
{
    protected D_LookForPlayer stateData;

    protected bool turnImmediately;
    protected bool isPlayerInMinAgroRange;
    protected bool isAllTurnsDone;
    protected bool isAllTurnsTimeDone;

    protected float lastTimeTurn;
    protected int amountOfTurnsDone;

    public LookForPlayerState(FiniteStateMachine stateMachine, Entity entity, string animBoolName, D_LookForPlayer stateData) : base(stateMachine, entity, animBoolName)
    {
        this.stateData = stateData;
    }

    public override void DoChecks()
    {
        base.DoChecks();
        isPlayerInMinAgroRange = entity.CheckPlayerInMinAgroRange();
    }

    public override void Enter()
    {
        base.Enter();

        isAllTurnsDone = false;
        isAllTurnsTimeDone = false;

        lastTimeTurn = startTime;
        amountOfTurnsDone = 0;

        entity.SetVelocity(0);
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if(turnImmediately)
        {
            entity.Flip();
            lastTimeTurn = Time.time;
            amountOfTurnsDone++;
            turnImmediately = false;
        }
        else if(Time.time >= lastTimeTurn + stateData.timeBetweenTurns && !isAllTurnsDone) 
        {
            entity.Flip();
            lastTimeTurn = Time.time;
            amountOfTurnsDone++;
        }

        if(amountOfTurnsDone >= stateData.amountsOfTurn)
        {
            isAllTurnsDone = true;
        }

        if(Time.time >= lastTimeTurn + stateData.timeBetweenTurns)
        {
            isAllTurnsTimeDone = true;
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }

    public void SetTurnImmediately(bool doFlip)
    {
        turnImmediately = doFlip;
    }
}
