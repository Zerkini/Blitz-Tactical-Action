using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Seeker : MonoBehaviour {

    #region PathFindingVariables
    [SerializeField]
    private Transform target;
    [SerializeField]
    private float speed = 5;
    [SerializeField]
    private LineRenderer gunLine;
    private Vector2[] path;
    private int targetIndex;
    private Vector2 targetLocation;
    private bool moving = false;
    #endregion

    #region ShootingVariables

    private AudioSource gunAudio;
    private float nextFire;
    [SerializeField]
    private float fireRate = .25f, weaponRange = 20;
    private WaitForSeconds shotDuration = new WaitForSeconds(.3f);
    private bool enemyInRange;
    #endregion

    void Start()
    {
        gunAudio = GetComponent<AudioSource>();
    }

    //NOTE: Laser trafia dość niedokładnie względem miejsca kliknięcia, nie powinno to być ważne, ale można się zastanowić, czy sposób pobrania miejsca do trafienia poprzez raycast jest błędem
    void Update()
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
        if (Physics.Raycast(transform.position, enemy.transform.position, out hit, weaponRange))
        {
            gunLine.SetPosition(0, transform.position);
            gunLine.SetPosition(1, hit.transform.position);
            StartCoroutine(ShotEffect());
        }
        else
        {
            Debug.DrawRay(transform.position, enemy.transform.position, Color.green);
        }
        
    }


    private void ShootClickedPoint()
    {
        nextFire = Time.time + fireRate;
        RaycastHit hit;
        Vector3 shotLocation;
        Debug.Log("ShootClickedPoint");
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 200))
        {
            shotLocation = hit.point;
            shotLocation.z = 2;
            if (Physics.Raycast(transform.position, shotLocation, out hit, 100))
            {
                gunLine.SetPosition(0, transform.position);
                gunLine.SetPosition(1, shotLocation);
                Debug.Log("hit.point: " + hit.point);
                Debug.Log("hit.transform.position: " + shotLocation);
                StartCoroutine(ShotEffect());
            }
            else
            {
                Debug.Log("no hit" );
            }
        }
    }

    public void OnPathFound(Vector2[] newPath, bool pathSuccessful)
    {
        if (pathSuccessful)
        {
            path = newPath;
            StopCoroutine("FollowPath");
            StartCoroutine("FollowPath");
        }
    }

    private IEnumerator ShotEffect()
    {
        gunAudio.Play();
        gunLine.enabled = true;
        yield return shotDuration;
        gunLine.enabled = false;
    }

    private IEnumerator FollowPath()
    {
        Vector3 currentWaypoint = path[0];
        targetIndex = 0;

        while (true)
        {
            if (transform.position == currentWaypoint)
            {
                targetIndex++;
                if (targetIndex >= path.Length)
                {
                    moving = false;
                    yield break;
                }
                currentWaypoint = path[targetIndex];
                currentWaypoint.z = 0;
            }

            transform.position = Vector3.MoveTowards(transform.position, currentWaypoint, speed * Time.deltaTime);
            yield return null;
        }
    }

    public GameObject GetClosestObject(String tag)
    {
        GameObject[] objectsWithTag = GameObject.FindGameObjectsWithTag(tag);
        GameObject closestObject = null;
        for (int i = 0; i < objectsWithTag.Length; i++)
        {
            if (closestObject == null)
            {
                closestObject = objectsWithTag[i];
            }
            if (Vector3.Distance(transform.position, objectsWithTag[i].transform.position) <= Vector3.Distance(transform.position, closestObject.transform.position))
            {
                closestObject = objectsWithTag[i];
            }
        }
        return closestObject;
    }

    public void OnDrawGizmos()
    {
        if (path != null)
        {
            for (int i = targetIndex; i < path.Length; i++)
            {
                Gizmos.color = Color.black;
                Gizmos.DrawCube(path[i], Vector3.one);

                if (i == targetIndex)
                {
                    Gizmos.DrawLine(transform.position, path[i]);
                }
                else
                {
                    Gizmos.DrawLine(path[i - 1], path[i]);
                }
            }
        }
    }
}
