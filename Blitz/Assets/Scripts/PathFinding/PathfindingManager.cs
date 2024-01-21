using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathfindingManager : MonoBehaviour {

    private Queue<PathRequest> pathRequestQueue = new Queue<PathRequest>();
    private PathRequest currentPathRequest;

    private static PathfindingManager instance;
    private PathfindingAStar pathfinding;
    private bool isProcessingPath;

    private void Start()
    {
        instance = this;
        pathfinding = GetComponent<PathfindingAStar>();
    }

    public static void RequestPath(Vector2 pathStart, Vector2 pathEnd, Action<Vector2[], bool> callback)
    {
        PathRequest newRequest = new PathRequest(pathStart, pathEnd, callback);
        instance.pathRequestQueue.Enqueue(newRequest);
        instance.TryProcessNext();
    }

    private void TryProcessNext()
    {
        if (!isProcessingPath && pathRequestQueue.Count > 0)
        {
            currentPathRequest = pathRequestQueue.Dequeue();
            isProcessingPath = true;
            pathfinding.StartPathfinding(currentPathRequest.pathStart, currentPathRequest.pathEnd);
        }
    }

    public void FinishedProcessingPath(Vector2[] path, bool success)
    {
        currentPathRequest.callback(path, success);
        isProcessingPath = false;
        TryProcessNext();
    }

    struct PathRequest
    {
        public Vector2 pathStart;
        public Vector2 pathEnd;
        public Action<Vector2[], bool> callback;

        public PathRequest(Vector2 pathStart, Vector2 pathEnd, Action<Vector2[], bool> callback)
        {
            this.pathStart = pathStart;
            this.pathEnd = pathEnd;
            this.callback = callback;
        }
    }
}
