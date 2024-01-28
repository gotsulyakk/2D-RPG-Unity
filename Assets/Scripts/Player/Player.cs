using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Entity
{   
    [Header("Attack details")]
    public Vector2[] attackMovementSpeeds;
    public bool isBusy { get; private set; }
    public float counterAttackDuration = 0.2f;

    [Header("Move Info")]
    public float moveSpeed = 8f;
    public float jumpForce = 10f;

    [Header("Dash Info")]
    [SerializeField] private float dashCooldown = 1f;
    private float dashCooldownTimer;
    public float dashForce = 20f;
    public float dashTime = 0.2f;
    public float dashDirection { get; private set;}

    [Header("Wall Info")]
    public float wallSlideSpeed = 3f;
    public float wallJumpForce = 10f;
    public float wallJumpTime = 1f;

    # region States
    public PlayerStateMachine stateMachine { get; private set; }
    public PlayerIdleState idleState { get; private set; }
    public PlayerMoveState moveState { get; private set; }
    public PlayerJumpState jumpState { get; private set; }
    public PlayerAirState airState { get; private set; }
    public PlayerWallSlideState wallSlideState { get; private set; }
    public PlayerWallJumpState wallJumpState { get; private set; }
    public PlayerDashState dashState { get; private set; }
    public PlayerCounterAttackState counterAttackState { get; private set; }

    public PlayerPrimaryAttackState primaryAttackState { get; private set; }
    # endregion

    protected override void Awake()
    {
        base.Awake();

        stateMachine = new PlayerStateMachine();

        idleState = new PlayerIdleState(this, stateMachine, "Idle");
        moveState = new PlayerMoveState(this, stateMachine, "Move");
        jumpState = new PlayerJumpState(this, stateMachine, "Jump");
        airState = new PlayerAirState(this, stateMachine, "Jump");
        wallSlideState = new PlayerWallSlideState(this, stateMachine, "WallSlide");
        wallJumpState = new PlayerWallJumpState(this, stateMachine, "Jump");
        dashState = new PlayerDashState(this, stateMachine, "Dash");

        primaryAttackState = new PlayerPrimaryAttackState(this, stateMachine, "Attack");
        counterAttackState = new PlayerCounterAttackState(this, stateMachine, "CounterAttack");
    }

    protected override void Start()
    {
        base.Start();

        stateMachine.Initialize(idleState);
    }

    protected override void Update()
    {
        base.Update();

        stateMachine.currentState.Update();

        CheckForDashInpupt();
    }

    public IEnumerator BusyFor(float _seconds)
    {
        isBusy = true;
        yield return new WaitForSeconds(_seconds);
        isBusy = false;
    }

    public void AnimationTrigger()  // We'll call this from the Animator
    {
        stateMachine.currentState.AnimationFinishTrigger();
    }

    public void CheckForDashInpupt()
    {
        if (IsWallDetected())
        {
            return;
        }

        dashCooldownTimer -= Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.LeftShift) && dashCooldownTimer < 0)
        {
            dashCooldownTimer = dashCooldown;
            dashDirection = Input.GetAxisRaw("Horizontal");

            if (dashDirection == 0)
            {
                dashDirection = facingDirection;
            }

            stateMachine.ChangeState(dashState);
        }
    }
}
