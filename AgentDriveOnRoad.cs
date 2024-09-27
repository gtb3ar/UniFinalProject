using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using System.Net.NetworkInformation;
using System;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;

public class AgentDriveOnRoad : Agent { // This is the calss for the AI to have its agent

    private TileController TileGenerator; 
    private Vector3 lastlocalPosition;
    private raycastCar car;
    private Rigidbody rb;
    private checkpointTracker CPT;

    private int strikes;
    [SerializeField] private int strikeOut;


    private void Awake() { // Grabbing all the nessercary object to function
        TileGenerator = GetComponentInChildren<TileController>();
        car = GetComponent<raycastCar>();
        rb = GetComponent<Rigidbody>();
        CPT = GameObject.FindGameObjectWithTag("TileContainer").GetComponent<checkpointTracker>();
        strikes = 0;

    }
    private void Start() {
        lastlocalPosition = transform.localPosition;
        InvokeRepeating("grassageHold", 1f, 1f);
    }

    public override void OnEpisodeBegin() { // When the agent re/starts generate a new map and reset the cars postion
        TileGenerator.generateNewMap(); 
        resetCar();
        
    }

    public override void CollectObservations(VectorSensor sensor) { //The inputs the AI recieves from the enviroment
        sensor.AddObservation(transform.localPosition); // Position of the car
        sensor.AddObservation(transform.localRotation); // Rotation of the car
        sensor.AddObservation(car.AgentVelocity()); // Velocity of the car
        sensor.AddObservation(strikes);
        sensor.AddObservation(CPT.getNextCheckpointPosition());
    }

    public override void OnActionReceived(ActionBuffers actions) { // When the agent makes a desicion, this translates the agents desicion into movement
        float moveX = 0f; 
        float moveZ = 0f;

        switch (actions.DiscreteActions[0]) {
            case 0: moveX = 0f; break;
            case 1: moveX = 1f; break;
            case 2: moveX = -1f; break;
        }
        switch (actions.DiscreteActions[1]) {
            case 0: moveZ = 0f; break;
            case 1: moveZ = 1f; break;
            case 2: moveZ = -1f; break;
        }
        //Appling movement
        car.steer = moveX;
        car.throttle = moveZ;

    }

    private void OnTriggerEnter(Collider other) { // On a collision with a trigger, if the trigger is named either end or reset points
        switch (other.name) {
            case "End": // End the episode and reward
                AddReward(0.3f);
                EndEpisode();
                break;
            case "Reset Points": // Reset the checkpoints at the start
                other.enabled = false;
                CPT.refreshCheckpoints();
                break;
            default:
                break;
        }
    }

    private void OnCollisionEnter(Collision collision) {
        switch (collision.gameObject.tag) {
            case "Rock":
                addStrike();
                AddReward(-0.3f);
                break;
            case "Tree":
                addStrike();
                AddReward(-0.2f);
                break;
            case "Tire":
                addStrike();
                AddReward(-0.1f);
                break;
            default : break;
        }
    }



    public override void Heuristic(in ActionBuffers actions) { //A was for the agent to understand my actions as a player if heuristic methods are used
        int axisX = 0;
        int axisZ = 0;

        if (Input.GetKey(KeyCode.W)) axisZ = 1;
        if (Input.GetKey(KeyCode.S)) axisZ = 2;
        if (Input.GetKey(KeyCode.D)) axisX = 1;
        if (Input.GetKey(KeyCode.A)) axisX = 2;

        ActionSegment<int> discreteActions = actions.DiscreteActions;
        discreteActions[0] = axisX;
        discreteActions[1] = axisZ;

    }

    private void grassageHold() {
        if (car.checkGrassage()) {
            AddReward(-0.1f);
            addStrike();
        }
    }

    private void FixedUpdate() { 

        if (transform.position.y < -5) { // Punishing if the agent is off the map
            AddReward(-0.3f);
            EndEpisode();
        }

        if (checkStrikeOut()) {
            EndEpisode();
        }
    }


    private void resetCar() { // Reset the car to its default position
        transform.SetLocalPositionAndRotation(new Vector3(0, 2f, 0), Quaternion.identity); ;
        rb.isKinematic = true; // Reset velocity
        rb.isKinematic = false;
    }

    public void Reward(float reward) { // A method to reward or punish externally
        AddReward(reward);
        Debug.Log("Reward added: " + reward);
    }

    public void CloseEpsiode() { // A method to close the episode externally
        EndEpisode();
    }

    private bool checkStrikeOut() {
        if (strikes == strikeOut) {
            strikes = 0;
            Debug.Log("Batter out!");
            return true;
        } else {
            return false;
        }
    }
    
    private void addStrike() {
        strikes++;
        Debug.Log("Strike! " + strikes);
    }

}


