using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour
{
    public FiniteStateMachine stateMachine;

    public D_Entity entityData;

    public int facingDirection { get; private set; } = 1;
    public Rigidbody2D RB { get; private set; }
    public Animator ANIM { get; private set; }
    public GameObject aliveGO { get; private set; }
    public AnimationToStateMachine ATSM { get; private set; }

    [SerializeField]
    private Transform wallCheck;
    [SerializeField]
    private Transform ledgeCheck;

    private Vector2 velocityWorkspace;

    private float currHealth;

    public int lastDamageDirection { get; private set; }

    public virtual void Start()
    {
        currHealth = entityData.maxHealth;

        aliveGO = transform.Find("Alive").gameObject;
        RB = aliveGO.GetComponent<Rigidbody2D>();
        ANIM = aliveGO.GetComponent<Animator>();
        ATSM= aliveGO.GetComponent<AnimationToStateMachine>();

        stateMachine = new FiniteStateMachine();
    }

    public virtual void Update()
    {
        stateMachine.currState.LogicUpdate();
    }
    public virtual void FixedUpdate()
    {
        stateMachine.currState.PhysicsUpdate();
    }

    public virtual void SetVelocity(float velocity)
    {
        velocityWorkspace.Set(facingDirection * velocity, RB.velocity.y);
        RB.velocity = velocityWorkspace;
    }
    public virtual void SetVelocity(float velocity, Vector2 angle, int direction)
    {
        angle.Normalize();
        velocityWorkspace.Set(angle.x * direction * velocity, angle.y * velocity);
        RB.velocity = velocityWorkspace;
    }

    public virtual bool CheckWall()
    {
        return Physics2D.Raycast(wallCheck.position, aliveGO.transform.right, entityData.wallCheckDistance, entityData.goundLayer);
    }
    public virtual bool CheckLedge()
    {
        return Physics2D.Raycast(ledgeCheck.position, Vector2.down, entityData.ledgeCheckDistance, entityData.goundLayer);
    }
    public virtual bool CheckPlayerInMinAgroRange()
    {
        return Physics2D.Raycast(wallCheck.position, wallCheck.right, entityData.closeRangeActionDistance, entityData.playerLayer);
    }
    public virtual bool CheckPlayerInMaxAgroRange()
    {
        return Physics2D.Raycast(wallCheck.position, wallCheck.right, entityData.maxAgroDist, entityData.playerLayer);
    }
    public virtual bool CheckPlayerInCloseRangeAction()
    {
        return Physics2D.Raycast(wallCheck.position, wallCheck.right, entityData.closeRangeActionDistance, entityData.playerLayer);
    }


    public virtual void DamageHop(float velocity)
    {
        velocityWorkspace.Set(RB.velocity.x, velocity);
        RB.velocity = velocityWorkspace;
    }

    public virtual void Damage(AttackDetails attackDetails)
    {
        currHealth -= attackDetails.damageAmount;

        DamageHop(entityData.damageHopSpeed);

        if(attackDetails.position.x > aliveGO.transform.position.x)
        {
            lastDamageDirection = -1;
        }
        else
        {
            lastDamageDirection = 1;
        }
    }


    public virtual void Flip()
    {
        facingDirection *= -1;
        aliveGO.transform.Rotate(0f, 180f, 0f);
    }


    public virtual void OnDrawGizmos()
    {
        Gizmos.DrawLine(wallCheck.position, wallCheck.position + (Vector3)(Vector2.right * facingDirection * entityData.wallCheckDistance));
        Gizmos.DrawLine(ledgeCheck.position, ledgeCheck.position + (Vector3)(Vector2.down * entityData.ledgeCheckDistance));
        Gizmos.DrawLine(wallCheck.position - new Vector3(0, 0.1f, 0), wallCheck.position - new Vector3(0, 0.1f, 0) + (Vector3)(Vector2.right * facingDirection * entityData.closeRangeActionDistance));
        Gizmos.DrawLine(wallCheck.position - new Vector3(0, 0.2f, 0), wallCheck.position - new Vector3(0, 0.2f, 0) + (Vector3)(Vector2.right * facingDirection * entityData.minAgroDist));
        Gizmos.DrawLine(wallCheck.position - new Vector3(0, 0.3f, 0), wallCheck.position - new Vector3(0, 0.3f, 0) + (Vector3)(Vector2.right * facingDirection * entityData.maxAgroDist));
    }
}
