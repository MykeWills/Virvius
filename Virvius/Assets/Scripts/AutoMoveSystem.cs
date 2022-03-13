using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoMoveSystem : MonoBehaviour
{
    private AudioSystem audioSystem;
    private MessageSystem messageSystem;
    public AudioClip messageSoundFx;
    private bool isMoving = false;
    private bool isActive = false;
    public float maxXYZ;
    public float minXYZ;
    public float moveSpeed;
    public float waitTime = 3;
    private float waitTimer;
    private bool wait;
    private bool hasReturned = false;
    public string[] activationMessage = new string[2];
    public enum Direction { AbscissaX, OrdinateY, ApplicateZ };
    public Direction moveDirection;
    private Vector3 updatedPosition = Vector3.zero;
    void Start()
    {
        audioSystem = AudioSystem.audioSystem;
        waitTimer = waitTime;
        updatedPosition = transform.localPosition;
    }

    // Update is called once per frame
    void FixedUpdate()
    { 
        //All moving platforms with Character controller must remain in FixedUpdate otherwise player will fall off platform
        if (!isActive) return;
        Move();
        Wait();
    }
    private void Move()
    {
        if (!isMoving) return;
        switch (moveDirection)
        {
            case Direction.AbscissaX: updatedPosition.x = !hasReturned ? maxXYZ : minXYZ; break;
            case Direction.OrdinateY: updatedPosition.y = !hasReturned ? maxXYZ : minXYZ; break;
            case Direction.ApplicateZ: updatedPosition.z = !hasReturned ? maxXYZ : minXYZ; break;
        }
        transform.localPosition = Vector3.MoveTowards(transform.localPosition, updatedPosition, Time.deltaTime * moveSpeed);
        if (transform.localPosition == updatedPosition)
        {
            wait = true;
            isMoving = false;
        }
    }
    private void Wait()
    {
        if (!wait) return;
        waitTimer -= Time.deltaTime;
        float clamp = Mathf.Clamp(waitTimer, 0, waitTime);
        if(clamp == 0)
        {
            hasReturned = !hasReturned;
            waitTimer = waitTime;
            wait = false;
            isMoving = true;
        }
    }
    public void ActivateMovingObject(bool active)
    {
        if (messageSystem == null)
            messageSystem = PlayerSystem.playerSystem.GetComponent<MessageSystem>();
        if (active == true)
        {
            audioSystem.PlayAudioSource(messageSoundFx, 1, 1, 128);
            messageSystem.SetMessage(activationMessage[0], MessageSystem.MessageType.Center);
        }
        else
        {
            messageSystem.SetMessage(activationMessage[1], MessageSystem.MessageType.Center);
        }
        isActive = active; 
        isMoving = active;
        wait = false;
        waitTimer = waitTime;
    }
}
