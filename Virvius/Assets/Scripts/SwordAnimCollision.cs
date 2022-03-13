using UnityEngine;

public class SwordAnimCollision : MonoBehaviour
{
    private InputSystem inputSystem;
    private PowerupSystem powerupSystem;
    private AudioSystem audioSsytem;
    private EnvironmentSystem environmentSystem;
    [SerializeField]
    private float[] returnRate;
    [SerializeField]
    float tiltAngle = -15;
    public MeshCollider swordCollider;
    public AudioClip[] swordSwipeFx = new AudioClip[2];
    private void Start()
    {
        inputSystem = InputSystem.inputSystem;
        audioSsytem = AudioSystem.audioSystem;
        powerupSystem = PowerupSystem.powerupSystem;
        environmentSystem = EnvironmentSystem.environmentSystem;
    }
    public void SetSwordCollision(int index)
    {
        if(index < 1) swordCollider.enabled = false;
        else swordCollider.enabled = true;
    }
    public void PlaySwordSwipe()
    {
        if (powerupSystem.powerEnabled[powerupSystem.powerIndex])
            audioSsytem.PlayAudioSource(powerupSystem.powerSound[powerupSystem.powerIndex], 1, 0.5f, 128);
        if(environmentSystem.headUnderWater)
            audioSsytem.PlayAudioSource(swordSwipeFx[1], 1, 0.7f, 150);
        else
            audioSsytem.PlayAudioSource(swordSwipeFx[0], 1, 0.7f, 150);
    }
   public void PlayHeadAnim()
    {
        inputSystem.AnimationEffect(new Vector3(0, tiltAngle, 0), returnRate[0], returnRate[1]);
    }
}
