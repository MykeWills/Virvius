using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnvironmentSystem : MonoBehaviour
{
    public static EnvironmentSystem environmentSystem;
    private OptionsSystem optionsSystem;
    private PlayerSystem playerSystem;
    private InputSystem inputSystem;
    private AudioSystem audioSystem;
    [SerializeField]
    private GameObject waveEffect;
    private RaycastHit hit;
    private Vector3 contactPoint;
    [SerializeField]
    private Image environmentUI;
    [SerializeField]
    private Sprite[] environmentSprites = new Sprite[3];
    [HideInInspector]
    public bool isDrowning = false;
    [HideInInspector]
    public bool environmentDamage = false;
    [HideInInspector]
    public int environmentIndex = 0;
    [HideInInspector]
    public int environmentSoundIndex = 0;
    private float environmentTime = 1f;
    private float environmentTimer;
    private SphereCollider col;
    public AudioClip playerDrSound;
    public AudioClip playerBrSound;
    public AudioClip playerGaSound;
    public AudioClip playerDiSound;
    public AudioClip environmentUnderWater;
    public AudioClip storming;
    private AudioClip curEnvironment;
    [HideInInspector]
    public  string[] environmentTag = new string[3] { "Lava", "Acid", "Water"};
    public bool headUnderWater = false;
    private void Awake()
    {
        environmentSystem = this;
    }

    // Start is called before the first frame update
    public void Start()
    {
        audioSystem = AudioSystem.audioSystem;
        playerSystem = PlayerSystem.playerSystem;
        inputSystem = InputSystem.inputSystem;
        optionsSystem = OptionsSystem.optionsSystem;
        col = GetComponent<SphereCollider>();
        curEnvironment = storming;
    }

    // Update is called once per frame
    void Update()
    {
        Environment(environmentIndex);
    }
    private void Environment(int index)
    {
        if (playerSystem.isDead)
            return;
        switch (index)
        {
            case 0: return;
            case 1:
                {
                    EnvironmentDamage(environmentTime, 10);
                    break;
                }
            case 2:
                {
                    EnvironmentDamage(environmentTime, 5);
                    break;
                }
            case 3:
                {
                    if (!isDrowning)
                    {
                        environmentTimer -= Time.deltaTime;
                        environmentTimer = Mathf.Clamp(environmentTimer, 0.0f, environmentTime);
                        if (environmentTimer == 0)
                            isDrowning = true;
                        //Debug.Log(environmentTimer);
                    }
                    else
                    {
                        EnvironmentDamage(1, 2);
                    }
                    break;
                }
        }
    }
    private void EnvironmentDamage(float time, int damageAmt)
    {
        environmentDamage = true;
        float speedAbsolute = 1.0f / environmentTimer;
        environmentTimer -= Time.deltaTime * speedAbsolute;
        environmentTimer = Mathf.Clamp(environmentTimer, 0.0f, time);
        if (environmentTimer == 0)
        {
            //environmentSoundIndex++;
            if (environmentSoundIndex > 1) environmentSoundIndex = 0;
            if (isDrowning)
                audioSystem.PlayAudioSource(playerDrSound, Random.Range(0.7f, 1), 1, 128);
            playerSystem.Damage(damageAmt);
            environmentTimer = time;
            environmentDamage = false;
        }
    }
    public void SetEnvironment(float time, int index)
    {
        environmentIndex = index;
        environmentTime = time;
        environmentTimer = environmentTime;
        if (isDrowning) isDrowning = false;
        environmentSoundIndex = 0;
        if (environmentDamage) environmentDamage = false;
    }
    private void OnTriggerStay(Collider other)
    {
        for (int en = 0; en < environmentTag.Length; en++)
        {
            if (other.gameObject.CompareTag(environmentTag[en]))
            {
                if (col.bounds.center.y <= other.bounds.max.y)
                {
                    if(!environmentUI.enabled && headUnderWater) ActiveEnvironmentUI(true);
                }
                else if (col.bounds.center.y > other.bounds.max.y)
                {
                    if (environmentUI.enabled) ActiveEnvironmentUI(false);
                }
                break;
            }
        }
    }
   
    public void ActiveEnvironmentUI(bool active)
    {
        environmentUI.enabled = active;
        if (AudioSystem.audioSystem.E_audioSrc.clip != environmentUnderWater)
            curEnvironment = AudioSystem.audioSystem.E_audioSrc.clip;
        audioSystem.AlterMusicStandalone(1, !active ? 1 : 0.6f);
        audioSystem.PlayGameEnvironment(active ? environmentUnderWater : curEnvironment, 1, active ? 0.8f : 0.6f, true);
        if(optionsSystem.environmentEffects)
            waveEffect.SetActive(active);
        else waveEffect.SetActive(false);
        if (!active && !playerSystem.isDead && isDrowning)
            audioSystem.PlayAudioSource(playerGaSound, Random.Range(0.7f, 1), 1, 128);
    }
    private void OnTriggerEnter(Collider other)
    {
        for (int e = 0; e < environmentTag.Length; e++)
        {
            if (other.gameObject.CompareTag(environmentTag[e])) 
            {
                if (e == 2)
                    SetEnvironment(25, 3);
                ActivateEnvironment(e + 1); 
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        for (int e = 0; e < environmentTag.Length; e++)
        {
            if (other.gameObject.CompareTag(environmentTag[e])) 
            {
                if(environmentTag[e] == "Water")
                    SetEnvironment(0, 0);
                ActiveEnvironmentUI(false);
                if (headUnderWater) 
                {
                    audioSystem.PlayAudioSource(playerBrSound, Random.Range(0.7f, 1), 1, 128);
                    switch (environmentTag[e])
                    {
                        case "Water": playerSystem.StartFadeEnvironmentUI(true, 3, new Color(0.85f, 0.85f, 0.85f, 1)); break;
                        case "Acid": playerSystem.StartFadeEnvironmentUI(true, 3, Color.green); break;
                        case "Lava": playerSystem.StartFadeEnvironmentUI(true, 3, Color.yellow); break;
                    }
                  
                    headUnderWater = false;
                }
            }

        }
    }
    public void ActivateEnvironment(int index)
    {
        if (index > 0)
        {
            float vel = 0;
            inputSystem.moveDirection.y = vel;
            headUnderWater = true;
            environmentUI.sprite = environmentSprites[index - 1];
            audioSystem.PlayAudioSource(playerDiSound, 1, 1, 128);
        }
    }
    public void ActivateSwimming(bool active)
    {
       
        inputSystem.isSwimming = active;
        if (active)
        {
            float vel = inputSystem.moveDirection.y / 2;
            inputSystem.moveDirection.y = vel;
        }
    }
}
