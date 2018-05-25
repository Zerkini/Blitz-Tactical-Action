using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Fighter {

    #region variables
    [SerializeField]
    protected Transform pathfindingTarget;
    protected Vector2 pathfindingTargetVector;
    protected Vector2 patrolTargetVector;
    protected Vector2 startingPosition;
    protected bool returningToStartingPostition;
    protected GameObject closestAlly;
    protected string direction;
    #endregion
    new protected void Start()
    {
        base.Start();
        targetTag = "Ally";
    }

    private void Update()
    {
        
    }

    protected void shootAllies()
    {
        //UWAGA: closetAlly mógł się zmienić w trakcie przechodzenia do osłony
        if (closestAlly != null)
        {
            if (Vector3.Distance(transform.position, closestAlly.transform.position) <= weaponRange && Time.time > nextFire)
            {
                ShootTargetInRange(closestAlly, targetTag, weaponDamage);
            }
        }
    }
   
    protected void Patrol()
    {
        if (!moving && !returningToStartingPostition)
        {
            this.returningToStartingPostition = true;
            this.moving = true;
            PathfindingManager.RequestPath(transform.position, this.patrolTargetVector, OnPathFound);
        }
        else if (!moving && returningToStartingPostition)
        {
            this.returningToStartingPostition = false;
            this.moving = true;
            PathfindingManager.RequestPath(transform.position, this.startingPosition, OnPathFound);
        }
    }


}