using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * \class   Polytope
 *
 * \brief   A polytope component.
 *
 * \author  Jaymie
 * \date    3/4/2019
 */
[RequireComponent(typeof(MeshFilter))]
public class Polytope : MonoBehaviour
{
    private List<Vector3> _vertices; // All vertices in this mesh

    // Start is called before the first frame update
    private void Start()
    {
        // Adding all vertices
        _vertices = new List<Vector3>();
        Mesh mesh = GetComponent<MeshFilter>().mesh;
        foreach (Vector3 vertex in mesh.vertices)
        {
            _vertices.Add(vertex);
        }
    }

    /**
     * \fn  public List<Vector3> GetWorldPositionVertices()
     *
     * \brief   Gets world position vertices
     *
     * \author  Jaymie
     * \date    3/4/2019
     *
     * \returns The world position vertices.
     */
    public List<Vector3> GetWorldPositionVertices()
    {
        // Transforms vertex from local to world space
        List<Vector3> wordPosVertices = new List<Vector3>();
        foreach (Vector3 vertex in _vertices)
        {
            wordPosVertices.Add(this.transform.TransformPoint(vertex));
        }

        return wordPosVertices;
    }

    public Vector3 GetFurthestPoint(Vector3 direction)
    {
        // Check each vertex
        List<Vector3> Vertices = GetWorldPositionVertices();

        float lastFurthest = Vector3.Dot(Vertices[0], direction);
        Vector3 result = Vertices[0];

        for (int i = 1; i < Vertices.Count; i++)
        {
            float thisPointResult = Vector3.Dot(Vertices[i], direction);
            if (thisPointResult > lastFurthest)
            {
                // New furthest found
                lastFurthest = thisPointResult;
                result = Vertices[i];
            }
        }

        return result;
    }

    public Vector3 GetCentre()
    {
        return transform.position;
    }
}
