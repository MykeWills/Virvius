using UnityEngine;

public class MultiSwitchSystem : MonoBehaviour
{
    private MessageSystem messageSystem;
    private AudioSystem audioSystem;
    [SerializeField]
    private string customMessage = "";
    [SerializeField]
    private bool setCustomFinalMessage = false;

    private string[] countMessage = new string[6]
    { 
        "Sequence Completed.",
        "One more to go.",
        "Two more to go.",
        "Three More to go.",
        "Four more to go.",
        "Five more to go."
    };
    public AudioClip completedSoundFx;
    [SerializeField]
    private AudioSource moveObjectSrc;
    [SerializeField]
    private AudioClip[] moveObjectSound = new AudioClip[3];
    public enum OpenDirection { X, Y, Z }
    [SerializeField]
    private OpenDirection openDirection;
    [SerializeField]
    private int switchAmount = 6;
    private int switchesPressed = 0;
    [SerializeField]
    private float moveSpeed = 20f;  
    [SerializeField]
    private float moveAmount = 20f;
    [SerializeField]
    private Transform[] doors = new Transform[2];
    private Vector3[] startPositions = new Vector3[2];
    private Vector3[] endPositions = new Vector3[2];
    private bool open = false;
    private bool doorsOpen = false;
    private void Awake()
    {
        audioSystem = AudioSystem.audioSystem;
        for (int d = 0; d < doors.Length; d++)
        {
            startPositions[d] = doors[d].localPosition;
        }
        switch (openDirection)
        {
            case OpenDirection.X:
                endPositions[0] = new Vector3(startPositions[0].x + moveAmount, startPositions[0].y, startPositions[0].z);
                endPositions[1] = new Vector3(startPositions[1].x - moveAmount, startPositions[1].y, startPositions[1].z);
                break;
            case OpenDirection.Y:
                endPositions[0] = new Vector3(startPositions[0].x, startPositions[0].y + moveAmount, startPositions[0].z);
                endPositions[1] = new Vector3(startPositions[1].x, startPositions[1].y - moveAmount, startPositions[1].z);
                break;
            case OpenDirection.Z:
                endPositions[0] = new Vector3(startPositions[0].x, startPositions[0].y, startPositions[0].z + moveAmount);
                endPositions[1] = new Vector3(startPositions[1].x, startPositions[1].y, startPositions[1].z - moveAmount);
                break;
        }
    }

    private void Start()
    {
      
    }
    void Update()
    {
        OpenDoors();
    }
    public void PressSwitch()
    {
        if (open) return;
        if (doorsOpen) return;
        if (messageSystem == null)
            messageSystem = PlayerSystem.playerSystem.GetComponent<MessageSystem>();
        switchesPressed--;
       
        if (switchesPressed < 1)
        {
            audioSystem.PlayAudioSource(completedSoundFx, 1, 1, 128);
            messageSystem.SetMessage(setCustomFinalMessage ? customMessage :countMessage[switchesPressed], MessageSystem.MessageType.Center);
            open = true;
            return;
        }
        else if (switchesPressed > 0)
            messageSystem.SetMessage(countMessage[switchesPressed], MessageSystem.MessageType.Center);

    }
    private void OpenDoors()
    {
        if (!open) return;
        if (moveObjectSrc != null)
        {
            if (moveObjectSrc.clip == moveObjectSound[0] && !moveObjectSrc.isPlaying)
            {
                moveObjectSrc.clip = moveObjectSound[1];
                moveObjectSrc.loop = true;
                moveObjectSrc.Play();
            }
        }
        for (int d = 0; d < doors.Length; d++)
        {
            doors[d].localPosition = Vector3.MoveTowards(doors[d].localPosition, endPositions[d], Time.deltaTime * moveSpeed);
        }
        if (doors[1].transform.localPosition == endPositions[1])
        {
            open = false;
            if (moveObjectSrc != null)
            {
                moveObjectSrc.clip = moveObjectSound[2];
                moveObjectSrc.loop = false;
                moveObjectSrc.Play();
            }
            doorsOpen = true;
        }
    }
    public void ResetObject()
    {
        if(open) open = false;
        if(doorsOpen) doorsOpen = false;
        if(switchesPressed != switchAmount) switchesPressed = switchAmount;
        for (int d = 0; d < doors.Length; d++)
        {
            if (doors[d].localPosition != startPositions[d]) doors[d].localPosition = startPositions[d];
        }
    }
}
