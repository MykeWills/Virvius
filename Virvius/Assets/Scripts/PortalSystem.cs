using UnityEngine;

public class PortalSystem : MonoBehaviour
{
    private PlayerSystem playerSystem;
    private OptionsSystem optionsSystem;
    private AudioSystem audioSystem;
    private GameSystem gameSystem;
    public Transform portalPad;
    public AudioClip portalSoundFx;
    [SerializeField]
    private int difficultyID = 0;
    [SerializeField]
    private bool setDifficulty = false;
    [SerializeField]
    private int sceneIndex = 0;
    [SerializeField]
    private bool loadSceneInstead = false;
    Vector3 destination;
    Quaternion rotation;
    void Start()
    {
        gameSystem = GameSystem.gameSystem;
        playerSystem = PlayerSystem.playerSystem;
        optionsSystem = OptionsSystem.optionsSystem;
        audioSystem = AudioSystem.audioSystem;
        if (!portalPad.gameObject.activeInHierarchy) return;
        destination = new Vector3(portalPad.position.x, portalPad.position.y + 5.5f, portalPad.position.z);
        rotation = new Quaternion(portalPad.rotation.x, portalPad.rotation.y, portalPad.rotation.z, portalPad.rotation.w);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (!loadSceneInstead)
            {
                audioSystem.PlayAudioSource(portalSoundFx, 1, 1, 128);
                if (setDifficulty) optionsSystem.SetSceneDifficulty(difficultyID);
                if (portalPad.gameObject.activeInHierarchy) playerSystem.WarpPlayer(destination, rotation);
            }
            else gameSystem.SetNewLevel(sceneIndex);
        }
    }
}
