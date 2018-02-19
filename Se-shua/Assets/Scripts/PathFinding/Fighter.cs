using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fighter: MonoBehaviour {

    #region PathFindingVariables
    [SerializeField]
    protected float speed = 5;
    protected Vector2[] path;
    protected int targetIndex;
    protected Vector2 targetLocation;
    protected bool moving = false;
    #endregion

    #region ShootingVariables
    [SerializeField]
    protected LineRenderer gunLine;
    protected AudioSource gunAudio;
    protected float nextFire;
    [SerializeField]
    protected float fireRate = .25f, weaponRange = 20;
    public float healthPoints = 100;
    protected WaitForSeconds shotDuration = new WaitForSeconds(.3f);
    protected bool enemyInRange;
    protected string targetTag = "Enemy";
    protected float weaponDamage = 30;
    #endregion

    private void Start()
    {
        gunAudio = GetComponent<AudioSource>();
    }

    private void Update()
    {
        
    }

    protected void OnPathFound(Vector2[] newPath, bool pathSuccessful)
    {
        if (pathSuccessful)
        {
            path = newPath;
            StopCoroutine("FollowPath");
            StartCoroutine("FollowPath");
        }
    }

    protected IEnumerator ShotEffect()
    {
        gunAudio.Play();
        gunLine.enabled = true;
        yield return shotDuration;
        gunLine.enabled = false;
    }

    protected IEnumerator FollowPath()
    {
        Vector3 currentWaypoint = path[0];
        targetIndex = 0;
        this.moving = true;

        while (true)
        {
            if (transform.position == currentWaypoint)
            {
                targetIndex++;
                if (targetIndex >= path.Length)
                {
                    this.moving = false;
                    yield break;
                }
                currentWaypoint = path[targetIndex];
                currentWaypoint.z = 0;
            }

            transform.position = Vector3.MoveTowards(transform.position, currentWaypoint, speed * Time.deltaTime);
            yield return null;
        }
    }

    protected GameObject GetClosestObject(String tag)
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

    protected void ShootTargetInRange(GameObject target, String tag, float damage)
    {
        nextFire = Time.time + fireRate;
        RaycastHit hit;

        if (Physics.Raycast(transform.position, target.transform.position - transform.position, out hit, weaponRange))
        {
            LaserEffect(hit);
            LaserDamage(hit, damage, tag);
        }
    }

    protected void LaserEffect(RaycastHit hit)
    {
        Vector3 shotLocation = hit.point;
        gunLine.SetPosition(0, transform.position);
        gunLine.SetPosition(1, shotLocation);
        StartCoroutine(ShotEffect());
    }

    protected void LaserDamage(RaycastHit hit, float damage, String tag)
    {
        if (hit.collider.tag.Equals(tag))
        {
            Fighter fighter = hit.collider.gameObject.GetComponent<Fighter>();
            fighter.DecreaseHealthPoints(damage);
        }
    }

    public void DecreaseHealthPoints(float damage)
    {
        healthPoints -= damage;
        if (healthPoints <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        StopAllCoroutines();
        gunLine.enabled = false;
        foreach (var component in gameObject.GetComponents<Component>())
        {
            if (!(component is Transform))
            {
                Destroy(component);
            }
        }
        Destroy(this);
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

