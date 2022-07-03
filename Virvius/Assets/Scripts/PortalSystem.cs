using UnityEngine;
using Rewired;
public class PortalSystem : MonoBehaviour
{
    private Player inputPlayer;
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
    private bool showResultsScreen = false;
    [SerializeField]
    private bool loadSceneInstead = false;
    Vector3 destination;
    Quaternion rotation;
    void Start()
    {
        inputPlayer = ReInput.players.GetPlayer(0);
        gameSystem = GameSystem.gameSystem;
        playerSystem = PlayerSystem.playerSystem;
        optionsSystem = OptionsSystem.optionsSystem;
        audioSystem = AudioSystem.audioSystem;
        if (!portalPad.gameObject.activeInHierarchy) return;
        destination = new Vector3(portalPad.position.x, portalPad.position.y + 5.5f, portalPad.position.z);
        rotation = new Quaternion(portalPad.rotation.x, portalPad.rotation.y, portalPad.rotation.z, portalPad.rotation.w);
    }
    private void Update()
    {
        if (!showResultsScreen) return;
        Results();
    }
    private void Results()
    {
        if (!gameSystem.showResults) return;
        if (inputPlayer.GetButtonUp("Start") || inputPlayer.GetButtonUp("Select"))
        {
            gameSystem.ShowResults(false);
            gameSystem.showResults = false;
            gameSystem.SetNewLevel(sceneIndex);
        }
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
            else 
            { 
                if(!showResultsScreen) gameSystem.SetNewLevel(sceneIndex); 
                else gameSystem.ShowResults(true);
            }
        }
    }
}
