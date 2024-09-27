using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class checkpoint : MonoBehaviour
{
    private checkpointTracker tracker; //A link for the checkpoint tracker to keep track of this object

    private void OnTriggerEnter(Collider other) { //Alerts the tracker of breach
        if (other.TryGetComponent<AgentDriveOnRoad>(out AgentDriveOnRoad agent)) {
            tracker.AgentThrouchCheckpoint(this);
        }
    }

    public void SetCheckpointTracker(checkpointTracker tracker) {
        this.tracker = tracker; // Sets the tracker
    }
}

