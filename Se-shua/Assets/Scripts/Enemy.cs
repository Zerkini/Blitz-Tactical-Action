using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Fighter {

    [SerializeField]
    private Transform pathfindingTarget;
    private Vector3 startingPosition;
    private bool returningToStartingPostition;

    private void Start()
    {
        this.startingPosition = transform.position;
    }

    private void Update()
    {
        if (!moving && !returningToStartingPostition)
        {
            returningToStartingPostition = true;
            this.moving = true;
            PathfindingManager.RequestPath(transform.position, pathfindingTarget.position, OnPathFound);
        }
        else if (!moving && returningToStartingPostition)
        {
            returningToStartingPostition = false;
            this.moving = true;
            PathfindingManager.RequestPath(transform.position, startingPosition, OnPathFound);
        }

    }
}
