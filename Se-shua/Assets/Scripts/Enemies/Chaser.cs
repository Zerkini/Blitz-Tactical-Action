using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chaser : Enemy
{

    #region States
    private bool patrolState, closingState, pathfindingCooldown = false;
    #endregion

    new private void Start()
    {
        base.Start();
        this.startingPosition = transform.position;
        this.patrolTargetVector = pathfindingTarget.position;
        SetToPatrolState();

    }

    void Update()
    {
        if (patrolState)
        {
            DetectAlliesPatrol();
            Patrol();
        }
        else if (closingState)
        {
            DetectAlliesCombat();
            CloseOnAlly();
            shootAllies();
        }
    }
    private void DetectAlliesPatrol()
    {
        closestAlly = GetClosestObject("Ally");
        if (closestAlly != null)
        {
            if (Vector3.Distance(transform.position, closestAlly.transform.position) <= detectionRange)
            {
                SetToClosingState();
            }
        }
    }

    private void CloseOnAlly()
    {
        if (closestAlly != null)
        {
            if ((Vector3.Distance(transform.position, closestAlly.transform.position) <= detectionRange))
            {
                if (!pathfindingCooldown)
                {
                    PathfindingManager.RequestPath(transform.position, closestAlly.transform.position, OnPathFound);
                    StartCoroutine(OneSecondCooldown());
                }
            }
        }
    }

    private void DetectAlliesCombat()
    {
        closestAlly = GetClosestObject("Ally");
        if (closestAlly != null)
        {
            if (Vector3.Distance(transform.position, closestAlly.transform.position) > detectionRange)
            {
                this.pathfindingTargetVector = pathfindingTarget.position;
                SetToPatrolState();
            }
        }
        else
        {
            SetToPatrolState();
        }
    }

    public IEnumerator OneSecondCooldown()
    {
        pathfindingCooldown = true;
        yield return new WaitForSeconds(1.0f);
        pathfindingCooldown = false;
    }


    private void SetToPatrolState()
    {
        patrolState = true;
        closingState = false;
    }

    private void SetToClosingState()
    {
        patrolState = false;
        closingState = true;
    }

}