using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class PlayerSystem : MonoBehaviour
{  
    //========================================================================================//
    //===================================[STATIC FIELDS]======================================//
    //========================================================================================//
    public static PlayerSystem playerSystem;
    //========================================================================================//
    //===================================[PRIVATE FIELDS]======================================//
    //========================================================================================//
    //[Class Access]++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++//
    
    [SerializeField]
    private GameObject rainEffect;
    [SerializeField]
    private ImpactReceiver playerImpact;
    [SerializeField]
    private float impactForce = 20f;
    private Renderer rainRenderer;
    private AudioSystem audioSystem;
    private LevelSystem levelSystem;
    private GameSystem gameSystem;
    private PowerupSystem powerupSystem;
    private MessageSystem messageSystem;
    private EnvironmentSystem environmentSystem;
    private CharacterController characterController;
    private WeaponSystem weaponSystem;
    private CommandSystem commandSystem;
    private InputSystem inputSystem;
    private OptionsSystem optionsSystem;
    //[Child Access]++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++//
    [SerializeField]
    private Transform head;
    //[Strings & Messages]++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++//
    private StringBuilder sb = new StringBuilder();
    private StringBuilder msgSb = new StringBuilder();
    [HideInInspector]
    public bool overKill = false;
    private string[] keyName = new string[3]
    {
        "blue",
        "yellow",
        "red"
    };
    private string[] deathWMessage = new string[5]
   {
        // swimming messages
        "were fed to the fishes",
        "became a saggy corpse",
        "drowned in a watery grave",
        "forgot to come up for air",
        "realized you don't have gills",
   };
    private string[] deathAMessage = new string[5]
    {
        // acid messages
        "took a nice acid bath",
        "turned into a rotting slag",
        "ran out of skin tissue",
        "forgot your hazmat suit at home",
        "died of radiation poisoning",
    };
    private string[] deathLMessage = new string[5]
    {
            // swimming messages
            "burned to a crisp",
            "melted into the lava",
            "took a nice hot bath",
            "fell into the lava",
            "were burned alive",
    };
    private string[] deathNMessage = new string[5]
    {
            // normal messages
            "took to many bullets",
            "were shot to death",
            "died",
            "were gunned down",
            "were obliterated",
    };
    private string[] deathPMessage = new string[5]
    {
            // player messages
            "comitted suicide",
            "did not want to live anymore",
            "became a suicide bomber",
            "pulled the trigger too early",
            "forgot to check the gun safety manual",
    };
    private string[] deathFMessage = new string[5]
    {
            // falling messages
            "fell to your death",
            "broke your legs",
            "thought you could fly",
            "forgot your parachute",
            "went skydiving",
    };
    private string[] uiMessage = new string[4]
    {
        "Classic Fullscreen UI Enabled.",
        "Classic Banner UI Enabled.",
        "Standard UI Enabled.",
        "Standard Visor Enabled."
    };
    private string[] levelTitle = new string[4]
    {
        "Welcome",
        "Prologue",
        "Fallen Scourge",
        "Virulent Vault"
    };
    //[structs]+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++//
    

    private Color lowHealthColor;
    private Color hColor = new Color(0, 1f, 0.5f, 1f);
    private Color aColorB = new Color(0, 0.5f, 1, 1f);
    private Color aColorY = new Color(1, 0.5f, 0, 1f);
    private Color aColorR = new Color(1, 0.1f, 0, 1f);
    private Color hBanColor = new Color(0.2f, 0.2f, 0.2f, 1f);
    //[Variables]+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++//
    private bool lowHealth = false;
    private bool isReviver = false;
    private bool isHit = false;
    private bool suicideDamage = false;
    private bool[] fadeEnviroUI = new bool[4] { false, true, false, false };
    private bool[] enviroUIActive = new bool[4] { false, true, false, false };
    private float[] fadeEnviroTime = new float[4] { 1, 1, 1, 1 };
    private float flashTime = 0.3f;
    private float flashTimer;
    private float flashSigmaTimer;
    private float health = 100;
    private float armor = 0;
    private float reviverTime = 1;
    private float reviverTimer;
    private float time = 0;
    private float colorChangeVal = 0;
    private float hitCapTime = 1;
    private float hitCapTimer = 1;
    [HideInInspector]
    public int maxArmor = 50;
    private int maxHealth = 200;
    private int uISelectIndex = 3;

    //[public Access (Non Inspector)]+++++++++++++++++++++++++++++++++++++++++++++++++++++++++//
    [HideInInspector]
    public Color visorColor = new Color(1, 0.98f, 0.7f, 0.1f);
    [HideInInspector]
    public Color visorColor2 = new Color(1, 0.98f, 0.7f, 0.1f);
    [HideInInspector]
    public string[] weaponMessage = new string[10]
    {
        "Battle Sword Selected.",
        "Combat Shotgun Selected.",
        "Spiker Cannon Selected.",
        "Ultra Shotgun Selected.",
        "MiniGun Selected.",
        "Grenade Launcher Selected",
        "Rocket Launcher Selected",
        "",
        "Photon Cannon Selected",
        "",
    };
    [HideInInspector]
    public int versionIndex = 3;
    [HideInInspector]
    public int versionID = 0;
    [HideInInspector]
    public bool fallDamage = false;
    [HideInInspector]
    public bool[] keyCards = new bool[3];
    [HideInInspector]
    public bool isDamaged = false;
    [HideInInspector]
    public bool isFlashUI = false;    
    [HideInInspector]
    public bool isFlashSigmaUI = false;
    [HideInInspector]
    public bool isDead = false;
    //========================================================================================//
    //===================================[INSPECTOR FIELDS]====================================//
    //========================================================================================//
    //[private Access (Inspector)]++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++//
    [Header("Player Transform")]
    [SerializeField]
    private Transform originParent;
    [SerializeField]
    private GameObject goreExplode;
    [Space]
    [Header("Player UI")]
    [SerializeField]
    private GameObject HUD;
    [SerializeField]
    private GameObject[] uiVersions = new GameObject[4];
    [SerializeField]
    private GameObject[] armorBannerObject = new GameObject[4];
    public Image crosshair;
    [SerializeField]
    private Image flashUI;
    [SerializeField]
    private Image flashSigmaUI;
    [SerializeField]
    private Image[] hBanner = new Image[2];
    [SerializeField]
    private Image[] hBar = new Image[2];
    [SerializeField]
    private Image[] hMaxBar = new Image[2];
    [SerializeField]
    private Image[] hIcon = new Image[4];
    [SerializeField]
    private Image[] aBar = new Image[2];
    [SerializeField]
    private Image[] aIcon = new Image[4];
    [SerializeField]
    private Sprite[] aSprites = new Sprite[3];
    [SerializeField]
    private Text[] hAmtText = new Text[4];
    [SerializeField]
    private Text[] aAmtText = new Text[4];
    [SerializeField]
    private Sprite[] flashSprites = new Sprite[4];
    [SerializeField]
    private Image[] enviromentUIActiveImage = new Image[3];
    [Space]
    [Header("Player Sound")]
    [SerializeField]
    private AudioClip[] playerDmgSounds = new AudioClip[3];
    [SerializeField]
    private AudioClip[] playerHealSounds = new AudioClip[3];
    [SerializeField]
    private AudioClip[] playerArmorSounds = new AudioClip[3];
    [SerializeField]
    private AudioClip[] playerDiedSounds = new AudioClip[3];
    [SerializeField]
    private Image[] kV0Icons = new Image[3];
    [SerializeField]
    private Image[] kV1Icons = new Image[3];
    [SerializeField]
    private Image[] kV2Icons = new Image[3];
    [SerializeField]
    private Image[] kV3Icons = new Image[3];
    private Image[,] kCIconsArray;
    [SerializeField]
    private Image[] kIconBanners = new Image[3];
    [SerializeField]
    private AudioClip playerKeyCardSound;
    [HideInInspector]
    public Vector3 collisionPoint;
    private string[] weaponPickupTags = new string[10]
    {
        "",
        "Shotgun",
        "SpikerCannon",
        "UltraShotgun",
        "Minigun",
        "GrenadeLauncher",
        "RocketLauncher",
        "Railgun",
        "PhotonCannon",
        "MSigma"
    }; 
    private string[] weaponPickupMessage = new string[10]
    {
        "",
        "got the Combat Shotgun!",
        "got the Spiker Cannon!",
        "got the Ultra Shotgun!",
        "got the Revolt Minigun!",
        "got the Gyro Grenade Launcher!",
        "got the Horus Rocket Launcher!",
        "got the Pulsar Railgun !",
        "got the Photon Cannon!",
        "got the MSigma Tech!"
    };
    private string[] weaponAmmoPickupTags = new string[10]
   {
        "",
        "ShotgunAmmo",
        "SpikerAmmo",
        "",
        "MinigunAmmo",
        "GrenadeAmmo",
        "RocketAmmo",
        "RailAmmo",
        "PhotonAmmo",
        "SigmaAmmo"
   };
    private string[] weaponAmmoPickupMessage = new string[10]
   {
        "",
        "found Shotgun Shells.",
        "found Hardware Spikes.",
        "",
        "found Minigun Rounds.",
        "found Grenades.",
        "found Rockets.",
        "found Rail Slugs.",
        "found Photon Cells.",
        "found Sigma Energy",
   };
    //========================================================================================//
    //===================================[UNITY FUNCTIONS]====================================//
    //========================================================================================//
    private void Awake()
    {
        playerSystem = this;
        optionsSystem = OptionsSystem.optionsSystem;
    }
    public void Start()
    {
        gameSystem = GameSystem.gameSystem;
        messageSystem = MessageSystem.messageSystem;
        environmentSystem = EnvironmentSystem.environmentSystem;
        audioSystem = AudioSystem.audioSystem;
        commandSystem = CommandSystem.commandSystem;
        characterController = GetComponent<CharacterController>();
        gameSystem.SetPlayerScenePosition(gameSystem.sceneIndex);
        versionIndex = 3;
        uISelectIndex = versionIndex;
        UIVersion(versionIndex);
        flashTimer = flashTime;
        inputSystem = InputSystem.inputSystem;
        ApplyPlayerHealthAndArmor();
        rainRenderer = rainEffect.GetComponent<Renderer>();
        rainRenderer.material.shader = Shader.Find("Unlit/Rain");
        collisionPoint = playerCollisionPoint();
    }
    //[Game Loop]-----------------------------------------------------------------------------//
    private void Update()
    {
        if (gameSystem.BlockedAttributesActive()) return;
        ResetPlayer();
        EnvironmentUIFade();
        if (isDead)
            return;
        time = Time.deltaTime;
        LowHealthPulse();
        Reviver();
        Handler();
        ScreenFlash();
        SigmaFlash();
        HitCapping();
        if (health  < 1)
            KillPlayer();
        SelectUIVersion();
    }
    public Vector3 playerCollisionPoint()
    {
        Vector3 cam = Camera.main.transform.position;
        return cam;
    }
    private void HitCapping()
    {
        if (!isHit) return;
        hitCapTimer -= time;
        hitCapTimer = Mathf.Clamp01(hitCapTimer);
        if(hitCapTimer == 0)
        {
            hitCapTimer = hitCapTime;
            isHit = false;
        }
    }
    //[Player System Collision]---------------------------------------------------------------//
    private void OnTriggerEnter(Collider other)
    {
        // Player Collisions with Environment
        string tag = other.gameObject.tag;
        Physics.IgnoreCollision(other.gameObject.GetComponent<Collider>(), characterController);
        if (other.gameObject.CompareTag("Lava")) environmentSystem.SetEnvironment(0.5f, 1);
        else if (other.gameObject.CompareTag("Acid")) environmentSystem.SetEnvironment(1.45f, 2);
        // Player Collisions with Items
        if (other.gameObject.CompareTag("Health100"))
        {
            audioSystem.PlayAudioSource(playerHealSounds[2], 1, 1, 128);
            RecoverHealth(100, true);
            messageSystem.SetMessage(BuildFixedMessages("found Medical supplies!"), MessageSystem.MessageType.Top);
            other.gameObject.SetActive(false);
        }
        else if (other.gameObject.CompareTag("Health25"))
        {
            if (health < 100)
            {
                audioSystem.PlayAudioSource(playerHealSounds[1], 1, 1, 128);
                RecoverHealth(25, false);
                messageSystem.SetMessage(BuildFixedMessages("obtained a Medical kit."), MessageSystem.MessageType.Top);
                other.gameObject.SetActive(false);
            }
        }
        else if (other.gameObject.CompareTag("Health15"))
        {
            if (health < 100)
            {
                audioSystem.PlayAudioSource(playerHealSounds[0], 1, 1, 128);
                RecoverHealth(15, false);
                messageSystem.SetMessage(BuildFixedMessages("obtained a Medical package."), MessageSystem.MessageType.Top);
                other.gameObject.SetActive(false);
            }
        }
        else if (other.gameObject.CompareTag("Health1"))
        {
            if (health < 200)
            {
                audioSystem.PlayAudioSource(playerHealSounds[3], 1, 1, 128);
                RecoverHealth(2, true);
                messageSystem.SetMessage(BuildFixedMessages("obtained a Medical vial."), MessageSystem.MessageType.Top);
                other.gameObject.SetActive(false);
            }
        }
        else if (other.gameObject.CompareTag("Armor2"))
        {
            if (armor < maxArmor)
            {
                audioSystem.PlayAudioSource(playerArmorSounds[3], 1, 1, 128);
                ObtainArmor(2, maxArmor);
                messageSystem.SetMessage(BuildFixedMessages("picked up an Armor Shard."), MessageSystem.MessageType.Top);
                other.gameObject.SetActive(false);
            }
        }
        else if (other.gameObject.CompareTag("Armor50"))
        {
            if (armor < 50)
            {
                audioSystem.PlayAudioSource(playerArmorSounds[0], 1, 1, 128);
                ObtainArmor(50, 50);
                messageSystem.SetMessage(BuildFixedMessages("picked up Light Armor."), MessageSystem.MessageType.Top);
                other.gameObject.SetActive(false);
            }
        }
        else if (other.gameObject.CompareTag("Armor150"))
        {
            if (armor < 150)
            {
                audioSystem.PlayAudioSource(playerArmorSounds[1], 1, 1, 128);
                ObtainArmor(150, 150);
                messageSystem.SetMessage(BuildFixedMessages("picked up Combat Armor."), MessageSystem.MessageType.Top);
                other.gameObject.SetActive(false);
            }
        }
        else if (other.gameObject.CompareTag("Armor200"))
        {
            if (armor < 200)
            {
                audioSystem.PlayAudioSource(playerArmorSounds[2], 1, 1, 128);
                ObtainArmor(200, 200);
                messageSystem.SetMessage(BuildFixedMessages("equipped Ultra Armor!"), MessageSystem.MessageType.Top);
                other.gameObject.SetActive(false);
            }
        }
        else if (other.gameObject.CompareTag("GruntPack"))
        {
            if (weaponSystem.weaponAmmo[1] < weaponSystem.weaponMaxAmmo[1])
            {
                playerSystem.SetFlash(true);
                if (weaponSystem.weaponObtained[1] && !weaponSystem.weaponObtained[3])
                {
                    if (weaponSystem.ReEquipWeapon())
                        weaponSystem.AutoSelectWeapon(1);
                }
                else if (weaponSystem.weaponObtained[1] && weaponSystem.weaponObtained[3])
                {
                    if (weaponSystem.ReEquipWeapon())
                        weaponSystem.AutoSelectWeapon(3);
                }
                weaponSystem.GetAmmo(1, 2);
                messageSystem.SetMessage(BuildFixedMessages("got 2 Shells."), MessageSystem.MessageType.Top);
                other.transform.parent.gameObject.SetActive(false);
            }
        }
        else if (other.gameObject.CompareTag("ElitePack"))
        {
            if (weaponSystem.weaponAmmo[5] < weaponSystem.weaponMaxAmmo[5])
            {
                playerSystem.SetFlash(true);
                if (weaponSystem.weaponObtained[5] && !weaponSystem.weaponObtained[5])
                {
                    if (weaponSystem.ReEquipWeapon())
                        weaponSystem.AutoSelectWeapon(5);
                }
                else if (weaponSystem.weaponObtained[5] && weaponSystem.weaponObtained[5])
                {
                    if (weaponSystem.ReEquipWeapon())
                        weaponSystem.AutoSelectWeapon(5);
                }
                weaponSystem.GetAmmo(5, 2);
                messageSystem.SetMessage(BuildFixedMessages("got 2 Grenades."), MessageSystem.MessageType.Top);
                other.transform.parent.gameObject.SetActive(false);
            }
        }
        // Player collisions with Keys
        else if (other.gameObject.CompareTag("BlueKey"))
        {
            GiveKey(0);
            other.gameObject.SetActive(false);
        }
        else if (other.gameObject.CompareTag("YellowKey"))
        {
            GiveKey(1);
            other.gameObject.SetActive(false);
        }
        else if (other.gameObject.CompareTag("RedKey"))
        {
            GiveKey(2);
            other.gameObject.SetActive(false);
        }
        // Player collisions with Weapons

        for (int t = 1; t < weaponPickupTags.Length; t++)
        {
            if(tag == weaponPickupTags[t])
            {
                SetFlash(true);
                if (!weaponSystem.weaponObtained[t])
                {
                    weaponSystem.weaponObtained[t] = true;
                    if (optionsSystem.autoSwitchNew)
                        weaponSystem.AutoSelectWeapon(t);
                }
                else
                {
                    if (weaponSystem.ReEquipWeapon())
                        weaponSystem.AutoSelectWeapon(t);
                }
                weaponSystem.GetAmmo(t, weaponSystem.defaultWeaponAmmo[t] * 2);
                messageSystem.SetMessage(BuildFixedMessages(weaponPickupMessage[t]), MessageSystem.MessageType.Top);
                other.gameObject.SetActive(false);
                break;
            }
        }
        for (int t = 1; t < weaponAmmoPickupTags.Length; t++)
        {
            if (tag == weaponAmmoPickupTags[t])
            {
                if (weaponSystem.weaponAmmo[t] < weaponSystem.weaponMaxAmmo[t])
                {
                    playerSystem.SetFlash(true);
                    if (t == 1)
                    {
                        if (weaponSystem.weaponObtained[t] && !weaponSystem.weaponObtained[t + 2])
                        {
                            if (weaponSystem.ReEquipWeapon())
                                weaponSystem.AutoSelectWeapon(t);
                        }
                        else if (weaponSystem.weaponObtained[t] && weaponSystem.weaponObtained[t + 2])
                        {
                            if (weaponSystem.ReEquipWeapon())
                                weaponSystem.AutoSelectWeapon(t + 2);
                        }
                    }
                    else
                    {
                        if (weaponSystem.weaponObtained[t])
                        {
                            if (weaponSystem.ReEquipWeapon())
                                weaponSystem.AutoSelectWeapon(t);
                        }
                    }
                    weaponSystem.GetAmmo(t, weaponSystem.defaultWeaponAmmo[t]);
                    messageSystem.SetMessage(BuildFixedMessages(weaponAmmoPickupMessage[t]), MessageSystem.MessageType.Top);
                    other.gameObject.SetActive(false);
                }
                break;
            }
        }
    }
    public void GiveKey(int ID)
    {
        audioSystem.PlayAudioSource(playerKeyCardSound, 1, 1, 128);
        SetActiveKey(ID, true);
        if(!isFlashUI)
            playerSystem.SetFlash(true);
        messageSystem.SetMessage(BuildFixedMessages("Aquired " + keyName[ID] + " key card!"), MessageSystem.MessageType.Top);
    }
    public void RemoveKey(int ID)
    {
        SetActiveKey(ID, false);
        messageSystem.SetMessage(BuildFixedMessages("Threw Away the"+ keyName[ID] + " key card!"), MessageSystem.MessageType.Top);
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Lava")) environmentSystem.SetEnvironment(0, 0);
        else if (other.gameObject.CompareTag("Acid")) environmentSystem.SetEnvironment(0, 0);
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (isDead) return;
        if (collision.gameObject.CompareTag("ShotgunBullet"))
        {
            int rangeMultiplier = optionsSystem.difficultyActive[0] ? 6 :
                                     optionsSystem.difficultyActive[1] ? 11 :
                                     optionsSystem.difficultyActive[2] ? 21 :
                                     optionsSystem.difficultyActive[3] ? 26 : 1;
            int rndDamage = Random.Range(1, rangeMultiplier);
            if (health < rndDamage + 1) overKill = true;
            Damage(rndDamage);
            collision.gameObject.SetActive(false);
        }
        if (collision.gameObject.CompareTag("GrenadeEBullet"))
        {
            if (collision.gameObject.TryGetComponent(out GrenadeSystem grenadeSystem))
            {
                grenadeSystem.Detonate();
                if (health < 26) overKill = true;
                Damage(Random.Range(10, 15)); playerImpact.AddImpact(-transform.forward, impactForce);
            }
        }
        if (collision.gameObject.CompareTag("SawBlade"))
        {
            int rangeMultiplier = optionsSystem.difficultyActive[0] ? 12 :
                                    optionsSystem.difficultyActive[1] ? 21 :
                                    optionsSystem.difficultyActive[2] ? 31 :
                                    optionsSystem.difficultyActive[3] ? 41 : 1;
            int rndDamage = Random.Range(1, rangeMultiplier);
            if (health < rndDamage + 1) overKill = true;
            Damage(rndDamage);
            playerImpact.AddImpact(-transform.forward, impactForce);
            isHit = true;
        }
        if (collision.gameObject.CompareTag("DinEnemy") && !isHit)
        {
            int rangeMultiplier = optionsSystem.difficultyActive[0] ? 6 :
                                     optionsSystem.difficultyActive[1] ? 11 :
                                     optionsSystem.difficultyActive[2] ? 21 :
                                     optionsSystem.difficultyActive[3] ? 31 : 1;
            int rndDamage = Random.Range(1, rangeMultiplier);
            if (health < rndDamage + 1) overKill = true;
            Damage(rndDamage);
            playerImpact.AddImpact(-transform.forward, impactForce);
            isHit = true;
        }
        if (collision.gameObject.CompareTag("ObstacleBullet"))
        {
            int rangeMultiplier = optionsSystem.difficultyActive[0] ? 4 :
                                    optionsSystem.difficultyActive[1] ? 6 :
                                    optionsSystem.difficultyActive[2] ? 11 :
                                    optionsSystem.difficultyActive[3] ? 16 : 1;
            int rndDamage = Random.Range(1, rangeMultiplier);
            if (health < rndDamage + 1) overKill = true;
            Damage(rndDamage);
        }
        if (collision.gameObject.CompareTag("GrenadeBullet"))
        {
            if (collision.gameObject.TryGetComponent(out GrenadeSystem grenadeSystem))
            {
                if (grenadeSystem.isDetonated)
                {
                    suicideDamage = true;
                    if (health < 26) overKill = true;
                    int damage = powerupSystem.powerEnabled[0] ? 25 * 5 : 25;
                    Damage(damage); playerImpact.AddImpact(-transform.forward, impactForce / 2);
                }
            }
        }
        if (collision.gameObject.CompareTag("RocketBullet"))
        {
            if (collision.gameObject.TryGetComponent(out RocketSystem rocketSystem))
            {
                if (rocketSystem.isDetonated)
                {
                    suicideDamage = true;
                    if (health < 26) overKill = true;
                    int damage = powerupSystem.powerEnabled[0] ? 25 * 5 : 25;
                    Damage(damage); playerImpact.AddImpact(-transform.forward, impactForce / 2);
                }
            }
        }
        if (collision.gameObject.CompareTag("RocketBulletMini"))
        {
            if (collision.gameObject.TryGetComponent(out RocketSubSystem rocketSubSystem))
            {
                if (rocketSubSystem.isDetonated)
                {
                    suicideDamage = true;
                    if (health < 16) overKill = true;
                    int damage = powerupSystem.powerEnabled[0] ? 15 * 5 : 15;
                    Damage(damage); playerImpact.AddImpact(-transform.forward, impactForce / 3);
                }
            }
        }
        if (collision.gameObject.CompareTag("SigmaBullet"))
        {
            if (collision.gameObject.TryGetComponent(out SigmaSystem sigmaSystem))
            {
                if (sigmaSystem.isDetonated)
                {
                    suicideDamage = true;
                    if (health < 51) overKill = true;
                    int damage = powerupSystem.powerEnabled[0] ? 50 * 5 : 50;
                    Damage(damage); playerImpact.AddImpact(-transform.forward, impactForce * 1.5f);
                }
            }
        }
    }
    //========================================================================================//
    //====================================[GAME FUNCTIONS]====================================//
    //========================================================================================//
    //[Player Statistics]---------------------------------------------------------------------//
    private void Reviver()
    {
        if (!isReviver && !powerupSystem.powerEnabled[4]) return;
        else if (isReviver && !powerupSystem.powerEnabled[4]) { isReviver = false; return; }
        reviverTimer -= time;
        reviverTimer = Mathf.Clamp(reviverTimer, 0, reviverTime);
        if (reviverTimer == 0 && health < 200)
        {
            audioSystem.PlayAudioSource(powerupSystem.powerSound[4], 1, 1, 128);
            reviverTimer = reviverTime;
            SetFlash(true);
            RecoverHealth(5, true);
        }
    }
    public void ActivateReviver()
    {
        reviverTimer = reviverTime;
        isReviver = true;
    }
    public void RecoverHealth(int amt, bool overhealth)
    {
        SetFlash(true);
        health += amt;
        int limit = overhealth ? maxHealth : 100;
        health = Mathf.Clamp(health, 0, limit);
        if (health == limit) health = limit;
        ApplyPlayerHealthAndArmor();
    }
    public void ObtainArmor(int amt, int armorMax)
    {
        SetFlash(true);
        maxArmor = armorMax;
        armor += amt;
        armor = Mathf.Clamp(armor, 0, armorMax);
        if (armor == armorMax) armor = armorMax;
        ApplyPlayerHealthAndArmor();
    }
    //[UI Elements]---------------------------------------------------------------------------//
    private void Handler()
    {
        float aFill = Map(armor, 0, maxArmor, 0, 1);
        float hMinFill = Map(health, 0, 100, 0, 1);
        float hMaxFill = Map(health - 100, 0, 100, 0, 1);

        if (armor > 0 && !armorBannerObject[versionIndex].activeInHierarchy)
            armorBannerObject[versionIndex].SetActive(true);
        else
        {
            if (aBar[versionID].fillAmount != aFill) aBar[versionID].fillAmount = aFill;
        }
        if (health > 100)
        {
            if (!hMaxBar[versionID].enabled)
                hMaxBar[versionID].enabled = true;
            if (hMaxBar[versionID].fillAmount != hMaxFill)
            {
                hMaxBar[versionID].fillAmount = hMaxFill;
                hBar[versionID].fillAmount = 1;
            }
        }
        else
        {
            if (hMaxBar[versionID].enabled)
                hMaxBar[versionID].enabled = false;
            if (hBar[versionID].fillAmount != hMinFill)
            {
                hBar[versionID].fillAmount = hMinFill;
                hMaxBar[versionID].fillAmount = 0;
            }
        }
    }
    public void ChangeVisorColorOverhaul()
    {
        if (powerupSystem.powerActive) return;
        crosshair.color = visorColor2;
        hBanner[1].color = visorColor;
        SetKeyBannerColor(visorColor);
        hMaxBar[1].color = visorColor;
        SetUIColor(hAmtText[3], hBar[1], hIcon[3], visorColor2);
        SetUIColor(aAmtText[3], aBar[1], null, visorColor2);
        ApplyPlayerHealthAndArmor();
        weaponSystem.ApplyAmmo();
    }
    private void SetActiveKey(int kID, bool active)
    {
        if(kCIconsArray == null)
        {
            kCIconsArray = new Image[4, 3]
            {
                { kV0Icons[0], kV0Icons[1],kV0Icons[2] },
                { kV1Icons[0], kV1Icons[1],kV1Icons[2] },
                { kV2Icons[0], kV2Icons[1],kV2Icons[2] },
                { kV3Icons[0], kV3Icons[1],kV3Icons[2] },
            };
        }
        for (int y = 0; y < 4; y++)
        {
            for (int x = 0; x < 3; x++)
            {
                if (x == kID)
                {
                    if (keyCards[x] != active) keyCards[x] = active;
                    kCIconsArray[y, x].enabled = active;
                    break;
                }
            }
        }

    }
    private void LowHealthPulse()
    {
        if (versionIndex < 2) return;
        if (powerupSystem.powerActive) return;
        int activeIndex = 0;
        if (versionIndex == 2) activeIndex = 0;
        else if (versionIndex == 3) activeIndex = 1;

        if (lowHealth) 
        { 
            lowHealthColor = new Color(Mathf.Lerp(0.2f, 1, Mathf.PingPong(Time.time, 1)), 0.2f, 0.2f, 1);
            if (optionsSystem.vignetteLayer.enabled.value && optionsSystem.vignetteLayer.color.value != lowHealthColor && !powerupSystem.powerActive) 
                optionsSystem.vignetteLayer.color.value = lowHealthColor;
        }
        else
        {
            if (optionsSystem.vignetteLayer.enabled.value && optionsSystem.vignetteLayer.color.value != Color.black && !powerupSystem.powerActive)
                optionsSystem.vignetteLayer.color.value = Color.black;
            if (versionIndex > 1 && versionIndex < 3)
            {
                if (lowHealthColor != hBanColor)
                    lowHealthColor = hBanColor;
            }
            else if (versionIndex > 2)
            {
                if (lowHealthColor != visorColor)
                {
                    lowHealthColor = visorColor;
                    SetKeyBannerColor(visorColor);
                }
            }
            else
            {
                if (lowHealthColor != Color.white)
                    lowHealthColor = Color.white;
            }
        }
        if (hBanner[activeIndex].color != lowHealthColor)
        {
            hBanner[activeIndex].color = lowHealthColor;
            SetKeyBannerColor(lowHealthColor);
        }

    }
    private void ScreenFlash()
    {
        if (!isFlashUI)
            return;
        int spriteIndex = 0;
        Color colorReturn;
        Color colorReturn2;
        Color colorChange = Color.white;
        if (powerupSystem.powerEnabled[powerupSystem.powerIndex])
        {
            colorReturn = powerupSystem.powerColor[powerupSystem.powerIndex];
            colorReturn2 = powerupSystem.powerColor[powerupSystem.powerIndex];
        }
        else { colorReturn = visorColor; colorReturn2 = visorColor2; }
        if (versionIndex > 2)
        {
            if (isDamaged) colorChange = powerupSystem.powerEnabled[5] ? Color.cyan : powerupSystem.powerEnabled[4] ? Color.yellow : Color.red;
            colorChangeVal += 0.025f;
            colorChangeVal = Mathf.Clamp01(colorChangeVal);
            Color lerpColor = Color.Lerp(colorChange, colorReturn, colorChangeVal);
            Color lerpColor2 = Color.Lerp(colorChange, colorReturn2, colorChangeVal);
            Color lerpColor3 = Color.Lerp(colorChange, Color.black, colorChangeVal);
            if (isDamaged && !lowHealth)
            {
                if(!powerupSystem.powerActive)
                    optionsSystem.vignetteLayer.color.value = lerpColor3;
                SetUIColor(hAmtText[versionIndex], hBar[versionID], hIcon[versionIndex], lerpColor2);
            }
            hBanner[1].color = lerpColor;
            SetKeyBannerColor(lerpColor);
            if (colorChangeVal == 1)
            {
                SetFlash(false);
                if (isDamaged) { suicideDamage = false; isDamaged = false; }
            }
        }
        else
        {
            if (isDamaged) spriteIndex = powerupSystem.powerEnabled[5] ? 3 : 1;
            else spriteIndex = powerupSystem.powerEnabled[4] ? 2 : 0; ;
            if (flashUI.sprite != flashSprites[spriteIndex])
                flashUI.sprite = flashSprites[spriteIndex];

            flashUI.enabled = true;
            flashTimer -= time * 2;
            flashTimer = Mathf.Clamp(flashTimer, 0.0f, flashTime);
            flashUI.color = new Color(flashUI.color.r, flashUI.color.g, flashUI.color.b, flashTimer);
            if (flashTimer == 0 && colorChangeVal == 0)
            {
                flashUI.enabled = false;
                SetFlash(false);
                if (isDamaged) isDamaged = false;
            }
        }
    }
    public void SetFlash(bool active)
    {
        if (active) { if (!optionsSystem.showFlashEffects) return; }
        isFlashUI = active;
        flashTimer = flashTime;
        colorChangeVal = 0;
    }

    private void SigmaFlash()
    {
        if (!isFlashSigmaUI) return;
        if (!flashSigmaUI.enabled) flashSigmaUI.enabled = true;
        flashSigmaTimer += time;
        flashSigmaTimer = Mathf.Clamp(flashSigmaTimer, 0.0f, 1.5f);
        flashSigmaUI.color = Color.Lerp(Color.white, new Color(1, 1, 1, 0), flashSigmaTimer * 2);
        if (flashSigmaTimer == 1.5f )
        {
            SetSigmaFlash(false);
        }
    }
    public void SetSigmaFlash(bool active)
    {
        if (active) { if (!optionsSystem.showFlashEffects) return; }
        isFlashSigmaUI = active;
        flashSigmaTimer = 0;
    }
    public void DisplayDeathMessage()
    {
        int rng = Random.Range(0, 5);
        if (environmentSystem.environmentIndex == 1)
            messageSystem.SetMessage(BuildFixedMessages(deathLMessage[rng]), MessageSystem.MessageType.Top);
        else if (environmentSystem.environmentIndex == 2)
            messageSystem.SetMessage(BuildFixedMessages(deathAMessage[rng]), MessageSystem.MessageType.Top);
        else if (environmentSystem.environmentIndex == 3)
            messageSystem.SetMessage(BuildFixedMessages(deathWMessage[rng]), MessageSystem.MessageType.Top);
        else if (environmentSystem.environmentIndex == 0)
        {
            if (fallDamage)
            {
                messageSystem.SetMessage(BuildFixedMessages(deathFMessage[rng]), MessageSystem.MessageType.Top);
                fallDamage = false;
            }
            else if (suicideDamage)
            {
                messageSystem.SetMessage(BuildFixedMessages(deathPMessage[rng]), MessageSystem.MessageType.Top);
                suicideDamage = false;
            }
            else
                messageSystem.SetMessage(BuildFixedMessages(deathNMessage[rng]), MessageSystem.MessageType.Top);
        }

    }
    public void ApplyPlayerHealthAndArmor()
    {
        if (health < 26) { SetUIColor(hAmtText[versionIndex], hBar[versionID], hIcon[versionIndex], Color.red); lowHealth = true; }
        else
        {
            if (versionIndex > 1 && versionIndex < 3)
                SetUIColor(hAmtText[versionIndex], hBar[versionID], hIcon[versionIndex], hColor);
            else if (versionIndex > 2)
                SetUIColor(hAmtText[versionIndex], hBar[versionID], hIcon[versionIndex], powerupSystem.powerActive ? powerupSystem.powerColor[powerupSystem.powerIndex] : visorColor2);
            else if (versionIndex < 2) SetUIColor(hAmtText[versionIndex], hBar[versionID], hIcon[versionIndex], Color.white);
            lowHealth = false;
        }
        if (armor > 0)
        {
            SetArmorSprites();
            if (!armorBannerObject[versionIndex].activeInHierarchy)
                armorBannerObject[versionIndex].SetActive(true);
            if (armor < 26)
            {
                if (versionIndex > 1)
                    SetUIColor(aAmtText[versionIndex], aBar[versionID], aIcon[versionIndex], Color.red);
                else
                    SetUIColor(aAmtText[versionIndex], aBar[versionID], null, Color.red);
            }
            else if (armor > 25)
            {
                if (versionIndex > 1 && versionIndex < 3)
                {
                    if (maxArmor == 150)
                        SetUIColor(aAmtText[versionIndex], aBar[versionID], aIcon[versionIndex], aColorY);
                    else if (maxArmor == 200)
                        SetUIColor(aAmtText[versionIndex], aBar[versionID], aIcon[versionIndex], aColorR);
                    else
                        SetUIColor(aAmtText[versionIndex], aBar[versionID], aIcon[versionIndex], aColorB);
                }
                else if (versionIndex > 2)
                    SetUIColor(aAmtText[versionIndex], aBar[versionID], aIcon[versionIndex], powerupSystem.powerActive ? powerupSystem.powerColor[powerupSystem.powerIndex] : visorColor2);
                else
                    SetUIColor(aAmtText[versionIndex], aBar[versionID], null, Color.white);
            }
        }
        else if (armorBannerObject[versionIndex].activeInHierarchy)
            armorBannerObject[versionIndex].SetActive(false);
        for (int v = 0; v < 4; v++)
        {
            if (aAmtText[v].text != FormatValues(armor))
                aAmtText[v].text = FormatValues(armor);
            if (hAmtText[v].text != FormatValues(health))
                hAmtText[v].text = FormatValues(health);
        }
    }
    private void SetArmorSprites()
    {
        for (int a = 0; a < 2; a++)
        {
            if (maxArmor == 150)
                aIcon[a].sprite = aSprites[1];
            else if (maxArmor == 200)
                aIcon[a].sprite = aSprites[2];
            else
                aIcon[a].sprite = aSprites[0];
        }
    }
    private void SelectUIVersion()
    {
        if (Input.GetKeyDown(KeyCode.Equals) && versionIndex != 3) uISelectIndex++;
        else if (Input.GetKeyDown(KeyCode.Minus) && versionIndex != 0) uISelectIndex--;
        else return;
        if (uISelectIndex > 3) uISelectIndex = 3;
        else if (uISelectIndex < 0) uISelectIndex = 0;
        messageSystem.SetMessage(BuildPersonalMessages(uiMessage[uISelectIndex]), MessageSystem.MessageType.Top);
        versionIndex = uISelectIndex;
        UIVersion(versionIndex);
    }
    public void UIVersion(int version)
    {
        if(powerupSystem == null) powerupSystem = PowerupSystem.powerupSystem;
        if(weaponSystem == null) weaponSystem = WeaponSystem.weaponSystem;
        // Set UI Attributed Banners
        if (version > 2) versionID = 1;
        else versionID = 0;
        // Set the inpending UI to be active
        for (int v = 0; v < 4; v++)
        {
            if (v == version)
                uiVersions[v].SetActive(true);
            else
                uiVersions[v].SetActive(false);
        }
        if (powerupSystem.powerActive)
            powerupSystem.SetPowerUI(versionID);
        ApplyPlayerHealthAndArmor();
        weaponSystem.ApplyAmmo();
    }
    //[Player Commands]-----------------------------------------------------------------------//
    public void StartFadeEnvironmentUI(bool active, int index, Color color)
    {
        if (!optionsSystem.environmentEffects) 
        {

            for (int e = 0; e < enviromentUIActiveImage.Length; e++)
            {
                if (enviromentUIActiveImage[e].enabled)
                    enviromentUIActiveImage[e].enabled = false;
            }
            if(rainEffect.activeInHierarchy) rainEffect.SetActive(false);
            return; 
        }
        if (active) 
        {

            if (index > 1)
            {
                enviromentUIActiveImage[index].enabled = true;
                enviromentUIActiveImage[index].color = color;
            }
            else rainEffect.SetActive(true);
        }
        fadeEnviroTime[index] = 1;
        enviroUIActive[index] = active;
        fadeEnviroUI[index] = true;
    }
   
    private void EnvironmentUIFade()
    {
        //if (!optionsSystem.environmentEffects) return;
        if (!fadeEnviroUI[0] && !fadeEnviroUI[1] && !fadeEnviroUI[2] && !fadeEnviroUI[3]) return;

        //LIGHTRAIN
        if (fadeEnviroUI[0])
        {
            fadeEnviroTime[0] -= time;
            fadeEnviroTime[0] = Mathf.Clamp01(fadeEnviroTime[0]);

            rainRenderer.material.SetFloat("_Distortion", -Mathf.Lerp(enviroUIActive[0] ? 5 : 0, enviroUIActive[0] ? 0 : 5, fadeEnviroTime[0]));
            if (fadeEnviroTime[0] == 0)
            {
                rainRenderer.material.SetFloat("_Size", -15);
                rainRenderer.material.SetFloat("_DropSizeX", 3);
                rainRenderer.material.SetFloat("_DropSizeY", 2);
                rainRenderer.material.SetFloat("_TrailSizeX", 2.1f);
                rainRenderer.material.SetFloat("_TrailSizeY", 0.5f);
                if (!enviroUIActive[0]) rainEffect.SetActive(false);
                fadeEnviroUI[0] = false;
            }
        }
        //HEAVYRAIN
        if (fadeEnviroUI[1])
        {
            fadeEnviroTime[1] -= time;
            fadeEnviroTime[1] = Mathf.Clamp01(fadeEnviroTime[1]);

            rainRenderer.material.SetFloat("_Distortion", -Mathf.Lerp(enviroUIActive[1] ? 5 : 0, enviroUIActive[1] ? 0 : 5, fadeEnviroTime[1]));
            if (fadeEnviroTime[1] == 0)
            {
                rainRenderer.material.SetFloat("_Size", -15);
                rainRenderer.material.SetFloat("_DropSizeX", 3);
                rainRenderer.material.SetFloat("_DropSizeY", 2);
                rainRenderer.material.SetFloat("_TrailSizeX", 2.1f);
                rainRenderer.material.SetFloat("_TrailSizeY", 0.5f);
                if (!enviroUIActive[1]) rainEffect.SetActive(false);
                fadeEnviroUI[1] = false;
            }
        }
        //HEATWAVE
        if (fadeEnviroUI[2])
        {
            fadeEnviroTime[2] -= time * 2;
            fadeEnviroTime[2] = Mathf.Clamp01(fadeEnviroTime[2]);
            enviromentUIActiveImage[2].color = new Color(enviromentUIActiveImage[2].color.r,
                                                         enviromentUIActiveImage[2].color.g,
                                                         enviromentUIActiveImage[2].color.b,
                                                           Mathf.Lerp(enviroUIActive[2] ? 1 : 0, enviroUIActive[2] ? 0 : 1, fadeEnviroTime[2]));
            if (fadeEnviroTime[2] == 0)
            {
                if (!enviroUIActive[2]) enviromentUIActiveImage[2].enabled = false;
                fadeEnviroUI[2] = false;
            }
        }
        //WATERFALL
        if (fadeEnviroUI[3])
        {
            fadeEnviroTime[3] -= time;
            fadeEnviroTime[3] = Mathf.Clamp01(fadeEnviroTime[3]);
            enviromentUIActiveImage[3].color = new Color(enviromentUIActiveImage[3].color.r,
                                                         enviromentUIActiveImage[3].color.g,
                                                         enviromentUIActiveImage[3].color.b,
                                                         Mathf.Lerp(enviroUIActive[3] ? 1 : 0, enviroUIActive[3] ? 0 : 1, fadeEnviroTime[3]));
            if (fadeEnviroTime[3] == 0)
            {
                if (!enviroUIActive[3]) enviromentUIActiveImage[3].enabled = false;
                fadeEnviroUI[3] = false;
            }
        }
    }
    public void Damage(int amount)
    {
        if (commandSystem.masterCodesActive[0]) return;
        if (!environmentSystem.environmentDamage && isDamaged) return;
        isDamaged = true;
        SetFlash(true);
        if (powerupSystem.powerEnabled[5]) return;
        inputSystem.DamageAnimation();
        inputSystem.SetScreenShakeEffect(Random.Range(1f, 2), 0.175f);
        if (!environmentSystem.isDrowning)
        {
            if (environmentSystem.environmentDamage)
            {
                if(environmentSystem.environmentSoundIndex == 0) audioSystem.PlayAudioSource(playerDmgSounds[Random.Range(0, playerDmgSounds.Length)], 1, 1, 128);
            }
            else audioSystem.PlayAudioSource(playerDmgSounds[Random.Range(0, playerDmgSounds.Length)], 1, 1, 128);
            if (armor > 0)
            {
                float intervalA = 0;
                float intervalH = 0;
                if (armor > 150) { intervalA = 1f; intervalH = 0.0f; }
                else if (armor <= 150 && armor > 50) { intervalA = 0.75f; intervalH = 0.25f; }
                else if (armor <= 50 && armor > 0) { intervalA = 0.50f; intervalH = 0.50f; }
                armor -= powerupSystem.powerEnabled[2] ? (amount * intervalA) / 2 : (amount * intervalA);
                health -= powerupSystem.powerEnabled[2] ? (amount * intervalH) / 2 : (amount * intervalH);
            }
            else if (armor < 1)
            {
                if (armorBannerObject[versionIndex].activeInHierarchy) armorBannerObject[versionIndex].SetActive(false);
                health -= powerupSystem.powerEnabled[2] ? amount / 2 : amount;
            }
        }
        else
        {
            health -= amount;
        }
        health = Mathf.Clamp(health, 0, maxHealth);
        armor = Mathf.Clamp(armor, 0, maxArmor);
        fallDamage = false;
        ApplyPlayerHealthAndArmor();
    }

    public void MutilatePlayer()
    {
        if (commandSystem.masterCodesActive[0]) return;
        goreExplode.transform.position = transform.position;
        goreExplode.SetActive(true);
    }
    public void SetupNewLevel()
    {
        messageSystem.EraseMessages();
        inputSystem.ResetInputSystem();
        weaponSystem.ResetWeaponSystem();
        powerupSystem.ResetPowerupSystem();
        UIVersion(versionIndex);
        HUD.SetActive(true);
        overKill = false;
        SetSigmaFlash(false);
        goreExplode.SetActive(false);
        if (flashSigmaUI.enabled) flashSigmaUI.enabled = false;
        //=======================================================
        characterController.enabled = false;
        fallDamage = false;
        transform.SetParent(originParent);
        gameSystem.SetPlayerScenePosition(gameSystem.sceneIndex);
        isDamaged = false;
        suicideDamage = false;
        SetFlash(false);
        optionsSystem.SetSceneMusic(gameSystem.sceneIndex);
        for (int kc = 0; kc < 3; kc++) SetActiveKey(kc, false);
        Vector3 headLoc = Vector3.zero;
        Vector3 headRot = Vector3.zero;
        Quaternion rot = Quaternion.Euler(headRot);
        head.localRotation = rot;
        head.localPosition = headLoc;
        if (versionIndex > 1 && versionIndex < 3)
        {
            if (lowHealthColor != hBanColor)
                lowHealthColor = hBanColor;
        }
        else if (versionIndex > 2)
        {
            if (lowHealthColor != visorColor)
                lowHealthColor = visorColor;
        }
        else
        {
            if (lowHealthColor != Color.white)
                lowHealthColor = Color.white;
        }
        for (int mb = 0; mb < 2; mb++)
        {
            hMaxBar[mb].fillAmount = 0;
        }
        hBanner[1].color = visorColor;
        SetKeyBannerColor(visorColor);
        if (versionIndex > 1 && versionIndex < 3)
            SetUIColor(hAmtText[versionIndex], hBar[versionID], hIcon[versionIndex], hColor);
        else if (versionIndex > 2)
            SetUIColor(hAmtText[versionIndex], hBar[versionID], hIcon[versionIndex], visorColor2);
        else SetUIColor(hAmtText[versionIndex], hBar[versionID], hIcon[versionIndex], Color.white);
        health = 100;
        SetUIColor(aAmtText[versionIndex], aBar[versionID], null, Color.clear);
        for (int a = 0; a < 4; a++)
            armorBannerObject[a].SetActive(false);
        armor = 0;
        maxArmor = 50;
        SetArmorSprites();
        for (int et = 0; et < 4; et++)
        {
            enviromentUIActiveImage[et].enabled = false;
            enviromentUIActiveImage[et].color = Color.white;
            fadeEnviroTime[et] = 1;
            enviroUIActive[et] = false;
        }
        environmentSystem.SetEnvironment(0, 0);
        environmentSystem.ActivateEnvironment(0);
        environmentSystem.ActiveEnvironmentUI(false);
        levelSystem = AccessLevel();
        if (levelSystem != null) levelSystem.ResetLevel();
        characterController.enabled = true;
        crosshair.enabled = true;
        ApplyPlayerHealthAndArmor();
        messageSystem.SetMessage(levelTitle[gameSystem.sceneIndex], MessageSystem.MessageType.Display);
    }
    public void SetupLevel()
    {
        messageSystem.EraseMessages();
        inputSystem.ResetInputSystem();
        powerupSystem.ResetPowerupSystem();
        UIVersion(versionIndex);
        HUD.SetActive(true);
        overKill = false;
        SetSigmaFlash(false);
        goreExplode.SetActive(false);
        if (flashSigmaUI.enabled) flashSigmaUI.enabled = false;
        characterController.enabled = false;
        fallDamage = false;
        transform.SetParent(originParent);
        gameSystem.SetPlayerScenePosition(gameSystem.sceneIndex);
        isDamaged = false;
        suicideDamage = false;
        SetFlash(false);
        optionsSystem.SetSceneMusic(gameSystem.sceneIndex);
        for (int kc = 0; kc < 3; kc++) SetActiveKey(kc, false);
        Vector3 headLoc = Vector3.zero;
        Vector3 headRot = Vector3.zero;
        Quaternion rot = Quaternion.Euler(headRot);
        head.localRotation = rot;
        head.localPosition = headLoc;
        SetKeyBannerColor(visorColor);
        for (int et = 0; et < 4; et++)
        {
            enviromentUIActiveImage[et].enabled = false;
            enviromentUIActiveImage[et].color = Color.white;
            fadeEnviroTime[et] = 1;
            enviroUIActive[et] = false;
        }
        environmentSystem.SetEnvironment(0, 0);
        environmentSystem.ActivateEnvironment(0);
        environmentSystem.ActiveEnvironmentUI(false);
        levelSystem = AccessLevel();
        if (levelSystem != null) levelSystem.ResetLevel();
        characterController.enabled = true;
        messageSystem.SetMessage(levelTitle[gameSystem.sceneIndex], MessageSystem.MessageType.Display);
    }
    public void ActivateLevelEnviromentSounds()
    {
        if (levelSystem == null)
            levelSystem = AccessLevel();

        levelSystem.ActivateEnvironment();
    }
    public void ResetPlayer()
    {
        if (!isDead) return;
        if (inputSystem.inputPlayer.GetButtonUp("Start") && isDead || inputSystem.inputPlayer.GetButtonUp("Select") && isDead)
        {
            messageSystem.EraseMessages();
            inputSystem.ResetInputSystem();
            weaponSystem.ResetWeaponSystem();
            powerupSystem.ResetPowerupSystem();
            for(int kc = 0; kc < 3; kc++) SetActiveKey(kc, false);
            UIVersion(versionIndex);
            HUD.SetActive(true);
            goreExplode.SetActive(false);
            //=======================================================
            characterController.enabled = false;
            fallDamage = false;
            overKill = false;
            SetSigmaFlash(false);
            if (flashSigmaUI.enabled) flashSigmaUI.enabled = false;
            transform.SetParent(originParent);
            gameSystem.SetPlayerScenePosition(gameSystem.sceneIndex);
            isDamaged = false;
            suicideDamage = false;
            SetFlash(false);
            audioSystem.PlayGameMusic(optionsSystem.musicClip, 1, 1, true);
            Vector3 headLoc = Vector3.zero;
            Vector3 headRot = Vector3.zero;
            Quaternion rot = Quaternion.Euler(headRot);
            head.localRotation = rot;
            head.localPosition = headLoc;
            if (versionIndex > 1 && versionIndex < 3)
            {
                if (lowHealthColor != hBanColor)
                    lowHealthColor = hBanColor;
            }
            else if (versionIndex > 2)
            {
                if (lowHealthColor != visorColor)
                    lowHealthColor = visorColor;
            }
            else
            {
                if (lowHealthColor != Color.white)
                    lowHealthColor = Color.white;
            }
            for (int mb = 0; mb < 2; mb++)
            {
                hMaxBar[mb].fillAmount = 0;
            }
            hBanner[1].color = visorColor;
            SetKeyBannerColor(visorColor);
            if (versionIndex > 1 && versionIndex < 3)
                SetUIColor(hAmtText[versionIndex], hBar[versionID], hIcon[versionIndex], hColor);
            else if (versionIndex > 2)
                SetUIColor(hAmtText[versionIndex], hBar[versionID], hIcon[versionIndex], visorColor2);
            else SetUIColor(hAmtText[versionIndex], hBar[versionID], hIcon[versionIndex], Color.white);
            health = 100;
            SetUIColor(aAmtText[versionIndex], aBar[versionID], null, Color.clear);
            for(int a = 0; a < 4; a++)
                armorBannerObject[a].SetActive(false);
            armor = 0;
            maxArmor = 50;
            SetArmorSprites();
            for (int et = 0; et < 4; et++)
            {
                enviromentUIActiveImage[et].enabled = false;
                enviromentUIActiveImage[et].color = Color.white;
                fadeEnviroTime[et] = 1;
                enviroUIActive[et] = false;
            }
            environmentSystem.SetEnvironment(0, 0);
            environmentSystem.ActivateEnvironment(0);
            environmentSystem.ActiveEnvironmentUI(false);
            levelSystem = AccessLevel();
            if (levelSystem != null) levelSystem.ResetLevel();
            characterController.enabled = true;
            crosshair.enabled = true;
            ApplyPlayerHealthAndArmor();
            messageSystem.SetMessage(levelTitle[gameSystem.sceneIndex], MessageSystem.MessageType.Display);
        }
        else if (transform.localPosition == gameSystem.scenePositions[gameSystem.sceneIndex] && isDead)
            isDead = false;
    }
    private void KillPlayer()
    {
        if (commandSystem.masterCodesActive[0]) return;
        if (overKill) { MutilatePlayer(); overKill = false; }
        isDead = true;
        HUD.SetActive(false);
        flashUI.enabled = false;
        crosshair.enabled = false;
        powerupSystem.ResetPowerupSystem();
        SetSigmaFlash(false);
        if (flashSigmaUI.enabled) flashSigmaUI.enabled = false;
        DisplayDeathMessage();
        weaponSystem.ShutOffMinigunSound();
        if (environmentSystem.environmentIndex == 0)
            audioSystem.PlayAudioSource(playerDiedSounds[0], 1, 1, 128);
        else
            audioSystem.PlayAudioSource(playerDiedSounds[1], 1, 1, 128);
        audioSystem.MusicPlayStop(false);
        Vector3 headLoc = new Vector3(head.localPosition.x, -8, 0);
        Vector3 headRot = new Vector3(head.localRotation.x, head.localRotation.y, -50);
        Quaternion rot = Quaternion.Euler(headRot);
        head.localRotation = rot;
        head.localPosition = headLoc;
        weaponSystem.weapons[weaponSystem.weaponIndex].SetActive(false);
        messageSystem.SetMessage("Press [Start] to Restart Level...", MessageSystem.MessageType.Center);
    }
    public void WarpPlayer(Vector3 destination, Quaternion rotation)
    {
        characterController.enabled = false;
        fallDamage = false;
        transform.SetParent(originParent);
        transform.position = destination;
        transform.rotation = rotation;
        environmentSystem.headUnderWater = false;
        environmentSystem.ActivateSwimming(false);
        environmentSystem.SetEnvironment(0, 0);
        environmentSystem.ActiveEnvironmentUI(false);
        Vector3 headLoc = Vector3.zero;
        Vector3 headRot = Vector3.zero;
        Quaternion rot = Quaternion.Euler(headRot);
        head.localRotation = rot;
        //head.localPosition = headLoc;
        characterController.enabled = true;


    }
    //========================================================================================//
    //====================================[UTILITY FUNCTIONS]=================================//
    //========================================================================================//
    private void SetUIColor(Text amtText, Image bar, Image icon, Color color)
    {
        if (amtText != null)
        {
            if (amtText.color != color)
                amtText.color = color;
        }
        if (bar != null)
        {
            if (bar.color != color)
                bar.color = color;
        }
        if (icon != null)
        {
            if (icon.color != color)
                icon.color = color;
        }
       
    }
    private void SetKeyBannerColor(Color color)
    {
        for (int kc = 0; kc < kIconBanners.Length; kc++)
        {
            if (kIconBanners[kc].color != color)
                kIconBanners[kc].color = color;
        }
    }
    public void ClearOutRenderTexture(RenderTexture renderTexture)
    {
        RenderTexture rt = RenderTexture.active;
        RenderTexture.active = renderTexture;
        GL.Clear(true, true, Color.clear);
        RenderTexture.active = rt;
    }
    private float Map(float value, float inMin, float inMax, float outMin, float outMax)
    {
        return (value - inMin) * (outMax - outMin) / (inMax - inMin) + outMin;
    }
    private string FormatValues(float value)
    {
        value = Mathf.FloorToInt(value);
        if (sb.Length > 0)
            sb.Clear();
        sb.Append(value);
        return sb.ToString();
    }
    private string BuildFixedMessages(string pickupName)
    {
        if (msgSb.Length > 0)
            msgSb.Clear();
        msgSb.Append("You " + pickupName);
        return msgSb.ToString();
    }
    public string BuildPersonalMessages(string pickupName)
    {
        if (msgSb.Length > 0)
            msgSb.Clear();
        msgSb.Append(pickupName);
        return msgSb.ToString();
    }
    //[Level Management]----------------------------------------------------------------------//
    public LevelSystem AccessLevel()
    {
        LevelSystem level = GameObject.Find("Scene").GetComponent<LevelSystem>();
        if (level != null) return level;
        else { Debug.Log("Level was not accessed. Level was not reset."); return null; }
    }
}