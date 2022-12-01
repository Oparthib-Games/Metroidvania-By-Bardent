using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCtrl : MonoBehaviour
{
    private float movementInputDirection;

    private int amountOfJumpsLeft;
    private int facingDirection = 1;
    private int lastWallJumpedDirection;

    private bool isFacingRight = true;
    private bool isWalking;
    private bool isGrounded;
    private bool canNormalJump;
    private bool canWallJump;
    private bool isTouchingWall;
    private bool isWallSliding;
    private bool isAttemptingToJump;
    private bool checkJumpMultiplier;
    private bool canMove = true;
    private bool canFlip = true;
    private bool hasWallJumped;
    private bool isTouchingLedge;
    private bool canClimbLedge = false;
    private bool ledgeDetected;
    private bool isDashing;

    private float jumpTimer;
    private float turnTimer;
    private float wallJumpTimer;
    private float dashTimeLeft;
    private float lastImageXpos;
    private float lastDash = -100f;

    private Vector2 ledgePosBottom;
    private Vector2 ledgePos1;
    private Vector2 ledgePos2;

    private Rigidbody2D RB;
    private Animator ANIM;

    public float movementSpeed = 10.0f;
    public float jumpForce = 16.0f;
    public float groundCheckRadius;
    public int amountOfJumps = 1;
    public float wallCheckDistance;
    public float wallSlideSpeed;
    public float movementForceInAir;
    public float airDragMultiplier = 0.95f;
    public float variableJumpHeightMultiplier = 0.5f;
    public float wallHopForce;
    public float wallJumpForce;
    public float jumpTimerSet = 0.15f;
    public float turnTimerSet = 0.1f;
    public float wallJumpTimerSet = 0.5f;
    public float dashTime;
    public float dashSpeed;
    public float distanceBetweenImages;
    public float dashCoolDown;

    public float ledgeClimbXOffset1 = 0f;
    public float ledgeClimbYOffset1 = 0f;    
    public float ledgeClimbXOffset2 = 0f;
    public float ledgeClimbYOffset2 = 0f;

    public Vector2 wallHopDirection;
    public Vector2 wallJumpDirection;


    public Transform groundCheck;
    public Transform wallCheck;
    public Transform ledgeCheck;

    public LayerMask groundLayerMask;

    void Start()
    {
        RB = GetComponent<Rigidbody2D>();
        ANIM = GetComponent<Animator>();
        amountOfJumpsLeft = amountOfJumps;
        wallHopDirection.Normalize();
        wallJumpDirection.Normalize();
    }

    void Update()
    {
        CheckInput();
        CheckMovementDirection();
        UpdateAnimations();
        CheckIfCanJump();
        CheckIfWallSliding();
        CheckJump();
        checkLedgeClimb();
        CheckDash();
    }
    private void FixedUpdate()
    {
        ApplyMovement();
        CheckSurroundings();
    }

    private void CheckSurroundings()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayerMask);

        isTouchingWall = Physics2D.Raycast(wallCheck.position, transform.right, wallCheckDistance, groundLayerMask);

        isTouchingLedge = Physics2D.Raycast(ledgeCheck.position, transform.right, wallCheckDistance, groundLayerMask);

        if(isTouchingWall && !isTouchingLedge && !ledgeDetected)
        {
            ledgeDetected = true;
            ledgePosBottom = wallCheck.position;
        }
    }

    private void CheckIfCanJump()
    {
        if (isGrounded && RB.velocity.y <= 0.01f)
        {
            amountOfJumpsLeft = amountOfJumps;
        }

        if(isTouchingWall)
        {
            canWallJump = true;
        }

        if (amountOfJumpsLeft <= 0)
        {
            canNormalJump = false;
        }
        else
        {
            canNormalJump = true;
        }
    }

    private void CheckInput()
    {
        movementInputDirection = Input.GetAxisRaw("Horizontal");

        if (Input.GetButtonDown("Jump"))
        {
            if(isGrounded || (amountOfJumps > 0 && !isTouchingWall))
            {
                NormalJump();
            }
            else
            {
                jumpTimer = jumpTimerSet;
                isAttemptingToJump = true;
            }
        }

        if(Input.GetButtonDown("Horizontal") && isTouchingWall)
        {
            if (!isGrounded && movementInputDirection != facingDirection)
            {
                canMove = false;
                canFlip = false;
                turnTimer = turnTimerSet;
            }
        }

        if(turnTimer >= 0)
        {
            turnTimer -= Time.deltaTime;

            if(turnTimer <= 0)
            {
                canMove = true;
                canFlip = true;
            }
        }

        if (checkJumpMultiplier && !Input.GetButton("Jump"))
        {
            checkJumpMultiplier = false;
            RB.velocity = new Vector2(RB.velocity.x, RB.velocity.y * variableJumpHeightMultiplier);
        }

        if(Input.GetButtonDown("Dash"))
        {
            if(Time.time >= (lastDash + dashCoolDown))
            AttemptToDash();
        }
    }

    private void AttemptToDash()
    {
        isDashing = true;
        dashTimeLeft = dashTime;
        lastDash = Time.time;

        PlayerAfterImagePool.Instance.GetFromPool();
        lastImageXpos = transform.position.x;
    }

    private void CheckDash()
    {
        if (isDashing)
        {
            if(dashTimeLeft > 0)
            {
                canMove = false;
                canFlip = false;
                RB.velocity = new Vector2(dashSpeed * facingDirection, RB.velocity.y);
                dashTimeLeft -= Time.deltaTime;

                if (Mathf.Abs(transform.position.x - lastImageXpos) > distanceBetweenImages)
                {
                    PlayerAfterImagePool.Instance.GetFromPool();
                    lastImageXpos = transform.position.x;
                }
            }

            if(dashTimeLeft <= 0 || isTouchingWall)
            {
                isDashing = false;
                canMove = true;
                canFlip = true;
            }
        }
    }

    private void ApplyMovement()
    {
        // ------------------> To handle movement in air
        //if (!isGrounded && !isWallSliding  && movementInputDirection != 0)
        //{
        //    Vector2 forceToAdd = new Vector2(movementForceInAir * movementInputDirection, 0);
        //    RB.AddForce(forceToAdd);

        //    if(Mathf.Abs(RB.velocity.x) > movementSpeed)
        //    {
        //        RB.velocity = new Vector2(movementSpeed * movementInputDirection, RB.velocity.y);
        //    }
        //}
        
        if (!isGrounded && !isWallSliding && movementInputDirection == 0)
        {
            RB.velocity = new Vector2(RB.velocity.x * airDragMultiplier, RB.velocity.y);
        }
        else if(canMove)
        {
            RB.velocity = new Vector2(movementSpeed * movementInputDirection, RB.velocity.y);
        }

        if(isWallSliding)
        {
            if(RB.velocity.y < -wallSlideSpeed)
            {
                RB.velocity = new Vector2(RB.velocity.x, wallSlideSpeed);
            }
        }
    }


    private void CheckJump()
    {
        if(jumpTimer > 0)
        {
            if(!isGrounded && isTouchingWall && movementInputDirection != 0 && movementInputDirection != facingDirection)
            {
                WallJump();
            }
            else if(isGrounded)
            {
                NormalJump();
            }
        }

        if(isAttemptingToJump)
        {
            jumpTimer -= Time.deltaTime;
        }

        if(wallJumpTimer > 0)
        {
            if(hasWallJumped && movementInputDirection == -lastWallJumpedDirection)
            {
                RB.velocity = new Vector2(RB.velocity.x, 0);
                hasWallJumped = false;
            }
            else if (wallJumpTimer <= 0)
            {
                hasWallJumped = false;
            }
            else
            {
                wallJumpTimer -= Time.deltaTime;
            }
        }
    }

    private void NormalJump()
    {
        if (canNormalJump)
        {
            RB.velocity = new Vector2(RB.velocity.x, jumpForce);
            amountOfJumpsLeft--;
            jumpTimer = 0;
            isAttemptingToJump = false;
            checkJumpMultiplier = true;
        }
    }

    private void WallJump()
    {
        if (canWallJump) 
        {
            RB.velocity = new Vector2(RB.velocity.x, 0);
            isWallSliding = false;
            amountOfJumpsLeft = amountOfJumps;
            amountOfJumpsLeft--;
            Vector2 forceToAdd = new Vector2(wallJumpForce * wallJumpDirection.x * movementInputDirection, wallJumpForce * wallJumpDirection.y);
            RB.AddForce(forceToAdd, ForceMode2D.Impulse);
            jumpTimer = 0;
            isAttemptingToJump = false;
            checkJumpMultiplier = true;
            turnTimer = 0;
            canMove = true;
            canFlip = true;
            hasWallJumped = true;
            wallJumpTimer = wallJumpTimerSet;
            lastWallJumpedDirection = -facingDirection;
        }
    }

    private void checkLedgeClimb()
    {
        if(ledgeDetected && !canClimbLedge)
        {
            canClimbLedge = true;

            ledgePos1 = new Vector2(Mathf.Floor(ledgePosBottom.x + wallCheckDistance) - (ledgeClimbXOffset1 * facingDirection), Mathf.Floor(ledgePosBottom.y) + ledgeClimbYOffset1);
            ledgePos2 = new Vector2(Mathf.Floor(ledgePosBottom.x + wallCheckDistance) + (ledgeClimbXOffset2 * facingDirection), Mathf.Floor(ledgePosBottom.y) + ledgeClimbYOffset2);

            canMove = false;
            canFlip = false;

            ANIM.SetBool("canClimbLedge", canClimbLedge);
        }

        if (canClimbLedge)
        {
            transform.position = ledgePos1;
        }
    }

    public void FinisheLedgeClimb_AnimEvent()
    {
        canClimbLedge = false;
        transform.position = ledgePos2;
        canMove = true;
        canFlip = true;
        ledgeDetected = false;
        ANIM.SetBool("canClimbLedge", canClimbLedge);
    }

    private void CheckMovementDirection()
    {
        if (isFacingRight && movementInputDirection < 0)
        {
            Flip();
        }
        else if (!isFacingRight && movementInputDirection > 0)
        {
            Flip();
        }

        if (Mathf.Abs(RB.velocity.x) >= 0.01f)
        {
            isWalking = true;
        }
        else
        {
            isWalking = false;
        }
    }

    private void CheckIfWallSliding()
    {
        if (isTouchingWall && movementInputDirection == facingDirection && RB.velocity.y < 0 && !canClimbLedge)
        {
            isWallSliding = true;
        }
        else
        {
            isWallSliding = false;
        }
    }

    public int GetFacingDirection()
    {
        return facingDirection;
    }
    public void DisableFlip_AnimEvent()
    {
        canFlip = false;
    }
    public void EnableFlip_AnimEvent()
    {
        canFlip = true;
    }
    private void Flip()
    {
        if(!isWallSliding && canFlip)
        {
            facingDirection *= -1;
            isFacingRight = !isFacingRight;
            transform.Rotate(0.0f, 180.0f, 0.0f);
        }
    }

    private void UpdateAnimations()
    {
        ANIM.SetBool("isWalking", isWalking);
        ANIM.SetBool("isGrounded", isGrounded);
        ANIM.SetFloat("yVelocity", RB.velocity.y);
        ANIM.SetBool("isWallSliding", isWallSliding);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);

        Gizmos.DrawLine(wallCheck.position, new Vector3(wallCheck.position.x + wallCheckDistance, wallCheck.position.y, wallCheck.position.z));
    }
}
