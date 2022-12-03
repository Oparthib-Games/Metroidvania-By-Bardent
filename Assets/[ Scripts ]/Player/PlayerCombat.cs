using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    [SerializeField]
    private Transform attack1HitBoxPos;
    [SerializeField]
    private LayerMask damageableLayer;
    [SerializeField]
    private float attack1Radius, attack1Damage;
    [SerializeField]
    private float inputTimer;
    [SerializeField]
    private bool combatEnabled;
    private bool gotInput;
    private bool isAttacking;
    private bool isFirstAttack;

    private float lastInputTime = Mathf.NegativeInfinity;

    private float[] attackDetails = new float[2];

    private Animator ANIM;

    private void Start()
    {
        ANIM = GetComponent<Animator>();
        ANIM.SetBool("canAttack", combatEnabled);
    }

    private void Update()
    {
        CheckCombatInput();
        CheckAttack();
    }

    private void CheckCombatInput()
    {
        if(Input.GetMouseButtonDown(0))
        {
            if(combatEnabled)
            {
                gotInput = true;
                lastInputTime = Time.time;
            }
        }
    }

    private void CheckAttack()
    {
        if (gotInput)
        {
            if(!isAttacking)
            {
                gotInput = false;
                isAttacking = true;
                isFirstAttack = !isFirstAttack;
                ANIM.SetBool("attack1", true);
                ANIM.SetBool("firstAttack", isFirstAttack);
                ANIM.SetBool("isAttacking", isAttacking);
            }
        }

        if(Time.time >= lastInputTime + inputTimer)
        {
            gotInput = false;
        }
    }

    public void Damage(float[] attackDetails)
    {
        if (this.GetComponent<PlayerCtrl>().GetDashStatus()) return;

        int damageDirection;

        this.GetComponent<PlayerStats>().DecreaseHealth(attackDetails[0]);

        if (attackDetails[1] > transform.position.x)
        {
            damageDirection = -1;
        }
        else
        {
            damageDirection = 1;
        }

        this.GetComponent<PlayerCtrl>().Knockback(damageDirection);
    }

    public void CheckAttack1HitBox_AnimEvent()
    {
        Collider2D[] detectedObjects = Physics2D.OverlapCircleAll(attack1HitBoxPos.position, attack1Radius, damageableLayer);

        attackDetails[0] = attack1Damage;
        attackDetails[1] = transform.position.x;

        foreach (Collider2D collider in detectedObjects)
        {
            collider.transform.parent.SendMessage("Damage", attackDetails);
        }
    }

    public void FinishAttack_AnimEvent()
    {
        isAttacking = false;
        ANIM.SetBool("isAttacking", isAttacking);
        ANIM.SetBool("attack1", false);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(attack1HitBoxPos.position, attack1Radius);
    }
}
