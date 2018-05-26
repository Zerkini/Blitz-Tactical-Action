using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialChaser : Chaser
{
    public bool standingBy = true;
    #region States
    private bool patrolState, closingState, pathfindingCooldown = false;
    #endregion

    new private void Start()
    {
        targetTag = "Ally";
        gunAudioHit = GetComponents<AudioSource>()[0];
        gunAudioMiss = GetComponents<AudioSource>()[1];
    }

    void Update()
    {
        if (patrolState)
        {
            DetectAlliesPatrol();
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
            float distance = Vector3.Distance(transform.position, closestAlly.transform.position);
            if (2 < distance && distance <= detectionRange)
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

    public void moveToPosition(Vector2 repositionTargetVector)
    {
        PathfindingManager.RequestPath(transform.position, repositionTargetVector, OnPathFound);
        this.startingPosition = transform.position;
        this.patrolTargetVector = repositionTargetVector;
        SetToPatrolState();
    }
}