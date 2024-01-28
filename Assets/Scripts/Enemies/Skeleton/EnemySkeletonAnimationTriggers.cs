using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySkeletonAnimationTriggers : MonoBehaviour
{
    private EnemySkeleton enemySkeleton => GetComponentInParent<EnemySkeleton>();

    private void AnimationTrigger()
    {
        enemySkeleton.AnimationFinishTrigger();
    }

    private void AttackTrigger()
    {
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(enemySkeleton.attackCheck.position, enemySkeleton.attackRadius);

        foreach (Collider2D enemy in hitEnemies)
        {
            if (enemy.GetComponent<Player>() != null)
            {
                enemy.GetComponent<Player>().TakeDamage();
            }
        }
    }

    private void OpenCounterAttackWindow()
    {
        enemySkeleton.OpenCounterAttackWindow();
    }

    private void CloseCounterAttackWindow()
    {
        enemySkeleton.CloseCounterAttackWindow();
    }
}
