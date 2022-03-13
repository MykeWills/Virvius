using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraneDropSystem : MonoBehaviour
{
    private MessageSystem messageSystem;
    [SerializeField]
    private MoveCraneDoor moveCraneDoor;
    [SerializeField]
    private Vector3[] dropPosition = new Vector3[2];
    [SerializeField]
    private GameObject[] crates = new GameObject[2];
    private Rigidbody[] crateRb;
    [SerializeField]
    private Vector3 collectPosition;
    private Vector3 nextPosition;
    private Vector3 returnPosition;
    private Vector3 startPosition;
    [SerializeField]
    private Vector3[] openRotation = new Vector3[2];
    private Vector3 nextRotation;
    [SerializeField]
    private float moveSpeed = 30.0f;
    [SerializeField]
    private float rotationSpeed = 10.0f;
    private AudioSource audioSrc;
    [SerializeField]
    private AudioClip[] craneFxs = new AudioClip[2];
    [SerializeField]
    private Transform[] clampArms = new Transform[2];
    private bool isMoving = false;
    private bool goCollect = false;
    private bool moveClamp = false;
    private bool isOpen = false;
    private bool isReturned = true;
    private bool[] dropFinished = new bool[2];
    private float time = 0;
    private float waitTime = 1.5f;
    private float waitTimer = 0;
    private bool isWaiting = false;
    [HideInInspector]
    public bool craneActive = false;

    private int dropIndex = 0;

    private Vector3 cratePosition;
    
    private void Start()
    {
        messageSystem = MessageSystem.messageSystem;
        audioSrc = GetComponent<AudioSource>();
        crateRb = new Rigidbody[crates.Length];
        cratePosition = new Vector3(0, -13.5f, 0);
        for (int rb = 0; rb < crates.Length; rb++)
        {
            crateRb[rb] = crates[rb].GetComponent<Rigidbody>();
            crateRb[rb].useGravity = false;
            crateRb[rb].transform.localPosition = cratePosition;
            if(rb != 0)
                crateRb[rb].gameObject.SetActive(false);
            else 
                crateRb[rb].gameObject.SetActive(true);
        }
        startPosition = transform.position;
    }

    // Update is called once per frame
    private void Update()
    { 
        // time in seconds
        time = Time.deltaTime;
        Wait();
        if (isWaiting) return;
        Clamp();
        Move();
     
    }
    private void Move()
    {
        if (!isMoving) return;

        //crane moving to drop point
        if (!goCollect)
        {
            //MOVING THE CRANE TO DROP POSITION====>>>>>>>>>>>>>
            transform.position = Vector3.MoveTowards(transform.position, nextPosition, time * (moveSpeed / 3) * 2);
            if (transform.position == nextPosition)
            {
                audioSrc.clip = craneFxs[0];
                audioSrc.loop = false;
                audioSrc.Play();
                //Start moving the clamp
                moveClamp = true;
                //Set the crank rotation to [OPEN]
                isOpen = true;
                //stop moving the crane
                isMoving = false;
                //CRANK IS NOW OPENING===============>>>>>>>>>>>>>>>>
                if (isOpen)
                {
                    crateRb[dropIndex].useGravity = true;
                    crateRb[dropIndex].transform.parent = transform.parent;
                }
            }
        }
        //crane moving to collect point
        else
        {
            //Set the return position of the crane to [COLLECT]/[GOTOSTART]
            if (returnPosition != (!isReturned ? collectPosition : startPosition))
                returnPosition = (!isReturned ? collectPosition : startPosition);
            //MOVING THE CRANE TO RETURN POSITION====>>>>>>>>>>>>>
            transform.position = Vector3.MoveTowards(transform.position, returnPosition, time * moveSpeed);
            if (transform.position == returnPosition)
            {
                if (!isReturned)
                {
                    audioSrc.clip = craneFxs[1];
                    audioSrc.loop = false;
                    audioSrc.Play();
                    isWaiting = true;
                }
                else
                {
                    audioSrc.clip = craneFxs[1];
                    audioSrc.loop = false;
                    audioSrc.Play();
                    //crane is finished collecting
                    goCollect = false;
                    //stop moving the crane
                    isMoving = false;
                    //crane is not active anymore
                    craneActive = false;
                    if(dropIndex == 2) 
                        messageSystem.SetMessage("Crane is Offline.", MessageSystem.MessageType.Center);
                }
            }
        }
      
    }
    private void Wait()
    {
        if (!isWaiting) return;
        //decrease waiting period
        waitTimer -= time;
        waitTimer = Mathf.Clamp(waitTimer, 0, waitTime);
        //finished waiting
        if (waitTimer == 0)
        {
            //reset the waitTimer
            waitTimer = waitTime;
            //while CLAMP is open
            if (isOpen)
            {
                dropIndex++;
                if (dropIndex > 2) dropIndex = 2;
                //Set the crank rotation to [CLOSE]
                isOpen = false;
                //activate clamp
                moveClamp = true;
                audioSrc.clip = craneFxs[0];
                audioSrc.loop = true;
                audioSrc.Play();
            }
            //while CLAMP is closed
            else
            {
                if (!goCollect)
                {
                    //Switch crane attributes to [COLLECT] parcel
                    goCollect = true;
                    //start moving the crane to collect parcel
                    isMoving = true;
                    audioSrc.clip = craneFxs[0];
                    audioSrc.loop = true;
                    audioSrc.Play();
                }
                else
                {
                    if (dropIndex < 2)
                    {
                        crateRb[dropIndex].transform.parent = transform;
                        crateRb[dropIndex].transform.localPosition = cratePosition;
                        crateRb[dropIndex].useGravity = false;
                        crateRb[dropIndex].gameObject.SetActive(true);

                    }
                    //crane is now moving to start position
                    isReturned = true;
                    //keep crane moving
                    isMoving = true;
                    audioSrc.clip = craneFxs[0];
                    audioSrc.loop = true;
                    audioSrc.Play();
                }
            }
            isWaiting = false;
        }
    }
    private void Clamp()
    {
        if (!moveClamp) return;
        //Set the rotation to [OPEN/CLOSE]
        if (nextRotation != openRotation[isOpen ? 1 : 0])
            nextRotation = openRotation[isOpen ? 1 : 0];
        //rotate the clamp to open/close
        //clamp arm left = {clamp arm a = 1 = reverse rotation}
        for (int a = 0; a < 2; a++)
            clampArms[a].localRotation = Quaternion.RotateTowards(clampArms[a].localRotation, Quaternion.Euler((a == 1) ? nextRotation : -nextRotation), time * rotationSpeed);
        //rotation has finished
        if (clampArms[1].localRotation == Quaternion.Euler(nextRotation))
        {
            audioSrc.clip = craneFxs[1];
            audioSrc.loop = false;
            audioSrc.Play();
            //Stop moving the clamp
            moveClamp = false;
            //Start waiting for parcel drop
            isWaiting = true;

        }
       
    }
    public void SelectDropPosition(int index)
    {
      
        if (craneActive) return;
        if (dropIndex == 2) return;
        if (dropFinished[index]) return;
        if (audioSrc == null) Start();
        audioSrc.clip = craneFxs[0];
        audioSrc.loop = true;
        audioSrc.Play();
        //Active the Crane
        craneActive = true;
        //clamp is not open
        isOpen = false;
        //Switch crane attributes to [DROP] parcel
        goCollect = false;
        //crane is no longer in return point
        isReturned = false;
        //Set the current destination drop point
        nextPosition = dropPosition[index];
        //Start moving the crane
        isMoving = true;
        dropFinished[index] = true;
    }
    public void ResetObject()
    {
        audioSrc.Stop();
        for (int a = 0; a < 2; a++)
            clampArms[a].rotation = Quaternion.identity;

        for (int rb = 0; rb < crates.Length; rb++)
        {
            crateRb[rb].transform.parent = transform;
            crateRb[rb].useGravity = false;
            crateRb[rb].transform.localPosition = cratePosition;
            if (rb != 0)
                crateRb[rb].gameObject.SetActive(false);
            else
                crateRb[rb].gameObject.SetActive(true);
        }
        for (int d = 0; d < dropFinished.Length; d++)
            dropFinished[d] = false;
        moveClamp = false;
        isWaiting = false;
        dropIndex = 0;
        waitTimer = waitTime;
        craneActive = false;
        isOpen = false;
        goCollect = false;
        isReturned = false;
        transform.position = startPosition;
        isMoving = false;
        moveCraneDoor.ResetObject();
    }
}
