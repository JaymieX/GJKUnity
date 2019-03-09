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
            // Next assignment
            state.FinishRun = true;
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
