using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour {

    [SerializeField]
    private Vector2 gridSize;
    [SerializeField]
    private float nodeRadius;
    [SerializeField]
    private Transform player;

    private Node[,] grid;
    private float nodeDiameter;
    private int gridTilesNumberX, gridTilesNumberY;
    
    

    private void Start()
    {
        nodeDiameter = nodeRadius * 2;
        gridTilesNumberX = Mathf.RoundToInt(gridSize.x / nodeDiameter);
        gridTilesNumberY = Mathf.RoundToInt(gridSize.y / nodeDiameter);
        CreateGrid();
    }

    private void CreateGrid()
    {
        grid = new Node[gridTilesNumberX, gridTilesNumberY];
        Vector2 bottomLeftNode = transform.position - Vector3.right * gridSize.x / 2 - Vector3.up * gridSize.y / 2;
        for (int x = 0; x < gridTilesNumberX; x++)
        {
           for (int y = 0; y < gridTilesNumberY; y++)
            {
                Vector2 newNode = bottomLeftNode + Vector2.right * (x * nodeDiameter + nodeRadius) + Vector2.up * (y * nodeDiameter + nodeRadius);
                bool walkable = !(Physics.CheckSphere(newNode, nodeRadius));
                grid[x, y] = new Node(walkable, newNode);
            }
        }
    }

    public Node GetNodeFromPosition(Vector2 position)
    {
        float percentX = (position.x / gridSize.x + 0.5f);
        float percentY = (position.y / gridSize.y + 0.5f);
        percentX = Mathf.Clamp01(percentX);
        percentY = Mathf.Clamp01(percentY);

        int x = Mathf.RoundToInt((gridTilesNumberX - 1) * percentX);
        int y = Mathf.RoundToInt((gridTilesNumberY - 1) * percentY);
        return grid[x, y];
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, new Vector3(gridSize.x, gridSize.y, 1));

        if (grid != null)
        {
            Node playerNode = GetNodeFromPosition(player.position);
            foreach (Node node in grid)
            {
                Gizmos.color = (node.walkable) ? Color.green : Color.red;
                if (playerNode == node)
                {
                    Gizmos.color = Color.cyan;
                }
                Gizmos.DrawCube(node.position, Vector3.one * (nodeDiameter - 0.1f));

            }
        }
    }
}
