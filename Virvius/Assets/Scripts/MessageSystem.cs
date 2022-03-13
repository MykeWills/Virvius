using System.Text;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MessageSystem : MonoBehaviour
{
    public static MessageSystem messageSystem;
    private OptionsSystem optionsSystem;
    private PlayerSystem playerSystem;
    private StringBuilder sbT = new StringBuilder();
    private StringBuilder sbC = new StringBuilder();
    public Text[] messageTopSlots = new Text[5];
    public Text[] messageTopAltSlots = new Text[5];
    public Text messageCenterText;
    public Text messageDisplayText;
    private FadeText fadeTText;
    private FadeText fadeCText;
    public enum MessageType { Center, Top, Display };
    [HideInInspector]
    public MessageType messageType;
    private bool buildMessage = false;
    private float buildTime = 0.01f;
    private float buildTimer = 0f;
    private string curMessage;
    private int charIndex = 0;
    private bool[] displayMessage = new bool[3];
    
    private float displaySpeed = 5;
    private Vector3 startPos = new Vector3();
    private Vector3 endPos;
    private float alpha = 0;
    private RectTransform rectTransform;
    private void Awake()
    {
        messageSystem = this;
    }
    public void Start()
    {
        optionsSystem = OptionsSystem.optionsSystem;
        playerSystem = PlayerSystem.playerSystem;
        rectTransform = messageDisplayText.GetComponent<RectTransform>();
        startPos = rectTransform.anchoredPosition;
        float distance = Screen.height / 36;
        endPos = new Vector3(startPos.x, startPos.y + distance / 2, startPos.z);
    }
    private void Update()
    {
        BuildCenterMessage();
        DisplayMainMessage();
    }
    // Update is called once per frame
    public void SetMessage(string message, MessageType messageType)
    {
        if(optionsSystem == null)
            optionsSystem = OptionsSystem.optionsSystem;
        switch (messageType)
        {
            // static message type that will shut off over time.
            case MessageType.Top:
                {
                    if (!optionsSystem.showMessages) return;
                    sbT.Clear();
                    sbT.Append(message);
                    Text messageTopText = AccessSlot();
                    fadeTText = messageTopText.GetComponent<FadeText>();
                    messageTopText.text = sbT.ToString();
                    messageTopText.enabled = true;
                    fadeTText.waitTimer = fadeTText.waitTime;
                    fadeTText.fade = true;
                    break;
                } 
            // dynamically built message then faded out over time.
            case MessageType.Center:
                {
                    if (!optionsSystem.showMessages) return;
                    sbC.Clear();
                    charIndex = 0;
                    curMessage = message;
                    buildTimer = buildTime;
                    buildMessage = true;
                    messageCenterText.enabled = true;
                    messageCenterText.text = null;
                    fadeCText = messageCenterText.GetComponent<FadeText>();
                    fadeCText.waitTimer = fadeCText.waitTime;
                    fadeCText.buildMessageAttribute[0] = true;
                    break;
                }
            case MessageType.Display:
                {
                    if (!optionsSystem.showMessages) return;
                    messageDisplayText.text = message;
                    messageDisplayText.enabled = true;
                    displayMessage[0] = true;
                    displayMessage[1] = true;
                    break;
                }
        }
    }
    private Text AccessSlot()
    {
        Text[] messages = new Text[messageTopSlots.Length];
        if (playerSystem.versionIndex > 2)
            messages = messageTopAltSlots;
        else messages = messageTopSlots;
        for (int s = 0; s < messages.Length; s++)
        {
            if (!messages[s].enabled)
                return messages[s];
        }
        for (int s = 0; s < messages.Length - 1; s++)
        {
            messages[s].text = messages[s + 1].text;
        }
        return messages[4];
    }
    public void EraseMessages()
    {
        Text[] messages = new Text[messageTopSlots.Length];
        if (playerSystem.versionIndex > 2)
            messages = messageTopAltSlots;
        else messages = messageTopSlots;
        for (int s = 0; s < messages.Length; s++)
        {
            if (messages[s].enabled)
            {
                FadeText slot = messages[s].GetComponent<FadeText>();
                slot.ResetTextParameters();
                messages[s].text = null;
                messages[s].enabled = false;
            }
        }
    }
    public void BuildCenterMessage()
    {
        if (!buildMessage) return;
        buildTimer -= Time.deltaTime;
        float clamp = Mathf.Clamp(buildTimer, 0, buildTime);
        if (clamp == 0)
        {
            if (charIndex < curMessage.Length)
            {
                sbC.Append(curMessage[charIndex]);
                messageCenterText.text = sbC.ToString();
                buildTimer = buildTime;
                charIndex++;
            }
        }
        if (messageCenterText.text == curMessage)
        {
            fadeCText.buildMessageAttribute[1] = true;
            buildMessage = false;
        }
    }
    public void DisplayMainMessage()
    {
        if (displayMessage[0])
        {
            alpha += 0.04f;
            float a = Mathf.Clamp(alpha, 0, 1);
            Color fade = new Color(messageDisplayText.color.r, messageDisplayText.color.g, messageDisplayText.color.b, Mathf.Lerp(0, 1, a));
            messageDisplayText.color = fade;
            if (a == 1)
            {
                alpha = 0;
                displayMessage[0] = false;
            }
        }
        if (displayMessage[1])
        {
            Vector3 pos = Vector3.MoveTowards(rectTransform.anchoredPosition, endPos, Time.deltaTime * displaySpeed);
            rectTransform.anchoredPosition = pos;
            Vector3 newPos = new Vector3(rectTransform.anchoredPosition.x, rectTransform.anchoredPosition.y, endPos.z);
            if(newPos == endPos)
            {
                displayMessage[1] = false;
                displayMessage[2] = true;
            }
        }
        if (displayMessage[2])
        {
            alpha += 0.02f;
            float a = Mathf.Clamp(alpha, 0, 1);
            Color fade = new Color(messageDisplayText.color.r, messageDisplayText.color.g, messageDisplayText.color.b, Mathf.Lerp(1, 0, a));
            messageDisplayText.color = fade;
            if (a == 1)
            {
                rectTransform.anchoredPosition = startPos;
                messageDisplayText.enabled = false;
                messageDisplayText.text = null;
                alpha = 0;
                displayMessage[2] = false;
            }
        }

    }
}
