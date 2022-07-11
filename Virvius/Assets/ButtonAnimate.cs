using UnityEngine;
using UnityEngine.UI;

public class ButtonAnimate : MonoBehaviour
{
    [SerializeField]
    private Image buttonImage;
    [SerializeField]
    private Image buttonLogo;
    [SerializeField]
    private float[] frameRate = new float[2] 
    {
        0.06f,
        0.06f
    };
    private int[] curFrame = new int[2];
    private float[] frameTimer = new float[2];
    [SerializeField]
    private Sprite[] buttonSpriteArray; 
    [SerializeField]
    private Sprite[] logoSpriteArray;

    public bool animateLogo = true;
    public bool animateButton = true;
    private bool logoActive = false;
    private bool buttonActive = false;

    private float time;

    void Update()
    {
        time = Time.unscaledDeltaTime;
        LogoAnimation();
        ButtonAnimation();
    }
    public void StartAnimation(bool active)
    {
        for (int f = 0; f < 2; f++)
        {
            curFrame[f] = 0;
        }
        if (animateLogo)
        {
            buttonLogo.enabled = active;
            logoActive = active;
            buttonLogo.sprite = logoSpriteArray[curFrame[0]];
        }
        if (animateButton)
        {
            buttonActive = active;
            buttonImage.sprite = buttonSpriteArray[curFrame[1]];
        }
    }
    private void LogoAnimation()
    {
        if (!animateLogo) return;
        if (!logoActive) return;
        frameTimer[0] -= time;
        if (frameTimer[0] <= 0)
        {
            frameTimer[0] = frameRate[0];
            curFrame[0] = (curFrame[0] + 1) % logoSpriteArray.Length;
            buttonLogo.sprite = logoSpriteArray[curFrame[0]];
            if (curFrame[0] == buttonSpriteArray.Length - 1)
            {
                curFrame[0] = 0;
                buttonLogo.sprite = logoSpriteArray[curFrame[0]];
            }

        }
    }
    private void ButtonAnimation()
    {
        if (!animateButton) return;
        if (!buttonActive) return;
        frameTimer[1] -= time;
        if (frameTimer[1] <= 0)
        {
            frameTimer[1] = frameRate[1];
            curFrame[1] = (curFrame[1] + 1) % buttonSpriteArray.Length;
            buttonImage.sprite = buttonSpriteArray[curFrame[1]];
            if (curFrame[1] == buttonSpriteArray.Length - 1) buttonActive = false;
        }
    }
}
