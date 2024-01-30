using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordSkillController : MonoBehaviour
{
    private Animator anim;
    private Rigidbody2D rb;
    private CircleCollider2D circleCollider;
    private Player player;

    private bool canRotate = true;
    private bool isReturning;
    [SerializeField] private float returnSpeed = 20f;

    [Header("Bouncing sword")]
    [SerializeField] private float bounceForce = 10f;
    private bool isBouncing;
    private int maxBounces;
    private List<Transform> enemyTargets;
    private int targetIndex = 0;

    private void Awake()
    {
        anim = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody2D>();
        circleCollider = GetComponent<CircleCollider2D>();
    }

    private void Update()
    {
        if (canRotate)
        {
            transform.right = rb.velocity;
        }

        if (isReturning)
        {
            transform.position = Vector2.MoveTowards(transform.position, player.transform.position, returnSpeed * Time.deltaTime);

            if (Vector2.Distance(transform.position, player.transform.position) < 0.2f)
            {
                player.CatchSword();
            }
        }
        SwordBounceLogic();
    }

    private void SwordBounceLogic()
    {
        if (isBouncing && enemyTargets.Count > 0)
        {
            transform.position = Vector2.MoveTowards(transform.position, enemyTargets[targetIndex].position, bounceForce * Time.deltaTime);

            if (Vector2.Distance(transform.position, enemyTargets[targetIndex].position) < 0.1f)
            {
                targetIndex++;
                maxBounces--;

                if (maxBounces <= 0)
                {
                    isBouncing = false;
                    isReturning = true;
                }

                if (targetIndex >= enemyTargets.Count)
                {
                    targetIndex = 0;
                }
            }
        }
    }

    public void SetupBouncingSword(bool _isBouncing, int _maxBounces)
    {
        isBouncing = _isBouncing;
        maxBounces = _maxBounces;

        enemyTargets = new List<Transform>();
    }

    public void ThrowSword(Vector2 _direction, float _gravity, Player _player)
    {   player = _player;
        rb.velocity = _direction;
        rb.gravityScale = _gravity;

        anim.SetBool("Spinning", true);
    }

    public void ReturnSword()
    {
        rb.constraints = RigidbodyConstraints2D.FreezeAll;
        // rb.isKinematic = false;
        transform.parent = null;
        isReturning = true;
    }

    private void OnTriggerEnter2D(Collider2D other) 
    {
        if (isReturning)
            return;

        if (other.GetComponent<Enemy>() != null)
        {
            // Find all enemies in range and add them to the list
            if (isBouncing && enemyTargets.Count <= 0)
            {
                Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 10);

                foreach (Collider2D collider in colliders)
                {
                    if (collider.GetComponent<Enemy>() != null)
                    {
                        enemyTargets.Add(collider.transform);
                    }
                }
            }
        }

        StuckInto(other);
    }

    private void StuckInto(Collider2D other)
    {

        canRotate = false;
        circleCollider.enabled = false;

        rb.isKinematic = true;
        rb.constraints = RigidbodyConstraints2D.FreezeAll;

        if (isBouncing && enemyTargets.Count > 0)
            return;

        anim.SetBool("Spinning", false);
        transform.parent = other.transform;
    }
}
