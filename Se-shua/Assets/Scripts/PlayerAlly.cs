using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAlly : Fighter {

    private void Start()
    {
        gunAudio = GetComponent<AudioSource>();
        healthPoints = 300;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            MoveToClickedPoint();
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

    private void ShootClickedPoint()
    {
        nextFire = Time.time + fireRate;
        RaycastHit hit;
        Vector3 shotLocation;
        Vector3 shotStart = transform.position;
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 200))
        {
            shotLocation = hit.point - transform.position;
            shotLocation.z = 0;
            shotStart = RandomPenetratingShot(shotStart, 0.8f);
            //shotLocation = DecreaseAccuracy(shotLocation);
            if (Physics.Raycast(shotStart, shotLocation, out hit, 100))
            {
                LaserEffect(hit);
                LaserDamage(hit, weaponDamage, targetTag);
            }
        }
    }

}
