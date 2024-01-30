using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordSkill : Skill
{
    [Header("Skill info")]
    [SerializeField] private GameObject swordPrefab;
    [SerializeField] private Vector2 throwingForce;
    [SerializeField] private float swordGravity;

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

        swordController.ThrowSword(finalDirection, swordGravity, player);

        player.AssignNewSword(newSword);

        AimDotsActive(false);
    }

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
}
