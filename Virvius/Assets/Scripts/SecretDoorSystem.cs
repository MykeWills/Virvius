using System.Text;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(AudioSource))]
public class SecretDoorSystem : MonoBehaviour
{
   
    public enum Direction { Forward, Back, Left, Right, Up, Down };
    StringBuilder tagSb = new StringBuilder();
    [HideInInspector]
    public bool open = false;
    private bool returnToOrigin = true;
    private float originTime = 4f;
    private float originTimer;
    private bool pushed = false;
    public float pushDirAmt = 1f;
    public float slideDirAmt = 20f;
    public float moveSpeed = 5f;
    public AudioClip[] doorSoundFx = new AudioClip[2];
    private float waitTime = 2f;
    private float waitTimer;
    private bool wait = false;
    public Direction pushInDirection;
    public Direction slideInDirection;
    private Vector3 pushDirection = Vector3.zero;
    private Vector3 slideDirection = Vector3.zero;
    private Vector3 curPosition;
    private AudioSource audioSrc;
    private bool[] soundActive = new bool[2];
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
        "RocketBulletMini",
   };
    [Space]
    [Header("Surface Linking")]
    [SerializeField]
    private NavMeshLink[] navMeshLinks;

    public void Start()
    {
        audioSrc = GetComponent<AudioSource>();
        curPosition = transform.localPosition;
        switch (pushInDirection)
        {
            case Direction.Forward: pushDirection = new Vector3(curPosition.x, curPosition.y, curPosition.z + pushDirAmt); break;
            case Direction.Back: pushDirection = new Vector3(curPosition.x, curPosition.y, curPosition.z - pushDirAmt); break;
            case Direction.Left: pushDirection = new Vector3(curPosition.x - pushDirAmt, curPosition.y, curPosition.z); break;
            case Direction.Right: pushDirection = new Vector3(curPosition.x + pushDirAmt, curPosition.y, curPosition.z); break;
            case Direction.Up: pushDirection = new Vector3(curPosition.x, curPosition.y + pushDirAmt, curPosition.z); break;
            case Direction.Down: pushDirection = new Vector3(curPosition.x, curPosition.y - pushDirAmt, curPosition.z); break;
        }
        switch (slideInDirection)
        {
            case Direction.Forward: slideDirection = new Vector3(pushDirection.x, pushDirection.y, pushDirection.z + slideDirAmt); break;
            case Direction.Back: slideDirection = new Vector3(pushDirection.x, pushDirection.y, pushDirection.z - slideDirAmt); break;
            case Direction.Left: slideDirection = new Vector3(pushDirection.x - slideDirAmt, pushDirection.y, pushDirection.z); break;
            case Direction.Right: slideDirection = new Vector3(pushDirection.x + slideDirAmt, pushDirection.y, pushDirection.z); break;
            case Direction.Up: slideDirection = new Vector3(pushDirection.x, pushDirection.y + slideDirAmt, pushDirection.z); break;
            case Direction.Down: slideDirection = new Vector3(pushDirection.x, pushDirection.y - slideDirAmt, pushDirection.z); break;
        }
        waitTimer = waitTime;
        SetNavLinks(false);
    }
    void Update()
    {
        Opendoor();
    }
    private void SetNavLinks(bool active)
    {
        if (navMeshLinks.Length > 0)
        {
            for (int l = 0; l < navMeshLinks.Length; l++)
                navMeshLinks[l].enabled = active;
        }
    }
    private void Opendoor()
    {
        if (!open) return;
        float time = Time.deltaTime;
        if (!returnToOrigin)
        {
            if (!pushed)
            {
                if (!soundActive[0])
                {
                    audioSrc.PlayOneShot(doorSoundFx[0]);
                    audioSrc.loop = false;
                    soundActive[0] = true;
                }
                transform.localPosition = Vector3.MoveTowards(transform.localPosition, pushDirection, time * moveSpeed);
                if (transform.localPosition == pushDirection)
                    wait = true;
                if (wait)
                {
                    waitTimer -= time;
                    float clamp = Mathf.Clamp(waitTimer, 0, waitTime);
                    if (clamp == 0)
                    {
                        for (int s = 0; s < soundActive.Length; s++)
                            soundActive[s] = false;
                        pushed = true;
                    }
                }
            }
            else
            {
                if (!soundActive[1])
                {
                    audioSrc.loop = true;
                    audioSrc.clip = doorSoundFx[1];
                    audioSrc.Play();
                    soundActive[1] = true;
                }
                transform.localPosition = Vector3.MoveTowards(transform.localPosition, slideDirection, time * (moveSpeed * 2));
                if (transform.localPosition == slideDirection)
                {
                    if (!soundActive[0])
                    {
                        audioSrc.clip = null;
                        audioSrc.Stop();
                        audioSrc.loop = false;
                        audioSrc.PlayOneShot(doorSoundFx[0]);
                        soundActive[0] = true;
                        SetNavLinks(true);
                    }
                    returnToOrigin = true;
                }
            }
        }
        else
        {
            originTimer -= time;
            originTimer = Mathf.Clamp(originTimer, 0, originTime);
            if (originTimer != 0) return;
            if (pushed)
            {
                if (soundActive[0])
                {
                    audioSrc.PlayOneShot(doorSoundFx[1]);
                    audioSrc.loop = true;
                    soundActive[0] = false;
                    SetNavLinks(false);
                }
                transform.localPosition = Vector3.MoveTowards(transform.localPosition, pushDirection, time * (moveSpeed * 2));
                if (transform.localPosition == pushDirection)
                    wait = false;
                if (!wait)
                {
                    waitTimer -= time;
                    float clamp = Mathf.Clamp(waitTimer, 0, waitTime);
                    if (clamp == 0)
                        pushed = false;
                }
            }
            else
            {
                if (soundActive[1])
                {
                    audioSrc.loop = false;
                    audioSrc.clip = doorSoundFx[0];
                    audioSrc.Play();
                    soundActive[1] = false;
                }
                transform.localPosition = Vector3.MoveTowards(transform.localPosition, curPosition, time * moveSpeed);
                if (transform.localPosition == curPosition)
                {
                    if (!soundActive[0])
                    {
                        audioSrc.clip = null;
                        audioSrc.Stop();
                        audioSrc.loop = false;
                        audioSrc.PlayOneShot(doorSoundFx[0]);
                        soundActive[0] = false;
                    }
                    open = false;
                    pushed = false;
                    returnToOrigin = false;
                    originTimer = originTime;
                    waitTimer = waitTime;
                    for (int s = 0; s < soundActive.Length; s++)
                        soundActive[s] = false;
                }
            }
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (open) return;
        if(tagSb.Length > 0) tagSb.Clear();
        tagSb = tagSb.Append(collision.gameObject.tag);
        for(int t = 0; t < tags.Length; t++)
        {
            if(tags[t] == tagSb.ToString())
            {
                if (!open) open = true;
                if (t != 0 && t != 8) collision.gameObject.SetActive(false);
                break;
            }
        }
    }
    public void ResetObject()
    {
        SetNavLinks(false);
        transform.localPosition = curPosition;
        audioSrc.Stop();
        for (int s = 0; s < soundActive.Length; s++)
            soundActive[s] = false;
        open = false;
        pushed = false;
        wait = false;
        returnToOrigin = false;
        originTimer = originTime;
        waitTimer = waitTime;
    }
}
