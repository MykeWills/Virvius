using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeText : MonoBehaviour
{
    public Text nextSlot;
    private Text curText;
    private FadeText fadeText;
    private float alpha = 0;
    [HideInInspector]
    public bool fade = false;
    [HideInInspector]
    public bool wait = false;
    [HideInInspector]
    public bool[] buildMessageAttribute = new bool[3];
    [HideInInspector]
    public float waitTime = 1.0f;
    [HideInInspector]
    public float waitTimer;
    private void Start()
    {
        curText = GetComponent<Text>();
        if(nextSlot != null)
            fadeText = nextSlot.GetComponent<FadeText>();
        waitTimer = waitTime;
    }

    // Update is called once per frame
    void Update()
    {
        Fade();
        Wait();
        Build();
    }
    private void Fade()
    {
        if (!fade) return;
        alpha += 0.05f;
        float clampValue = Mathf.Clamp(alpha, 0, 1f);
        curText.color = new Color(curText.color.r, curText.color.g, curText.color.b, Mathf.Lerp(0, 1, clampValue));
        if (clampValue == 1) { alpha = 0; fade = false;  wait = true; }
    }
    private void Wait()
    {
        if (!wait) return;
        waitTimer -= Time.deltaTime * 1.5f;
        if (nextSlot != null)
        {
            if (!nextSlot.enabled)
            {
                nextSlot.text = curText.text;
                fadeText.curText.color = Color.white;
                nextSlot.enabled = true;
                fadeText.wait = true;
                wait = false;
                waitTimer = waitTime;
                curText.color = new Color(curText.color.r, curText.color.g, curText.color.b, 0);
                curText.text = null;
                curText.enabled = false;
            }
        }
        float clampTime = Mathf.Clamp(waitTimer, 0, waitTime);
        if (clampTime == 0)
        {
            wait = false;
            waitTimer = waitTime;
            curText.color = new Color(curText.color.r, curText.color.g, curText.color.b, 0);
            curText.text = null;
            curText.enabled = false;
        }
    }
    private void Build()
    {
        if (buildMessageAttribute[0])
        {
            alpha += 0.05f;
            float clampValue = Mathf.Clamp(alpha, 0, 1f);
            curText.color = new Color(curText.color.r, curText.color.g, curText.color.b, Mathf.Lerp(0, 1, clampValue));
            if (clampValue == 1)
            {
                alpha = 0;
                buildMessageAttribute[0] = false;
            }
        }
        if (buildMessageAttribute[1])
        {
            waitTimer -= Time.deltaTime * 1.5f;
            float clampTime = Mathf.Clamp(waitTimer, 0, waitTime);
            if (clampTime == 0)
            {
                buildMessageAttribute[1] = false;
                waitTimer = waitTime;
                buildMessageAttribute[2] = true;
            }
        }
        if (buildMessageAttribute[2])
        {
            alpha += 0.05f;
            float clampAValue = Mathf.Clamp(alpha, 0, 1f);
            curText.color = new Color(curText.color.r, curText.color.g, curText.color.b, Mathf.Lerp(1, 0, clampAValue));
            if (clampAValue == 1)
            {
                alpha = 0;
                curText.text = null;
                curText.enabled = false;
                buildMessageAttribute[2] = false;
            }
        }
    }
    public void ResetTextParameters()
    {
        alpha = 0; 
        fade = false;
        wait = false;
        waitTimer = waitTime;
        curText.color = new Color(curText.color.r, curText.color.g, curText.color.b, 0);
        curText.text = null;
        curText.enabled = false;
    }
}
