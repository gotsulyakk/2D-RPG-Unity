using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordSkillController : MonoBehaviour
{
    private Animator anim;
    private Rigidbody2D rb;
    private CircleCollider2D circleCollider;
    private Player player;

    private void Awake()
    {
        anim = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody2D>();
        circleCollider = GetComponent<CircleCollider2D>();
    }

    public void ThrowSword(Vector2 _direction, float _gravity)
    {
        rb.velocity = _direction;
        rb.gravityScale = _gravity;
    }
}
