using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StunState : State
{
    protected D_StunState stateData;

    protected bool isStunTimeOver;
    protected bool isGrounded;
    protected bool isMovemetnStopped;

    public StunState(FiniteStateMachine stateMachine, Entity entity, string animBoolName, D_StunState stateData) : base(stateMachine, entity, animBoolName)
    {
        this.stateData = stateData;
    }

    public override void DoChecks()
    {
        base.DoChecks();

        isGrounded = !entity.CheckLedge();
    }

    public override void Enter()
    {
        base.Enter();

        isStunTimeOver = false;
        isMovemetnStopped = false;

        entity.SetVelocity(stateData.stunKnockbackSpeed, stateData.stunKnockbackAngle, entity.lastDamageDirection);
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if(Time.time >= startTime + stateData.stunTime)
        {
            isStunTimeOver = true;
        }

        if(isGrounded && Time.time >= startTime + stateData.stunKnockbackTime && !isMovemetnStopped)
        {
            isMovemetnStopped = true;
            entity.SetVelocity(0f);
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}
