using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sentinel : Enemy {

    #region States
    private bool patrolState, seekingCoverState, combatState, seekingCover = false;
    #endregion

    // Use this for initialization
    new private void Start () {
        base.Start();
        this.startingPosition = transform.position;
        this.patrolTargetVector = pathfindingTarget.position;
        SetToPatrolState();

    }

    // Update is called once per frame
    void Update () {
        if (patrolState)
        {
            DetectAlliesPatrol();
            Patrol();
        }
        else if (seekingCoverState)
        {
            if (!seekingCover)
            {
                seekCover();
            }
            if (!this.moving)
            {
                this.seekingCover = false;
                SetToCombatState();
            }
        }
        else if (combatState)
        {
            shootAllies();
            DetectAlliesCombat();
        }
    }

    private void DetectAlliesPatrol()
    {
        closestAlly = GetClosestObject("Ally");
        if (closestAlly != null)
        {
            if (Vector3.Distance(transform.position, closestAlly.transform.position) <= detectionRange)
            {
                SetToSeekingCoverState();
                direction = DetermineDirection(closestAlly);
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
        SetToPatrolState();
    }


    private void seekCover()
    {
        this.seekingCover = true;
        direction = DetermineDirection(closestAlly);
        GoToClosetCoverWithDirection(direction);
    }

    private string DetermineDirection(GameObject ally)
    {
        string direction = null;
        Vector2 vector = ally.transform.position - transform.position;
        float angle = Vector2.Angle(Vector2.up, vector);
        if (vector.x > 0 && 45 < angle && angle < 135)
        {
            direction = "right";
        }
        else if (angle >= 135)
        {
            direction = "down";
        }
        else if (vector.x <= 0 && 45 < angle && angle < 135)
        {
            direction = "left";
        }
        else if (angle <= 45)
        {
            direction = "up";
        }
        return direction;
    }

    private void GoToClosetCoverWithDirection(string direction)
    {
        //UWAGA, nie wiem czemu gdy szuka osłony LEFT jest błąd w znajdywaniu ścieżki, target node wydaje się legitny
        //WYGLĄDA NA TO, ZE JUZ DZIALA - na wszelki wypadek tu zostawie - poza tym to do usuniecia
        Node targetNode = Grid.GetClosestNodeWithCover(transform.position, direction);
        pathfindingTargetVector.x = targetNode.position[0];
        pathfindingTargetVector.y = targetNode.position[1];
        this.moving = true;
        PathfindingManager.RequestPath(transform.position, pathfindingTargetVector, OnPathFound);
    }

    private void SetToPatrolState()
    {
        patrolState = true;
        seekingCoverState = false;
        combatState = false;
    }

    private void SetToCombatState()
    {
        patrolState = false;
        seekingCoverState = false;
        combatState = true;
    }

    private void SetToSeekingCoverState()
    {
        patrolState = false;
        seekingCoverState = true;
        combatState = false;
    }
}
