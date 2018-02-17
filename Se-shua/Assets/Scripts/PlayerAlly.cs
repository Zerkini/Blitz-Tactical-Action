using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAlly : Fighter
{

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            MoveToClickedPoint();
        }
        GameObject closestEnemy = GetClosestObject("Enemy");

        if (Vector3.Distance(transform.position, closestEnemy.transform.position) <= weaponRange && Time.time > nextFire)
        {
            ShootEnemyInRange(closestEnemy);
        }

        if (Input.GetMouseButtonDown(1) && Time.time > nextFire)
        {
            ShootClickedPoint();
        }
    }

    private void MoveToClickedPoint()
    {
        RaycastHit hit;
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100))
        {
            this.targetLocation = hit.point;
        }
        PathfindingManager.RequestPath(transform.position, targetLocation, OnPathFound);
    }

    private void ShootEnemyInRange(GameObject enemy)
    {
        nextFire = Time.time + fireRate;

        RaycastHit hit;
        if (Physics.Raycast(transform.position, enemy.transform.position - transform.position, out hit, weaponRange))
        {
            Vector3 shotLocation = hit.point;
            gunLine.SetPosition(0, transform.position);
            gunLine.SetPosition(1, shotLocation);
            StartCoroutine(ShotEffect());
        }

    }

    private void ShootClickedPoint()
    {
        nextFire = Time.time + fireRate;
        RaycastHit hit;
        Vector3 shotLocation;
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 200))
        {
            shotLocation = hit.point - transform.position;
            shotLocation.z = 0;
            if (Physics.Raycast(transform.position, shotLocation, out hit, 100))
            {
                gunLine.SetPosition(0, transform.position);
                gunLine.SetPosition(1, hit.point);
                StartCoroutine(ShotEffect());
            }
        }
    }
}
