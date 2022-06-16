using UnityEngine;

public class LightSystem : MonoBehaviour
{
    [SerializeField]
    private Light spotLight;
    public enum Effect { none, flicker, pulse, strobe, fade, trigger }
    public enum Type { spot, directional, point, }
    [Header("Light Settings")]
    [SerializeField]
    private Effect lightEffect;
    [SerializeField]
    private Type lightType;
    [Range(0f, 10f)]
    [SerializeField]
    private float flashSpeed = 1;
    [Range(0f, 100f)]
    [SerializeField]
    private float lightIntensity = 10;
    [Range(0f, 100f)]
    [SerializeField]
    private float lightRange = 50;
    private bool lightActive = true;
    private float lightTime = 0;
    private float lightTimer = 0;
    private float time = 0;
    private bool lightOff = false;

    [Header("flicker Settings")]
    private float flickerTime = 1;
    [Range(0, 10)]
    [SerializeField]
    private float flickerMinRange = 0; 
    [Range(0, 10)]
    [SerializeField]
    private float flickerMaxRange = 1;
    [Space]
    [Header("Pulse Settings")]
    [SerializeField]
    private float pulseTime = 1;
    private bool pulseIn = false;
    private int pulseInterval = 1;
    [Space]
    [Header("Strobe Settings")]
    [SerializeField]
    private float strobeTime = 1;
    private bool strobeIn = false;
    [Space]
    [Header("Fade Settings")]
    [SerializeField]
    private float fadeTime = 1;
    [Space]
    [Header("Trigger Settings")]
    [SerializeField]
    private bool lightOn = true;

    private void Start()
    {
        lightActive = lightOn;
        SetLightEffect();
    }
    void Update()
    {
        time = Time.deltaTime;
        Timer();
    }
    private void SetLightEffect()
    {
        if (spotLight.range != lightRange) spotLight.range = lightRange;
        switch (lightType)
        {
            case Type.spot: if (spotLight.type != LightType.Spot) spotLight.type = LightType.Spot; break;
            case Type.directional: if (spotLight.type != LightType.Directional) spotLight.type = LightType.Directional; break;
            case Type.point: if (spotLight.type != LightType.Point) spotLight.type = LightType.Point; break;
        }
        switch (lightEffect)
        {
            case Effect.flicker:
                lightOff = !lightOff;
                flickerTime = Random.Range(flickerMinRange, flickerMaxRange);
                if (spotLight.intensity != lightIntensity) spotLight.intensity = lightIntensity;
                lightTime = flickerTime;
                spotLight.enabled = lightOff;
                lightTimer = lightTime;
                break;

            case Effect.pulse:
                if (!spotLight.enabled) spotLight.enabled = true;
                pulseIn = !pulseIn;
                pulseInterval = pulseIn ? 1 : -1;
                lightTime = pulseTime;
                break;

            case Effect.strobe:
                strobeIn = !strobeIn;
                spotLight.enabled = strobeIn;
                lightTime = strobeTime;
                lightTimer = lightTime;
                if (spotLight.intensity != lightIntensity) spotLight.intensity = lightIntensity;
                break;

            case Effect.fade:
                if (!spotLight.enabled) spotLight.enabled = true;
                lightTime = fadeTime;
                lightTimer = lightTime;
                break;

            case Effect.none: 
                if(!spotLight.enabled) spotLight.enabled = true;
                if (spotLight.intensity != lightIntensity) spotLight.intensity = lightIntensity;
                return;
            case Effect.trigger:
                spotLight.enabled = lightActive;
                if (spotLight.intensity != lightIntensity) spotLight.intensity = lightIntensity;
                return;
        }
    }
    private void Timer()
    {
        switch (lightEffect)
        {
            case Effect.none: SetLightEffect(); return;
            case Effect.flicker:
                lightTimer -= time * flashSpeed;
                lightTimer = Mathf.Clamp(lightTimer, 0, lightTime);
                if (lightTimer == 0) SetLightEffect();
                break;
            case Effect.pulse:
                lightTimer += ((time * flashSpeed) * pulseInterval);
                lightTimer = Mathf.Clamp(lightTimer, 0, pulseTime);
                spotLight.intensity = Mathf.Lerp(0, lightIntensity, lightTimer);
                if (lightTimer == 0 || lightTimer == pulseTime) SetLightEffect();
                break;
            case Effect.strobe:
                lightTimer -= (time * flashSpeed);
                lightTimer = Mathf.Clamp(lightTimer, 0, lightTime);
                if (lightTimer == 0) SetLightEffect();
                break;
            case Effect.fade:
                lightTimer -= (time * flashSpeed);
                lightTimer = Mathf.Clamp(lightTimer, 0, lightTime);
                spotLight.intensity = Mathf.Lerp(0, lightIntensity, lightTimer);
                if (lightTimer == 0) SetLightEffect();
                break;
            case Effect.trigger: return;
        }
    }
    private void ActivateLight(bool active)
    {
        lightActive = active;
        SetLightEffect();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (lightEffect != Effect.trigger) return;
        if (other.gameObject.CompareTag("Player")) ActivateLight(!lightOn);
    }
}
