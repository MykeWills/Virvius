using UnityEngine;
using UnityEngine.AI;
using System.Text;

public class DoorSystem : MonoBehaviour
{
    private PlayerSystem playerSystem;
    private MessageSystem messageSystem;
    private AudioSystem audioSystem;
    private StringBuilder stringBuilder = new StringBuilder();
    private string[] tags = new string[3] { "Player", "Enemy", "DinEnemy" };
    private bool isOpen = false;
    private bool openDoor = false;
    [HideInInspector]
    public bool lockDoor = false;
    private bool[] keyDoor = new bool[3];
    public AudioClip DoorOpenSfx;
    public AudioClip DoorCloseSfx;
    public AudioClip DoorBangSfx;
    public AudioClip[] popupSfx;
    private AudioSource audioSrc;
    public enum DoorType { Normal, Blue, Yellow, Red, Locked }
    public DoorType doorType;
    public enum DirectionRight { Up, Right, UpRight }
    public enum DirectionLeft { Down, Left, DownLeft }

    public float moveSpeed;
    [Header("Right Door")]
    public DirectionRight directionRight;
    public Transform rightDoor;
    public Vector3 maxRightVector;
    private Vector3 rightVector = Vector3.zero;
    [Space]
    [Header("Left Door")]
    public DirectionLeft directionLeft;
    public Transform leftDoor;
    public Vector3 maxLeftVector;
    private Vector3 leftVector = Vector3.zero;
    private bool grantedAccess = false;
    private bool[] obstructions = new bool[3];
    private 
    void Start()
    {
        audioSystem = AudioSystem.audioSystem;
        audioSrc = GetComponent<AudioSource>();
        leftDoor.localPosition = Vector3.zero;
        rightDoor.localPosition = Vector3.zero;
        SelectDoorType(doorType);
        //SetNavLinks(false);
    }
    // Update is called once per frame
    void Update()
    {
        
        OpenDoor();
    }
    //private void SetNavLinks(bool active)
    //{
    //    if (navMeshLinks.Length > 0)
    //    {
    //        for (int l = 0; l < navMeshLinks.Length; l++)
    //            navMeshLinks[l].enabled = active;
    //    }
    //}
    private void PlayDoorSound(AudioClip clip)
    {
        if (audioSrc.isPlaying)
            audioSrc.Stop();
        audioSrc.clip = clip;
        audioSrc.Play();
    }
    public void LockDoor(bool active)
    {
        lockDoor = active;
    }
    public void ResetObject()
    {
        //SetNavLinks(false);
        openDoor = false;
        isOpen = false;
        leftDoor.localPosition = Vector3.zero;
        rightDoor.localPosition = Vector3.zero;
        SelectDoorType(doorType);
        for(int t = 0; t < obstructions.Length; t++) obstructions[t] = false;
    }
    private void SelectDoorType(DoorType type)
    {
        switch (type)
        {
            case DoorType.Normal: LockDoor(false); break;
            case DoorType.Locked: LockDoor(true); break;
            case DoorType.Blue: grantedAccess = false; break;
            case DoorType.Yellow: grantedAccess = false; break;
            case DoorType.Red: grantedAccess = false; break;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        string tag = other.gameObject.tag;
        for (int t = 0; t < tags.Length; t++)
        {
            if (other.gameObject.CompareTag(tags[t]))
            {
                obstructions[t] = true;
                if (playerSystem == null)
                {
                    playerSystem = PlayerSystem.playerSystem;
                    messageSystem = playerSystem.GetComponent<MessageSystem>();
                }
        
                switch (doorType)
                {
                    case DoorType.Normal:
                        {
                            if (lockDoor) 
                            {
                                if (tags[0] == tag) messageSystem.SetMessage("Door Locked!", MessageSystem.MessageType.Center); 
                                return; 
                            }
                            PlayDoorSound(DoorOpenSfx);
                            isOpen = false;
                            openDoor = true;
                            break;
                        }
                    case DoorType.Locked:
                        {
                            if (lockDoor) 
                            {
                                if (tags[0] == tag) messageSystem.SetMessage("Door Locked!", MessageSystem.MessageType.Center); 
                                return; 
                            }
                            PlayDoorSound(DoorOpenSfx);
                            isOpen = false;
                            openDoor = true;
                            break;
                        }
                    case DoorType.Blue:
                        {
                            if (playerSystem.keyCards[0])
                            {
                                if (tags[0] == tag)
                                {
                                    audioSystem.PlayAudioSource(popupSfx[0], 1, 1, 128);
                                    if (!grantedAccess) { messageSystem.SetMessage("Blue Access Card Granted!", MessageSystem.MessageType.Center); grantedAccess = true; }
                                }
                                PlayDoorSound(DoorOpenSfx);
                                isOpen = false;
                                openDoor = true;
                            }
                            else
                            {
                                if (tags[0] == tag)
                                {
                                    audioSystem.PlayAudioSource(popupSfx[1], 1, 1, 128);
                                    messageSystem.SetMessage("Blue Card Restricted Access", MessageSystem.MessageType.Center);
                                }
                            }
                            break;
                        }
                    case DoorType.Yellow:
                        {
                            if (playerSystem.keyCards[1])
                            {
                                if (tags[0] == tag)
                                {
                                    audioSystem.PlayAudioSource(popupSfx[0], 1, 1, 128);
                                    if (!grantedAccess)
                                    {
                                        messageSystem.SetMessage("Yellow Access Card Granted!", MessageSystem.MessageType.Center); grantedAccess = true;
                                    }
                                }
                                PlayDoorSound(DoorOpenSfx);
                                isOpen = false;
                                openDoor = true;
                            }
                            else
                            {
                                if (tags[0] == tag)
                                {
                                    audioSystem.PlayAudioSource(popupSfx[1], 1, 1, 128);
                                    messageSystem.SetMessage("Yellow Card Restricted Access", MessageSystem.MessageType.Center);
                                }
                            }
                            break;
                        }
                    case DoorType.Red:
                        {
                            if (playerSystem.keyCards[2])
                            {
                                if (tags[0] == tag)
                                {
                                    audioSystem.PlayAudioSource(popupSfx[0], 1, 1, 128);
                                    if (!grantedAccess)
                                    {
                                        messageSystem.SetMessage("Red Access Card Granted!", MessageSystem.MessageType.Center); grantedAccess = true;
                                    }
                                }
                                PlayDoorSound(DoorOpenSfx);
                                isOpen = false;
                                openDoor = true;
                            }
                            else
                            {
                                if (tags[0] == tag)
                                {
                                    audioSystem.PlayAudioSource(popupSfx[1], 1, 1, 128);
                                    messageSystem.SetMessage("Red Card Restricted Access", MessageSystem.MessageType.Center);
                                }
                            }
                            break;
                        }
                }
                break;
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (stringBuilder.Length > 0) stringBuilder.Clear();
        stringBuilder.Append(other.gameObject.tag);
       
        for (int t = 0; t < tags.Length; t++)
        {
            if (tags[t] == stringBuilder.ToString())
            {
                obstructions[t] = false;
                if (!ObstructorsActive())
                {
                    PlayDoorSound(DoorCloseSfx);
                    isOpen = true;
                    //SetNavLinks(false);
                    openDoor = true;
                }
                break;
            }
        }
      
    }
    private bool ObstructorsActive()
    {
        for (int o = 0; o < obstructions.Length; o++)
        {
            if (obstructions[o]) return true;
        }
        return false;
    }
    private void OpenDoor()
    {
        if (!openDoor) return;
        switch (directionRight)
        {
            case DirectionRight.Up: rightVector.y = maxRightVector.y; break;
            case DirectionRight.Right: rightVector.x = maxRightVector.x; break;
            case DirectionRight.UpRight: rightVector.x = maxRightVector.x; rightVector.y = maxRightVector.y; break;
        }
        Vector3 VectorR = isOpen ? Vector3.zero : rightVector;
        rightDoor.localPosition = Vector3.MoveTowards(rightDoor.localPosition, VectorR, Time.deltaTime * moveSpeed);

        switch (directionLeft)
        {
            case DirectionLeft.Down: leftVector.y = maxLeftVector.y; break;
            case DirectionLeft.Left: leftVector.x = maxLeftVector.x; break;
            case DirectionLeft.DownLeft: leftVector.x = maxLeftVector.x; leftVector.y = maxLeftVector.y; break;
        }
        Vector3 VectorL = isOpen ? Vector3.zero : leftVector;
        leftDoor.localPosition = Vector3.MoveTowards(leftDoor.localPosition, VectorL, Time.deltaTime * moveSpeed);

        if(leftDoor.localPosition == VectorL && rightDoor.localPosition == VectorR)
        {
            //if(!isOpen)
            //    SetNavLinks(true);
            PlayDoorSound(DoorBangSfx);
            openDoor = false;
        }
    }
}
