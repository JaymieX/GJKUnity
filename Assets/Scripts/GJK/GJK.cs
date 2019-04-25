using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GJK
{
    private static Vector3 SupportFunction(Vector3 direction, Polytope polytopeA, Polytope polytopeB)
    {
        Vector3 tempPointA = polytopeA.GetFurthestPoint(direction);
        Vector3 tempPointB = polytopeB.GetFurthestPoint(-direction);
        return tempPointA + -tempPointB;
    }

    private static Edge FindClosestEdge(Vector3 origin, ref GJKState state)
    {
        List<Edge> edges = new List<Edge>();


        for (int i = 0; i < state.CurrentSimplex.GetSize(); i++)
        {
            Vector3 a = state.CurrentSimplex.PeekAt(i);
            Vector3 b;

            if (i + 1 != state.CurrentSimplex.GetSize())
            {
                b = state.CurrentSimplex.PeekAt(i + 1);
            }
            else
            {
                b = state.CurrentSimplex.PeekAt(0);
            }

            edges.Add(new Edge(a, b, origin, i));
        }

        edges.Sort();

        return edges[0];
    }

    private static void ProcessSimplex(ref GJKState state)
    {
        // Line
        if (state.CurrentSimplex.GetSize() == 2)
        {
            Vector3 pointA = state.CurrentSimplex.PeekFront();
            Vector3 pointB = state.CurrentSimplex.PeekBack();

            // Get line direction
            // Since A to B was last used
            Vector3 directionBToA = pointA - pointB;
            // Get B to origin
            Vector3 directionBToO = new Vector3(0f, 0f, 0f) - pointB;

            // Check if they B to A is in same direction as B to origin
            // So it usually means the origin is in between
            if (directionBToA.IsInSameDirection(directionBToO))
            {
                // Triple cross product
                state.LastDirection = Vector3.Cross(Vector3.Cross(directionBToA, directionBToO), directionBToA);

                // Add third point
                state.CurrentSimplex.Push(SupportFunction(state.LastDirection, state.GetPolytopeA, state.GetPolytopeB));
            }
            else // Second point is shit
            {
                // Drop oldest point and try again with direction B to origin
                state.LastDirection = directionBToO;
                state.CurrentSimplex.PopFront();
            }
        }
        // Triangle
        else if (state.CurrentSimplex.GetSize() == 3)
        {
            // Clear lines
            state.MiscDebugLines.Clear();

            // Get the triangle's normal
            Vector3 pointA = state.CurrentSimplex.PeekAt(0); // Oldest
            Vector3 pointB = state.CurrentSimplex.PeekAt(1);
            Vector3 pointC = state.CurrentSimplex.PeekAt(2); // Newest

            Vector3 trigNormal = Vector3.Cross(pointA - pointC, pointB - pointC);

            // Get C to origin
            Vector3 directionCToO = new Vector3(0f, 0f, 0f) - pointC;

            // Get new direction
            Vector3 newDirection;

            // Check if we got the right direction for normal
            if (trigNormal.IsInOppositeDirection(directionCToO))
            {
                // Negate normal
                //newDirection = Vector3.Cross(Vector3.Cross(-trigNormal, directionCToO), -trigNormal);
                newDirection = -trigNormal;
            }
            else
            {
                //newDirection = Vector3.Cross(Vector3.Cross(trigNormal, directionCToO), trigNormal);
                newDirection = trigNormal;
            }

            state.LastDirection = newDirection.normalized;

            // Add 4th point
            state.CurrentSimplex.Push(SupportFunction(state.LastDirection, state.GetPolytopeA, state.GetPolytopeB));
        }
        // Tetrahedron
        else
        {
            Vector3 origin = Vector3.zero;

            Vector3 point0 = state.CurrentSimplex.PeekAt(0);
            Vector3 point1 = state.CurrentSimplex.PeekAt(1);
            Vector3 point2 = state.CurrentSimplex.PeekAt(2);
            Vector3 point3 = state.CurrentSimplex.PeekAt(3);

            Matrix4x4 d0 = new Matrix4x4();
            Matrix4x4 d1 = new Matrix4x4();
            Matrix4x4 d2 = new Matrix4x4();
            Matrix4x4 d3 = new Matrix4x4();
            Matrix4x4 d4 = new Matrix4x4();

            d0.SetRow(0, new Vector4(point0.x, point0.y, point0.z, 1f));
            d0.SetRow(1, new Vector4(point1.x, point1.y, point1.z, 1f));
            d0.SetRow(2, new Vector4(point2.x, point2.y, point2.z, 1f));
            d0.SetRow(3, new Vector4(point3.x, point3.y, point3.z, 1f));

            d1.SetRow(0, new Vector4(origin.x, origin.y, origin.z, 1f));
            d1.SetRow(1, new Vector4(point1.x, point1.y, point1.z, 1f));
            d1.SetRow(2, new Vector4(point2.x, point2.y, point2.z, 1f));
            d1.SetRow(3, new Vector4(point3.x, point3.y, point3.z, 1f));

            d2.SetRow(0, new Vector4(point0.x, point0.y, point0.z, 1f));
            d2.SetRow(1, new Vector4(origin.x, origin.y, origin.z, 1f));
            d2.SetRow(2, new Vector4(point2.x, point2.y, point2.z, 1f));
            d2.SetRow(3, new Vector4(point3.x, point3.y, point3.z, 1f));

            d3.SetRow(0, new Vector4(point0.x, point0.y, point0.z, 1f));
            d3.SetRow(1, new Vector4(point1.x, point1.y, point1.z, 1f));
            d3.SetRow(2, new Vector4(origin.x, origin.y, origin.z, 1f));
            d3.SetRow(3, new Vector4(point3.x, point3.y, point3.z, 1f));

            d4.SetRow(0, new Vector4(point0.x, point0.y, point0.z, 1f));
            d4.SetRow(1, new Vector4(point1.x, point1.y, point1.z, 1f));
            d4.SetRow(2, new Vector4(point2.x, point2.y, point2.z, 1f));
            d4.SetRow(3, new Vector4(origin.x, origin.y, origin.z, 1f));

            // Determinants
            float det0 = d0.determinant;
            float det1 = d1.determinant;
            float det2 = d2.determinant;
            float det3 = d3.determinant;
            float det4 = d4.determinant;

            // Degenerate simplex :(
            if (det0 == 0f)
            {
                state.CurrentSimplex.PopBack();
            }
            // Check if all five det have same sign
            else if (Mathf.Sign(det0) == Mathf.Sign(det1)
                && Mathf.Sign(det1) == Mathf.Sign(det2)
                && Mathf.Sign(det2) == Mathf.Sign(det3)
                && Mathf.Sign(det3) == Mathf.Sign(det4))
            {
                state.FinishRun = true;
                Debug.Log("Collided.");

                EPA(ref state);
            }
            // Drop point 0
            else if (Mathf.Sign(det0) != Mathf.Sign(det1))
            {
                state.CurrentSimplex.RemoveAt(0);
            }
            // Drop point 1
            else if (Mathf.Sign(det0) != Mathf.Sign(det2))
            {
                state.CurrentSimplex.RemoveAt(1);
            }
            // Drop point 2
            else if (Mathf.Sign(det0) != Mathf.Sign(det3))
            {
                state.CurrentSimplex.RemoveAt(2);
            }
            // Drop point 3
            else if (Mathf.Sign(det0) != Mathf.Sign(det4))
            {
                state.CurrentSimplex.RemoveAt(3);
            }
        }
    }

    private static EPAData EPA(ref GJKState state)
    {
        Vector3 origin = Vector3.zero - state.CurrentSimplex.PeekBack();
        
        while(true)
        {
            Edge e = FindClosestEdge(origin, ref state);
            Vector3 supportP = SupportFunction(e.EdgeNormal, state.GetPolytopeA, state.GetPolytopeB);

            float d = Vector3.Dot(supportP, e.EdgeNormal);
            if (d - e.Distance < 0.000001)
            {
                EPAData data = new EPAData();
                data.Normal = e.EdgeNormal;
                data.Depth = d;

                return data;
            }
            else
            {
                state.CurrentSimplex.Insert(e.PointA, supportP);
            }
        }
    }

    public static void Collided(ref GJKState state)
    {
        Polytope polytopeA = state.GetPolytopeA;
        Polytope polytopeB = state.GetPolytopeB;

        // More than 20 it
        if (state.Iteration > 20)
        {
            state.FinishRun = true;
            state.IsCollided = false;
        }
        else
        {
            // First GJK run
            if (state.CurrentSimplex.GetSize() == 0)
            {
                // Add the initial point
                state.CurrentSimplex.Push(SupportFunction(polytopeB.GetCentre() - polytopeA.GetCentre(), polytopeA, polytopeB));

                // Get a search direction from first point to origin
                if (state.LastDirection == Vector3.zero)
                {
                    Vector3 directionAToO = new Vector3(0f, 0f, 0f) - state.CurrentSimplex.PeekBack();
                    state.LastDirection = directionAToO;
                }
            }
            // Second GJK run
            else if (state.CurrentSimplex.GetSize() == 1)
            {
                // Add next point
                state.CurrentSimplex.Push(SupportFunction(state.LastDirection, polytopeA, polytopeB));

                // Check if this point passes origin
                if (state.CurrentSimplex.PeekBack().IsInOppositeDirection(state.LastDirection))
                {
                    // This object is definably not colliding as the second point in the direction of 
                    // origin is not even passing it
                    state.FinishRun = true;
                    state.IsCollided = false;
                }
            }
            else // Begin it
            {
                ProcessSimplex(ref state);
            }
        }

        state.Iteration++;
    }
}
