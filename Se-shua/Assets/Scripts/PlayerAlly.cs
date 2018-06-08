using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAlly : Fighter {

    [SerializeField]
    private bool selected = false;
    private string numberTag;
    [SerializeField]
    GameObject selectionHighlight;
    [SerializeField]
    private float selectedFireRate;

    new private void Start()
    {
        base.Start();
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
            if (Input.GetMouseButtonDown(1))
            {
                MoveToClickedPoint();
            }
            if (Input.GetMouseButtonDown(0) && Time.time > nextFire)
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
                transform.position = new Vector3(transform.position.x, transform.position.y, 0.3f);
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
        if (Vector3.Distance(transform.position, targetLocation) > 0.5)
        {
            PathfindingManager.RequestPath(transform.position, targetLocation, OnPathFound);
        }
    }

    private void ShootClickedPoint()
    {
        nextFire = Time.time + selectedFireRate;
        RaycastHit hit;
        Vector3 shotLocation;
        Vector3 shotStart = transform.position;
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 10000))
        {
            shotLocation = hit.point - transform.position;
            shotLocation.z = 0;
            shotStart = RandomHeightShot(shotStart);
            if (Physics.Raycast(shotStart, shotLocation, out hit, weaponRange))
            {
                LaserEffect(hit.point, true);
                LaserDamage(hit, weaponDamage, targetTag);
            }
            else if (Physics.Raycast(shotStart, shotLocation, out hit, 10000))
            {
                LaserEffect(hit.point, false);
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
        else if (Input.GetKeyDown(KeyCode.Keypad3) || Input.GetKeyDown(KeyCode.Alpha3))
        {
            if (numberTag.Equals("3"))
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
