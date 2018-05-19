using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sentry : Enemy
{
    [SerializeField]
    GameObject alarmHighlight;

    #region States
    private bool patrolState, alarmingState, alarmCooldown = false;
    #endregion

    new private void Start()
    {
        base.Start();
        this.startingPosition = transform.position;
        this.patrolTargetVector = pathfindingTarget.position;
        SetToPatrolState();
        alarmHighlight.SetActive(false);

    }

    void Update()
    {
        if (patrolState)
        {
            DetectAlliesPatrol();
            Patrol();
        }
        else if (alarmingState)
        {
            Alarm();
        }
    }

    private void DetectAlliesPatrol()
    {
        closestAlly = GetClosestObject("Ally");
        if (closestAlly != null)
        {
            if (Vector3.Distance(transform.position, closestAlly.transform.position) <= detectionRange)
            {
                SetToAlarmingState();
            }
        }
    }

    private void Alarm()
    {
        closestAlly = GetClosestObject("Ally");
        if (closestAlly != null)
        {
            if (Vector3.Distance(transform.position, closestAlly.transform.position) <= detectionRange)
            {
                //AlarmLeadingAI(closestAlly.transform.position);
                if (moving)
                {
                    StopCoroutine("FollowPath");
                    moving = false;
                }
                alarmHighlight.SetActive(true);
                if (!alarmCooldown)
                {
                    StartCoroutine(FiveSecondCooldown());
                }
            }
            else
            {
                if (!alarmCooldown)
                {
                    SetToPatrolState();
                    alarmHighlight.SetActive(false);
                    return;
                }
            }
        }
        else
        {
            SetToPatrolState();
        }
    }

    public IEnumerator FiveSecondCooldown()
    {
        alarmCooldown = true;
        yield return new WaitForSeconds(5.0f);
        alarmCooldown = false;
    }

    private void SetToPatrolState()
    {
        patrolState = true;
        alarmingState = false;
    }

    private void SetToAlarmingState()
    {
        patrolState = false;
        alarmingState = true;
    }

}