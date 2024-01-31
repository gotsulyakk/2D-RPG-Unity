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
    private float returnSpeed;

    private float freezeTimeDuration;

    [Header("Bouncing sword")]
    private float bounceForce;
    private bool isBouncing;
    private int maxBounces;
    private List<Transform> enemyTargets;
    private int targetIndex = 0;

    [Header("Piercing sword")]
    [SerializeField] private int maxPierces;

    [Header("Spinning sword")]
    private float maxTravelDistance;
    private float spinDuration;
    private float spinTimer;
    private bool wasStopped;
    private bool isSpinning;

    private float hitTimer;
    private float hitCooldown;
    // for better impact
    private float spinDirection;


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

        SpinningLogic();
    }

    private void DestroyMe()
    {
        Destroy(gameObject);
    }

    #region Bouncing sword
    private void SwordBounceLogic()
    {
        if (isBouncing && enemyTargets.Count > 0)
        {
            transform.position = Vector2.MoveTowards(transform.position, enemyTargets[targetIndex].position, bounceForce * Time.deltaTime);

            if (Vector2.Distance(transform.position, enemyTargets[targetIndex].position) < 0.1f)
            {
                SwordSkillDamage(enemyTargets[targetIndex].GetComponent<Enemy>());

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

    public void SetupBouncingSword(bool _isBouncing, int _maxBounces, float _bounceForce)
    {
        isBouncing = _isBouncing;
        maxBounces = _maxBounces;
        bounceForce = _bounceForce;

        enemyTargets = new List<Transform>();
    }

    private void SetupTargetsForBouncing(Collider2D other)
    {
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
    }
    #endregion

    #region Piercing sword
    public void SetupPiercingSword(int _maxPierces)
    {
        maxPierces = _maxPierces;
    }
    #endregion

    #region Spinning sword
    public void SetupSpinningSword(bool _isSpinning, float _maxTravelDistance, float _spinDuration, float _hitCooldown)
    {
        isSpinning = _isSpinning;
        maxTravelDistance = _maxTravelDistance;
        spinDuration = _spinDuration;
        hitCooldown = _hitCooldown;
    }

    private void SpinningLogic()
    {
        if (isSpinning)
        {
            if (Vector2.Distance(player.transform.position, transform.position) > maxTravelDistance && !wasStopped)
            {
                StopWhenSpinning();
            }

            if (wasStopped)
            {
                spinTimer -= Time.deltaTime;

                // slightly move the sword when spinning
                transform.position = Vector2.MoveTowards(transform.position, new Vector2(transform.position.x + spinDirection, transform.position.y), 1.5f * Time.deltaTime);

                if (spinTimer < 0)
                {
                    isReturning = true;
                    isSpinning = false;
                }

                hitTimer -= Time.deltaTime;

                if (hitTimer < 0)
                {
                    hitTimer = hitCooldown;

                    Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 1);

                    foreach (Collider2D collider in colliders)
                    {
                        if (collider.GetComponent<Enemy>() != null)
                        {
                            SwordSkillDamage(collider.GetComponent<Enemy>());
                        }
                    }
                }
            }
        }
    }

    private void StopWhenSpinning()
    {
        wasStopped = true;
        rb.constraints = RigidbodyConstraints2D.FreezePosition;
        spinTimer = spinDuration;
    }
    #endregion

    public void ThrowSword(Vector2 _direction, float _gravity, Player _player, float _freezeTimeDuration, float _returnSpeed)  // SetupSword in course
    {   player = _player;
        rb.velocity = _direction;
        rb.gravityScale = _gravity;
        freezeTimeDuration = _freezeTimeDuration;
        returnSpeed = _returnSpeed;

        if (maxPierces <= 0)
            anim.SetBool("Spinning", true);

        spinDirection = Mathf.Clamp(rb.velocity.x, -1, 1);

        Invoke("DestroyMe", 7f);
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
            Enemy enemy = other.GetComponent<Enemy>();

            SwordSkillDamage(enemy);
        }

        other.GetComponent<Enemy>()?.TakeDamage();

        SetupTargetsForBouncing(other);

        StuckInto(other);
    }

    private void SwordSkillDamage(Enemy enemy)
    {
        enemy.TakeDamage();
        enemy.StartCoroutine("FreezeTimeFor", freezeTimeDuration);
    }

    private void StuckInto(Collider2D other)
    {
        if (isSpinning)
        {
            StopWhenSpinning();
            return;
        }

        if (maxPierces > 0 && other.GetComponent<Enemy>() != null)
        {
            maxPierces--;
            return;
        }

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
