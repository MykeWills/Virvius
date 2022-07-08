using UnityEngine;

public class ElevatorTrigger : MonoBehaviour
{
    public SwitchSystem mainSwitch;
    private Transform holderReturn;
    public bool dontMoveElevator = false;
    private CharacterController characterController;
    private void Start()
    {
        holderReturn = GameObject.Find("GameSystem/Game").transform;
    }
    private void OnTriggerEnter(Collider other)
    {
        //IF PLAYER IS NOT MOVING WITH PLATFORM. PLATFORM MOVEMENT IS NOT UNDER FIXED UPDATE!!!!!
        if (other.gameObject.CompareTag("Player"))
        {
            if (characterController == null) characterController = PlayerSystem.playerSystem.GetComponent<CharacterController>();
            Physics.IgnoreCollision(GetComponent<Collider>(), characterController);
            if (other.gameObject.transform.parent != transform)
            {
                other.gameObject.transform.parent = transform;
                if (mainSwitch == null) return;
                if (!dontMoveElevator)
                {
                    bool active = !mainSwitch.hasMoved;
                    if(active) mainSwitch.ActivateSwitch(mainSwitch.switchSubType);
                    else mainSwitch.DeactivateSwitch(mainSwitch.switchSubType);
                }
            }
        }
    }
   
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            other.gameObject.transform.parent = holderReturn;
        }
    }
}
