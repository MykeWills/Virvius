using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleCameraMovement : MonoBehaviour
{
    // Range over which height varies.
    public float heightScale = 1.0f;
    public float widthScale = 1.0f;
    public float lengthScale = 1.0f;

    // Distance covered per second along X axis of Perlin plane.
    public float xScale = 1.0f;
    public float yScale = 1.0f;
    public float zScale = 1.0f;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float length = lengthScale * Mathf.PerlinNoise(Time.time * zScale, 0.0f);
        float height = heightScale * Mathf.PerlinNoise(Time.time * xScale, 0.0f);
        float width = widthScale * Mathf.Sin(Time.time * yScale);
        Vector3 pos = transform.localPosition;
        pos.y = height;
        pos.x = width;
        pos.z = length;
        transform.localPosition = pos;
    }
}
