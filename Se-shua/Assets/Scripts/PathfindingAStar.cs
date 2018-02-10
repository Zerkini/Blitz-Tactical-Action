using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathfindingAStar : MonoBehaviour
{

    private Grid grid;
    public Transform seeker, target;

    private void Awake()
    {
        grid = GetComponent<Grid>();
    }

    private void Update()
    {
        FindPath(seeker.position, target.position);
    }


    public void FindPath(Vector2 startPosition, Vector2 targetPosition)
    {
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
                RetracePath(startNode, targetNode);
                return;
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
    }

    void RetracePath(Node startNode, Node targetNode)
    {
        List<Node> path = new List<Node>();
        Node currentNode = targetNode;

        while (currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }
        path.Reverse();
        grid.path = path;
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