using System.Collections;
using UnityEngine;

public class Node : IHeapItem<Node> {

    public bool walkable, coverUp, coverDown, coverLeft, coverRight;
    public Vector2 position;
    public int currentCost, estimatedCost, gridPositionX, gridPositionY, heapIndex;
    public Node parent;

    public Node(bool walkable, Vector2 position, int gridPositionX, int gridPositionY)
    {
        this.walkable = walkable;
        this.position = position;
        this.gridPositionX = gridPositionX;
        this.gridPositionY = gridPositionY;
    }

    public Node(bool walkable, bool coverUp, bool coverDown, bool coverLeft, bool coverRight, Vector2 position, int gridPositionX, int gridPositionY)
    {
        this.walkable = walkable;
        this.position = position;
        this.gridPositionX = gridPositionX;
        this.gridPositionY = gridPositionY;
        this.coverDown = coverDown;
        this.coverLeft = coverLeft;
        this.coverRight = coverRight;
        this.coverUp = coverUp;
    }

    public int getTotalCost()
    {
        return currentCost + estimatedCost;
    }

    public int HeapIndex
    {
        get
        {
            return heapIndex;
        }
        set
        {
            heapIndex = value;
        }
    }

    public int CompareTo(Node nodeToCompare)
    {
        int compare = getTotalCost().CompareTo(nodeToCompare.getTotalCost());
        if(compare == 0)
        {
            compare = estimatedCost.CompareTo(nodeToCompare.estimatedCost);
        }
        return -compare;
    }
}
