using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatDummy : MonoBehaviour
{
    [SerializeField]
    private float maxHealth, kncockbackSpeedX, kncockbackSpeedY, knockbackDuration, kncockbackDeathSpeedX, kncockbackDeathSpeedY, deathTorque;
    private float currHealth;
    [SerializeField]
    private bool applyKnockback;
    private bool isKnockback;
    private float knockbackStart;
    [SerializeField]
    private GameObject hitParticle;

    private PlayerCtrl playerCtrl;

    private GameObject aliveGO, brokenTopGO, brokenBottomGO;
    private Rigidbody2D aliveRB, brokenTopRB, brokenBottomRB;
    private Animator aliveAnim;

    private int playerFacingDirection;
    private bool playerOnLeft;

    private void Start()
    {
        currHealth = maxHealth;
        playerCtrl = GameObject.Find("Player").GetComponent<PlayerCtrl>();

        aliveGO = transform.Find("Alive").gameObject;
        brokenTopGO = transform.Find("Broken Top").gameObject;
        brokenBottomGO = transform.Find("Broken Bottom").gameObject;

        aliveAnim = aliveGO.GetComponent<Animator>();

        aliveRB = aliveGO.GetComponent<Rigidbody2D>();
        brokenTopRB = brokenTopGO.GetComponent<Rigidbody2D>();
        brokenBottomRB = brokenBottomGO.GetComponent<Rigidbody2D>();

        aliveGO.SetActive(true);
        brokenTopGO.SetActive(false);
        brokenBottomGO.SetActive(false);
    }


    private void Update()
    {
        checkKnokback();
    }

    private void Damage(float[] attackDetails)
    {
        print("Damage");
        currHealth -= attackDetails[0];
        playerFacingDirection = playerCtrl.GetFacingDirection();

        Instantiate(hitParticle, aliveGO.transform.position, Quaternion.Euler(0, 0, Random.Range(0, 360f)));

        if(playerFacingDirection == 1)
        {
            playerOnLeft = true;
        } else
        {
            playerOnLeft = false;
        }

        aliveAnim.SetBool("player_on_left", playerOnLeft);
        aliveAnim.SetTrigger("damage");

        if(applyKnockback && currHealth > 0.0f)
        {
            Knockback();
        }
        else
        {
            Die();
        }
    }

    private void Knockback()
    {
        isKnockback = true;
        knockbackStart = Time.time;
        aliveRB.velocity = new Vector2(kncockbackSpeedX * playerFacingDirection, kncockbackSpeedY);
    }

    private void checkKnokback()
    {
        if(Time.time >= knockbackStart + knockbackDuration && isKnockback)
        {
            isKnockback = false;
            aliveRB.velocity = new Vector2(0, aliveRB.velocity.y);
        }
    }

    private void Die()
    {
        aliveGO.SetActive(false);
        brokenTopGO.SetActive(true);
        brokenBottomGO.SetActive(true);

        brokenTopGO.transform.position = aliveGO.transform.position;
        brokenBottomGO.transform.position = aliveGO.transform.position;

        brokenBottomRB.velocity = new Vector2(kncockbackSpeedX * playerFacingDirection, kncockbackDeathSpeedY);
        brokenTopRB.velocity = new Vector2(kncockbackDeathSpeedX * playerFacingDirection, kncockbackDeathSpeedY);
        brokenTopRB.AddTorque(deathTorque * -playerFacingDirection, ForceMode2D.Impulse);

    }
}
