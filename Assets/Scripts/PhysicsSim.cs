using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsSim : MonoBehaviour
{
    public Vector3 velocity;
    public Vector3 AngularVelocity;

    public float mass;

    // Start is called before the first frame update
    void Start()
    {
        velocity = new Vector3();
        AngularVelocity = new Vector3();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        gameObject.transform.position += velocity * Time.fixedDeltaTime;
        gameObject.transform.Rotate(AngularVelocity);
    }
}
