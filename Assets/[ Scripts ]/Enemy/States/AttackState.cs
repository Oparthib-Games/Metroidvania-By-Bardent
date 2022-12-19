using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackState : State
{
    protected Transform attackPos;

    protected bool isAnimationFinished;
    protected bool isPlayerInMinAgroRange;
    
    public AttackState(FiniteStateMachine stateMachine, Entity entity, string animBoolName, Transform attackPos) : base(stateMachine, entity, animBoolName)
    {
        this.attackPos = attackPos;
    }

    public override void DoChecks()
    {
        base.DoChecks();

        isPlayerInMinAgroRange = entity.CheckPlayerInMinAgroRange();
    }

    public override void Enter()
    {
        base.Enter();

        entity.ATSM.attackState = this;
        isAnimationFinished = false;
        entity.SetVelocity(0);
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }

    public virtual void TriggerAttack()
    {

    }
    public virtual void FinishAttack()
    {
        isAnimationFinished = true;
    }
}
