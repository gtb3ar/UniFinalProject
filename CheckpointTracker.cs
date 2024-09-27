using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor.UI;
using UnityEngine;

public class checkpointTracker : MonoBehaviour {
    private List<checkpoint> checkpointsList; //Creates a list of the checkpoints to track
    private GameObject[] tiles; //Tiles on map
    private checkpoint[] currentCheckpoints; //checkpoints inside the tile
    private AgentDriveOnRoad agent; //The AI agents
    private int nextCheckpoint; //Numeric value for the checkpoint the agent should next go through on the list

    private void Awake() { // Finds agents
        agent = GameObject.FindGameObjectWithTag("Agent").GetComponent<AgentDriveOnRoad>();

    }

    public void refreshCheckpoints() { // Renews the count 
        tiles = GameObject.FindGameObjectsWithTag("Tile"); //Gets all tiles, then the gates in those tiles, then individually stores them in the list
        checkpointsList = new List<checkpoint>();

        foreach (GameObject tile in tiles) {
            currentCheckpoints = tile.GetComponentsInChildren<checkpoint>();
            foreach (checkpoint checkpoint in currentCheckpoints) {

                checkpointsList.Add(checkpoint);
                checkpoint.SetCheckpointTracker(this);
            }
        }
        nextCheckpoint = 0; //Reset the marker
    }

    public void AgentThrouchCheckpoint(checkpoint checkpoint) { // Listening to the check points, if the agent passes through and its the corect checkpoint it gains points
        if (checkpointsList.IndexOf(checkpoint) == nextCheckpoint) {
            nextCheckpoint += 1;
            agent.Reward(0.1f + (float)(checkpointsList.IndexOf(checkpoint)*0.01));
        } else {
            agent.Reward(-0.1f);
        }
    }

    public Vector3 getNextCheckpointPosition() {
        try {
            return checkpointsList[nextCheckpoint].GetComponent<Transform>().position;
        } catch {
            return Vector3.zero;
        }
        
    }

}
