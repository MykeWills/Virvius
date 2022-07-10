using UnityEngine;
using UnityEngine.AI;

public class AmbushSystem : MonoBehaviour
{
    public enum Direction { Forward, Back, Left, Right, Up, Down }
    public Direction slideInDirection;
    public float slideDirAmt = 20;
    public float moveSpeed = 2f;
    public AudioClip[] doorSoundFx = new AudioClip[2];
    private Vector3[] curPosition;
    private Vector3[] slideDirection;
    private AudioSource[] audioSrc;
    private bool open = false;
    private int doorCount = 0;
    public bool ambushActivated = false;
    private bool[] soundActive1;
    private bool[] soundActive2;
    [SerializeField]
    private Transform[] doors;
    void Start()
    {
        curPosition = new Vector3[transform.childCount];
        slideDirection = new Vector3[transform.childCount];
        audioSrc = new AudioSource[transform.childCount];
        soundActive1 = new bool[transform.childCount];
        soundActive2 = new bool[transform.childCount];
        for (int t = 0; t < doors.Length; t++)
        {
            audioSrc[t] = doors[t].GetComponent<AudioSource>();
            curPosition[t] = doors[t].localPosition;

            switch (slideInDirection)
            {
                case Direction.Forward: slideDirection[t] = new Vector3(curPosition[t].x, curPosition[t].y, curPosition[t].z + slideDirAmt); break;
                case Direction.Back: slideDirection[t] = new Vector3(curPosition[t].x, curPosition[t].y, curPosition[t].z - slideDirAmt); break;
                case Direction.Left: slideDirection[t] = new Vector3(curPosition[t].x - slideDirAmt, curPosition[t].y, curPosition[t].z); break;
                case Direction.Right: slideDirection[t] = new Vector3(curPosition[t].x + slideDirAmt, curPosition[t].y, curPosition[t].z); break;
                case Direction.Up: slideDirection[t] = new Vector3(curPosition[t].x, curPosition[t].y + slideDirAmt, curPosition[t].z); break;
                case Direction.Down: slideDirection[t] = new Vector3(curPosition[t].x, curPosition[t].y - slideDirAmt, curPosition[t].z); break;
            }
        }
    }
    void Update()
    {
        Opendoors();
    }
    private void Opendoors()
    {
        if (!open) return;
        for (int t = 0; t < doors.Length; t++)
        {
            if (!soundActive1[t])
            {
                audioSrc[t].clip = doorSoundFx[1];
                audioSrc[t].loop = true;
                audioSrc[t].Play();
                soundActive1[t] = true;
            }
            doors[t].transform.localPosition = Vector3.MoveTowards(doors[t].transform.localPosition, slideDirection[t], Time.deltaTime * (moveSpeed * 2));
            if (doors[t].transform.localPosition == slideDirection[t])
            {
                if (!soundActive2[t])
                {
                    audioSrc[t].clip = null;
                    audioSrc[t].Stop();
                    audioSrc[t].loop = false;
                    audioSrc[t].PlayOneShot(doorSoundFx[0]);
                    soundActive2[t] = true;
                    //SetNavLinks(true);
                }
                doorCount++;
            }
        }
        if (doorCount == doors.Length)
        {
            open = false;
            ambushActivated = true;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player") && !open && !ambushActivated) open = true;
    }
    public void ResetObject()
    {
        for (int t = 0; t < doors.Length; t++)
        {
            doors[t].transform.localPosition = curPosition[t];
            audioSrc[t].clip = null;
            audioSrc[t].Stop();
            audioSrc[t].loop = false;
            soundActive1[t] = false;
            soundActive2[t] = false;
        }
        ambushActivated = false;
        doorCount = 0;
        open = false;
    }
    public void ActivateObjectState()
    {
        for (int t = 0; t < doors.Length; t++)
        {
            doors[t].transform.localPosition = slideDirection[t];
            if (doors[t].transform.localPosition == slideDirection[t])
            {
                if (!soundActive2[t])
                {
                    audioSrc[t].clip = null;
                    audioSrc[t].Stop();
                    audioSrc[t].loop = false;
                    audioSrc[t].PlayOneShot(doorSoundFx[0]);
                    soundActive2[t] = true;
                }
                doorCount++;
            }
        }
        ambushActivated = true;
    }
}
