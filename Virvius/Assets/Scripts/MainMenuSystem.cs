using UnityEngine;
using UnityEngine.UI;
using Rewired;
using UnityEngine.Video;

public class MainMenuSystem : MonoBehaviour
{
    // Start is called before the first frame update
    private GameSystem gameSystem;
    private OptionsSystem optionsSystem;
    private IntroSystem introSystem;
    private Player inputPlayer;
    [SerializeField]
    private Image pressStartBG;
    [SerializeField]
    private Text pressStartText;
    private bool startGame = false;
    [SerializeField]
    private Image[] controllerUIImage;
    [SerializeField]
    private Image[] KeyboardUIImage;    
    [SerializeField]
    private Text[] controllerUIText;
    [SerializeField]
    private Text[] KeyboardUIText;
    [SerializeField]
    private Image blackScreen;
    private bool blackScreenActive = false;
    private float fadeInterval = 0;
    [SerializeField]
    private VideoPlayer videoPlayer;
    private AudioSource videoAudioSrc;

    [SerializeField]
    private VideoClip previewClip;
    [SerializeField]
    private RenderTexture renderTexture;
    void Start()
    {
        introSystem = IntroSystem.introSystem;
        inputPlayer = ReInput.players.GetPlayer(0);
        gameSystem = GameSystem.gameSystem;
        optionsSystem = OptionsSystem.optionsSystem;
        videoAudioSrc = videoPlayer.GetComponent<AudioSource>();
        videoPlayer.audioOutputMode = VideoAudioOutputMode.Direct;
        pressStartBG.enabled = false;
        optionsSystem.CheckMouseActive();

    }

    // Update is called once per frame
    void Update()
    {
        if (gameSystem.isLoading) return;
        if (gameSystem.isGameStarted) return;
        if (optionsSystem.optionsOpen) return;
        if (optionsSystem.quitOpen) return;
        FadeBlackScreen();
        PulseAlpha();
        if (!startGame)
        {
            if (introSystem.gameObject.activeInHierarchy)
            {
                ResetVideo();
                blackScreenActive = true;
                introSystem.gameObject.SetActive(false);
                OpenMainMenu(true);
            }
            else
            {
                if (inputPlayer.GetButtonDown("Start") || inputPlayer.GetButtonDown("Select"))
                {
                    OpenMainMenu(true);
                }
            }
        }
        else
        {
            if (!gameSystem.mainmenuOpen) return;
            if (Input.GetKeyDown(KeyCode.Escape) || inputPlayer.GetButtonDown("B"))
            {
                OpenMainMenu(false);
            }
        }
    }
    public void ResetVideo()
    {
        videoPlayer.clip = null;
        ClearOutRenderTexture(renderTexture);
     
        videoPlayer.clip = previewClip;
        if (videoPlayer.isActiveAndEnabled)
        {
            if (!videoPlayer.isPlaying)
            {
                
                videoPlayer.playOnAwake = true;
                videoPlayer.enabled = false;
                videoPlayer.enabled = true;
                //videoPlayer.Play();
            }
        }
    }
    private void FadeBlackScreen()
    {
        if (!blackScreenActive) return;
        blackScreen.color = new Color(blackScreen.color.r, blackScreen.color.g, blackScreen.color.b, Mathf.Lerp(1, 0, fadeInterval));
        fadeInterval += Time.unscaledDeltaTime * 2;
        fadeInterval = Mathf.Clamp01(fadeInterval);
        if (fadeInterval == 1)
            blackScreenActive = false;
    }
    public void OpenMainMenu(bool active)
    {
        pressStartBG.enabled = !active;
        pressStartText.color = !active ? Color.white : Color.clear;
        pressStartText.enabled = !active;
        gameSystem.GameMouseActive(active, active ? CursorLockMode.Confined : CursorLockMode.Locked);
        gameSystem.ManualPause();
        startGame = active;
    }
    private void PulseAlpha()
    {
        if (startGame) return;
        float pulse = Mathf.Sin(Time.unscaledTime * 7f) * 0.5f + 0.5f;
        pressStartText.color = new Color(1, 1, 1, pulse);
        //pressStartBG.color = new Color(pressStartBG.color.r, pressStartBG.color.g, pressStartBG.color.b, pulse);
    }
    public enum Navigation { Keyboard, Controller };
    public void SetNavigationUI(Navigation navigation)
    {
        switch (navigation)
        {
            case Navigation.Keyboard:
                {
                    for (int k = 0; k < KeyboardUIImage.Length; k++) KeyboardUIImage[k].enabled = true;
                    for (int c = 0; c < controllerUIImage.Length; c++) controllerUIImage[c].enabled = false;
                    for (int k = 0; k < KeyboardUIText.Length; k++) KeyboardUIText[k].enabled = true;
                    for (int c = 0; c < controllerUIText.Length; c++) controllerUIText[c].enabled = false;
                    break;
                }
            case Navigation.Controller:
                {
                    for (int k = 0; k < KeyboardUIImage.Length; k++) KeyboardUIImage[k].enabled = false;
                    for (int c = 0; c < controllerUIImage.Length; c++) controllerUIImage[c].enabled = true;
                    for (int k = 0; k < KeyboardUIText.Length; k++) KeyboardUIText[k].enabled = false;
                    for (int c = 0; c < controllerUIText.Length; c++) controllerUIText[c].enabled = true;
                    break;
                }
        }
        
       
    }
    public static void ClearOutRenderTexture(RenderTexture renderTexture)
    {
        RenderTexture rt = RenderTexture.active;
        RenderTexture.active = renderTexture;
        GL.Clear(true, true, Color.clear);
        RenderTexture.active = rt;
    }
}
