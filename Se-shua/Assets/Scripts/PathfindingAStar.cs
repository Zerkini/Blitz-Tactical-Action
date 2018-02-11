using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class PathfindingAStar : MonoBehaviour
{

    private Grid grid;
    private PathfindingManager pathfindingManager;

    private void Awake()
    {
        grid = GetComponent<Grid>();
        pathfindingManager = GetComponent<PathfindingManager>();
    }

    public void StartPathfinding(Vector2 pathStart, Vector2 pathEnd)
    {
        StartCoroutine(FindPath(pathStart, pathEnd));
    }


    public IEnumerator FindPath(Vector2 startPosition, Vector2 targetPosition)
    {
        Vector2[] waypoints = new Vector2[0];
        bool pathSuccess = false;

        Node startNode = grid.GetNodeFromPosition(startPosition);
        Node targetNode = grid.GetNodeFromPosition(targetPosition);

        Heap<Node> openSet = new Heap<Node>(grid.MaxSize);
        HashSet<Node> closedSet = new HashSet<Node>();
        openSet.Add(startNode);

        while (openSet.Count > 0)
        {
            Node currentNode = openSet.RemoveFirst();
            
            closedSet.Add(currentNode);

            if (currentNode == targetNode)
            {
                pathSuccess = true;
                break;
            }

            foreach (Node neighbour in grid.GetNeighbours(currentNode))
            {
                if(neighbour.walkable && !closedSet.Contains(neighbour))
                {
                    int newCurrentCostToNeighbour = currentNode.currentCost + GetDistance(currentNode, neighbour);
                    if (newCurrentCostToNeighbour < neighbour.currentCost || !openSet.Contains(neighbour))
                    {
                        neighbour.currentCost = newCurrentCostToNeighbour;
                        neighbour.estimatedCost = GetDistance(neighbour, targetNode);
                        neighbour.parent = currentNode;

                        if (!openSet.Contains(neighbour))
                        {
                            openSet.Add(neighbour);
                            openSet.SortDown(neighbour);
                        }
                    }
                }
            }
        }
        yield return null;
        if (pathSuccess)
        {
            waypoints = RetracePath(startNode, targetNode);
        }
        pathfindingManager.FinishedProcessingPath(waypoints, pathSuccess);
    }

    public Vector2[] RetracePath(Node startNode, Node targetNode)
    {
        List<Node> path = new List<Node>();
        Node currentNode = targetNode;

        while (currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }
        Vector2[] waypoints = ConvertNodePathToWaypointPath(path);
        Array.Reverse(waypoints);
        grid.path = path;
        return waypoints;
    }

    private Vector2[] ConvertNodePathToWaypointPath(List<Node> path)
    {
        List<Vector2> waypoints = new List<Vector2>();
        Vector2 oldDirection = Vector2.zero;

        for (int i = 1; i < path.Count; i++)
        {
            Vector2 newDirection = new Vector2(path[i - 1].gridPositionX - path[i].gridPositionX, path[i - 1].gridPositionY - path[i].gridPositionY);
            if(newDirection != oldDirection)
            {
                waypoints.Add(path[i].position);
            }
            oldDirection = newDirection;
        }
        return waypoints.ToArray();
    }

    int GetDistance(Node startNode, Node targetNode)
    {
        int DistanceX = Mathf.Abs(startNode.gridPositionX - targetNode.gridPositionX);
        int DistanceY = Mathf.Abs(startNode.gridPositionY - targetNode.gridPositionY);

        if (DistanceX > DistanceY)
        {
            return 14 * DistanceY + 10 * (DistanceX - DistanceY);
        }
        else
        {
            return 14 * DistanceX + 10 * (DistanceY - DistanceX);
        }

    }

}