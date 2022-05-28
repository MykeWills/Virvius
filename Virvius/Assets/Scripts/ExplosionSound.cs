using UnityEngine;

public class ExplosionSound : MonoBehaviour
{
    private EnvironmentSystem environmentSystem;
    [SerializeField]
    private AudioClip[] explosionFxs = new AudioClip[2];
    private AudioSource audioSource;
    private void OnEnable()
    {
        if (audioSource == null) audioSource = GetComponent<AudioSource>();
        if (environmentSystem == null) environmentSystem = EnvironmentSystem.environmentSystem;
        audioSource.loop = false;
        audioSource.volume = 1;
        audioSource.pitch = Random.Range(0.9f, 1.1f);
        int soundID = 0;
        soundID = environmentSystem.headUnderWater ? 1 : 0;
        audioSource.clip = explosionFxs[soundID];
        audioSource.Play();
    }
}
