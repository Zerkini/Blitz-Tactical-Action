using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node {

    public bool walkable;
    public Vector2 position;
    public int currentCost, estimatedCost, gridPositionX, gridPositionY;
    public Node parent;

    public Node(bool walkable, Vector2 position, int gridPositionX, int gridPositionY)
    {
        this.walkable = walkable;
        this.position = position;
        this.gridPositionX = gridPositionX;
        this.gridPositionY = gridPositionY;
    }

    public int getTotalCost()
    {
        return currentCost + estimatedCost;
    }
}
