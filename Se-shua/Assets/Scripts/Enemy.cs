using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Fighter {

    [SerializeField]
    private Transform pathfindingTarget;
    private Vector2 pathfindingTargetVector;
    private Vector2 patrolTargetVector;
    private Vector2 startingPosition;
    private bool returningToStartingPostition;
    GameObject closestAlly;
    string direction;
    #region States
    private bool patrolState, seekingCoverState, combatState, seekingCover = false;
    #endregion
    private void Start()
    {
        targetTag = "Ally";
        gunAudio = GetComponent<AudioSource>();
        this.startingPosition = transform.position;
        this.patrolTargetVector = pathfindingTarget.position;
        SetToPatrolState();
    }

    private void Update()
    {
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
        if (Vector3.Distance(transform.position, closestAlly.transform.position) <= detectionRange)
        {
            SetToSeekingCoverState();
            direction = DetermineDirection(closestAlly);
        }
    }

    private void DetectAlliesCombat()
    {
        closestAlly = GetClosestObject("Ally");
        if (Vector3.Distance(transform.position, closestAlly.transform.position) > detectionRange)
        {
            this.pathfindingTargetVector = pathfindingTarget.position;
            SetToPatrolState();
        }
    }

    private void seekCover()
    {
        this.seekingCover = true;
        direction = DetermineDirection(closestAlly);
        GoToClosetCoverWithDirection(direction);
    }

    private void shootAllies()
    {
        //UWAGA: closetAlly mógł się zmienić w trakcie przechodzenia do osłony
        if (Vector3.Distance(transform.position, closestAlly.transform.position) <= weaponRange && Time.time > nextFire)
        {
            ShootTargetInRange(closestAlly, targetTag, weaponDamage);
        }
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

    private void Patrol()
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