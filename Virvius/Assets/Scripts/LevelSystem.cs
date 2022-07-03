using UnityEngine;
using System.Collections.Generic;
public class LevelSystem : MonoBehaviour
{
    private OptionsSystem optionsSystem;
    private GameSystem gameSystem;
    public AmbushSystem[] ambushSystems;
    public EnemyGSystem[] gruntSystems;
    public EnemyDSystem[] dinSystems;
    public EnemyESystem[] eliteSystems;
    public SecretDoorSystem[] secretDoorSystems;
    public SwitchSystem[] switches;
    public GameObject[] pickupItems;
    public MessageTriggerSystem[] messageTriggerSystems;
    public AudioSource[] levelEnvironment;
    public DoorSystem[] doorSystems;
    public SpawnSystem[] spawnSystems;
    public List<Transform> enemyTargets;
    public CraneDropSystem[] craneDropSystem;
    public ExplodingCrateSystem[] explodingCrateSystems;
    public ExplodeTrigger[] explodingTriggerSystems;
    public DoorTrigger[] doorTriggers;
    public GameObject[] difficulty0Enemies;
    public GameObject[] difficulty1Enemies;
    public GameObject[] difficulty2Enemies;
    public GameObject[] difficulty3Enemies;
    [SerializeField]
    private Transform resultCameraPlaceholder;
    private Vector3 resultCameraPosition;
    private Quaternion resultCameraRotation;
    public void ResetLevel()
    {

        if (gameSystem == null) gameSystem = GameSystem.gameSystem;
        gameSystem.totalLevelSecrets = 0;
        gameSystem.totalLevelEnemies = 0;
        if (resultCameraPlaceholder != null)
        {
            resultCameraPosition = resultCameraPlaceholder.position;
            resultCameraRotation = resultCameraPlaceholder.rotation;
            gameSystem.resultCamPosition = resultCameraPosition;
            gameSystem.resultCamRotation = resultCameraRotation;
        }
        SetDifficultyEnemies(ActiveDifficulty());
        gameSystem.totalLevelSecrets = secretDoorSystems.Length;
        for (int a = 0; a < ambushSystems.Length; a++)
            if (ambushSystems[a] != null || ambushSystems[a].gameObject.activeInHierarchy) ambushSystems[a].ResetObject();
        for (int a = 0; a < gruntSystems.Length; a++)
            if (gruntSystems[a] != null || gruntSystems[a].gameObject.activeInHierarchy) gruntSystems[a].ResetObject(true);
        for (int a = 0; a < dinSystems.Length; a++)
            if (dinSystems[a] != null || dinSystems[a].gameObject.activeInHierarchy) dinSystems[a].ResetObject(true);
        for (int a = 0; a < eliteSystems.Length; a++)
            if (eliteSystems[a] != null || eliteSystems[a].gameObject.activeInHierarchy) eliteSystems[a].ResetObject(true);
        for (int a = 0; a < secretDoorSystems.Length; a++)
            if (secretDoorSystems[a] != null || secretDoorSystems[a].gameObject.activeInHierarchy) secretDoorSystems[a].ResetObject();
        for (int a = 0; a < messageTriggerSystems.Length; a++)
            if (messageTriggerSystems[a] != null || messageTriggerSystems[a].gameObject.activeInHierarchy) 
            { 
                if(messageTriggerSystems[a].isSecret) gameSystem.totalLevelSecrets++;
                messageTriggerSystems[a].ResetObject(); 
            }
        for (int a = 0; a < switches.Length; a++)
            if (switches[a] != null || switches[a].gameObject.activeInHierarchy) switches[a].ResetObject(); 
        for (int a = 0; a < doorSystems.Length; a++)
            if (doorSystems[a] != null || doorSystems[a].gameObject.activeInHierarchy) doorSystems[a].ResetObject();
        for (int a = 0; a < spawnSystems.Length; a++)
            if(spawnSystems[a] != null || spawnSystems[a].gameObject.activeInHierarchy) spawnSystems[a].ResetObject();
        for (int a = 0; a < pickupItems.Length; a++)
            pickupItems[a].SetActive(true);
        for (int a = 0; a < craneDropSystem.Length; a++)
            craneDropSystem[a].ResetObject();  
        for (int a = 0; a < explodingCrateSystems.Length; a++)
            explodingCrateSystems[a].ResetObject();  
        for (int a = 0; a < explodingTriggerSystems.Length; a++)
            explodingTriggerSystems[a].ResetObject();
        for (int a = 0; a < doorTriggers.Length; a++)
            doorTriggers[a].ResetObject();

        ActivateEnvironment();
    }
    public void ActivateEnvironment()
    {
        for (int a = 0; a < levelEnvironment.Length; a++)
        {
            if (!levelEnvironment[a].isPlaying) levelEnvironment[a].Play();
        }
    }
    private void SetDifficultyEnemies(int index)
    {
        if (difficulty0Enemies == null) return;
        if (difficulty1Enemies == null) return;
        if (difficulty2Enemies == null) return;
        if (difficulty3Enemies == null) return;
        switch (index)
        {
            case 0: 
                {
                    for (int e = 0; e < difficulty0Enemies.Length; e++) difficulty0Enemies[e].SetActive(true);
                    for (int e = 0; e < difficulty1Enemies.Length; e++) difficulty1Enemies[e].SetActive(false);
                    for (int e = 0; e < difficulty2Enemies.Length; e++) difficulty2Enemies[e].SetActive(false);
                    for (int e = 0; e < difficulty3Enemies.Length; e++) difficulty3Enemies[e].SetActive(false);
                    gameSystem.totalLevelEnemies = difficulty0Enemies.Length;
                    break; 
                }
            case 1:
                {
                    for (int e = 0; e < difficulty0Enemies.Length; e++) difficulty0Enemies[e].SetActive(true);
                    for (int e = 0; e < difficulty1Enemies.Length; e++) difficulty1Enemies[e].SetActive(true);
                    for (int e = 0; e < difficulty2Enemies.Length; e++) difficulty2Enemies[e].SetActive(false);
                    for (int e = 0; e < difficulty3Enemies.Length; e++) difficulty3Enemies[e].SetActive(false);
                    gameSystem.totalLevelEnemies = difficulty0Enemies.Length + difficulty1Enemies.Length; 
                    break;
                }
            case 2:
                {
                    for (int e = 0; e < difficulty0Enemies.Length; e++) difficulty0Enemies[e].SetActive(true);
                    for (int e = 0; e < difficulty1Enemies.Length; e++) difficulty1Enemies[e].SetActive(true);
                    for (int e = 0; e < difficulty2Enemies.Length; e++) difficulty2Enemies[e].SetActive(true);
                    for (int e = 0; e < difficulty3Enemies.Length; e++) difficulty3Enemies[e].SetActive(false);
                    gameSystem.totalLevelEnemies = difficulty0Enemies.Length + difficulty1Enemies.Length + difficulty2Enemies.Length;
                    break;
                }
            case 3:
                {
                    for (int e = 0; e < difficulty0Enemies.Length; e++) difficulty0Enemies[e].SetActive(true);
                    for (int e = 0; e < difficulty1Enemies.Length; e++) difficulty1Enemies[e].SetActive(true);
                    for (int e = 0; e < difficulty2Enemies.Length; e++) difficulty2Enemies[e].SetActive(true);
                    for (int e = 0; e < difficulty3Enemies.Length; e++) difficulty3Enemies[e].SetActive(true);
                    gameSystem.totalLevelEnemies = difficulty0Enemies.Length + difficulty1Enemies.Length + difficulty2Enemies.Length + difficulty3Enemies.Length;
                   
                    break;
                }
        }
        Debug.Log(gameSystem.totalLevelEnemies);
    }
    private int ActiveDifficulty()
    {
        if (optionsSystem == null) optionsSystem = OptionsSystem.optionsSystem;
        for(int d = 0; d < optionsSystem.difficultyActive.Length; d++)
        {
            if (optionsSystem.difficultyActive[d]) return d;
        }
        return 3;
    }
}
