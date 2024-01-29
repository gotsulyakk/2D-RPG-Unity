using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloneSkill : Skill
{
    [Header("Clone Skill Settings")]
    [SerializeField] private GameObject clonePrefab;
    [SerializeField] private float cloneLiveTime;
    [Space]
    [SerializeField] private bool canAttack;

    public void SpawnClone(Transform _clonePosition)
    {
        GameObject newClone = Instantiate(clonePrefab);
        newClone.GetComponent<CloneSkillController>().SetupClone(_clonePosition, cloneLiveTime, canAttack);
    }
}
