using UnityEngine;

public class WorldGenerator : MonoBehaviour {
    public GameObject cube;
    public int width, height, large;
    public float detail, seed;


    void Start() {
        GenerateMap();
    }

    void Update() {

    }

    void GenerateMap() {
        for (int x = 0; x < width; x++)
            for (int z = 0; z < large; z++) {
                height = (int)(Mathf.PerlinNoise((x / 2 + seed) / detail, (z / 2 + seed) / detail) * detail);
                for (int y = 0; y < height; y++)
                    Instantiate(cube, new Vector3(x, y, z), Quaternion.identity);
            }
    }
}