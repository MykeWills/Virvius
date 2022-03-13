using UnityEngine;
using System.Collections.Generic;
public class LevelSystem : MonoBehaviour
{
    public AmbushSystem[] ambushSystems;
    public GruntSystem[] gruntSystems;
    public DinSystem[] dinSystems;
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

    //private void Start()
    //{
    //    enemyTargets = new List<Transform>(gruntSystems.Length + dinSystems.Length);
    //}
    //public void BuildEnemyDatabase()
    //{
    //    if (enemyTargets.Count > 0) enemyTargets.Clear();
    //    for (int gs = 0; gs < gruntSystems.Length; gs++) 
    //    { 
    //        if (!gruntSystems[gs].isDead && !enemyTargets.Contains(gruntSystems[gs].transform)) enemyTargets.Add(gruntSystems[gs].transform); 
    //        else enemyTargets.Remove(gruntSystems[gs].transform); 
    //    }
    //    for (int gs = 0; gs < dinSystems.Length; gs++) 
    //    { 
    //        if (!dinSystems[gs].isDead && !enemyTargets.Contains(dinSystems[gs].transform)) enemyTargets.Add(dinSystems[gs].transform); 
    //        else enemyTargets.Remove(dinSystems[gs].transform); 
    //    }
    //}
    public void ResetLevel()
    {
        for (int a = 0; a < ambushSystems.Length; a++)
            if (ambushSystems[a] != null || ambushSystems[a].gameObject.activeInHierarchy) ambushSystems[a].ResetObject();
        for (int a = 0; a < gruntSystems.Length; a++)
            if (gruntSystems[a] != null || gruntSystems[a].gameObject.activeInHierarchy) gruntSystems[a].ResetObject(true);
        for (int a = 0; a < dinSystems.Length; a++)
            if (dinSystems[a] != null || dinSystems[a].gameObject.activeInHierarchy) dinSystems[a].ResetObject(true);
        for (int a = 0; a < secretDoorSystems.Length; a++)
            if (secretDoorSystems[a] != null || secretDoorSystems[a].gameObject.activeInHierarchy) secretDoorSystems[a].ResetObject();
        for (int a = 0; a < messageTriggerSystems.Length; a++)
            if (messageTriggerSystems[a] != null || messageTriggerSystems[a].gameObject.activeInHierarchy) messageTriggerSystems[a].ResetObject();
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
        ActivateEnvironment();
    }
    public void ActivateEnvironment()
    {
        for (int a = 0; a < levelEnvironment.Length; a++)
        {
            if (!levelEnvironment[a].isPlaying) levelEnvironment[a].Play();
        }
    }
}
