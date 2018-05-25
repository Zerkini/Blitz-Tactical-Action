using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour{

    [SerializeField]
    private static int gridX = 140, gridY = 140;
    private static Vector2 gridSize = new Vector2(gridX, gridY);

    [SerializeField]
    private static float nodeRadius = 0.3f;

    //public Transform seeker, target;
    private static Node[,] grid;
    private float nodeDiameter;
    private static int gridTilesNumberX, gridTilesNumberY;
    public List<Node> path;
    public static List<Node> nodesWithLeftCover = new List<Node>(), nodesWithRightCover = new List<Node>(), nodesWithUpCover = new List<Node>(), nodesWithDownCover = new List<Node>();

    private void Awake()
    {
        nodeDiameter = nodeRadius * 2;
        gridTilesNumberX = Mathf.RoundToInt(gridSize.x / nodeDiameter);
        gridTilesNumberY = Mathf.RoundToInt(gridSize.y / nodeDiameter);
        CreateGrid();
    }

    public int MaxSize
    {
        get
        {
            return gridTilesNumberX * gridTilesNumberY;
        }
    }

    private void CreateGrid()
    {
        grid = new Node[gridTilesNumberX, gridTilesNumberY];
       
        for (int x = 0; x < gridTilesNumberX; x++)
        {
            for (int y = 0; y < gridTilesNumberY; y++)
            {
                grid[x, y] = CreateNodeWithWalkability(x, y);
            }
        }
        for (int x = 0; x < gridTilesNumberX; x++)
        {
            for (int y = 0; y < gridTilesNumberY; y++)
            {
                Node node = grid[x, y];
                if (node.walkable)
                {
                    AddCoversToNode(node);
                }
            }
        }
    }

    private Node CreateNodeWithWalkability(int x, int y)
    {
        Vector2 bottomLeftNode = transform.position - Vector3.right * gridSize.x / 2 - Vector3.up * gridSize.y / 2;
        Vector3 newNode = bottomLeftNode + Vector2.right * (x * nodeDiameter + nodeRadius) + Vector2.up * (y * nodeDiameter + nodeRadius);
        newNode.z = 4;
        bool walkable = !(Physics.CheckSphere(newNode, nodeRadius));
        return new Node(walkable, newNode, x, y);
    }

    private void AddCoversToNode(Node node)
    {
        int x = node.gridPositionX;
        int y = node.gridPositionY;

        if (x > 0 && grid[x - 1, y].walkable == false && y > 0 && grid[x - 1, y - 1].walkable == false && y < gridTilesNumberY - 1 && grid[x - 1, y + 1].walkable == false)
        {
            node.coverLeft = true;
            nodesWithLeftCover.Add(node);
        }
        if (x < gridTilesNumberX - 1 && grid[x + 1, y].walkable == false && y > 0 && grid[x + 1, y - 1].walkable == false && y < gridTilesNumberY - 1 && grid[x + 1, y + 1].walkable == false)
        {
            node.coverRight = true;
            nodesWithRightCover.Add(node);
        }
        if (y > 0 && grid[x, y - 1].walkable == false && x > 0 && grid[x - 1, y - 1].walkable == false && x < gridTilesNumberX + 1 && grid[x + 1, y - 1].walkable == false)
        {
            node.coverDown = true;
            nodesWithDownCover.Add(node);
        }
        if (y < gridTilesNumberY - 1 && grid[x, y + 1].walkable == false && x > 0 && grid[x - 1, y + 1].walkable == false && x < gridTilesNumberX +1 && grid[x + 1, y + 1].walkable == false)
        {
            node.coverUp = true;
            nodesWithUpCover.Add(node);
        }
    }

    public static Node GetNodeFromPosition(Vector2 position)
    {
        float percentX = (position.x / gridSize.x + 0.5f);
        float percentY = (position.y / gridSize.y + 0.5f);
        percentX = Mathf.Clamp01(percentX);
        percentY = Mathf.Clamp01(percentY);

        int x = Mathf.RoundToInt((gridTilesNumberX - 1) * percentX);
        int y = Mathf.RoundToInt((gridTilesNumberY - 1) * percentY);
        return grid[x, y];
    }


    public List<Node> GetNeighbours(Node node)
    {
        List<Node> neighbours = new List<Node>();

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <=1; y++)
            {
                if (x != 0 || y != 0){
                    if (NeighbourIsOnTheGrid(x, y, node)){
                        neighbours.Add(grid[node.gridPositionX + x, node.gridPositionY + y]);
                    }
                }

            }
        }
        return neighbours;
    }

    private bool NeighbourIsOnTheGrid(int x, int y, Node node)
    {
        return (node.gridPositionX + x >= 0 && node.gridPositionX + x < gridTilesNumberX && node.gridPositionY + y >= 0 && node.gridPositionY + y < gridTilesNumberY);
    }

    public static Node GetClosestNodeWithCover(Vector2 position, string direction)
    {
        Node nodePosition = GetNodeFromPosition(position);
        Node closestNode = null;
        float shortestDistance = 100000000;
        if (direction.Equals("right"))
        {
            foreach(Node node in nodesWithRightCover)
            {
                if (PathfindingAStar.GetDistance(nodePosition, node) < shortestDistance)
                {
                    shortestDistance = PathfindingAStar.GetDistance(nodePosition, node);
                    closestNode = node;
                }
            }
        }
        else if (direction.Equals("left"))
        {
            foreach (Node node in nodesWithLeftCover)
            {
                if (PathfindingAStar.GetDistance(nodePosition, node) < shortestDistance)
                {
                    shortestDistance = PathfindingAStar.GetDistance(nodePosition, node);
                    closestNode = node;
                }
            }
        }
        else if (direction.Equals("up"))
        {
            foreach (Node node in nodesWithUpCover)
            {
                if (PathfindingAStar.GetDistance(nodePosition, node) < shortestDistance)
                {
                    shortestDistance = PathfindingAStar.GetDistance(nodePosition, node);
                    closestNode = node;
                }
            }
        }
        else if (direction.Equals("down"))
        {
            foreach (Node node in nodesWithDownCover)
            {
                if (PathfindingAStar.GetDistance(nodePosition, node) < shortestDistance)
                {
                    shortestDistance = PathfindingAStar.GetDistance(nodePosition, node);
                    closestNode = node;
                }
            }
        }
        return closestNode;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, new Vector3(gridSize.x, gridSize.y, 1));

        if (grid != null)
        {
            //Node seekerNode = GetNodeFromPosition(seeker.position);
            foreach (Node node in grid)
            {
                if (!node.walkable)
                {
                    Gizmos.color = Color.red;
                }
                else if (node.coverDown)
                {
                    Gizmos.color = Color.black;
                }
                else
                {
                    Gizmos.color = Color.green;
                }
                if (path != null)
                {
                    if (path.Contains(node))
                    {
                        Gizmos.color = Color.white;
                    }
                }
                //if (seekerNode == node)
                //{
                //    Gizmos.color = Color.white;
                //}
                Gizmos.DrawCube(node.position, Vector3.one * (nodeDiameter - 0.1f));

            }
        }
    }
}
