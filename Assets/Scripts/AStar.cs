using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
public class AStar : MonoBehaviour {
    public Material pathMaterial, startMaterial, goalMaterial, solutionMaterial;
    private Node startNode, goalNode;
    private GameObject startCube, goalCube;

    private int[,] mazeMap;
    private GameObject[,] mazeObjMap;
    private bool isDone;

    private void Start() {
    }
    private void Update() {
        HandleInput();
    }

    private void HandleInput() {
        if (Input.GetMouseButtonDown(0)) {
            if (isDone) ClearPath();
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit)) {
                GameObject hitObject = hit.collider.gameObject;

                // Check if the clicked object is a cube
                if (hitObject.CompareTag("Path")) {
                    // Get the position of the clicked cube
                    int x = Mathf.RoundToInt(hitObject.transform.position.x);
                    int y = Mathf.RoundToInt(hitObject.transform.position.z);

                    // Toggle between setting start and end points
                    if (startNode == null) {
                        startNode = new Node(x, y);
                        hitObject.GetComponent<Renderer>().material = startMaterial;
                        startCube = hitObject;
                    } else if (goalNode == null) {
                        goalNode = new Node(x, y);
                        hitObject.GetComponent<Renderer>().material = goalMaterial;
                        goalCube = hitObject;

                        // Perform A* search once both start and end points are selected
                        mazeMap = MazeGenerator.maze;
                        mazeObjMap = MazeGenerator.mazeObj;
                        List<Node> path = AStarSearch(startNode, goalNode);

                        foreach (Node node in path) {
                            mazeObjMap[node.x, node.y].GetComponent<Renderer>().material = solutionMaterial;
                        }
                        // Reset start and end nodes for the next selection
                        startNode = null;
                        goalNode = null;
                        isDone = true;
                    }
                }
            }
        }
    }

    void ClearPath() {
        for (int i = 0; i < mazeMap.GetLength(0); i++) 
            for (int j = 0; j < mazeMap.GetLength(1); j++) 
                if (mazeObjMap[i, j].CompareTag("Path"))
                    mazeObjMap[i, j].GetComponent<Renderer>().material = pathMaterial;
        isDone = false;
    }

    List<Node> AStarSearch(Node start, Node goal) {

        List<Node> openList = new List<Node>();
        HashSet<Node> closedList = new HashSet<Node>();

        openList.Add(start);

        while (openList.Count > 0) {
            // Find the node with the lowest total cost in the open list
            Node current = openList[0];
            for (int i = 1; i < openList.Count; i++) {
                if (openList[i].TotalCost < current.TotalCost) {
                    current = openList[i];
                }
            }

            openList.Remove(current);
            closedList.Add(current);

            // If the goal is reached, reconstruct and return the path
            if (current.x == goal.x && current.y == goal.y) {
                return ReconstructPath(current);
            }

            // Generate successors
            List<Node> successors = GenerateSuccessors(current);

            foreach (Node successor in successors) {
                // Skip if the successor is in the closed list
                if (closedList.Contains(successor)) {
                    continue;
                }

                // Calculate costs
                int newCost = current.cost + 1; // Assuming each step has a cost of 1
                int heuristic = CalculateHeuristic(successor, goal);

                // If the successor is not in the open list or has a lower cost, update it
                if (!openList.Contains(successor) || newCost + heuristic < successor.TotalCost) {
                    successor.cost = newCost;
                    successor.heuristic = heuristic;
                    successor.parent = current;

                    if (!openList.Contains(successor)) {
                        openList.Add(successor);
                    }
                }
            }
        }

        // No path found
        return new List<Node>();
    }

    // Generate successor nodes
    List<Node> GenerateSuccessors(Node node) {
        List<Node> successors = new List<Node>();

        // Define possible moves (up, down, left, right)
        int[] dx = { -1, 1, 0, 0 };
        int[] dy = { 0, 0, -1, 1 };

        for (int i = 0; i < 4; i++) {
            int newX = node.x + dx[i];
            int newY = node.y + dy[i];

            // Check if the new position is within the labyrinth bounds and is a valid path (not a wall)
            if (newX >= 0 && newX < mazeMap.GetLength(0) && newY >= 0 && newY < mazeMap.GetLength(1) && mazeMap[newX, newY] == 0) {
                successors.Add(new Node(newX, newY));
            }
        }

        return successors;
    }

    // Calculate heuristic value (Euclidean distance)
    int CalculateHeuristic(Node node, Node goal) {
        return Mathf.Abs(node.x - goal.x) + Mathf.Abs(node.y - goal.y);
    }

    // Reconstruct the path from the goal node to the start node
    List<Node> ReconstructPath(Node goal) {
        List<Node> path = new List<Node>();
        Node current = goal;

        while (current != null) {
            path.Add(current);
            current = current.parent;
        }

        path.Reverse(); // Reverse the path to get it from start to goal
        return path;
    }

    private class Node {
        public int x, y;
        public int cost;
        public int heuristic; // heuristic value (h(n))
        public Node parent;

        public Node(int x, int y) {
            this.x = x;
            this.y = y;
        }

        public int TotalCost {
            get { return cost + heuristic; }
        }
    }
}
