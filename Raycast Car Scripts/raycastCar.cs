using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEditor.Timeline.Actions;
using UnityEngine;
using TMPro;
using System;

public class raycastCar : MonoBehaviour
{
    //Settings up variables
    private Rigidbody rigidBody;
    private raycastWheel[] wheels;

    private float leftWheelAngle;
    private float rightWheelAngle;

    [Header("Control")]
    [SerializeField] private bool AI;
    public float steer { get; set; }
    public float throttle { get; set; }

    [Header("Inputs")]
    [SerializeField] private Transform centerofMass; //A gameobject to easily move the center of mass

    private float steerInput; // Inputs for steer and throttle, so that both a player and AI can access it
    private float verticalInput;
    
    [Header("Specification")]
    [SerializeField] private float wheelBase; //Wheel base is the distance between the front and rear axcels
    [SerializeField] private float turningCircle; //This is the distance from the back wheel out to the bisect of the front wheel at full lock
    [SerializeField] private float rearTrack; // Distance to the centre of the rear axcel from the hub
    public AnimationCurve sidewaysGripCurve; // A visual method of creating a friction curve for the cars grip
    public float enginePowerMultiplier; // A numeric value for the power of the engine

    [Header("UI")]
    [SerializeField] private TMP_Text rpmField; //All to be fed to the UI
    [SerializeField] private TMP_Text gearField;
    [SerializeField] private TMP_Text velocityField;

    private double currentVelocity; // Current velocity of the car
    public float gripRatio { get; set; } // Sideways/Forward velocity
    void Start()
    { // Identifying nessercary components 
        rigidBody = GetComponent<Rigidbody>();
        rigidBody.centerOfMass = centerofMass.localPosition;
        wheels = GetComponentsInChildren<raycastWheel>(); //Identifying wheels on the vehicle
    }

    void Update()
    {   // Call every frame
        UpdateVelocity();
        ApplySteering();
        ApplyStatsToUI();
        ApplyThrottle();
        UpdateGrip();

    }

    private void ApplyStatsToUI() { //Populates the UI with new values 
        rpmField.text = "--";
        if (wheels[0].rpm < 0) {
            gearField.text = "R";
            velocityField.text = "-" + currentVelocity.ToString();
        } else {
            gearField.text = "D";
            velocityField.text = currentVelocity.ToString();
        }
    }

    public bool checkGrassage() { // Checks if the car has more than one wheel on the grass
        int grassage = 0;
        foreach (raycastWheel wheel in wheels) { //Looping through the wheels
            if (wheel.IsGrounded) {
                if (wheel.GetMaterial == "Grass") {
                    grassage++; //keeping runnning total of the ones on the grass
                }
            }
        }
        if (grassage > 1) {
            return true; 
        } else { return false; }
    }
    private void ApplySteering() { //Applying the steering volume to the car with Aackerman steering
        if (AI) {
            steerInput = steer;
        } else {
            steerInput = Input.GetAxis("Horizontal");
        }
        // Checking for right or left and calculating both angles
        if (steerInput > 0) { // Right
            leftWheelAngle = Mathf.Rad2Deg * Mathf.Atan(wheelBase / (turningCircle + (rearTrack / 2)));
            rightWheelAngle = Mathf.Rad2Deg * Mathf.Atan(wheelBase / (turningCircle - (rearTrack / 2)));
        } else if (steerInput < 0) { // Left
            leftWheelAngle = -(Mathf.Rad2Deg * Mathf.Atan(wheelBase / (turningCircle - (rearTrack / 2))));
            rightWheelAngle = -(Mathf.Rad2Deg * Mathf.Atan(wheelBase / (turningCircle + (rearTrack / 2))));
        } else {// No input
            leftWheelAngle = 0;
            rightWheelAngle = 0;
        }
        // Applying the steering to wheels that have the steering quality
        foreach (raycastWheel wheel in wheels) {
            if (wheel.left && wheel.steering) {
                wheel.steerAngle = leftWheelAngle;
            } else if (wheel.right && wheel.steering) {
                wheel.steerAngle = rightWheelAngle;
            }

        }
    }

    public void ApplyThrottle() { // Applying throttle input to the car
        if (AI) {
            verticalInput = throttle;
        } else {
            verticalInput = Input.GetAxis("Vertical");
        }
        
        foreach (raycastWheel wheel in wheels) { // Applying throttle to the wheels with the driven quality
            if (wheel.driven) { wheel.verticalInput = verticalInput; }
        }
        }



    private void UpdateVelocity() { //Updating the velocity of the car in mph
        Vector3 velocityVector = rigidBody.velocity;
        currentVelocity = Math.Truncate(2.2369 * velocityVector.magnitude);
    }

    public float AgentVelocity() { //giving the raw velocity to the agent
        Vector3 currentForce = transform.InverseTransformDirection(rigidBody.velocity);
        return currentForce.x;
    }

    public void ApplyForce(Vector3 force, Vector3 position) { //A method for other scripts to add force to the car
        rigidBody.AddForceAtPosition(force, position);
    }

    private void UpdateGrip() { // Updates the grip ratio of the car
        Vector3 currentForce = transform.InverseTransformDirection(rigidBody.velocity);
        float currentForceRatio = Math.Abs(currentForce.x/currentForce.z);
        

        gripRatio = Mathf.Lerp(1- sidewaysGripCurve.Evaluate(currentForceRatio), 0, Time.deltaTime*30);
    }
}
