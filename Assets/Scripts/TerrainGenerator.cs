using System.Collections;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;


public class TerrainGenerator : MonoBehaviour
{
    [Header("Elements: ")]
    [SerializeField] MeshFilter filter;

    [Header("Brush Settings: ")]
    [SerializeField] int brushRadius;
    [SerializeField] float brushStrength;
    [SerializeField] float brushFallback;

    [Header("Data: ")]
    [SerializeField] int gridSize;
    [SerializeField] float gridScale;
    [SerializeField] float isoValue;

    List<Vector3> vertices = new List<Vector3>();
    List<int> triangles = new List<int>();

    SquareGrid squareGrid;
    float[,] grid;

    void Awake()
    {
        InputManager.onTouching += TouchingCallBack;
    }

    // Start is called before the first frame update
    void Start()
    {
        grid = new float[gridSize, gridSize];

        for (int y = 0; y < gridSize; y++)
            for (int x = 0; x < gridSize; x++)
                grid[x, y] = isoValue + .1f;

        squareGrid = new SquareGrid(gridSize - 1, gridScale, isoValue);

        GenerateMesh();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void TouchingCallBack(Vector3 worldPosition)
    {
        Debug.Log(worldPosition);

        worldPosition.z = 0;

        Vector2Int gridPosition = GetGridPositionFromWorldPosition(worldPosition);

        for (int y = gridPosition.y - brushRadius; y <= gridPosition.y + brushRadius; y++)
        {
            for (int x = gridPosition.x - brushRadius; x <= gridPosition.x + brushRadius; x++)
            {
                Vector2Int currentGridPosition = new Vector2Int(x, y);

                if (!isValidGridPosition(currentGridPosition))
                {
                    Debug.LogError("Invalid position");
                    continue;
                }

                float distance = Vector2.Distance(currentGridPosition, gridPosition);
                float factor = brushStrength * Mathf.Exp(-distance * brushFallback / brushRadius);

                grid[currentGridPosition.x, currentGridPosition.y] -= factor;
            }
        }

        GenerateMesh();
    }

    void GenerateMesh()
    {
        Mesh mesh = new Mesh();

        vertices.Clear();
        triangles.Clear();

        squareGrid.Update(grid);

        //Square square = new Square(Vector3.zero, gridScale);
        //square.Triangulate(isoValue, new float[] { topRightValue, bottomRightValue, bottomLeftValue, topLeftValue });

        mesh.vertices = squareGrid.GetVertices();
        mesh.triangles = squareGrid.GetTriangles();

        filter.mesh = mesh;

    }
    bool isValidGridPosition(Vector2Int gridPosition)
    {
        return gridPosition.x >= 0 && gridPosition.x < gridSize && gridPosition.y >= 0 && gridPosition.y < gridSize;
    }

    Vector2 GetWorldPositionFromGridPosition(int x, int y)
    {
        Vector2 worldPosition = new Vector2(x, y) * gridScale;

        worldPosition.x -= (gridSize * gridScale) / 2f - gridScale / 2f;
        worldPosition.y -= (gridSize * gridScale) / 2f - gridScale / 2f;

        return worldPosition;
    }

    Vector2Int GetGridPositionFromWorldPosition(Vector2 worldPosition)
    {
        Vector2Int gridPosition = new Vector2Int();

        gridPosition.x = Mathf.FloorToInt(worldPosition.x / gridScale + gridSize / 2f - gridScale / 2f);
        gridPosition.y = Mathf.FloorToInt(worldPosition.y / gridScale + gridSize / 2f - gridScale / 2f);


        return gridPosition;
    }

#if UNITY_EDITOR

    void OnDrawGizmos() 
    {
        if(!EditorApplication.isPlaying) return;

        Gizmos.color = Color.red;

        for (int y = 0; y < grid.GetLength(1); y++) 
        {
            for (int x = 0; x < grid.GetLength(0); x++) 
            { 
                //Vector2 gridPosition = new Vector2(x, y);
                Vector2 worldPosition = GetWorldPositionFromGridPosition(x, y);

                Gizmos.DrawSphere(worldPosition, gridScale / 4f);

                Handles.Label(worldPosition + Vector2.up * gridScale / 3f, grid[x, y].ToString());
            }
        }
    }

#endif
}


