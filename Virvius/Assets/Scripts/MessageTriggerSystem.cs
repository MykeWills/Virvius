using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MessageTriggerSystem : MonoBehaviour
{
    private MessageSystem messageSystem;
    private AudioSystem audioSystem;
    public enum TriggerType { Instance, Continuous };
    private BoxCollider boxCollider;
    [Header("Set the message to pop up on screen")]
    public string triggerMessage;
    [Space]
    [Header("Set the Message pop up amount")]
    public TriggerType triggerType;
    [Space]
    [Header("Set speed of message pop up")]
    public float repeatTime = 2f;
    [Space]
    [Header("0 for Instance | 1 for Continuous")]
    public AudioClip[] triggerSounds = new AudioClip[2];
    private float repeatTimer;
    private bool repeat = false;
    private bool shutOffMessage = false;
    [SerializeField]
    private GameObject activeObject;
    private void Start()
    {
        messageSystem = MessageSystem.messageSystem;
        audioSystem = AudioSystem.audioSystem;
        boxCollider = GetComponent<BoxCollider>();
    }
    private void Update()
    {
        if (shutOffMessage) return;
        if (!repeat) return;
        repeatTimer -= Time.deltaTime;
        float clamp = Mathf.Clamp(repeatTimer, 0, repeatTime);
        if (clamp == 0)
        {
            repeatTimer = repeatTime;
            messageSystem.SetMessage(triggerMessage, MessageSystem.MessageType.Center);
            audioSystem.PlayAudioSource(triggerSounds[1], 1, 1, 128);
        }
    }
    public void ShutMessageOff(bool active)
    {
        shutOffMessage = active;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
            SetTriggerType(triggerType);
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
            repeat = false;
    }
    private void SetTriggerType(TriggerType type)
    {
        switch (type)
        {
            case TriggerType.Instance:
                {
                    if (activeObject != null)
                    {
                        if (!activeObject.activeInHierarchy) ShutMessageOff(true);
                    }
                    if (shutOffMessage) return;
                    messageSystem.SetMessage(triggerMessage, MessageSystem.MessageType.Center);
                    audioSystem.PlayAudioSource(triggerSounds[0], 1, 1, 128);
                    boxCollider.enabled = false;
                    break;
                }
            case TriggerType.Continuous:
                {
                    if(activeObject != null)
                    {
                        if (!activeObject.activeInHierarchy) ShutMessageOff(true);
                    }
                    if (shutOffMessage) return;
                    repeatTimer = repeatTime;
                    messageSystem.SetMessage(triggerMessage, MessageSystem.MessageType.Center);
                    audioSystem.PlayAudioSource(triggerSounds[1], 1, 1, 128);
                    repeat = true;
                    break;
                }
        }
    }
    public void ResetObject()
    {
        repeat = false;
        repeatTimer = repeatTime;
        boxCollider.enabled = true;
        shutOffMessage = false;
    }
}
