using System.Text;
using UnityEngine;

public class DoorTrigger : MonoBehaviour
{
    private StringBuilder sb = new StringBuilder();
    private BoxCollider boxCollider;
    [SerializeField]
    private DoorSystem[] doorSystems = new DoorSystem[2];
    private MessageSystem messageSystem;
    private string[] status = new string[2]
    {
        "Unlocked",
        "Locked"
    };

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if(sb.Length  > 0) sb.Clear();
            for(int d = 0; d < doorSystems.Length; d++)
            {
                doorSystems[d].lockDoor = !doorSystems[d].lockDoor;
            }
            if (messageSystem == null) messageSystem = MessageSystem.messageSystem;
            int sIndex = doorSystems[0].lockDoor ? 1 : 0;
            sb.Append("Door" + (doorSystems.Length > 1 ? "s" : "") + " " + status[sIndex]);
            messageSystem.SetMessage(sb.ToString(), MessageSystem.MessageType.Top);
            if (boxCollider == null) boxCollider = GetComponent<BoxCollider>();
            boxCollider.enabled = false;
        }
    }
    public void ResetObject()
    {
        if (boxCollider == null) boxCollider = GetComponent<BoxCollider>();
        boxCollider.enabled = true;
    }
}
