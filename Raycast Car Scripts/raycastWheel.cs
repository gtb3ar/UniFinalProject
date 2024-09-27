using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem.Android;

public class raycastWheel : MonoBehaviour
{

    //Properties
    // Recieveing inputs from the car
    public float verticalInput { get; set; }
    public float steerAngle { get; set; }
    //Holds the rotations per minute of the wheels 
    public float rpm { get; set; }
    
    //Parent
    private Rigidbody rigidBody;
    private raycastCar body;
    //Child
    private Transform wheelMesh;
    //Misc
    private Vector3 suspensionForce;

    [Header("Identity")] //Gives the wheel the right qualitys so it is delivered the correct input
    public bool left;
    public bool right;
    public bool steering;
    public bool driven;
    [SerializeField] private float rpmLimiter;
    

    [Header("Wheel")] 
    [SerializeField] private float wheelRadius; 

    private float wheelCircumference;
    private Vector3 wheelVelocity;
    private float sidewaysForce; //To be applied
    private float forwardForce;
    private float counterForce;

    [Header("Spring")]
    [SerializeField] private float springRestLength; // The distance at which the spring should naturally sit
    [SerializeField] private float springExtensionConstant; //The distance at which the spring should extend to
    [SerializeField] private float springStrength; //A numeric was to decide the power of the spring

    [SerializeField] private float compressionAtRest; //Hold the value for the spring compression percentage when the car is stationary
    public float compressiveGrip; //Current compression/Compression at rest

    private float springMaxExtension; // Max spring length
    private float springMinExtension; // Min spring length
    private float springLastFrameLength; //The previous frames spring length
    private float springExtension; // The springs current extension
    private float springForce; // The force applied back to the car
    private float springVelocity; // The velocity of the tip of the spring 
    private float springMultiplier = 10000f; // A easy way to bulk strengthen the spring unseen to hte editor

    [Header("Shock Absorber")]
    [SerializeField] private float shockAbsorberStrength; // A numeric way to decide on the power of the shock absorber

    private float shockAbsorberForce; // The opposing force applied to the car countering the spring
    private float shockAbsorberMultiplier = 1000f; // An easy way to bulk strengthen the spring unseen to the editor

    [Header("Steering")] 
    [SerializeField] private float timeToSteer; //A numeric respresentation of the time taken to reach full lock on one side to the other

    
    private float wheelAngle; // The current wheel angle
    private bool grounded = false; // A value that says whether the vehicle is touching the ground
    private string material; // A string representing the material the car is grounded on 


    public bool IsGrounded { 
        get { return grounded; }
    }

    public string GetMaterial {
        get { return material; }
    }

    private void Start() { // Identify the car and the mesh for the wheel
        rigidBody = GetComponentInParent<Rigidbody>();
        body = GetComponentInParent<raycastCar>();
        wheelMesh = GetComponentInChildren<MeshRenderer>().GetComponent<Transform>();
        springMinExtension = springRestLength - springExtensionConstant;
        springMaxExtension = springRestLength+ springExtensionConstant;
    }

    private void FixedUpdate() {
        Vector3 up = transform.TransformDirection(Vector3.up);
        grounded = false; // reset the grounded value

        if (Physics.Raycast(transform.position, -up, out RaycastHit hit, springMaxExtension +  wheelRadius) ) { //Sending a raycast to the floor at the max length of the spring + the wheel

            grounded = true; // It hit the ground, so ground = true
            material = hit.collider.gameObject.name; // It hit the ground, so the material is the namme of the object

            //Storing the previous spring length, and caluclating the current spring length
            springLastFrameLength = springExtension;
            springExtension = hit.distance - wheelRadius;
            springExtension = Mathf.Clamp(springExtension, springMinExtension, springMaxExtension);  //Making sure the spring extension is within the max and minimum

            // Calculating the compression percentage
            float compression = -(springExtension / springMaxExtension)+1; 

            // Applying force based of the compression of the spring
            springForce = springStrength * springMultiplier * compression;

            //Calculating spring velocity (V = D/T), then shock force
            springVelocity = (springLastFrameLength - springExtension) / Time.fixedDeltaTime;
            shockAbsorberForce = shockAbsorberStrength * shockAbsorberMultiplier * springVelocity; //Force to counter the spring force

            wheelVelocity = transform.InverseTransformDirection(rigidBody.GetPointVelocity(hit.point)); // Calculating the velocity of the wheel

            if (compression > 0 && compressionAtRest > 0) { // As long as the springs are compressed, creates the forceward force of the vehicle
                compressiveGrip = compression / compressionAtRest;
                forwardForce = verticalInput * body.enginePowerMultiplier * compressiveGrip;
                
            } else {
                forwardForce = verticalInput * body.enginePowerMultiplier;
            }

            sidewaysForce = (float)(wheelVelocity.x * springForce); // The force that will make the car drift or slide
            counterForce = (float)(sidewaysForce * body.gripRatio); // the force that will stop the car from drifting

            //Applying final force to the car
            suspensionForce = (springForce + shockAbsorberForce) * up;
            body.ApplyForce(suspensionForce + (sidewaysForce * -transform.right) + (counterForce * transform.right), hit.point);
            if (rpm < rpmLimiter) {
                body.ApplyForce((forwardForce * transform.forward), hit.point);
            }
            updateRpm(); // Updating the rotations per minute of the wheel
            
        }
        updateMesh(springExtension + wheelRadius); // Updating the mesh to the wheel
        body.ApplyForce(Vector3.down * (float)1000, transform.position); // Applying a little bit of downforce to compress the springs more (cheating)
        

    }

    private void Update() {
        wheelAngle = Mathf.Lerp(wheelAngle, steerAngle, timeToSteer * Time.deltaTime); //Smoothly interpolating between the steer angles
        transform.localRotation = Quaternion.Euler(Vector3.up * wheelAngle);

    }

    private void updateRpm() { // UPdate the roations per minute of the wheel
        rpm = wheelVelocity.z / ((wheelRadius * 2) * Mathf.PI);
    }

    private void updateMesh(float distance) { //Updating the matching of the wheel to its mesh
        wheelMesh.position = transform.position + -transform.up * (distance - wheelRadius);
        wheelMesh.transform.Rotate(rpm * 5, 0, 0);
    }
}
