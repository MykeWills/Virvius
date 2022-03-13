using UnityEngine;
public class AudioSystem : MonoBehaviour
{ 
    public static AudioSystem audioSystem;
    public AudioSource[] altAudioSources;
    public AudioSource V_audioSrc;
    public AudioSource M_audioSrc;
    public AudioSource E_audioSrc;
    
    public Transform soundFxPool;
    public GameObject soundPrefab;
    public bool expandPool = true;
    private static float volPercentage = 0;
    private static bool isFading = false;
    private static bool fadeVolIn = true;
    private static float fadeTimeInSeconds = 1;
    public void Awake()
    {
        audioSystem = this;
    }
    public void PlayGameMusic(AudioClip clip, float pitch, float volume, bool loop)
    {

        M_audioSrc.clip = clip;
        M_audioSrc.pitch = pitch;
        M_audioSrc.volume = volume;
        M_audioSrc.loop = loop;
        M_audioSrc.Play();
    }
    //public void SetGameMusic(AudioClip clip)
    //{
    //    audioSystem.M_audioSrc.clip = clip;
    //    if (!audioSystem.M_audioSrc.isPlaying) audioSystem.M_audioSrc.Play();
    //}
    public void PlayGameEnvironment(AudioClip clip, float pitch, float volume, bool loop)
    {
        
        E_audioSrc.clip = clip;
        E_audioSrc.pitch = pitch;
        E_audioSrc.volume = volume;
        E_audioSrc.loop = loop;
        E_audioSrc.Play();

    }
    private void Update()
    {
        AudioUpdate(M_audioSrc, fadeVolIn, fadeTimeInSeconds);
    }
    public void PlayAltAudioSource(int sourceNum, AudioClip clip, float pitch, float volume, bool loop, bool active)
    {
       
        if (active)
        {
            altAudioSources[sourceNum].clip = clip;
            altAudioSources[sourceNum].pitch = pitch;
            altAudioSources[sourceNum].volume = volume;
            altAudioSources[sourceNum].loop = loop;
            if (!altAudioSources[sourceNum].isPlaying) altAudioSources[sourceNum].Play();
        }
        else
        {
            altAudioSources[sourceNum].clip = null;
            altAudioSources[sourceNum].pitch = 1;
            altAudioSources[sourceNum].volume = 1;
            altAudioSources[sourceNum].loop = false;
            if (altAudioSources[sourceNum].isPlaying) altAudioSources[sourceNum].Stop();
        }
    }
    public void PlayAudioSource(AudioClip clip, float pitch, float volume, int priority)
    {
        AudioSource source = GetPooledAudioSource();
        if (source != null)
        {
            source.pitch = pitch;
            source.volume = volume;
            source.clip = clip;
            source.priority = priority;
            source.Play();
        }
    }
    public AudioSource GetPooledAudioSource()
    {
        for (int a = 0; a < soundFxPool.childCount; a++)
        {
            AudioSource selectedAudio = soundFxPool.GetChild(a).GetComponent<AudioSource>();
            if (!selectedAudio.isPlaying)
                return selectedAudio;
        }
        if (expandPool)
        {
            GameObject newAudio = Instantiate(soundPrefab, soundFxPool);
            AudioSource selectedNewAudio = newAudio.GetComponent<AudioSource>();
            return selectedNewAudio;
        }
        else
            return null;
    }
    public void FadeMusic(bool fadeIn, float seconds)
    {
        isFading = true;
        fadeVolIn = fadeIn;
        fadeTimeInSeconds = seconds;
    }
    public void MusicPlayStop(bool play)
    {
        if (play) { if (!M_audioSrc.isPlaying) M_audioSrc.Play(); }
        else { if (M_audioSrc.isPlaying) M_audioSrc.Stop(); }
    }
    public void VideoPlayStop(bool play)
    {
        if (play)
        { 
            if (!V_audioSrc.isPlaying) 
            { 
                if(V_audioSrc.isActiveAndEnabled) 
                    V_audioSrc.Play(); 
            } 
        }
        else { if (!V_audioSrc.isPlaying) V_audioSrc.Stop(); }
    }
    public void EnvironmentPlayStop(bool play)
    {
        if (play) { if (!E_audioSrc.isPlaying) E_audioSrc.Play(); }

        else { if (!E_audioSrc.isPlaying) E_audioSrc.Stop(); }
    }
    public void MusicPauseUnPause(bool pause)
    {
        if (pause) { if (M_audioSrc.isPlaying) M_audioSrc.Pause(); }
        else { if (!M_audioSrc.isPlaying) M_audioSrc.Play(); }

    }
    public void OnSelectable(AudioClip clip)
    {
        PlayAudioSource(clip, 1, 1, 128);
    }
    private void AudioUpdate(AudioSource audio, bool fadeIn, float time)
    {
        if (!isFading || time == 0) return;
        float speedAbsolute = 1.0f / time;  // speed desired by user
        float speedDirection = speedAbsolute * (fadeIn ? +1 : -1);  // + or -
        float deltaVolume = Time.unscaledDeltaTime * speedDirection;  // how much volume changes in 1 frame
        volPercentage += deltaVolume;  // implement change
        volPercentage = Mathf.Clamp(volPercentage, 0.0f, 1.0f);  // make sure you're in 0..100% 
        audio.volume = volPercentage;
        if (volPercentage == 0.0f || volPercentage == 1.0f) isFading = false;
    }
    public void AlterMusicStandalone(float pitch, float vol)
    {
        M_audioSrc.pitch = pitch;
        M_audioSrc.volume = vol;
    }
    
}
