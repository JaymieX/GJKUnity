using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public float Speed;
    public float RotSpeed;

    private float _yaw = 0f, _pitch = 0f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.A))
        {
            transform.position += -transform.right * Speed * Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.D))
        {
            transform.position += transform.right * Speed * Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.W))
        {
            transform.position += transform.forward * Speed * Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.S))
        {
            transform.position += -transform.forward * Speed * Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.Q))
        {
            _pitch -= RotSpeed * Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.E))
        {
            _pitch += RotSpeed * Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.R))
        {
            _yaw -= RotSpeed * Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.F))
        {
            _yaw += RotSpeed * Time.deltaTime;
        }

        transform.rotation = Quaternion.Euler(_yaw, _pitch, 0f);
    }
}
