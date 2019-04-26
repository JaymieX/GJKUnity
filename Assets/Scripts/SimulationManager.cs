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
                // Physics res
                float c = 0.5f;

                PhysicsSim compA = _state.GetPolytopeA.GetComponent<PhysicsSim>();
                PhysicsSim compB = _state.GetPolytopeB.GetComponent<PhysicsSim>();

                compA.velocity = UIController.initVelocutyA;
                compB.velocity = UIController.initVelocutyB;

                float massTotal = compA.mass + compB.mass;

                Vector3 newVelocityA =
                    (c * compB.mass) * (compB.velocity - compA.velocity) +
                    (compA.mass * compA.velocity) +
                    (compB.mass * compB.velocity);
                newVelocityA.x /= massTotal;
                newVelocityA.y /= massTotal;
                newVelocityA.z /= massTotal;

                Vector3 newVelocityB =
                    (c * compA.mass) * (compA.velocity - compB.velocity) +
                    (compA.mass * compA.velocity) +
                    (compB.mass * compB.velocity);
                newVelocityB.x /= massTotal;
                newVelocityB.y /= massTotal;
                newVelocityB.z /= massTotal;

                compA.velocity = newVelocityA;
                compB.velocity = newVelocityB;

                // Angular
                float vr = Vector3.Dot(_state.epaData.Normal, (compA.velocity - compB.velocity));

                // Contacts
                Vector3 cpA = compA.transform.position + (_state.epaData.Normal * _state.epaData.Depth);
                Vector3 cpB = compB.transform.position + (-_state.epaData.Normal * _state.epaData.Depth);

                float j =
            -vr * (c + 1f) /
            (1f / compA.mass + 1f / compB.mass) +
            HelpFunction(_state.epaData.Normal, 300, cpA) +
            HelpFunction(_state.epaData.Normal, 500, cpB);

                Vector3 jn = _state.epaData.Normal * j;

                compA.AngularVelocity = compA.velocity + (Vector3.Cross(cpA, jn) / 300);
                compB.AngularVelocity = compB.velocity + (Vector3.Cross(cpB, -jn) / 500);

                _state = null;
            }
        }
    }

    private float HelpFunction(Vector3 normal, float i, Vector3 r)
    {
        return Vector3.Dot
            (
            normal,
            Vector3.Cross((Vector3.Cross(r, normal) / i), r)
            );
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
                    // Debug.Log("Draw line " + i + " " + j);
                }
            }
        }

        if (_state != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(Vector3.zero, _state.LastDirection);

            if (_state.MiscDebugLines.Count != 0)
            {
                Gizmos.color = Color.green;
                for (int i = 0; i < _state.MiscDebugLines.Count; i++)
                {
                    Gizmos.DrawLine(Vector3.zero, _state.MiscDebugLines[i]);
                }
            }
        }
    }

    public static void RenewState()
    {
        _state = null;
    }
}
