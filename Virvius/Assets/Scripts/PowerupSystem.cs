using UnityEngine;
using UnityEngine.UI;
using System.Text;

public class PowerupSystem : MonoBehaviour
{
    public static PowerupSystem powerupSystem;
    private PlayerSystem playerSystem;
    private WeaponSystem weaponSystem;
    private MessageSystem messageSystem;
    private OptionsSystem optionsSystem;
    private CommandSystem commandSystem;
    private AudioSystem audioSystem;
    private StringBuilder sb = new StringBuilder();
    private StringBuilder msgSb = new StringBuilder();
    private int curWeaponID = 0;
    [HideInInspector]
    public bool[] powerEnabled = new bool[6];
    public Sprite[] powerSprite = new Sprite[6];
    private float powerTime = 30f;
    private string[] powerName = new string[6] { "V-Damage", "Vanisher", "Punisher", "Berzerker", "Reviver", "Divinity" };
    public AudioClip[] powerSound = new AudioClip[6];
    private float depleteTime = 1.0f;
    [HideInInspector]
    public Color[] powerColor = new Color[6]
    {
        // purple
        new Color(0.25f, 0, 1, 0.35f),
        // white
        new Color(1, 1, 1, 0.35f),
        // red
        new Color(1, 0, 0, 0.35f),
        // yellow
        new Color(1, 0.7f, 0, 0.35f),
        // green
        new Color(0, 1, 0, 0.35f),
        // blue
        new Color(0, 0, 1, 0.35f)
    };
    private float powerTimer = 0;
    [HideInInspector]
    public bool powerActive = false;
    [HideInInspector]
    public int powerIndex = 0;
    private int soundInterval = 3;
    public Image[] powerUI = new Image[2];
    public Light powerLight;
    public Image[] powerbarIcon = new Image[2];
    public Image[] powerBar = new Image[2];
    public Text[] powerAmt = new Text[2];
    public Text[] powerNameText = new Text[2];
    public GameObject[] powerBanner = new GameObject[2];
    [HideInInspector]
    public int BPowerMultiplier = 1;
    public enum Powers { VDamage, Vanisher, Punisher, Berserker, Reviver, Divinity }
    private Powers curPower;
    private void Awake()
    {
        powerupSystem = this;
    }
    public void Start()
    {
        messageSystem = MessageSystem.messageSystem;
        playerSystem = PlayerSystem.playerSystem;
        weaponSystem = WeaponSystem.weaponSystem;
        optionsSystem = OptionsSystem.optionsSystem;
        audioSystem = AudioSystem.audioSystem;
        commandSystem = CommandSystem.commandSystem;
        BPowerMultiplier = 1;
    }
    void Update()
    {
        Power();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (!powerActive)
        {
            if (other.gameObject.CompareTag("VDamage")) 
            { 
                curPower = Powers.VDamage;
                messageSystem.SetMessage(BuildMessages("V-Damage! - Quint Damage"), MessageSystem.MessageType.Top);
                other.gameObject.SetActive(false); 
                powerActive = true; 
            }
            else if (other.gameObject.CompareTag("Vanish")) 
            { 
                curPower = Powers.Vanisher;
                messageSystem.SetMessage(BuildMessages("Vanisher! - Invisibility"), MessageSystem.MessageType.Top);
                other.gameObject.SetActive(false); 
                powerActive = true; 
            }
            else if (other.gameObject.CompareTag("Punish")) 
            {
                curWeaponID = weaponSystem.weaponIndex;
                curPower = Powers.Punisher;
                messageSystem.SetMessage(BuildMessages("Punisher! - Unarmed Brutality"), MessageSystem.MessageType.Top);
                other.gameObject.SetActive(false); 
                powerActive = true; 
            }
            else if (other.gameObject.CompareTag("Berserk")) 
            {
                curPower = Powers.Berserker;
                messageSystem.SetMessage(BuildMessages("Berserker! - Haste"), MessageSystem.MessageType.Top);
                other.gameObject.SetActive(false); 
                powerActive = true; 
            }
            else if (other.gameObject.CompareTag("Regen")) 
            { 
                curPower = Powers.Reviver;
                messageSystem.SetMessage(BuildMessages("Regenerator! - Health Regeneration"), MessageSystem.MessageType.Top);
                other.gameObject.SetActive(false); 
                powerActive = true; 
            }
            else if (other.gameObject.CompareTag("Divine")) 
            { 
                curPower = Powers.Divinity;
                messageSystem.SetMessage(BuildMessages("Divinity! - God Mode"), MessageSystem.MessageType.Top);
                other.gameObject.SetActive(false); 
                powerActive = true; 
            }
            ActivatePower(curPower);
            if (powerEnabled[2])
                weaponSystem.AutoSelectWeapon(0);
            else if (powerEnabled[4])
                playerSystem.ActivateReviver();
        }
    }
    private void ActivatePower(Powers power)
    {
        if (!powerActive) return;
            switch (power)
        {
            case Powers.VDamage: powerIndex = 0; break;
            case Powers.Vanisher: powerIndex = 1; break;
            case Powers.Punisher: powerIndex = 2; break;
            case Powers.Berserker: powerIndex = 3; break;
            case Powers.Reviver: powerIndex = 4; break;
            case Powers.Divinity: powerIndex = 5; break;
        }
        for (int pb = 0; pb < powerBanner.Length; pb++) 
            powerBanner[pb].SetActive(true);
        powerUI[0].enabled = true;
        for (int p = 0; p < 2; p++)
        {
            powerBar[p].color = powerColor[powerIndex];
            powerUI[p].color = powerColor[powerIndex];
            powerNameText[p].color = powerColor[powerIndex];
            powerbarIcon[p].sprite = powerSprite[powerIndex];
            powerNameText[p].text = powerName[powerIndex];
            powerAmt[p].color = powerColor[powerIndex];
            if (optionsSystem.vignetteLayer.enabled.value && optionsSystem.vignetteLayer.color.value != powerColor[powerIndex])
                optionsSystem.vignetteLayer.color.value = powerColor[powerIndex];
        }
        powerLight.color = powerColor[powerIndex];
        powerLight.enabled = true;
        audioSystem.PlayAudioSource(powerSound[powerIndex], 1, 1, 128);
        for(int p = 0; p < powerEnabled.Length; p++){ if (p == powerIndex) powerEnabled[p] = true;  else powerEnabled[p] = false; }
        powerLight.intensity = powerEnabled[2] ? 4f : 7f;
        powerTimer = powerTime;
        playerSystem.ApplyPlayerHealthAndArmor();
        weaponSystem.ApplyAmmo();
    }
    public void SetPowerUI(int index)
    {
        powerBanner[index].SetActive(true);
        powerUI[0].enabled = true;
        powerUI[index].color = powerColor[powerIndex];
        powerNameText[index].color = powerColor[powerIndex];
        powerbarIcon[index].sprite = powerSprite[powerIndex];
        powerNameText[index].text = powerName[powerIndex];
    }
    private float Map(float value, float inMin, float inMax, float outMin, float outMax)
    {
        return (value - inMin) * (outMax - outMin) / (inMax - inMin) + outMin;
    }
    private string FormatValues(int value)
    {
        if (sb.Length > 0)
            sb.Clear();
        sb.Append(value);
        return sb.ToString();
    }
    private void Power()
    {
        if (BPowerMultiplier != (commandSystem.masterCodesActive[3] ? 3 : (powerEnabled[3] ? 2 : 1)))
            BPowerMultiplier = commandSystem.masterCodesActive[3] ? 3 : (powerEnabled[3] ? 2 : 1);
        if (!powerActive) return;
        float reduceTime = Time.deltaTime;
        powerTimer -= reduceTime;
        int time = Mathf.RoundToInt(powerTimer);
        if (time <= 3 && time >= 0)
        {
            depleteTime -= reduceTime;
            if (depleteTime < 0)
                depleteTime = 1;
            Color depleteColor = new Color(powerColor[powerIndex].r, powerColor[powerIndex].g, powerColor[powerIndex].b, Mathf.Lerp(1, 0, depleteTime));
            for (int p = 0; p < 2; p++)
            {
                powerUI[p].color = depleteColor;
                if (optionsSystem.vignetteLayer.enabled.value && optionsSystem.vignetteLayer.color.value != depleteColor)
                    optionsSystem.vignetteLayer.color.value = depleteColor;
                if (playerSystem.versionIndex < 3)
                    break;
            }
        }
        if (time == soundInterval && soundInterval > 0)
        {
            soundInterval--;
            audioSystem.PlayAudioSource(powerSound[powerIndex], 1, 1, 128);
        }
        if (time < 1)
        {
            powerLight.enabled = false;
            powerUI[0].enabled = false;
            powerUI[1].color = playerSystem.visorColor;
            if (optionsSystem.vignetteLayer.enabled.value && optionsSystem.vignetteLayer.color.value != Color.black)
                optionsSystem.vignetteLayer.color.value = Color.black;
            powerTimer = 0;
            powerActive = false;
            soundInterval = 3;
            for (int p = 0; p < powerEnabled.Length; p++)
            {
                if (powerEnabled[p]) 
                { 
                    powerEnabled[p] = false;
                    if (p == 2)
                    {
                        if(curWeaponID != 0) weaponSystem.AutoSelectWeapon(curWeaponID);
                        else weaponSystem.AutoSelectWeapon(1);
                    }
                }

            }
            for (int pb = 0; pb < powerBanner.Length; pb++)
                powerBanner[pb].SetActive(false);
            playerSystem.ApplyPlayerHealthAndArmor();
            weaponSystem.ApplyAmmo();
        }
        float powerFill = Map(powerTimer, 0, powerTime, 0, 1);
        if (powerBar[playerSystem.versionID].fillAmount != powerFill)
        {
            powerAmt[playerSystem.versionID].text = FormatValues(time);
            powerBar[playerSystem.versionID].fillAmount = powerFill;
        }
    }
    private string BuildMessages(string pickupName)
    {
        if (msgSb.Length > 0)
            msgSb.Clear();
        msgSb.Append(pickupName + ".");
        return msgSb.ToString();
    }
    public void ResetPowerupSystem()
    {
        powerLight.enabled = false;

        powerUI[1].color = playerSystem.visorColor;
        powerUI[0].enabled = false;
        powerTimer = 0;
        powerActive = false;
        soundInterval = 3;
        for (int p = 0; p < powerEnabled.Length; p++)
            if (powerEnabled[p]) powerEnabled[p] = false;
        for (int p = 0; p < 2; p++)
            powerBanner[p].SetActive(false);
        powerLight.intensity = powerEnabled[2] ? 4f : 7f;
    }
    public void Cheat(int val)
    {
        Powers[] powers = new Powers[6] { Powers.VDamage, Powers.Vanisher, Powers.Berserker, Powers.Punisher, Powers.Reviver, Powers.Divinity };
        powerActive = true;
        ActivatePower(powers[val]);
        if (powerEnabled[2])
            weaponSystem.AutoSelectWeapon(0);
        else if (powerEnabled[4])
            playerSystem.ActivateReviver();
    }
}
