using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SwitchSystem : MonoBehaviour
{
    private BoxCollider boxCollider;
    private MessageSystem messageSystem;
    private AudioSystem audioSystem;
    [SerializeField]
    private string customMessage = "";
    [SerializeField]
    private bool setCustomFinalMessage = false;
    private string normalMessage = "Sequence Completed.";
    [SerializeField]
    private AudioSource moveObjectSrc;
    [SerializeField]
    private AudioClip[] moveObjectSound = new AudioClip[3];
    [HideInInspector]
    public bool hasMoved = false;
    [HideInInspector]
    public bool isActive = false;
    [HideInInspector]
    public bool isCoActive = false;
    public MessageTriggerSystem messageTrigger;
    private bool isTimer = false;
    private bool isPressed = false;
    private bool isCoPressed = false;
    [HideInInspector]
    public bool isMoving = false;
    private bool setCorrespondantSwitch = false;
    private float switchTimer;
    public float switchTime = 5;
    private int switchCounter = 2;
    private float movePressedSpeed = 2;
    private Vector3 updatedPosition = Vector3.zero;
    public bool switchActivated = false;
    public enum SwitchType { Press, Shoot, Step };
    public enum SwitchSubType { Move, Spawn, Timed, Counter, EventOnly };
    public enum SwitchActivationType { MoveDirection, Activation, Instantiation, EventOnly };
    public enum Direction { AbscissaX, OrdinateY, ApplicateZ };

    private string[] tags = new string[11]
   {
        "Sword",
        "ShotgunBullet",
        "SpikeBullet",
        "MinigunBullet",
        "GrenadeBullet",
        "RocketBullet",
        "RailBullet",
        "PhotonBullet",
        "SigmaBullet",
        "ObstacleBullet",
        "RocketBulletMini"
   };
    [Header("Main Switch Options")]
    public SwitchType switchType;
    [Space]
    public SwitchSubType switchSubType;
    public SwitchActivationType activationType;
    [Space]
    public Direction moveDirection;
    public Direction rotateDirection;
    public float maxXYZ;
    public float minXYZ;
    public float moveSpeed;
    [Space]
    [SerializeField]
    private bool rotateMoveObj = false;
    [SerializeField]
    private float rotateSpeed;
    [SerializeField]
    private float rotateAmt = 1;
    [SerializeField]
    private bool reverseUpdatedPos = false;
    [Space]
    [Header("Switch Assignment")]
    public GameObject activationObject;
    public GameObject[] eventObject;
    [Space]
    public AudioClip pressSoundFx;
    public AudioClip[] messageSoundFx = new AudioClip[2];
    public Transform[] pressInObject = new Transform[2];
    public GameObject[] OnOffObjects = new GameObject[4];
    public Light[] litLights = new Light[2];
    public GameObject correspondingSwitch;
    [Space]
    public Transform instantiationPos;
    public Transform instantiationPool;
    [SerializeField]
    private int dropCranePosIndex = 0;
    [Space]
    [Header("Surface Linking")]
    [SerializeField]
    private NavMeshLink[] navMeshLinkUpper;
    [SerializeField]
    private bool allowLinkValueChangeUpper = false;
    [SerializeField]
    private NavMeshLink[] navMeshLinkLower;
    [SerializeField]
    private bool allowLinkValueChangeLower = false;
    [Space]
    [Header("Door Preferences")]
    [SerializeField]
    private bool lockedDoorEvent = false;
    [SerializeField]
    private bool multipleDoors = false;
    //========================================================================================//
    //===================================[UNITY FUNCTIONS]====================================//
    //========================================================================================//
    private void Start()
    {
        audioSystem = AudioSystem.audioSystem;
        boxCollider = GetComponent<BoxCollider>();
        if (activationObject != null)
            updatedPosition = activationObject.transform.localPosition;
        ResetObject();
    }
    private void Update()
    {
        Timer();
        MoveButtonPressed();
    }
    void FixedUpdate()
    {
        //All moving platforms with Character controller must remain in FixedUpdate otherwise player will fall off platform
        MoveCurrentObject();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player") && !isActive || other.gameObject.CompareTag("Crate") && !isActive)
        {
            if (switchType == SwitchType.Press || switchType == SwitchType.Step)
            {
                if(eventObject.Length > 0)
                {
                    if (eventObject[0].TryGetComponent(out CraneDropSystem craneDropSystem))
                    {
                        if (craneDropSystem.craneActive) return;
                    }
                }
                ActivateSwitch(switchSubType);
                if (pressSoundFx != null) audioSystem.PlayAudioSource(pressSoundFx, 1, 1, 128);
            }

        }

    }
    private void OnCollisionEnter(Collision collision)
    {
        string tag = collision.gameObject.tag;
        for (int t = 0; t < tags.Length; t++)
        {
            if (tag == tags[t])
            {
                if (switchType == SwitchType.Shoot && !isActive)
                {
                    audioSystem.PlayAudioSource(pressSoundFx, 1, 1, 128);
                    ActivateSwitch(switchSubType);
                }
                break;
            }
        }
    }

    //========================================================================================//
    //===================================[MOVING PLATFORM]====================================//
    //========================================================================================//
    //Goes into Fixed Update to move the platform  in a direction
    private void MoveCurrentObject()
    {
        if (!isMoving) return;
        switch (moveDirection)
        {
            case Direction.AbscissaX: updatedPosition.x = !hasMoved ? !reverseUpdatedPos ? maxXYZ : minXYZ : !reverseUpdatedPos ? minXYZ : maxXYZ; break;
            case Direction.OrdinateY: updatedPosition.y = !hasMoved ? !reverseUpdatedPos ? maxXYZ : minXYZ : !reverseUpdatedPos ? minXYZ : maxXYZ; break;
            case Direction.ApplicateZ: updatedPosition.z = !hasMoved ? !reverseUpdatedPos ? maxXYZ : minXYZ : !reverseUpdatedPos ? minXYZ : maxXYZ; break;
        }
        Vector3 axis = Vector3.zero;
      
        if (rotateMoveObj)
        {
            switch (rotateDirection)
            {
                case Direction.AbscissaX: axis = new Vector3(rotateAmt * Time.deltaTime * rotateSpeed, 0, 0); break;
                case Direction.OrdinateY: axis = new Vector3(0, rotateAmt * Time.deltaTime * rotateSpeed, 0); break;
                case Direction.ApplicateZ: axis = new Vector3(0, 0, rotateAmt * Time.deltaTime * rotateSpeed); break;
            }
            activationObject.transform.Rotate(axis);
        }
        activationObject.transform.localPosition = Vector3.MoveTowards(activationObject.transform.localPosition, updatedPosition, Time.deltaTime * moveSpeed);
        if (moveObjectSrc != null)
        {
            if (moveObjectSrc.clip == moveObjectSound[0] && !moveObjectSrc.isPlaying)
            {
                moveObjectSrc.clip = moveObjectSound[1];
                moveObjectSrc.loop = true;
                moveObjectSrc.Play();
            }
        }
        if (activationObject.transform.localPosition == updatedPosition)
        {
            isMoving = false;
            if (moveObjectSrc != null)
            {
                moveObjectSrc.clip = moveObjectSound[2];
                moveObjectSrc.loop = false;
                moveObjectSrc.Play();
            }
        }
    }
    private void PlayMoveObjectSound()
    {
        if (moveObjectSrc != null)
        {
            moveObjectSrc.clip = moveObjectSound[0];
            moveObjectSrc.loop = false;
            if (!moveObjectSrc.isPlaying) moveObjectSrc.Play();
        }
    }
    //========================================================================================//
    //===================================[TIMER ACTIVATION]===================================//
    //========================================================================================//
    private void Timer()
    {
        if (!isTimer) return;
        switchTimer -= Time.deltaTime;
        float time = Mathf.Clamp(switchTimer, 0, switchTime);
        if (time == 0)
        {
            DeactivateSwitch(switchSubType);
            isTimer = false;
            switchTimer = switchTime;
        }
    }

    //========================================================================================//
    //===================================[SWITCH/COSWITCH FUNCTIONS]==========================//
    //========================================================================================//
    private void MoveButtonPressed()
    {
        //Moves the direction back and forth of the button based on isActive boolean

        //Move both main switch and co-switch pressinobjects ON/ON
        if (switchSubType == SwitchSubType.Counter)
        {
            if (!isCoPressed && !isPressed) return;
            float Y = isActive ? 0 : 0.25f;
            Vector3 newPos = new Vector3(0, Y, 0);
            float delta = Time.deltaTime * movePressedSpeed;
            if (isPressed)
            {
                pressInObject[0].localPosition = Vector3.MoveTowards(pressInObject[0].localPosition, newPos, delta);
                if (pressInObject[0].localPosition == newPos)
                    isPressed = false;
            }
            if (isCoPressed)
            {
                pressInObject[1].localPosition = Vector3.MoveTowards(pressInObject[1].localPosition, newPos, delta);
                if (pressInObject[1].localPosition == newPos)
                    isCoPressed = false;
            }
        }
        //Move the main switch pressinobject & reverse the co-switch to opposite.  [main/co] ON/OFF [main/co] OFF/ON
        else
        {
            if (!isPressed) return;
            float Y = isActive ? 0 : 0.25f;
            Vector3 newPos = new Vector3(0, Y, 0);
            pressInObject[0].localPosition = Vector3.MoveTowards(pressInObject[0].localPosition, newPos, Time.deltaTime * movePressedSpeed);
            if (setCorrespondantSwitch)
            {
                float coY = isActive ? 0.25f : 0;
                Vector3 newCoPos = new Vector3(0, coY, 0);
                pressInObject[1].localPosition = Vector3.MoveTowards(pressInObject[1].localPosition, newCoPos, Time.deltaTime * movePressedSpeed);
                if (pressInObject[0].localPosition == newPos && pressInObject[1].localPosition == newCoPos)
                    isPressed = false;
            }
            else
            {
                if (pressInObject[0].localPosition == newPos)
                    isPressed = false;
            }
        }
    }
    public void DeactivateSwitch(SwitchSubType subType)
    {
        if (!isActive) return;
        isActive = false;
        ActivateSwitchSubType(subType);
    }
    public void ActivateSwitch(SwitchSubType subType)
    {
        if (isActive) return;
        isActive = true;
        ActivateSwitchSubType(subType);
    }
    private void ActivateSwitchSubType(SwitchSubType subType)
    {
        isPressed = true;
        if (subType != SwitchSubType.Counter) ChangeSwitchAttributes();
      
        switch (subType)
        {
            // Will activate a moving Object in a direction
            case SwitchSubType.Move: SetActivationType(activationType); break;
            // Will activate an object to apear or disappear
            case SwitchSubType.Spawn: SetActivationType(activationType); break;
            // Will Keep object active for a brief time
            case SwitchSubType.Timed: isTimer = true; SetActivationType(activationType); break;
            // Will add a counter to object to activate 
            case SwitchSubType.Counter:
                {
                    if (messageSystem == null)
                        messageSystem = PlayerSystem.playerSystem.GetComponent<MessageSystem>();
                    switchCounter--;
                    litLights[0].enabled = false;
                    OnOffObjects[0].SetActive(false);
                    OnOffObjects[1].SetActive(true);
                    if (switchCounter < 1)
                    {
                        if (isActive)
                        {
                            audioSystem.PlayAudioSource(messageSoundFx[1], 1, 1, 128); 
                            messageSystem.SetMessage(setCustomFinalMessage ? customMessage : normalMessage, MessageSystem.MessageType.Center);
                        }
                        SetActivationType(activationType);
                    }
                    else if (switchCounter > 0)
                    {
                        if (isActive)
                        {
                            audioSystem.PlayAudioSource(messageSoundFx[0], 1, 1, 128);
                            messageSystem.SetMessage("One more to go.", MessageSystem.MessageType.Center);
                        }
                        ActivateEvent(0, isActive);
                    }
                    break;
                }
            case SwitchSubType.EventOnly: SetActivationType(activationType); break;
        }
    }
    private void ChangeSwitchAttributes()
    {
        int index = isActive ? 1 : 0;
        int index2 = !isActive ? 3 : 2;
        for (int l = 0; l < 2; l++)
        {
            if (l == index)
                litLights[l].enabled = true;
            else litLights[l].enabled = false;
        }
        int arrayIndex = setCorrespondantSwitch ? OnOffObjects.Length : 2;
        for (int g = 0; g < arrayIndex; g++)
        {
            if (g == index) OnOffObjects[g].SetActive(true);
            else if (g == index2) OnOffObjects[g].SetActive(true);
            else OnOffObjects[g].SetActive(false);
        }
    }
    private void SetActivationType(SwitchActivationType type)
    {
        if (messageTrigger != null) messageTrigger.ShutMessageOff(isActive);
        switch (type)
        {
            case SwitchActivationType.MoveDirection:
                {
                    if (activationObject == null) { Debug.LogError("No Activation Object to Move!"); return; }
                    isMoving = true;
                    PlayMoveObjectSound();
                    hasMoved = !hasMoved;
                    ActivateEvent(0, isActive);
                    break;
                }
            case SwitchActivationType.Activation:
                {
                    if (activationObject == null) { Debug.LogError("No Activation Object to Activate!"); return; }
                    bool active = activationObject.activeInHierarchy;
                    activationObject.gameObject.SetActive(!active);
                    ActivateEvent(0, isActive);
                    break;
                }
            case SwitchActivationType.Instantiation:
                {
                    if (activationObject == null) { Debug.LogError("No Activation Object to Instantiate!"); return; }
                    if (!isActive) return;
                    GameObject newObject = Instantiate(activationObject, instantiationPool);
                    newObject.transform.position = instantiationPos.position;
                    ActivateEvent(0, isActive);
                    break;
                }
            case SwitchActivationType.EventOnly: ActivateEvent(0, isActive); break;
        }
        switchActivated = true;
    }
    private void ActivateEvent(int index, bool active)
    {
        if(eventObject.Length < 1) return;
        if (eventObject[index].TryGetComponent(out Light light)) 
        { 
            light.enabled = active; 
            return; 
        }
        if (multipleDoors)
        {
            for (int d = 0; d < eventObject.Length; d++)
            {
                if (eventObject[d].TryGetComponent(out DoorSystem door))
                {
                    door.LockDoor(!lockedDoorEvent ? !active : active);
                    return;
                }
            }
        }
        else
        {
            if (eventObject[index].TryGetComponent(out DoorSystem door))
            {
                door.LockDoor(!lockedDoorEvent ? !active : active);
                return;
            }
        }
        if (eventObject[index].TryGetComponent(out AutoMoveSystem autoMoveSystem)) 
        { 
            autoMoveSystem.ActivateMovingObject(active); 
            return; 
        }
        if (eventObject[index].TryGetComponent(out CrusherSystem crusherSystem))
        {
            crusherSystem.ActivateCrush(!active);
            return;
        }
        
        if (eventObject[0].TryGetComponent(out CraneDropSystem craneDropSystem))
        {
            craneDropSystem.SelectDropPosition(dropCranePosIndex);
            return;
        }
        if (eventObject[0].TryGetComponent(out MultiSwitchSystem multiSwitchSystem))
        {
            if(!active) multiSwitchSystem.ResetObject();
            else multiSwitchSystem.PressSwitch();
            return;
        }
        Debug.LogError("No Light, Door or Auto Moving component on event Object!");
    }
    public void CoSwitchCounter()
    {
        if (messageSystem == null)
            messageSystem = PlayerSystem.playerSystem.GetComponent<MessageSystem>();
        isCoActive = true;
        isCoPressed = true;
        switchCounter--;
        litLights[1].enabled = false;
        OnOffObjects[2].SetActive(false);
        OnOffObjects[3].SetActive(true);
        if (switchCounter < 1)
        {
            audioSystem.PlayAudioSource(messageSoundFx[1], 1, 1, 128);
            messageSystem.SetMessage(setCustomFinalMessage ? customMessage : normalMessage, MessageSystem.MessageType.Center);
            SetActivationType(activationType);
            ActivateEvent(1, isCoActive);
        }
        else if (switchCounter > 0)
        {
            audioSystem.PlayAudioSource(messageSoundFx[0], 1, 1, 128);
            messageSystem.SetMessage("One more to go.", MessageSystem.MessageType.Center);
            ActivateEvent(1, isCoActive);
        }

    }
    public void ResetObject()
    {
        DeactivateSwitch(switchSubType);
        isTimer = false;
        switchTimer = switchTime;
        hasMoved = false;
        for (int e = 0; e < eventObject.Length; e++)
        {
            if (eventObject[0] == null) break;
            if (!eventObject[0].TryGetComponent(out CraneDropSystem craneDropSystem))
                ActivateEvent(e, false);
           
        }
        switchCounter = 2;
        if (switchSubType == SwitchSubType.Counter)
        {
            isCoActive = false;
            isCoPressed = false;
            correspondingSwitch.SetActive(true);
            setCorrespondantSwitch = correspondingSwitch.activeInHierarchy;
            for (int l = 0; l < 2; l++)
                litLights[l].enabled = true;
            for (int g = 0; g < OnOffObjects.Length; g++)
            {
                if (g == 0) OnOffObjects[g].SetActive(true);
                else if (g == 2) OnOffObjects[g].SetActive(true);
                else OnOffObjects[g].SetActive(false);
            }
            float Y = isActive ? 0 : 0.25f;
            Vector3 newPos = new Vector3(0, Y, 0);
            pressInObject[0].localPosition = newPos;
            pressInObject[1].localPosition = newPos;
        }
        else
        {
            setCorrespondantSwitch = correspondingSwitch.activeInHierarchy;
            int index = isActive ? 1 : 0;
            int index2 = isActive ? 2 : 3;
            for (int l = 0; l < 2; l++)
            {
                if (l == index)
                    litLights[l].enabled = true;
                else litLights[l].enabled = false;
            }
            int arrayIndex = setCorrespondantSwitch ? OnOffObjects.Length : 2;
            for (int g = 0; g < arrayIndex; g++)
            {
                if (g == index) OnOffObjects[g].SetActive(true);
                else if (g == index2) OnOffObjects[g].SetActive(true);
                else OnOffObjects[g].SetActive(false);
            }
            float Y = isActive ? 0 : 0.25f;
            Vector3 newPos = new Vector3(0, Y, 0);
            pressInObject[0].localPosition = newPos;

            if (setCorrespondantSwitch)
            {
                float coY = isActive ? 0.25f : 0f;
                Vector3 newCoPos = new Vector3(0, coY, 0);
                pressInObject[1].localPosition = newCoPos;
            }
        }
        switch (moveDirection)
        {
            case Direction.AbscissaX: updatedPosition.x = !hasMoved ? !reverseUpdatedPos ? maxXYZ: minXYZ : !reverseUpdatedPos ? minXYZ : maxXYZ; break;
            case Direction.OrdinateY: updatedPosition.y = !hasMoved ? !reverseUpdatedPos ? maxXYZ : minXYZ : !reverseUpdatedPos ? minXYZ : maxXYZ; break;
            case Direction.ApplicateZ: updatedPosition.z = !hasMoved ? !reverseUpdatedPos ? maxXYZ : minXYZ : !reverseUpdatedPos ? minXYZ : maxXYZ; break;
        }
        if (activationObject != null)
            activationObject.transform.localPosition = updatedPosition;
        if (activationType == SwitchActivationType.Activation) 
        { 
            if (activationObject != null) 
            {
                bool active = activationObject.activeInHierarchy;
                activationObject.gameObject.SetActive(active); 
            } 
        }
        if (switchType == SwitchType.Press || switchType == SwitchType.Step)
            boxCollider.isTrigger = true;
        else boxCollider.isTrigger = false;
        switchActivated = false;
    }
    public void ActivateObjectState()
    {
        isPressed = true;
        if (switchSubType != SwitchSubType.Counter) ChangeSwitchAttributes();
        isActive = true;
        SetActivationType(activationType);
        switchActivated = true;
    }
}