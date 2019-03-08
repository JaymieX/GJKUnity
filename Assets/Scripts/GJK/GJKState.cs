using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Simplex
{
    private LinkedList<Vector3> _vertices;

    public Simplex()
    {
        _vertices = new LinkedList<Vector3>();
    }

    public void Push(Vector3 vertex)
    {
        _vertices.AddLast(vertex);
    }

    public Vector3 PeekFront()
    {
        return _vertices.First.Value;
    }

    public Vector3 PeekBack()
    {
        return _vertices.Last.Value;
    }

    public Vector3 PopFront()
    {
        Vector3 result = PeekFront();
        _vertices.RemoveFirst();

        return result;
    }

    public Vector3 PopBack()
    {
        Vector3 result = PeekBack();
        _vertices.RemoveLast();

        return result;
    }

    public Vector3 PeekAt(int index)
    {
        return _vertices.ElementAt(index);
    }

    public int GetSize()
    {
        return _vertices.Count;
    }
}

public class GJKState
{
    /**
     * \property    public bool IsCollided
     *
     * \brief   Gets or sets a value indicating whether this object is collided
     *
     * \returns True if this object is collided, false if not.
     */
    public bool IsCollided { get; set; }

    /**
     * \property    public bool FinishRun
     *
     * \brief   Gets if the algorithm has finish running
     *
     * \returns True if finish run, false if not.
     */
    public bool FinishRun { get; set; }

    /**
     * \property    public int Iteration
     *
     * \brief   Gets the GJK iteration
     *
     * \returns The iteration.
     */
    public int Iteration { get; set; }

    public Vector3 LastDirection { get; set; }

    public Simplex CurrentSimplex { get; set; }

    private Polytope _polytopeA, _polytopeB;
    public Polytope GetPolytopeA { get { return _polytopeA; } }
    public Polytope GetPolytopeB { get { return _polytopeB; } }

    /**
     * \fn  public GJKState(ref Polytope polytopeA, ref Polytope polytopeB)
     *
     * \brief   Constructor
     *
     * \author  Jaymie
     * \date    3/4/2019
     *
     * \param [in]  polytopeA   The polytope a.
     * \param [in]  polytopeB   The polytope b.
     */
    public GJKState(ref Polytope polytopeA, ref Polytope polytopeB)
    {
        IsCollided = false;
        FinishRun = false;
        Iteration = 0;

        _polytopeA = polytopeA;
        _polytopeB = polytopeB;

        CurrentSimplex = new Simplex();
    }
}
