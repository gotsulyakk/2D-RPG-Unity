using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkeletonBattleState : EnemyState
{   
    private EnemySkeleton enemySkeleton;
    private Transform player;
    private int moveDirection;

    public SkeletonBattleState(Enemy _enemyBase, EnemyStateMachine _stateMachine, string _animBoolName, EnemySkeleton _enemySkeleton) : base(_enemyBase, _stateMachine, _animBoolName)
    {
        this.enemySkeleton = _enemySkeleton;
    }

    public override void Enter()
    {
        base.Enter();

        player = GameObject.Find("Player").transform;
    }

    public override void Update()
    {
        base.Update();

        if (enemySkeleton.IsPlayerDetected())
        {
            if (enemySkeleton.IsPlayerDetected().distance < enemySkeleton.attackDistance)
            {
                // stateMachine.ChangeState(enemySkeleton.attackState);
                Debug.Log("Attack");
                enemySkeleton.ZeroVelocity();
                return;
            }
        }

        if (player.position.x > enemySkeleton.transform.position.x)
        {
            moveDirection = 1;
        }
        else
        {
            moveDirection = -1;
        }

        enemySkeleton.SetVelocity(enemySkeleton.moveSpeed * moveDirection, rb.velocity.y);
    }

    public override void Exit()
    {
        base.Exit();
    }
}
