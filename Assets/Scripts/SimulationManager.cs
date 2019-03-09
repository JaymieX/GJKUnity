using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimulationManager : MonoBehaviour
{
    public Polytope PolytopeA, PolytopeB;

    private List<Vector3> _minkowskisumPoints;
    private List<Vector3> _gjkSupportPoints;

    private static GJKState _state;

    public static Vector3 initDirection;

    // Start is called before the first frame update
    private void Awake()
    {
        _minkowskisumPoints = new List<Vector3>();
        _gjkSupportPoints = new List<Vector3>();
    }

    // Update is called once per frame
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (_state == null)
            {
                _state = new GJKState(ref PolytopeA, ref PolytopeB);
                _state.LastDirection = initDirection;
            }

            _minkowskisumPoints.Clear();
            _minkowskisumPoints = MinkowskiSum.CalcMinkowskiSum(PolytopeA, PolytopeB);

            _gjkSupportPoints.Clear();

            if (!_state.FinishRun)
            {
                GJK.Collided(ref _state);

                for (int i = 0; i < _state.CurrentSimplex.GetSize(); i++)
                {
                    _gjkSupportPoints.Add(_state.CurrentSimplex.PeekAt(i));
                }
            }
            else
            {
                _state = null;
            }
        }
    }

    private void OnDrawGizmos()
    {
        // Clear Gizmo
        ADScript.Gizmos.Clear();

        if (_minkowskisumPoints != null)
        {
            ADScript.Gizmos.color = Color.yellow;

            // Draw Minkowski Sum
            foreach (Vector3 vertex in _minkowskisumPoints)
            {
                ADScript.Gizmos.DrawSphere(vertex, 0.08f);
            }
        }

        ADScript.Gizmos.color = Color.green;

        if (_gjkSupportPoints != null)
        {
            foreach (Vector3 vertex in _gjkSupportPoints)
            {
                ADScript.Gizmos.DrawSphere(vertex, 0.15f);
            }

            ADScript.Gizmos.color = Color.blue;
            int count = _gjkSupportPoints.Count;
            for (int i = 0; i < count; i++)
            {
                for (int j = i + 1; j < count; j++)
                {
                    ADScript.Gizmos.DrawLine(_gjkSupportPoints[i], _gjkSupportPoints[j]);
                    Debug.Log("Draw line " + i + " " + j);
                }
            }
        }

        if (_state != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(Vector3.zero, _state.LastDirection);
        }
    }

    public static void RenewState()
    {
        _state = null;
    }
}
