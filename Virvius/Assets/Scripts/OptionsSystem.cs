using UnityEngine;
using System.Text;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine.Rendering.PostProcessing;
using Rewired;

public class OptionsSystem : MonoBehaviour
{
    //========================================================================================//
    //===================================[STATIC FIELDS]======================================//
    //========================================================================================//
    public static OptionsSystem optionsSystem;
    //========================================================================================//
    //===================================[PRIVATE FIELDS]======================================//
    //========================================================================================//
    //[Class Access]++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++//
    [SerializeField]
    private GameSystem gameSystem;
    [SerializeField]
    private IntroSystem introSystem;
    private WeaponSystem weaponSystem;
    private PlayerSystem playerSystem;
    private Player inputPlayer;
    private AudioSystem audioSystem;
    [SerializeField]
    private MainMenuSystem mainMenuSystem;
    [HideInInspector]
    public bool optionsLoaded = false;
    private ButtonHighlight buttonHighlight;
    private bool testVibrate = false;
    private bool startFade = false;
    private float fadePercentage = 0;
    private Image[] imageElements;
    private Text[] textElements;
    private float fadeTimeInSeconds = 1;
    private bool fadeColorIn = true;
    //[Strings & Buttons]++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++//
    private StringBuilder stringBuilderValue = new StringBuilder();
    private StringBuilder stringBuilderMessage = new StringBuilder();
    private StringBuilder stringBuilderResolution = new StringBuilder();
    private StringBuilder stringBuilderIntToText = new StringBuilder();
    private Selectable b_Select;
    private Selectable p_Select;
    [HideInInspector]
    public AudioConfiguration audioConfig;
    private AudioConfiguration defaultConfig;
    private Vector3 currentMousePosition;

    //===================================================
    // INSPECTOR ASSIGNMENT FIELDS
    //===================================================

    [Header("Options Menu")]
    [SerializeField]
    private Scrollbar[] panelScrollBars = new Scrollbar[5];
    [SerializeField]
    private GameObject player;
    [SerializeField]
    private GameObject gameUI;
    [SerializeField]
    private AudioClip selectFx;
    [SerializeField]
    private GameObject[] optionPanels = new GameObject[5];
    [SerializeField]
    private GameObject optionSelection;
    [SerializeField]
    private PostProcessVolume processVolume;
    [SerializeField]
    private Text highlightedMessage = null;
    [SerializeField]
    private Button[] selectionFirstSelected;
    private Image[] imageFirstSelected = new Image[5];
    private Button[] panelFirstSelected;
    [HideInInspector]
    public bool optionsOpen = true;
    private bool selectingOption = false;
    private bool[] currentContent;
    private bool[] panelOpen = new bool[5] { true, false, false, false, false };
    private string[] onOffText = new string[2] { "OFF", "ON" };
    private string[] panelHighlightedMessages = new string[5]
    {
        "Adjust/view [game] settings.",
        "Change/view [control] settings.",
        "Adjust/view [UI] settings.",
        "Adjust/view [video] settings.",
        "Adjust/view [audio] settings."

    };
    [HideInInspector]
    public  bool canQuit = false;
    private int panelIndex = 0;
    private int selectionIndex = 0;
    private int contentIndex = 0;
    private int charIndex = 0;

    [SerializeField]
    private Image[] OptionsWindowPriColorElements;
    [SerializeField]
    private Image[] OptionsWindowSubColorElements;
    [SerializeField]
    private Image[] mainWindowIElements;
    [SerializeField]
    private Text[] mainWindowTElements;
   
    [Space]
    [Header("Quit Menu")]
    [HideInInspector]
    public bool quitOpen = false;
    [SerializeField]
    private GameObject quitWindow;
    [SerializeField]
    private Image[] quitWindowIElements;
    [SerializeField]
    private Text[] quitWindowTElements;
    [SerializeField]
    private Button declineButton;
    [SerializeField]
    private GameObject fileSelection;
    [Space]
    [Header("Game Settings")]
    [SerializeField]
    private Image[] gameWindowIElements;
    [SerializeField]
    private Text[] gameWindowTElements;
    [SerializeField]
    private Button[] gameWindowContent = new Button[8];
    [SerializeField]
    private Text[] gameContentSubText;
    private bool[] gameHighlightedContent = new bool[8];
    private string[] gameHighlightedMessages = new string[8]
    {
        "Change game difficulty.",
        "Enable/disable weapon auto aim.",
        "Enable/disable auto saving.",
        "Enable/disable camera bobbing.",
        "Enable/disable weapon auto switching on pickup.",
        "Enable/disable weapon auto switching when out of ammo.",
        "Enable/disable weapon auto centering.",
        "Change players dominate weapon hand.",
    };
    [Space]

    [Header("Control Settings")]
    [SerializeField]
    private Image[] controlWindowIElements;
    [SerializeField]
    private Text[] controlWindowTElements;
    [SerializeField]
    private Button[] controlWindowContent = new Button[9];
    [SerializeField]
    private Text[] controlContentSubText;
    [SerializeField]
    private Image[] controlContentSlider = new Image[2];
    private bool[] controlHighlightedContent = new bool[9];
    private string[] controlHighlightedMessages = new string[9]
    {
        "Enable/disable controller/gamepad.",
        "Invert the 'Y' vertical look axis.",
        "Change the vertical look sensitivity.",
        "Change the horizontal look sensitivity.",
        "Smooth the look rotation.",
        "Enable/disable controller vibration.",
        "player will always move at run speed.",
        "Enable/disable air movement.",
        "Lock/Unlock Y Axis [Auto Aim required]."
    };
    [Space]

    [Header("UI Settings")]
    [SerializeField]
    private Image[] uiWindowIElements;
    [SerializeField]
    private Text[] uiWindowTElements;
    [SerializeField]
    private Button[] uiWindowContent = new Button[8];
    [SerializeField]
    private Text[] uiContentSubText;
    private bool[] uiHighlightedContent = new bool[8];
    private string[] uiHighlightedMessages = new string[8]
    {
        "Display messages for interactive/pickup objects.",
        "Display date and time on screen.",
        "Display game total frames per second.",
        "Enable/disable flash effects for damage/pickups.",
        "Change overall visual HUD type.",
        "Change the default color of Visor HUD. (Visor Only)",
        "Change the crosshair style.",
        "Enable/disable environmental UI effects."
    };
    [Space]

    [Header("Video Settings")]
    [SerializeField]
    private Image[] videoWindowIElements;
    [SerializeField]
    private Text[] videoWindowTElements;
    [SerializeField]
    private Button[] videoWindowContent = new Button[24];
    [SerializeField]
    private Text[] videoContentSubText;
    private bool[] videoHighlightedContent = new bool[24];
    private string[] videoHighlightedMessages = new string[24]
    {
        "Toggle between fullscreen/windowed.",
        "Set the overall visual quality.",
        "Change the game resolution.",
        "Enable/disable visual sync.",
        "Enable/disable high definition range.",
        "Change field of view aspect value.",
        "Enable/disable bloom.",
        "Enable/disable chromatic abberation.",
        "Enable/disable lens distortion.",
        "Enable/disable ambient occlusion.",
        "Enable/disable color grading.",
        "Set the Color Grading Mode.",
        "set the Color grading Tone Mapper.",
        "Adjust the game brightness.",
        "Enable/disable grain.",
        "Set grain intensity.",
        "Enable/disable vignette.",
        "Set vignette Intensity.",
        "Set Bloom Intensity.",
        "Set the Color Grading Saturation.",
        "Set the Color Grading Contrast.",
        "Set the Color Grading Lift.",
        "Set the Color Grading Gamma.",
        "Set the Color Grading Gain."
    };
    [SerializeField]
    private Image[] videoContentSlider;
    [Space]

    [Header("Audio Settings")]
    [SerializeField]
    private Text applyText;
    [SerializeField]
    private Image[] audioWindowIElements;
    [SerializeField]
    private Text[] audioWindowTElements;
    [SerializeField]
    private Button[] audioWindowContent = new Button[12];
    [SerializeField]
    private Text[] audioContentSubText;
    [SerializeField]
    private Image[] audioContentSlider = new Image[5];
    private bool[] audioHighlightedContent = new bool[12];
    private string[] audioHighlightedMessages = new string[12]
    {
        "Change the master volume setting.",
        "Change the music volume setting.",
        "Change the sound volume setting.",
        "Change the environment volume setting.",
        "Change the damage volume setting.",
        "Change the speaker mode setting.",
        "Change the number of virtual voices.",
        "Change the change the number of real voices.",
        "Change the audio sample rate.",
        "Change the dps buffer length.",
        "Change games current Soundtrack.",
        "Apply new audio configuration settings.",

    };
    //===============================================
    //OPTION VALUES
    //===============================================

    //----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    // Game Fields----------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    //----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    //DIFFICULTY~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    [HideInInspector]
    public int difficultyIndex = 0;
    private string[] difficultySubText = new string[4]
    {
        "EASY",
        "NORMAL",
        "HARD",
        "VERYHARD"
    };
    [HideInInspector]
    public bool[] difficultyActive = new bool[4] { false, false, false, false };
    //AUTOAIM~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    [HideInInspector]
    public bool autoAim = false;
    [HideInInspector]
    public int autoAIndex = 1;
    //AUTOSAVE~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    [HideInInspector]
    public bool autoSave = false;
    [HideInInspector]
    public int autoSIndex = 1;
    //CAMERABOBBING~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    [HideInInspector]
    public bool cameraBobbing = true;
    [HideInInspector]
    public int cameraBIndex = 1;
    //WEAPONHAND~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    [HideInInspector]
    public int handIndex = 0;
    private string[] handSubText = new string[3]
    {
        "CENTER",
        "RIGHT",
        "LEFT"
    };
    [HideInInspector]
    public bool[] handActive = new bool[3] { false, false, false };
    //SWITCHWEAPONNEW~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    [HideInInspector]
    public bool autoSwitchNew = false;
    [HideInInspector]
    public int switchNewIndex = 1;
    //SWITCHWEAPONEMPTY~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    [HideInInspector]
    public bool autoSwitchEmpty = false;
    [HideInInspector]
    public int switchEmptyIndex = 1;
    //WEAPONBOBBING~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    [HideInInspector]
    public bool weaponMovement = false;
    [HideInInspector]
    public int weaponMoveIndex = 1;
    //----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    // Control Fields-------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    //----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    //CONTROLLERENABLED~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    [HideInInspector]
    public bool controllerEnabled = true;
    [HideInInspector]
    public int controllerIndex = 0;
    //INVERTY~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    [HideInInspector]
    public bool invertY = false;
    [HideInInspector]
    public int invertIndex = 0;
    //SENSITIVITY~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    [HideInInspector]
    public float[] sensitivity = new float[2] { 2.5f, 2.5f };
    //SMOOTHROTATION~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    [HideInInspector]
    public bool smoothRotation = true;
    [HideInInspector]
    public int smoothIndex = 0;
    //VIBRATION~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    [HideInInspector]
    public bool vibration = false;
    [HideInInspector]
    public int vibrationIndex = 0;
    //ALWAYSRUN~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    [HideInInspector]
    public bool alwaysRun = true;
    [HideInInspector]
    public int runIndex = 0;
    //AIRCONTROL~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    [HideInInspector]
    public bool airControl = true;
    [HideInInspector]
    public int airIndex = 0;
    //LOXKYAXIS~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    [HideInInspector]
    public bool lockYAxis = true;
    [HideInInspector]
    public int lockYIndex = 0;
    //----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    // UI Fields------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    //----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    //UIMESSAGES~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    [HideInInspector]
    public int messagesActiveIndex = 1;
    [HideInInspector]
    public bool showMessages = true;
    //SHOWTIME~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    private StringBuilder stringBuilderTime = new StringBuilder();
    [SerializeField]
    private Text dateTimeText;
    private string timeFormat = "HH:mm dd MMMM, yyyy";
    private string date = "Date: ";
    [HideInInspector]
    public int timeActiveIndex = 0;
    private bool showTime = false;
    //FRAMERATE~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    private StringBuilder stringBuilderFps = new StringBuilder();
    [SerializeField]
    private Text fpsText;
    private float fpsCount = 0;
    private float fpsTimer = 0;
    private float fpsTime = 0.25f;
    private string fpsLabel = " FPS: ";
    private string screenSize = "Resolution: ";
    [HideInInspector]
    public int fpsActiveIndex = 0;
    private bool showFrameRate = false;
    //FLASHEFFECTS~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    [HideInInspector]
    public bool showFlashEffects = true;
    [HideInInspector]
    public int flashIndex = 1;
    //HUDTYPE~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    [HideInInspector]
    public string[] hudSubText = new string[4] { "CLASSIC FULL", "CLASSIC", "STYLISH", "VISOR" };
    [HideInInspector]
    public int hudIndex = 3;
    //HUDVISORCOLOR~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    [HideInInspector]
    public string[] VisorColorSubText = new string[11] {"DEFAULT", "MARINE", "DEMONIC", "ANGELIC", "DEVILISH", "CRYPT", "DESTINY", "IMPERIAL", "PATRIOT", "GRAVESTONE", "DEATHWISH" };
    [HideInInspector]
    public int visorColorIndex = 0;
    [HideInInspector]
    public Color currentVisorColor = new Color(1, 0.98f, 0.7f, 0.1f);
    private Color[] visorColor = new Color[11]
    {
        //DEFAULT - fire red
        new Color(1f, 0.15f, 0f, 0.6f),

        //MARINE - navy green
        new Color(0.6f, 0.66f, 0.37f, 0.1f),

        //DEMONIC - dark blue
        new Color(0.23f, 0.33f, 0.45f, 0.8f),

        //ANGELIC - light blue theme
        new Color(0f, 1, 1, 0.1f),

        //DEVILISH - fire red theme
        new Color(1, 0.3f, 0, 0.5f),

        //CRYPT - brown theme
        new Color(0.25f, 0.16f, 0.2f, 0.5f),

        //DESTINY - pink theme
        new Color(1, 0.5f, 1, 0.5f),

        //IMPERIAL - imperial pink theme
        new Color(1f, 0f, 0.5f, 0.5f),

        //PATRIOT - blue theme
        new Color(0, 0.7f, 1, 0.5f),

        //GRAVESTONE - slate theme
        new Color(0.5f, 0.5f, 0.6f, 0.5f),

        //DEATHWISH - ghost purple theme
        new Color(0.5f, 0.5f, 1, 0.5f)
    };
    private Color[] visorColor2 = new Color[11]
{
        //DEFAULT - navy green
        new Color(1f, 1f, 0.7f, 0.1f),

        //MARINE - dark Green theme
        new Color(0.26f, 0.41f, 0.25f, 0.3f),

        //DEMONIC - gore red theme
        new Color(0.9f, 0.2f, 0.27f, 0.3f),

        //ANGELIC - gold theme
        new Color(1, 0.9f, 0.37f, 0.2f),

        //DEVILISH - flame yellow theme
        new Color(1, 0.5f, 0, 0.3f),

        //CRYPT - tan theme
        new Color(0.8f, 0.7f, 0.45f, 0.2f),

        //DESTINY - candy green theme
        new Color(0, 1f, 0.58f, 0.3f),

        //IMPERIAL - lime yellow theme
        new Color(0.8f, 1f, 0.4f, 0.2f),

        //PATRIOT - light red theme
        new Color(1, 0.3f, 0.3f, 0.2f),

        //GRAVESTONE - slime green theme
        new Color(0.3f, 1f, 0.5f, 0.2f),

        //DEATHWISH - yellow theme
        new Color(1, 1f, 0f, 0.2f)
};
    //CROSSHAIR~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    [HideInInspector]
    public string[] crosshairSubText = new string[7] { "CRISSCROSS", "CROSS", "DOT", "ROUND", "CIRCLE", "SQUARE", "NONE" };
    [HideInInspector]
    public int crosshairIndex = 0;
    [SerializeField]
    private Sprite[] crosshairSprites = new Sprite[6];

    //----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    // Video Fields---------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    //----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    //WINDOWED~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    [HideInInspector]
    public string[] fullScreenSubText = new string[4] { "EXCLUSIVE", "FULLSCREEN", "MAXIMIZED", "WINDOWED" };
    private FullScreenMode screenMode = FullScreenMode.FullScreenWindow;
    [HideInInspector]
    public int fullScreenIndex = 1;
    //QUALITY~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    [HideInInspector]
    public string[] qualitySubText = new string[6] { "GOOD", "GREAT", "AWESOME", "CRAZY", "INSANE", "INCREDIBLE" };
    [HideInInspector]
    public int qualityIndex = 0;
    //RESOLUTION~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    private Resolution[] resolutions;
    [HideInInspector]
    public int resolutionIndex = 0;
    [HideInInspector]
    public int width = 0;
    [HideInInspector]
    public int height = 0;
    [HideInInspector]
    public int refreshRate = 0;
    //VSYNC~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    private bool enableVSync = true;
    [HideInInspector]
    public int vSyncIndex = 1;
    //HDRMODE~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    private bool enableHDR = false;
    [HideInInspector]
    public int hdrIndex = 0;
    //FOV~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    [SerializeField]
    private Camera gameCamera;
    [SerializeField]
    private Camera clipCamera;
    [HideInInspector]
    public float fieldOfView = 60;
    [HideInInspector]
    public float bloomIntensity = 1f;
    //POSTPROCESSING~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    private Bloom bloomLayer = null;
    private ChromaticAberration chromaticAberrationLayer = null;
    private LensDistortion lensLayer = null;
    private AmbientOcclusion ambientLayer = null;
    private ColorGrading colorGradingLayer = null;
    private AutoExposure autoExposureLayer = null;
    private Grain grainLayer = null;
    [HideInInspector]
    public Vignette vignetteLayer = null;
    [HideInInspector]
    public int[] postEffectIndex = new int[8];
    [HideInInspector]
    public bool[] enablePostEffect = new bool[8];
    //COLORGRADINGMODE~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    private GradingMode gradingMode = GradingMode.LowDefinitionRange;
    [HideInInspector]
    public int gradingIndex = 0;
    [HideInInspector]
    public string[] colorGradingModeName = new string[3]
    {
        "LDR",
        "HDR",
        "EXTERNAL"
    };
    //COLORGRADINGTONEMAPPER~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    private Tonemapper tonemapper = Tonemapper.None;
    [HideInInspector]
    public int toneMapIndex = 0;
    [HideInInspector]
    public string[] colorGradingToneName = new string[4]
    {
        "NONE",
        "NEUTRAL",
        "ACES",
        "CUSTOM"
    };
    //COLORGRADING-SATURATION~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    [HideInInspector]
    public float saturation = 1;
    //COLORGRADING-CONTRAST~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    [HideInInspector]
    public float contrast = 3;
    //COLORGRADING-LIFT~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    [HideInInspector]
    public float lift = 0;
    //COLORGRADING-GAMMA~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    [HideInInspector]
    public float gamma = 0;
    //COLORGRADING-GAIN~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    [HideInInspector]
    public float gain = 0;
    //BRIGHTNESS/AUTOEXPOSURE-EXPOSURECOMPENSATION~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    [HideInInspector]
    public float brightness = 1;

    //GRAIN~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    [HideInInspector]
    public float grainIntensity = 0.2f;
    //VIGNETTE~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    [HideInInspector]
    public float vignetteIntensity = 0.2f;
    //ENVIRONMENTEFFECTS~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    [HideInInspector]
    public bool environmentEffects = true;
    [HideInInspector]
    public int environmentIndex = 0;
    //----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    // Audio Fields---------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    //----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    //VOLUME~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    [SerializeField]
    private AudioMixer audioMixer;
    private string[] volumeName = new string[5]
    {
        "MASTER",
        "MUSIC",
        "SOUND",
        "ENVIRONMENT",
        "DAMAGE"
    };
    [HideInInspector]
    public float[] volumeInterval = new float[5] { 0, 0, 0, 0, 0 };
    [HideInInspector]
    public int[] voiceInterval = new int[12] { 1, 2, 4, 8, 16, 32, 50, 64, 100, 128, 256, 512 };
    [HideInInspector]
    public int[] sampleInterval = new int[6] { 11025, 22050, 44100, 48000, 88200, 96000 };
    [HideInInspector]
    public int[] bufferInterval = new int[11] { 32, 64, 128, 256, 340, 480, 512, 1024, 2048, 4096, 8192 };
    //SPEAKERMODE~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    private AudioSpeakerMode speakerMode;
    private int virtualVoice = 0;
    private int realVoice = 0;
    private int sampleRate = 0;
    private int dpsBuffer = 0;
    [HideInInspector]
    public int virtualVoiceIndex = 0;
    [HideInInspector]
    public int realVoiceIndex = 0;
    [HideInInspector]
    public int sampleRateIndex = 0;
    [HideInInspector]
    public int dpsBufferIndex = 0;
    [HideInInspector]
    public int speakerIndex = 0;
    [HideInInspector]
    public string[] speakerName = new string[6]
    {
        "MONO",
        "STEREO",
        "SURROUND",
        "SURROUND 5.1",
        "SURROUND 7.1",
        "PROLOGIC DTS"
    }; 
    [HideInInspector]
    public int soundtrackIndex = 0;
    [HideInInspector]
    public string[] songName = new string[7]
    {
        "The Grind",
        "Gates of Dispair",
        "Beast Within",
        "Damp Halls",
        "The Crypt",
        "Technical Chaos",
        "Dark Omen",
    };
    private string[] resolutionIDs = new string[3]
    {
        "SD ",
        "HD ",
        "UHD "
    };
    [SerializeField]
    private AudioClip[] musicTracks = new AudioClip[7];
    [HideInInspector]
    public AudioClip musicClip;
    private float unScaledDTime = 0;
    //====================================================================
    //=======================UnityMethods=================================
    //====================================================================
    private void Awake()
    {
        // Make Options Accessible
        optionsSystem = this;
        defaultConfig = AudioSettings.GetConfiguration();
      
    }
    void OnEnable()
    {
        ReInput.ControllerConnectedEvent += OnControllerConnected;
        ReInput.ControllerDisconnectedEvent += OnControllerDisconnected;
        //AudioSettings.OnAudioConfigurationChanged += OnAudioConfigurationChanged;
    }
    void Start()
    {
        gameSystem = GameSystem.gameSystem;
       
        // Grab player input from Rewired
        inputPlayer = ReInput.players.GetPlayer(0);
      
        // Set the char build timer
        fpsTimer = fpsTime;
        // first buttons to auto select when selecting menus
        panelFirstSelected = new Button[5] { gameWindowContent[0], controlWindowContent[0], uiWindowContent[0], videoWindowContent[0], audioWindowContent[10] };
        p_Select = selectionFirstSelected[0].GetComponent<Selectable>();

        // temporary-REMOVE LATER
        // Assume game settings as first window open...
        // Panel Index will be [Default] 0 for Game Panel.
        panelIndex = 0;
        
        // Set the current content categories to [Default] for Game Panel - CANNOT BE NULL
        currentContent = gameHighlightedContent;
        // Set default values for all content in each panel
        resolutions = Screen.resolutions;
        for (int i = 0; i < resolutions.Length; i++)
        {
            if (resolutions[i].width == Screen.currentResolution.width && resolutions[i].height == Screen.currentResolution.height)
                resolutionIndex = i;
        }
        width = Screen.currentResolution.width;
        height = Screen.currentResolution.height;
        refreshRate = Screen.currentResolution.refreshRate;
        processVolume.profile.TryGetSettings(out bloomLayer);
        processVolume.profile.TryGetSettings(out chromaticAberrationLayer);
        processVolume.profile.TryGetSettings(out lensLayer);
        processVolume.profile.TryGetSettings(out ambientLayer);
        processVolume.profile.TryGetSettings(out colorGradingLayer);
        processVolume.profile.TryGetSettings(out autoExposureLayer);
        processVolume.profile.TryGetSettings(out grainLayer);
        processVolume.profile.TryGetSettings(out vignetteLayer);
        for (int i = 0; i < 5; i++) imageFirstSelected[i] = selectionFirstSelected[i].GetComponent<Image>();
    }
    void Update()
    {
        unScaledDTime = Time.unscaledDeltaTime;
        KeyControl();
        CheckMouseActive();
        ButtonManagement();
        ShowDateTime();
        ShowFPS();
        AudioApplyButtonActive();
        FadeSingle(textElements, imageElements, fadeColorIn, fadeTimeInSeconds);
       
    }
    //====================================================================
    //=======================OptionMethods================================
    //====================================================================
    public void CheckMouseActive()
    {
        if (!gameSystem.isPaused) return;
        if (Cursor.visible && inputPlayer.controllers.Keyboard.GetAnyButton())
        {
            mainMenuSystem.SetNavigationUI(MainMenuSystem.Navigation.Keyboard);
            gameSystem.GameMouseActive(false, CursorLockMode.Locked);
            currentMousePosition = Input.mousePosition;
        }
        if (inputPlayer.controllers.Joysticks.Count > 0)
        {
            if (Cursor.visible && inputPlayer.controllers.Joysticks[0].GetAnyButton())
            {
                mainMenuSystem.SetNavigationUI(MainMenuSystem.Navigation.Controller);
                gameSystem.GameMouseActive(false, CursorLockMode.Locked);
                currentMousePosition = Input.mousePosition;
            }
        }
        if (Input.mousePosition != currentMousePosition && !Cursor.visible)
        {
            mainMenuSystem.SetNavigationUI(MainMenuSystem.Navigation.Keyboard);
            gameSystem.GameMouseActive(true, CursorLockMode.Confined);
            currentMousePosition = Input.mousePosition;
        }
    }
    public void ApplyActiveInputUI()
    {
        if (inputPlayer.controllers.Joysticks.Count > 0)
        {
            mainMenuSystem.SetNavigationUI(MainMenuSystem.Navigation.Controller);
        }
        else
        {
            mainMenuSystem.SetNavigationUI(MainMenuSystem.Navigation.Keyboard);
        }
    }
    private void KeyControl()
    {
        if (gameSystem.isLoading) return;
        if (CommandSystem.commandOpen) return;
        if (!optionsOpen && !quitOpen) return;
        if (optionsOpen)
        {
            canQuit = false;
            if (inputPlayer.GetButtonDown("RSB") && !selectingOption) { DefaultOptions(); selectingOption = true; }
            if (Input.GetKeyDown(KeyCode.Escape) && !selectingOption || inputPlayer.GetButtonDown("B") && !selectingOption)
            {
                BackButton();
                audioSystem.PlayAudioSource(selectFx, 1, 1, 128);
                selectingOption = true;
            }
            if (inputPlayer.GetButtonDown("RB") && !selectingOption || Input.GetKeyDown(KeyCode.RightAlt) && !selectingOption)
            {
                panelIndex++;
                if (panelIndex > 4) panelIndex = 0;
                SelectSettingPanel(panelIndex);
                selectingOption = true;
            }
            if (inputPlayer.GetButtonDown("LB") && !selectingOption || Input.GetKeyDown(KeyCode.LeftAlt) && !selectingOption)
            {
                panelIndex--;
                if (panelIndex < 0) panelIndex = 4;
                SelectSettingPanel(panelIndex);
                selectingOption = true;
            }
            if (inputPlayer.GetButtonUp("RSB") || inputPlayer.GetButtonUp("RB") || Input.GetKeyUp(KeyCode.RightShift) || inputPlayer.GetButtonUp("RB") ||
                     Input.GetKeyUp(KeyCode.RightShift) || Input.GetKeyUp(KeyCode.Escape) || inputPlayer.GetButtonUp("B")) selectingOption = false;
        }
        else if (quitOpen)
        {
            if (Input.GetKeyDown(KeyCode.Escape) && !selectingOption || inputPlayer.GetButtonDown("B") && !selectingOption)
            {
                OpenQuitMenu(false);
                audioSystem.PlayAudioSource(selectFx, 1, 1, 128);
                selectingOption = true;
            }
            if (Input.GetKeyUp(KeyCode.Escape) || inputPlayer.GetButtonUp("B")) selectingOption = false;
        }
    }
    public void BackButton()
    {
        
        OpenOptions(false);
        gameSystem.OpenMainSelection(true);
        audioSystem.PlayAudioSource(selectFx, 1, 1, 128);
    }
    private void ButtonManagement()
    {
        if (!optionsOpen) return;
        if (inputPlayer.GetAxis("LSHUI") == 0 && selectingOption) selectingOption = false;
        else if (inputPlayer.GetButtonUp("LSHUI")) selectingOption = false;
        if (selectingOption) return;
        float axisValue = inputPlayer.GetAxis("LSHUI");
        if (axisValue != 0)
        {
            switch (panelIndex)
            {
                //GAME
                case 0:
                    {
                        if (!gameHighlightedContent[contentIndex]) return;
                        switch (contentIndex)
                        {
                            //DIFFICULTY
                            case 0: difficultyIndex = RunContentAction(difficultyIndex, 3); SetDifficulty(); break;
                            //AUTOAIM
                            case 1: autoAIndex = RunContentAction(autoAIndex, 1); SetAutoAim(); break;
                            //AUTOSAVE
                            case 2: autoSIndex = RunContentAction(autoSIndex, 1); SetAutoSave(); break;
                            //CAMERABOB
                            case 3: cameraBIndex = RunContentAction(cameraBIndex, 1); SetCameraBobbing(); break;
                            //AUTOSWITCHNEW
                            case 4: switchNewIndex = RunContentAction(switchNewIndex, 1); SetAutoSwitchNew(); break;
                            //AUTOSWITCHEMPTY
                            case 5: switchEmptyIndex = RunContentAction(switchEmptyIndex, 1); SetAutoSwitchEmpty(); break;
                            //WEAPONBOB
                            case 6: weaponMoveIndex = RunContentAction(weaponMoveIndex, 1); SetWeaponMovement(); break;
                            //WEAPONHAND
                            case 7: handIndex = RunContentAction(handIndex, 2); SetWeaponHand(); break;
                        }
                        break;
                    }
                //CONTROL
                case 1:
                    {
                        if (!controlHighlightedContent[contentIndex]) return;
                        switch (contentIndex)
                        {
                            //ENABLECONTROLLER
                            case 0: controllerIndex = RunContentAction(controllerIndex, 1); SetControllerActive(); break;
                            //INVERT
                            case 1: invertIndex = RunContentAction(invertIndex, 1); SetInvertActive(); break;
                            //YSENSITIVITY
                            case 2:
                                {
                                    sensitivity[1] += (axisValue / 10);
                                    sensitivity[1] = Mathf.Clamp(sensitivity[1], 0.1f, 10);
                                    controlContentSubText[contentIndex].text = FormatValuesOne(sensitivity[1]);
                                    controlContentSlider[1].fillAmount = sensitivity[1] / 10;
                                    break;
                                }
                            //XSENSITIVITY
                            case 3:
                                {
                                    sensitivity[0] += (axisValue / 10);
                                    sensitivity[0] = Mathf.Clamp(sensitivity[0], 0.1f, 10);
                                    controlContentSubText[contentIndex].text = FormatValuesOne(sensitivity[0]);
                                    controlContentSlider[0].fillAmount = sensitivity[0] / 10;
                                    break;
                                }
                            //SMOOTHROTATION
                            case 4: smoothIndex = RunContentAction(smoothIndex, 1); SetSmoothActive(); break;
                            //VIBRATION
                            case 5: vibrationIndex = RunContentAction(vibrationIndex, 1); SetVibrationActive(); break;
                            //ALWAYSRUN
                            case 6: runIndex = RunContentAction(runIndex, 1); SetRunActive(); break;
                            //AIRCONTROL
                            case 7: airIndex = RunContentAction(airIndex, 1); SetAirControlActive(); break;     
                            //LOCKYAXIS
                            case 8: lockYIndex = RunContentAction(lockYIndex, 1); SetLockYActive(); break;
                        }
                        break;
                    }
                //UI
                case 2:
                    {
                        if (!uiHighlightedContent[contentIndex]) return;
                        switch (contentIndex)
                        {
                            //UIMESSAGES
                            case 0: messagesActiveIndex = RunContentAction(messagesActiveIndex, 1); SetMessagesActive(); break;
                            //DATE&TIME
                            case 1: timeActiveIndex = RunContentAction(timeActiveIndex, 1); SetTimeActive(); break;
                            //FPS
                            case 2: fpsActiveIndex = RunContentAction(fpsActiveIndex, 1); SetFPSActive(); break;
                            //FLASHEFFECTS
                            case 3: flashIndex = RunContentAction(flashIndex, 1); SetFlashEffectsActive(); break;
                            //HUDTYPE
                            case 4: hudIndex = RunContentAction(hudIndex, 3); SetHUD(); break;
                            //VISORCOLOR
                            case 5: visorColorIndex = RunContentAction(visorColorIndex, 10); SetVisorColor(); break;
                            //CROSSHAIR
                            case 6: crosshairIndex = RunContentAction(crosshairIndex, 6); SetCrosshair(); break;   
                            //CROSSHAIR
                            case 7: environmentIndex = RunContentAction(environmentIndex, 1); SetEnvironmentEffectsActive(); break;

                        }
                        break;
                    }
                //VIDEO
                case 3:
                    {
                        if (!videoHighlightedContent[contentIndex]) return;
                        switch (contentIndex)
                        {
                            //WINDOWEDMODE
                            case 0: fullScreenIndex = RunContentAction(fullScreenIndex, 3); SetWindowedScreen(); break;
                            //GAMEQUALITY
                            case 1: qualityIndex = RunContentAction(qualityIndex, 5); SetQuality(); break;
                            //GAMERESOLUTION
                            case 2: resolutionIndex = RunContentAction(resolutionIndex, resolutions.Length - 1); SetResolution(); break;
                            //VSYNC
                            case 3: vSyncIndex = RunContentAction(vSyncIndex, 1); SetVSync(); break;
                            //HDRMODE
                            case 4: hdrIndex = RunContentAction(hdrIndex, 1); SetHDRMode(); break;
                            //FOV
                            case 5:
                                {
                                    fieldOfView += (axisValue / 10);
                                    fieldOfView = Mathf.Clamp(fieldOfView, 60, 120);
                                    SetFieldOfView(fieldOfView);
                                    break;
                                }
                            //BLOOM
                            case 6: SetPostProcessingEffect(0); break;
                            //CHROMATICABBERATION
                            case 7: SetPostProcessingEffect(1); break;
                            //LENSDISTORTION
                            case 8: SetPostProcessingEffect(2); break;
                            //AMBIENTOCCLUSION
                            case 9: SetPostProcessingEffect(3); break;
                            //COLORGRADE
                            case 10: SetPostProcessingEffect(4); break;
                            //GRADEMODE
                            case 11: 
                                {
                                    if (!videoWindowContent[11].interactable) return;
                                    gradingIndex = RunContentAction(gradingIndex, colorGradingModeName.Length - 1); 
                                    SetColorGradingMode(); break; 
                                }
                            //GRADETONE
                            case 12:
                                {
                                    if (!videoWindowContent[12].interactable) return; 
                                    toneMapIndex = RunContentAction(toneMapIndex, colorGradingToneName.Length - 1); 
                                    SetColorGradingTone(); break;
                                }
                            //SATURATION
                            case 19:
                                {
                                    if (!videoWindowContent[19].interactable) return;
                                    saturation += (axisValue);
                                    saturation = Mathf.Clamp(saturation, -100, 100);
                                    SetSaturation(saturation);
                                    break;
                                }
                            //CONTRAST
                            case 20:
                                {
                                    if (!videoWindowContent[19].interactable) return;
                                    contrast += (axisValue);
                                    contrast = Mathf.Clamp(contrast, -100, 100);
                                    SetContrast(contrast);
                                    break;
                                }
                            //LIFT
                            case 21:
                                {
                                    if (!videoWindowContent[20].interactable) return;
                                    lift += (axisValue / 100);
                                    lift = Mathf.Clamp(lift, -1, 1);
                                    SetLift(lift);
                                    break;
                                }
                            //GAMMA
                            case 22:
                                {
                                    if (!videoWindowContent[22].interactable) return;
                                    gamma += (axisValue / 100);
                                    gamma = Mathf.Clamp(gamma, -1, 1);
                                    SetGamma(gamma);
                                    break;
                                }
                            //GAIN
                            case 23:
                                {
                                    if (!videoWindowContent[23].interactable) return;
                                    gain += (axisValue / 100);
                                    gain = Mathf.Clamp(gain, -1, 1);
                                    SetGain(gain);
                                    break;
                                }
                            //BRIGHTNESS
                            case 13:
                                {
                                    brightness += (axisValue / 10);
                                    brightness = Mathf.Clamp(brightness, 0, 10);
                                    SetBrightness(brightness);
                                    break;
                                }
                            //GRAIN
                            case 14: SetPostProcessingEffect(5); break;
                            //GRAINAMT     
                            case 15:
                                {
                                    if (!videoWindowContent[15].interactable) return;
                                    grainIntensity += (axisValue / 100);
                                    grainIntensity = Mathf.Clamp(grainIntensity, 0, 1);
                                    SetGrain(grainIntensity);
                                    break;
                                }
                            //VIGNETTE
                            case 16: SetPostProcessingEffect(6); break;
                            //VIGNETTEAMT
                            case 17:
                                {
                                    if (!videoWindowContent[17].interactable) return;
                                    vignetteIntensity += (axisValue / 100);
                                    vignetteIntensity = Mathf.Clamp(vignetteIntensity, 0, 1);
                                    SetVignette(vignetteIntensity);
                                    break;
                                }
                            //BLOOMAMT
                            case 18:
                                {
                                    if (!videoWindowContent[18].interactable) return;
                                    bloomIntensity += (axisValue / 10);
                                    bloomIntensity = Mathf.Clamp(bloomIntensity, 0, 10);
                                    SetBloom(bloomIntensity);
                                    break;
                                }
                        }
                        break;
                    }
                //AUDIO
                case 4:
                    {
                        if (!audioHighlightedContent[contentIndex]) return;
                        //VOLUMEALLSETTINGS
                        if (contentIndex < 5)
                        {
                            volumeInterval[contentIndex] += (axisValue / 1000);
                            volumeInterval[contentIndex] = Mathf.Clamp(volumeInterval[contentIndex], 0.01f, 1);
                            SetAudioValue(contentIndex);
                        }
                        //SPEAKERMODE/CONFIG
                        else
                        {
                            switch (contentIndex)
                            {
                                case 5: speakerIndex = RunContentAction(speakerIndex, speakerName.Length - 1); SetSpeakerMode(); break;
                                case 6: virtualVoiceIndex = RunContentAction(virtualVoiceIndex, voiceInterval.Length - 1); SetVirtualVoices(); break;
                                case 7: realVoiceIndex = RunContentAction(realVoiceIndex, voiceInterval.Length - 1); SetRealVoices(); break;
                                case 8: sampleRateIndex = RunContentAction(sampleRateIndex, sampleInterval.Length - 1); SetSampleRate(); break;
                                case 9: dpsBufferIndex = RunContentAction(dpsBufferIndex, bufferInterval.Length - 1); SetDPSBuffer(); break;
                                case 10: soundtrackIndex = RunContentAction(soundtrackIndex, songName.Length - 1); SetMusicSoundtrack(); break;
                            }
                        }
                        break;
                    }
            }
        }
    }
    public void DefaultOptions()
    {
        if(inputPlayer == null)
            Start();
        //------------------------------------------------------------------------
        //GAME SETTINGS-----------------------------------------------------------
        //------------------------------------------------------------------------
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~DIFFICULTY
        difficultyIndex = 1;
        SetDifficulty();
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~AUTOAIM
        autoAIndex = 1;
        SetAutoAim();
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~AUTOSAVE
        autoSIndex = 1;
        SetAutoSave();
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~CAMERABOBBING
        cameraBIndex = 1;
        SetCameraBobbing();
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~WEAPONHAND
        handIndex = 1;
        SetWeaponHand();
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~AUTOSWITCHNEW
        switchNewIndex = 1;
        SetAutoSwitchNew();
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~AUTOSWITCHEMPTY
        switchEmptyIndex = 1;
        SetAutoSwitchEmpty();
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~WEAPONMOVEMENT
        weaponMoveIndex = 1;
        SetWeaponMovement();

        //------------------------------------------------------------------------
        //CONTROL SETTINGS--------------------------------------------------------
        //------------------------------------------------------------------------
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~CONTROLLERENABLED
        controllerIndex = inputPlayer.controllers.Joysticks.Count > 0 ? 1 : 0;
        SetControllerActive();
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~VIBRATION
        vibrationIndex = 1;
        SetVibrationActive();
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~INVERTY
        invertIndex = 0;
        SetInvertActive();
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~X&YSENSITIVITY
        for(int s = 0; s < sensitivity.Length; s++)
        {
            sensitivity[s] = 2.5f;
        }
        SetSensitivityY();
        SetSensitivityX();
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~SMOOTHROTATION
        smoothIndex = 0;
        SetSmoothActive();
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ALWAYSRUN
        runIndex = 0;
        SetRunActive();
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~AIRCONTROL
        airIndex = 0;
        SetAirControlActive();
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~LOCKYAXIS
        lockYIndex = 0;
        SetLockYActive();
        //------------------------------------------------------------------------
        //UI SETTINGS-------------------------------------------------------------
        //------------------------------------------------------------------------
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~SHOWMESSAGES
        messagesActiveIndex = 1;
        SetMessagesActive();
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~SHOWTIME
        timeActiveIndex = 0;
        SetTimeActive();
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~SHOWFRAMERATE
        fpsActiveIndex = 0;
        SetFPSActive();
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~FLASHEFFECTS
        flashIndex = 1;
        SetFlashEffectsActive();
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~HUDSTYLE
        hudIndex = 3;
        SetHUD();
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~THEMECOLOR
        visorColorIndex = 0;
        SetVisorColor();
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~CROSSHAIRSTYLE
        crosshairIndex = 0;
        SetCrosshair();
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ENVIRONMENTEFFECTS
        environmentIndex = 0;
        SetEnvironmentEffectsActive();
        //------------------------------------------------------------------------
        //VIDEO SETTINGS----------------------------------------------------------
        //------------------------------------------------------------------------
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~WINDOWEDMODE
        fullScreenIndex = 1;
        SetWindowedScreen();
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~QUALITY
        qualityIndex = 0;
        QualitySettings.SetQualityLevel(qualityIndex);
        SetQuality();
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~RESOLUTION
        width = Screen.width;
        height = Screen.height;
        refreshRate = 60;
        Screen.SetResolution(width, height, screenMode, refreshRate);
        for (int i = 0; i < resolutions.Length; i++)
        {
            if (resolutions[i].width == Screen.currentResolution.width && resolutions[i].height == Screen.currentResolution.height)
                resolutionIndex = i;
        }
        SetResolution();
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~VSYNC
        vSyncIndex = 1;
        SetVSync();
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~MONITORHDR
        CheckHDRMode();
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~FIELDOFVIEW
        fieldOfView = 90;
        SetFieldOfView(fieldOfView);
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~POSTPROCESSINGEFFECTS
        for (int p = 0; p < 8; p++)
        {
            enablePostEffect[p] = false;
            postEffectIndex[p] = 0;
            switch (p)
            {
                case 0: bloomLayer.enabled.value = false; videoContentSubText[6].text = ONOFFSubText(false);
                    videoWindowContent[18].interactable = false; 
                    break;
                case 1: chromaticAberrationLayer.enabled.value = false; videoContentSubText[7].text = ONOFFSubText(false); break;
                case 2: lensLayer.enabled.value = false; videoContentSubText[8].text = ONOFFSubText(false); break;
                case 3: ambientLayer.enabled.value = false; videoContentSubText[9].text = ONOFFSubText(false); break;
                case 4: colorGradingLayer.enabled.value = false; videoContentSubText[10].text = ONOFFSubText(false);
                    videoWindowContent[11].interactable = false;
                    videoWindowContent[12].interactable = false;
                    videoWindowContent[19].interactable = false;
                    videoWindowContent[20].interactable = false;
                    videoWindowContent[21].interactable = false;
                    videoWindowContent[22].interactable = false;
                    videoWindowContent[23].interactable = false;
                    break;
                case 5: grainLayer.enabled.value = false; videoContentSubText[14].text = ONOFFSubText(false);
                    videoWindowContent[15].interactable = false;
                    break;
                case 6: vignetteLayer.enabled.value = false; 
                    videoContentSubText[16].text = ONOFFSubText(false);
                    videoWindowContent[17].interactable = false;
                    break;
            }
           
        }
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~BRIGHTNESS
        brightness = 2;
        SetBrightness(brightness);
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~BLOOMAMOUNT
        bloomIntensity = 1;
        SetBloom(bloomIntensity);
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~COLORGRADINGMODE
        gradingIndex = 0;
        SetColorGradingMode();
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~COLORGRADINGTONEMAPPER
        toneMapIndex = 0;
        SetColorGradingTone();
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~COLORGRADINGTONESATURATION
        saturation = 0;
        SetSaturation(saturation);
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~COLORGRADINGTONECONTRAST
        contrast = 0;
        SetContrast(contrast);
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~COLORGRADINGTONELIFT
        lift = 0;
        SetLift(lift / 100);
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~COLORGRADINGTONEGAMMA
        gamma = 0;
        SetGamma(gamma / 100);
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~COLORGRADINGTONEGAIN
        gain = 0;
        SetGain(gain / 100);
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~GRAINAMOUNT
        grainIntensity = 0.2f;
        SetGrain(grainIntensity);
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~VIGNETTEAMOUNT
        vignetteIntensity = 0.2f;
        SetVignette(vignetteIntensity);
        //------------------------------------------------------------------------
        //AUDIO SETTINGS----------------------------------------------------------
        //------------------------------------------------------------------------
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~SOUNDTRACKPLAYER
        soundtrackIndex = 0;
        SetMusicSoundtrack();
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ALLVOLUMEVALUES
        for (int v = 0; v < 5; v++)
        {
            volumeInterval[v] = 0.5f;
            audioContentSubText[v].text = FormatValuesPercent(volumeInterval[v]);
            audioContentSlider[v].fillAmount = volumeInterval[v];
            SetAudioValue(v);
        }
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~SPEAKERMODE
        speakerIndex = 1;
        SetSpeakerMode();
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~VIRTUALVOICES
        for (int vv = 0; vv < voiceInterval.Length; vv++)
        {
            if (defaultConfig.numVirtualVoices == voiceInterval[vv])
                virtualVoiceIndex = vv;
        }
        SetVirtualVoices();
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~REALVOICES
        for (int rv = 0; rv < voiceInterval.Length; rv++)
        {
            if (defaultConfig.numRealVoices == voiceInterval[rv])
                realVoiceIndex = rv;
        }
        SetRealVoices();
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~SAMPLERATE
        for (int s = 0; s < sampleInterval.Length; s++)
        {
            if (defaultConfig.sampleRate == sampleInterval[s])
                sampleRateIndex = s;
        }
        SetSampleRate();
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~DPSBUFFER
        for (int b = 0; b < bufferInterval.Length; b++)
        {
            if (defaultConfig.dspBufferSize == bufferInterval[b])
                dpsBufferIndex = b;
        }
        SetDPSBuffer();
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~AUDIOCONFIGURATION
        audioConfig = defaultConfig;
        ApplyAudioConfiguration();
        optionsLoaded = false;
        SetApplyButtonActive(false);
        if (optionsOpen) SelectSettingPanel(panelIndex);
    }
    public void LoadOptions()
    {
        if (inputPlayer == null)
            Start();
        if (playerSystem == null)
        {
            player.SetActive(true);
            gameUI.SetActive(true);
            playerSystem = PlayerSystem.playerSystem;
        }
        if(audioSystem == null) audioSystem = AudioSystem.audioSystem;
        gameSystem.SetMasterStart();
        if (gameSystem.Load())
        {
            //------------------------------------------------------------------------
            //GAME SETTINGS-----------------------------------------------------------
            //------------------------------------------------------------------------

            //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~DIFFICULTY
            SetDifficulty();
            //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~AUTOAIM
            SetAutoAim();
            //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~AUTOSAVE
            SetAutoSave();
            //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~CAMERABOBBING
            SetCameraBobbing();
            //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~WEAPONHAND
            SetWeaponHand();
            //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~AUTOSWITCHNEW
            SetAutoSwitchNew();
            //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~AUTOSWITCHEMPTY
            SetAutoSwitchEmpty();
            //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~WEAPONMOVEMENT
            SetWeaponMovement();

            //------------------------------------------------------------------------
            //CONTROL SETTINGS--------------------------------------------------------
            //------------------------------------------------------------------------

            //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~CONTROLLERENABLED
            controllerIndex = inputPlayer.controllers.Joysticks.Count > 0 ? 1 : 0;
            SetControllerActive();
            //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~VIBRATION
            SetVibrationActive();
            //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~INVERTY
            SetInvertActive();
            //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~X&YSENSITIVTY
            SetSensitivityY();
            SetSensitivityX();
            //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~SMOOTHROTATION
            SetSmoothActive();
            //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ALWAYSRUN
            SetRunActive();
            //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~AIRCONTROL
            SetAirControlActive();
            //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~LOCKYAXIS
            SetLockYActive();
            //------------------------------------------------------------------------
            //UI SETTINGS-------------------------------------------------------------
            //------------------------------------------------------------------------

            //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~SHOWMESSAGES
            SetMessagesActive();
            //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~SHOWTIME
            SetTimeActive();
            //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~SHOWFRAMERATE
            SetFPSActive();
            //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~FLASHEFFECTS
            SetFlashEffectsActive();
            //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~HUDSTYLE
            SetHUD();
            //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~COLORTHEME
            SetVisorColor();
            //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~CROSSHAIRSTYLE
            SetCrosshair();
            //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ENVIRONMENTEFFECTS
            SetEnvironmentEffectsActive();
            //------------------------------------------------------------------------
            //VIDEO SETTINGS----------------------------------------------------------
            //------------------------------------------------------------------------

            //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~WINDOWEDMODE
            SetWindowedScreen();
            //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~QUALITY
            QualitySettings.SetQualityLevel(qualityIndex);
            SetQuality();
            //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~RESOLUTION
            Screen.SetResolution(width, height, screenMode, refreshRate);
            for (int i = 0; i < resolutions.Length; i++)
            {
                if (resolutions[i].width == Screen.currentResolution.width && resolutions[i].height == Screen.currentResolution.height)
                    resolutionIndex = i;
            }
            SetResolution();
            //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~VSYNC
            SetVSync();
            //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~MONITORHDR
            CheckHDRMode();
            //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~FIELDOFVIEW
            SetFieldOfView(fieldOfView);
            //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~BRIGHTNESS
            SetBrightness(brightness);
            //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~POSTPROCESSINGEFFECTS
            int effectVal = 6;
            for (int p = 0; p < 5; p++)
            {
                enablePostEffect[p] = false;

                switch (p)
                {
                    case 0: bloomLayer.enabled.value = postEffectIndex[p] == 1 ? true : false;
                        videoWindowContent[18].interactable = postEffectIndex[p] == 1 ? true : false;
                        break;

                    case 1: chromaticAberrationLayer.enabled.value = postEffectIndex[p] == 1 ? true : false; break;
                    case 2: lensLayer.enabled.value = postEffectIndex[p] == 1 ? true : false; break;
                    case 3: ambientLayer.enabled.value = postEffectIndex[p] == 1 ? true : false; break;
                    case 4: colorGradingLayer.enabled.value = postEffectIndex[p] == 1 ? true : false;
                        videoWindowContent[11].interactable = postEffectIndex[p] == 1 ? true : false;
                        videoWindowContent[12].interactable = postEffectIndex[p] == 1 ? true : false;
                        videoWindowContent[19].interactable = postEffectIndex[p] == 1 ? true : false;
                        videoWindowContent[20].interactable = postEffectIndex[p] == 1 ? true : false;
                        videoWindowContent[21].interactable = postEffectIndex[p] == 1 ? true : false;
                        videoWindowContent[22].interactable = postEffectIndex[p] == 1 ? true : false;
                        videoWindowContent[23].interactable = postEffectIndex[p] == 1 ? true : false; break;
                    case 5: grainLayer.enabled.value = postEffectIndex[p] == 1 ? true : false; break;
                    case 6: vignetteLayer.enabled.value = postEffectIndex[p] == 1 ? true : false;
                        videoWindowContent[15].interactable = postEffectIndex[p] == 1 ? true : false;
                        videoWindowContent[17].interactable = postEffectIndex[p] == 1 ? true : false;
                        break;
                }
                videoContentSubText[effectVal].text = ONOFFSubText(postEffectIndex[p] == 1 ? true : false);
                effectVal++;
            }
            //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~BLOOMAMOUNT
            SetBloom(bloomIntensity);
            //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~COLORGRADINGMODE
            SetColorGradingMode();
            //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~COLORGRADINGTONEMAPPER
            SetColorGradingTone();
            //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~COLORGRADINGTONESATURATION
            SetSaturation(saturation);
            //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~COLORGRADINGTONECONTRAST
            SetContrast(contrast);
            //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~COLORGRADINGTONELIFT
            SetLift(lift / 100);
            //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~COLORGRADINGTONEGAMMA
            SetGamma(gamma / 100);
            //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~COLORGRADINGTONEGAIN
            SetGain(gain / 100);
            //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~GRAINAMOUNT
            SetGrain(grainIntensity);
            //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~VIGNETTEAMOUNT
            SetVignette(vignetteIntensity);
            //------------------------------------------------------------------------
            //AUDIO SETTINGS----------------------------------------------------------
            //------------------------------------------------------------------------

            //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~MUSICSOUNDTRACK
            musicClip = musicTracks[soundtrackIndex];
            audioContentSubText[10].text = songName[soundtrackIndex];
            //SetMusicSoundtrack();
            //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ALLVOLUMEVALUES
            for (int v = 0; v < 5; v++)
            {
                audioContentSubText[v].text = FormatValuesPercent(volumeInterval[v]);
                audioContentSlider[v].fillAmount = volumeInterval[v];
                SetAudioValue(v);
            }
            //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~SPEAKERMODE
            SetSpeakerMode();
            //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~VIRTUALVOICES
            SetVirtualVoices();
            //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~REALVOICES
            SetRealVoices();
            //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~SAMPLERATE
            SetSampleRate();
            //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~DPSBUFFER
            SetDPSBuffer();
            //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~AUDIOCONFIGURATION
            ApplyAudioConfiguration();
            optionsLoaded = true;
            SetApplyButtonActive(false);
        }
        else DefaultOptions();
        if (player.activeInHierarchy) { player.SetActive(false); }
        if (gameUI.activeInHierarchy) { gameUI.SetActive(false); }
    }
  
    private string InstantMessages()
    {
        // Build messages per char into string builder
        if (ContentSelected(currentContent))
        {
            switch (panelIndex)
            {
                //GAME
                case 0: stringBuilderMessage.Append(gameHighlightedMessages[contentIndex]); break;
                //CONTROL
                case 1: stringBuilderMessage.Append(controlHighlightedMessages[contentIndex]); break;
                //UI
                case 2: stringBuilderMessage.Append(uiHighlightedMessages[contentIndex]); break;
                //VIDEO
                case 3: stringBuilderMessage.Append(videoHighlightedMessages[contentIndex]); break;
                //AUDIO
                case 4: stringBuilderMessage.Append(audioHighlightedMessages[contentIndex]); break;
            }
        }
        else
        {
            stringBuilderMessage.Append(panelHighlightedMessages[selectionIndex]);
        }

        // return appended message
        return stringBuilderMessage.ToString();
    } 
    private int RoundOff(int number, int interval)
    {
        int remainder = number % interval;
        number += (remainder < interval / 2) ? -remainder : (interval - remainder);
        return number;
    }
    private string ConvertIntToText(int value)
    {
        if (stringBuilderIntToText.Length > 0)
            stringBuilderIntToText.Clear();
        stringBuilderIntToText.Append(value);
        return stringBuilderIntToText.ToString();
    }
    private string FormatValuesPercent(float value)
    {
        if (stringBuilderValue.Length > 0)
            stringBuilderValue.Clear();
        stringBuilderValue.Append(Mathf.RoundToInt(value * 100) + "%");
        return stringBuilderValue.ToString();
    }
    private string FormatValuesOnePercent(float value)
    {
        if (stringBuilderValue.Length > 0)
            stringBuilderValue.Clear();
        stringBuilderValue.Append(Mathf.RoundToInt(value) + "%");
        return stringBuilderValue.ToString();
    }
    private string FormatValuesOne(float value)
    {
        if (stringBuilderValue.Length > 0)
            stringBuilderValue.Clear();
        stringBuilderValue.Append(Mathf.FloorToInt(value * 1));
        return stringBuilderValue.ToString();
    }
    private string FormatValuesTen(float value)
    {
        if (stringBuilderValue.Length > 0)
            stringBuilderValue.Clear();
        stringBuilderValue.Append(value * 10);
        return stringBuilderValue.ToString();
    }
    private void SetHighlightedContent(bool active)
    {
        bool[] content = new bool[5];
        //Make sure all content is shut off if active
        for (int c = 0; c < gameHighlightedContent.Length; c++) { if (gameHighlightedContent[c]) gameHighlightedContent[c] = false; }
        for (int c = 0; c < controlHighlightedContent.Length; c++) { if (controlHighlightedContent[c]) controlHighlightedContent[c] = false; }
        for (int c = 0; c < uiHighlightedContent.Length; c++) { if (uiHighlightedContent[c]) uiHighlightedContent[c] = false; }
        for (int c = 0; c < videoHighlightedContent.Length; c++) { if (videoHighlightedContent[c]) videoHighlightedContent[c] = false; }
        for (int c = 0; c < audioHighlightedContent.Length; c++) { if (audioHighlightedContent[c]) audioHighlightedContent[c] = false; }
        if (!active) return;
        //Set the active content
        switch (panelIndex)
        {
            //GAME
            case 0: content = gameHighlightedContent; break;
            //CONTROL
            case 1: content = controlHighlightedContent; break;
            //UI
            case 2: content = uiHighlightedContent; break;
            //VIDEO
            case 3: content = videoHighlightedContent; break;
            //AUDIO
            case 4: content = audioHighlightedContent; break;
        }
        currentContent = content;
        for (int ac = 0; ac < content.Length; ac++)
        {
            if (ac == contentIndex) content[ac] = true;
            else content[ac] = false;
        }
    }
    private bool ContentSelected(bool[] content)
    {
        //Check if any content is active return true if there is
        for (int ac = 0; ac < content.Length; ac++)
        {
            if (content[ac]) return true;
        }
        return false;
    }
    private string ONOFFSubText(bool active)
    {
        return onOffText[active ? 1 : 0];
    }
    private float Map(float value, float inMin, float inMax, float outMin, float outMax)
    {
        return (value - inMin) * (outMax - outMin) / (inMax - inMin) + outMin;
    }
    public void OpenOptions(bool active)
    {
        optionSelection.SetActive(active);

        if (quitOpen) OpenQuitMenu(false);
        if (active)
        {
          
            canQuit = false;
            gameSystem.mainmenuOpen = false;
            SelectSettingPanel(panelIndex);
        }
        else
        {
            gameSystem.Save();
            for (int sp = 0; sp < optionPanels.Length; sp++)
                optionPanels[sp].SetActive(false);


        }
        optionsOpen = active;
    }
    public void SelectSettingPanel(int index)
    {
        panelIndex = index;
        contentIndex = 0;
        for (int sw = 0; sw < panelOpen.Length; sw++)
        {
            if (sw == panelIndex)
            {
                if (!optionPanels[sw].activeInHierarchy)
                    optionPanels[sw].SetActive(true);
                panelScrollBars[panelIndex].value = 1;
                SetSelectedPanel(panelIndex);
            }
            else
            {
                if (optionPanels[sw].activeInHierarchy)
                    optionPanels[sw].SetActive(false);
            }
        }
    }
    public void SetSelectedPanel(int index)
    {
        for (int p = 0; p < selectionFirstSelected.Length; p++)
        {
            if (p == index)
            {
                b_Select = selectionFirstSelected[p].GetComponent<Selectable>();
                buttonHighlight = selectionFirstSelected[p].GetComponent<ButtonHighlight>();
                buttonHighlight.Activate(true);
                b_Select.Select();
                break;
            }
        }
    }
    public void Quit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    private int RunContentAction(int index, int maxIndex)
    {
        if (inputPlayer.GetAxis("LSHUI") == 1 || inputPlayer.GetAxis("LSHUI") == -1 || inputPlayer.GetButtonDown("LSHUI"))
        {
            index += inputPlayer.GetAxis("LSHUI") > 0 ? 1 : -1;
            index = Mathf.Clamp(index, 0, maxIndex);
            audioSystem.PlayAudioSource(selectFx, 1, 1, 128);
            selectingOption = true;
            return index;
        }
        return index; 
        
    }

    //+++++++++++++++++++ Needs to be accessible for Button Events +++++++++
    public void HighlightPanelContent(int selectedIndex)
    {
        contentIndex = selectedIndex;
        SetHighlightedContent(true);
        if (charIndex != 0) charIndex = 0;
        if (stringBuilderMessage.Length > 0)
            stringBuilderMessage.Clear();
        highlightedMessage.text = InstantMessages();
    }
    public void HighlightPanelSelection(int selectedIndex)
    {
        selectionIndex = selectedIndex;
        SetHighlightedContent(false);
        if (charIndex != 0) charIndex = 0;
        if (stringBuilderMessage.Length > 0)
            stringBuilderMessage.Clear();
        highlightedMessage.text = InstantMessages();
    }
    //====================================================================
    //=======================GameMethods==================================
    //==================================================================== 
    public void SetDifficulty()
    {
        for(int d = 0; d < 4; d++)
        {
            if (difficultyIndex == d) difficultyActive[d] = true;
            else difficultyActive[d] = false;
        }
        gameContentSubText[0].text = difficultySubText[difficultyIndex];
    }
    public void ApplyDifficulty()
    {
        difficultyIndex++;
        if (difficultyIndex > 3) difficultyIndex = 0;
        for (int d = 0; d < difficultyActive.Length; d++)
        {
            if (d == difficultyIndex) difficultyActive[d] = true;
            else difficultyActive[d] = false;
        }
        gameContentSubText[0].text = difficultySubText[difficultyIndex];
    }
    //--------------------------------------------------------------------
    public void SetAutoAim()
    {
        autoAim = autoAIndex == 1 ? true : false;
        gameContentSubText[1].text = ONOFFSubText(autoAim);
    }
    public void ApplyAutoAim()
    {
        autoAim = !autoAim;
        autoAIndex = autoAim ? 1 : 0;
        gameContentSubText[1].text = ONOFFSubText(autoAim);
    }
    //--------------------------------------------------------------------
    public void SetAutoSave()
    {
        autoSave = autoSIndex == 1 ? true : false;
        gameContentSubText[2].text = ONOFFSubText(autoSave);
    }
    public void ApplyAutoSave()
    {
        autoSave = !autoSave;
        autoSIndex = autoSave ? 1 : 0;
        gameContentSubText[2].text = ONOFFSubText(autoSave);
    }
    //--------------------------------------------------------------------
    public void SetCameraBobbing()
    {
        cameraBobbing = cameraBIndex == 1 ? true : false;
        gameContentSubText[3].text = ONOFFSubText(cameraBobbing);
    }
    public void ApplyCameraBobbing()
    {
        cameraBobbing = !cameraBobbing;
        cameraBIndex = cameraBobbing ? 1 : 0;
        gameContentSubText[3].text = ONOFFSubText(cameraBobbing);
    }
    //--------------------------------------------------------------------
    public void SetWeaponHand()
    {
        for (int wh = 0; wh < 3; wh++)
        {
            if (handIndex == wh) handActive[wh] = true;
            else handActive[wh] = false;
        }
        if (weaponSystem == null) weaponSystem = WeaponSystem.weaponSystem;
        weaponSystem.SelectWeaponHand(handIndex);
        gameContentSubText[7].text = handSubText[handIndex];
    }
    public void ApplyWeaponHand()
    {
        handIndex++;
        if (handIndex > 2) handIndex = 0;
        for (int d = 0; d < handActive.Length; d++)
        {
            if (d == handIndex) handActive[d] = true;
            else handActive[d] = false;
        }
        if (weaponSystem == null) weaponSystem = WeaponSystem.weaponSystem;
        weaponSystem.SelectWeaponHand(handIndex);
        gameContentSubText[7].text = handSubText[handIndex];
    }
    //--------------------------------------------------------------------
    public void SetAutoSwitchNew()
    {
        autoSwitchNew = switchNewIndex == 1 ? true : false;
        gameContentSubText[4].text = ONOFFSubText(autoSwitchNew);
    }
    public void ApplyAutoSwitchNew()
    {
        autoSwitchNew = !autoSwitchNew;
        switchNewIndex = autoSwitchNew ? 1 : 0;
        gameContentSubText[4].text = ONOFFSubText(autoSwitchNew);
    }
    //--------------------------------------------------------------------
    public void SetAutoSwitchEmpty()
    {
        autoSwitchEmpty = switchEmptyIndex == 1 ? true : false;
        gameContentSubText[5].text = ONOFFSubText(autoSwitchEmpty);
    }
    public void ApplyAutoSwitchEmpty()
    {
        autoSwitchEmpty = !autoSwitchEmpty;
        switchEmptyIndex = autoSwitchEmpty ? 1 : 0;
        gameContentSubText[5].text = ONOFFSubText(autoSwitchEmpty);
    }
    public void SetWeaponMovement()
    {
        weaponMovement = weaponMoveIndex == 1 ? true : false;
        gameContentSubText[6].text = ONOFFSubText(weaponMovement);
    }
    public void ApplyWeaponMovement()
    {
        weaponMovement = !weaponMovement;
        weaponMoveIndex = weaponMovement ? 1 : 0;
        gameContentSubText[6].text = ONOFFSubText(weaponMovement);
    }
    //====================================================================
    //=======================ControlMethods===============================
    //==================================================================== 
    public void SetControllerActive()
    {
        controllerEnabled = controllerIndex == 1 ? true : false;
        controlWindowContent[5].interactable = controllerEnabled;

        if(!controllerEnabled) controlContentSubText[5].text = "Disabled";
        controlContentSubText[0].text = ONOFFSubText(controllerEnabled);
        EnableController();
    }
    public void ApplyControllerActive()
    {
        controllerEnabled = !controllerEnabled;
        controllerIndex = controllerEnabled ? 1 : 0;
        controlWindowContent[5].interactable = controllerIndex == 1 ? true : false;
        if (controllerIndex == 0) controlContentSubText[5].text = "Disabled";
        controlContentSubText[0].text = ONOFFSubText(controllerEnabled);
        EnableController();
    }
    //--------------------------------------------------------------------
    public void SetInvertActive()
    {
        invertY = invertIndex == 1 ? true : false;
        controlContentSubText[1].text = ONOFFSubText(invertY);
    }
    public void ApplyInvertActive()
    {
        invertY = !invertY;
        invertIndex = invertY ? 1 : 0;
        controlContentSubText[1].text = ONOFFSubText(invertY);
    }
    //--------------------------------------------------------------------
    public void SetSensitivityY()
    {
        if (sensitivity[1].GetType() != typeof(int)) sensitivity[1] = Mathf.Round(sensitivity[1]);
        if (sensitivity[1] > 10) sensitivity[1] = 1f;
        sensitivity[1] = Mathf.Clamp(sensitivity[1], 0.1f, 10);
        controlContentSubText[2].text = FormatValuesOne(sensitivity[1]);
        controlContentSlider[1].fillAmount = sensitivity[1] / 10;
    }
    public void ApplySensitivityY()
    {
        if (sensitivity[1].GetType() != typeof(int)) sensitivity[1] = Mathf.Round(sensitivity[1]);
        sensitivity[1] += 1;
        if (sensitivity[1] > 10) sensitivity[1] = 1f;
        sensitivity[1] = Mathf.Clamp(sensitivity[1], 0.1f, 10);

        controlContentSubText[contentIndex].text = FormatValuesOne(sensitivity[1]);
        controlContentSlider[1].fillAmount = sensitivity[1] / 10;
    }
    public void SetSensitivityX()
    {
        if (sensitivity[0].GetType() != typeof(int)) sensitivity[0] = Mathf.Round(sensitivity[0]);
        if (sensitivity[0] > 10) sensitivity[0] = 1f;
        sensitivity[0] = Mathf.Clamp(sensitivity[0], 0.1f, 10);
        controlContentSubText[3].text = FormatValuesOne(sensitivity[0]);
        controlContentSlider[0].fillAmount = sensitivity[1] / 10;
    }
    public void ApplySensitivityX()
    {
        if(sensitivity[0].GetType() != typeof(int)) sensitivity[0] = Mathf.Round(sensitivity[0]);
        sensitivity[0] += 1;
        if (sensitivity[0] > 10) sensitivity[0] = 1f;
        sensitivity[0] = Mathf.Clamp(sensitivity[0], 0.1f, 10);

        controlContentSubText[contentIndex].text = FormatValuesOne(sensitivity[0]);
        controlContentSlider[0].fillAmount = sensitivity[0] / 10;
    }
    //--------------------------------------------------------------------
    public void SetSmoothActive()
    {
        smoothRotation = smoothIndex == 1 ? true : false;
        controlContentSubText[4].text = ONOFFSubText(smoothRotation);
    }
    public void ApplySmoothActive()
    {
        smoothRotation = !smoothRotation;
        smoothIndex = smoothRotation ? 1 : 0;
        controlContentSubText[4].text = ONOFFSubText(smoothRotation);
    }
    //--------------------------------------------------------------------
    public void SetVibrationActive()
    {
        vibration = vibrationIndex == 1 ? true : false;

        if (vibrationIndex == 1 && testVibrate)
        {
            inputPlayer.SetVibration(0, 1f, 0.25f);
            testVibrate = false; 
        }
        else if (vibrationIndex == 0) 
            testVibrate = true;
        if (!controllerEnabled) controlContentSubText[5].text = "Disabled";
        else
            controlContentSubText[5].text = ONOFFSubText(vibration);
    }
    public void ApplyVibrationActive()
    {
        vibration = !vibration;
        vibrationIndex = vibration ? 1 : 0;
        if (vibrationIndex == 1 && testVibrate) 
        {
            inputPlayer.SetVibration(0, 1f, 0.25f);
            testVibrate = false; 
        }
        else if (vibrationIndex == 0) 
            testVibrate = true;
        if (!controllerEnabled) controlContentSubText[5].text = "Disabled";
        else controlContentSubText[5].text = ONOFFSubText(vibration);
    }
    //--------------------------------------------------------------------
    public void SetRunActive()
    {
        alwaysRun = runIndex == 1 ? true : false;
        controlContentSubText[6].text = ONOFFSubText(alwaysRun);
    }
    public void ApplyRunActive()
    {
        alwaysRun = !alwaysRun;
        runIndex = alwaysRun ? 1 : 0;
        controlContentSubText[6].text = ONOFFSubText(alwaysRun);
    }
    //--------------------------------------------------------------------
    public void SetAirControlActive()
    {
        airControl = airIndex == 1 ? true : false;
        controlContentSubText[7].text = ONOFFSubText(airControl);
    }
    public void ApplyAirControlActive()
    {
        airControl = !airControl;
        airIndex = airControl ? 1 : 0;
        controlContentSubText[7].text = ONOFFSubText(airControl);
    }
    //--------------------------------------------------------------------
    public void SetLockYActive()
    {
        lockYAxis = lockYIndex == 1 ? true : false;
        controlContentSubText[8].text = ONOFFSubText(lockYAxis);
    }
    public void ApplyLockYActive()
    {
        lockYAxis = !lockYAxis;
        lockYIndex = lockYAxis ? 1 : 0;
        controlContentSubText[8].text = ONOFFSubText(lockYAxis);
    }
    //====================================================================
    //=======================UIMethods====================================
    //==================================================================== 
    public void SetMessagesActive()
    {
        showMessages = messagesActiveIndex == 1 ? true : false;
        uiContentSubText[0].text = ONOFFSubText(showMessages);
    }
    public void ApplyMessagesActive()
    {
        showMessages = !showMessages;
        messagesActiveIndex = showMessages ? 1 : 0;
        uiContentSubText[0].text = ONOFFSubText(showMessages);
    }
    //--------------------------------------------------------------------
    public void SetTimeActive()
    {
        showTime = timeActiveIndex == 1 ? true : false;
        uiContentSubText[1].text = ONOFFSubText(showTime);
    }
    public void ApplyTimeActive()
    {
        showTime = !showTime;
        timeActiveIndex = showTime ? 1 : 0;
        uiContentSubText[1].text = ONOFFSubText(showTime);
    }
    public string DateTime()
    {
        stringBuilderTime.Clear();
        stringBuilderTime.Append(date + System.DateTime.Now.ToString(timeFormat));
        return stringBuilderTime.ToString();
    }
    public void ShowDateTime()
    {
        if (!showTime) { if (dateTimeText.text != null) dateTimeText.text = null; return; }
        if (dateTimeText.text != DateTime()) dateTimeText.text = DateTime();
    }
    //--------------------------------------------------------------------
    public void SetFPSActive()
    {
        showFrameRate = fpsActiveIndex == 1 ? true : false;
        uiContentSubText[2].text = ONOFFSubText(showFrameRate);
    }
    public void ApplyFPSActive()
    {
        showFrameRate = !showFrameRate;
        fpsActiveIndex = showFrameRate ? 1 : 0;
        uiContentSubText[2].text = ONOFFSubText(showFrameRate);
    }
    public string FPS()
    {
        stringBuilderFps.Clear();
        fpsCount = (1 / unScaledDTime);
        int rID = 0;
        if (Screen.currentResolution.width < 1280 && Screen.currentResolution.height < 720) rID = 0;
        else if ((Screen.currentResolution.width > 1279 && Screen.currentResolution.width < 1921) && Screen.currentResolution.height > 719 && Screen.currentResolution.height <= 1080) rID = 1;
        else rID = 2;
        stringBuilderFps.Append(resolutionIDs[rID] + screenSize + Screen.currentResolution.width + " X " + Screen.currentResolution.height + " "+ Screen.currentResolution.refreshRate + "HZ" + fpsLabel + Mathf.Round(fpsCount));
        return stringBuilderFps.ToString();
    }
    public void ShowFPS()
    {
        if (!showFrameRate) { if (fpsText.text != null) fpsText.text = null; return; }
        fpsTimer -= unScaledDTime;
        fpsTimer = Mathf.Clamp(fpsTimer, 0, fpsTime);
        if (fpsTimer == 0)
        {
            if (fpsText.text != FPS()) fpsText.text = FPS();
            fpsTimer = fpsTime;
        }
    }
    //--------------------------------------------------------------------
    public void SetFlashEffectsActive()
    {
        showFlashEffects = flashIndex == 1 ? true : false;
        uiContentSubText[3].text = ONOFFSubText(showFlashEffects);
    }
    public void ApplyEffectsActive()
    {
        showFlashEffects = !showFlashEffects;
        flashIndex = showFlashEffects ? 1 : 0;
        uiContentSubText[3].text = ONOFFSubText(showFlashEffects);
    }
    //--------------------------------------------------------------------
    public void SetHUD()
    {
        playerSystem.versionIndex = hudIndex;
        playerSystem.UIVersion(playerSystem.versionIndex);
        uiContentSubText[4].text = hudSubText[hudIndex];
    }
    public void ApplyHUD()
    {
        hudIndex++;
        if (hudIndex > 3) hudIndex = 0;
        playerSystem.versionIndex = hudIndex;
        playerSystem.UIVersion(playerSystem.versionIndex);
        uiContentSubText[4].text = hudSubText[hudIndex];
    }
    //--------------------------------------------------------------------
    public void SetVisorColor()
    {
        if(visorColorIndex < 1)
        {
            playerSystem.visorColor = visorColor2[visorColorIndex];
            playerSystem.visorColor2 = visorColor2[visorColorIndex];
            ChangeOptionsUIColor(visorColor[visorColorIndex], visorColor[visorColorIndex]);
            playerSystem.ChangeVisorColorOverhaul();
           
        }
        else
        {
            playerSystem.visorColor = visorColor[visorColorIndex];
            playerSystem.visorColor2 = visorColor2[visorColorIndex];
            ChangeOptionsUIColor(visorColor[visorColorIndex], visorColor2[visorColorIndex]);
        }
        playerSystem.ChangeVisorColorOverhaul();
        uiContentSubText[5].text = VisorColorSubText[visorColorIndex];
    }
    public void ApplyVisorColor()
    {
        visorColorIndex++;
        if (visorColorIndex > 10) visorColorIndex = 0;
        if (visorColorIndex < 1)
        {
            playerSystem.visorColor = visorColor2[visorColorIndex];
            playerSystem.visorColor2 = visorColor2[visorColorIndex];
            ChangeOptionsUIColor(visorColor[visorColorIndex], visorColor[visorColorIndex]);
            playerSystem.ChangeVisorColorOverhaul();
        }
        else
        {
            playerSystem.visorColor = visorColor[visorColorIndex];
            playerSystem.visorColor2 = visorColor2[visorColorIndex];
            ChangeOptionsUIColor(visorColor[visorColorIndex], visorColor2[visorColorIndex]);
        }
        playerSystem.ChangeVisorColorOverhaul();
        uiContentSubText[5].text = VisorColorSubText[visorColorIndex];
    }
    public void ChangeOptionsUIColor(Color color1, Color color2)
    {
        float alpha = 0.5f;
        Color[] alphaReturnedColor = new Color[2] { new Color(color1.r, color1.g, color1.b, alpha), new Color(color2.r, color2.g, color2.b, alpha), };
        for(int c1 = 0; c1 < OptionsWindowPriColorElements.Length; c1++) OptionsWindowPriColorElements[c1].color = alphaReturnedColor[0];
        for (int c2 = 0; c2 < OptionsWindowSubColorElements.Length; c2++) OptionsWindowSubColorElements[c2].color = alphaReturnedColor[1];
    }
    //--------------------------------------------------------------------
    public void SetCrosshair()
    {
        if (crosshairIndex < 6)
        {
            if (!playerSystem.crosshair.enabled) playerSystem.crosshair.enabled = true;
            playerSystem.crosshair.sprite = crosshairSprites[crosshairIndex];
        }
        else if (playerSystem.crosshair.enabled) playerSystem.crosshair.enabled = false;
        uiContentSubText[6].text = crosshairSubText[crosshairIndex];
    }
    public void ApplyCrosshair()
    {
        crosshairIndex++;
        if (crosshairIndex > 6) crosshairIndex = 0;
        if (crosshairIndex < 6)
        {
            if (!playerSystem.crosshair.enabled) playerSystem.crosshair.enabled = true;
            playerSystem.crosshair.sprite = crosshairSprites[crosshairIndex];
        }
        else if (playerSystem.crosshair.enabled) playerSystem.crosshair.enabled = false;
        uiContentSubText[6].text = crosshairSubText[crosshairIndex];
    }
    //--------------------------------------------------------------------
    public void SetEnvironmentEffectsActive()
    {
        environmentEffects = environmentIndex == 1 ? true : false;
        uiContentSubText[7].text = ONOFFSubText(environmentEffects);
    }
    public void ApplyEnvironmentEffectsActive()
    {
        environmentEffects = !environmentEffects;
        environmentIndex = environmentEffects ? 1 : 0;
        uiContentSubText[7].text = ONOFFSubText(environmentEffects);
    }
    //====================================================================
    //=======================VideoMethods=================================
    //====================================================================
    public void SetWindowedScreen()
    {
        switch (fullScreenIndex)
        {
            case 0: { screenMode = FullScreenMode.ExclusiveFullScreen; break; }
            case 1: { screenMode = FullScreenMode.FullScreenWindow; break; }
            case 2: { screenMode = FullScreenMode.MaximizedWindow; break; }
            case 3: { screenMode = FullScreenMode.Windowed; break; }
        }
        if (Screen.fullScreenMode != screenMode)
            Screen.SetResolution(Screen.currentResolution.width, Screen.currentResolution.height, screenMode, Screen.currentResolution.refreshRate);
        videoContentSubText[0].text = fullScreenSubText[fullScreenIndex];
    }
    public void ApplyWindowedScreen()
    {
        fullScreenIndex++;
        if (fullScreenIndex > 3) fullScreenIndex = 0;
        switch (fullScreenIndex)
        {
            case 0: { screenMode = FullScreenMode.ExclusiveFullScreen; break; }
            case 1: { screenMode = FullScreenMode.FullScreenWindow; break; }
            case 2: { screenMode = FullScreenMode.MaximizedWindow; break; }
            case 3: { screenMode = FullScreenMode.Windowed; break; }
        }
        if (Screen.fullScreenMode != screenMode)
            Screen.SetResolution(Screen.currentResolution.width, Screen.currentResolution.height, screenMode, Screen.currentResolution.refreshRate);
        videoContentSubText[0].text = fullScreenSubText[fullScreenIndex];
    }
    public void SetQuality()
    {
        if (QualitySettings.GetQualityLevel() != qualityIndex)
            QualitySettings.SetQualityLevel(qualityIndex);
        videoContentSubText[1].text = qualitySubText[qualityIndex];
    }
    public void ApplyQuality()
    {
        qualityIndex++;
        if (qualityIndex > 5) qualityIndex = 0;
        if(QualitySettings.GetQualityLevel() != qualityIndex)
            QualitySettings.SetQualityLevel(qualityIndex);
        videoContentSubText[1].text = qualitySubText[qualityIndex];
    }
    public void SetResolution()
    {
        Resolution resolution = resolutions[resolutionIndex];
        width = resolution.width;
        height = resolution.height;
        refreshRate = resolution.refreshRate;
        if (width == Screen.currentResolution.width && height == Screen.currentResolution.height) return;
        Screen.SetResolution(width, height, screenMode, refreshRate);
        videoContentSubText[contentIndex].text = ResolutionSubText(width, height, refreshRate);
    }
    public void SetCustomResolution(int w, int h, int refreshRate)
    {
        if (w == Screen.currentResolution.width && h == Screen.currentResolution.height) return;
        Screen.SetResolution(w, h, screenMode, refreshRate);
        videoContentSubText[2].text = ResolutionSubText(w, h, refreshRate);
    }
    public void ApplyResolution()
    {
        resolutionIndex++;
        if (resolutionIndex > resolutions.Length - 1) resolutionIndex = 0;
        Resolution resolution = resolutions[resolutionIndex];
        width = resolution.width;
        height = resolution.height;
        refreshRate = resolution.refreshRate;
        Screen.SetResolution(width, height, screenMode, refreshRate);
        videoContentSubText[contentIndex].text = ResolutionSubText(width, height, refreshRate);
    }
    public string ResolutionSubText(int w, int h, int r)
    {
        stringBuilderResolution.Clear();
        stringBuilderResolution.Append(w + " x " + h + " " + r + "Hz");
        return stringBuilderResolution.ToString();
    }
    public void SetVSync()
    {
        QualitySettings.vSyncCount = vSyncIndex;
        enableVSync = vSyncIndex == 1 ? true : false;
        videoContentSubText[3].text = ONOFFSubText(enableVSync);
    }
    public void ApplyVSync()
    {
        if (QualitySettings.vSyncCount == 1)
            QualitySettings.vSyncCount = 0;
        else if (QualitySettings.vSyncCount == 0)
            QualitySettings.vSyncCount = 1;
        vSyncIndex = QualitySettings.vSyncCount;
        bool active = vSyncIndex == 1 ? true : false;
        videoContentSubText[3].text = ONOFFSubText(active);
    }
    public void SetHDRMode()
    {
        if (!HDROutputSettings.main.available) return;
        enableHDR = hdrIndex == 1 ? true : false;
        HDROutputSettings.main.RequestHDRModeChange(enableHDR);
        videoContentSubText[3].text = ONOFFSubText(enableHDR);
    }
    public void ApplyHDRMode()
    {
        if (!HDROutputSettings.main.available) return;
        if (HDROutputSettings.main.active) HDROutputSettings.main.RequestHDRModeChange(false);
        else if (!HDROutputSettings.main.active) HDROutputSettings.main.RequestHDRModeChange(true);
        enableHDR = HDROutputSettings.main.active;
        videoContentSubText[3].text = ONOFFSubText(enableHDR);
    }
    public void CheckHDRMode()
    {
        if (HDROutputSettings.main.available)
        {
            videoWindowContent[4].interactable = true;
            enableHDR = HDROutputSettings.main.active;
            videoContentSubText[3].text = ONOFFSubText(enableHDR);
        }
        else
        {
            videoWindowContent[4].interactable = false;
            videoContentSubText[4].text = "Unsupported";
        }
    }
    public void SetFieldOfView(float value)
    {
        videoContentSubText[5].text = FormatValuesOne(value);
        videoContentSlider[0].fillAmount = Map(value, 60, 120, 0, 1);
        gameCamera.fieldOfView = value;
        clipCamera.fieldOfView = value;
        if (weaponSystem == null) weaponSystem = WeaponSystem.weaponSystem;
        weaponSystem.SetFOVAdjustment();
    }
    public void ApplyFieldOfView()
    {
        if (fieldOfView.GetType() != typeof(int)) fieldOfView = RoundOff(Mathf.CeilToInt(fieldOfView), 10);
        fieldOfView += 10;
        if (fieldOfView > 120) fieldOfView = 60f;
        gameCamera.fieldOfView = fieldOfView;
        clipCamera.fieldOfView = fieldOfView;
      videoContentSubText[5].text = FormatValuesOne(fieldOfView);
        videoContentSlider[0].fillAmount = Map(fieldOfView, 60, 120, 0, 1);
    }
    public void SetBloom(float value)
    {
        videoContentSubText[18].text = FormatValuesOne(value);
        videoContentSlider[1].fillAmount = Map(value, 0, 10, 0, 1);
        bloomLayer.intensity.value = value;
    }
    public void ApplyBloom()
    {
        if (bloomIntensity.GetType() != typeof(int)) bloomIntensity = Mathf.Round(bloomIntensity);
        bloomIntensity += 1;
        if (bloomIntensity > 10) bloomIntensity = 0;
        bloomIntensity = Mathf.Clamp(bloomIntensity, 0.0f, 10);
        videoContentSubText[18].text = FormatValuesOne(bloomIntensity);
        videoContentSlider[1].fillAmount = Map(bloomIntensity, 0, 10, 0, 1);
        bloomLayer.intensity.value = bloomIntensity;
    }
    public void SetPostProcessingEffect(int index)
    {
        if (inputPlayer.GetAxis("LSHUI") == 1 || inputPlayer.GetAxis("LSHUI") == -1 || inputPlayer.GetButtonDown("LSHUI"))
        {
            postEffectIndex[index] += inputPlayer.GetAxis("LSHUI") > 0 ? 1 : -1;
            postEffectIndex[index] = Mathf.Clamp(postEffectIndex[index], 0, 1);
            enablePostEffect[index] = postEffectIndex[index] == 1 ? true : false;
            switch (index)
            {
                case 0: bloomLayer.enabled.value = enablePostEffect[index];
                        videoWindowContent[18].interactable = enablePostEffect[index]; break;
                case 1: chromaticAberrationLayer.enabled.value = enablePostEffect[index]; break;
                case 2: lensLayer.enabled.value = enablePostEffect[index]; break;
                case 3: ambientLayer.enabled.value = enablePostEffect[index]; break;
                case 4: colorGradingLayer.enabled.value = enablePostEffect[index]; 
                        videoWindowContent[11].interactable = enablePostEffect[index]; 
                        videoWindowContent[12].interactable = enablePostEffect[index]; 
                        videoWindowContent[19].interactable = enablePostEffect[index]; 
                        videoWindowContent[20].interactable = enablePostEffect[index]; 
                        videoWindowContent[21].interactable = enablePostEffect[index]; 
                        videoWindowContent[22].interactable = enablePostEffect[index]; 
                        videoWindowContent[23].interactable = enablePostEffect[index]; break;
                case 5: grainLayer.enabled.value = enablePostEffect[index]; 
                        videoWindowContent[15].interactable = enablePostEffect[index]; break;
                case 6: vignetteLayer.enabled.value = enablePostEffect[index]; 
                        videoWindowContent[17].interactable = enablePostEffect[index]; break;
            }
            videoContentSubText[contentIndex].text = ONOFFSubText(enablePostEffect[index]);
            selectingOption = true;
        }
    }
    public void SetpostProcessingIndividual(int ID, int val, int contentIndex)
    {
        postEffectIndex[ID] = val;
        enablePostEffect[ID] = postEffectIndex[ID] == 1 ? true : false;
        switch (ID)
        {
            case 0:
                bloomLayer.enabled.value = enablePostEffect[ID];
                videoWindowContent[18].interactable = enablePostEffect[ID]; break;
            case 1: chromaticAberrationLayer.enabled.value = enablePostEffect[ID]; break;
            case 2: lensLayer.enabled.value = enablePostEffect[ID]; break;
            case 3: ambientLayer.enabled.value = enablePostEffect[ID]; break;
            case 4:
                colorGradingLayer.enabled.value = enablePostEffect[ID];
                videoWindowContent[11].interactable = enablePostEffect[ID];
                videoWindowContent[12].interactable = enablePostEffect[ID];
                videoWindowContent[19].interactable = enablePostEffect[ID];
                videoWindowContent[20].interactable = enablePostEffect[ID];
                videoWindowContent[21].interactable = enablePostEffect[ID];
                videoWindowContent[22].interactable = enablePostEffect[ID];
                videoWindowContent[23].interactable = enablePostEffect[ID]; break;
            case 5:
                grainLayer.enabled.value = enablePostEffect[ID];
                videoWindowContent[15].interactable = enablePostEffect[ID]; break;
            case 6:
                vignetteLayer.enabled.value = enablePostEffect[ID];
                videoWindowContent[17].interactable = enablePostEffect[ID]; break;
        }
        videoContentSubText[contentIndex].text = ONOFFSubText(enablePostEffect[ID]);
    }
    public void ApplyPostProcessingEffect(int index)
    {
        switch (index)
        {
            case 0: 
                {
                    bloomLayer.enabled.value = !bloomLayer.enabled.value;
                    postEffectIndex[index] = bloomLayer.enabled.value ? 1 : 0;
                    enablePostEffect[index] = bloomLayer.enabled.value;
                    videoContentSubText[6].text = ONOFFSubText(enablePostEffect[index]);
                    videoWindowContent[18].interactable = enablePostEffect[index];
                    break; 
                }
            case 1:
                {
                    chromaticAberrationLayer.enabled.value = !chromaticAberrationLayer.enabled.value;
                    postEffectIndex[index] = chromaticAberrationLayer.enabled.value ? 1 : 0;
                    enablePostEffect[index] = chromaticAberrationLayer.enabled.value;
                    videoContentSubText[7].text = ONOFFSubText(enablePostEffect[index]);
                    break;
                }
            case 2:
                {
                    lensLayer.enabled.value = !lensLayer.enabled.value;
                    postEffectIndex[index] = lensLayer.enabled.value ? 1 : 0;
                    enablePostEffect[index] = lensLayer.enabled.value;
                    videoContentSubText[8].text = ONOFFSubText(enablePostEffect[index]);
                    break;
                }
            case 3:
                {
                    ambientLayer.enabled.value = !ambientLayer.enabled.value;
                    postEffectIndex[index] = ambientLayer.enabled.value ? 1 : 0;
                    enablePostEffect[index] = ambientLayer.enabled.value;
                    videoContentSubText[9].text = ONOFFSubText(enablePostEffect[index]);
                    break;
                }
            case 4:
                {
                    colorGradingLayer.enabled.value = !colorGradingLayer.enabled.value;
                    postEffectIndex[index] = colorGradingLayer.enabled.value ? 1 : 0;
                    enablePostEffect[index] = colorGradingLayer.enabled.value;
                    videoContentSubText[10].text = ONOFFSubText(enablePostEffect[index]);
                    videoWindowContent[11].interactable = enablePostEffect[index];
                    videoWindowContent[12].interactable = enablePostEffect[index];
                    videoWindowContent[19].interactable = enablePostEffect[index];
                    videoWindowContent[20].interactable = enablePostEffect[index];
                    videoWindowContent[21].interactable = enablePostEffect[index];
                    videoWindowContent[22].interactable = enablePostEffect[index];
                    videoWindowContent[23].interactable = enablePostEffect[index];
                    break;
                }
            case 5:
                {
                    grainLayer.enabled.value = !grainLayer.enabled.value;
                    postEffectIndex[index] = grainLayer.enabled.value ? 1 : 0;
                    enablePostEffect[index] = grainLayer.enabled.value;
                    videoContentSubText[14].text = ONOFFSubText(enablePostEffect[index]);
                    videoWindowContent[15].interactable = enablePostEffect[index];
                    break;
                }
            case 6:
                {
                    vignetteLayer.enabled.value = !vignetteLayer.enabled.value;
                    postEffectIndex[index] = vignetteLayer.enabled.value ? 1 : 0;
                    enablePostEffect[index] = vignetteLayer.enabled.value;
                    videoContentSubText[16].text = ONOFFSubText(enablePostEffect[index]);
                    videoWindowContent[17].interactable = enablePostEffect[index];
                    break;
                }
        }
        
    }
    public void SetColorGradingMode()
    {
        switch (colorGradingModeName[gradingIndex])
        {
            case "LDR": gradingMode = GradingMode.LowDefinitionRange; break;
            case "HDR": gradingMode = GradingMode.HighDefinitionRange; break;
            case "EXTERNAL": gradingMode = GradingMode.External; break;
        }
        colorGradingLayer.gradingMode.value = gradingMode;
        videoContentSubText[11].text = colorGradingModeName[gradingIndex];
    }
    public void ApplyColorGradingMode()
    {
        gradingIndex++;
        if (gradingIndex > 2) gradingIndex = 0;
        switch (colorGradingModeName[gradingIndex])
        {
            case "LDR": gradingMode = GradingMode.LowDefinitionRange; break;
            case "HDR": gradingMode = GradingMode.HighDefinitionRange; break;
            case "EXTERNAL": gradingMode = GradingMode.External; break;
        }
        colorGradingLayer.gradingMode.value = gradingMode;
        videoContentSubText[11].text = colorGradingModeName[gradingIndex];
    }
    public void SetColorGradingTone()
    {
        switch (colorGradingToneName[toneMapIndex])
        {
            case "NONE": tonemapper = Tonemapper.None; break;
            case "NEUTRAL": tonemapper = Tonemapper.Neutral; break;
            case "ACES": tonemapper = Tonemapper.ACES; break;
            case "CUSTOM": tonemapper = Tonemapper.Custom; break;
        }
        colorGradingLayer.tonemapper.value = tonemapper;
        videoContentSubText[12].text = colorGradingToneName[toneMapIndex];
    }
    public void ApplyColorGradingTone()
    {
        toneMapIndex++;
        if (toneMapIndex > 3) toneMapIndex = 0;
        switch (colorGradingToneName[toneMapIndex])
        {
            case "NONE": tonemapper = Tonemapper.None; break;
            case "NEUTRAL": tonemapper = Tonemapper.Neutral; break;
            case "ACES": tonemapper = Tonemapper.ACES; break;
            case "CUSTOM": tonemapper = Tonemapper.Custom; break;
        }
        colorGradingLayer.tonemapper.value = tonemapper;
        videoContentSubText[12].text = colorGradingToneName[toneMapIndex];
    }
    public void SetSaturation(float value)
    {
        videoContentSlider[5].fillAmount = Map(value, -100, 100, 0, 1);
        videoContentSubText[19].text = FormatValuesOnePercent(videoContentSlider[5].fillAmount * 100);
        colorGradingLayer.saturation.value = value;
    }
    public void ApplySaturation()
    {
        if (saturation.GetType() != typeof(int)) saturation = Mathf.Round(saturation);
        saturation += 5f;
        if (saturation > 100) saturation = -100f;
        saturation = Mathf.Clamp(saturation, -100, 100);
        videoContentSlider[5].fillAmount = Map(saturation, -100, 100, 0, 1);
        videoContentSubText[19].text = FormatValuesOnePercent(videoContentSlider[5].fillAmount * 100);
        colorGradingLayer.contrast.value = saturation;
    }
    public void SetContrast(float value)
    {
        videoContentSlider[6].fillAmount = Map(value, -100, 100, 0, 1);
        videoContentSubText[20].text = FormatValuesOnePercent(videoContentSlider[6].fillAmount * 100);
        colorGradingLayer.contrast.value = value;
    }
    public void ApplyContrast()
    {
        if (contrast.GetType() != typeof(int)) contrast = Mathf.Round(contrast);
        contrast += 5f;
        if (contrast > 100) contrast = -100f;
        contrast = Mathf.Clamp(contrast, -100, 100);
        videoContentSlider[6].fillAmount = Map(contrast, -100, 100, 0, 1);
        videoContentSubText[20].text = FormatValuesOnePercent(videoContentSlider[6].fillAmount * 100);
        colorGradingLayer.contrast.value = contrast;
    }
    public void SetLift(float value)
    {
        videoContentSlider[7].fillAmount = Map(value, -1, 1, 0, 1);
        videoContentSubText[21].text = FormatValuesPercent(videoContentSlider[7].fillAmount);
        colorGradingLayer.lift.value = new Vector4(1, 1, 1, value);
    }
    public void ApplyLift()
    {
        lift += 0.05f;
        if (lift > 1) lift = -1;
        lift = Mathf.Clamp(lift, -1, 1);
        videoContentSlider[7].fillAmount = Map(lift, -1, 1, 0, 1);
        videoContentSubText[21].text = FormatValuesPercent(videoContentSlider[7].fillAmount);
        colorGradingLayer.lift.value = new Vector4(1, 1, 1, lift);
    }
    public void SetGamma(float value)
    {
        videoContentSlider[8].fillAmount = Map(value, -1, 1, 0, 1);
        videoContentSubText[22].text = FormatValuesPercent(videoContentSlider[8].fillAmount);
        colorGradingLayer.gamma.value = new Vector4(1, 1, 1, value);
    }
    public void ApplyGamma()
    {
        gamma += 0.1f;
        if (gamma > 1f) gamma = -1;
        gamma = Mathf.Clamp(gamma, -1, 1);
        videoContentSlider[8].fillAmount = Map(gamma, -1, 1, 0, 1);
        videoContentSubText[22].text = FormatValuesPercent(videoContentSlider[8].fillAmount);
        colorGradingLayer.gamma.value = new Vector4(1, 1, 1, gamma);
    }
    public void SetGain(float value)
    {
        videoContentSlider[9].fillAmount = Map(value, -1, 1, 0, 1);
        videoContentSubText[23].text = FormatValuesPercent(videoContentSlider[9].fillAmount);
        colorGradingLayer.gain.value = new Vector4(1, 1, 1, value);
    }
    public void ApplyGain()
    {
        gain += 0.1f;
        if (gain > 1.05f) gain = -1;
        gain = Mathf.Clamp(gain, -1, 1.05f);
        videoContentSlider[9].fillAmount = Map(gain, -1, 1, 0, 1);
        videoContentSubText[23].text = FormatValuesPercent(videoContentSlider[9].fillAmount);
        colorGradingLayer.gain.value = new Vector4(1, 1, 1, gain);
    }
    public void SetBrightness(float value)
    {
        videoContentSlider[2].fillAmount = Map(value, 0, 10, 0, 1);
        videoContentSubText[13].text = FormatValuesOne(value);
        autoExposureLayer.keyValue.value = value;
    }
    public void ApplyBrightness()
    {
        if (brightness.GetType() != typeof(int)) brightness = Mathf.Round(brightness);
        brightness += 1;
        if (brightness > 10) brightness = 1f;
        brightness = Mathf.Clamp(brightness, 0.1f, 10);
        videoContentSubText[13].text = FormatValuesOne(brightness);
        videoContentSlider[2].fillAmount = Map(brightness, 0, 10, 0, 1);
        autoExposureLayer.keyValue.value = brightness;
    }
    public void SetGrain(float value)
    {
        videoContentSlider[3].fillAmount = Map(value, 0, 1, 0, 1);
        videoContentSubText[15].text = FormatValuesTen(value);
        grainLayer.intensity.value = value;
    }
    public void ApplyGrain()
    {
        grainIntensity += 0.1f;
        if (grainIntensity > 1.1f) grainIntensity = 0f;
        grainIntensity = Mathf.Clamp(grainIntensity, 0.0f, 1.1f);
        videoContentSubText[15].text = FormatValuesTen(grainIntensity);
        videoContentSlider[3].fillAmount = Map(grainIntensity, 0, 1f, 0, 1);
        grainLayer.intensity.value = grainIntensity;
    }
    public void SetVignette(float value)
    {
        videoContentSubText[17].text = FormatValuesTen(value);
        videoContentSlider[4].fillAmount = Map(value, 0, 1, 0, 1);
        vignetteLayer.intensity.value = value;
    }
    public void ApplyVignette()
    {
        vignetteIntensity += 0.1f;
        if (vignetteIntensity > 1.1f) vignetteIntensity = 0f;
        vignetteIntensity = Mathf.Clamp(vignetteIntensity, 0.0f, 1.1f);
        videoContentSubText[17].text = FormatValuesTen(vignetteIntensity);
        videoContentSlider[4].fillAmount = Map(vignetteIntensity, 0, 1, 0, 1);
        vignetteLayer.intensity.value = vignetteIntensity;
    }

    //====================================================================
    //=======================AudioMethods=================================
    //====================================================================
    public void SetAudioValue(int parameterIndex)
    {
        audioContentSubText[parameterIndex].text = FormatValuesPercent(volumeInterval[parameterIndex]);
        audioContentSlider[parameterIndex].fillAmount = volumeInterval[parameterIndex];
        SetVolume(volumeName[parameterIndex], volumeInterval[parameterIndex]);
    }
    public void SetVolume(string parameter, float value)
    {
        if (value == 0) value = 0.01f;
        audioMixer.SetFloat(parameter, (Mathf.Log10(value) * 30));
    }
    public void ApplyAudioValue(int parameterIndex)
    {
        volumeInterval[parameterIndex] = AdjustVolRounding(volumeInterval[parameterIndex]);
        volumeInterval[parameterIndex] += 0.01f;
        if (volumeInterval[parameterIndex] > 1) { volumeInterval[parameterIndex] = 0.01f; }
        audioContentSubText[parameterIndex].text = FormatValuesPercent(volumeInterval[parameterIndex]);
        audioContentSlider[parameterIndex].fillAmount = volumeInterval[parameterIndex];
        SetVolume(volumeName[parameterIndex], volumeInterval[parameterIndex]);
    }
    public float AdjustVolRounding(float value)
    {
        float vol = value * 100;
        if (vol.GetType() != typeof(int)) vol = RoundOff(Mathf.CeilToInt(vol), 1);
        vol = vol / 100;
        return vol;
    }
    public void SetSpeakerMode()
    {
        switch (speakerName[speakerIndex])
        {
            case "MONO": speakerMode = AudioSpeakerMode.Mono; break;
            case "STEREO": speakerMode = AudioSpeakerMode.Stereo; break;
            case "SURROUND": speakerMode = AudioSpeakerMode.Surround; break;
            case "SURROUND 5.1": speakerMode = AudioSpeakerMode.Mode5point1; break;
            case "SURROUND 7.1": speakerMode = AudioSpeakerMode.Mode7point1; break;
            case "PROLOGIC DTS": speakerMode = AudioSpeakerMode.Prologic; break;
        }
        if (audioConfig.speakerMode != speakerMode) audioConfig.speakerMode = speakerMode;
        audioContentSubText[5].text = speakerName[speakerIndex];
        SetApplyButtonActive(true);
    }
    public void ApplySpeakerMode()
    {
        speakerIndex++;
        if (speakerIndex > 5) speakerIndex = 0;
        switch (speakerName[speakerIndex])
        {
            case "MONO": speakerMode = AudioSpeakerMode.Mono; break;
            case "STEREO": speakerMode = AudioSpeakerMode.Stereo; break;
            case "SURROUND": speakerMode = AudioSpeakerMode.Surround; break;
            case "SURROUND 5.1": speakerMode = AudioSpeakerMode.Mode5point1; break;
            case "SURROUND 7.1": speakerMode = AudioSpeakerMode.Mode7point1; break;
            case "PROLOGIC DTS": speakerMode = AudioSpeakerMode.Prologic; break;
        }
        if (audioConfig.speakerMode != speakerMode) audioConfig.speakerMode = speakerMode;
        audioContentSubText[5].text = speakerName[speakerIndex];
        SetApplyButtonActive(true);
    }
    public void SetVirtualVoices()
    {
        virtualVoice = voiceInterval[virtualVoiceIndex];
        if (audioConfig.numVirtualVoices != virtualVoice) audioConfig.numVirtualVoices = virtualVoice; 
        audioContentSubText[6].text = ConvertIntToText(virtualVoice);
        SetApplyButtonActive(true);
    }
    public void ApplyVirtualVoices()
    {
        virtualVoiceIndex++;
        if (virtualVoiceIndex > voiceInterval.Length - 1) virtualVoiceIndex = 0;
        virtualVoice = voiceInterval[virtualVoiceIndex];
        if (audioConfig.numVirtualVoices != virtualVoice) audioConfig.numVirtualVoices = virtualVoice;
        audioContentSubText[6].text = ConvertIntToText(virtualVoice);
        SetApplyButtonActive(true);
    }
    public void SetRealVoices()
    {
        realVoice = voiceInterval[realVoiceIndex];
        if (audioConfig.numRealVoices != realVoice) audioConfig.numRealVoices = realVoice;
        audioContentSubText[7].text = ConvertIntToText(realVoice);
        SetApplyButtonActive(true);
    }
    public void ApplyRealVoices()
    {
        realVoiceIndex++;
        if (realVoiceIndex > voiceInterval.Length - 1) realVoiceIndex = 0;
        realVoice = voiceInterval[realVoiceIndex];
        if (audioConfig.numRealVoices != realVoice) audioConfig.numRealVoices = realVoice;
        audioContentSubText[7].text = ConvertIntToText(realVoice);
        SetApplyButtonActive(true);
    }
    public void SetSampleRate()
    {
        sampleRate = sampleInterval[sampleRateIndex];
        if (audioConfig.sampleRate != sampleRate) audioConfig.sampleRate = sampleRate; 
        audioContentSubText[8].text = ConvertIntToText(sampleRate);
        SetApplyButtonActive(true);
    }
    public void ApplySampleRate()
    {
        sampleRateIndex++;
        if (sampleRateIndex > sampleInterval.Length - 1) sampleRateIndex = 0;
        sampleRate = sampleInterval[sampleRateIndex];
        if (audioConfig.sampleRate != sampleRate) audioConfig.sampleRate = sampleRate;
        audioContentSubText[8].text = ConvertIntToText(sampleRate);
        SetApplyButtonActive(true);
    }
    public void SetDPSBuffer()
    {
        dpsBuffer = bufferInterval[dpsBufferIndex];
        if (audioConfig.dspBufferSize != dpsBuffer) audioConfig.dspBufferSize = dpsBuffer;
        audioContentSubText[9].text = ConvertIntToText(dpsBuffer);
        SetApplyButtonActive(true);
    }
    public void ApplyDPSBuffer()
    {
        dpsBufferIndex++;
        if (dpsBufferIndex > bufferInterval.Length - 1) dpsBufferIndex = 0;
        dpsBuffer = bufferInterval[dpsBufferIndex];
        if (audioConfig.dspBufferSize != dpsBuffer) audioConfig.dspBufferSize = dpsBuffer;
        audioContentSubText[9].text = ConvertIntToText(dpsBuffer);
        SetApplyButtonActive(true);
    }
    public void ApplyAudioConfiguration()
    {
        AudioSettings.Reset(audioConfig);
        if(audioSystem.isActiveAndEnabled && introSystem.isStartPressed)
            audioSystem.MusicPlayStop(true);
       
        if (!gameSystem.isGameStarted) 
        {
            mainMenuSystem.ResetVideo();
        }
        else playerSystem.ActivateLevelEnviromentSounds();
        SetApplyButtonActive(false);
        panelScrollBars[4].value = 1;
        b_Select = panelFirstSelected[4];
        b_Select.Select();
    }
    private void SetApplyButtonActive(bool active)
    {
        audioWindowContent[11].interactable = active;
    }
    private void AudioApplyButtonActive()
    {
        if (panelIndex != 4) return;
        if (!audioWindowContent[11].interactable) 
        { 
            if(applyText.color != Color.white)
                applyText.color = Color.white; 
            return; 
        }
        applyText.color = new Color(applyText.color.r, applyText.color.g, applyText.color.b, Mathf.Sin(Time.unscaledTime * 5) * 0.5f + 0.5f);

    }
    public void OpenQuitMenu(bool active)
    {
        if (!active)
        {
            for (int i = 0; i < quitWindowIElements.Length; i++) quitWindowIElements[i].color = new Color(quitWindowIElements[i].color.r, quitWindowIElements[i].color.g, quitWindowIElements[i].color.b, 0);
            for (int t = 0; t < quitWindowTElements.Length; t++) quitWindowTElements[t].color = new Color(quitWindowTElements[t].color.r, quitWindowTElements[t].color.g, quitWindowTElements[t].color.b, 0);
            quitWindow.SetActive(active);
            quitOpen = false;
            gameSystem.OpenMainSelection(true);
        }
        else
        {
            fileSelection.SetActive(false);
            gameSystem.OpenMainSelection(false);
            quitWindow.SetActive(active);
            FadeQuitMenu(active ? 1 : 0);
        }
       
    }
    public void SetMusicSoundtrack()
    {
        musicClip = musicTracks[soundtrackIndex];
        audioContentSubText[10].text = songName[soundtrackIndex];
        if (audioSystem.M_audioSrc.clip == musicClip) return;
        audioSystem.PlayGameMusic(musicClip, 1, 1, true);
    }
    public void ApplyMusicSoundTrack()
    {
        soundtrackIndex++;
        if (soundtrackIndex > musicTracks.Length - 1) soundtrackIndex = 0;
        musicClip = musicTracks[soundtrackIndex];
        audioContentSubText[10].text = songName[soundtrackIndex];
        audioSystem.PlayGameMusic(musicClip, 1, 1, true);
    }

    //===================================================================
    //======================OptionMethods================================
    //===================================================================
    public void EnableController()
    {
        if (!controllerEnabled)
        {
            if (inputPlayer.controllers.Joysticks.Count > 0) { inputPlayer.controllers.Joysticks[0].enabled = false; controlWindowContent[0].interactable = true; }
            else { controlWindowContent[0].interactable = false; controlContentSubText[0].text = "Not Connected"; controlContentSubText[5].text = "Disabled"; p_Select = selectionFirstSelected[0].GetComponent<Selectable>(); p_Select.Select(); }
            controlWindowContent[5].interactable = false;
            //inputPlayer.controllers.Mouse.enabled = true;
            //inputPlayer.controllers.Keyboard.enabled = true;
        }
        else
        {
            if (inputPlayer.controllers.Joysticks.Count > 0)
                inputPlayer.controllers.Joysticks[0].enabled = true;
            //inputPlayer.controllers.Mouse.enabled = false;
            //inputPlayer.controllers.Keyboard.enabled = false;
            controlWindowContent[0].interactable = true;
            controlWindowContent[5].interactable = true;
            controlContentSubText[0].text = ONOFFSubText(controllerEnabled);
            controlContentSubText[5].text = ONOFFSubText(vibration);
        }
    }
    private void OnControllerConnected(ControllerStatusChangedEventArgs args)
    {
        if (args.controllerType != ControllerType.Joystick) return;
        if (!controllerEnabled)
        {
            controllerEnabled = true;
            controllerIndex = 1;
            SetControllerActive();
        }
        ReInput.ControllerDisconnectedEvent += OnControllerDisconnected;
        ReInput.ControllerConnectedEvent -= OnControllerConnected;


    }
    private void OnControllerDisconnected(ControllerStatusChangedEventArgs args)
    {
        if (args.controllerType != ControllerType.Joystick) return;
        if (controllerEnabled)
        {
            controllerEnabled = false;
            controllerIndex = 0;
            SetControllerActive();
        }
        ReInput.ControllerConnectedEvent += OnControllerConnected;
        ReInput.ControllerDisconnectedEvent -= OnControllerDisconnected;

    }
    private void FadeSingle(Text[] tElements, Image[] iElements, bool fadeIn, float time)
    {
        if (!startFade || time == 0) return;
        canQuit = false;
        float speedAbsolute = 1.0f / time;  // speed desired by user
        float speedDirection = speedAbsolute * (fadeIn ? +1 : -1);  // + or -
        float deltaVolume = unScaledDTime * speedDirection;  // how much color changes in 1 frame
        fadePercentage += deltaVolume;  // implement change
        fadePercentage = Mathf.Clamp(fadePercentage, 0.0f, 1f);  // make sure you're in 0..100% 
        for (int i = 0; i < iElements.Length; i++) iElements[i].color = new Color(iElements[i].color.r, iElements[i].color.g, iElements[i].color.b, fadePercentage);
        for (int t = 0; t < tElements.Length; t++) tElements[t].color = new Color(tElements[t].color.r, tElements[t].color.g, tElements[t].color.b, fadePercentage);
        if (fadePercentage == 0.0f || fadePercentage == 1f) 
        { 
            canQuit = true; 
            startFade = false;
            if (fadePercentage == 1 && !quitOpen) gameSystem.mainmenuOpen = true;
            else if (fadePercentage == 1 && quitOpen) { p_Select = declineButton.GetComponent<Selectable>(); p_Select.Select(); }
            else if (fadePercentage == 0 && quitOpen)
                quitOpen = false;
        }
    }

    private void FadeMenu(Text[] tElements, Image[] iElements, bool fadeIn, float time)
    {
        for (int i = 0; i < iElements.Length; i++) iElements[i].color = new Color(iElements[i].color.r, iElements[i].color.g, iElements[i].color.b, fadeIn ? 0 : 1);
        for (int t = 0; t < tElements.Length; t++) tElements[t].color = new Color(tElements[t].color.r, tElements[t].color.g, tElements[t].color.b, fadeIn ? 0 : 1);
        fadePercentage = 0;
        startFade = fadeIn;
        textElements = tElements;
        imageElements = iElements;
        fadeTimeInSeconds = time;
    }
    public void FadeMainMenu(int fadeAlphaTo)
    {
        FadeMenu(mainWindowTElements, mainWindowIElements, fadeAlphaTo == 1 ? true : false, 0.5f);
    }
    public void FadeQuitMenu(int fadeAlphaTo)
    {
        quitOpen = true;
        gameSystem.mainmenuOpen = false;
        FadeMenu(quitWindowTElements, quitWindowIElements, fadeAlphaTo == 1 ? true : false, 0.5f);
    }

}
