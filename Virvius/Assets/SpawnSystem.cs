using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnSystem : MonoBehaviour
{
    public enum ObjectType { Grunt, Din, 
                             Health0, Health1, Health2, health3, 
                             Armor0, Armor1, Armor2, Armor3, 
                             Power0, Power1, Power2, Power3, Power4, Power5, 
                             Weapon1, Weapon2, Weapon3, Weapon4, Weapon5, Weapon6, Weapon7, Weapon8, Weapon9,
                             Ammo1, Ammo2, Ammo4, Ammo5, Ammo6, Ammo7, Ammo8, Ammo9, None }
    private PlayerSystem playerSystem;
    [SerializeField]
    private AudioClip warpSoundSfx;
    [SerializeField]
    private Transform[] spawnPool = new Transform[33];
    [SerializeField]
    private ParticleSystem warpEffect;
    [Header("Set Spawn Object")]
    [SerializeField]
    private ObjectType objectType0;
    [Space]
    [SerializeField]
    private ObjectType objectType1;
    [Space]
    [SerializeField]
    private ObjectType objectType2;
    [Space]
    [SerializeField]
    private ObjectType objectType3;
    [Space]
    [SerializeField]
    private ObjectType objectType4;
    [Space]
    [SerializeField]
    private ObjectType objectType5;
    [Space]
    [SerializeField]
    private ObjectType objectType6;
    [Space]
    [SerializeField]
    private ObjectType objectType7;
    private ObjectType[] objectType;
    [SerializeField]
    private Transform[] spawnPoints;
    [SerializeField]
    private AudioSource[] spawnAudioSrcs;
    [SerializeField]
    private GameObject[] spawnObjects = new GameObject[33];
    private GameObject spawnedObject;
    [SerializeField]
    private float spawnTime = 1;
    private float spawnTimer = 0;
    private float time = 0;
    [SerializeField]
    private int spawnAmount = 0;
    [SerializeField]
    private bool spawnAllAtOnce = false;    
    [SerializeField]
    private bool randomizePositions = false;
    private int objIndex = 0;
    private int posIndex = 0;
    private bool spawnFinished = false;
    private bool startSpawning = false;
    private int objectIndex = 0;

    private void Start()
    {
        warpEffect.gameObject.SetActive(false);
        objectType = new ObjectType[8] { objectType0, objectType1, objectType2, objectType3, objectType4, objectType5, objectType6, objectType7 };
    }

    private void Update()
    {
        time = Time.deltaTime;
        Spawn();
    }
    private void SpawnObjectType(ObjectType type) 
    {
        switch (type)
        {
            case ObjectType.Grunt: objIndex = 0;  break;
            case ObjectType.Din: objIndex = 1; break;
            case ObjectType.Health0: objIndex = 2; break;
            case ObjectType.Health1: objIndex = 3; break;
            case ObjectType.Health2: objIndex = 4; break;
            case ObjectType.health3: objIndex = 5; break;
            case ObjectType.Armor0: objIndex = 6; break;
            case ObjectType.Armor1: objIndex = 7; break;
            case ObjectType.Armor2: objIndex = 8; break;
            case ObjectType.Armor3: objIndex = 9; break;
            case ObjectType.Power0: objIndex = 10; break;
            case ObjectType.Power1: objIndex = 11; break;
            case ObjectType.Power2: objIndex = 12; break;
            case ObjectType.Power3: objIndex = 13; break;
            case ObjectType.Power4: objIndex = 14; break;
            case ObjectType.Power5: objIndex = 15; break;
            case ObjectType.Weapon1: objIndex = 16; break;
            case ObjectType.Weapon2: objIndex = 17; break;
            case ObjectType.Weapon3: objIndex = 18; break;
            case ObjectType.Weapon4: objIndex = 19; break;
            case ObjectType.Weapon5: objIndex = 20; break;
            case ObjectType.Weapon6: objIndex = 21; break;
            case ObjectType.Weapon7: objIndex = 22; break;
            case ObjectType.Weapon8: objIndex = 23; break;
            case ObjectType.Weapon9: objIndex = 24; break;
            case ObjectType.Ammo1: objIndex = 25; break;
            case ObjectType.Ammo2: objIndex = 26; break;
            case ObjectType.Ammo4: objIndex = 27; break;
            case ObjectType.Ammo5: objIndex = 28; break;
            case ObjectType.Ammo6: objIndex = 29; break;
            case ObjectType.Ammo7: objIndex = 30; break;
            case ObjectType.Ammo8: objIndex = 31; break;
            case ObjectType.Ammo9: objIndex = 32; break;
            
        }
        AccessSpawn(objIndex);
    }
    public void AccessSpawn(int index)
    {
        //access the spawnobject from pool or instantiate one into pool
        warpEffect.gameObject.SetActive(false);
        spawnedObject = AccessPool(index, spawnPool[index], spawnObjects[index]);
        if(!spawnedObject.activeInHierarchy) spawnedObject.SetActive(true);
        //Randomize the spawn position
        if (randomizePositions) posIndex = Random.Range(0, spawnPoints.Length);
        //Spawn in symetrical order
        else
        {
            if (posIndex < spawnPoints.Length)
                posIndex++;
        }
        //set the position of the spawned object
        if (index < 2)
        {
            //try to access the enemy systems and reset them, set the spawn point then engage player right away.
            if (spawnedObject.TryGetComponent(out GruntSystem gruntSystem)) 
            {
                gruntSystem.ResetObject(false);
                gruntSystem.navAgent.Warp(spawnPoints[posIndex].transform.position);
                gruntSystem.transform.localPosition = spawnPoints[posIndex].transform.localPosition;
                gruntSystem.EngagePlayer();
            }
            else  if (spawnedObject.TryGetComponent(out DinSystem dinSystem))
            {
                dinSystem.ResetObject(false);
                dinSystem.navAgent.Warp(spawnPoints[posIndex].transform.position);
                dinSystem.transform.localPosition = spawnPoints[posIndex].transform.localPosition;
                dinSystem.EngagePlayer();
            }
        }
        else spawnedObject.transform.position = spawnPoints[posIndex].transform.position;
        warpEffect.gameObject.SetActive(true);
        warpEffect.transform.localPosition = spawnedObject.transform.localPosition;
        if (spawnAllAtOnce) return;
        spawnAudioSrcs[posIndex].PlayOneShot(warpSoundSfx);


    }
    private void Spawn()
    {
        if (startSpawning)
        {
            if (!spawnAllAtOnce)
            {
                spawnTimer -= time;
                spawnTimer = Mathf.Clamp(spawnTimer, 0, spawnTime);
                if (spawnTimer == 0)
                {
                    if (playerSystem.isDead) startSpawning = false;
                    if (objectIndex < spawnAmount) 
                    {
                        spawnTimer = spawnTime;
                        if (objectType[objectIndex] != ObjectType.None)
                            SpawnObjectType(objectType[objectIndex]);
                        //Spawns the next item/object in the list
                        objectIndex++;
                        return;
                    }
                    startSpawning = false;
                }
            }
            else if(spawnAllAtOnce)
            {
                spawnAudioSrcs[posIndex].PlayOneShot(warpSoundSfx);
                for (int s = 0; s < spawnAmount; s++)
                    SpawnObjectType(objectType[s]);
                startSpawning = false;
            }
        }
    }
    private GameObject AccessPool(int index, Transform pool, GameObject instantiateObj)
    {
        for (int b = 0; b < pool.childCount; b++)
        {
            if (!pool.GetChild(b).gameObject.activeInHierarchy)
            {
                return pool.GetChild(b).gameObject;
            }
        }
        if (GameSystem.expandBulletPool)
        {
            GameObject newObj = Instantiate(instantiateObj, pool);
            return newObj;
        }
        else return null;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player") && !spawnFinished) 
        {
            if (playerSystem == null) playerSystem = other.GetComponent<PlayerSystem>();
            spawnTimer = spawnTime; 
            startSpawning = true; 
            spawnFinished = true; 
        }
    }
    public void ResetObject()
    {
        startSpawning = false;
        spawnFinished = false;
        spawnTimer = spawnTime;
        posIndex = 0;
        objIndex = 0;
        for (int s = 0; s < 32; s++)
        {
            for (int obj = 0; obj < spawnPool[s].childCount; obj++)
            {
                if (spawnPool[s].childCount > 0)
                {
                    if (spawnPool[s].GetChild(obj).gameObject.activeInHierarchy)
                    {
                        spawnPool[s].GetChild(obj).gameObject.SetActive(false);
                    }
                }
            }
        }

    }
}
