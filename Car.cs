using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Car : MonoBehaviour
{
    [SerializeField] private float motorTorque = 400f;
    [SerializeField] private float maxSteer = 30f;
    private Rigidbody rigidBody;
    [SerializeField] private Transform centerOfMass;

    public float steer { get; set; }
    public float throttle { get; set; }

    private Wheel[] wheels;

    private void Start() {
        wheels = GetComponentsInChildren<Wheel>();
        rigidBody = GetComponent<Rigidbody>();
        rigidBody.centerOfMass = centerOfMass.localPosition;
    }

    public void stopWheels() {
        foreach (var wheel in wheels) {
            wheel.stopWheel();
        }
    }

    private void Update() {

        foreach (var wheel in wheels) {
            wheel.steerAngle = steer * maxSteer;
            wheel.torque = throttle * motorTorque;
        }
    }
}
