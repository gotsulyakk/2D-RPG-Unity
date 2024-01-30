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

        anim.SetBool("Spinning", false);

        canRotate = false;
        circleCollider.enabled = false;

        rb.isKinematic = true;
        rb.constraints = RigidbodyConstraints2D.FreezeAll;

        transform.parent = other.transform;
    }
}
