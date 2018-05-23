using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAlly : Fighter {

    [SerializeField]
    private bool selected = false;
    private string numberTag;
    [SerializeField]
    GameObject selectionHighlight;

    private void Start()
    {
        gunAudio = GetComponent<AudioSource>();
        healthPoints = 300;
        numberTag = text.GetComponent<TextMesh>().text;
        selectionHighlight.SetActive(false);
    }

    private void Update()
    {
        CheckSelection();
        SetCover();
        if (selected)
        {
            selectionHighlight.SetActive(true);
            if (Input.GetMouseButtonDown(0))
            {
                MoveToClickedPoint();
            }

            if (Input.GetMouseButtonDown(1) && Time.time > nextFire)
            {
                ShootClickedPoint();
            }
        }
        else
        {
            selectionHighlight.SetActive(false);
            ShootEnemies();
        }
    }

    private void SetCover()
    {
        if (!moving)
        {
            Node nodePosition = Grid.GetNodeFromPosition(transform.position);
            if ((nodePosition.coverUp || nodePosition.coverDown || nodePosition.coverLeft || nodePosition.coverRight) && transform.position.z == 0)
            {
                transform.position = new Vector3(transform.position.x, transform.position.y, 0.9f);
            }
        }
        else if (transform.position.z != 0)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, 0);
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
            shotStart = RandomHeightShot(shotStart);
            //shotLocation = DecreaseAccuracy(shotLocation);
            if (Physics.Raycast(shotStart, shotLocation, out hit, 100))
            {
                LaserEffect(hit.point);
                LaserDamage(hit, weaponDamage, targetTag);
            }
        }
    }


    private void CheckSelection()
    {
        if (Input.GetKeyDown(KeyCode.Keypad1) || Input.GetKeyDown(KeyCode.Alpha1)){
            if (numberTag.Equals("1"))
            {
                selected = true;
            }
            else
            {
                selected = false;
            }
        }
        else if (Input.GetKeyDown(KeyCode.Keypad2) || Input.GetKeyDown(KeyCode.Alpha2)){
            if (numberTag.Equals("2"))
            {
                selected = true;
            }
            else
            {
                selected = false;
            }
        }
    }

    private void ShootEnemies()
    {
        GameObject closestEnemy = GetClosestObject(targetTag);
        if (closestEnemy != null)
        {
            if (Vector3.Distance(transform.position, closestEnemy.transform.position) <= weaponRange && Time.time > nextFire)
            {
                ShootTargetInRange(closestEnemy, targetTag, weaponDamage);
            }
        }
    }
}
