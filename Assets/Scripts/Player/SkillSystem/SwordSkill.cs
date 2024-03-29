using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO: In Bouncing sword first attack is with different force than the rest of the bounces
public enum SwordType
{
    Normal,
    Bouncing,
    Piercing,
    Spinning
}

public class SwordSkill : Skill
{
    public SwordType swordType = SwordType.Normal;

    [Header("Bouncing sword info")]
    [SerializeField] private float bounceGravity;
    [SerializeField] private int maxBounces;
    [SerializeField] private float bounceForce;

    [Header("Piercing sword info")]
    [SerializeField] private float piercingGravity;
    [SerializeField] private int maxPierces;  // consider better name pierceAmount, because it's not only the max amount of pierces, but the amount of pierces left

    [Header("Spinning sword info")]
    [SerializeField] private float hitCooldown = 0.35f;
    [SerializeField] private float maxTravelDistance = 7f;
    [SerializeField] private float spinDuration = 2f;
    [SerializeField] private float spinGravity = 1f;

    [Header("Skill info")]
    [SerializeField] private GameObject swordPrefab;
    [SerializeField] private Vector2 throwingForce;
    [SerializeField] private float swordGravity;
    [SerializeField] private float freezeTimeDuration;
    [SerializeField] private float returnSpeed = 12f;

    private Vector2 finalDirection;

    [Header("Aim dots")]
    [SerializeField] private GameObject aimDotPrefab;
    [SerializeField] private Transform aimDotParent;
    [SerializeField] private int numberOfDots;
    [SerializeField] private float dotSpacing;

    private GameObject[] aimDots;

    protected override void Start()
    {
        base.Start();

        GenerateAimDots();

        SetupGravity();
    }

    protected override void Update()
    {
        if (Input.GetKeyUp(KeyCode.Mouse1))
        {
            finalDirection = new Vector2(AimDirection().normalized.x * throwingForce.x, AimDirection().normalized.y * throwingForce.y);
        }

        if (Input.GetKey(KeyCode.Mouse1))
        {
            for (int i = 0; i < aimDots.Length; i++)
            {
                aimDots[i].transform.position = AimDotsPosition(i * dotSpacing);
            }
        }
    }

    public void SpawnSword()
    {
        GameObject newSword = Instantiate(swordPrefab, player.transform.position, transform.rotation);
        SwordSkillController swordController = newSword.GetComponent<SwordSkillController>();

        if (swordType == SwordType.Bouncing)
        {
            swordController.SetupBouncingSword(true, maxBounces, bounceForce);  // check if works as intended
        }
        else if (swordType == SwordType.Piercing)
        {
            swordController.SetupPiercingSword(maxPierces);
        }
        else if (swordType == SwordType.Spinning)
        {
            swordController.SetupSpinningSword(true, maxTravelDistance, spinDuration, hitCooldown);
        }

        swordController.ThrowSword(finalDirection, swordGravity, player, freezeTimeDuration, returnSpeed);

        player.AssignNewSword(newSword);

        AimDotsActive(false);
    }

    private void SetupGravity()
    {
        if (swordType == SwordType.Bouncing)
        {
            swordGravity = bounceGravity;
        }
        else if (swordType == SwordType.Piercing)
        {
            swordGravity = piercingGravity;
        }
        else if (swordType == SwordType.Spinning)
        {
            swordGravity = spinGravity;
        }
    }

    #region Aiming
    public Vector2 AimDirection()
    {
        Vector2 playerPosition = player.transform.position;
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = mousePosition - playerPosition;

        return direction;
    }

    private void GenerateAimDots()
    {
        aimDots = new GameObject[numberOfDots];

        for (int i = 0; i < numberOfDots; i++)
        {
            aimDots[i] = Instantiate(aimDotPrefab, player.transform.position, Quaternion.identity, aimDotParent);
            aimDots[i].SetActive(false);
        }
    }

    public void AimDotsActive(bool _active)
    {
        for (int i = 0; i < aimDots.Length; i++)
        {
            aimDots[i].SetActive(_active);
        }
    }

    private Vector2 AimDotsPosition(float _time)
    {
        Vector2 position = (Vector2)player.transform.position + new Vector2(
            AimDirection().normalized.x * throwingForce.x,
            AimDirection().normalized.y * throwingForce.y) * _time + 0.5f * (Physics2D.gravity * swordGravity) * (_time * _time);
        
        return position;
    }
    #endregion
}
