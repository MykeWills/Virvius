using UnityEngine;

public class CoSwitchSystem : MonoBehaviour
{
    private SwitchSystem switchSystem;
    private BoxCollider boxCollider;
    private AudioSystem audioSystem;
    private string[] tags = new string[11]
   {
        "Sword",
        "ShotgunBullet",
        "SpikeBullet",
        "MinigunBullet",
        "GrenadeBullet",
        "RocketBullet",
        "RailBullet",
        "PhotonBullet",
        "SigmaBullet",
        "ObstacleBullet",
        "RocketBulletMini",
   };
    private void Start()
    {
        audioSystem = AudioSystem.audioSystem;
        switchSystem = transform.parent.GetComponent<SwitchSystem>();
        boxCollider = GetComponent<BoxCollider>();
        if (switchSystem.switchType == SwitchSystem.SwitchType.Press ||
            switchSystem.switchType == SwitchSystem.SwitchType.Step)
            boxCollider.isTrigger = true;
        else boxCollider.isTrigger = false;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player") || other.gameObject.CompareTag("Crate"))
        {
            if (switchSystem.switchSubType == SwitchSystem.SwitchSubType.Counter)
            {
                if (!switchSystem.isCoActive)
                {
                    audioSystem.PlayAudioSource(switchSystem.pressSoundFx, 1, 1, 128);
                    switchSystem.CoSwitchCounter();
                }
            }
            else
            {
                if (switchSystem.isActive)
                {
                    audioSystem.PlayAudioSource(switchSystem.pressSoundFx, 1, 1, 128);
                    switchSystem.DeactivateSwitch(switchSystem.switchSubType);
                }
            }
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        string tag = collision.gameObject.tag;
        for (int t = 0; t < tags.Length; t++)
        {
            if (tag == tags[t])
            {
                if (switchSystem.switchType == SwitchSystem.SwitchType.Shoot)
                {
                    if (switchSystem.switchSubType == SwitchSystem.SwitchSubType.Counter)
                    {
                        if (!switchSystem.isCoActive)
                        {
                            audioSystem.PlayAudioSource(switchSystem.pressSoundFx, 1, 1, 128);
                            switchSystem.CoSwitchCounter();
                        }
                    }
                    else
                    {
                        if (switchSystem.isActive)
                        {
                            audioSystem.PlayAudioSource(switchSystem.pressSoundFx, 1, 1, 128);
                            switchSystem.DeactivateSwitch(switchSystem.switchSubType);
                        }
                    }
                }
                break;
            }
        }
    }
}