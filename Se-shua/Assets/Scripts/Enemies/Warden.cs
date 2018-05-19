using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Warden : Enemy {

    #region States
    private bool watchState, combatState;
    #endregion

    new private void Start () {
        base.Start();
        this.startingPosition = transform.position;
        SetToWatchState();
    }

    void Update () {
        if (watchState)
        {
            DetectAlliesWatch();
        }
        else if (combatState)
        {
            shootAllies();
            DetectAlliesCombat();
        }
    }

    private void DetectAlliesWatch()
    {
        closestAlly = GetClosestObject("Ally");
        if (closestAlly != null)
        {
            if (Vector3.Distance(transform.position, closestAlly.transform.position) <= detectionRange)
            {
                SetToCombatState();
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
                SetToWatchState();
            }
        }
        else
        {
            SetToWatchState();
        }
    }

    private void SetToWatchState()
    {
        watchState = true;
        combatState = false;
    }

    private void SetToCombatState()
    {
        watchState = false;
        combatState = true;
    }
}
