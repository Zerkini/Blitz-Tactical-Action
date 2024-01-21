using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ally : Fighter{
    
    private void Update()
    {
        GameObject closestEnemy = GetClosestObject(targetTag);
        if (Vector3.Distance(transform.position, closestEnemy.transform.position) <= weaponRange && Time.time > nextFire)
        {
            ShootTargetInRange(closestEnemy, targetTag, weaponDamage);
        }
    }
}
