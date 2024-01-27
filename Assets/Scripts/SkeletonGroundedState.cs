using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkeletonGroundedState : EnemyState
{
    protected EnemySkeleton enemySkeleton;
    protected Transform player;

    public SkeletonGroundedState(Enemy _enemyBase, EnemyStateMachine _stateMachine, string _animBoolName, EnemySkeleton _enemySkeleton) : base(_enemyBase, _stateMachine, _animBoolName)
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

        if (enemySkeleton.IsPlayerDetected() || Vector2.Distance(enemySkeleton.transform.position, player.position) < 2)  // If the player is detected or in range (e.g. behind), go to battle state
        {
            stateMachine.ChangeState(enemySkeleton.battleState);
        }
    }

    public override void Exit()
    {
        base.Exit();
    }
}
