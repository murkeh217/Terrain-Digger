using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SquareTester : MonoBehaviour
{  
    Vector2 topRight;
    Vector2 bottomRight;
    Vector2 bottomLeft;
    Vector2 topLeft;

    Vector2 topCenter;
    Vector2 rightCenter;
    Vector2 bottomCenter;
    Vector2 leftCenter;


    [Header("Settings: ")]
    [SerializeField] float gridScale;
    [SerializeField] float isoValue;
    List<Vector3> vertices = new List<Vector3>();
    List<int> triangles = new List<int>();    

    [Header("Elements: ")]
    [SerializeField] MeshFilter filter;

    [Header("Configuration: ")]
    [SerializeField] float topRightValue;
    [SerializeField] float bottomRightValue;
    [SerializeField] float bottomLeftValue;
    [SerializeField] float topLeftValue;

    void Start()
    {
        topRight = gridScale * Vector2.one / 2;
        bottomRight = topRight + gridScale * Vector2.down;
        bottomLeft = bottomRight + gridScale * Vector2.left;
        topLeft = bottomLeft + gridScale * Vector2.up;

        topCenter = topRight + gridScale / 2 * Vector2.left;
        rightCenter = bottomRight + gridScale / 2 * Vector2.up;
        bottomCenter = bottomLeft + gridScale / 2 * Vector2.right;
        leftCenter = topLeft + gridScale / 2 * Vector2.down;
    }

    // Update is called once per frame
    void Update()
    {
        Mesh mesh = new Mesh();

        vertices.Clear();
        triangles.Clear();

        Square square = new Square(Vector3.zero, gridScale);
        square.Triangulate(isoValue, new float[] { topRightValue, bottomRightValue, bottomLeftValue, topLeftValue });

        mesh.vertices = square.GetVertices();
        mesh.triangles = square.GetTriangles();

        filter.mesh = mesh;
    }

 
    private void OnDrawGizmos() 
    {
        Gizmos.color = Color.red;

        Gizmos.DrawSphere(topRight, gridScale / 8f);
        Gizmos.DrawSphere(bottomRight, gridScale / 8f);
        Gizmos.DrawSphere(bottomLeft, gridScale / 8f);
        Gizmos.DrawSphere(topLeft, gridScale / 8f);

        Gizmos.color = Color.green;

        Gizmos.DrawSphere(topCenter, gridScale / 16f);
        Gizmos.DrawSphere(rightCenter, gridScale / 16f);
        Gizmos.DrawSphere(bottomCenter, gridScale / 16f);
        Gizmos.DrawSphere(leftCenter, gridScale / 16f);



    }
}
