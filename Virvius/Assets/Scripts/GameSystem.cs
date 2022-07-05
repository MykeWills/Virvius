﻿using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using System;
using Rewired;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine.SceneManagement;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

public class GameSystem : MonoBehaviour
{
    private StringBuilder loadSb = new StringBuilder();
    private StringBuilder[] resultSb = new StringBuilder[3];
    public static GameSystem gameSystem;
    public static bool expandBulletPool = true;
    public bool isGameStarted = false;
    private OptionData optionData = new OptionData();
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
    //[HideInInspector]
    public Vector3[] sceneRotations = new Vector3[4]
    {
        new Vector3(0, 0, 0),
        new Vector3(0, 0, 0),
        new Vector3(0, 0, 0),
        new Vector3(0, 0, 0)
    };
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
    private string OPTION_FILE = "O_data";
    private string FILE_EXTENSION = ".vir";
    private string dataPath;
    [Header("Menu Assignment")]
    [SerializeField]
    private Selectable[] b_Select = new Selectable[6];
    private bool[] currentMenuButtonSelected = new bool[6] { true, false, false, false, false, false };

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
    [Header("Enemy Access")]
    public Transform[] enemyBulletPools = new Transform[2];
    public Transform[] bulletHolePool = new Transform[4];
    public Transform enemyAmmoPool;
    public Transform[] enemyWeaponPools = new Transform[2];

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
    }
    private void LevelTime()
    {
        if (!levelActive)
        {
            ResetLevelStats();
            return;
        }
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
   

    private void Pause()
    {
        if (isLoading) return;
        if (CommandSystem.commandOpen) return;
        if (!isGameStarted) return;
        if (optionsSystem.quitOpen) return;
        if (inputPlayer.GetButtonDown("Start") && optionsSystem.canQuit && !optionsSystem.optionsOpen && !optionsSystem.fileSelectionOpen)
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
            Application.backgroundLoadingPriority = UnityEngine.ThreadPriority.High;
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
    }
    private void StartLevel()
    {
        // Shut off Loading Camera
        if (loadingCamera.activeInHierarchy) loadingCamera.SetActive(false);
        // turn on Player Object
        player.transform.localPosition = scenePositions[sceneIndex];
        player.SetActive(true);
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

        if (sceneIndex > 1) playerSystem.SetupLevel();
        else { SetMasterStart(); playerSystem.SetupNewLevel(); }
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
    public bool Load()
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
    public void Save()
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
