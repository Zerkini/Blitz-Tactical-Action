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
    protected AudioSource gunAudioHit;
    protected AudioSource gunAudioMiss;
    protected float nextFire;
    [SerializeField]
    protected float fireRate = .25f, weaponRange = 10, detectionRange = 10;
    public float healthPoints = 100;
    protected WaitForSeconds shotDuration = new WaitForSeconds(.3f);
    protected bool enemyInRange;
    protected string targetTag = "Enemy";
    protected float weaponDamage = 30;
    #endregion

    #region miscellaneousVariables
    [SerializeField]
    protected GameObject text;
    #endregion


    protected void Start()
    {
        gunAudioHit = GetComponents<AudioSource>()[0];
        gunAudioMiss = GetComponents<AudioSource>()[1];
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

    protected IEnumerator ShotEffectHit()
    {
        gunAudioHit.Play();
        gunLine.enabled = true;
        yield return shotDuration;
        gunLine.enabled = false;
    }

    protected IEnumerator ShotEffectMiss()
    {
        gunAudioMiss.Play();
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
                //currentWaypoint.z = 0;
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
        Vector3 raycastStart = RandomHeightShot(transform.position);
        Vector3 raycastDirection = DecreaseAccuracy(target.transform.position - raycastStart);
        raycastDirection.z = 0;
        if (Physics.Raycast(raycastStart, raycastDirection, out hit, weaponRange))
        {
            LaserEffect(hit.point, true);
            LaserDamage(hit, damage, tag);
        }
        else if(Physics.Raycast(raycastStart, raycastDirection, out hit, 10000)){
            LaserEffect(hit.point, false);
        }
    }

    protected Vector3 DecreaseAccuracy(Vector3 vector)
    {
        System.Random random = new System.Random();
        float maxRandom = 1.6f;
        float minRandom = 0.4f;
        vector.x *=  (float)(random.NextDouble() * (maxRandom - minRandom) + minRandom);
        vector.y *= (float)(random.NextDouble() * (maxRandom - minRandom) + minRandom);
        return vector;
    }

    protected Vector3 RandomHeightShot(Vector3 vector)
    {
        System.Random random = new System.Random();
        float maxRandom = -1.05f;
        float minRandom = -2f;
        vector.z = (float)(random.NextDouble() * (maxRandom - minRandom) + minRandom); //strzaly ponad oslona sa na wysokosciach pomiedzy -2 i -1.05
        return vector;
    }

    protected void LaserEffect(Vector3 shotLocation, bool hit)
    {
        Vector3 shotOrigin = transform.position;
        shotOrigin.z = -4;
        shotLocation.z = -4;
        gunLine.SetPosition(0, shotOrigin);
        gunLine.SetPosition(1, shotLocation);
        if (hit)
        {
            StartCoroutine(ShotEffectHit());
        }
        else
        {
            StartCoroutine(ShotEffectMiss());
        }
        
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
        if (text != null)
        {
            Destroy(text);
        }
        if (gameObject.tag.Equals("Ally"))
        {
            DecisionTree.PlayerDestroyedAlert();
        }
        Destroy(gameObject);
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

