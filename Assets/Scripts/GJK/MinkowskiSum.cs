using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinkowskiSum
{
    public static List<Vector3> CalcMinkowskiSum(Polytope a, Polytope b)
    {
        List<Vector3> points = new List<Vector3>();

        // Get vertices
        List<Vector3> verticesA = a.GetWorldPositionVertices();
        List<Vector3> verticesB = b.GetWorldPositionVertices();

        foreach (Vector3 vertexA in verticesA)
        {
            foreach (Vector3 vertexB in verticesB)
            {
                // A + -B
                points.Add(vertexA + (-vertexB));
            }
        }

        return points;
    }
}
