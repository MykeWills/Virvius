using UnityEngine;
using UnityEngine.UI;
using System.Text;
using Rewired;

public class CommandSystem : MonoBehaviour
{
    public static CommandSystem commandSystem;
    [SerializeField]
    private AudioClip[] inputSfx = new AudioClip[6];
    private GameSystem gameSystem;
    private OptionsSystem optionsSystem;
    private PlayerSystem playerSystem;
    private PowerupSystem powerupSystem;
    private WeaponSystem weaponSystem;
    private Vector2 startPosition = new Vector2(0, 560);
    private RectTransform commandWindowTransform;
    [SerializeField]
    private float windowSpeed = 150;
    private bool animateCommandWindow = false;
    private Player inputPlayer;
    [SerializeField]
    private Text textLabel;
    [SerializeField]
    private Text inputText;
    private StringBuilder[] sb = new StringBuilder[6]
    {
        new StringBuilder(),
        new StringBuilder(),
        new StringBuilder(),
        new StringBuilder(),
        new StringBuilder(),
        new StringBuilder()
    };
    [SerializeField]
    private GameObject commandWindow;
    [SerializeField]
    private InputField inputField;
    [SerializeField]
    private Text[] inputRecieved;
    private bool submit = false;
    public static bool commandOpen = false;
    private bool superUser = false;
    private bool superUserEnabled = false;
    [HideInInspector]
    public bool[] masterCodesActive = new bool[4];
  
    private string[] status = new string[2]
    {
        "[OFF]",
        "[ON]"
    };
    private string[] alphabet = new string[26] { "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z" };
    private string errorText = "_Invalid Command:/";
    private string[] errorMessages = new string[19]
    {
        "",
        " { Please Choose 1 [On] / 0 [OFF]. }",
        " { Please Choose Range From [0-2]. }",
        " { Please Choose Range From [0-3]. }",
        " { Please Choose Range From [0-4]. }",
        " { Please Choose Range From [0-5]. }",
        " { Please Choose Range From [0-6]. }",
        " { Please Choose Range From [0-7]. }",
        " { Please Choose Range From [0-8]. }",
        " { Please Choose Range From [0-9]. }",
        " { Please Choose Range From [0-10]. }",
        " { No valid weapon parameter exists. }",
        " { HDR not compatible with current monitor. }",
        " { Bloom is disabled, use [_bloom=1] to re-enable. }",
        " { Color Grading is disabled, use [_colorgrade=1] to re-enable. }",
        " { Please Choose Range From [0-100]. }",
        " { Please Choose Range From [0-11]. }",
        " { Please Choose Range From [1-100]. }",
        " { Please Choose Range From [60-120]. }"

    };
    private string[] cheatcodes = new string[71]
    {
        "_power=",
        "_weapon=",
        "_ammo=",
        "_health=",
        "_armor=",
        "_key=",
        "_difficulty=",
        "_autoaim=",
        "_autosave=",
        "_bobbing=",
        "_autoswitchnew=",
        "_autoswitchempty=",
        "_centering=",
        "_controller=",
        "_vibration=",
        "_inverty=",
        "_sensitivity_y=",
        "_sensitivity_x=",
        "_smoothing=",
        "_running=",
        "_aircontrol=",
        "_locky=",
        "_messages=",
        "_showtime=",
        "_showfps=",
        "_flash=",
        "_hud=",
        "_theme=",
        "_crosshair=",
        "_environment=",
        "_window=",
        "_quality=",
        "_resolution=",
        "_vsync=",
        "_hdr=",
        "_fov=",
        "_brightness=",
        "_bloom=",
        "_bloomval=",
        "_chromatic=",
        "_lens=",
        "_ambient=",
        "_colorgrade=",
        "_colormode=",
        "_colortone=",
        "_colorsat=",
        "_colorcon=",
        "_colorlift=",
        "_colorgamma=",
        "_colorgain=",
        "_grain=",
        "_grainval=",
        "_vignette=",
        "_vignetteval=",
        "_soundtrack=",
        "_volmaster=",
        "_volmusic=",
        "_volsound=",
        "_volenvironment=",
        "_volplayer=",
        "_speakermode=",
        "_virtualvoices=",
        "_realvoices=",
        "_samplerate=",
        "_buffer=",
        "_unlockvsuperiuservdebug?=",
        "_virgod=",
        "_virinf=",
        "_virinv=",
        "_virtur=",
        "_vir?master="

    };
    private string[] powerNames = new string[6]
    {
        "V-Damage",
        "Vanisher",
        "Berserker",
        "Punisher",
        "Reviver",
        "Divinity"
    };
    private string[] weaponNames = new string[10]
    {
        "Battle Sword",
        "Combat Shotgun",
        "Spiker Cannon",
        "Ultra Shotgun",
        "Revolt Minigun",
        "Gyro Grenade",
        "Horus Launcher",
        "Helix Railgun",
        "Photon Cannon",
        "M_Sigma X_800",
    };
    private string[] ammoNames = new string[10]
   {
        "",
        "Shotgun Shells",
        "Hardware Spikes",
        "Shotgun Shells",
        "Minigun Rounds",
        "Grenades",
        "Rockets",
        "Rail Slugs",
        "Photon Cells",
        "Signma Energy"
   };
    private string[] healthNames = new string[4]
    {
        "Health Vial",
        "Medical Package",
        "Medical Kit",
        "Medical Supplies"
    };
    private string[] armorNames = new string[4]
   {
        "Armor Shard",
        "Light Armor",
        "Combat Armor",
        "Ultra Armor"
   };
    private string[] keyNames = new string[3]
  {
        "Blue Card",
        "Yellow Card",
        "Red Card"
  };
    private string[] difficultyNames = new string[4]
{
        "Easy ",
        "Normal",
        "Hard",
        "Very Hard"
};
    private void Awake()
    {
        commandSystem = this;
    }
    void Start()
    {
        gameSystem = GameSystem.gameSystem;
        commandWindow = transform.GetChild(0).gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        if (!gameSystem.isGameStarted) return;
        AnimateWindow();

        if(Input.anyKeyDown && commandOpen)
        {
            for (int c = 0; c < alphabet.Length; c++)
            {
                if (Input.GetKeyDown(alphabet[c])) AudioSystem.audioSystem.PlayAudioSource(inputSfx[3], 1, 1, 128);
            }
        }
        if (Input.GetKeyDown(KeyCode.BackQuote) && !submit)
        {
          
            if (!commandOpen)
            {
                AudioSystem.audioSystem.PlayAudioSource(inputSfx[0], 1, 1, 128);
                commandOpen = true;
                animateCommandWindow = true;
                commandWindow.SetActive(true);
                inputField.text = null;
                ClearCommandWindow();
                inputField.ActivateInputField();
                gameSystem.GameMouseActive(true, CursorLockMode.Confined);
                submit = true;
            }
            else if (commandOpen)
            {
                AudioSystem.audioSystem.PlayAudioSource(inputSfx[5], 1, 1, 128);
                commandOpen = false;
                animateCommandWindow = true;
                if (!gameSystem.isPaused) gameSystem.GameMouseActive(false, CursorLockMode.Locked);
            }
        }
       
        if (Input.GetKeyUp(KeyCode.BackQuote) || Input.GetKeyUp(KeyCode.Return)) submit = false;
        if (!commandOpen) return;
        if (Input.GetKeyDown(KeyCode.Return) && !submit)
        {
            if (inputField.text == null) return;
            SubmitInput();
            submit = true;
        }
        if (Input.GetKeyDown(KeyCode.F12) && !superUserEnabled)
        {
            superUserEnabled = true;
            inputField.caretColor = Color.cyan;
            inputText.color = Color.blue;
        }
        else if (Input.GetKeyDown(KeyCode.F12) && superUserEnabled)
        {
            superUserEnabled = false;
            inputField.caretColor = Color.white;
            inputText.color = Color.white;
        }
        if (commandOpen && !inputField.isFocused)
            inputField.ActivateInputField();
        
    }
    private void AnimateWindow()
    {
        if (!animateCommandWindow) return;
        if (commandWindowTransform == null) commandWindowTransform = commandWindow.GetComponent<RectTransform>();
        commandWindowTransform.anchoredPosition = Vector3.MoveTowards(commandWindowTransform.anchoredPosition, commandOpen ? Vector2.zero : startPosition, Time.unscaledDeltaTime * windowSpeed);
        if (commandWindowTransform.anchoredPosition == Vector2.zero)
        {
            animateCommandWindow = false;
        }
        else if (commandWindowTransform.anchoredPosition == startPosition)
        {
            commandWindow.SetActive(false);
            animateCommandWindow = false;
        }
    }
    private bool ValidInput()
    {
        string curInput = inputField.text.ToLower();
        sb[0].Clear();
        for (int i = 0; i < cheatcodes.Length; i++)
        {
            if (curInput.Contains(cheatcodes[i]))
            {
                for (int c = 0; c < curInput.Length; c++)
                {
                    if (curInput[c] == '_')
                        curInput = curInput.Remove(0, c);
                }
                sb[0].Append(curInput);
                return true;
            }
        }
        SendCommandError(0);
        return false;
       
    }
    private void SendCommandError(int errorID)
    {
        AudioSystem.audioSystem.PlayAudioSource(inputSfx[4], 1, 1, 128);
        if (textLabel.color != Color.white) textLabel.color = Color.white;
        for (int s = 0; s < sb.Length; s++)
            if (sb[s].Length > 0) sb[s].Clear();
        sb[0].Append(errorText + inputField.text.ToLower() + errorMessages[errorID]);
        ShiftRecievedInput(false);
        inputField.text = null;
        inputField.ActivateInputField();
    }
    public void SubmitInput()
    {
        if(playerSystem == null) playerSystem = PlayerSystem.playerSystem;
        if(powerupSystem == null) powerupSystem = PowerupSystem.powerupSystem;
        if(weaponSystem == null) weaponSystem = WeaponSystem.weaponSystem;
        if (optionsSystem == null) optionsSystem = OptionsSystem.optionsSystem;
        if(inputPlayer == null) inputPlayer = ReInput.players.GetPlayer(0);
        if(textLabel.color != Color.white) textLabel.color = Color.white;
        //if input contains any of the cheat codes
        if (!ValidInput()) return;
        //split the measures from [NAME]/[VALUE] 
        string[] input = sb[0].ToString().Split('=');
        //now that input container stored input values clear all containers
        for (int s = 0; s < sb.Length; s++)
            if (sb[s].Length > 0) sb[s].Clear();
        //set the first container to store the [NAME]
        sb[0].Append(input[0]);
        //set the third contrainer to store the [VALUE]
        sb[2].Append(input[1]);
        //set the fourth container to the first piece of the final output message
        sb[3].Append(sb[0].ToString() + "=" + sb[2].ToString());

        //switch to which [NAME] is in the first container
        if (textLabel.color != Color.green) textLabel.color = Color.green;
        switch (sb[0].ToString())
        {
            //###############################################
            //################# Master Codes ################
            //###############################################
            case "_unlockvsuperiuservdebug?":
                {
                    
                    if (!superUserEnabled) { SendCommandError(0); return; }
                    //convert [VALUE] string to an integer
                    int? valConversion = StringConversionChecker(sb[2].ToString(), 1);
                    if (valConversion == null) return;
                    int val = valConversion.Value;
                    //set the second container to store the final output message to user
                    sb[1].Append(sb[3].ToString() + " [Debug <|V|>  Mode: " + status[val] + " ]");
                    //apply cheatcode parameter
                    superUser = (val == 1) ? true : false;
                    textLabel.color = Color.yellow;
                    AudioSystem.audioSystem.PlayAudioSource(inputSfx[2], 1, 1, 128);
                    break;
                }
            case "_virgod":
                {
                    if (!superUser) { SendCommandError(0); return; }
                    //convert [VALUE] string to an integer
                    int? valConversion = StringConversionChecker(sb[2].ToString(), 1);
                    if (valConversion == null) return;
                    int val = valConversion.Value;
                    //set the second container to store the final output message to user
                    sb[1].Append(sb[3].ToString() + " [God <|V|> Mode: " + status[val] + " ]");
                    //apply cheatcode parameter
                    masterCodesActive[0] = (val == 1) ? true : false;
                    textLabel.color = Color.red;
                    AudioSystem.audioSystem.PlayAudioSource(inputSfx[2], 1, 1, 128);
                    break;
                }
            case "_virinf":
                {
                    if (!superUser) { SendCommandError(0); return; }
                    //convert [VALUE] string to an integer
                    int? valConversion = StringConversionChecker(sb[2].ToString(), 1);
                    if (valConversion == null) return;
                    int val = valConversion.Value;
                    //set the second container to store the final output message to user
                    sb[1].Append(sb[3].ToString() + " [Infinite Ammo <|V|> Mode: " + status[val] + " ]");
                    //apply cheatcode parameter
                    masterCodesActive[1] = (val == 1) ? true : false;
                    textLabel.color = Color.red;
                    AudioSystem.audioSystem.PlayAudioSource(inputSfx[2], 1, 1, 128);
                    break;
                }
            case "_virinv":
                {
                    if (!superUser) { SendCommandError(0); return; }
                    //convert [VALUE] string to an integer
                    int? valConversion = StringConversionChecker(sb[2].ToString(), 1);
                    if (valConversion == null) return;
                    int val = valConversion.Value;
                    //set the second container to store the final output message to user
                    sb[1].Append(sb[3].ToString() + " [Invisibility <|V|> Mode: " + status[val] + " ]");
                    //apply cheatcode parameter
                    masterCodesActive[2] = (val == 1) ? true : false;
                    textLabel.color = Color.red;
                    AudioSystem.audioSystem.PlayAudioSource(inputSfx[2], 1, 1, 128);
                    break;
                }
            case "_virtur":
                {
                    if (!superUser) { SendCommandError(0); return; }
                    //convert [VALUE] string to an integer
                    int? valConversion = StringConversionChecker(sb[2].ToString(), 1);
                    if (valConversion == null) return;
                    int val = valConversion.Value;
                    //set the second container to store the final output message to user
                    sb[1].Append(sb[3].ToString() + " [Turbo <|V|> Mode: " + status[val] + " ]");
                    //apply cheatcode parameter
                    masterCodesActive[3] = (val == 1) ? true : false;
                    textLabel.color = Color.red;
                    AudioSystem.audioSystem.PlayAudioSource(inputSfx[2], 1, 1, 128);
                    break;
                }
            case "_vir?master":
                {
                    if (!superUser) { SendCommandError(0); return; }
                    //convert [VALUE] string to an integer
                    int? valConversion = StringConversionChecker(sb[2].ToString(), 1);
                    if (valConversion == null) return;
                    int val = valConversion.Value;
                    //set the second container to store the final output message to user
                    sb[1].Append(sb[3].ToString() + " [Master <|V|> Mode: " + status[val] + " ]");
                    //apply cheatcode parameter
                    //All master codes
                    for (int mc = 0; mc < masterCodesActive.Length; mc++)
                        masterCodesActive[mc] = (val == 1) ? true : false;
                    //all keys
                    for (int k = 0; k < 3; k++)
                        playerSystem.GiveKey(k);
                    //full armor
                    playerSystem.ObtainArmor(200, 200);
                    //full health
                    playerSystem.RecoverHealth(100, true);
                    //full ammo
                    for (int a = 1; a < 10; a++) weaponSystem.GetAmmo(a, weaponSystem.weaponMaxAmmo[a]);
                    //all guns
                    for (int w = 0; w < 10; w++) weaponSystem.weaponObtained[w] = true;
                    weaponSystem.AutoSelectWeapon(8);
                    textLabel.color = Color.red;
                    AudioSystem.audioSystem.PlayAudioSource(inputSfx[2], 1, 1, 128);
                    break;
                }
            //###############################################
            //################# Player Codes ################
            //###############################################
            case "_power":
                {
                    //convert [VALUE] string to an integer
                    int? valConversion = StringConversionChecker(sb[2].ToString(), powerNames.Length - 1);
                    if (valConversion == null) return; 
                    int val = valConversion.Value;
                    //set the second container to store the final output message to user
                    sb[1].Append(sb[3].ToString() + " [" + powerNames[val] + " Enabled]");
                    //apply cheatcode parameter
                    powerupSystem.Cheat(val);
                    AudioSystem.audioSystem.PlayAudioSource(inputSfx[1], 1, 1, 128);
                    break;
                }
            case "_weapon":
                {
                    if (sb[2].ToString() == "all")
                    {   
                        //set the second container to store the final output message to user
                        sb[1].Append(sb[3].ToString() + " [All Weapons Enabled]");
                        //apply cheatcode parameter
                        for (int w = 0; w < 10; w++) weaponSystem.weaponObtained[w] = true;
                        weaponSystem.AutoSelectWeapon(9);
                        AudioSystem.audioSystem.PlayAudioSource(inputSfx[1], 1, 1, 128);
                    }
                    else
                    {
                        //convert [VALUE] string to an integer
                        int? valConversion = StringConversionChecker(sb[2].ToString(), weaponNames.Length - 1);
                        if (valConversion == null) return; 
                        int val = valConversion.Value;
                        //set the second container to store the final output message to user
                        sb[1].Append(sb[3].ToString() + " [" + weaponNames[val] + " Enabled]");
                        //apply cheatcode parameter
                        if (!weaponSystem.weaponObtained[val]) weaponSystem.weaponObtained[val] = true;
                        weaponSystem.AutoSelectWeapon(val);
                        AudioSystem.audioSystem.PlayAudioSource(inputSfx[1], 1, 1, 128);
                    }
                    break;
                }
            case "_ammo":
                {
                    //ALLAMMO
                    if (sb[2].ToString() == "all")
                    {
                        //set the second container to store the final output message to user
                        sb[1].Append(sb[3].ToString() + " [All Ammo Enabled]");
                        //apply cheatcode parameter
                        for (int a = 1; a < 10; a++) weaponSystem.GetAmmo(a, weaponSystem.defaultWeaponAmmo[a]);
                        AudioSystem.audioSystem.PlayAudioSource(inputSfx[1], 1, 1, 128);
                    }
                    //MAXAMMO
                    else if (sb[2].ToString() == "max")
                    { 
                        //set the second container to store the final output message to user
                        sb[1].Append(sb[3].ToString() + " [Max Ammo Enabled]");
                        //apply cheatcode parameter
                        for (int a = 1; a < 10; a++) weaponSystem.GetAmmo(a, weaponSystem.weaponMaxAmmo[a]);
                        AudioSystem.audioSystem.PlayAudioSource(inputSfx[1], 1, 1, 128);
                    }
                    //INDIVIDUALAMMO
                    else
                    {
                        //convert [VALUE] string to an integer
                        int? valConversion = StringConversionChecker(sb[2].ToString(), ammoNames.Length - 1);
                        if (valConversion == null) return; 
                        int val = valConversion.Value;
                        //temporary until all weapons are designed...
                        if (val == 0) { SendCommandError(11); return; }
                        else
                        {
                            //set the second container to store the final output message to user
                            sb[1].Append(sb[3].ToString() + " [" + ammoNames[val] + " +" + weaponSystem.defaultWeaponAmmo[val] + "]");
                            //apply cheatcode parameter
                            weaponSystem.GetAmmo(val, weaponSystem.defaultWeaponAmmo[val]);
                            AudioSystem.audioSystem.PlayAudioSource(inputSfx[1], 1, 1, 128);
                            break;
                        }
                    }
                    break;
                }
            case "_health":
                {
                    //convert [VALUE] string to an integer
                    int? valConversion = StringConversionChecker(sb[2].ToString(), healthNames.Length - 1);
                    if (valConversion == null) return; 
                    int val = valConversion.Value;
                    //set the default health values & if health can continue after 100 health
                    int healthAmt = 0;
                    bool overHealth = false;
                    switch (val)
                    {
                        case 0: healthAmt = 2; overHealth = true; break;
                        case 1: healthAmt = 15; overHealth = false; break;
                        case 2: healthAmt = 25; overHealth = false; break;
                        case 3: healthAmt = 100; overHealth = true; break;
                    }
                    //set the second container to store the final output message to user
                    sb[1].Append(sb[3].ToString() + " [" + healthNames[val] + " +" + healthAmt + " Health]");
                    //apply cheatcode parameter
                    playerSystem.RecoverHealth(healthAmt, overHealth);
                    AudioSystem.audioSystem.PlayAudioSource(inputSfx[1], 1, 1, 128);
                    break;
                }
            case "_armor":
                {
                    //convert [VALUE] string to an integer
                    int? valConversion = StringConversionChecker(sb[2].ToString(), armorNames.Length - 1);
                    if (valConversion == null) return; 
                    int val = valConversion.Value;
                    //set the default armor values & armor max values
                    int armorAmt = 0;
                    int armorLimit = 0;
                    switch (val)
                    {
                        case 0: armorAmt = 2; armorLimit = playerSystem.maxArmor; break;
                        case 1: armorAmt = 50; armorLimit = 50; break;
                        case 2: armorAmt = 150; armorLimit = 150; break;
                        case 3: armorAmt = 200; armorLimit = 200; break;
                    }
                    //set the second container to store the final output message to user
                    sb[1].Append(sb[3].ToString() + " [" + armorNames[val] + " +" + armorAmt + " Armor]");
                    //apply cheatcode parameter
                    playerSystem.ObtainArmor(armorAmt, armorLimit);
                    AudioSystem.audioSystem.PlayAudioSource(inputSfx[1], 1, 1, 128);
                    break;
                }
            case "_key":
                {
                    //ALLKEYS
                    if (sb[2].ToString() == "all")
                    {
                        //set the second container to store the final output message to user
                        sb[1].Append(sb[3].ToString() + " [All keys Obtained]");
                        //apply cheatcode parameter
                        for (int k = 0; k < 3; k++)
                            playerSystem.GiveKey(k);
                        AudioSystem.audioSystem.PlayAudioSource(inputSfx[1], 1, 1, 128);
                    }
                    else if (sb[2].ToString() == "none")
                    {
                        //set the second container to store the final output message to user
                        sb[1].Append(sb[3].ToString() + " [All keys Removed]");
                        //apply cheatcode parameter
                        for (int k = 0; k < 3; k++)
                            playerSystem.RemoveKey(k);
                        AudioSystem.audioSystem.PlayAudioSource(inputSfx[1], 1, 1, 128);
                    }
                    //INDIVIUALKEYS
                    else
                    {
                        //convert [VALUE] string to an integer
                        int? valConversion = StringConversionChecker(sb[2].ToString(), keyNames.Length - 1);
                        if (valConversion == null) return; 
                        int val = valConversion.Value;
                        //set the second container to store the final output message to user
                        sb[1].Append(sb[3].ToString() + " [" + keyNames[val] + " Obtained]");
                        //apply cheatcode parameter
                        playerSystem.GiveKey(val);
                        AudioSystem.audioSystem.PlayAudioSource(inputSfx[1], 1, 1, 128);
                    }
                    break;
                }
            //###############################################
            //################# Option Codes ################
            //###############################################
            //-----------------------------------GAMESETTINGS
            case "_difficulty":
                {
                    //convert [VALUE] string to an integer
                    int? valConversion = StringConversionChecker(sb[2].ToString(), difficultyNames.Length - 1);
                    if (valConversion == null) return; 
                    int val = valConversion.Value;
                    //set the second container to store the final output message to user
                    sb[1].Append(sb[3].ToString() + " [" + difficultyNames[val] + " Difficulty Selected]");
                    //apply cheatcode parameter
                    optionsSystem.difficultyIndex = val;
                    optionsSystem.SetDifficulty();
                    AudioSystem.audioSystem.PlayAudioSource(inputSfx[1], 1, 1, 128);
                    break;
                }
            case "_autoaim":
                {
                    //convert [VALUE] string to an integer
                    int? valConversion = StringConversionChecker(sb[2].ToString(), 1);
                    if (valConversion == null) return; 
                    int val = valConversion.Value;
                    //set the second container to store the final output message to user
                    sb[1].Append(sb[3].ToString() + " [AutoAim: " + status[val] +"]");
                    //apply cheatcode parameter
                    optionsSystem.autoAIndex = val;
                    optionsSystem.SetAutoAim();
                    AudioSystem.audioSystem.PlayAudioSource(inputSfx[1], 1, 1, 128);
                    break;
                }
            case "_autosave":
                {
                    //convert [VALUE] string to an integer
                    int? valConversion = StringConversionChecker(sb[2].ToString(), 1);
                    if (valConversion == null) return; 
                    int val = valConversion.Value;
                    //set the second container to store the final output message to user
                    sb[1].Append(sb[3].ToString() + " [AutoSave: " + status[val] + "]");
                    //apply cheatcode parameter
                    optionsSystem.autoSIndex = val;
                    optionsSystem.SetAutoSave();
                    AudioSystem.audioSystem.PlayAudioSource(inputSfx[1], 1, 1, 128);
                    break;
                }
            case "_bobbing":
                {
                    //convert [VALUE] string to an integer
                    int? valConversion = StringConversionChecker(sb[2].ToString(), 1);
                    if (valConversion == null) return; 
                    int val = valConversion.Value;
                    //set the second container to store the final output message to user
                    sb[1].Append(sb[3].ToString() + " [Camera Bobbing: " + status[val] + "]");
                    //apply cheatcode parameter
                    optionsSystem.cameraBIndex = val;
                    optionsSystem.SetCameraBobbing();
                    AudioSystem.audioSystem.PlayAudioSource(inputSfx[1], 1, 1, 128);
                    break;
                }
            case "_autoswitchnew":
                {
                    //convert [VALUE] string to an integer
                    int? valConversion = StringConversionChecker(sb[2].ToString(), 1);
                    if (valConversion == null) return; 
                    int val = valConversion.Value;
                    //set the second container to store the final output message to user
                    sb[1].Append(sb[3].ToString() + " [Auto Switch Weapon [New]: " + status[val] + "]");
                    //apply cheatcode parameter
                    optionsSystem.switchNewIndex = val;
                    optionsSystem.SetAutoSwitchNew();
                    AudioSystem.audioSystem.PlayAudioSource(inputSfx[1], 1, 1, 128);
                    break;
                }
            case "_autoswitchempty":
                {
                    //convert [VALUE] string to an integer
                    int? valConversion = StringConversionChecker(sb[2].ToString(), 1);
                    if (valConversion == null) return; 
                    int val = valConversion.Value;
                    //set the second container to store the final output message to user
                    sb[1].Append(sb[3].ToString() + " [Auto Switch Weapon [Empty]: " + status[val] + "]");
                    //apply cheatcode parameter
                    optionsSystem.switchEmptyIndex = val;
                    optionsSystem.SetAutoSwitchEmpty();
                    AudioSystem.audioSystem.PlayAudioSource(inputSfx[1], 1, 1, 128);
                    break;
                }
            case "_centering":
                {
                    //convert [VALUE] string to an integer
                    int? valConversion = StringConversionChecker(sb[2].ToString(), 1);
                    if (valConversion == null) return; 
                    int val = valConversion.Value;
                    //set the second container to store the final output message to user
                    sb[1].Append(sb[3].ToString() + " [Weapon Auto Centering: " + status[val] + "]");
                    //apply cheatcode parameter
                    optionsSystem.weaponMoveIndex = val;
                    optionsSystem.SetWeaponMovement();
                    AudioSystem.audioSystem.PlayAudioSource(inputSfx[1], 1, 1, 128);
                    break;
                }
            //--------------------------------CONTROLSETTINGS
            case "_controller":
                {
                    //convert [VALUE] string to an integer
                    int? valConversion = StringConversionChecker(sb[2].ToString(), 1);
                    if (valConversion == null) return; 
                    int val = valConversion.Value;
                    //send a message to the user if the controller isnt connected
                    if (inputPlayer.controllers.Joysticks.Count < 1) { sb[1].Append(sb[3].ToString() + " [Controller/Gamepad [Not] Connected]"); }
                    else
                    {
                        //set the second container to store the final output message to user
                        sb[1].Append(sb[3].ToString() + " [Controller/Gamepad: " + status[val] + "]");
                        //apply cheatcode parameter
                        optionsSystem.controllerIndex = val;
                        optionsSystem.SetControllerActive();
                        AudioSystem.audioSystem.PlayAudioSource(inputSfx[1], 1, 1, 128);
                    }
                    break;
                }
            case "_vibration":
                {
                    //convert [VALUE] string to an integer
                    int? valConversion = StringConversionChecker(sb[2].ToString(), 1);
                    if (valConversion == null) return; 
                    int val = valConversion.Value;
                    //send a message to the user if the controller isnt connected
                    if (inputPlayer.controllers.Joysticks.Count < 1) { sb[1].Append(sb[3].ToString() + " [Controller/Gamepad [Not] Connected]"); }
                    else
                    {
                        //set the second container to store the final output message to user
                        sb[1].Append(sb[3].ToString() + " [Controller/Gamepad Vibration: " + status[val] + "]");
                        //apply cheatcode parameter
                        optionsSystem.vibrationIndex = val;
                        optionsSystem.SetVibrationActive();
                        AudioSystem.audioSystem.PlayAudioSource(inputSfx[1], 1, 1, 128);
                    }
                    break;
                }
            case "_inverty":
                {
                    //convert [VALUE] string to an integer
                    int? valConversion = StringConversionChecker(sb[2].ToString(), 1);
                    if (valConversion == null) return; 
                    int val = valConversion.Value;
                    //set the second container to store the final output message to user
                    sb[1].Append(sb[3].ToString() + " [Invert Y Axis: " + status[val] + "]");
                    //apply cheatcode parameter
                    optionsSystem.invertIndex = val;
                    optionsSystem.SetInvertActive();
                    AudioSystem.audioSystem.PlayAudioSource(inputSfx[1], 1, 1, 128);
                    break;
                }
            case "_sensitivity_y":
                {
                    //convert [VALUE] string to an integer
                    int? valConversion = StringConversionChecker(sb[2].ToString(), 10);
                    if (valConversion == null) return; 
                    int val = valConversion.Value;
                    //set the second container to store the final output message to user
                    sb[1].Append(sb[3].ToString() + " [Y Axis Sensitivity: " + sb[2].ToString() + "]");
                    //apply cheatcode parameter
                    optionsSystem.sensitivity[1] = val;
                    optionsSystem.SetSensitivityY();
                    AudioSystem.audioSystem.PlayAudioSource(inputSfx[1], 1, 1, 128);
                    break;
                }
            case "_sensitivity_x":
                {
                    //convert [VALUE] string to an integer
                    int? valConversion = StringConversionChecker(sb[2].ToString(), 10);
                    if (valConversion == null) return; 
                    int val = valConversion.Value;
                    //set the second container to store the final output message to user
                    sb[1].Append(sb[3].ToString() + " [X Axis Sensitivity: " + sb[2].ToString() + "]");
                    //apply cheatcode parameter
                    optionsSystem.sensitivity[0] = val;
                    optionsSystem.SetSensitivityX();
                    AudioSystem.audioSystem.PlayAudioSource(inputSfx[1], 1, 1, 128);
                    break;
                }
            case "_smoothing":
                {
                    //convert [VALUE] string to an integer
                    int? valConversion = StringConversionChecker(sb[2].ToString(), 1);
                    if (valConversion == null) return; 
                    int val = valConversion.Value;
                    //set the second container to store the final output message to user
                    sb[1].Append(sb[3].ToString() + " [Smooth Rotation: " + status[val] + "]");
                    //apply cheatcode parameter
                    optionsSystem.smoothIndex = val;
                    optionsSystem.SetSmoothActive();
                    AudioSystem.audioSystem.PlayAudioSource(inputSfx[1], 1, 1, 128);
                    break;
                }
            case "_running":
                {
                    //convert [VALUE] string to an integer
                    int? valConversion = StringConversionChecker(sb[2].ToString(), 1);
                    if (valConversion == null) return; 
                    int val = valConversion.Value;
                    //set the second container to store the final output message to user
                    sb[1].Append(sb[3].ToString() + " [Always Run: " + status[val] + "]");
                    //apply cheatcode parameter
                    optionsSystem.runIndex = val;
                    optionsSystem.SetRunActive();
                    AudioSystem.audioSystem.PlayAudioSource(inputSfx[1], 1, 1, 128);
                    break;
                }
            case "_aircontrol":
                {
                    //convert [VALUE] string to an integer
                    int? valConversion = StringConversionChecker(sb[2].ToString(), 1);
                    if (valConversion == null) return; 
                    int val = valConversion.Value;
                    //set the second container to store the final output message to user
                    sb[1].Append(sb[3].ToString() + " [Air Control: " + status[val] + "]");
                    //apply cheatcode parameter
                    optionsSystem.airIndex = val;
                    optionsSystem.SetAirControlActive();
                    AudioSystem.audioSystem.PlayAudioSource(inputSfx[1], 1, 1, 128);
                    break;
                }
            case "_locky":
                {
                    //convert [VALUE] string to an integer
                    int? valConversion = StringConversionChecker(sb[2].ToString(), 1);
                    if (valConversion == null) return; 
                    int val = valConversion.Value;
                    //set the second container to store the final output message to user
                    sb[1].Append(sb[3].ToString() + " [Lock Y Axis: " + status[val] + "]");
                    //apply cheatcode parameter
                    optionsSystem.lockYIndex = val;
                    optionsSystem.SetLockYActive();
                    AudioSystem.audioSystem.PlayAudioSource(inputSfx[1], 1, 1, 128);
                    break;
                }
            //-------------------------------------UISETTINGS
            case "_messages":
                {
                    //convert [VALUE] string to an integer
                    int? valConversion = StringConversionChecker(sb[2].ToString(), 1);
                    if (valConversion == null) return; 
                    int val = valConversion.Value;
                    //set the second container to store the final output message to user
                    sb[1].Append(sb[3].ToString() + " [Show Messages: " + status[val] + "]");
                    //apply cheatcode parameter
                    optionsSystem.messagesActiveIndex = val;
                    optionsSystem.SetMessagesActive();
                    AudioSystem.audioSystem.PlayAudioSource(inputSfx[1], 1, 1, 128);
                    break;
                }
            case "_showtime":
                {
                    //convert [VALUE] string to an integer
                    int? valConversion = StringConversionChecker(sb[2].ToString(), 1);
                    if (valConversion == null) return; 
                    int val = valConversion.Value;
                    //set the second container to store the final output message to user
                    sb[1].Append(sb[3].ToString() + " [Show Date & Time: " + status[val] + "]");
                    //apply cheatcode parameter
                    optionsSystem.timeActiveIndex = val;
                    optionsSystem.SetTimeActive();
                    AudioSystem.audioSystem.PlayAudioSource(inputSfx[1], 1, 1, 128);
                    break;
                }
            case "_showfps":
                {
                    //convert [VALUE] string to an integer
                    int? valConversion = StringConversionChecker(sb[2].ToString(), 1);
                    if (valConversion == null) return; 
                    int val = valConversion.Value;
                    //set the second container to store the final output message to user
                    sb[1].Append(sb[3].ToString() + " [Show Frames Per Second: " + status[val] + "]");
                    //apply cheatcode parameter
                    optionsSystem.fpsActiveIndex = val;
                    optionsSystem.SetFPSActive();
                    AudioSystem.audioSystem.PlayAudioSource(inputSfx[1], 1, 1, 128);
                    break;
                }
            case "_flash":
                {
                    //convert [VALUE] string to an integer
                    int? valConversion = StringConversionChecker(sb[2].ToString(), 1);
                    if (valConversion == null) return; 
                    int val = valConversion.Value;
                    //set the second container to store the final output message to user
                    sb[1].Append(sb[3].ToString() + " [Flash Effects: " + status[val] + "]");
                    //apply cheatcode parameter
                    optionsSystem.flashIndex = val;
                    optionsSystem.SetFlashEffectsActive();
                    AudioSystem.audioSystem.PlayAudioSource(inputSfx[1], 1, 1, 128);
                    break;
                }
            case "_hud":
                {
                    //convert [VALUE] string to an integer
                    int? valConversion = StringConversionChecker(sb[2].ToString(), optionsSystem.hudSubText.Length - 1);
                    if (valConversion == null) return; 
                    int val = valConversion.Value;
                    //set the second container to store the final output message to user
                    sb[1].Append(sb[3].ToString() + " [" + optionsSystem.hudSubText[val] + " Screen Mode Selected]");
                    //apply cheatcode parameter
                    optionsSystem.hudIndex = val;
                    optionsSystem.SetHUD();
                    AudioSystem.audioSystem.PlayAudioSource(inputSfx[1], 1, 1, 128);
                    break;
                }
            case "_theme":
                {
                    //convert [VALUE] string to an integer
                    int? valConversion = StringConversionChecker(sb[2].ToString(), optionsSystem.VisorColorSubText.Length - 1);
                    if (valConversion == null) return; 
                    int val = valConversion.Value;
                    //set the second container to store the final output message to user
                    sb[1].Append(sb[3].ToString() + " [" + optionsSystem.VisorColorSubText[val] + " Visor Theme Selected]");
                    //apply cheatcode parameter
                    optionsSystem.visorColorIndex = val;
                    optionsSystem.SetVisorColor();
                    AudioSystem.audioSystem.PlayAudioSource(inputSfx[1], 1, 1, 128);
                    break;
                }
            case "_crosshair":
                {
                    //convert [VALUE] string to an integer
                    int? valConversion = StringConversionChecker(sb[2].ToString(), optionsSystem.crosshairSubText.Length - 1);
                    if (valConversion == null) return; 
                    int val = valConversion.Value;
                    //set the second container to store the final output message to user
                    sb[1].Append(sb[3].ToString() + " [" + optionsSystem.crosshairSubText[val] + " Crosshair Selected]");
                    //apply cheatcode parameter
                    optionsSystem.crosshairIndex = val;
                    optionsSystem.SetCrosshair();
                    AudioSystem.audioSystem.PlayAudioSource(inputSfx[1], 1, 1, 128);
                    break;
                }
            case "_environment":
                {
                    //convert [VALUE] string to an integer
                    int? valConversion = StringConversionChecker(sb[2].ToString(), 1);
                    if (valConversion == null) return; 
                    int val = valConversion.Value;
                    //set the second container to store the final output message to user
                    sb[1].Append(sb[3].ToString() + " [Environment Effects: " + status[val] + "]");
                    //apply cheatcode parameter
                    optionsSystem.environmentIndex = val;
                    optionsSystem.SetEnvironmentEffectsActive();
                    AudioSystem.audioSystem.PlayAudioSource(inputSfx[1], 1, 1, 128);
                    break;
                }
            //----------------------------------VIDEOSETTINGS
            case "_window":
                {
                    //convert [VALUE] string to an integer
                    int? valConversion = StringConversionChecker(sb[2].ToString(), optionsSystem.fullScreenSubText.Length - 1);
                    if (valConversion == null) return; 
                    int val = valConversion.Value;
                    //set the second container to store the final output message to user
                    sb[1].Append(sb[3].ToString() + " [" + optionsSystem.fullScreenSubText[val] + " Selected]");
                    //apply cheatcode parameter
                    optionsSystem.fullScreenIndex = val;
                    optionsSystem.SetWindowedScreen();
                    AudioSystem.audioSystem.PlayAudioSource(inputSfx[1], 1, 1, 128);
                    break;
                }
            case "_quality":
                {
                    //convert [VALUE] string to an integer
                    int? valConversion = StringConversionChecker(sb[2].ToString(), optionsSystem.qualitySubText.Length - 1);
                    if (valConversion == null) return; 
                    int val = valConversion.Value;
                    //set the second container to store the final output message to user
                    sb[1].Append(sb[3].ToString() + " [" + optionsSystem.qualitySubText[val] +" Quality Selected]");
                    //apply cheatcode parameter
                    optionsSystem.qualityIndex = val;
                    optionsSystem.SetQuality();
                    AudioSystem.audioSystem.PlayAudioSource(inputSfx[1], 1, 1, 128);
                    break;
                }
            case "_resolution":
                {
                    //check if the X character exists
                    if (!sb[2].ToString().Contains("x")) { Debug.Log("Doesn't Contain X"); SendCommandError(0); return; }
                    //find the X character
                    for (int x = 0; x < sb[2].Length; x++)
                    {
                        if (sb[2][x] == 'x')
                        {
                            //split the third container string at X
                            string[] dString = sb[2].ToString().Split('x');
                            //replace current string with first new splitted string
                            sb[2] = sb[2].Replace(sb[2].ToString(), dString[0]);
                            //set the fifth containter to store the second splitted string
                            sb[4].Append(dString[1]);
                            //check if the slash character exists
                            if (!sb[4].ToString().Contains("/")) { Debug.Log("Doesn't Contain / default to current screen HZ"); sb[5].Append(Screen.currentResolution.refreshRate); break; }
                            //find the slash character
                            for (int hz = 0; hz < sb[4].ToString().Length; hz++)
                            {
                                if (sb[4].ToString()[hz] == '/')
                                {
                                    //split the fifth container string at / (slash)
                                    string[] sString = sb[4].ToString().Split('/');
                                    //replace current string with first new splitted string
                                    sb[4] = sb[4].Replace(sb[4].ToString(), sString[0]);
                                    //set the sixth containter to store the second splitted string
                                    sb[5].Append(sString[1]);
                                }
                            }
                        }
                    }
                    //convert [VALUE] strings to an integer
                    if (!int.TryParse(sb[2].ToString(), out int parsable0)) { Debug.Log("0 not parsable"); SendCommandError(0); return; }
                    if (!int.TryParse(sb[4].ToString(), out int parsable1)) { Debug.Log("1 not parsable"); SendCommandError(0); return; }
                    if (!int.TryParse(sb[5].ToString(), out int parsable2)) { Debug.Log("2 not parsable"); SendCommandError(0); return; }

                    int[] val = new int[3] { parsable0, parsable1, parsable2 };
                    //return error if value is higher or lower than parameter value
                    if (val[0] > 7680 || val[0] < 320 && val[1] > 4320 || val[1] < 240) { Debug.Log("Below or Above recommended resolution"); SendCommandError(0); return; }
                    //set the second container to store the final output message to user
                    sb[1].Append(sb[3].ToString() + " [" + sb[2].ToString() + "X" + sb[4].ToString() +"/"+ sb[5].ToString() + "HZ Resolution Selected]");
                    //apply cheatcode parameter
                    optionsSystem.SetCustomResolution(val[0], val[1], val[2]);
                    AudioSystem.audioSystem.PlayAudioSource(inputSfx[1], 1, 1, 128);
                    break;
                }
            case "_vsync":
                {
                    //convert [VALUE] string to an integer
                    int? valConversion = StringConversionChecker(sb[2].ToString(), 1);
                    if (valConversion == null) return;
                    int val = valConversion.Value;
                    //set the second container to store the final output message to user
                    sb[1].Append(sb[3].ToString() + " [ V-Sync: " + status[val] + "]");
                    //apply cheatcode parameter
                    optionsSystem.vSyncIndex = val;
                    optionsSystem.SetVSync();
                    AudioSystem.audioSystem.PlayAudioSource(inputSfx[1], 1, 1, 128);
                    break;
                }
            case "_hdr":
                {
                    if (!HDROutputSettings.main.available) { SendCommandError(12); return; }
                    //convert [VALUE] string to an integer
                    int? valConversion = StringConversionChecker(sb[2].ToString(), 1);
                    if (valConversion == null) return;
                    int val = valConversion.Value;
                    //set the second container to store the final output message to user
                   
                    sb[1].Append(sb[3].ToString() + " [High Definition Range: " + status[val] + "]");
                    //apply cheatcode parameter
                    optionsSystem.hdrIndex = val;
                    optionsSystem.SetHDRMode();
                    AudioSystem.audioSystem.PlayAudioSource(inputSfx[1], 1, 1, 128);
                    break;
                }
            case "_fov":
                {
                    //convert [VALUE] string to an integer
                    int? valConversion = FOVConversionChecker(sb[2].ToString(), 120, 60);
                    if (valConversion == null) return;
                    int val = valConversion.Value;
                    //set the second container to store the final output message to user
                    sb[1].Append(sb[3].ToString() + " [Field of View Range: " + val + "]");
                    //apply cheatcode parameter
                    optionsSystem.fieldOfView = val;
                    optionsSystem.SetFieldOfView(optionsSystem.fieldOfView);
                    AudioSystem.audioSystem.PlayAudioSource(inputSfx[1], 1, 1, 128);
                    break;
                } 
            case "_brightness":
                {
                    //convert [VALUE] string to an integer
                    int? valConversion = StringConversionChecker(sb[2].ToString(), 10);
                    if (valConversion == null) return;
                    int val = valConversion.Value;
                    //set the second container to store the final output message to user
                    sb[1].Append(sb[3].ToString() + " [Screen Brightness: " + val + "]");
                    //apply cheatcode parameter
                    optionsSystem.brightness = val;
                    optionsSystem.SetBrightness(optionsSystem.brightness);
                    AudioSystem.audioSystem.PlayAudioSource(inputSfx[1], 1, 1, 128);
                    break;
                }
            case "_bloom":
                {
                    //convert [VALUE] string to an integer
                    int? valConversion = StringConversionChecker(sb[2].ToString(), 1);
                    if (valConversion == null) return;
                    int val = valConversion.Value;
                    //set the second container to store the final output message to user
                    sb[1].Append(sb[3].ToString() + " [Bloom Enabled: " + status[val] + " ]");
                    //apply cheatcode parameter
                    optionsSystem.SetpostProcessingIndividual(0, val, 6);
                    AudioSystem.audioSystem.PlayAudioSource(inputSfx[1], 1, 1, 128);
                    break;
                }
            case "_bloomval":
                {
                    //convert [VALUE] string to an integer
                    if (!optionsSystem.enablePostEffect[0]) { SendCommandError(13); return; }
                    int? valConversion = StringConversionChecker(sb[2].ToString(), 10);
                    if (valConversion == null) return;
                    int val = valConversion.Value;
                    //set the second container to store the final output message to user
                    sb[1].Append(sb[3].ToString() + " [Bloom Intensity: " + val + " ]");
                    //apply cheatcode parameter
                    optionsSystem.bloomIntensity = val;
                    optionsSystem.SetBloom(optionsSystem.bloomIntensity);
                    AudioSystem.audioSystem.PlayAudioSource(inputSfx[1], 1, 1, 128);
                    break;
                }
            case "_chromatic":
                {
                    //convert [VALUE] string to an integer

                    int? valConversion = StringConversionChecker(sb[2].ToString(), 1);
                    if (valConversion == null) return;
                    int val = valConversion.Value;
                    //set the second container to store the final output message to user
                    sb[1].Append(sb[3].ToString() + " [Chromatic Aberration Enabled: " + status[val] + " ]");
                    //apply cheatcode parameter
                    optionsSystem.SetpostProcessingIndividual(1, val, 7);
                    AudioSystem.audioSystem.PlayAudioSource(inputSfx[1], 1, 1, 128);
                    break;
                }
            case "_lens":
                {
                    //convert [VALUE] string to an integer
                    int? valConversion = StringConversionChecker(sb[2].ToString(), 1);
                    if (valConversion == null) return;
                    int val = valConversion.Value;
                    //set the second container to store the final output message to user
                    sb[1].Append(sb[3].ToString() + " [Lens Distortion Enabled: " + status[val] + " ]");
                    //apply cheatcode parameter
                    optionsSystem.SetpostProcessingIndividual(2, val, 8);
                    AudioSystem.audioSystem.PlayAudioSource(inputSfx[1], 1, 1, 128);
                    break;
                }
            case "_ambient":
                {
                    //convert [VALUE] string to an integer
                    int? valConversion = StringConversionChecker(sb[2].ToString(), 1);
                    if (valConversion == null) return;
                    int val = valConversion.Value;
                    //set the second container to store the final output message to user
                    sb[1].Append(sb[3].ToString() + " [Ambient Occlusion Enabled: " + status[val] + " ]");
                    //apply cheatcode parameter
                    optionsSystem.SetpostProcessingIndividual(3, val, 9);
                    AudioSystem.audioSystem.PlayAudioSource(inputSfx[1], 1, 1, 128);
                    break;
                }
            case "_colorgrade":
                {
                    //convert [VALUE] string to an integer
                    int? valConversion = StringConversionChecker(sb[2].ToString(), 1);
                    if (valConversion == null) return;
                    int val = valConversion.Value;
                    //set the second container to store the final output message to user
                    sb[1].Append(sb[3].ToString() + " [Color Grading Enabled: " + status[val] + " ]");
                    //apply cheatcode parameter
                    optionsSystem.SetpostProcessingIndividual(4, val, 10);
                    AudioSystem.audioSystem.PlayAudioSource(inputSfx[1], 1, 1, 128);
                    break;
                }
            case "_colormode":
                {
                    //convert [VALUE] string to an integer
                    int? valConversion = StringConversionChecker(sb[2].ToString(), optionsSystem.colorGradingModeName.Length - 1);
                    if (valConversion == null) return;
                    int val = valConversion.Value;
                    //set the second container to store the final output message to user
                    sb[1].Append(sb[3].ToString() + " [ [" + optionsSystem.colorGradingModeName[val] + "] Color Grading (Mode) Selected]");
                    //apply cheatcode parameter
                    optionsSystem.SetpostProcessingIndividual(0, val, 11);
                    AudioSystem.audioSystem.PlayAudioSource(inputSfx[1], 1, 1, 128);
                    break;
                }
            case "_colortone":
                {
                    //convert [VALUE] string to an integer
                    int? valConversion = StringConversionChecker(sb[2].ToString(), optionsSystem.colorGradingToneName.Length - 1);
                    if (valConversion == null) return;
                    int val = valConversion.Value;
                    //set the second container to store the final output message to user
                    sb[1].Append(sb[3].ToString() + " [ [" + optionsSystem.colorGradingToneName[val] + "] Color Grading (Tone Mapping) Selected]");
                    //apply cheatcode parameter
                    optionsSystem.SetpostProcessingIndividual(0, val, 12);
                    AudioSystem.audioSystem.PlayAudioSource(inputSfx[1], 1, 1, 128);
                    break;
                }
            case "_colorsat":
                {
                    //convert [VALUE] string to an integer
                    int? valConversion = GCSettingsConversionChecker(sb[2].ToString());
                    if (valConversion == null) return;
                    int val = valConversion.Value;
                    //set the second container to store the final output message to user
                    sb[1].Append(sb[3].ToString() + " [Color Grading (Saturation): [" + val + "%] ]");
                    //apply cheatcode parameter
                    optionsSystem.saturation = Map(val, 0, 100, -100, 100);
                    optionsSystem.SetSaturation(optionsSystem.saturation);
                    AudioSystem.audioSystem.PlayAudioSource(inputSfx[1], 1, 1, 128);
                    break;
                }
            case "_colorcon":
                {
                    //convert [VALUE] string to an integer
                    int? valConversion = GCSettingsConversionChecker(sb[2].ToString());
                    if (valConversion == null) return;
                    int val = valConversion.Value;
                    //set the second container to store the final output message to user
                    sb[1].Append(sb[3].ToString() + " [Color Grading (Contrast): [" + val + "%] ]");
                    //apply cheatcode parameter
                    optionsSystem.contrast = Map(val, 0, 100, -100, 100);
                    optionsSystem.SetContrast(optionsSystem.contrast);
                    AudioSystem.audioSystem.PlayAudioSource(inputSfx[1], 1, 1, 128);
                    break;
                }
            case "_colorlift":
                {
                    //convert [VALUE] string to an integer
                    int? valConversion = GCSettingsConversionChecker(sb[2].ToString());
                    if (valConversion == null) return;
                    int val = valConversion.Value;
                    //set the second container to store the final output message to user
                    sb[1].Append(sb[3].ToString() + " [Color Grading (Lift): [" + val + "%] ]");
                    //apply cheatcode parameter
                    optionsSystem.lift = Map(val, 0, 100, -1, 1);
                    optionsSystem.SetLift(optionsSystem.lift);
                    AudioSystem.audioSystem.PlayAudioSource(inputSfx[1], 1, 1, 128);
                    break;
                }
            case "_colorgamma":
                {
                    //convert [VALUE] string to an integer
                    int? valConversion = GCSettingsConversionChecker(sb[2].ToString());
                    if (valConversion == null) return;
                    int val = valConversion.Value;
                    //set the second container to store the final output message to user
                    sb[1].Append(sb[3].ToString() + " [Color Grading (Gamma): [" + val + "%] ]");
                    //apply cheatcode parameter
                    optionsSystem.gamma = Map(val, 0, 100, -1, 1);
                    optionsSystem.SetGamma(optionsSystem.gamma);
                    AudioSystem.audioSystem.PlayAudioSource(inputSfx[1], 1, 1, 128);
                    break;
                }
            case "_colorgain":
                {
                    //convert [VALUE] string to an integer
                    int? valConversion = GCSettingsConversionChecker(sb[2].ToString());
                    if (valConversion == null) return;
                    int val = valConversion.Value;
                    //set the second container to store the final output message to user
                    sb[1].Append(sb[3].ToString() + " [Color Grading (Gain): [" + val + "%] ]");
                    //apply cheatcode parameter
                    optionsSystem.gain = Map(val, 0, 100, -1, 1);
                    optionsSystem.SetGain(optionsSystem.gain);
                    AudioSystem.audioSystem.PlayAudioSource(inputSfx[1], 1, 1, 128);
                    break;
                }
            case "_grain":
                {
                    //convert [VALUE] string to an integer
                    int? valConversion = StringConversionChecker(sb[2].ToString(), 1);
                    if (valConversion == null) return;
                    int val = valConversion.Value;
                    //set the second container to store the final output message to user
                    sb[1].Append(sb[3].ToString() + " [Grain Enabled: " + status[val] + "]");
                    //apply cheatcode parameter
                    optionsSystem.SetpostProcessingIndividual(5, val, 14);
                    AudioSystem.audioSystem.PlayAudioSource(inputSfx[1], 1, 1, 128);
                    break;
                }
            case "_grainval":
                {
                    //convert [VALUE] string to an integer
                    int? valConversion = StringConversionChecker(sb[2].ToString(), 10);
                    if (valConversion == null) return;
                    int val = (valConversion.Value);
                    //set the second container to store the final output message to user
                    sb[1].Append(sb[3].ToString() + " [Grain Intensity: " + val + "]");
                    //apply cheatcode parameter
                    optionsSystem.grainIntensity = val;
                    optionsSystem.SetGrain(optionsSystem.grainIntensity / 10);
                    AudioSystem.audioSystem.PlayAudioSource(inputSfx[1], 1, 1, 128);
                    break;
                }
            case "_vignette":
                {
                    //convert [VALUE] string to an integer
                    int? valConversion = StringConversionChecker(sb[2].ToString(), 1);
                    if (valConversion == null) return;
                    int val = valConversion.Value;
                    //set the second container to store the final output message to user
                    sb[1].Append(sb[3].ToString() + " [Vignette Enabled: " + status[val] + "]");
                    //apply cheatcode parameter
                    optionsSystem.SetpostProcessingIndividual(6, val, 16);
                    AudioSystem.audioSystem.PlayAudioSource(inputSfx[1], 1, 1, 128);
                    break;
                }
            case "_vignetteval":
                {
                    //convert [VALUE] string to an integer
                    int? valConversion = StringConversionChecker(sb[2].ToString(), 10);
                    if (valConversion == null) return;
                    int val = valConversion.Value;
                    //set the second container to store the final output message to user
                    sb[1].Append(sb[3].ToString() + " [Vignette Intensity: " + val + "]");
                    //apply cheatcode parameter
                    optionsSystem.vignetteIntensity = val;
                    optionsSystem.SetVignette(optionsSystem.vignetteIntensity / 10);
                    AudioSystem.audioSystem.PlayAudioSource(inputSfx[1], 1, 1, 128);
                    break;
                }
            //----------------------------------AUDIOSETTINGS
            case "_soundtrack":
                {
                    //convert [VALUE] string to an integer
                    int? valConversion = StringConversionChecker(sb[2].ToString(), 6);
                    if (valConversion == null) return;
                    int val = valConversion.Value;
                    //set the second container to store the final output message to user
                    sb[1].Append(sb[3].ToString() + " [Virvius Soundtrack [" + optionsSystem.songName[val] + "] selected]");
                    //apply cheatcode parameter
                    optionsSystem.soundtrackIndex = val;
                    optionsSystem.SetMusicSoundtrack();
                    AudioSystem.audioSystem.PlayAudioSource(inputSfx[1], 1, 1, 128);
                    break;
                }
            case "_volmaster":
                {
                    //convert [VALUE] string to an integer
                    int? valConversion = AudioSettingsConversionChecker(sb[2].ToString());
                    if (valConversion == null) return;
                    int val = valConversion.Value;
                    //set the second container to store the final output message to user
                    sb[1].Append(sb[3].ToString() + " [Volume (Master): [" + val + "%] ]");
                    //apply cheatcode parameter
                    optionsSystem.volumeInterval[0] = Map(val, 0, 100, 0, 1);
                    optionsSystem.SetAudioValue(0);
                    AudioSystem.audioSystem.PlayAudioSource(inputSfx[1], 1, 1, 128);
                    break;
                }
            case "_volmusic":
                {
                    //convert [VALUE] string to an integer
                    int? valConversion = AudioSettingsConversionChecker(sb[2].ToString());
                    if (valConversion == null) return;
                    int val = valConversion.Value;
                    //set the second container to store the final output message to user
                    sb[1].Append(sb[3].ToString() + " [Volume (Music): [" + val + "%] ]");
                    //apply cheatcode parameter
                    optionsSystem.volumeInterval[1] = Map(val, 0, 100, 0, 1);
                    Debug.Log(optionsSystem.volumeInterval[1]);
                    optionsSystem.SetAudioValue(1);
                    AudioSystem.audioSystem.PlayAudioSource(inputSfx[1], 1, 1, 128);
                    break;
                }
            case "_volsound":
                {
                    //convert [VALUE] string to an integer
                    int? valConversion = AudioSettingsConversionChecker(sb[2].ToString());
                    if (valConversion == null) return;
                    int val = valConversion.Value;
                    //set the second container to store the final output message to user
                    sb[1].Append(sb[3].ToString() + " [Volume (Sound): [" + val + "%] ]");
                    //apply cheatcode parameter
                    optionsSystem.volumeInterval[2] = Map(val, 0, 100, 0, 1);
                    optionsSystem.SetAudioValue(2);
                    AudioSystem.audioSystem.PlayAudioSource(inputSfx[1], 1, 1, 128);
                    break;
                }
            case "_volenvironment":
                {
                    //convert [VALUE] string to an integer
                    int? valConversion = AudioSettingsConversionChecker(sb[2].ToString());
                    if (valConversion == null) return;
                    int val = valConversion.Value;
                    //set the second container to store the final output message to user
                    sb[1].Append(sb[3].ToString() + " [Volume (Environment): [" + val + "%] ]");
                    //apply cheatcode parameter
                    optionsSystem.volumeInterval[3] = Map(val, 0, 100, 0, 1);
                    optionsSystem.SetAudioValue(3);
                    AudioSystem.audioSystem.PlayAudioSource(inputSfx[1], 1, 1, 128);
                    break;
                }
            case "_volplayer":
                {
                    //convert [VALUE] string to an integer
                    int? valConversion = AudioSettingsConversionChecker(sb[2].ToString());
                    if (valConversion == null) return;
                    int val = valConversion.Value;
                    //set the second container to store the final output message to user
                    sb[1].Append(sb[3].ToString() + " [Volume (Player): [" + val + "%] ]");
                    //apply cheatcode parameter
                    optionsSystem.volumeInterval[4] = Map(val, 0, 100, 0, 1);
                    optionsSystem.SetAudioValue(4);
                    AudioSystem.audioSystem.PlayAudioSource(inputSfx[1], 1, 1, 128);
                    break;
                }
            case "_speakermode":
                {
                    //convert [VALUE] string to an integer
                    int? valConversion = StringConversionChecker(sb[2].ToString(), 6);
                    if (valConversion == null) return;
                    int val = valConversion.Value;
                    //set the second container to store the final output message to user
                    sb[1].Append(sb[3].ToString() + " [Speaker Mode: [" + optionsSystem.speakerName[val] + "] Selected]");
                    //apply cheatcode parameter
                    optionsSystem.speakerIndex = val;
                    optionsSystem.SetSpeakerMode();
                    optionsSystem.ApplyAudioConfiguration();
                    AudioSystem.audioSystem.PlayAudioSource(inputSfx[1], 1, 1, 128);
                    break;
                }
            case "_virtualvoices":
                {
                    //convert [VALUE] string to an integer
                    int? valConversion = VoiceSettingsConversionChecker(sb[2].ToString());
                    if (valConversion == null) return;
                    int val = valConversion.Value;
                    //set the second container to store the final output message to user
                    sb[1].Append(sb[3].ToString() + " [Virtual Voices: [" + optionsSystem.voiceInterval[val] + "] Selected]");
                    //apply cheatcode parameter
                    optionsSystem.virtualVoiceIndex = val;
                    optionsSystem.SetVirtualVoices();
                    optionsSystem.ApplyAudioConfiguration();
                    AudioSystem.audioSystem.PlayAudioSource(inputSfx[1], 1, 1, 128);
                    break;
                }
            case "_realvoices":
                {
                    //convert [VALUE] string to an integer
                    int? valConversion = VoiceSettingsConversionChecker(sb[2].ToString());
                    if (valConversion == null) return;
                    int val = valConversion.Value;
                    //set the second container to store the final output message to user
                    sb[1].Append(sb[3].ToString() + " [Real Voices: [" + optionsSystem.voiceInterval[val] + "] Selected]");
                    //apply cheatcode parameter
                    optionsSystem.realVoiceIndex = val;
                    optionsSystem.SetRealVoices();
                    optionsSystem.ApplyAudioConfiguration();
                    AudioSystem.audioSystem.PlayAudioSource(inputSfx[1], 1, 1, 128);
                    break;
                }
            case "_samplerate":
                {
                    //convert [VALUE] string to an integer
                    int? valConversion = StringConversionChecker(sb[2].ToString(), optionsSystem.sampleInterval.Length - 1);
                    if (valConversion == null) return;
                    int val = valConversion.Value;
                    //set the second container to store the final output message to user
                    sb[1].Append(sb[3].ToString() + " [Sample Rate: [" + optionsSystem.sampleInterval[val] + "] Selected]");
                    //apply cheatcode parameter
                    optionsSystem.sampleRateIndex = val;
                    optionsSystem.SetSampleRate();
                    optionsSystem.ApplyAudioConfiguration();
                    AudioSystem.audioSystem.PlayAudioSource(inputSfx[1], 1, 1, 128);
                    break;
                }
            case "_buffer":
                {
                    //convert [VALUE] string to an integer
                    int? valConversion = StringConversionChecker(sb[2].ToString(), optionsSystem.bufferInterval.Length - 1);
                    if (valConversion == null) return;
                    int val = valConversion.Value;
                    //set the second container to store the final output message to user
                    sb[1].Append(sb[3].ToString() + " [DPS Buffer: [" + optionsSystem.bufferInterval[val] + "] Selected]");
                    //apply cheatcode parameter
                    optionsSystem.dpsBufferIndex = val;
                    optionsSystem.SetDPSBuffer();
                    optionsSystem.ApplyAudioConfiguration();
                    AudioSystem.audioSystem.PlayAudioSource(inputSfx[1], 1, 1, 128);
                    break;
                }
        }

        inputRecieved[8].text = sb[1].ToString();
        ShiftRecievedInput(true);
        inputField.text = null;
        inputField.ActivateInputField();
    }
    private float Map(float value, float inMin, float inMax, float outMin, float outMax)
    {
        return (value - inMin) * (outMax - outMin) / (inMax - inMin) + outMin;
    }
    private int? StringConversionChecker(string message, int maxCodeIDLength)
    {
        //convert [VALUE] string to an integer
        if (!int.TryParse(message, out int parsable)) { SendCommandError(maxCodeIDLength); return null; }
        int? val = parsable;
        //return error if value is higher or lower than parameter value
        if (val > maxCodeIDLength || val < 0) { SendCommandError(maxCodeIDLength); return null; }
        return val;
    }
    private int? FOVConversionChecker(string message, int maxCodeIDLength, int minCodeIDLength)
    {
        //convert [VALUE] string to an integer
        if (!int.TryParse(message, out int parsable)) { SendCommandError(18); return null; }
        int? val = parsable;
        //return error if value is higher or lower than parameter value
        if (val > maxCodeIDLength || val < minCodeIDLength) { SendCommandError(18); return null; }
        return val;
    }
    private int? GCSettingsConversionChecker(string message)
    {
        //convert [VALUE] string to an integer
        if (!int.TryParse(message, out int parsable)) { SendCommandError(15); return null; }
        int? val = parsable;
        //return error if value is higher or lower than parameter value
        if (val > 100 || val < 0) { SendCommandError(15); return null; }
        return val;
    }
    private int? AudioSettingsConversionChecker(string message)
    {
        //convert [VALUE] string to an integer
        if (!int.TryParse(message, out int parsable)) { SendCommandError(17); return null; }
        int? val = parsable;
        //return error if value is higher or lower than parameter value
        if (val > 100 || val < 0.01f) { SendCommandError(17); return null; }
        return val;
    }
    private int? VoiceSettingsConversionChecker(string message)
    {
        //convert [VALUE] string to an integer
        if (!int.TryParse(message, out int parsable)) { SendCommandError(16); return null; }
        int? val = parsable;
        //return error if value is higher or lower than parameter value
        if (val > 11 || val < 0) { SendCommandError(16); return null; }
        return val;
    }
    private void ShiftRecievedInput(bool active)
    {
        for(int i = inputRecieved.Length -2; i > 0; i--)
        {
            inputRecieved[i].text = inputRecieved[i - 1].text;
        }
        inputRecieved[0].text = active ? sb[3].ToString(): sb[0].ToString();
    }
    private void ClearCommandWindow()
    {
        for(int ir = 0; ir < inputRecieved.Length; ir++)
        {
            inputRecieved[ir].text = null;
        }
    }
}
