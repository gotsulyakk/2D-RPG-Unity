using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordSkill : Skill
{
    [Header("Skill info")]
    [SerializeField] private GameObject swordPrefab;
    [SerializeField] private Vector2 throwingDirection;
    [SerializeField] private float swordGravity;

    public void SpawnSword()
    {
        GameObject newSword = Instantiate(swordPrefab, player.transform.position, transform.rotation);
        SwordSkillController swordController = newSword.GetComponent<SwordSkillController>();

        swordController.ThrowSword(throwingDirection, swordGravity);
    }
}
