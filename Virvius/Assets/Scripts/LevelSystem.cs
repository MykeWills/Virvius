using UnityEngine;
using System.Collections.Generic;
public class LevelSystem : MonoBehaviour
{
    private OptionsSystem optionsSystem;
    private GameSystem gameSystem;
    [Header("Level Systems")]
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
    [Space]
    [Header("Enemy Difficulty Sorters")]
    public GameObject[] difficulty0Enemies;
    public GameObject[] difficulty1Enemies;
    public GameObject[] difficulty2Enemies;
    public GameObject[] difficulty3Enemies;
    [Space]
    [Header("Level Result Camera")]
    [SerializeField]
    private Transform resultCameraPlaceholder;
    private Vector3 resultCameraPosition;
    private Quaternion resultCameraRotation;
    //===================================================================
    //====================Save/Loading===================================
    //===================================================================
    [HideInInspector]
    public bool[] ambushesActivated;
    [HideInInspector]
    public bool[] enemiesDeadGrunt;
    [HideInInspector]
    public bool[] enemiesDeadDin;
    [HideInInspector]
    public bool[] enemiesDeadElite;
    [HideInInspector]
    public bool[] switchesActivated;
    [HideInInspector]
    public bool[] pickupsObtained;
    [HideInInspector]
    public bool[] spawnersActivated;
    [HideInInspector]
    public bool[] messagesActivated;
    [HideInInspector]
    public bool[] cratesExploded;
    [HideInInspector]
    public bool[] explodingTriggersActivated;   
    [HideInInspector]
    public bool[] doorTriggersActivated;
    [HideInInspector]
    public Vector3[] enemiesPositionGrunt;
    [HideInInspector]
    public Vector3[] enemiesPositionDin;
    [HideInInspector]
    public Vector3[] enemiesPositionElite;
    [HideInInspector]
    public Quaternion[] enemiesRotationGrunt;
    [HideInInspector]
    public Quaternion[] enemiesRotationDin;
    [HideInInspector]
    public Quaternion[] enemiesRotationElite;
    private void Start()
    {
        ambushesActivated = new bool[ambushSystems.Length];
        enemiesDeadGrunt = new bool[gruntSystems.Length];
        enemiesDeadDin = new bool[dinSystems.Length];
        enemiesDeadElite = new bool[eliteSystems.Length];
        switchesActivated = new bool[switches.Length];
        pickupsObtained = new bool[pickupItems.Length];
        spawnersActivated = new bool[spawnSystems.Length];
        messagesActivated = new bool[messageTriggerSystems.Length];
        cratesExploded = new bool[explodingCrateSystems.Length];
        explodingTriggersActivated = new bool[explodingTriggerSystems.Length];
        doorTriggersActivated = new bool[doorTriggers.Length];
        enemiesPositionGrunt = new Vector3[gruntSystems.Length];
        enemiesRotationGrunt = new Quaternion[gruntSystems.Length];
        enemiesPositionDin = new Vector3[dinSystems.Length];
        enemiesRotationDin = new Quaternion[dinSystems.Length];
        enemiesPositionElite = new Vector3[eliteSystems.Length];
        enemiesRotationElite = new Quaternion[eliteSystems.Length];
    }
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
    public void LoadLevel()
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
                if (messageTriggerSystems[a].isSecret) gameSystem.totalLevelSecrets++;
                messageTriggerSystems[a].ResetObject();
            }
        for (int a = 0; a < switches.Length; a++)
            if (switches[a] != null || switches[a].gameObject.activeInHierarchy) switches[a].ResetObject();
        for (int a = 0; a < doorSystems.Length; a++)
            if (doorSystems[a] != null || doorSystems[a].gameObject.activeInHierarchy) doorSystems[a].ResetObject();
        for (int a = 0; a < spawnSystems.Length; a++)
            if (spawnSystems[a] != null || spawnSystems[a].gameObject.activeInHierarchy) spawnSystems[a].ResetObject();
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

        ambushesActivated = new bool[ambushSystems.Length];
        enemiesDeadGrunt = new bool[gruntSystems.Length];
        enemiesDeadDin = new bool[dinSystems.Length];
        enemiesDeadElite = new bool[eliteSystems.Length];
        switchesActivated = new bool[switches.Length];
        pickupsObtained = new bool[pickupItems.Length];
        spawnersActivated = new bool[spawnSystems.Length];
        messagesActivated = new bool[messageTriggerSystems.Length];
        cratesExploded = new bool[explodingCrateSystems.Length];
        explodingTriggersActivated = new bool[explodingTriggerSystems.Length];
        doorTriggersActivated = new bool[doorTriggers.Length];

        ambushesActivated = gameSystem.ambushesActivated;
        enemiesDeadGrunt = gameSystem.enemiesDeadGrunt;
        enemiesDeadDin = gameSystem.enemiesDeadDin;
        enemiesDeadElite = gameSystem.enemiesDeadElite;
        switchesActivated = gameSystem.switchesActivated;
        pickupsObtained = gameSystem.pickupsObtained;
        spawnersActivated = gameSystem.spawnersActivated;
        messagesActivated = gameSystem.messagesActivated;
        cratesExploded = gameSystem.cratesExploded;
        explodingTriggersActivated = gameSystem.explodingTriggersActivated;
        doorTriggersActivated = gameSystem.doorTriggersActivated;

        for (int a = 0; a < ambushesActivated.Length; a++)
            if (ambushesActivated[a]) ambushSystems[a].ActivateObjectState();
        for (int a = 0; a < enemiesDeadGrunt.Length; a++)
            if (enemiesDeadGrunt[a]) gruntSystems[a].ActivateObjectState(gameSystem.playerData.enemyGPosition[a], gameSystem.playerData.enemyGRotation[a]);
        for (int a = 0; a < enemiesDeadDin.Length; a++)
            if (enemiesDeadDin[a]) dinSystems[a].ActivateObjectState(gameSystem.playerData.enemyDPosition[a], gameSystem.playerData.enemyDRotation[a]);
        for (int a = 0; a < enemiesDeadElite.Length; a++)
            if (enemiesDeadElite[a]) eliteSystems[a].ActivateObjectState(gameSystem.playerData.enemyEPosition[a], gameSystem.playerData.enemyERotation[a]);
        for (int a = 0; a < switchesActivated.Length; a++)
            if (switchesActivated[a]) switches[a].ActivateObjectState();
        for (int a = 0; a < pickupsObtained.Length; a++)
            if (pickupsObtained[a]) pickupItems[a].SetActive(false);
        for (int a = 0; a < spawnersActivated.Length; a++)
            if (spawnersActivated[a]) spawnSystems[a].ActivateObjectState();
        for (int a = 0; a < messagesActivated.Length; a++)
            if (messagesActivated[a]) messageTriggerSystems[a].ActivateObjectState();
        for (int a = 0; a < cratesExploded.Length; a++)
            if (cratesExploded[a]) explodingCrateSystems[a].ActivateObjectState();
        for (int a = 0; a < explodingTriggersActivated.Length; a++)
            if (explodingTriggersActivated[a]) explodingTriggerSystems[a].ActivateObjectState();
        for (int a = 0; a < doorTriggersActivated.Length; a++)
            if (doorTriggersActivated[a]) doorTriggers[a].ActivateObjectState();

        gameSystem.SetPreviouslyDroppedItems();
    }
    public void SaveLevel()
    {
        for(int a = 0; a < ambushesActivated.Length; a++)
            if (ambushSystems[a].ambushActivated) ambushesActivated[a] = ambushSystems[a].ambushActivated;
        for (int a = 0; a < enemiesDeadGrunt.Length; a++)
            if(gruntSystems[a].isDead) enemiesDeadGrunt[a] = gruntSystems[a].isDead;
        for (int a = 0; a < enemiesDeadDin.Length; a++)
            if (dinSystems[a].isDead) enemiesDeadDin[a] = dinSystems[a].isDead;
        for (int a = 0; a < enemiesDeadElite.Length; a++)
            if (eliteSystems[a].isDead) enemiesDeadElite[a] = eliteSystems[a].isDead;
        for (int a = 0; a < switchesActivated.Length; a++)
            if (switches[a].switchActivated) switchesActivated[a] = switches[a].switchActivated;
        for (int a = 0; a < pickupsObtained.Length; a++)
            if (!pickupItems[a].activeInHierarchy) pickupsObtained[a] = true;
        for (int a = 0; a < spawnersActivated.Length; a++)
            if (spawnSystems[a].spawnActivated) spawnersActivated[a] = spawnSystems[a].spawnActivated;
        for (int a = 0; a < messagesActivated.Length; a++)
            if (messageTriggerSystems[a].messageActivated) messagesActivated[a] = messageTriggerSystems[a].messageActivated;
        for (int a = 0; a < cratesExploded.Length; a++)
            if (explodingCrateSystems[a].crateExploded) cratesExploded[a] = explodingCrateSystems[a].crateExploded;
        for (int a = 0; a < explodingTriggersActivated.Length; a++)
            if (explodingTriggerSystems[a].explodingTriggerActivated) explodingTriggersActivated[a] = explodingTriggerSystems[a].explodingTriggerActivated;
        for (int a = 0; a < doorTriggersActivated.Length; a++)
            if (doorTriggers[a].doorTriggerActivated) doorTriggersActivated[a] = doorTriggers[a].doorTriggerActivated;
        for (int a = 0; a < enemiesPositionGrunt.Length; a++)
            enemiesPositionGrunt[a] = gruntSystems[a].transform.position;
        for (int a = 0; a < enemiesRotationGrunt.Length; a++)
            enemiesRotationGrunt[a] = gruntSystems[a].transform.rotation;
        for (int a = 0; a < enemiesPositionDin.Length; a++)
            enemiesPositionDin[a] = dinSystems[a].transform.position;
        for (int a = 0; a < enemiesRotationDin.Length; a++)
            enemiesRotationDin[a] = dinSystems[a].transform.rotation;
        for (int a = 0; a < enemiesPositionElite.Length; a++)
            enemiesPositionElite[a] = eliteSystems[a].transform.position;
        for (int a = 0; a < enemiesRotationElite.Length; a++)
            enemiesRotationElite[a] = eliteSystems[a].transform.rotation;
    }

    public void ActivateEnvironment()
    {
        for (int a = 0; a < levelEnvironment.Length; a++)
            if (!levelEnvironment[a].isPlaying) levelEnvironment[a].Play();
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
            case 4:
                {
                    //for testing purposes to check if and difficulty active enemies are missing from list
                    for (int e = 0; e < difficulty0Enemies.Length; e++) difficulty0Enemies[e].SetActive(false);
                    for (int e = 0; e < difficulty1Enemies.Length; e++) difficulty1Enemies[e].SetActive(false);
                    for (int e = 0; e < difficulty2Enemies.Length; e++) difficulty2Enemies[e].SetActive(false);
                    for (int e = 0; e < difficulty3Enemies.Length; e++) difficulty3Enemies[e].SetActive(false);
                    gameSystem.totalLevelEnemies = 0;
                    break;
                }
        }

    }
    private int ActiveDifficulty()
    {
        if (optionsSystem == null) optionsSystem = OptionsSystem.optionsSystem;
        for(int d = 0; d < optionsSystem.difficultyActive.Length; d++)
            if (optionsSystem.difficultyActive[d]) return d;
        return 3;
    }
}
