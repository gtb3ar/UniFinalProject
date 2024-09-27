using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class GameManager : MonoBehaviour
{
    public static GameManager instance { get; private set; }
    public InputController inputController { get; private set; }

    // Start is called before the first frame update
    void Awake() {
        instance = this;
        inputController = GetComponentInChildren<InputController>();
    }
}
