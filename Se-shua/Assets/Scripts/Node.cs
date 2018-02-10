using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node {

    public bool walkable;
    public Vector2 position;

    public Node(bool walkable, Vector2 position)
    {
        this.walkable = walkable;
        this.position = position;
    }
}
