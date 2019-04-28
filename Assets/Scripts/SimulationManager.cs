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

                // Contacts
                Vector3 lpA = _state.CurrentSimplex.PopBack();
                Vector3 lpB = _state.CurrentSimplex.PopBack();

                Vector3 cpA = Vector3.Lerp(lpA, lpB, 0.5f);
                Vector3 cpB = cpA;

                // Inert
                float invVal = 1f / 0.16f;

                Matrix4x4 rotInert = Matrix4x4.identity;
                rotInert.SetRow(0, new Vector4(invVal, 0f, 0f, 0f));
                rotInert.SetRow(1, new Vector4(0f, invVal, 0f, 0f));
                rotInert.SetRow(2, new Vector4(0f, 0f, 0f, invVal));

                float j =
                    (-(1f + c) * Vector3.Dot(compA.velocity - compB.velocity, _state.epaData.Normal)) /
                    ((1f / compA.mass + 1f / compB.mass) +
                    Vector3.Dot(
                        Vector3.Cross(rotInert * Vector3.Cross(cpA, _state.epaData.Normal), cpA) +
                        Vector3.Cross(rotInert * Vector3.Cross(cpB, _state.epaData.Normal), cpB),
                        _state.epaData.Normal
                        ));

                Vector4 wa = rotInert *
                    (
                    Vector3.Cross(
                        cpA,
                        j * _state.epaData.Normal
                        )
                    );

                compA.AngularVelocity.x += wa.x;
                compA.AngularVelocity.y += wa.y;
                compA.AngularVelocity.z += wa.z;

                Vector4 wb = rotInert *
                    (
                    Vector3.Cross(
                        cpB,
                        j * _state.epaData.Normal
                        )
                    );

                compB.AngularVelocity.x -= wb.x;
                compB.AngularVelocity.y -= wb.y;
                compB.AngularVelocity.z -= wb.z;

                compA.velocity += ((j / compA.mass) * _state.epaData.Normal);

                compB.velocity += ((j / compB.mass) * -_state.epaData.Normal);

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
