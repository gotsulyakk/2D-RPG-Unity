using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkeletonStunnedState : EnemyState
{
    private EnemySkeleton enemySkeleton;

    public SkeletonStunnedState(Enemy _enemy, EnemyStateMachine _stateMachine, string _animBoolName, EnemySkeleton _enemySkeleton) : base(_enemy, _stateMachine, _animBoolName)
    {
        this.enemySkeleton = _enemySkeleton;
    }

    public override void Enter()
    {
        base.Enter();

        enemySkeleton.fx.InvokeRepeating("RedColorBlink", 0, .1f);

        stateTimer = enemySkeleton.stunDuration;
        rb.velocity = new Vector2(-enemySkeleton.facingDirection * enemySkeleton.stunKnockbackDirection.x, enemySkeleton.stunKnockbackDirection.y);
    }

    public override void Update()
    {
        base.Update();

        if (stateTimer < 0)
        {
            stateMachine.ChangeState(enemySkeleton.idleState);
        }
    }

    public override void Exit()
    {
        base.Exit();

        enemySkeleton.fx.Invoke("CancelRedColorBlink", 0);
    }
}
