using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Edge : IComparable
{
    public Vector3 PointA, PointB;

    public float Distance;

    public Vector3 EdgeNormal;

    public int index;

    public Edge(Vector3 a, Vector3 b, Vector3 origin, int i)
    {
        PointA = a;
        PointB = b;

        index = i;

        // Line segment
        Vector3 lineSeg = PointB - PointA;

        // Normal
        EdgeNormal = Vector3.Cross(Vector3.Cross(lineSeg, origin), lineSeg);
        EdgeNormal = EdgeNormal.normalized;

        // Distance
        Distance = Vector3.Dot(origin, EdgeNormal);
    }

    public int CompareTo(object obj)
    {
        if (obj == null) return 1;

        Edge otherEdge = obj as Edge;
        if (otherEdge != null)
            return this.Distance.CompareTo(otherEdge.Distance);
        else
            throw new ArgumentException("Object is not a Edge");
    }
}
