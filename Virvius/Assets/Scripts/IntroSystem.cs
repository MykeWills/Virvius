using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using Rewired;

public class IntroSystem : MonoBehaviour
{
    public static IntroSystem introSystem;
    [SerializeField]
    private AudioClip bootSfx;    
    [SerializeField]
    private AudioClip startMenuMusic;
    private AudioSystem audioSystem;
    [SerializeField]
    private OptionsSystem optionsSystem;
    [SerializeField]
    private GameObject intro;
    [SerializeField]
    private GameObject menu;
    [SerializeField]
    private GameObject mainSelection;
    [SerializeField]
    private GameObject load;
    [SerializeField]
    private Image bootImage;
    [SerializeField]
    private Image bootVideo;
    [SerializeField]
    private Sprite[] bootSprites = new Sprite[3];
    [SerializeField]
    private Image flashStartShot;
    private bool flashScreen = false;
    private bool switchFlash = false;
    private float flashTimer = 0;
    private VideoPlayer videoPlayer;
    [SerializeField]
    private VideoClip[] videoClips = new VideoClip[3];
    private AudioSource videoAudioSrc;
    [SerializeField]
    private RenderTexture renderTexture;
    private Player inputPlayer;
    private bool fadeOut = true;
    [SerializeField]
    private Text pressStartText;
    private float time = 0;
    private float fadeInterval = 0;
    [SerializeField]
    private Image blackScreen;
    [HideInInspector]
    public bool isStartPressed = false;
    [SerializeField]
    private float waitTime = 5;
    private float waitTimer;
    private bool isWaiting = false;
    private int clipIndex = 0;

    private bool[] screenActive = new bool[5] { false, true, true, true, true };

    private void Awake()
    {
        introSystem = this;
    }
    void Start()
    {
        audioSystem = AudioSystem.audioSystem;
        videoAudioSrc = GetComponent<AudioSource>();
        videoPlayer = GetComponent<VideoPlayer>();
        inputPlayer = ReInput.players.GetPlayer(0);
        ClearOutRenderTexture(renderTexture);
        bootVideo.enabled = false;
        videoPlayer.playOnAwake = false;
        videoPlayer.isLooping = false;
        videoPlayer.clip = videoClips[0];
        videoPlayer.Prepare();
        pressStartText.enabled = false;
        waitTimer = waitTime;
        bootImage.sprite = bootSprites[0];
        videoAudioSrc.volume = 0;
        optionsSystem.LoadOptions();
    }

    void Update()
    {
        time = Time.deltaTime;
        ScreenProcess();
        FlashScreen();
    }
    private void FlashScreen()
    {
        if (!flashScreen) return;
        if (!flashStartShot.enabled) flashStartShot.enabled = true;
        flashTimer += time * (!switchFlash ? 10 : -1f); 
        flashTimer = Mathf.Clamp01(flashTimer);
        flashStartShot.color = new Color(flashStartShot.color.r, flashStartShot.color.g, flashStartShot.color.b, flashTimer);
        if(flashTimer == 0 || flashTimer == 1)
        {
            if (flashTimer == 1) { switchFlash = true; flashTimer = 0; }
            else if(flashTimer == 0) { flashScreen = false; flashStartShot.enabled = false; }
        }

    }
    private void PulseAlpha()
    {
        float pulse = Mathf.Sin(Time.unscaledTime * 7f) * 0.5f + 0.5f;
        pressStartText.color = new Color(1, 1, 1, pulse);
    }
    private void ScreenProcess()
    {
        if (isStartPressed) return;
        if (screenActive[clipIndex]) return;
        if (clipIndex < 2)
        {
            if (bootImage.sprite != bootSprites[clipIndex])
                bootImage.sprite = bootSprites[clipIndex];
        }
        else
        {
            if (bootImage.enabled) bootImage.enabled = false;
            if (!bootVideo.enabled) { bootVideo.enabled = true; videoAudioSrc.volume = 1; }

        }
        if (!isWaiting) 
        {
            blackScreen.color = new Color(blackScreen.color.r, blackScreen.color.g, blackScreen.color.b, Mathf.Lerp(fadeOut ? 1 : 0, fadeOut? 0 : 1, fadeInterval));
            fadeInterval += (clipIndex > 4) ? time * 0.5f : time * 2;
            fadeInterval = Mathf.Clamp01(fadeInterval);
      
            if (fadeInterval == 1)
            {
               
                fadeInterval = 0;

                if (fadeOut == true) isWaiting = true;
                else
                {
                   
                    fadeOut = !fadeOut; 
                    screenActive[clipIndex] = true; 
                    clipIndex++;
                    if (clipIndex > 4)
                    {
                        StartGameMenu();
                    }
                    else
                    {
                        if (clipIndex > 1)
                        {
                            ClearOutRenderTexture(renderTexture);
                            if (videoPlayer.clip != videoClips[clipIndex - 2])
                                videoPlayer.clip = videoClips[clipIndex - 2];
                            if (!videoPlayer.isPlaying) videoPlayer.Play();
                        }
                        screenActive[clipIndex] = false;
                    }
                }
            }
        }
        else
        {
            if (clipIndex >= 4)
            {
                if (!pressStartText.enabled) {  pressStartText.enabled = true; audioSystem.PlayGameMusic(startMenuMusic, 1, 1, true); }
                PulseAlpha();
                if (!videoPlayer.isLooping) videoPlayer.isLooping = true;
                if (inputPlayer.GetButtonDown("Start") || inputPlayer.GetButtonDown("Select") || Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
                {
                    audioSystem.PlayAudioSource(bootSfx, 1, 1, 128);
                    fadeOut = !fadeOut;
                    isWaiting = false;
                    flashScreen = true;
                    pressStartText.color = Color.white;
                    pressStartText.enabled = false;
                }
            }
  
            else
            {
                if (inputPlayer.GetAnyButtonDown())
                {
                    if (waitTimer != waitTime) waitTimer = waitTime;
                    isWaiting = false;
                    fadeOut = !fadeOut;
                }
                if (clipIndex > 1)
                {
                    if (!videoPlayer.isPlaying)
                    {
                        fadeOut = !fadeOut;
                        isWaiting = false;
                    }
                }
                else
                {
                    waitTimer -= time;
                    waitTimer = Mathf.Clamp(waitTimer, 0, waitTime);
                    if (waitTimer == 0)
                    {
                        if (waitTimer != waitTime) waitTimer = waitTime;
                        isWaiting = false;
                        fadeOut = !fadeOut;
                    }
                }
            }
        }
    }
    private void StartGameMenu()
    {
        isStartPressed = true;
        if (videoPlayer.isPlaying) videoPlayer.Stop();
        waitTimer = waitTime;
        clipIndex = 0;
        isWaiting = false;
        fadeOut = true;
        bootVideo.enabled = false;
      
        menu.SetActive(true);
        load.SetActive(true);
        intro.SetActive(false);
     
        audioSystem.VideoPlayStop(true);
    }
  
    public static void ClearOutRenderTexture(RenderTexture renderTexture)
    {
        RenderTexture rt = RenderTexture.active;
        RenderTexture.active = renderTexture;
        GL.Clear(true, true, Color.clear);
        RenderTexture.active = rt;
    }
}
