using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class InputController : MonoBehaviour
{
    private string inputSteerAxis = "Horizontal";
    private string inputThrottleAxis = "Vertical";
    

    public float throttleInput { get; set; }
    public float steerInput { get; set; }

    void Awake() {

    }

    // Update is called once per frame
    void Update() {
        
        //steerInput = Input.GetAxis(inputSteerAxis);
        //throttleInput = Input.GetAxis(inputThrottleAxis);
        
    }
}
