using UnityEngine;

public class PortalSystem : MonoBehaviour
{
    private PlayerSystem playerSystem;
    private AudioSystem audioSystem;
    public Transform portalPad;
    public AudioClip portalSoundFx;
    Vector3 destination;
    Quaternion rotation;
    void Start()
    {
        playerSystem = PlayerSystem.playerSystem;
        audioSystem = AudioSystem.audioSystem;
        destination = new Vector3(portalPad.position.x, portalPad.position.y + 5.5f, portalPad.position.z);
        rotation = new Quaternion(portalPad.rotation.x, portalPad.rotation.y, portalPad.rotation.z, portalPad.rotation.w);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            audioSystem.PlayAudioSource(portalSoundFx, 1, 1, 128);
            playerSystem.WarpPlayer(destination, rotation);
        }
    }
}
