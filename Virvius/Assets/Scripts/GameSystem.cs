using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using System;
using Rewired;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine.SceneManagement;
using System.Text;

public class GameSystem : MonoBehaviour
{
    private StringBuilder loadSb = new StringBuilder();
    private StringBuilder[] resultSb = new StringBuilder[3];
    public static GameSystem gameSystem;
    public static bool expandBulletPool = true;
    public bool isGameStarted = false;
    private OptionData optionData = new OptionData();
    [HideInInspector]
    public PlayerData playerData = new PlayerData();
    private BinaryFormatter _BinaryFormatter = new BinaryFormatter();
    [SerializeField]
    private AudioSystem audioSystem;
    [SerializeField]
    private PlayerSystem playerSystem;
    [SerializeField]
    private InputSystem inputSystem;
    [SerializeField]
    private MessageSystem messageSystem;
    [SerializeField]
    private WeaponSystem weaponSystem;
    [SerializeField]
    private PowerupSystem powerupSystem;
    [SerializeField]
    private EnvironmentSystem environmentSystem;
    [SerializeField]
    private MainMenuSystem mainMenuSystem;
    [SerializeField]
    private ButtonAnimate[] buttonAnimates = new ButtonAnimate[6];
    [SerializeField]
    private BobSystem[] bobSystem = new BobSystem[10];
    [SerializeField]
    private OptionsSystem optionsSystem;
    private Player inputPlayer;

    //[HideInInspector]
    public Vector3[] scenePositions = new Vector3[4]
    {
        new Vector3(0, 0, 0),
        new Vector3(0, 11, 0),
        new Vector3(0, 18,-245),
        new Vector3(0, -2, 0)
    };
    public Vector3 loadedPosition = Vector3.zero;
    public Quaternion loadedRotation = Quaternion.identity;
    //[HideInInspector]
    public Vector3[] sceneRotations = new Vector3[4]
    {
        new Vector3(0, 0, 0),
        new Vector3(0, 0, 0),
        new Vector3(0, 0, 0),
        new Vector3(0, 0, 0)
    };
    [HideInInspector]
    public bool loadFromFile = false;
    private AsyncOperation async = new AsyncOperation();
    private bool firstTimeBoot = false;
    [HideInInspector]
    public bool isPaused = false;
    [HideInInspector]
    public bool isLoading = false;
    [HideInInspector]
    public int sceneIndex = 0;
    private int curSceneIndex = 0;
    private string gamePath = "Data/";
    private string OPTION_FILE = "o_data";
    private string[] SAVE_FILE = new string[9]
    {
        "s0_data",
        "s1_data",
        "s2_data",
        "s3_data",
        "s4_data",
        "s5_data",
        "s6_data",
        "s7_data",
        "s8_data"
    };
    private string FILE_EXTENSION = ".vir";
    private string dataPath;
    [Header("Menu Assignment")]
    [SerializeField]
    private Selectable[] b_Select = new Selectable[6];
    private bool[] currentMenuButtonSelected = new bool[7] { true, false, false, false, false, false, false };
    [SerializeField]
    private AudioClip[] pauseOpenSfx = new AudioClip[2];
    [SerializeField]
    private GameObject optionSelection;
    [SerializeField]
    private GameObject mainUI;
    [SerializeField]
    private GameObject mainSelection;
    [SerializeField]
    private GameObject mainNavigation;
    [SerializeField]
    private GameObject[] fileSelection;
    [SerializeField]
    private GameObject gameUI;
    [SerializeField]
    private GameObject player;
    [SerializeField]
    private GameObject bootIntro;
    [SerializeField]
    private Image videoUI;
    [SerializeField]
    private VideoPlayer videoPlayer;
    [Space]
    [Header("Loading Menu")]
    [SerializeField]
    private GameObject loadingCamera;
    [SerializeField]
    private GameObject loadingMenu;
    [SerializeField]
    private Text loadingText;    
    [SerializeField]
    private Image loadingBar;
    [HideInInspector]
    public bool mainmenuOpen = false;
    private Transform defaultTransform;
    [Space]
    [Header("Results Screen")]
    [SerializeField]
    private GameObject resultsScreen;
    public int totalLevelSecrets = 0;
    public int totalLevelEnemies = 0;
    [HideInInspector]
    public float levelTime = 0;
    private bool levelActive = false;
    [HideInInspector]
    public int totalKills = 0;
    [HideInInspector]
    public int secretsFound = 0;
    [SerializeField]
    private Text gameTimeText;
    [SerializeField]
    private Text secretsAmtText;
    [SerializeField]
    private Text killsAmtText;
    [SerializeField]
    private Camera mainCamera;
    [SerializeField]
    private Camera clipCamera;
    [HideInInspector]
    public bool showResults = false;
    [SerializeField]
    private Transform resultCamera;
    [HideInInspector]
    public Vector3 resultCamPosition;
    [HideInInspector]
    public Quaternion resultCamRotation;
    [Space]
    [Header("Save Settings")]
    [SerializeField]
    private Image saveIcon;
    [SerializeField]
    private float saveTime = 5;
    private float saveTimer = 0;
    private bool isSaving;
    [HideInInspector]
    public bool[] ambushesActivated;
    [HideInInspector]
    public bool[] enemiesDeadGrunt;
    [HideInInspector]
    public bool[] enemiesDeadDin;
    [HideInInspector]
    public bool[] enemiesDeadElite;
    [HideInInspector]
    public bool[] switchesActivated;
    [HideInInspector]
    public bool[] pickupsObtained;
    [HideInInspector]
    public bool[] spawnersActivated;
    [HideInInspector]
    public bool[] messagesActivated;
    [HideInInspector]
    public bool[] cratesExploded;
    [HideInInspector]
    public bool[] explodingTriggersActivated;
    [HideInInspector]
    public bool[] doorTriggersActivated;

    public bool[] dropItem0Activated;

    public bool[] dropItem1Activated;

    public bool[] dropItem2Activated;
    [SerializeField]
    private Transform[] enemyDropPool = new Transform[3];
    [SerializeField]
    private GameObject[] dropPoolPrefabs = new GameObject[3];

 [Space]
    [Header("Pool Access")]
    private int[] bulletpoolAmt = new int[4] { 130, 70, 40, 10 };
    public Transform[] enemyBulletPools = new Transform[2];
    public GameObject[] bulletHolePrefabs = new GameObject[4];
    public Transform[] bulletHolePool = new Transform[4];
    public Transform enemyAmmoPool;
    public Transform[] enemyWeaponPools = new Transform[2];
 
    [HideInInspector]
    public bool fileMenuOpen = false;
    [SerializeField]
    private Button saveButtonActivation;
    [SerializeField]

    private void Awake()
    {
        if (gameSystem == null)
        {
            DontDestroyOnLoad(gameObject);
            gameSystem = this;
        }
        else if (gameSystem != this)
        {
            Destroy(gameObject);
        }
        dataPath = Application.dataPath + "/";
    }
    private void Start()
    {
        inputPlayer = ReInput.players.GetPlayer(0);
        defaultTransform = GameObject.Find("GameSystem/Game").transform;
        for(int sb = 0; sb < resultSb.Length; sb++) resultSb[sb] = new StringBuilder();
        //load normally
        GameMouseActive(false, CursorLockMode.Locked);
        ResetPools();
        //load debug
        //GameMouseActive(true, CursorLockMode.Confined);
    }
    private void Update()
    {
        LevelTime();
        LoadScene(sceneIndex);
        Pause();
        Saving();
    }
    private void Saving()
    {
        if (!isSaving) return;
        saveTimer -= Time.unscaledDeltaTime;
        saveTimer = Mathf.Clamp(saveTimer, 0, saveTime);
        if (saveTimer == 0)
        {
            saveIcon.enabled = false;
            isSaving = false;
        }
    }
    private void LevelTime()
    {
        if (!levelActive) return;
        levelTime += Time.unscaledDeltaTime;
    }
    public void ResetLevelStats()
    {
        if (levelTime != 0) levelTime = 0;
        if (totalKills > 0) totalKills = 0;
        if (secretsFound > 0) secretsFound = 0;
    }
    public void ShowResults(bool active)
    {
        showResults = active;
        resultsScreen.SetActive(active);
        SetLevelResultCamPosition();
        gameUI.SetActive(!active);
        for (int sb = 0; sb < resultSb.Length; sb++) if(resultSb[sb].Length > 0) resultSb[sb].Clear();
        resultSb[0].Append(optionsSystem.SetTime(levelTime));
        resultSb[1].Append(secretsFound + " / " + totalLevelSecrets);
        resultSb[2].Append(totalKills + " / " + totalLevelEnemies);
        gameTimeText.text = active ? resultSb[0].ToString() : null;
        secretsAmtText.text = active ? resultSb[1].ToString(): null; 
        killsAmtText.text = active ? resultSb[2].ToString(): null;
        mainCamera.enabled = !active;
        clipCamera.enabled = !active;
        audioSystem.MusicPlayStop(false);
    }
   
    private bool GameAttributesOpen()
    {
        if (isLoading) return true;
        if (CommandSystem.commandOpen) return true;
        if (!isGameStarted) return true;
        if (optionsSystem.quitOpen) return true;
        if (!optionsSystem.canQuit) return true;
        if (optionsSystem.optionsOpen) return true;
        if (optionsSystem.fileSelectionOpen) return true;
        return false;
    }
    private void Pause()
    {
        if (GameAttributesOpen()) return;
        if (inputPlayer.GetButtonDown("Start") && !isPaused || inputPlayer.GetButtonDown("B") && isPaused || inputPlayer.GetButtonDown("Start") && isPaused)
        {
            isPaused = !isPaused;
            int open = isPaused ? 1 : 0;
            GameMouseActive(isPaused, isPaused ? CursorLockMode.Confined : CursorLockMode.Locked);
            audioSystem.PlayAudioSource(pauseOpenSfx[open], 1, 1, 128);
            Time.timeScale = isPaused ? 0 : 1;
            OpenMainSelection(isPaused);
        }
    }
    public void OpenMainSelection(bool active)
    {
        optionsSystem.FadeMainMenu(active ? 1 : 0);
        mainUI.SetActive(active);
        mainSelection.SetActive(active);
        mainNavigation.SetActive(active);
        if (active) 
        {
            if (sceneIndex > 1) saveButtonActivation.interactable = true;
            else saveButtonActivation.interactable = false;
            SelectFileSelectable(0);
            SetButtonAnimates(0);
        }
    }
    public void SetButtonAnimates(int ID)
    {
        for (int b = 0; b < buttonAnimates.Length; b++)
        {
            if (b == ID) buttonAnimates[b].StartAnimation(true);
            else buttonAnimates[b].StartAnimation(false);
        }
    }
    public void SetCurrentlySelected(int index)
    {
        for(int s = 0; s < currentMenuButtonSelected.Length; s++)
        {
            if (index == s) currentMenuButtonSelected[s] = true;
            else currentMenuButtonSelected[s] = false;
        }
    }
    public void SetMasterStart()
    {
        if (firstTimeBoot) return;
        playerSystem.Start();
        environmentSystem.Start();
        weaponSystem.Start();
        inputSystem.Start();
        powerupSystem.Start();
        messageSystem.Start();
        for (int bs = 0; bs < bobSystem.Length; bs++)
        {
            bobSystem[bs].Start();
        }
        firstTimeBoot = true;
    }
    public void ManualPause()
    {
        isPaused = !isPaused;
        int open = isPaused ? 1 : 0;
        if (isPaused) optionsSystem.FadeMainMenu(1);
        optionsSystem.OpenFileSelection(false);
        GameMouseActive(isPaused, isPaused ? CursorLockMode.Confined : CursorLockMode.Locked);
        audioSystem.PlayAudioSource(pauseOpenSfx[open], 1, 1, 128);
        Time.timeScale = isPaused ? 0 : 1;
        mainUI.SetActive(isPaused);
        mainSelection.SetActive(isPaused);
        mainNavigation.SetActive(isPaused);
        if (isPaused) { SelectFileSelectable(0); SetButtonAnimates(0); }
        if (optionSelection.activeInHierarchy) optionsSystem.OpenOptions(false);
        optionsSystem.ApplyActiveInputUI();
    }
    public void LoadGame(int sceneParameter)
    {
        sceneIndex = sceneParameter;
        GameMouseActive(false, CursorLockMode.Locked);
        audioSystem.MusicPlayStop(false);
        audioSystem.EnvironmentPlayStop(false);
        audioSystem.VideoPlayStop(false);
        levelActive = false;
        if (!loadingMenu.activeInHierarchy) loadingMenu.SetActive(true);
        if (!loadingCamera.activeInHierarchy) loadingCamera.SetActive(true);
    }
    private void LoadScene(int sceneIndex)
    {
        if (!isLoading) return;
        if (curSceneIndex != sceneIndex)
        {
            Application.backgroundLoadingPriority = ThreadPriority.High;
            async = SceneManager.LoadSceneAsync(sceneIndex, LoadSceneMode.Single);
            async.allowSceneActivation = false;
            curSceneIndex = sceneIndex;
            ResetPools();
        }
        else
        {
           
            if (!async.isDone && async.progress != 1)
            {
                loadingBar.fillAmount = Map(Mathf.RoundToInt(async.progress * 100), 0, 100, 0, 1);
                loadSb.Append(Mathf.RoundToInt(async.progress * 100) + "/100");
                loadingText.text = loadSb.ToString();
                loadSb.Clear();
                if (async.progress >= 0.9f)
                {        
                    async.allowSceneActivation = true;
                    loadingBar.fillAmount = 100;
                    loadSb.Clear();
                    loadSb.Append("100/100");
                    loadingText.text = loadSb.ToString();
                }
            }
            else if(SceneManager.GetActiveScene().isLoaded) StartLevel();
        }
        
    }
    public void ResetPools()
    {
        for(int p = 0; p < enemyBulletPools.Length; p++)
        {
            for (int c = 0; c < enemyBulletPools[p].childCount; c++) 
            {
                if (enemyBulletPools[p].GetChild(c).gameObject.activeInHierarchy)
                    enemyBulletPools[p].GetChild(c).gameObject.SetActive(false);
            }

        }
        for (int p = 0; p < enemyWeaponPools.Length; p++)
        {
            for (int c = 0; c < enemyWeaponPools[p].childCount; c++)
            {
                if (enemyWeaponPools[p].GetChild(c).gameObject.activeInHierarchy)
                    enemyWeaponPools[p].GetChild(c).gameObject.SetActive(false);
            }

        }
        for (int c = 0; c < enemyAmmoPool.childCount; c++)
        {
            if (enemyAmmoPool.GetChild(c).gameObject.activeInHierarchy)
                enemyAmmoPool.GetChild(c).gameObject.SetActive(false);
        }

        int[] poolAmt = new int[4]
        {
            bulletHolePool[0].childCount,
            bulletHolePool[1].childCount,
            bulletHolePool[2].childCount,
            bulletHolePool[3].childCount
        };
        for (int c = 0; c < bulletHolePool.Length; c++)
        {
            if(poolAmt[c]  != bulletpoolAmt[c])
            {
                int totalholes = bulletpoolAmt[c] - poolAmt[c];
                for(int h = 0; h < totalholes; h++)
                    Instantiate(bulletHolePrefabs[c], bulletHolePool[c]);
            }
        }
    }
    private void StartLevel()
    {
        // Shut off Loading Camera
        if (loadingCamera.activeInHierarchy) loadingCamera.SetActive(false);
        // Shut off Loading Screen
        if (loadingMenu.activeInHierarchy) loadingMenu.SetActive(false);
        // Turn on Game UI
        gameUI.SetActive(true);
 
        // Loading is now finished
        if (isPaused)
        {
            isPaused = !isPaused;
            GameMouseActive(isPaused, isPaused ? CursorLockMode.Confined : CursorLockMode.Locked);
            Time.timeScale = isPaused ? 0 : 1;
        }

        if (sceneIndex > 1) 
        { 
            player.SetActive(true); 
            playerSystem.SetupPlayer(false); 
        }
        else 
        { 
            SetMasterStart();
            player.SetActive(true);
            playerSystem.SetupPlayer(true);
        }
        isLoading = false;
        isGameStarted = true;
        levelActive = true;
        mainMenuSystem.FadeBlackScreen(false);
    }
    public void SetNewGame()
    {
        isLoading = true;
        if(playerSystem.transform.parent != defaultTransform)
            playerSystem.transform.parent = defaultTransform;
        // Shut off LoadSelection
        if (optionsSystem.fileSelectionOpen) optionsSystem.OpenFileSelection(false);
        // Shut off BootIntro
        if (bootIntro.activeInHierarchy) bootIntro.SetActive(false);
        // Shut off GameUI
        if (gameUI.activeInHierarchy) gameUI.SetActive(false);
        // Shut off Player object
        if (player.activeInHierarchy) player.SetActive(false);  
        // Shut off Player object
        if (mainSelection.activeInHierarchy) mainSelection.SetActive(false);
        // Shut off Video UI
        if (videoUI.enabled) videoUI.enabled = false;
        // Shut off Video Player
        if (videoPlayer.isPlaying) videoPlayer.Stop();
        if(mainmenuOpen) mainmenuOpen = false;
        // Shut off NavigationUI
        if (mainNavigation.activeInHierarchy) mainNavigation.SetActive(false);
        // Load the first Scene
        curSceneIndex = 0;
        loadingBar.fillAmount = 0;
        if (loadSb.Length > 0) loadSb.Clear();
        LoadGame(1);
    }
    public void SetNewLevel(int sceneID)
    {
        isLoading = true;
        if (playerSystem.transform.parent != defaultTransform)
            playerSystem.transform.parent = defaultTransform;
        // Shut off LoadSelection
        if (optionsSystem.fileSelectionOpen) optionsSystem.OpenFileSelection(false);
        // Shut off BootIntro
        if (bootIntro.activeInHierarchy) bootIntro.SetActive(false);
        // Shut off GameUI
        if (gameUI.activeInHierarchy) gameUI.SetActive(false);
        // Shut off Player object
        if (player.activeInHierarchy) player.SetActive(false);
        // Shut off Player object
        if (mainSelection.activeInHierarchy) mainSelection.SetActive(false);
        // Shut off Video UI
        if (videoUI.enabled) videoUI.enabled = false;
        // Shut off Video Player
        if (videoPlayer.isPlaying) videoPlayer.Stop();
        if (mainmenuOpen) mainmenuOpen = false;
        // Shut off NavigationUI
        if (mainNavigation.activeInHierarchy) mainNavigation.SetActive(false);
        // Load the first Scene
        curSceneIndex = 0;
        loadingBar.fillAmount = 0;
        if (loadSb.Length > 0) loadSb.Clear();
        LoadGame(sceneID);
    }
    public bool BlockedAttributesActive()
    {
        if (!isGameStarted) return true;
        else if (isPaused) return true;
        else if (isLoading) return true;
        else if (showResults) return true;
        else if (CommandSystem.commandOpen) return true;
        else return false;
    }
    private float Map(float value, float inMin, float inMax, float outMin, float outMax)
    {
        return (value - inMin) * (outMax - outMin) / (inMax - inMin) + outMin;
    }
    public void SelectMenuSelectable()
    {
        for (int s = 0; s < b_Select.Length; s++)
        {
            if (b_Select[s].interactable)
            {
                if (currentMenuButtonSelected[s]) { b_Select[s].Select(); break;}
            }
        }
    }
    public void SelectFileSelectable(int currID)
    {
        for (int s = 0; s < b_Select.Length; s++)
        {
            if (s == currID) { currentMenuButtonSelected[s] = true; b_Select[s].Select(); }
            else currentMenuButtonSelected[s] = false;
        }
    }
    public void GameMouseActive(bool active, CursorLockMode lockMode)
    {
        Cursor.lockState = lockMode;
        Cursor.visible = active;
    }
    public bool LoadOptionData()
    {
        if (File.Exists(dataPath + gamePath + OPTION_FILE + FILE_EXTENSION))
        {
            Stream stream = File.Open(dataPath + gamePath + OPTION_FILE + FILE_EXTENSION, System.IO.FileMode.Open);
            optionData = (OptionData)_BinaryFormatter.Deserialize(stream);
            stream.Close();
            //------------------------------------------------------------------------
            //GAME SETTINGS--------------------------------------------------------
            //------------------------------------------------------------------------
            optionsSystem.difficultyIndex = optionData.difficultyIndex;
            optionsSystem.autoAIndex = optionData.autoAIndex;
            optionsSystem.autoSIndex = optionData.autoSIndex;
            optionsSystem.cameraBIndex = optionData.cameraBIndex;
            optionsSystem.handIndex = optionData.handIndex;
            optionsSystem.switchNewIndex = optionData.switchNewIndex;
            optionsSystem.switchEmptyIndex = optionData.switchEmptyIndex;
            optionsSystem.weaponMoveIndex = optionData.weaponBIndex;
            //------------------------------------------------------------------------
            //CONTROL SETTINGS--------------------------------------------------------
            //------------------------------------------------------------------------
            optionsSystem.controllerIndex = optionData.controllerIndex;
            optionsSystem.invertIndex = optionData.invertIndex;
            optionsSystem.sensitivity = optionData.sensitivity;
            optionsSystem.smoothIndex = optionData.smoothIndex;
            optionsSystem.vibrationIndex = optionData.vibrationIndex;
            optionsSystem.runIndex = optionData.runIndex;
            optionsSystem.airIndex = optionData.airIndex;
            optionsSystem.lockYIndex = optionData.lockYIndex;
            //------------------------------------------------------------------------
            //UI SETTINGS-------------------------------------------------------------
            //------------------------------------------------------------------------
            optionsSystem.messagesActiveIndex = optionData.messagesActiveIndex;
            optionsSystem.timeActiveIndex = optionData.timeActiveIndex;
            optionsSystem.fpsActiveIndex = optionData.fpsActiveIndex;
            optionsSystem.flashIndex = optionData.flashIndex;
            optionsSystem.hudIndex = optionData.hudIndex;
            optionsSystem.visorColorIndex = optionData.visorColorIndex;
            optionsSystem.crosshairIndex = optionData.crosshairIndex;
            optionsSystem.environmentIndex = optionData.environmentIndex;
            //------------------------------------------------------------------------
            //VIDEO SETTINGS----------------------------------------------------------
            //------------------------------------------------------------------------
            optionsSystem.fullScreenIndex = optionData.fullScreenIndex;
            optionsSystem.qualityIndex = optionData.qualityIndex;
            optionsSystem.resolutionIndex = optionData.resolutionIndex;
            optionsSystem.width = optionData.width;
            optionsSystem.height = optionData.height;
            optionsSystem.refreshRate = optionData.refreshRate;
            optionsSystem.vSyncIndex = optionData.vSyncIndex;
            optionsSystem.fieldOfView = optionData.fieldOfView;
            optionsSystem.brightness = optionData.brightness;
            optionsSystem.postEffectIndex = new int[8];
            optionsSystem.postEffectIndex = optionData.postEffectIndex;
            optionsSystem.gradingIndex = optionData.gradingIndex;
            optionsSystem.toneMapIndex = optionData.toneMapIndex;
            optionsSystem.saturation = optionData.saturation;
            optionsSystem.contrast = optionData.contrast;
            optionsSystem.lift = optionData.lift;
            optionsSystem.gamma = optionData.gamma;
            optionsSystem.gain = optionData.gain;
            optionsSystem.grainIntensity = optionData.grainIntensity;
            optionsSystem.vignetteIntensity = optionData.vignetteIntensity;
            //------------------------------------------------------------------------
            //AUDIO SETTINGS----------------------------------------------------------
            //------------------------------------------------------------------------
            optionsSystem.volumeInterval = new float[5] 
            { 
                optionData.volumeInterval[0], 
                optionData.volumeInterval[1], 
                optionData.volumeInterval[2], 
                optionData.volumeInterval[3], 
                optionData.volumeInterval[4], 
            };
            //optionsSystem.volumeInterval = optionData.volumeInterval;
            optionsSystem.virtualVoiceIndex = optionData.virtualVoiceIndex;
            optionsSystem.realVoiceIndex = optionData.realVoiceIndex;
            optionsSystem.sampleRateIndex = optionData.sampleRateIndex;
            optionsSystem.dpsBufferIndex = optionData.dpsBufferIndex;
            optionsSystem.speakerIndex = optionData.speakerIndex;
            optionsSystem.soundtrackIndex = optionData.soundtrackIndex;
            return true;
        }
        else return false;
    }
    public void SaveOptionData()
    {
        if (!Directory.Exists(dataPath + gamePath))
            Directory.CreateDirectory(dataPath + gamePath);
        Stream stream = File.Create(dataPath + gamePath + OPTION_FILE + FILE_EXTENSION);
        //------------------------------------------------------------------------
        //GAME SETTINGS--------------------------------------------------------
        //------------------------------------------------------------------------
        optionData.difficultyIndex = optionsSystem.difficultyIndex;
        optionData.autoAIndex = optionsSystem.autoAIndex;
        optionData.autoSIndex = optionsSystem.autoSIndex;
        optionData.cameraBIndex = optionsSystem.cameraBIndex;
        optionData.handIndex = optionsSystem.handIndex;
        optionData.switchNewIndex = optionsSystem.switchNewIndex;
        optionData.switchEmptyIndex = optionsSystem.switchEmptyIndex;
        optionData.weaponBIndex = optionsSystem.weaponMoveIndex;
        //------------------------------------------------------------------------
        //CONTROL SETTINGS--------------------------------------------------------
        //------------------------------------------------------------------------
        optionData.controllerIndex = optionsSystem.controllerIndex;
        optionData.invertIndex = optionsSystem.invertIndex;
        optionData.sensitivity = optionsSystem.sensitivity;
        optionData.smoothIndex = optionsSystem.smoothIndex;
        optionData.vibrationIndex = optionsSystem.vibrationIndex;
        optionData.runIndex = optionsSystem.runIndex;
        optionData.airIndex = optionsSystem.airIndex;
        optionData.lockYIndex = optionsSystem.lockYIndex;
        //------------------------------------------------------------------------
        //UI SETTINGS-------------------------------------------------------------
        //------------------------------------------------------------------------
        optionData.messagesActiveIndex = optionsSystem.messagesActiveIndex;
        optionData.timeActiveIndex = optionsSystem.timeActiveIndex;
        optionData.fpsActiveIndex = optionsSystem.fpsActiveIndex;
        optionData.flashIndex = optionsSystem.flashIndex;
        optionData.hudIndex = optionsSystem.hudIndex;
        optionData.visorColorIndex = optionsSystem.visorColorIndex;
        optionData.crosshairIndex = optionsSystem.crosshairIndex;
        optionData.environmentIndex = optionsSystem.environmentIndex;
        //------------------------------------------------------------------------
        //VIDEO SETTINGS----------------------------------------------------------
        //------------------------------------------------------------------------
        optionData.fullScreenIndex = optionsSystem.fullScreenIndex;
        optionData.qualityIndex = optionsSystem.qualityIndex;
        optionData.resolutionIndex = optionsSystem.resolutionIndex;
        optionData.width = optionsSystem.width;
        optionData.height = optionsSystem.height;
        optionData.refreshRate = optionsSystem.refreshRate;
        optionData.vSyncIndex = optionsSystem.vSyncIndex;
        optionData.fieldOfView = optionsSystem.fieldOfView;
        optionData.brightness = optionsSystem.brightness;
        optionData.postEffectIndex = optionsSystem.postEffectIndex;
        optionData.bloomIntensity = optionsSystem.bloomIntensity;
        optionData.gradingIndex = optionsSystem.gradingIndex;
        optionData.toneMapIndex = optionsSystem.toneMapIndex;
        optionData.saturation = optionsSystem.saturation;
        optionData.contrast = optionsSystem.contrast;
        optionData.lift = optionsSystem.lift;
        optionData.gamma = optionsSystem.gamma;
        optionData.gain = optionsSystem.gain;
        optionData.grainIntensity = optionsSystem.grainIntensity;
        optionData.vignetteIntensity = optionsSystem.vignetteIntensity;
        //------------------------------------------------------------------------
        //AUDIO SETTINGS----------------------------------------------------------
        //------------------------------------------------------------------------
        optionData.volumeInterval = optionsSystem.volumeInterval;
        optionData.speakerIndex = optionsSystem.speakerIndex;
        optionData.virtualVoiceIndex = optionsSystem.virtualVoiceIndex;
        optionData.realVoiceIndex = optionsSystem.realVoiceIndex;
        optionData.sampleRateIndex = optionsSystem.sampleRateIndex;
        optionData.dpsBufferIndex = optionsSystem.dpsBufferIndex;
        optionData.soundtrackIndex = optionsSystem.soundtrackIndex;

        _BinaryFormatter.Serialize(stream, optionData);
        stream.Close();
    }

    public void LoadData(int slotID)
    {
        if (!File.Exists(dataPath + gamePath + SAVE_FILE[slotID] + FILE_EXTENSION)) return;
        Stream stream = File.Open(dataPath + gamePath + SAVE_FILE[slotID] + FILE_EXTENSION, FileMode.Open);
        playerData = (PlayerData)_BinaryFormatter.Deserialize(stream);
        stream.Close();

        loadFromFile = true;
        ResetLevelStats();
     
        //------------------------------------------------------------------------
        //PLAYER SETTINGS---------------------------------------------------------
        //------------------------------------------------------------------------
       
        loadedPosition = playerData.playerPosition;
        loadedRotation = playerData.playerRotation;
    
        //------------------------------------------------------------------------
        //GAME SETTINGS-----------------------------------------------------------
        //------------------------------------------------------------------------

        sceneIndex = playerData.sceneIndex;
        levelTime = playerData.levelTime;

        ambushesActivated = new bool[playerData.ambushesActivated.Length];
        for (int a = 0; a < ambushesActivated.Length; a++)
            ambushesActivated[a] = playerData.ambushesActivated[a];

        enemiesDeadGrunt = new bool[playerData.enemiesDeadGrunt.Length];
        for (int a = 0; a < enemiesDeadGrunt.Length; a++)
            enemiesDeadGrunt[a] = playerData.enemiesDeadGrunt[a];

        enemiesDeadDin = new bool[playerData.enemiesDeadDin.Length];
        for (int a = 0; a < enemiesDeadDin.Length; a++)
            enemiesDeadDin[a] = playerData.enemiesDeadDin[a];

        enemiesDeadElite = new bool[playerData.enemiesDeadElite.Length];
        for (int a = 0; a < enemiesDeadElite.Length; a++)
            enemiesDeadElite[a] = playerData.enemiesDeadElite[a];

        switchesActivated = new bool[playerData.switchesActivated.Length];
        for (int a = 0; a < switchesActivated.Length; a++)
            switchesActivated[a] = playerData.switchesActivated[a];

        pickupsObtained = new bool[playerData.pickupsObtained.Length];
        for (int a = 0; a < pickupsObtained.Length; a++)
            pickupsObtained[a] = playerData.pickupsObtained[a];

        spawnersActivated = new bool[playerData.spawnersActivated.Length];
        for (int a = 0; a < spawnersActivated.Length; a++)
            spawnersActivated[a] = playerData.spawnersActivated[a];

        messagesActivated = new bool[playerData.messagesActivated.Length];
        for (int a = 0; a < messagesActivated.Length; a++)
            messagesActivated[a] = playerData.messagesActivated[a];

        cratesExploded = new bool[playerData.cratesExploded.Length];
        for (int a = 0; a < cratesExploded.Length; a++)
            cratesExploded[a] = playerData.cratesExploded[a];

        explodingTriggersActivated = new bool[playerData.explodingTriggersActivated.Length];
        for (int a = 0; a < explodingTriggersActivated.Length; a++)
            explodingTriggersActivated[a] = playerData.explodingTriggersActivated[a];

        doorTriggersActivated = new bool[playerData.doorTriggersActivated.Length];
        for (int a = 0; a < doorTriggersActivated.Length; a++)
            doorTriggersActivated[a] = playerData.doorTriggersActivated[a];

        ambushesActivated = playerData.ambushesActivated;
        enemiesDeadGrunt = playerData.enemiesDeadGrunt;
        enemiesDeadDin = playerData.enemiesDeadDin;
        enemiesDeadElite = playerData.enemiesDeadElite;
        pickupsObtained = playerData.pickupsObtained;
        switchesActivated = playerData.switchesActivated;
        spawnersActivated = playerData.spawnersActivated;
        messagesActivated = playerData.messagesActivated;
        cratesExploded = playerData.cratesExploded;
        explodingTriggersActivated = playerData.explodingTriggersActivated;
        doorTriggersActivated = playerData.doorTriggersActivated;

        //---------------------------------------------------------------------------------------------
        //SET THE TOTAL AMOUNT OF OBJECTS IN POOL TO MATCH PREVIOUS POOL AMOUNT & ACTIVATE THEM + SET POSITIONS
        //---------------------------------------------------------------------------------------------

        dropItem0Activated = new bool[playerData.dropItem0Activated.Length];
        dropItem1Activated = new bool[playerData.dropItem1Activated.Length];
        dropItem2Activated = new bool[playerData.dropItem2Activated.Length];

        dropItem0Activated = playerData.dropItem0Activated;
        dropItem1Activated = playerData.dropItem1Activated;
        dropItem2Activated = playerData.dropItem2Activated;

        SetNewLevel(sceneIndex);
    }
    public void SetPreviouslyDroppedItems()
    {
        if (enemyDropPool[0].childCount != playerData.dropPoolChildCount[0])
        {
            int totalChildCount = playerData.dropPoolChildCount[0] - enemyDropPool[0].childCount;
            for (int e = 0; e < totalChildCount; e++)
                Instantiate(dropPoolPrefabs[0], enemyDropPool[0]);
        }
        if (enemyDropPool[1].childCount != playerData.dropPoolChildCount[1])
        {
            int totalChildCount = playerData.dropPoolChildCount[1] - enemyDropPool[1].childCount;
            for (int e = 0; e < totalChildCount; e++)
                Instantiate(dropPoolPrefabs[1], enemyDropPool[1]);
        }
        if (enemyDropPool[2].childCount != playerData.dropPoolChildCount[2])
        {
            int totalChildCount = playerData.dropPoolChildCount[2] - enemyDropPool[2].childCount;
            for (int e = 0; e < totalChildCount; e++)
                Instantiate(dropPoolPrefabs[2], enemyDropPool[2]);
        }

        for(int a = 0; a < dropItem0Activated.Length; a++)
        {
            enemyDropPool[0].GetChild(a).transform.position = playerData.enemyDrop0Positions[a];
            enemyDropPool[0].GetChild(a).gameObject.SetActive(dropItem0Activated[a]);
        }
        for (int a = 0; a < dropItem1Activated.Length; a++)
        {
            enemyDropPool[1].GetChild(a).transform.position = playerData.enemyDrop1Positions[a];
            enemyDropPool[1].GetChild(a).gameObject.SetActive(dropItem1Activated[a]);
        }
        for (int a = 0; a < dropItem2Activated.Length; a++)
        {
            enemyDropPool[2].GetChild(a).transform.position = playerData.enemyDrop2Positions[a];
            enemyDropPool[2].GetChild(a).gameObject.SetActive(dropItem2Activated[a]);
        }
    }
    public void SetPreviousWeaponStats()
    {
        weaponSystem.weaponAmmo = playerData.weaponAmmo;
        weaponSystem.weaponEquipped = playerData.weaponEquipped;
        weaponSystem.weaponObtained = playerData.weaponObtained;
        weaponSystem.weaponIndex = playerData.weaponIndex;
        weaponSystem.ApplyAmmo();
        weaponSystem.WeaponSetup(weaponSystem.weaponList[weaponSystem.weaponIndex]);
    }
    public void SetPreviousPlayerStats()
    {
        playerSystem.health = playerData.health;
        playerSystem.armor = playerData.armor;
        playerSystem.maxArmor = playerData.maxArmor;
        playerSystem.maxHealth = playerData.maxHealth;
        playerSystem.ApplyPlayerHealthAndArmor();
        playerSystem.keyCards = new bool[3]
        {
                playerData.keyCards[0],
                playerData.keyCards[1],
                playerData.keyCards[2]
        };
        for (int k = 0; k < playerSystem.keyCards.Length; k++)
        {
            if (playerSystem.keyCards[k])
                playerSystem.SetActiveKey(k, playerSystem.keyCards[k]);
        }
    }
    public void SaveData(int slotID)
    {
        if (!Directory.Exists(dataPath + gamePath))
            Directory.CreateDirectory(dataPath + gamePath);
        Stream stream = File.Create(dataPath + gamePath + SAVE_FILE[slotID] + FILE_EXTENSION);
        //===========================================================================================================================================
        //WEAPON SETTINGS============================================================================================================================
        //===========================================================================================================================================

        //-------------------------------------------------------------------------------------------------------------------------------------------
        //STORE ALL CURRENT WEAPON AMMO
        //-------------------------------------------------------------------------------------------------------------------------------------------
        playerData.weaponAmmo = new int[weaponSystem.weaponAmmo.Length];
        playerData.weaponAmmo = weaponSystem.weaponAmmo;
        //-------------------------------------------------------------------------------------------------------------------------------------------
        //STORE THE WHICH WEAPON IS EQUIPPED
        //-------------------------------------------------------------------------------------------------------------------------------------------
        playerData.weaponEquipped = new bool[weaponSystem.weaponEquipped.Length];
        playerData.weaponEquipped = weaponSystem.weaponEquipped;
        //-------------------------------------------------------------------------------------------------------------------------------------------
        //STORE ALL CURRENT OBTAINED WEAPONS
        //-------------------------------------------------------------------------------------------------------------------------------------------
        playerData.weaponObtained = new bool[weaponSystem.weaponObtained.Length];
        playerData.weaponObtained = weaponSystem.weaponObtained;
        //-------------------------------------------------------------------------------------------------------------------------------------------
        //STORE ALL CURRENT WEAPON INDEX NUMBER
        //-------------------------------------------------------------------------------------------------------------------------------------------
        playerData.weaponIndex = weaponSystem.weaponIndex;

        //-------------------------------------------------------------------------------------------------------------------------------------------
        //STORE ALL CURRENT PLAYER ATTRIBUTES & POSITION
        //-------------------------------------------------------------------------------------------------------------------------------------------
        playerData.health = playerSystem.health;
        playerData.armor = playerSystem.armor;
        playerData.maxArmor = playerSystem.maxArmor;
        playerData.maxHealth = playerSystem.maxHealth;
        playerData.keyCards = new bool[3]
        {
                playerSystem.keyCards[0],
                playerSystem.keyCards[1],
                playerSystem.keyCards[2]
        };
        playerData.playerPosition = playerSystem.transform.position;
        playerData.playerRotation = playerSystem.transform.rotation;

        //===========================================================================================================================================
        //GAME SETTINGS==============================================================================================================================
        //===========================================================================================================================================

        //-------------------------------------------------------------------------------------------------------------------------------------------
        //STORE CURRENT SCENE
        //-------------------------------------------------------------------------------------------------------------------------------------------
        playerData.sceneIndex = sceneIndex;
        //-------------------------------------------------------------------------------------------------------------------------------------------
        //SET THE DIFFICULTY NAME FOR SAVEFILE
        //-------------------------------------------------------------------------------------------------------------------------------------------
        switch (optionsSystem.difficultyIndex)
        {
            case 0: playerData.difficultyName = " [Easy] "; break;
            case 1: playerData.difficultyName = " [Normal] "; break;
            case 2: playerData.difficultyName = " [Hard] "; break;
            case 3: playerData.difficultyName = " [Demonic] "; break;
        }
        //-------------------------------------------------------------------------------------------------------------------------------------------
        //SET THE EPISODE NAME FOR SAVEFILE
        //-------------------------------------------------------------------------------------------------------------------------------------------
        if (sceneIndex > 2 && sceneIndex < 11) playerData.episodeName = " Episode 1: Fallen Foundry ";
        else if (sceneIndex > 10 && sceneIndex < 21) playerData.episodeName = " Episode 2: Torture Sanctum ";
        else if (sceneIndex > 20 && sceneIndex < 31) playerData.episodeName = " Episode 3: Command Headquarters ";
        else if (sceneIndex > 30 && sceneIndex < 41) playerData.episodeName = " Episode 4: Dark Abyss ";
        else playerData.episodeName = " Episode 0: Development Tech Demo ";
        //-------------------------------------------------------------------------------------------------------------------------------------------
        //SET THE SCENE NAME FOR SAVEFILE
        //-------------------------------------------------------------------------------------------------------------------------------------------
        switch (sceneIndex)
        {
            case 2: playerData.levelName = "- Level ?: Fallen Scourge"; break;
            case 3: playerData.levelName = "- Level 1: Virulent Vault"; break;
        }
        //-------------------------------------------------------------------------------------------------------------------------------------------
        //STORE THE CURRENT LEVEL TIME FOR SAVEFILE
        //-------------------------------------------------------------------------------------------------------------------------------------------
        playerData.gameTime = optionsSystem.SetTime(levelTime);
        playerData.levelTime = levelTime;
        LevelSystem levelSystem = playerSystem.AccessLevel();
        if (levelSystem == null) { Debug.LogError("Level System Not Accessed."); return; }
        //-------------------------------------------------------------------------------------------------------------------------------------------
        //GRAB TOTAL LEVEL ASSETS STATUSES FOR SAVEFILE
        //-------------------------------------------------------------------------------------------------------------------------------------------
        levelSystem.SaveLevel();
        //-------------------------------------------------------------------------------------------------------------------------------------------
        //STORE POSITIONS & ROTATIONS OF THE ENEMIES-------------------------------------------------------------------------------------------------
        //-------------------------------------------------------------------------------------------------------------------------------------------
        //Grunt>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
        playerData.enemyGPosition = new SerializableVector3[levelSystem.enemiesPositionGrunt.Length];
        playerData.enemyGRotation = new SerializableQuaternion[levelSystem.enemiesPositionGrunt.Length];
        for (int e = 0; e < levelSystem.enemiesPositionGrunt.Length; e++)
        {
            playerData.enemyGPosition[e] = levelSystem.enemiesPositionGrunt[e];
            playerData.enemyGRotation[e] = levelSystem.enemiesRotationGrunt[e];
        }
        //Din>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
        playerData.enemyDPosition = new SerializableVector3[levelSystem.enemiesPositionDin.Length];
        playerData.enemyDRotation = new SerializableQuaternion[levelSystem.enemiesPositionDin.Length];
        for (int e = 0; e < levelSystem.enemiesPositionDin.Length; e++)
        {
            playerData.enemyDPosition[e] = levelSystem.enemiesPositionDin[e];
            playerData.enemyDRotation[e] = levelSystem.enemiesRotationDin[e];
        }
        //Elite>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
        playerData.enemyEPosition = new SerializableVector3[levelSystem.enemiesPositionElite.Length];
        playerData.enemyERotation = new SerializableQuaternion[levelSystem.enemiesPositionElite.Length];
        for (int e = 0; e < levelSystem.enemiesPositionElite.Length; e++)
        {
            playerData.enemyEPosition[e] = levelSystem.enemiesPositionElite[e];
            playerData.enemyERotation[e] = levelSystem.enemiesRotationElite[e];
        }
        //-------------------------------------------------------------------------------------------------------------------------------------------
        //STORE WHAT LEVEL ITEMS WERE ALREADY ACTIVATE/OBTAINED/KILLED BY THE PLAYER
        //-------------------------------------------------------------------------------------------------------------------------------------------
        playerData.ambushesActivated = levelSystem.ambushesActivated;
        playerData.enemiesDeadGrunt = levelSystem.enemiesDeadGrunt;
        playerData.enemiesDeadDin = levelSystem.enemiesDeadDin;
        playerData.enemiesDeadElite = levelSystem.enemiesDeadElite;
        playerData.switchesActivated = levelSystem.switchesActivated;
        playerData.pickupsObtained = levelSystem.pickupsObtained;
        playerData.spawnersActivated = levelSystem.spawnersActivated;
        playerData.messagesActivated = levelSystem.messagesActivated;
        playerData.cratesExploded = levelSystem.cratesExploded;
        playerData.explodingTriggersActivated = levelSystem.explodingTriggersActivated;
        playerData.doorTriggersActivated = levelSystem.doorTriggersActivated;
        //-------------------------------------------------------------------------------------------------------------------------------------------
        //STORE THE ITEM DROP POOL TOTAL NUMBER OF DROPPED OBJECTS AND THEIR POSITIONS-----------------------------------------------------------------------
        //-------------------------------------------------------------------------------------------------------------------------------------------
        playerData.dropPoolChildCount = new int[3]
        {
            enemyDropPool[0].childCount,
            enemyDropPool[1].childCount,
            enemyDropPool[2].childCount,
        };
        playerData.enemyDrop0Positions = new SerializableVector3[enemyDropPool[0].childCount];
        for (int p = 0; p < enemyDropPool[0].childCount; p++)
            playerData.enemyDrop0Positions[p] = enemyDropPool[0].GetChild(p).position;
        playerData.enemyDrop1Positions = new SerializableVector3[enemyDropPool[1].childCount];
        for (int p = 0; p < enemyDropPool[1].childCount; p++)
            playerData.enemyDrop1Positions[p] = enemyDropPool[1].GetChild(p).position;
        playerData.enemyDrop2Positions = new SerializableVector3[enemyDropPool[2].childCount];
        for (int p = 0; p < enemyDropPool[2].childCount; p++)
            playerData.enemyDrop2Positions[p] = enemyDropPool[2].GetChild(p).position;
        //-------------------------------------------------------------------------------------------------------------------------------------------
        //STORE THE ITEM DROP POOL ACTIVE/DEACTIVE DROP OBJECTS
        //-------------------------------------------------------------------------------------------------------------------------------------------
        dropItem0Activated = new bool[enemyDropPool[0].childCount];
        dropItem1Activated = new bool[enemyDropPool[1].childCount];
        dropItem2Activated = new bool[enemyDropPool[2].childCount];

        for (int c = 0; c < enemyDropPool[0].childCount; c++)
            dropItem0Activated[c] = enemyDropPool[0].GetChild(c).gameObject.activeInHierarchy; 
        for (int c = 0; c < enemyDropPool[1].childCount; c++)
            dropItem1Activated[c] = enemyDropPool[1].GetChild(c).gameObject.activeInHierarchy; 
        for (int c = 0; c < enemyDropPool[2].childCount; c++)
            dropItem2Activated[c] = enemyDropPool[2].GetChild(c).gameObject.activeInHierarchy;

        playerData.dropItem0Activated = new bool[dropItem0Activated.Length];
        playerData.dropItem1Activated = new bool[dropItem1Activated.Length];
        playerData.dropItem2Activated = new bool[dropItem2Activated.Length];

        playerData.dropItem0Activated = dropItem0Activated;
        playerData.dropItem1Activated = dropItem1Activated;
        playerData.dropItem2Activated = dropItem2Activated;
        _BinaryFormatter.Serialize(stream, playerData);
        stream.Close();

    }

    public void LoadFileInformation(Text[] slotSText, Text[] slotLText)
    {
        string[] fileNames = new string[9]
        {
            "s0_data",
            "s1_data",
            "s2_data",
            "s3_data",
            "s4_data",
            "s5_data",
            "s6_data",
            "s7_data",
            "s8_data"
        };
        for (int s = 0; s < 9; s++)
        {
            if (File.Exists(dataPath + gamePath + fileNames[s] + FILE_EXTENSION))
            {
                Stream stream = File.Open(dataPath + gamePath + fileNames[s] + FILE_EXTENSION, FileMode.Open);
                playerData = (PlayerData)_BinaryFormatter.Deserialize(stream);
                stream.Close();
                if (s == 0)
                    slotLText[s].text = "Auto_Save " + playerData.episodeName + playerData.levelName + playerData.difficultyName + optionsSystem.SetTime(playerData.levelTime);
                else
                    slotLText[s].text = playerData.episodeName + playerData.levelName + playerData.difficultyName + optionsSystem.SetTime(playerData.levelTime);
            }
            else if (!File.Exists(dataPath + gamePath + fileNames[s] + FILE_EXTENSION))
            {
                slotLText[s].text = "Empty Slot";
            }
            if(s > 0) 
                slotSText[s - 1].text = slotLText[s].text;
        }
    }
    public void AutoSave()
    {
        if (!optionsSystem.autoSave) return;
        SaveData(0);
        SaveOptionData();
        saveIcon.enabled = true;
        saveTimer = saveTime;
        isSaving = true;
    }
    public void SetPlayerScenePosition(int index)
    {
        //Player Object must be shut off in order for this to work.
        player.transform.localPosition = scenePositions[index]; 
        player.transform.localRotation = Quaternion.Euler(sceneRotations[index]); 

    }
    public void SetLevelResultCamPosition()
    {
        resultCamera.transform.position = resultCamPosition;
        resultCamera.transform.rotation = resultCamRotation;
    }
}
[Serializable]
public struct OptionData
{
    //------------------------------------------------------------------------
    //GAME SETTINGS-----------------------------------------------------------
    //------------------------------------------------------------------------
    public int difficultyIndex;
    public int autoAIndex;
    public int autoSIndex;
    public int cameraBIndex;
    public int handIndex;
    public int switchNewIndex;
    public int switchEmptyIndex;
    public int weaponBIndex;
    //------------------------------------------------------------------------
    //CONTROL SETTINGS--------------------------------------------------------
    //------------------------------------------------------------------------
    public int controllerIndex;
    public int invertIndex;
    public float[] sensitivity;
    public int smoothIndex;
    public int vibrationIndex;
    public int runIndex;
    public int airIndex;
    public int lockYIndex;
    //------------------------------------------------------------------------
    //UI SETTINGS-------------------------------------------------------------
    //------------------------------------------------------------------------
    public int messagesActiveIndex;
    public int timeActiveIndex;
    public int fpsActiveIndex;
    public int flashIndex;
    public int hudIndex;
    public int visorColorIndex;
    public int crosshairIndex;
    //------------------------------------------------------------------------
    //VIDEO SETTINGS----------------------------------------------------------
    //------------------------------------------------------------------------
    public int fullScreenIndex;
    public int qualityIndex;
    public int resolutionIndex;
    public int width; 
    public int height; 
    public int refreshRate;
    public int vSyncIndex;
    public float fieldOfView;
    public int[] postEffectIndex;
    public float brightness;
    public int gradingIndex;
    public int toneMapIndex;
    public float saturation;
    public float contrast;
    public float lift;
    public float gamma;
    public float gain;
    public float bloomIntensity;  
    public float grainIntensity;  
    public float vignetteIntensity;
    public int environmentIndex;
    //------------------------------------------------------------------------
    //AUDIO SETTINGS----------------------------------------------------------
    //------------------------------------------------------------------------
    public float[] volumeInterval;
    public int speakerIndex;
    public int virtualVoiceIndex;
    public int realVoiceIndex;
    public int sampleRateIndex;
    public int dpsBufferIndex;
    public int soundtrackIndex;
}
[Serializable]
public struct PlayerData
{
    //----------------------
    //WEAPON SETTINGS
    //----------------------
    public int[] weaponAmmo;
    public int weaponIndex;
    public bool[] weaponObtained;
    public bool[] weaponEquipped;
    //----------------------
    //PLAYER SETTINGS
    //----------------------
    public float health;
    public float armor;
    public int maxArmor;
    public int maxHealth;
    public bool[] keyCards;
    public SerializableQuaternion playerRotation;
    public SerializableVector3 playerPosition;
    //----------------------
    //LEVEL SETTINGS
    //----------------------
    public bool[] ambushesActivated;
    public bool[] enemiesDeadGrunt;
    public bool[] enemiesDeadDin;
    public bool[] enemiesDeadElite;
    public bool[] switchesActivated;
    public bool[] pickupsObtained;
    public bool[] spawnersActivated;
    public bool[] messagesActivated;
    public bool[] cratesExploded;
    public bool[] explodingTriggersActivated;
    public bool[] doorTriggersActivated;
    public bool[] dropItem0Activated;
    public bool[] dropItem1Activated;
    public bool[] dropItem2Activated;
    public int[] dropPoolChildCount;
    public SerializableVector3[] enemyDrop0Positions;
    public SerializableVector3[] enemyDrop1Positions;
    public SerializableVector3[] enemyDrop2Positions;
    public SerializableVector3[] enemyGPosition;
    public SerializableQuaternion[] enemyGRotation;
    public SerializableVector3[] enemyDPosition;
    public SerializableQuaternion[] enemyDRotation;
    public SerializableVector3[] enemyEPosition;
    public SerializableQuaternion[] enemyERotation;
    //----------------------
    //GAME SETTINGS
    //----------------------
    public int sceneIndex;
    public string episodeName;
    public string levelName;
    public string difficultyName;
    public string gameTime;
    public float levelTime;
}

[Serializable]
public struct SerializableVector3
{
    public float x;
    public float y;
    public float z;
    public SerializableVector3(float rX, float rY, float rZ)
    {
        x = rX;
        y = rY;
        z = rZ;
    }
    public override string ToString()
    {
        return string.Format("[{0}, {1}, {2}]", x, y, z);
    }
    public static implicit operator Vector3(SerializableVector3 rValue)
    {
        return new Vector3(rValue.x, rValue.y, rValue.z);
    }
    public static implicit operator SerializableVector3(Vector3 rValue)
    {
        return new SerializableVector3(rValue.x, rValue.y, rValue.z);
    }
}
[Serializable]
public struct SerializableQuaternion
{
    public float x;
    public float y;
    public float z;
    public float w;

    public SerializableQuaternion(float rX, float rY, float rZ, float rW)
    {
        x = rX;
        y = rY;
        z = rZ;
        w = rW;
    }
    public override string ToString()
    {
        return string.Format("[{0}, {1}, {2}, {3}]", x, y, z, w);
    }
    public static implicit operator Quaternion(SerializableQuaternion rValue)
    {
        return new Quaternion(rValue.x, rValue.y, rValue.z, rValue.w);
    }
    public static implicit operator SerializableQuaternion(Quaternion rValue)
    {
        return new SerializableQuaternion(rValue.x, rValue.y, rValue.z, rValue.w);
    }
}
