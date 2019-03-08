using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangeMover : MonoBehaviour
{
    public Vector3 MaxRange;
    public float Speed;
    public Vector3 EulerAngles;

    private Vector3 _startingRange;
    private Vector3 _currentGoalPoint;
    private bool _goalPointIsStartingPoint;

    // Start is called before the first frame update
    void Start()
    {
        _startingRange = transform.position;
        _currentGoalPoint = MaxRange;
        _goalPointIsStartingPoint = false;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.position = Vector3.MoveTowards(transform.position, _currentGoalPoint, Speed * Time.fixedDeltaTime);
        if (Mathf.Abs(Vector3.Distance(transform.position, _currentGoalPoint)) < 0.1f)
        {
            _goalPointIsStartingPoint = !_goalPointIsStartingPoint;
            _currentGoalPoint = _goalPointIsStartingPoint ? MaxRange : _startingRange;
        }

        transform.Rotate(EulerAngles);
    }
}
