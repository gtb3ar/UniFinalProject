using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Visualiser : MonoBehaviour
{

    [SerializeField] Color[] colors; //Array of colors for the bars
    [SerializeField] float maxAmpitude; // Max bar height
    [SerializeField] float minAmpitude; // Min bar height
    [SerializeField] raycastWheel[] wheels; // Wheels that bars represent

    private RectTransform rectTransform;
    private Color currentColor;
    private float colorDistance;
    private VisualiserBar[] bars; // The bars

    // Start is called before the first frame update
    void Start()
    {
        bars = GetComponentsInChildren<VisualiserBar>(); // Setting up the bars on the visuliser
        rectTransform = GetComponent<RectTransform>(); // Transform for the UI componet
        colorDistance = (maxAmpitude-minAmpitude)/colors.Length; // The distance between the switch in colors
    }

    // Update is called once per frame
    void Update() {
        for (int i = 0; i < bars.Length; i++) { // For each bar
            Vector2 currentHeight = bars[i].GetComponent<RectTransform>().rect.size; //Grab the transform
            currentHeight.y = Mathf.Clamp(10 + (wheels[i].compressiveGrip * 15), minAmpitude, maxAmpitude); // Map the y value to the comressiveGrip of the correlating wheel and clamp the value inbetween the min and max Ampitude
            bars[i].GetComponent<RectTransform>().sizeDelta = currentHeight; //Apply new y value
            int colorIndex = Mathf.Clamp((int)Math.Truncate((currentHeight.y - minAmpitude) / colorDistance), 0, colors.Length - 1); // Decide on the color that the bar should be based off the y value
            bars[i].GetComponent<Image>().color = colors[colorIndex]; // Apply color

        }
    }

    public static float PercentageMap (float value, float valueMax, float targetMax) { // Mapping values to to a new range
        return value / valueMax * targetMax; 
    }
}
