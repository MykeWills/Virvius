using UnityEngine;

public class LightSystem : MonoBehaviour
{
    [SerializeField]
    private Light spotLight;
    public enum Effect { none, flicker, pulse, strobe }
    [SerializeField]
    private Effect lightEffect;
    [Range(0, 10)]
    [SerializeField]
    private float flashSpeed = 1;
    [Range(0, 50)]
    [SerializeField]
    private float lightIntensity = 1;
    private float lightTime = 0;
    private float lightTimer = 0;
    private float time = 0;
    private bool lightOff = false;

    [Header("flicker Settings")]
    [Range(0, 10)]
    [SerializeField]
    private float flickerTime = Random.Range(0.1f, 2.0f);
    [Range(0, 10)]
    [SerializeField]
    private float flickerRange = 1;
    [Space]
    [Header("Pulse Settings")]
    [SerializeField]
    private float pulseTime = 1;
    private bool pulseIn = false;

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        time = Time.deltaTime * flashSpeed;
        Timer();
    }
    private void SetLightEffect()
    {
        switch (lightEffect)
        {
            case Effect.flicker: 
                flickerTime = Random.Range(0.1f, flickerRange); 
                lightTime = flickerTime; 
                break;
            case Effect.pulse:
                pulseIn = !pulseIn;
                lightTime = pulseTime; 
                break;
        }
        lightOff = !lightOff;
        spotLight.enabled = lightOff;
    }
    private void Timer()
    {
        lightTimer -= time;
        lightTimer = Mathf.Clamp(lightTimer, 0, lightTime);
        if (lightEffect == Effect.pulse) spotLight.intensity = pulseIn ? lightTimer * 10  : -lightIntensity * -10;
        if(lightTimer == 0)
        {
            SetLightEffect();
        }
    }
}
