using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCraneDoor : MonoBehaviour
{
    [SerializeField]
    private float maxHeight;
    [SerializeField]
    private float minHeight;
    [SerializeField]
    private float doorSpeed = 90;
    private bool openDoor = false;
    private bool isOpen = false;
    private Vector3 doorPosition;
    private Transform door;
    private void Start()
    {
        door = transform.GetChild(0);
        doorPosition = new Vector3(0, minHeight, 0);
        door.transform.localPosition = doorPosition;
    }
    void Update()
    {
        Door();
    }
    private void Door()
    {
        if (!openDoor) return;
        door.transform.localPosition = Vector3.MoveTowards(door.transform.localPosition, doorPosition, Time.deltaTime * doorSpeed);
        if(door.transform.localPosition == doorPosition) openDoor = false;
    }
    private void OpenDoor(bool active)
    {
        isOpen = active;
        if (doorPosition != new Vector3(0, (isOpen ? maxHeight : minHeight), 0))
            doorPosition = new Vector3(0, (isOpen ? maxHeight : minHeight), 0);
        openDoor = true;
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Crane"))
            OpenDoor(true);
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Crane"))
            OpenDoor(false);
    }
    public void ResetObject()
    {
        doorPosition = new Vector3(0, minHeight, 0);
        door.transform.localPosition = doorPosition;
        openDoor = false;
        isOpen = false;
    }
}
