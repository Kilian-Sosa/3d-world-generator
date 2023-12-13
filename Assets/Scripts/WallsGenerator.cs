using UnityEngine;

public class WallsGenerator : ScriptableObject {
    private GameObject wallPrefab;
    private int width, height, large;
    private float detail, seed;

    public void Generate(GameObject wallPrefab, int width, int height, int large, float detail, float seed) {
        this.wallPrefab = wallPrefab;
        this.width = width;
        this.height = height;
        this.large = large;
        this.detail = detail;
        this.seed = seed;
        GenerateWalls();
    }

    void GenerateWalls() {
        int [,] mazeMap = MazeGenerator.maze;
        for (int x = 0; x < width; x++)
            for (int z = 0; z < large; z++) {
                if (mazeMap[x, z] == 0) continue; 
                height = (int)(Mathf.PerlinNoise((x / 2 + seed) / detail, (z / 2 + seed) / detail) * detail);
                for (int y = 0; y < height; y++)
                    Instantiate(wallPrefab, new Vector3(x, y, z), Quaternion.identity);
            }
    }
}