using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisualiserBar : MonoBehaviour
{
    private RectTransform rectTransform;
    private float height;

    // Start is called before the first frame update
    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 newSize = rectTransform.rect.size;
        newSize.y = height;
        
    }

    

}
