using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Elevator : MonoBehaviour
{
    [SerializeField]
    private float maxY;
    [SerializeField]
    private float minY;
    [SerializeField]
    private float moveSpeed;
    public bool activated = false;
    private Vector3 position = Vector3.zero;
    private Transform elevator;
    void Start()
    {
        elevator = transform.GetChild(0);
        position.y = minY;
        elevator.localPosition = position;
    }
    void FixedUpdate()
    {
        position.y = activated ? maxY : minY;
        float distance = Mathf.Clamp(position.y, minY, maxY);
        Vector3 SetPos = new Vector3(0, distance, 0);
        if(elevator.localPosition != SetPos)
            elevator.localPosition = Vector3.MoveTowards(elevator.localPosition, SetPos, Time.deltaTime * moveSpeed);
    }
    public void ResetObject()
    {
        elevator.localPosition = position;
    }
}
