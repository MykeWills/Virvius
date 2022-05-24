using System.Text;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponSystem : MonoBehaviour
{
    //========================================================================================//
    //===================================[STATIC FIELDS]=======================================//
    //========================================================================================//
    public static WeaponSystem weaponSystem;
    public static WeaponType wType;
    public static int spikerBarrelIndex = 0;
    public float[] max = new float[2];
    //========================================================================================//
    //===================================[PRIVATE FIELDS]======================================//
    //========================================================================================//
    //[Class Access]++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++//
    [SerializeField]
    private AudioSystem audioSystem;
    [SerializeField]
    private EnvironmentSystem environmentSystem;
    [SerializeField]
    private MessageSystem messageSystem;
    [SerializeField]
    private InputSystem inputSystem;
    private BobSystem[] bobSystem = new BobSystem[10];
    [SerializeField]
    private PlayerSystem playerSystem;
    [SerializeField]
    private PowerupSystem powerupSystem;
    [SerializeField]
    private GameSystem gameSystem;
    [SerializeField]
    private OptionsSystem optionsSystem;
    private CommandSystem commandSystem;
    //[Lists, Strings & Messages]+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++//
    private List<int> emitterW0List = new List<int> { 0, 1, 2, 3, 4, 5, 6 };
    private List<int> emitterW4List = new List<int> { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13 };
    private StringBuilder sb = new StringBuilder();
    private string[] weaponNames = new string[11]
   {
        "Battle\nSword",
        "Combat\nShotgun",
        "Spiker\nCannon",
        "Ultra\nShotgun",
        "Revolt\nMinigun",
        "Gyro\nGrenade",
        "Horus\nLauncher",
        "Helix\nRailgun",
        "Photon\nCannon",
        "M_Sigma\nX_1800",
        "Damnation\nBlade"
   };
    private string[] aimTags = new string[3]
    {
        "Enemy",
        "DinEnemy",
        "ObstacleExplosive"
    };
    //[Components]++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++//
    [HideInInspector]
    public Transform[] weaponEmitter;
    private AudioClip weaponSound;
    private AudioClip weaponUWSound;
    private Transform bulletPool;
    private GameObject bulletPrefab;
    //[Structs & Enums]+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++//
    public enum WeaponType { Unarmed, Shotgun, Spiker, UltraShotgun, MiniGun, GrenadeLauncher, RocketLauncher, RailGun, PhotonCannon, MSigma,  };
    private KeyCode[] keys = new KeyCode[10]
    {
        KeyCode.Alpha0,
        KeyCode.Alpha1,
        KeyCode.Alpha2,
        KeyCode.Alpha3,
        KeyCode.Alpha4,
        KeyCode.Alpha5,
        KeyCode.Alpha6,
        KeyCode.Alpha7,
        KeyCode.Alpha8,
        KeyCode.Alpha9
    };
    private RaycastHit raycastHit;
    private RaycastHit enemyHit;
    private LayerMask levelMask;
    private Vector3 holsterRayPoint;
    private Vector3 newTiltPosition;
    private Quaternion[] currentFinRotation = new Quaternion[4];
    private Quaternion[] currentSigRotation = new Quaternion[2];
    //[Variables]+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++//
    private bool gunFlash = false;
    private bool revUp = false;
    private bool beginReving = false;
    private bool beginShafting = false;
    private bool fullSpeed = false;
    private bool noAmmoActive = false;
    private bool isSwitched = false;
    private bool switchRight = false;
    private bool keySwitched = false;
    private bool autoSwitched = false;
    private float rayPointHeight = 5;
    public float holsterDistance = 10;
    private float tiltX = 0;
    private float tiltY = 0;
    private float gunflashTime = 0.05f;
    private float gunflashTimer;
    private float powerSoundTime = 0.5f;
    private float powerSoundTimer;
    private float revTime = 0.2f;
    private float revTimer;
    private float barrelSpeed = 80f;
    private float barrelMiniGunRate = 8f;
    private float barrelPhotonRate = 500f;
    private float time = 0;
    private float weaponSwitchRotSpeed = 50;
    private float weaponSwitchPosSpeed = 10;
    private float photonRevDown = 0;
    private bool photonFullSpeed = false;
    private bool photonReady = false;
    private bool sigmaReady = false;
    private int selectionIndex = 1;
    private int keyIndex = 1;
    private int weaponID = 0;
    private int handID = 0;
    //[public Access (Non Inspector)]+++++++++++++++++++++++++++++++++++++++++++++++++++++++++//
    [HideInInspector]
    public bool[] weaponEquipped;
    //[CHANGE VALUES TO OBTAIN EACH WEAPON]
    [HideInInspector]
    public bool[] weaponObtained;
    //[CHANGE VALUES TO MATCH TOTAL STARTING AMMO PER WEAPON]
    [HideInInspector]
    public int[] defaultWeaponAmmo; 
    [HideInInspector]
    public int[] weaponAmmo;
    //[CHANGE VALUES TO MATCH TOTAL MAX AMMO CAPACITY PER WEAPON]
    [HideInInspector]
    public int[] weaponMaxAmmo;
    [HideInInspector]
    public WeaponType[] weaponList = new WeaponType[10]
{
        WeaponType.Unarmed,
        WeaponType.Shotgun,
        WeaponType.Spiker,
        WeaponType.UltraShotgun,
        WeaponType.MiniGun,
        WeaponType.GrenadeLauncher,
        WeaponType.RocketLauncher,
        WeaponType.RailGun,
        WeaponType.PhotonCannon,
        WeaponType.MSigma
};
    //[HideInInspector]
    public Vector3[] holsteredReturnPos = new Vector3[10];
    //[HideInInspector]
    public Vector3[] holsteredPos = new Vector3[10];
    [HideInInspector]
    public bool isHolstered = false;
    [HideInInspector]
    public bool holsterActive = false;
    [HideInInspector]
    public bool isWeaponMovementFinished = true;
    [HideInInspector]
    public bool noAmmoHolstered = false;
    [HideInInspector]
    public bool isShooting = false;
    [HideInInspector]
    public bool ammoIsEmpty = false;
    [HideInInspector]
    public bool switchingWeapon = false;
    [HideInInspector]
    public float ammo = 0;
    [HideInInspector]
    public int weaponIndex = 1;
    //========================================================================================//
    //===================================[INSPECTOR FIELDS]====================================//
    //========================================================================================//
    //[private Access (Inspector)]++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++//
    [Header("Holster Settings")]
    [Range(0, 200)]
    [SerializeField]
    private float holsterRotSpeed = 120;
    [Range(0, 10)]
    [SerializeField]
    private float holsterPosSpeed = 4;
    [Space]
    [Header("Weapon Tilt Settings")]
    [Range(0, 10)]
    [SerializeField]
    private float tiltSpeed = 2;
    [Range(0, 90)]
    [SerializeField]
    private float tiltAmount = 30;
    [Space]
    [Header("Weapon Assignment")]
    [SerializeField]
    private Image[] weaponImage = new Image[4];
    [SerializeField]
    private Text[] weaponAmmoText = new Text[4];
    [SerializeField]
    private Text[] weaponName = new Text[2];
    [SerializeField]
    private AudioClip pickupSound;  
    [SerializeField]
    private AudioClip[] noAmmoSound = new AudioClip[2];
    [Space]
    [Header("Unarmed Specific ================================")]
    [SerializeField]
    private Animator anim;
    [SerializeField]
    private GameObject[] swordObject = new GameObject[2];
    [Space]
    [Header("Shotgun Specific ================================")]
    [SerializeField]
    private Transform[] shotgunEmitters = new Transform[7];
    [SerializeField]
    private MeshRenderer[] shotgunMuzzle = new MeshRenderer[1];
    [SerializeField]
    private ParticleSystem[] shotgunSmoke = new ParticleSystem[1];
    [SerializeField]
    private Light[] shotgunLight = new Light[1];
    [Space]
    [Header("Spiker Specific =================================")]
    [SerializeField]
    private Transform[] spikerEmitters = new Transform[3];
    [SerializeField]
    private MeshRenderer[] spikerMuzzle = new MeshRenderer[3];
    [SerializeField]
    private ParticleSystem[] spikerSmoke = new ParticleSystem[3];
    [SerializeField]
    private Light[] spikerLight = new Light[3];
    [Space]
    [Header("Ultra Shotgun Specific ================================")]
    [SerializeField]
    private Transform[] uShotgunEmitters = new Transform[14];
    [SerializeField]
    private MeshRenderer[] uShotgunMuzzle = new MeshRenderer[2];
    [SerializeField]
    private ParticleSystem[] uShotgunSmoke = new ParticleSystem[2];
    [SerializeField]
    private Light[] uShotgunLight = new Light[2];
    [Space]
    [Header("Minigun Specific ================================")]
    [SerializeField]
    private Transform minigunBarrel;
    [SerializeField]
    private Transform[] minigunEmitters = new Transform[12];
    [SerializeField]
    private MeshRenderer[] minigunMuzzle = new MeshRenderer[1];
    [SerializeField]
    private ParticleSystem[] minigunSmoke = new ParticleSystem[1];
    [SerializeField]
    private Light[] minigunLight = new Light[1];
    [SerializeField]
    private AudioClip[] minigunRevSfx = new AudioClip[5];
    [Space]
    [Header("Grenade Launcher Specific ================================")]
    [SerializeField]
    private Transform grenadeLauncherDrum;
    [SerializeField]
    private Transform[] grenadeLauncherEmitters = new Transform[1];
    [SerializeField]
    private MeshRenderer[] grenadeLauncherMuzzle = new MeshRenderer[1];
    [SerializeField]
    private ParticleSystem[] grenadeLauncherSmoke = new ParticleSystem[1];
    [SerializeField]
    private Light[] grenadeLauncherLight = new Light[1];
    [SerializeField]
    private AudioClip[] grenadeLauncherDrumSfx = new AudioClip[1];
    private int drumRotationAngle = 0;
    [SerializeField]
    private float drumRotationSpeed = 30f;
    private bool drumSound = false;
    private bool rotateDrum = false;
    [Space]
  
    [Header("Rocket Launcher Specific ================================")]
    [SerializeField]
    private Vector3[] loadStartPositions = new Vector3[5];
    [SerializeField]
    private Vector3[] loadEndPositions = new Vector3[5];
    private bool loadRockets = false;
    private int loadIndex = 0;
    [SerializeField]
    private float loadingSpeed = 20;
    [SerializeField]
    private float loadingBufferTime = 0.5f;
    private float loadingBufferTimer = 0f;
    private bool isBuffering = false;
    [SerializeField]
    private Transform[] rocketLoaderObjs = new Transform[5];
    [SerializeField]
    private Transform[] rocketEmitters = new Transform[3];
    [SerializeField]
    private MeshRenderer[] rocketMuzzle = new MeshRenderer[3];
    [SerializeField]
    private ParticleSystem[] rocketSmoke = new ParticleSystem[3];
    [SerializeField]
    private Light[] rocketLight = new Light[3];
    [SerializeField]
    private AudioClip[] rocketLoadSfx = new AudioClip[3];
    [Space]
    [Header("Rail Gun Specific ================================")]
    [SerializeField]
    private Transform[] railEmitters = new Transform[1];
    [SerializeField]
    private MeshRenderer[] railMuzzle = new MeshRenderer[1];
    [SerializeField]
    private ParticleSystem[] railSmoke = new ParticleSystem[1];
    [SerializeField]
    private Light[] railLight = new Light[1];
    [SerializeField]
    private GameObject railWeaponCoil;
    [SerializeField]
    private GameObject railCoilPrefab;
    [SerializeField]
    private Transform railCoilPool;
    [SerializeField]
    private Renderer railShieldRend;
    [SerializeField]
    private float railShieldTime = 0;
    private Color[] shieldColors = new Color[2]
    {
        new Color(0, 1f, 2.5f, 1),
        new Color(2, 0, 0, 1)
    };
    private float railShieldTimer = 0;
    private bool railShot = false;

    [Space]
    [Header("Photon Specific ================================")]
    [SerializeField]
    private Transform photonBarrel;
    [SerializeField]
    private Transform[] photonShafts = new Transform[4];
    private Vector3[] shaftEndRotations;
    private Quaternion[] shaftStartRotations = new Quaternion[4];
    [SerializeField]
    private Transform[] photonEmitter = new Transform[1];
    [SerializeField]
    private MeshRenderer[] photonMuzzle = new MeshRenderer[1];
    [SerializeField]
    private ParticleSystem[] photonSmoke = new ParticleSystem[1];
    [SerializeField]
    private Light[] photonLight = new Light[1];
    [SerializeField]
    private AudioClip[] photonRevSfx = new AudioClip[4];
    [SerializeField]
    private GameObject photonParticle;
    [Space]
    [Header("MSigma Specific ================================")]
    [SerializeField]
    private Transform[] sigmaDoors = new Transform[2];
    private Vector3[] doorEndRotations;
    private Quaternion[] doorStartRotations = new Quaternion[2];
    [SerializeField]
    private GameObject chargeParticle;
    [SerializeField]
    private Light engineLight;
    [SerializeField]
    private Transform[] rotationSigBarrels = new Transform[22];
    [SerializeField]
    private float[] autoRotationSpeed = new float[2] { 0, 0};
    [SerializeField]
    private float maxChargingSpeed = 300;
    [SerializeField]
    private float chargingIncreaseRate = 20;
    [SerializeField]
    private float chargingTime = 1;
    private float chargingTimer = 0;
    private float chargingIncreaseVal = 0;
    private bool sigmaCharging = false;
    private bool sigmaFullCharge = false;
    [SerializeField]
    private Transform[] sigmaEmitter = new Transform[1];
    [SerializeField]
    private MeshRenderer[] sigmaMuzzle = new MeshRenderer[1];
    [SerializeField]
    private ParticleSystem[] sigmaSmoke = new ParticleSystem[1];
    [SerializeField]
    private Light[] sigmaLight = new Light[1];
    [SerializeField]
    private AudioClip[] sigmaRevSfx = new AudioClip[4];
    [SerializeField]
    private GameObject sigmaParticle;
    [Space]
    [Header("Weapon Array Section ============================")]
    public GameObject[] weapons = new GameObject[10];
    [SerializeField]
    private AudioClip[] weaponSounds = new AudioClip[10];
    [SerializeField]
    private AudioClip[] weaponUWSounds = new AudioClip[10];
    [SerializeField]
    private Transform[] bulletPools = new Transform[10];
    [SerializeField]
    private GameObject[] bulletPrefabs = new GameObject[10];
    [SerializeField]
    private Sprite[] weaponAmmoSprites = new Sprite[10];
    [SerializeField]
    private Color[] weaponColors = new Color[10];
    private List<Transform> aimObjects = new List<Transform>();
    private bool aimObjectViewable = false;
    private Vector3 distanceVector = Vector3.zero;
    //========================================================================================//
    //===================================[UNITY FUNCTIONS]====================================//
    //========================================================================================//
    private void Awake()
    {
        weaponSystem = this;
    }
    public void Start()
    {
        for(int w = 0; w < weapons.Length; w++)
        {
            weapons[w].SetActive(false);
        }
        levelMask = 1 << LayerMask.NameToLayer("Level");
        gunflashTimer = gunflashTime;
        for (int sr = 0; sr < 4; sr++)
            shaftStartRotations[sr] = photonShafts[sr].localRotation;
        for (int sr = 0; sr < 2; sr++)
            doorStartRotations[sr] = sigmaDoors[sr].localRotation;
        shaftEndRotations = new Vector3[4]
        {
            new Vector3(-8, 0, 0),
            new Vector3(8, 0, 180),
            new Vector3(0, -8, 90),
            new Vector3(0, 8, -90)
        };
        doorEndRotations = new Vector3[2]
        {
            new Vector3(0, 0, -45),
            new Vector3(0, 0, 45)
        };
        for (int p = 0; p < holsteredReturnPos.Length; p++)
            weapons[p].transform.localPosition = holsteredReturnPos[p];
        // Set the default weapon for the game
        weaponObtained = new bool[10] { true, false, false, false, false, false, false, false, false, false };
        weaponEquipped = new bool[10] { false, false, false, false, false, false, false, false, false, false };
        defaultWeaponAmmo = new int[10] { 0, 25, 50, 25, 75, 5, 2, 4, 50, 1 };
        weaponAmmo = new int[10] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        weaponMaxAmmo = new int[10] { 0, 100, 150, 100, 300, 50, 20, 30, 200, 4 };
        AutoSelectWeapon(0);
    }
    private void Update()
    {
        if (gameSystem.BlockedAttributesActive()) return;
        if (playerSystem.isDead) return;
        time = Time.deltaTime * powerupSystem.BPowerMultiplier;
        AutoSelect(weaponID);
        SelectWeapon();
        KeyboardSelectWeapon();
        TiltWeapon();
        HolsterWeapon();
        GunFlash(wType);
        ShootWeapon();
        MinigunBarrel();
        GrenadeLauncherDrum();
        LoadRockets();
        RestoreWeaponCoil();
        PhotonBarrel();
        SigmaRotation();
    }
    public void SelectWeaponHand(int index)
    {
        handID = index;
        SetFOVAdjustment();
    }
    private void RestoreWeaponCoil()
    {
        if (wType != WeaponType.RailGun) return;
        if (!railShot) return;
        if (!railWeaponCoil.activeInHierarchy) railWeaponCoil.SetActive(true);
        railShieldTimer -= time;
        railShieldTimer = Mathf.Clamp(railShieldTimer, 0, railShieldTime);
        if (!railShieldRend.materials[0].IsKeywordEnabled("_Emission")) railShieldRend.materials[0].EnableKeyword("_Emission");
        railShieldRend.materials[0].SetColor("_EmissionColor", Color.Lerp(shieldColors[0], shieldColors[1], railShieldTimer) * 2);
        if (railShieldTimer == 0)
        {
          
            railShieldTimer = railShieldTime;
            railShot = false;
        }
    }
    //========================================================================================//
    //====================================[GAME FUNCTIONS]====================================//
    //========================================================================================//
    //[Public Access Methods]-----------------------------------------------------------------//
    public void AutoSelectWeapon(int weaponNum)
    {
        if (!switchingWeapon && !weaponEquipped[weaponNum] && !powerupSystem.powerEnabled[2])
        {
            weaponID = weaponNum;
            autoSwitched = true;
            switchingWeapon = true;
        }
        else if (powerupSystem.powerEnabled[2])
        {
            if (!swordObject[1].activeInHierarchy)
            {
                weaponID = 0;
                autoSwitched = true;
                switchingWeapon = true;
            }
        }
    }
    public void WeaponSetup(WeaponType type)
    {
        // Set the weapon type to match the weapon index - FYI. Weapon Index controls which weapon is active.
        // Grab the current weapon bobbing system
        if (bobSystem[weaponIndex] != weapons[weaponIndex].GetComponent<BobSystem>())
            bobSystem[weaponIndex] = weapons[weaponIndex].GetComponent<BobSystem>();
        bobSystem[weaponIndex].Start();
        bobSystem[weaponIndex].resetBobSystem(weaponList[weaponIndex]);
        weapons[weaponIndex].transform.localPosition = holsteredReturnPos[weaponIndex];
        weapons[weaponIndex].transform.localRotation = Quaternion.identity;
     
        isHolstered = false;
        isWeaponMovementFinished = true;
        holsterActive = false;
        isShooting = false;
        noAmmoActive = false;
        if (wType == WeaponType.MiniGun)
        {
            beginReving = false;
            revUp = false;
            powerSoundTimer = 0;
        }
      
        else if (type == WeaponType.PhotonCannon)
        {
            for (int sr = 0; sr < 4; sr++)
                photonShafts[sr].localRotation = shaftStartRotations[sr];
            photonRevDown = 0;
            photonBarrel.localRotation = Quaternion.identity;
            photonReady = false;
            beginShafting = false;
        }
        else if(wType == WeaponType.MSigma)
        {
            audioSystem.PlayAltAudioSource(4, environmentSystem.headUnderWater ? sigmaRevSfx[3] : sigmaRevSfx[2], 1, 1, false, false);
            chargingTimer = chargingTime;
            for (int sr = 0; sr < 2; sr++)
                sigmaDoors[sr].localRotation = doorStartRotations[sr];
            chargingIncreaseVal = 0;
            sigmaCharging = false;
            sigmaFullCharge = false;
            sigmaReady = false;
        }
        //else if (wType == WeaponType.Unarmed) anim.ResetTrigger("Swing");
        weapons[weaponIndex].SetActive(false);
        weaponEquipped[weaponIndex] = false;
        ShutOffMinigunSound();
        switch (type)
        {
            case WeaponType.Unarmed: weaponIndex = 0; break;
            case WeaponType.Shotgun: weaponIndex = 1; weaponEmitter = shotgunEmitters; break;
            case WeaponType.Spiker: weaponIndex = 2; weaponEmitter = spikerEmitters; break;
            case WeaponType.UltraShotgun: weaponIndex = 3; weaponEmitter = uShotgunEmitters; break;
            case WeaponType.MiniGun: weaponIndex = 4; weaponEmitter = minigunEmitters; break;
            case WeaponType.GrenadeLauncher: weaponIndex = 5; weaponEmitter = grenadeLauncherEmitters; break;
            case WeaponType.RocketLauncher: weaponIndex = 6; weaponEmitter = rocketEmitters; break;
            case WeaponType.RailGun: weaponIndex = 7; weaponEmitter = railEmitters; break;
            case WeaponType.PhotonCannon: weaponIndex = 8; weaponEmitter = photonEmitter; break;
            case WeaponType.MSigma: weaponIndex = 9; weaponEmitter = sigmaEmitter; break;
        }
        ammoIsEmpty = false;

        SelectWeaponHand(optionsSystem.handIndex);
        if (type == WeaponType.RailGun)
        {
            railShot = false;
            railShieldTimer = railShieldTime;
            railShieldRend.materials[0].SetColor("_EmissionColor", shieldColors[0] * 2);
            if (railWeaponCoil.activeInHierarchy) railWeaponCoil.SetActive(false);
            railWeaponCoil.SetActive(true);
        }
        // Make sure all muzzle images a shut off.
        //SHOTGUN------------------------------------------------------------------------------------------
        if (shotgunMuzzle[0].enabled) shotgunMuzzle[0].enabled = false;
        //SPIKER-------------------------------------------------------------------------------------------
        for (int m = 0; m < 3; m++) { if (spikerMuzzle[m].enabled) spikerMuzzle[m].enabled = false; }
        //ULTRASHOTGUN-------------------------------------------------------------------------------------
        for (int m = 0; m < 2; m++) { if (uShotgunMuzzle[m].enabled) uShotgunMuzzle[m].enabled = false; }
        //MINIGUN------------------------------------------------------------------------------------------
        if (minigunMuzzle[0].enabled) minigunMuzzle[0].enabled = false;
        //GRENADELAUNCHER----------------------------------------------------------------------------------
        if (grenadeLauncherMuzzle[0].enabled) grenadeLauncherMuzzle[0].enabled = false;
        //ROCKETLAUNCHER-----------------------------------------------------------------------------------
        for (int m = 0; m < 3; m++) { if (rocketMuzzle[m].enabled) rocketMuzzle[m].enabled = false; }
        //RAILGUN------------------------------------------------------------------------------------------
        if (railMuzzle[0].enabled) railMuzzle[0].enabled = false;
        //PHOTONCANNON-------------------------------------------------------------------------------------
        if (photonMuzzle[0].enabled) photonMuzzle[0].enabled = false;
        //MSIGMA-------------------------------------------------------------------------------------------
        if (sigmaMuzzle[0].enabled) sigmaMuzzle[0].enabled = false;

        // if smoke particles are active, stop them
        //SHOTGUN------------------------------------------------------------------------------------------
        if (shotgunSmoke[0].isPlaying) shotgunSmoke[0].Stop();
        //SPIKER-------------------------------------------------------------------------------------------
        for (int s = 0; s < 3; s++) { if (spikerSmoke[s].isPlaying) spikerSmoke[s].Stop(); }
        //ULTRASHOTGUN-------------------------------------------------------------------------------------
        for (int s = 0; s < 2; s++) { if (uShotgunSmoke[s].isPlaying) uShotgunSmoke[s].Stop(); }
        //MINIGUN------------------------------------------------------------------------------------------
        if (minigunSmoke[0].isPlaying) minigunSmoke[0].Stop();
        //GRENADELAUNCHER----------------------------------------------------------------------------------
        if (grenadeLauncherSmoke[0].isPlaying) grenadeLauncherSmoke[0].Stop();
        //ROCKETLAUNCHER-----------------------------------------------------------------------------------
        for (int s = 0; s < 3; s++) { if (rocketSmoke[s].isPlaying) rocketSmoke[s].Stop(); }
        //RAILGUN------------------------------------------------------------------------------------------
        if (railSmoke[0].isPlaying) railSmoke[0].Stop();
        //PHOTONCANNON-------------------------------------------------------------------------------------
        if (photonSmoke[0].isPlaying) photonSmoke[0].Stop();
        //MSIGMA-------------------------------------------------------------------------------------------
        if (sigmaSmoke[0].isPlaying) sigmaSmoke[0].Stop();

        // if gun flash are active, shut off
        //SHOTGUN------------------------------------------------------------------------------------------
        if (shotgunLight[0].enabled) shotgunLight[0].enabled = false;
        //SPIKER-------------------------------------------------------------------------------------------
        for (int l = 0; l < 3; l++) { if (spikerLight[l].enabled) spikerLight[l].enabled = false; }
        //ULTRASHOTGUN-------------------------------------------------------------------------------------
        for (int l = 0; l < 2; l++) { if (uShotgunLight[l].enabled) uShotgunLight[l].enabled = false; }
        //MINIGUN------------------------------------------------------------------------------------------
        if (minigunLight[0].enabled) minigunLight[0].enabled = false;
        //GRENADELAUNCHER----------------------------------------------------------------------------------
        if (grenadeLauncherLight[0].enabled) grenadeLauncherLight[0].enabled = false;
        //ROCKETLAUNCHER-----------------------------------------------------------------------------------
        for (int l = 0; l < 3; l++) { if (rocketLight[l].enabled) rocketLight[l].enabled = false; }
        //RAILGUN------------------------------------------------------------------------------------------
        if (railLight[0].enabled) railLight[0].enabled = false;
        //PHOTONCANNON-------------------------------------------------------------------------------------
        if (photonLight[0].enabled) photonLight[0].enabled = false;
        //MSIGMA-------------------------------------------------------------------------------------------
        if (sigmaLight[0].enabled) sigmaLight[0].enabled = false;

        // Shut off ammo banner if unarmed.
        for (int sp = 0; sp < 4; sp++)
        {
            weaponImage[sp].enabled = UnarmedActive();
            weaponAmmoText[sp].enabled = UnarmedActive();
        }
        for (int wn = 0; wn < 2; wn++)
            weaponName[wn].text = weaponNames[powerupSystem.powerEnabled[2] ? 10 : weaponIndex];
        // Set the weapon active that matches the weapon index then shut off the rest.
        weapons[weaponIndex].SetActive(true);
        if (weaponIndex == 0)
        {
            RestoreWeaponPosition();
            bool bBlade = powerupSystem.powerEnabled[2] ? true : false;
            swordObject[0].SetActive(!bBlade);
            swordObject[1].SetActive(bBlade);
        }
        // Grab the current weapon bobbing system
        if (bobSystem[weaponIndex] != weapons[weaponIndex].GetComponent<BobSystem>())
            bobSystem[weaponIndex] = weapons[weaponIndex].GetComponent<BobSystem>();
        bobSystem[weaponIndex].SetBobVectors();
        // Set the current sound placement to active weapon sound.
        weaponSound = weaponSounds[weaponIndex];
        weaponUWSound = weaponUWSounds[weaponIndex];
        // Set the weapon equipped boolean that matches the weapon index true, then set rest as false.
        weaponEquipped[weaponIndex] = true;
        selectionIndex = weaponIndex;
        ApplyWeaponThemeColor();
        //Set the current weapon type placement as current weapon type.
        wType = type;
        if (weaponIndex < 1) return;
        // Apply current weapon ammo sprite to the ammo sprite image on UI game screen.
        for (int sp = 0; sp < weaponImage.Length; sp++)
            weaponImage[sp].sprite = weaponAmmoSprites[weaponIndex];
        // Set the current bullet placement to active bullet object.
        bulletPrefab = bulletPrefabs[weaponIndex];
        // Set the current bullet pool placement to active bullet pool.
        bulletPool = bulletPools[weaponIndex];
        // Set the current ammo placement to active weapon ammo & apply current ammo to game screen UI.
        ApplyAmmo();
    }
    public void ApplyAmmo()
    {
        int val = 0;
        int versionID = playerSystem.versionID;
        int versionIndex = playerSystem.versionIndex;
        if (weaponIndex == 3) val = 1;
        else val = weaponIndex;
        // Apply current weapon name to the ammo banner on the UI game screen.

        weaponName[versionID].text = weaponNames[weaponIndex];
        ammo = weaponAmmo[val];
        ApplyWeaponThemeColor();
        if (commandSystem == null) commandSystem = CommandSystem.commandSystem;
        if (!powerupSystem.powerEnabled[3] && !commandSystem.masterCodesActive[1])
        {
            if (ammo < 1)
            {
                beginReving = false;
                if (wType == WeaponType.MiniGun)
                    powerSoundTimer = 0;
                else if (wType == WeaponType.RocketLauncher)
                    loadRockets = false;
                ShiftWeapon();
                ammoIsEmpty = true;
                noAmmoHolstered = false;
            }
            else if (ammoIsEmpty && ammo > 0)
            {
                if (wType == WeaponType.RocketLauncher)
                {
                    LaunchRockets();
                }
                ammoIsEmpty = false;
                isHolstered = false;
            }
        }
        else
        {
            if (ammoIsEmpty)
            {
                if (wType == WeaponType.RocketLauncher)
                {
                    LaunchRockets();
                }
                ammoIsEmpty = false;
                isHolstered = false;

            }
        }
        if (weaponAmmoText[versionIndex].text != FormatValues(ammo))
            weaponAmmoText[versionIndex].text = FormatValues(ammo);
    }
    public bool ReEquipWeapon()
    {
        for (int w = 1; w < weaponAmmo.Length; w++)
            if (weaponAmmo[w] > 0) return false;
        return true;
    }
    public void ShutOffMinigunSound()
    {
        audioSystem.PlayAltAudioSource(0, minigunRevSfx[1], 1, 1f, false, false);
    }
    public void ResetWeaponSystem()
    {
        anim.Rebind();
        isShooting = false;
        ammoIsEmpty = false;
        noAmmoActive = false;
        beginReving = false;
        gunflashTimer = gunflashTime;
        gunFlash = false;
        drumSound = false;
        sigmaCharging = false;
        sigmaFullCharge = false;
        grenadeLauncherDrum.localRotation = Quaternion.identity;
        for (int bs = 0; bs < 10; bs++)
        {
            if (bobSystem[bs] != null)
                bobSystem[bs].isRecoiling = false;
        }
        isHolstered = false;
        noAmmoHolstered = false;
        isWeaponMovementFinished = true;
        isSwitched = false;
        switchingWeapon = false;
        switchRight = false;
        weapons[weaponIndex].transform.localPosition = holsteredReturnPos[weaponIndex];
        weapons[weaponIndex].transform.localRotation = Quaternion.identity;
        aimObjects.Clear();
        //This is only temporary until save/load state is created
        for (int w = 0; w < weaponObtained.Length; w++)
        {
            //
            if (w < 10)
            {
                if (w == 0)
                {
                    weaponSystem.weaponObtained[w] = true;
                    weaponSystem.weaponAmmo[w] = 0;
                    weaponSystem.ApplyAmmo();
                    weaponSystem.WeaponSetup(weaponSystem.weaponList[w]);
                }
                else
                {
                    weaponSystem.weaponObtained[w] = false;
                    weaponSystem.weaponAmmo[w] = 0;
                    weaponSystem.ApplyAmmo();
                }
            }

        }

    }
    public void GetAmmo(int index, int amt)
    {
        if (index < 1) return;
        audioSystem.PlayAudioSource(pickupSound, 1, 1, 128);
        weaponAmmo[index] += amt;
        if (weaponAmmo[index] > weaponMaxAmmo[index])
            weaponAmmo[index] = weaponMaxAmmo[index];
        ApplyAmmo();
    }
    //[Private Access Methods]----------------------------------------------------------------//
    private bool UnarmedActive()
    {
        if (weaponIndex > 0) return true;
        else return false;
    }
    private Vector3 FOVAdjustment(float[] max)
    {
        float[] minMax = new float[2] { max[0], max[1] };
        float fov = optionsSystem.fieldOfView;
        float adj = Map(fov, 60, 120, minMax[0], minMax[1]);
        float[] handPos = new float[3] { 0, 1.5f, -1.5f };
        if (weaponIndex == 0) handID = 0;
        else handID = optionsSystem.handIndex;
        return new Vector3(handPos[handID], 0.225f, adj);
    }
    public void SetFOVAdjustment()
    {
        transform.GetChild(0).localPosition = FOVAdjustment(max);
    }
    private float Map(float value, float inMin, float inMax, float outMin, float outMax)
    {
        return (value - inMin) * (outMax - outMin) / (inMax - inMin) + outMin;
    }
    private int CheckWeaponAvailableID(bool downShift)
    {
        if (!downShift)
        {
            for (int w = weaponIndex; w < 10; w++)
            {
                if (weaponObtained[w] && weaponAmmo[w] > 0) return w;
            }
        }
        else
        {
            for (int w = weaponIndex - 1; w > -1; w--)
            {
                if (weaponObtained[w] && weaponAmmo[w] > 0) return w;
            }
        }
        return 0;
    }
    private bool CheckWeaponAvailable()
    {
        for (int w = weaponIndex; w < 10; w++)
        {
            if (weaponObtained[w] && weaponAmmo[w] > 0) return true;
        }
        return false;
    }
    private void ShiftWeapon()
    {
        //IF WEAPON RUNS OUT OF AMMO THE SHIFT TO THE NEXT WEAPON ============>>>
        if (optionsSystem == null) optionsSystem = OptionsSystem.optionsSystem;
        if (!optionsSystem.autoSwitchEmpty) return;
        switch (weaponIndex)
        {
            //shotgun
            case 1:
                {
                    //switch to spiker = Look for upper weapon before selecting sword
                    if (weaponObtained[2] && weaponAmmo[2] > 0)
                        AutoSelectWeapon(2);
                    else
                    {
                        //upshift
                        AutoSelectWeapon(CheckWeaponAvailable() ? CheckWeaponAvailableID(false) : 0);
                    }
                    break;
                }
            //spiker
            case 2:
                {
                    //switch to shotgun = Default to shotgun before selecting upper weapon.
                    if (weaponObtained[1] && weaponAmmo[1] > 0)
                        AutoSelectWeapon(1);
                    else

                    {   //upshift
                        AutoSelectWeapon(CheckWeaponAvailable() ? CheckWeaponAvailableID(false) : 0);
                    }
                    break;
                }
            //ultrashotgun
            case 3:
                {
                    //switch to Spiker = downgrade to spiker because no shells available
                    if (weaponObtained[2] && weaponAmmo[2] > 0)
                        AutoSelectWeapon(2);
                    else
                    {
                        //upshift
                        AutoSelectWeapon(CheckWeaponAvailable() ? CheckWeaponAvailableID(false) : 0);
                    }
                    break;
                }
            //minigun
            case 4:
                {
                    //switch to ultrashotgun = downgrade to the previous weapon
                    if (weaponObtained[3] && weaponAmmo[3] > 0)
                        AutoSelectWeapon(3);
                    else
                    {
                        //downshift
                        AutoSelectWeapon(CheckWeaponAvailable() ? CheckWeaponAvailableID(true) : 0);
                    }
                    break;
                }
            //grenadelauncher
            case 5:
                {
                    //switch to minigun = downgrade to the previous weapon
                    if (weaponObtained[4] && weaponAmmo[4] > 0)
                        AutoSelectWeapon(4);
                    else
                    {
                        AutoSelectWeapon(CheckWeaponAvailable() ? CheckWeaponAvailableID(true) : 0);
                    }
                    break;
                }
            //rocketlauncher
            case 6:
                {
                    //switch to grenadelauncher = downgrade to the previous weapon
                    if (weaponObtained[5] && weaponAmmo[5] > 0)
                        AutoSelectWeapon(5);
                    else
                    {
                        AutoSelectWeapon(CheckWeaponAvailable() ? CheckWeaponAvailableID(true) : 0);
                    }
                    break;
                }
            //Railgun
            case 7:
                {
                    //switch to grenadelauncher = downgrade to the previous weapon
                    if (weaponObtained[6] && weaponAmmo[6] > 0)
                        AutoSelectWeapon(6);
                    else
                    {
                        AutoSelectWeapon(CheckWeaponAvailable() ? CheckWeaponAvailableID(true) : 0);
                    }
                    break;
                }
            //PhotonCannon
            case 8:
                {
                    //switch to grenadelauncher = downgrade to the previous weapon
                    if (weaponObtained[7] && weaponAmmo[7] > 0)
                        AutoSelectWeapon(7);
                    else
                    {
                        AutoSelectWeapon(CheckWeaponAvailable() ? CheckWeaponAvailableID(true) : 0);
                    }
                    break;
                }
            //MSigma
            case 9:
                {
                    //switch to grenadelauncher = downgrade to the previous weapon
                    if (weaponObtained[8] && weaponAmmo[8] > 0)
                        AutoSelectWeapon(8);
                    else
                    {
                        AutoSelectWeapon(CheckWeaponAvailable() ? CheckWeaponAvailableID(true) : 0);
                    }
                    break;
                }
        }
    }
    private void AutoAim(Transform emitter, Vector3 target)
    {
        if (target != null)
            emitter.transform.LookAt(target);
    }
    private Vector3 TargetEnemy()
    {
        for (int e = 0; e < aimObjects.Count; e++)
        {
            if (aimObjects[e] == null) { aimObjects.Clear();  return Vector3.zero; }
            if (aimObjects[e].TryGetComponent(out GruntSystem gruntSystem))
            {
                if (gruntSystem.isDead)
                    aimObjects.Remove(aimObjects[e].transform);
            }
            else if (aimObjects[e].TryGetComponent(out DinSubSystem dinSubSystem))
            {
                if (dinSubSystem.dinSystem.isDead)
                    aimObjects.Remove(aimObjects[e].transform);
            }
            else if (!aimObjects[e].GetChild(0).gameObject.activeInHierarchy)
            {
                aimObjects.Remove(aimObjects[e].transform);
            }
        }
        if (aimObjects.Count < 1) return Vector3.zero;
      
        float distance = 0;
        float shortestDistance = 200;
        int enemyIndex = 0;

        for (int e = 0; e < aimObjects.Count; e++)
        {
            distance = Vector3.Distance(weapons[weaponIndex].transform.position, aimObjects[e].position);
            if (distance < shortestDistance)
            {
                shortestDistance = distance;
                enemyIndex = e;
            }
        }
        distanceVector = aimObjects[enemyIndex].position - weapons[weaponIndex].transform.position;
        if (Physics.Raycast(weapons[weaponIndex].transform.position, distanceVector, out enemyHit, 200))
        {
            if (enemyHit.collider.gameObject == aimObjects[enemyIndex].gameObject)
                aimObjectViewable = true;
            else aimObjectViewable = false;
        }
        if (distance == 0 || !aimObjectViewable) return Vector3.zero;
        else return aimObjects[enemyIndex].position;
    }
    private void OnTriggerEnter(Collider other)
    {
        string tag = other.tag;
        switch (tag)
        {
            case "Enemy":
                if (other.TryGetComponent(out GruntSystem gruntSystem))
                {
                    if (!gruntSystem.isDead)
                        aimObjects.Add(other.transform);
                    else aimObjects.Remove(other.transform);
                }
                break;
            case "DinEnemy":
                if (other.TryGetComponent(out DinSubSystem dinSubSystem))
                {
                    if (!dinSubSystem.dinSystem.isDead)
                        aimObjects.Add(other.transform);
                    else aimObjects.Remove(other.transform);
                }
                break;
            case "NuclearCrate":
                if (other.transform.GetChild(0).gameObject.activeInHierarchy) aimObjects.Add(other.transform);
                else aimObjects.Remove(other.transform);
                break;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        string tag = other.tag;
        switch (tag)
        {
            case "Enemy": aimObjects.Remove(other.transform); break;
            case "DinEnemy": aimObjects.Remove(other.transform); break;
            case "NuclearCrate": aimObjects.Remove(other.transform); break;
        }
    }
    private void RestoreWeaponPosition()
    {
        anim.Rebind();
        isShooting = false;
        ammoIsEmpty = false;
        noAmmoActive = false;
        beginReving = false;
        beginShafting = false;
        sigmaFullCharge = false;
        sigmaCharging = false;
        gunflashTimer = gunflashTime;
        gunFlash = false;
        for (int bs = 0; bs < 10; bs++)
        {
            if (bobSystem[bs] != null)
                bobSystem[bs].isRecoiling = false;
        }
        isHolstered = false;
        holsterActive = false;
        noAmmoHolstered = false;
        isWeaponMovementFinished = true;
        weapons[weaponIndex].transform.localPosition = holsteredReturnPos[weaponIndex];
        weapons[weaponIndex].transform.localRotation = Quaternion.identity;
    }
    private void AutoSelect(int wID)
    {
        if (!switchingWeapon && !autoSwitched) return;
        if (switchingWeapon && autoSwitched)
        {
            if (!isWeaponMovementFinished || isShooting) RestoreWeaponPosition();
            Quaternion[] rot = new Quaternion[2];
            rot[0] = Quaternion.Euler(new Vector3(30, weapons[weaponIndex].transform.localRotation.eulerAngles.y, 0));
            rot[1] = Quaternion.Euler(Vector3.zero);
            Vector3[] pos = new Vector3[2];
            pos[0] = new Vector3(holsteredReturnPos[weaponIndex].x, holsteredReturnPos[weaponIndex].y - 3, holsteredReturnPos[weaponIndex].z);
            pos[1] = holsteredReturnPos[weaponIndex];
            int index = isSwitched ? 1 : 0;
            weapons[weaponIndex].transform.localRotation = Quaternion.RotateTowards(weapons[weaponIndex].transform.localRotation, rot[index], time * weaponSwitchRotSpeed * 1.5f);
            weapons[weaponIndex].transform.localPosition = Vector3.MoveTowards(weapons[weaponIndex].transform.localPosition, pos[index], time * weaponSwitchPosSpeed * 1.5f);

            if (weapons[weaponIndex].transform.localRotation == rot[0] && weapons[weaponIndex].transform.localPosition == pos[0] && !isSwitched)
            {
                isSwitched = true;
                audioSystem.PlayAudioSource(environmentSystem.headUnderWater ? noAmmoSound[1] : noAmmoSound[0], 1, 1, 128);
                WeaponSetup(weaponList[wID]);
                if (rot[0].y != 0) rot[0] = Quaternion.Euler(new Vector3(30, 0, 0));
                pos[0] = new Vector3(holsteredReturnPos[weaponIndex].x, holsteredReturnPos[weaponIndex].y - 3, holsteredReturnPos[weaponIndex].z);
                pos[1] = holsteredReturnPos[weaponIndex];
                weapons[weaponIndex].transform.localRotation = rot[0];
                weapons[weaponIndex].transform.localPosition = pos[0];

            }
            else if (weapons[weaponIndex].transform.localRotation == rot[1] && weapons[weaponIndex].transform.localPosition == pos[1] && isSwitched)
            {
                autoSwitched = false;
                switchingWeapon = false;
                isSwitched = false;
            }
        }
    }
    private void KeyboardSelectWeapon()
    {
        if (powerupSystem.powerEnabled[2]) return;
        if (bobSystem[weaponIndex].isRecoiling) return;
        if (!switchingWeapon)
        {
            for (int i = 0; i < keys.Length; i++)
            {
                if (Input.GetKeyDown(keys[i]))
                {
                    keyIndex = i;
                    if (weaponObtained[keyIndex] && !weaponEquipped[keyIndex])
                    {
                        keySwitched = true;
                        switchingWeapon = true;
                        break;
                    }
                    else return;
                }
            }
        }
        else if (switchingWeapon && keySwitched && !autoSwitched)
        {
            if (!isWeaponMovementFinished || isShooting) RestoreWeaponPosition();
            Quaternion[] rot = new Quaternion[2];
            rot[0] = Quaternion.Euler(new Vector3(30, weapons[weaponIndex].transform.localRotation.eulerAngles.y, 0));
            rot[1] = Quaternion.Euler(Vector3.zero);
            Vector3[] pos = new Vector3[2];
            pos[0] = new Vector3(holsteredReturnPos[weaponIndex].x, holsteredReturnPos[weaponIndex].y - 3, holsteredReturnPos[weaponIndex].z);
            pos[1] = holsteredReturnPos[weaponIndex];
            int index = isSwitched ? 1 : 0;
            weapons[weaponIndex].transform.localRotation = Quaternion.RotateTowards(weapons[weaponIndex].transform.localRotation, rot[index], time * weaponSwitchRotSpeed * 1.5f);
            weapons[weaponIndex].transform.localPosition = Vector3.MoveTowards(weapons[weaponIndex].transform.localPosition, pos[index], time * weaponSwitchPosSpeed * 1.5f);

            if (weapons[weaponIndex].transform.localRotation == rot[0] && weapons[weaponIndex].transform.localPosition == pos[0] && !isSwitched)
            {
                isSwitched = true;
                audioSystem.PlayAudioSource(environmentSystem.headUnderWater ? noAmmoSound[1] : noAmmoSound[0], 1, 1, 128);
                WeaponSetup(weaponList[keyIndex]);
                messageSystem.SetMessage(playerSystem.BuildPersonalMessages(playerSystem.weaponMessage[keyIndex]), MessageSystem.MessageType.Top);
                if (rot[0].y != 0) rot[0] = Quaternion.Euler(new Vector3(30, 0, 0));
                pos[0] = new Vector3(holsteredReturnPos[weaponIndex].x, holsteredReturnPos[weaponIndex].y - 3, holsteredReturnPos[weaponIndex].z);
                pos[1] = holsteredReturnPos[weaponIndex];
                weapons[weaponIndex].transform.localRotation = rot[0];
                weapons[weaponIndex].transform.localPosition = pos[0];
                
            }
            else if (weapons[weaponIndex].transform.localRotation == rot[1] && weapons[weaponIndex].transform.localPosition == pos[1] && isSwitched)
            {
                switchingWeapon = false;
                isSwitched = false;
            }
        }
    }
    private void SelectWeapon()
    {
        if (powerupSystem.powerEnabled[2]) return;
        if (bobSystem[weaponIndex].isRecoiling) return;
        if (inputSystem.inputPlayer.GetButtonDown("Right") && !switchingWeapon)
        {
            keySwitched = false;
            switchingWeapon = true;
            if (!switchRight) switchRight = true;
        }
        else if(inputSystem.inputPlayer.GetButtonDown("Left") && !switchingWeapon)
        {
            keySwitched = false;
            switchingWeapon = true;
            if (switchRight) switchRight = false;
        }
        if (switchingWeapon && !keySwitched && !autoSwitched)
        {
            if (!isWeaponMovementFinished || isShooting) RestoreWeaponPosition();
            Quaternion[] rot = new Quaternion[2];
            rot[0] = Quaternion.Euler(new Vector3(30, weapons[weaponIndex].transform.localRotation.eulerAngles.y, 0));
            rot[1] = Quaternion.Euler(Vector3.zero);
            Vector3[] pos = new Vector3[2];
            pos[0] = new Vector3(holsteredReturnPos[weaponIndex].x, holsteredReturnPos[weaponIndex].y - 3, holsteredReturnPos[weaponIndex].z);
            pos[1] = holsteredReturnPos[weaponIndex];
            int index = isSwitched ? 1 : 0;
            weapons[weaponIndex].transform.localRotation = Quaternion.RotateTowards(weapons[weaponIndex].transform.localRotation, rot[index], time * weaponSwitchRotSpeed * 1.5f);
            weapons[weaponIndex].transform.localPosition = Vector3.MoveTowards(weapons[weaponIndex].transform.localPosition, pos[index], time * weaponSwitchPosSpeed * 1.5f);

            if (weapons[weaponIndex].transform.localRotation == rot[0] && weapons[weaponIndex].transform.localPosition == pos[0] && !isSwitched)
            {
                isSwitched = true;
                if (switchRight)
                {
                    if (selectionIndex == 9)
                        selectionIndex = -1;
                    for (int w = selectionIndex + 1; w < 10; w++)
                    {
                        if (weaponObtained[w])
                        {
                            audioSystem.PlayAudioSource(environmentSystem.headUnderWater ? noAmmoSound[1] : noAmmoSound[0], 1, 1, 128);
                            WeaponSetup(weaponList[w]);
                            messageSystem.SetMessage(playerSystem.BuildPersonalMessages(playerSystem.weaponMessage[w]), MessageSystem.MessageType.Top);
                            if (rot[0].y != 0) rot[0] = Quaternion.Euler(new Vector3(30, 0, 0));
                            pos[0] = new Vector3(holsteredReturnPos[weaponIndex].x, holsteredReturnPos[weaponIndex].y - 3, holsteredReturnPos[weaponIndex].z);
                            pos[1] = holsteredReturnPos[weaponIndex];
                            weapons[weaponIndex].transform.localRotation = rot[0];
                            weapons[weaponIndex].transform.localPosition = pos[0];
                            return;
                        }

                    }
                    audioSystem.PlayAudioSource(environmentSystem.headUnderWater ? noAmmoSound[1] : noAmmoSound[0], 1, 1, 128);
                    WeaponSetup(weaponList[0]);
                    messageSystem.SetMessage(playerSystem.BuildPersonalMessages(playerSystem.weaponMessage[0]), MessageSystem.MessageType.Top);
                    if (rot[0].y != 0) rot[0] = Quaternion.Euler(new Vector3(30, 0, 0));
                    pos[0] = new Vector3(holsteredReturnPos[weaponIndex].x, holsteredReturnPos[weaponIndex].y - 3, holsteredReturnPos[weaponIndex].z);
                    pos[1] = holsteredReturnPos[weaponIndex];
                    weapons[weaponIndex].transform.localRotation = rot[0];
                    weapons[weaponIndex].transform.localPosition = pos[0];
                }
                else
                {
                    if (selectionIndex == 0)
                        selectionIndex = 10;
                    for (int w = selectionIndex - 1; w > -1; w--)
                    {
                        if (weaponObtained[w])
                        {
                            audioSystem.PlayAudioSource(environmentSystem.headUnderWater ? noAmmoSound[1] : noAmmoSound[0], 1, 1, 128);
                            WeaponSetup(weaponList[w]);
                            messageSystem.SetMessage(playerSystem.BuildPersonalMessages(playerSystem.weaponMessage[w]), MessageSystem.MessageType.Top);
                            if (rot[0].y != 0) rot[0] = Quaternion.Euler(new Vector3(30, 0, 0));
                            pos[0] = new Vector3(holsteredReturnPos[weaponIndex].x, holsteredReturnPos[weaponIndex].y - 3, holsteredReturnPos[weaponIndex].z);
                            pos[1] = holsteredReturnPos[weaponIndex];
                            weapons[weaponIndex].transform.localRotation = rot[0];
                            weapons[weaponIndex].transform.localPosition = pos[0];
                            return;
                        }
                    }
                    audioSystem.PlayAudioSource(environmentSystem.headUnderWater ? noAmmoSound[1] : noAmmoSound[0], 1, 1, 128);
                    WeaponSetup(weaponList[0]);
                    messageSystem.SetMessage(playerSystem.BuildPersonalMessages(playerSystem.weaponMessage[0]), MessageSystem.MessageType.Top);
                    if (rot[0].y != 0) rot[0] = Quaternion.Euler(new Vector3(30, 0, 0));
                    pos[0] = new Vector3(holsteredReturnPos[weaponIndex].x, holsteredReturnPos[weaponIndex].y - 3, holsteredReturnPos[weaponIndex].z);
                    pos[1] = holsteredReturnPos[weaponIndex];
                    weapons[weaponIndex].transform.localRotation = rot[0];
                    weapons[weaponIndex].transform.localPosition = pos[0];
                }
            }
            else if (weapons[weaponIndex].transform.localRotation == rot[1] && weapons[weaponIndex].transform.localPosition == pos[1] && isSwitched)
            {
                switchingWeapon = false;
                isSwitched = false;
            }
        }
    }
    private void SigmaRotation()
    {
        if (!weaponEquipped[9]) return;

        for (int sr = 0; sr < 2; sr++)
        {
            if (currentSigRotation[sr] != (sigmaCharging ? Quaternion.Euler(doorEndRotations[sr]) : doorStartRotations[sr]))
                currentSigRotation[sr] = sigmaCharging ? Quaternion.Euler(doorEndRotations[sr]) : doorStartRotations[sr];
            if (sigmaDoors[sr].localRotation != currentSigRotation[sr])
                sigmaDoors[sr].localRotation = Quaternion.RotateTowards(sigmaDoors[sr].localRotation, currentSigRotation[sr], time * 45);
            if (sigmaDoors[1].localRotation == Quaternion.Euler(doorEndRotations[1]))
            {
                if (sigmaCharging && !sigmaReady) { engineLight.enabled = true; chargeParticle.SetActive(true); sigmaReady = true; }
            }
            else if (sigmaDoors[1].localRotation == doorStartRotations[1])
            {
                engineLight.enabled = false; chargeParticle.SetActive(false); sigmaReady = false;
            }
        }
        for (int s = 0; s < rotationSigBarrels.Length - 1; s++)
        {
            float charge = !sigmaCharging ? 0 : chargingIncreaseVal;
            float rotationSpeed = autoRotationSpeed[s] + charge;
            float rotationTime = time * rotationSpeed;
            rotationSigBarrels[s].Rotate(0, 0, (rotationTime * 3));
        }
        if (!sigmaReady) return;
       
        if (!sigmaCharging) { if (chargingIncreaseVal > 0) { chargingTimer = 1; sigmaFullCharge = false; chargingIncreaseVal = 0; } return; }
        chargingTimer -= time;
        inputSystem.SetVibration(1, chargingIncreaseVal / 100, !sigmaCharging ? 0 : 4);
        chargingTimer = Mathf.Clamp01(chargingTimer);
        inputSystem.shakeAmt = ((chargingIncreaseRate / 100) * 2);
        if (chargingTimer == 0) 
        { 
            chargingIncreaseVal += chargingIncreaseRate;
            chargingIncreaseVal = Mathf.Clamp(chargingIncreaseVal, 0, maxChargingSpeed);
            if (chargingIncreaseVal == maxChargingSpeed) sigmaFullCharge = true;
            inputSystem.shakeAmt = 0;
            chargingTimer = chargingTime;
        }
    }
    private void LoadRockets()
    {
        if (!weaponEquipped[6]) return;
        if (!loadRockets) return;
        if (isBuffering)
        {
            loadingBufferTimer -= time;
            loadingBufferTimer = Mathf.Clamp(loadingBufferTimer, 0, loadingBufferTime);
            if (loadingBufferTimer == 0) { loadingBufferTimer = loadingBufferTime; isBuffering = false; }
        }
        else
        {
            if (loadIndex < 1)
            {
                rocketLoaderObjs[loadIndex].localRotation = Quaternion.RotateTowards(rocketLoaderObjs[loadIndex].localRotation, Quaternion.Euler(loadEndPositions[loadIndex]), time * (loadingSpeed * 100));
                if (rocketLoaderObjs[loadIndex].localRotation == Quaternion.Euler(loadEndPositions[loadIndex]))
                {
                    audioSystem.PlayAudioSource(rocketLoadSfx[loadIndex], 1, 1, 128);
                    loadIndex++;
                }
            }
            else
            {
                rocketLoaderObjs[loadIndex].localPosition = Vector3.MoveTowards(rocketLoaderObjs[loadIndex].localPosition, loadEndPositions[loadIndex], time * ((loadIndex == 0) ? loadingSpeed / 4 : loadingSpeed));
                if (rocketLoaderObjs[loadIndex].localPosition == loadEndPositions[loadIndex])
                {
                    audioSystem.PlayAudioSource(rocketLoadSfx[loadIndex], 1, 1, 128);
                    loadIndex++;
                    if (loadIndex > rocketLoaderObjs.Length - 1)
                    {
                        loadRockets = false;
                        return;
                    }
                    isBuffering = true;
                }
               
            }
        }
    }
    private void LaunchRockets()
    {
        //================================================================================================
        //Prepare weapon loading animation when rockets are shot==========================================
        //================================================================================================
        if (loadRockets) return;
        
        //turn on the buffer
        isBuffering = true;
        //reset the loading index
        loadIndex = 0;
        //Set the bufferTime
        loadingBufferTimer = loadingBufferTime;
       
        //Set all loading object to starting positions
        for (int l = 0; l < rocketLoaderObjs.Length; l++)
        {
            if(l < 1) rocketLoaderObjs[l].localRotation = Quaternion.Euler(loadStartPositions[l]);
            else rocketLoaderObjs[l].localPosition = loadStartPositions[l];
        }
     
        //Start the animation
        loadRockets = true;
    }
    private void MinigunBarrel()
    {
        // disregard if minigun is not equipped
        if (!weaponEquipped[4]) return;
        if (!beginReving)
        {
            if (revUp) { audioSystem.PlayAltAudioSource(0, environmentSystem.headUnderWater ? minigunRevSfx[3] : minigunRevSfx[0], 1, 1f, false, true); revUp = false; }


            revTimer = Mathf.Clamp(revTimer - time / 6, 0, revTime);


            if (revTimer < revTime && fullSpeed) fullSpeed = false;
        }
        else
        {
            if (!revUp) { audioSystem.PlayAltAudioSource(0, environmentSystem.headUnderWater ? minigunRevSfx[4] : minigunRevSfx[2], 1, 1f, false, true); revUp = true; }

            //IF I WANT THE BARREL TO FIRE WHEN BARREL IS AT FULL SPEED
            revTimer = Mathf.Clamp(revTimer + time / 2, 0, revTime);

            //IF i WANT BARREL TO FIRE RIGHT AWAY
            //revTimer = revTime;


            if(revTimer == revTime && !fullSpeed) { fullSpeed = true; }

        }
        float theta = (revTimer / revTime) * barrelMiniGunRate * time * barrelSpeed;
        minigunBarrel.Rotate(Vector3.forward, -theta);
    }
    private void PhotonBarrel()
    {
        // disregard if photon is not equipped
        if (!weaponEquipped[8]) return;
        if (weaponAmmo[8] < 1) return;
        for (int sr = 0; sr < 4; sr++)
        {
            if (currentFinRotation[sr] != (beginShafting ? Quaternion.Euler(shaftEndRotations[sr]) : shaftStartRotations[sr]))
                currentFinRotation[sr] = beginShafting ? Quaternion.Euler(shaftEndRotations[sr]) : shaftStartRotations[sr];
            if (photonShafts[sr].localRotation != currentFinRotation[sr])
                photonShafts[sr].localRotation = Quaternion.RotateTowards(photonShafts[sr].localRotation, currentFinRotation[sr], time * 50);
            if (!photonReady)
            {
                if (photonShafts[3].localRotation == Quaternion.Euler(shaftEndRotations[3]))
                {
                    if (beginShafting) photonReady = true;
                }
            }
        }
        if (!photonReady) return;
        photonRevDown = Mathf.Clamp01(photonRevDown += time * (beginShafting ? 3 : -1));
        float downVal = photonRevDown;
        float theta = time * barrelPhotonRate * downVal;
        photonBarrel.Rotate(Vector3.forward, -theta);
        if (downVal == 1 && !photonFullSpeed) { photonFullSpeed = true; }
        else if (downVal == 0 && photonFullSpeed) { photonFullSpeed = false;  beginShafting = false; photonReady = false; }
    }
    private void GrenadeLauncherDrum()
    {
        // disregard if grenadelauncher is not equipped
        if (!weaponEquipped[5]) return;
        if (!rotateDrum) return;
        //if (bobSystem[5].isRecoiling) return;
        if (!drumSound)
        {
            audioSystem.PlayAudioSource(grenadeLauncherDrumSfx[0], 1, 1, 128);
            drumSound = true;
        }
        Quaternion newRotation = Quaternion.Euler(new Vector3(0, 0, drumRotationAngle));
        if (grenadeLauncherDrum.localRotation != newRotation)
        {
            grenadeLauncherDrum.localRotation = Quaternion.RotateTowards(grenadeLauncherDrum.localRotation, newRotation, time * drumRotationSpeed);
        }
        else rotateDrum = false;
    }
    private void HolsterWeapon()
    {
       
        if (switchingWeapon)
        {
            if (!isWeaponMovementFinished)
                isWeaponMovementFinished = true;

            return;  
        }
        // grab the player position for raycast with added raypoint height [Always updated each frame]
        holsterRayPoint = new Vector3(playerSystem.transform.position.x, playerSystem.transform.position.y + rayPointHeight, playerSystem.transform.position.z);
        // If [sword] is selected disregard
        if (weaponIndex < 1) return;
        // Holster start and end positions
        Vector3[] addedPosition = new Vector3[2] { holsteredReturnPos[weaponIndex], holsteredPos[weaponIndex] };
        //--------------------------------
        // Access if player still has ammo
        //--------------------------------
        if (!ammoIsEmpty)
        {
            //===========================================================================================================
            // if player has run out of ammo but has now obtained some, return weapon rotation and position back to normal
            //===========================================================================================================
            if (noAmmoHolstered)
            {
                if (isWeaponMovementFinished)
                    isWeaponMovementFinished = false;
                // return rotation of the weapon
                Quaternion rot = Quaternion.Euler(new Vector3(0, 0, 0));
                // rotate the weapons rotation towards new rotation
                weapons[weaponIndex].transform.localRotation = Quaternion.RotateTowards(weapons[weaponIndex].transform.localRotation, rot, time * (holsterRotSpeed * 5f));
                // move the weapon position back to holsteredReturnPos
                weapons[weaponIndex].transform.localPosition = Vector3.MoveTowards(weapons[weaponIndex].transform.localPosition, addedPosition[0], time * (holsterPosSpeed * 40));
                // when position and rotation has completed
                if (weapons[weaponIndex].transform.localRotation == rot && weapons[weaponIndex].transform.localPosition == addedPosition[0])
                {
                    isWeaponMovementFinished = true;
                    noAmmoHolstered = false;
                }

            }
            //======================================================
            // if player is against the wall activate raycast module
            //======================================================
            else
            {
                // create the angle variable, keep it the same if player is not within raypoint
                float distance = 0;
                // when player is [IN] distance of the raycast
                if (Physics.Raycast(holsterRayPoint, playerSystem.transform.forward, out raycastHit, holsterDistance, levelMask))
                {
                    // grab the value between max distance and current distance
                    distance = 5 - raycastHit.distance;
                    // clamp the value between 0/5
                    distance = Mathf.Clamp(distance, 0, 5);
                    // activate the holstered values
                    isHolstered = true;
                    // start moving and rotating
                    holsterActive = true;
                }
                // when player is [OUT] from distance of the raycast
                else if (!Physics.Raycast(holsterRayPoint, playerSystem.transform.forward, out raycastHit, holsterDistance, levelMask))
                {
                    // deactivate the holstered values
                    isHolstered = false;
                }
                // set multiplier for rotation to move [slower when closer] & [faster when leaving] the wall
                float multiplier = isHolstered ? 0.75f : 1.5f;
                // convert distance value into a rotation value
                float rotVal = Mathf.Clamp(distance * 4, 0, 7);
                // convert rotation value into an angle
                float angle = rotVal * 10;
                // set the angle in Euler and convert it into Quaternion
                Quaternion rot = Quaternion.Euler(new Vector3(0, angle, 0));
                // convert rotation value between 0-1
                float val = rotVal / 7;
                // create a Vector3 array with weapon return position and holstered postion with unaffected Y value
                Vector3[] updatedPos = new Vector3[2] { holsteredReturnPos[weaponIndex], new Vector3(holsteredPos[weaponIndex].x, weapons[weaponIndex].transform.localPosition.y, holsteredPos[weaponIndex].z)
                };
                // create a distance vector to lerp between both points in Vector3 array
                Vector3 dist = Vector3.Lerp(updatedPos[0], updatedPos[1], val);
                // if val is above 0 and below 1 keep weapon movement/rotation active
                if (val > 0 && val < 1) holsterActive = true;
                // if weapon movement is no longer active dont move/rotate weapon
                if (!holsterActive) return;
                if (isWeaponMovementFinished)
                    isWeaponMovementFinished = false;
                // rotate the weapons rotation towards new rotation
                weapons[weaponIndex].transform.localRotation = Quaternion.RotateTowards(weapons[weaponIndex].transform.localRotation, rot, time * (holsterRotSpeed * multiplier));
                // move the weapon position back to holsteredReturnPos
                weapons[weaponIndex].transform.localPosition = Vector3.MoveTowards(weapons[weaponIndex].transform.localPosition, dist, time * holsterPosSpeed);
                // when position and rotation has completed
                if (weapons[weaponIndex].transform.localRotation == rot && weapons[weaponIndex].transform.localPosition == holsteredReturnPos[weaponIndex])
                {
                    holsterActive = false;
                    isWeaponMovementFinished = true;
                }
            }
        }
        //-------------------------------------------------
        // if player has run out of ammo holster the weapon
        //-------------------------------------------------
        else
        {
            // deactivate if weapon is holstered 
            if (noAmmoHolstered) return;
            if (isWeaponMovementFinished)
                isWeaponMovementFinished = false;
            // Set the holstered angle in Euler and convert it into Quaternion
            Quaternion rot = Quaternion.Euler(new Vector3(0, 70, 0));
            // rotate the weapons rotation towards new rotation
            weapons[weaponIndex].transform.localRotation = Quaternion.RotateTowards(weapons[weaponIndex].transform.localRotation, rot, time * (holsterRotSpeed * 1.5f));
            // move the weapon position to holstered Position
            weapons[weaponIndex].transform.localPosition = Vector3.MoveTowards(weapons[weaponIndex].transform.localPosition, addedPosition[1], time * holsterPosSpeed);
            // when position and rotation has completed
            if (weapons[weaponIndex].transform.localRotation == rot && weapons[weaponIndex].transform.localPosition == addedPosition[1])
            {
                noAmmoHolstered = true;
                isWeaponMovementFinished = true;
            }
            
        }
    }
    private void TiltWeapon()
    {
        if (!optionsSystem.weaponMovement) return;
        tiltX = inputSystem.inputPlayer.GetAxis("RSH") * time * tiltAmount;
        tiltY = inputSystem.inputPlayer.GetAxis("RSV") * time * tiltAmount;
        float clampX = Mathf.Clamp(0 + tiltX, -1, 1);
        float clampY = Mathf.Clamp(0 + tiltY, -1, 1);
        newTiltPosition = new Vector3(clampX, clampY, 0);
        transform.localPosition = Vector3.Lerp(transform.localPosition, newTiltPosition, tiltSpeed * Time.deltaTime);
    }
    private void CheckAmmo()
    {
        int val = (weaponIndex == 3) ? 1 : weaponIndex;
        ammo = weaponAmmo[val];
        if (commandSystem == null) commandSystem = CommandSystem.commandSystem;
        if (!powerupSystem.powerEnabled[3] && !commandSystem.masterCodesActive[1])
        {
            if (ammo < 1)
            {
                beginShafting = false;
                beginReving = false;
                powerSoundTimer = 0;
                loadRockets = false;
                ammoIsEmpty = true;
                noAmmoHolstered = false;
                return;
            }
            else
            {
                if (wType == WeaponType.RocketLauncher) LaunchRockets();
                ammoIsEmpty = false;
                isHolstered = false;
            }
        }
        else
        {
            if (wType == WeaponType.RocketLauncher)
            {
                LaunchRockets();
            }
            ammoIsEmpty = false;
            isHolstered = false;
        }
    }
    private void ShootWeapon()
    {
        if (switchingWeapon) return;
        // disable shoot if weapon is holstered
        if (holsterActive || noAmmoHolstered) return;
        // shoot the weapon with [RT]
        if (inputSystem.inputPlayer.GetButton("RT"))
        {
            CheckAmmo();
            // check to see if current weapon ammo is empty first
            AudioClip weaponSfx = environmentSystem.headUnderWater ? noAmmoSound[1] : noAmmoSound[0];
            // if ammo is already empty and player is not [Berserk] or [Sword] equipped
            if (ammoIsEmpty && !powerupSystem.powerEnabled[3] && !commandSystem.masterCodesActive[1] && wType != WeaponType.Unarmed)
            {
                // play [click] sound only when sound boolean is not active
                if (!noAmmoActive)
                {
                    // play empty clip sound when player trys to shoot.
                    audioSystem.PlayAudioSource(weaponSfx, 1, 0.5f, 128);
                    // activate the sound boolean
                    noAmmoActive = true;
                    // if player is shooting deactivate the bool
                    isShooting = false;
                }
                // don't continue the method
                return;
            }
            // whem player has ammo fire the weapon
            // change the weapon gun shot sound based if the player is under water
            weaponSfx = environmentSystem.headUnderWater ? weaponUWSound : weaponSound;
            switch (wType)
            {
                case WeaponType.Unarmed:
                    {
                        if (AnimatorIsPlaying("Swing")) return;
                        FireWeaponType(wType);
                        break;
                    }
                case WeaponType.Shotgun:
                    {
                        if (noAmmoHolstered) return;
                        if (bobSystem[weaponIndex].isRecoiling) return;
                        inputSystem.RecoilEffect(-3, 0, 0, 40);
                        bobSystem[weaponIndex].isRecoiling = true;
                        isShooting = true;
                        gunFlash = true;
                        if (powerupSystem.powerEnabled[0])
                        {
                            audioSystem.PlayAudioSource(powerupSystem.powerSound[0], 1, 1f, 128);
                            audioSystem.PlayAudioSource(weaponSfx, 1, 0.5f, 150);
                        }
                        else audioSystem.PlayAudioSource(weaponSfx, 1, 1f, 128);
                        FireWeaponType(wType);
                        break;
                    }
                case WeaponType.Spiker:
                    {
                        if (noAmmoHolstered) return;
                        if (powerupSystem.powerEnabled[0])
                        {
                            powerSoundTimer -= time;
                            powerSoundTimer = Mathf.Clamp(powerSoundTimer, 0, powerSoundTime);
                            if (powerSoundTimer == 0)
                            {
                                powerSoundTimer = powerSoundTime;
                                audioSystem.PlayAudioSource(powerupSystem.powerSound[0], 1, 1f, 128);
                            }
                        }
                        if (bobSystem[weaponIndex].isRecoiling) return;
                        inputSystem.RecoilEffect(-1f, 0, 0, 100);
                        bobSystem[weaponIndex].isRecoiling = true;
                        isShooting = true;
                        gunFlash = true;
                        audioSystem.PlayAudioSource(weaponSfx, 1, powerupSystem.powerEnabled[0]? 0.5f : 1, 150);
                        FireWeaponType(wType);
                        break;
                    }
                case WeaponType.UltraShotgun:
                    {
                        if (noAmmoHolstered) return;
                        if (bobSystem[weaponIndex].isRecoiling) return;
                        inputSystem.RecoilEffect(-4, 0, 0, 50);
                        if (ammo > 0 && ammo < 2) { AutoSelectWeapon(1); return; }
                        bobSystem[weaponIndex].isRecoiling = true;
                        isShooting = true;
                        gunFlash = true;
                        if (powerupSystem.powerEnabled[0])
                        {
                            audioSystem.PlayAudioSource(powerupSystem.powerSound[0], 1, 1f, 128);
                            audioSystem.PlayAudioSource(weaponSfx, 1, 0.5f, 150);
                        }
                        else audioSystem.PlayAudioSource(weaponSfx, 1, 1f, 128);
                        FireWeaponType(wType);
                        break;
                    }
                case WeaponType.MiniGun:
                    {
                        if (noAmmoHolstered) return;
                        if (powerupSystem.powerEnabled[0] && fullSpeed)
                        {
                            powerSoundTimer -= time;
                            powerSoundTimer = Mathf.Clamp(powerSoundTimer, 0, powerSoundTime);
                            if (powerSoundTimer == 0)
                            {
                                powerSoundTimer = powerSoundTime;
                                audioSystem.PlayAudioSource(powerupSystem.powerSound[0], 1, 1f, 128);
                            }
                        }
                        if (bobSystem[weaponIndex].isRecoiling) return;
                        inputSystem.RecoilEffect(-0.75f, 0, 0, 90);
                        beginReving = true;
                        if (revTimer >= revTime)
                        {
                            bobSystem[weaponIndex].isRecoiling = true;
                            isShooting = true;
                            gunFlash = true;
                            if (powerupSystem.powerEnabled[0]) audioSystem.PlayAudioSource(weaponSfx, 1, 0.8f, 150);
                            else audioSystem.PlayAudioSource(weaponSfx, 1, 1f, 128);
                            FireWeaponType(wType);
                        }
                        break;
                    }
                case WeaponType.GrenadeLauncher:
                    {
                        if (noAmmoHolstered) return;
                        if (bobSystem[weaponIndex].isRecoiling) return;
                        rotateDrum = true;
                        inputSystem.RecoilEffect(-3, 0, 0, 40);
                        bobSystem[weaponIndex].isRecoiling = true;
                        isShooting = true;
                        gunFlash = true;
                        drumSound = false;
                        if (powerupSystem.powerEnabled[0])
                        {
                            audioSystem.PlayAudioSource(powerupSystem.powerSound[0], 1, 1f, 128);
                            audioSystem.PlayAudioSource(weaponSfx, 1, 0.5f, 150);
                        }
                        else audioSystem.PlayAudioSource(weaponSfx, 1, 1f, 128);
                        FireWeaponType(wType);
                        break;
                    }
                case WeaponType.RocketLauncher:
                    {
                        if (noAmmoHolstered) return;
                        if (bobSystem[weaponIndex].isRecoiling) return;
                        LaunchRockets();
                        inputSystem.RecoilEffect(-3, 0, 0, 40);
                        bobSystem[weaponIndex].isRecoiling = true;
                        isShooting = true;
                        gunFlash = true;
                      
                        if (powerupSystem.powerEnabled[0])
                        {
                            audioSystem.PlayAudioSource(powerupSystem.powerSound[0], 1, 1f, 128);
                            audioSystem.PlayAudioSource(weaponSfx, 1, 0.5f, 150);
                        }
                        else audioSystem.PlayAudioSource(weaponSfx, 1, 1f, 128);
                        FireWeaponType(wType);
                        break;
                    }
                case WeaponType.RailGun:
                    {
                        if (noAmmoHolstered) return;
                        if (bobSystem[weaponIndex].isRecoiling) return;
                        inputSystem.RecoilEffect(-5, 0, 0, 45);
                        bobSystem[weaponIndex].isRecoiling = true;
                        isShooting = true;
                        gunFlash = true;
                        if (powerupSystem.powerEnabled[0])
                        {
                            audioSystem.PlayAudioSource(powerupSystem.powerSound[0], 1, 1f, 128);
                            audioSystem.PlayAudioSource(weaponSfx, 1, 0.5f, 150);
                        }
                        else audioSystem.PlayAudioSource(weaponSfx, 1, 1f, 128);
                        FireWeaponType(wType);
                        break;
                    }
                case WeaponType.PhotonCannon:
                    {
                        if (noAmmoHolstered) return;
                        if (!beginShafting) { audioSystem.PlayAudioSource(environmentSystem.headUnderWater ? photonRevSfx[3] : photonRevSfx[2], 1, 1, 128); beginShafting = true; }
                        if (photonFullSpeed)
                        {
                            if (photonParticle.activeInHierarchy) photonParticle.SetActive(false);
                            if (powerupSystem.powerEnabled[0])
                            {
                                powerSoundTimer -= time;
                                powerSoundTimer = Mathf.Clamp(powerSoundTimer, 0, powerSoundTime);
                                if (powerSoundTimer == 0)
                                {
                                    powerSoundTimer = powerSoundTime;
                                    audioSystem.PlayAudioSource(powerupSystem.powerSound[0], 1, 1f, 128);
                                }
                            }
                            if (bobSystem[weaponIndex].isRecoiling) return;
                            inputSystem.RecoilEffect(-1, 0, 0, 50);
                            bobSystem[weaponIndex].isRecoiling = true;
                            isShooting = true;
                            gunFlash = true;
                            audioSystem.PlayAudioSource(weaponSfx, 1, powerupSystem.powerEnabled[0] ? 0.5f: 1, 128);
                            FireWeaponType(wType);
                        }
                        break;
                    }
                case WeaponType.MSigma:
                    {

                        if (noAmmoHolstered) return;
                        if (!sigmaCharging) 
                        {
                            if (!bobSystem[weaponIndex].isRecoiling) { inputSystem.SetScreenShakeEffect(0, 4); audioSystem.PlayAltAudioSource(4, environmentSystem.headUnderWater ? sigmaRevSfx[3] : sigmaRevSfx[2], 1, 1, false, true); sigmaCharging = true; }
                        }
                        if (sigmaFullCharge)
                        {
                            if (powerupSystem.powerEnabled[0])
                            {
                                powerSoundTimer -= time;
                                powerSoundTimer = Mathf.Clamp(powerSoundTimer, 0, powerSoundTime);
                                if (powerSoundTimer == 0)
                                {
                                    powerSoundTimer = powerSoundTime;
                                    audioSystem.PlayAudioSource(powerupSystem.powerSound[0], 1, 1f, 128);
                                }
                            }
                            if (bobSystem[weaponIndex].isRecoiling) return;
                            inputSystem.RecoilEffect(-1, 0, 0, 50);
                            bobSystem[weaponIndex].isRecoiling = true;
                            isShooting = true;
                            gunFlash = true;
                            audioSystem.PlayAudioSource(weaponSfx, 1, powerupSystem.powerEnabled[0] ? 0.5f : 1, 128);
                            FireWeaponType(wType);
                            sigmaCharging = false;
                            sigmaFullCharge = false;
                            chargeParticle.SetActive(false);
                        }
                        break;
                    }
            }
        }
        else if (inputSystem.inputPlayer.GetButtonUp("RT"))
        {
            isShooting = false;
            noAmmoActive = false;
            beginReving = false;
            beginShafting = false;
            sigmaCharging = false;
            if (wType == WeaponType.MiniGun)
                powerSoundTimer = 0;
            if (wType == WeaponType.PhotonCannon)
            {
                if(!photonParticle.activeInHierarchy) photonParticle.SetActive(true);
                audioSystem.PlayAudioSource(environmentSystem.headUnderWater ? photonRevSfx[1] : photonRevSfx[0], 1, 1, 128);
            }
            else if (wType == WeaponType.MSigma)
            {
                inputSystem.SetScreenShakeEffect(0, 0);
                audioSystem.PlayAltAudioSource(4, environmentSystem.headUnderWater ? sigmaRevSfx[1] : sigmaRevSfx[0], 1, 1, false, true);
            }
            else if (wType == WeaponType.Unarmed)
            {
                float speed = commandSystem.masterCodesActive[3] ? 3 : (powerupSystem.powerEnabled[3] ? 2 : 1);
                if (anim.GetFloat("SwingSpeed") != speed)
                    anim.SetFloat("SwingSpeed", speed);
                anim.ResetTrigger("Swing");
            }
           
        }
    }
    private bool AnimatorIsPlaying()
    {
        return anim.GetCurrentAnimatorStateInfo(0).length >
               anim.GetCurrentAnimatorStateInfo(0).normalizedTime - 1;
    }
    private bool AnimatorIsPlaying(string stateName)
    {
        return AnimatorIsPlaying() && anim.GetCurrentAnimatorStateInfo(0).IsName(stateName);
    }
    private void FireWeaponType(WeaponType type)
    {
        float duration = 0;
        int val = 0;
        int ammoReduceAmt = 0;
        switch (type)
        {
            case WeaponType.Unarmed:
                {
                    float speed = commandSystem.masterCodesActive[3] ? 3 : (powerupSystem.powerEnabled[3] ? 2 : 1);
                    if (anim.GetFloat("SwingSpeed") != speed)
                        anim.SetFloat("SwingSpeed", speed);
                    anim.SetTrigger("Swing");
                   
                    return;
                }

            case WeaponType.Shotgun:
                {
                    for (int e = 0; e < 6; e++)
                    {
                        int rnd = Random.Range(0, 7);
                        if (emitterW0List.Contains(rnd))
                            emitterW0List.Remove(rnd);
                        else
                        {
                            rnd = emitterW0List[Random.Range(0, emitterW0List.Count)];
                            emitterW0List.Remove(rnd);
                        }
                        Vector3 randomizeShot = new Vector3(Random.Range(-1, 4), Random.Range(-1, 4), 0);
                        Quaternion newRot = Quaternion.Euler(randomizeShot);
                        weaponEmitter[rnd].localRotation = newRot;
                        SetupBullet(weaponEmitter[rnd], 150000);
                        Vector3 reset = Vector3.zero;
                        weaponEmitter[rnd].parent.localRotation = Quaternion.Euler(reset);
                    }
                    emitterW0List.Clear();
                    for (int l = 0; l < 7; l++)
                        emitterW0List.Add(l);
                    if (shotgunSmoke[0].gameObject.activeInHierarchy) shotgunSmoke[0].gameObject.SetActive(false);
                    if (!shotgunSmoke[0].gameObject.activeInHierarchy) shotgunSmoke[0].gameObject.SetActive(true);
                    duration = powerupSystem.powerEnabled[0] ? 0.5f : 0.2f;
                    inputSystem.SetVibration(0, 1f, duration);
                    break;
                }
            case WeaponType.Spiker:
                {
                    SetupBullet(weaponEmitter[spikerBarrelIndex], 20000);
                    Vector3 reset = Vector3.zero;
                    weaponEmitter[spikerBarrelIndex].parent.localRotation = Quaternion.Euler(reset);
                    if (spikerSmoke[spikerBarrelIndex].gameObject.activeInHierarchy) spikerSmoke[spikerBarrelIndex].gameObject.SetActive(false);
                    if (!spikerSmoke[spikerBarrelIndex].gameObject.activeInHierarchy) spikerSmoke[spikerBarrelIndex].gameObject.SetActive(true);
                    duration = powerupSystem.powerEnabled[0] ? 0.3f : 0.1f;
                    inputSystem.SetVibration(1, 0.5f, duration);
                    break;
                }
            case WeaponType.UltraShotgun:
                {
                    bobSystem[weaponIndex].holdAnimation = true;
                    for (int e = 0; e < 13; e++)
                    {
                        int rnd = Random.Range(0, 14);
                        if (emitterW4List.Contains(rnd))
                            emitterW4List.Remove(rnd);
                        else
                        {
                            rnd = emitterW4List[Random.Range(0, emitterW4List.Count)];
                            emitterW4List.Remove(rnd);
                        }
                        Vector3 randomizeShot = new Vector3(Random.Range(-3, 4), Random.Range(-3, 4), 0);
                        Quaternion newRot = Quaternion.Euler(randomizeShot);
                        weaponEmitter[rnd].localRotation = newRot;
                        SetupBullet(weaponEmitter[rnd], 150000);
                        Vector3 reset = Vector3.zero;
                        weaponEmitter[rnd].parent.localRotation = Quaternion.Euler(reset);
                    }
                    emitterW4List.Clear();
                    for (int l = 0; l < 14; l++)
                        emitterW4List.Add(l);
                    for (int s = 0; s < uShotgunSmoke.Length; s++)
                    {
                        if (uShotgunSmoke[s].gameObject.activeInHierarchy) uShotgunSmoke[s].gameObject.SetActive(false);
                        if (!uShotgunSmoke[s].gameObject.activeInHierarchy) uShotgunSmoke[s].gameObject.SetActive(true);
                    }
                    duration = powerupSystem.powerEnabled[0] ? 0.7f : 0.4f;
                    inputSystem.SetVibration(1, 1f, duration);
                    break;
                }
            case WeaponType.MiniGun:
                {
                    for (int e = 0; e < minigunEmitters.Length; e++)
                    {
                        float[] rng = new float[2] { Random.Range(-0.25f, 0.26f), Random.Range(-0.25f, 0.26f) };
                        float[] rot = new float[2] { Random.Range(-1, 2), Random.Range(-1, 2) };
                        weaponEmitter[e].localPosition = new Vector3(rng[0], rng[1], 0);
                        weaponEmitter[e].localRotation = Quaternion.Euler(new Vector3(rot[0], rot[1], 0));
                    }
                    SetupBullet(weaponEmitter[Random.Range(0, minigunEmitters.Length)], 50000) ;
                    Vector3 reset = Vector3.zero;
                    weaponEmitter[0].parent.localRotation = Quaternion.Euler(reset);

                    if (minigunSmoke[0].gameObject.activeInHierarchy) minigunSmoke[0].gameObject.SetActive(false);
                    if (!minigunSmoke[0].gameObject.activeInHierarchy) minigunSmoke[0].gameObject.SetActive(true);
                    duration = powerupSystem.powerEnabled[0] ? 0.5f : 0.3f;
                    inputSystem.SetVibration(0, 1f, duration);
                    break;
                }
            case WeaponType.GrenadeLauncher:
                {
                    SetupBullet(weaponEmitter[0], 25000);
                    if (grenadeLauncherSmoke[0].gameObject.activeInHierarchy) grenadeLauncherSmoke[0].gameObject.SetActive(false);
                    if (!grenadeLauncherSmoke[0].gameObject.activeInHierarchy) grenadeLauncherSmoke[0].gameObject.SetActive(true);
                    duration = powerupSystem.powerEnabled[0] ? 1.3f : 1f;
                    inputSystem.SetVibration(1, 1f, duration);
                    drumRotationAngle += 60;
                    if (drumRotationAngle >= 360) { drumRotationAngle = 0; grenadeLauncherDrum.localRotation = Quaternion.Euler(new Vector3(0, 0, drumRotationAngle)); }

                    break;
                }
            case WeaponType.RocketLauncher:
                {
                    SetupBullet(weaponEmitter[0], 22500);
                    for (int s = 0; s < rocketSmoke.Length; s++)
                    {
                        if (rocketSmoke[s].gameObject.activeInHierarchy) rocketSmoke[s].gameObject.SetActive(false);
                        if (!rocketSmoke[s].gameObject.activeInHierarchy) rocketSmoke[s].gameObject.SetActive(true);
                    }
                    duration = powerupSystem.powerEnabled[0] ? 1.3f : 1f;
                    inputSystem.SetVibration(1, 1f, duration);
                    break;
                }
            case WeaponType.RailGun:
                {
                    SetupBullet(weaponEmitter[0], 100000);
                    if (railWeaponCoil.activeInHierarchy) railWeaponCoil.SetActive(false);
                    railShieldRend.materials[0].SetColor("_EmissionColor", shieldColors[1] * 2);
                    railShot = true;
                    for (int s = 0; s < railSmoke.Length; s++)
                    {
                        if (railSmoke[s].gameObject.activeInHierarchy) railSmoke[s].gameObject.SetActive(false);
                        if (!railSmoke[s].gameObject.activeInHierarchy) railSmoke[s].gameObject.SetActive(true);
                    }
                    duration = powerupSystem.powerEnabled[0] ? 1.3f : 1f;
                    inputSystem.SetVibration(1, 1f, duration);
                    break;
                }
            case WeaponType.PhotonCannon:
                {
                    for (int e = 0; e < photonEmitter.Length; e++)
                    {
                        //float[] rng = new float[2] { Random.Range(-0.75f, 0.76f), Random.Range(-0.75f, 0.76f) };
                        float[] rot = new float[2] { Random.Range(-0.75f, 0.76f), Random.Range(-0.75f, 0.76f) };
                        //weaponEmitter[e].localPosition = new Vector3(rng[0], rng[1], 0);
                        weaponEmitter[e].localRotation = Quaternion.Euler(new Vector3(rot[0], rot[1], 0));
                    }
                    SetupBullet(weaponEmitter[0], 20000);
                    if (photonSmoke[0].gameObject.activeInHierarchy) photonSmoke[0].gameObject.SetActive(false);
                    if (!photonSmoke[0].gameObject.activeInHierarchy) photonSmoke[0].gameObject.SetActive(true);
                    duration = powerupSystem.powerEnabled[0] ? 0.6f : 0.4f;
                    inputSystem.SetVibration(1, 1f, duration);
                    break;
                }
            case WeaponType.MSigma:
                {
                    SetupBullet(weaponEmitter[0], 5000);
                    inputSystem.SetScreenShakeEffect(5, 1.5f);
                    if (sigmaSmoke[0].gameObject.activeInHierarchy) sigmaSmoke[0].gameObject.SetActive(false);
                    if (!sigmaSmoke[0].gameObject.activeInHierarchy) sigmaSmoke[0].gameObject.SetActive(true);
                    duration = powerupSystem.powerEnabled[0] ? 0.6f : 0.4f;
                    inputSystem.SetVibration(1, 4f, duration);
                    break;
                }
        }
        if (weaponIndex == 3) 
        { 
            val = 1; 
            ammoReduceAmt = 2;
        }
        else { val = weaponIndex; ammoReduceAmt = 1; }

        if(commandSystem == null) commandSystem = CommandSystem.commandSystem;
        int powerVal = commandSystem.masterCodesActive[1] ? 0 : (powerupSystem.powerEnabled[3] ? 0 : ammoReduceAmt);
        weaponAmmo[val] -= powerVal;
        weaponAmmo[val] = Mathf.Clamp(weaponAmmo[val], 0, weaponMaxAmmo[val]);
        ApplyAmmo();
    }
    private void ApplyWeaponThemeColor()
    {
        // Current UI banner type index active
        int versionID = playerSystem.versionID;
        // Current UI screen theme index active
        int versionIndex = playerSystem.versionIndex;

        // [FOR SHOTGUN & ULTRA SHOTGUN ONLY]
        // Keep shotgun & ultra shotgun value = 1 [They use the same ammo]
        int val = 0;
        if (weaponIndex == 3) val = 1;
        else val = weaponIndex;

        // Check if ammo is low or not
        bool lowAmmo = false;
        if (ammo > weaponMaxAmmo[val] * 0.25f) lowAmmo = false;
        else if (ammo <= weaponMaxAmmo[val] * 0.25f) lowAmmo = true;

        // If player has a powerup active
        if (powerupSystem.powerActive)
        {

            // [STANDARD UI HUD] + POWERUP ACTIVE
            if (playerSystem.versionIndex > 1 && playerSystem.versionIndex < 3)
            {
                weaponName[versionID].color = weaponColors[weaponIndex];
                weaponAmmoText[playerSystem.versionIndex].color = powerupSystem.powerEnabled[3] ? weaponColors[weaponIndex] : lowAmmo ? Color.red : weaponColors[weaponIndex];
            }
            // [VISOR UI HUD] + POWERUP ACTIVE
            else if (versionIndex > 2)
            {
                weaponName[versionID].color = powerupSystem.powerColor[powerupSystem.powerIndex];
                weaponAmmoText[versionIndex].color = powerupSystem.powerEnabled[3] ? powerupSystem.powerColor[powerupSystem.powerIndex] : lowAmmo ? Color.red : powerupSystem.powerColor[powerupSystem.powerIndex];
            }
            // [CLASSIC/CLASSIC FULLSCREEN UI] + POWERUP ACTIVE
            else weaponAmmoText[playerSystem.versionIndex].color = powerupSystem.powerEnabled[3] ? Color.white : lowAmmo ? Color.red : Color.white;
        }
        // If player is normal
        else
        {   
            // [STANDARD UI HUD]
            if (playerSystem.versionIndex > 1 && playerSystem.versionIndex < 3)
            {
                weaponName[versionID].color = weaponColors[weaponIndex];
                weaponAmmoText[playerSystem.versionIndex].color = lowAmmo ? Color.red : weaponColors[weaponIndex];
            }
            // [VISOR UI HUD]
            else if (playerSystem.versionIndex > 2)
            {
                weaponName[versionID].color = playerSystem.visorColor2;
                weaponAmmoText[playerSystem.versionIndex].color = lowAmmo ? Color.red : playerSystem.visorColor2;
            }
            // [CLASSIC/CLASSIC FULLSCREEN UI]
            else weaponAmmoText[playerSystem.versionIndex].color = lowAmmo ? Color.red : Color.white;
        }
    }
    private void SetupBullet(Transform emitter, float bulletForce)
    {
        //-------------------------------------------------------------------------------
        //Setup the Emitter--------------------------------------------------------------
        //-------------------------------------------------------------------------------
        //This fixes the rocket firing upward? angling it down is better for centered accuracy
        if (wType == WeaponType.RocketLauncher)
            emitter.parent.localRotation = Quaternion.Euler(new Vector3(3, 0, 0));
        //Reset the parent emitter rotation to default
        else emitter.parent.localRotation = Quaternion.identity;
        //Set the parent emitter rotation to AimTarget if active in options
        if (optionsSystem.autoAim)
        {
            //Only target the enemy if Vector3 is selected from closest active/viewable aimObject transform
            if (TargetEnemy() != Vector3.zero) AutoAim(emitter.parent, TargetEnemy());
        }
        //-------------------------------------------------------------------------------
        //Setup the bullet---------------------------------------------------------------
        //-------------------------------------------------------------------------------
        //Find an inactive bullet from the weapon bullet pool
        GameObject bullet = AccessWeaponBullet();
        //Grab the bullets Rigidbody
        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        //Reset previous velocity to 0
        rb.velocity = Vector3.zero;
        //Set the bullets position to match the weapons emitter
        bullet.transform.position = emitter.position;
        //Set the bullets Rotation to match the weapons emitter
        bullet.transform.rotation = emitter.rotation;
        //Turn on the bullet
        bullet.SetActive(true);
        //kill the bullet after lifetime (seconds)
        switch (wType)
        {
            case WeaponType.GrenadeLauncher: 
                { 
                    if (bullet.TryGetComponent(out BulletSystem bulletSystem)) 
                        bulletSystem.SetupLifeTime(5); 
                    break; 
                }
            case WeaponType.RailGun: 
                {
                    GameObject railCoil = AccessRailCoil();
                    //Set the coil position to match the weapons emitter
                    Vector3 coilPos = new Vector3(emitter.position.x, emitter.position.y + 0.5f, emitter.position.z);
                    railCoil.transform.position = coilPos;
                    //Set the coil Rotation to match the weapons emitter
                    railCoil.transform.rotation = emitter.rotation;
                    //Turn on the coil
                    railCoil.SetActive(true);
                    break;
                }
            case WeaponType.MSigma: 
                { 
                    if (bullet.TryGetComponent(out BulletSystem bulletSystem)) 
                        bulletSystem.SetupLifeTime(10); 
                    break; 
                }

        }
        //Add force to the bullet
        rb.AddForce(emitter.transform.forward * bulletForce);
    }
    private void GunFlash(WeaponType wType)
    {
        if (!gunFlash) return;

        switch (wType)
        {
            case WeaponType.Shotgun:
                {
                    shotgunMuzzle[0].enabled = true;
                    shotgunLight[0].enabled = optionsSystem.showFlashEffects;
                    gunflashTimer -= time;
                    gunflashTimer = Mathf.Clamp(gunflashTimer, 0.0f, 0.05f);
                    if (gunflashTimer == 0.0f)
                    {
                        shotgunMuzzle[0].transform.Rotate(0, 30, 0);
                        shotgunMuzzle[0].enabled = false;
                        gunFlash = false;
                        shotgunLight[0].enabled = false;
                        gunflashTimer = gunflashTime;
                    }
                    break;
                }
            case WeaponType.Spiker:
                {
                    spikerMuzzle[spikerBarrelIndex].enabled = true;
                    spikerLight[spikerBarrelIndex].enabled = optionsSystem.showFlashEffects;
                    gunflashTimer -= time;
                    gunflashTimer = Mathf.Clamp(gunflashTimer, 0.0f, 0.05f);
                    if (gunflashTimer == 0.0f)
                    {
                        spikerMuzzle[spikerBarrelIndex].transform.Rotate(0, 30, 0);
                        spikerMuzzle[spikerBarrelIndex].enabled = false;
                        gunFlash = false;
                        spikerLight[spikerBarrelIndex].enabled = false;
                        gunflashTimer = gunflashTime;
                    }
                    break;
                }
            case WeaponType.UltraShotgun:
                {
                    for (int m = 0; m < 2; m++)
                    {
                        uShotgunMuzzle[m].enabled = true;
                        uShotgunLight[m].enabled = optionsSystem.showFlashEffects;
                    }
                    gunflashTimer -= time;
                    gunflashTimer = Mathf.Clamp(gunflashTimer, 0.0f, 0.05f);
                    if (gunflashTimer == 0.0f)
                    {
                        for (int m = 0; m < 2; m++)
                        {
                            uShotgunMuzzle[m].transform.Rotate(0, 30, 0);
                            uShotgunMuzzle[m].enabled = false;
                            uShotgunLight[m].enabled = false;
                        }
                        gunFlash = false;
                        gunflashTimer = gunflashTime;
                    }
                    break;
                }
            case WeaponType.MiniGun:
                {
                    for (int m = 0; m < 1; m++)
                    {
                        minigunMuzzle[m].enabled = true;
                        minigunLight[m].enabled = optionsSystem.showFlashEffects;
                    }
                    gunflashTimer -= time;
                    gunflashTimer = Mathf.Clamp(gunflashTimer, 0.0f, 0.05f);
                    if (gunflashTimer == 0.0f)
                    {
                        for (int m = 0; m < 1; m++)
                        {
                            minigunMuzzle[m].transform.Rotate(0, 30, 0);
                            minigunMuzzle[m].enabled = false;
                            minigunLight[m].enabled = false;
                        }
                        gunFlash = false;
                        gunflashTimer = gunflashTime;
                    }
                    break;
                }
            case WeaponType.GrenadeLauncher:
                {
                    for (int m = 0; m < 1; m++)
                    {
                        grenadeLauncherMuzzle[m].enabled = true;
                        grenadeLauncherLight[m].enabled = optionsSystem.showFlashEffects;
                    }
                    gunflashTimer -= time;
                    gunflashTimer = Mathf.Clamp(gunflashTimer, 0.0f, 0.05f);
                    if (gunflashTimer == 0.0f)
                    {
                        for (int m = 0; m < 1; m++)
                        {
                            grenadeLauncherMuzzle[m].transform.Rotate(0, 30, 0);
                            grenadeLauncherMuzzle[m].enabled = false;
                            grenadeLauncherLight[m].enabled = false;
                        }
                        gunFlash = false;
                        gunflashTimer = gunflashTime;
                    }
                    break;
                }
            case WeaponType.RocketLauncher:
                {
                    for (int m = 0; m < 3; m++)
                    {
                        rocketMuzzle[m].enabled = true;
                        rocketLight[m].enabled = optionsSystem.showFlashEffects;
                    }
                    gunflashTimer -= time;
                    gunflashTimer = Mathf.Clamp(gunflashTimer, 0.0f, 0.05f);
                    if (gunflashTimer == 0.0f)
                    {
                        for (int m = 0; m < 3; m++)
                        {
                            rocketMuzzle[m].transform.Rotate(0, 30, 0);
                            rocketMuzzle[m].enabled = false;
                            rocketLight[m].enabled = false;
                        }
                        gunFlash = false;
                        gunflashTimer = gunflashTime;
                    }
                    break;
                }
            case WeaponType.RailGun:
                {
                    for (int m = 0; m < 1; m++)
                    {
                        railMuzzle[m].enabled = true;
                        railLight[m].enabled = optionsSystem.showFlashEffects;
                    }
                    gunflashTimer -= time;
                    gunflashTimer = Mathf.Clamp(gunflashTimer, 0.0f, 0.05f);
                    if (gunflashTimer == 0.0f)
                    {
                        for (int m = 0; m < 1; m++)
                        {
                            railMuzzle[m].transform.Rotate(0, 30, 0);
                            railMuzzle[m].enabled = false;
                            railLight[m].enabled = false;
                        }
                        gunFlash = false;
                        gunflashTimer = gunflashTime;
                    }
                    break;
                }
            case WeaponType.PhotonCannon:
                {
                    for (int m = 0; m < 1; m++)
                    {
                        photonMuzzle[m].enabled = true;
                        photonLight[m].enabled = optionsSystem.showFlashEffects;
                    }
                    gunflashTimer -= time;
                    gunflashTimer = Mathf.Clamp(gunflashTimer, 0.0f, 0.05f);
                    if (gunflashTimer == 0.0f)
                    {
                        for (int m = 0; m < 1; m++)
                        {
                            photonMuzzle[m].transform.Rotate(0, 30, 0);
                            photonMuzzle[m].enabled = false;
                            photonLight[m].enabled = false;
                        }
                        gunFlash = false;
                        gunflashTimer = gunflashTime;
                    }
                    break;
                }
            case WeaponType.MSigma:
                {
                    for (int m = 0; m < 1; m++)
                    {
                        sigmaMuzzle[m].enabled = true;
                        sigmaLight[m].enabled = optionsSystem.showFlashEffects;
                    }
                    gunflashTimer -= time;
                    gunflashTimer = Mathf.Clamp(gunflashTimer, 0.0f, 0.05f);
                    if (gunflashTimer == 0.0f)
                    {
                        for (int m = 0; m < 1; m++)
                        {
                            sigmaMuzzle[m].transform.Rotate(0, 30, 0);
                            sigmaMuzzle[m].enabled = false;
                            sigmaLight[m].enabled = false;
                        }
                        gunFlash = false;
                        gunflashTimer = gunflashTime;
                    }
                    break;
                }
        }
    }
    private GameObject AccessWeaponBullet()
    {
        for (int b = 0; b < bulletPool.childCount; b++)
        {
            if (!bulletPool.GetChild(b).gameObject.activeInHierarchy)
                return bulletPool.GetChild(b).gameObject;
        }
        if (GameSystem.expandBulletPool)
        {
            GameObject newBullet = Instantiate(bulletPrefab, bulletPool);
            return newBullet;
        }
        else
            return null;
    }
    private GameObject AccessRailCoil()
    {
        for (int b = 0; b < railCoilPool.childCount; b++)
        {
            if (!railCoilPool.GetChild(b).gameObject.activeInHierarchy)
                return railCoilPool.GetChild(b).gameObject;
        }
        if (GameSystem.expandBulletPool)
        {
            GameObject newCoild = Instantiate(railCoilPrefab, railCoilPool);
            return newCoild;
        }
        else
            return null;
    }
    private string FormatValues(float value)
    {
        if (sb.Length > 0)
            sb.Clear();
        sb.Append(commandSystem.masterCodesActive[1] ? 888 : (powerupSystem.powerEnabled[3] ? 999 : value));
        return sb.ToString();
    }
}
