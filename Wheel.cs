using System.Collections;
using System.Collections.Generic;
using UnityEditor.UI;
using UnityEngine;
using UnityEngine.UIElements;

public class Wheel : MonoBehaviour
{

    [SerializeField] private bool steer;
    [SerializeField] private bool invertSteer;
    [SerializeField] private bool invertWheel;
    [SerializeField] private bool power;

    private AgentDriveOnRoad agent;

    public float steerAngle {  get;  set; }
    public float torque { get;  set; }

    private WheelCollider wheelCollider;
    private Transform wheelTransform;

    private void Awake() {
    }

    // Start is called before the first frame update
    void Start()
    {
        wheelCollider = GetComponentInChildren<WheelCollider>(); 
        wheelTransform = GetComponentInChildren<MeshRenderer>().GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        wheelCollider.GetWorldPose(out Vector3 colliderlocalPosition, out Quaternion colliderRotation);
        wheelTransform.GetLocalPositionAndRotation(out Vector3 meshlocalPosition, out Quaternion meshRotation);
        wheelTransform.rotation = colliderRotation;
    }

    private void FixedUpdate() {
        if (steer) {
            wheelCollider.steerAngle = steerAngle * (invertSteer ? -1 : 1);
        }

        if (power) {
            wheelCollider.motorTorque = torque;
        }
        
    }

    public void stopWheel() {
        wheelCollider.rotationSpeed = 0;
    }

}
