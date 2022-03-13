using UnityEngine;

public class SwordCollision : MonoBehaviour
{
    private EnvironmentSystem environmentSystem;
    [SerializeField]
    private GameObject[] sparkPrefab;
    private Transform[] sparkPool;
    public AudioClip[] swordClangFx;
    private GruntSystem enemyGrunt;
    private DinSystem enemyDin;
    private AudioSystem audioSystem;
    private new ParticleSystem particleSystem;
    private RaycastHit hit;
    private Vector3 hitPosition;
    [HideInInspector]
    public SparkType sparkType;
    private Transform sPool;
    private string[] tags = new string[5]
    {
        "Enemy",
        "Water",
        "Lava",
        "Acid",
        "Player",
    };
    public enum SparkType { spark, blood}
    public enum CollisionType { Misc, Enemies, None };
    public void Start()
    {
        environmentSystem = EnvironmentSystem.environmentSystem;
    }
    public void SetCollisionType(CollisionType type)
    {
        float rnd = Random.Range(0.9f, 1.3f);
        if (audioSystem == null) audioSystem = AudioSystem.audioSystem;
        switch (type)
        {
            case CollisionType.Misc:
                {
                    AudioClip clip = environmentSystem.headUnderWater ? swordClangFx[2] : swordClangFx[0];
                    audioSystem.PlayAltAudioSource(2, clip, rnd, 1, false, true);
                    break;
                }
            case CollisionType.Enemies:
                {
                    audioSystem.PlayAltAudioSource(2, swordClangFx[1], rnd, 1, false, true);
                    break;
                }
            case CollisionType.None:
                {
                    //Dont play a sound at all.
                    break;
                }
        }
    }
    private void OnCollisionEnter(Collision collision)
    {

        string tag = collision.gameObject.tag;
        hitPosition = collision.contacts[0].point;
        sparkType = SparkType.spark;
        switch (tag)
        {
            case "Enemy":
                {
                    sparkType = SparkType.blood;
                    
                    if (Grunt(collision.gameObject))
                    {
                        if (!enemyGrunt.isDead) 
                        { 
                            SetCollisionType(CollisionType.Enemies); 
                        }
                    }
                    EnableSpark();
                    return;
                }
            case "DinEnemy":
                {
                    sparkType = SparkType.blood;

                    if (Din(collision.gameObject))
                    {
                        if (!enemyDin.isDead)
                        {
                            SetCollisionType(CollisionType.Enemies);
                        }
                    }
                    EnableSpark();
                    return;
                }
        }
        SetCollisionType(CollisionType.Misc);
        EnableSpark();
    }
    public bool Grunt(GameObject enemyObject)
    {
        if (enemyObject.GetComponent<GruntSystem>() == null) return false;
        else
        {
            enemyGrunt = enemyObject.GetComponent<GruntSystem>();
            return true;
        }
    }
    public bool Din(GameObject enemyObject)
    {
        if (enemyObject.GetComponent<DinSystem>() == null) return false;
        else
        {
            enemyDin = enemyObject.GetComponent<DinSystem>();
            return true;
        }
    }
    public void EnableSpark()
    {
        GameObject spark = AccessSpark();
        if (particleSystem != spark.GetComponent<ParticleSystem>()) particleSystem = spark.GetComponent<ParticleSystem>();
        Vector3 fwd = transform.TransformDirection(Vector3.forward);
        if (Physics.Raycast(transform.position, fwd, out hit, Mathf.Infinity))
            spark.transform.position = hitPosition;
        spark.SetActive(true);
        particleSystem.Play();

    }
    private GameObject AccessSpark()
    {
        int index = 0;
        if (sPool == null)
        {
            sPool = GameObject.Find("GameSystem/Game/ObjectPool/SparkPool").transform;
            sparkPool = new Transform[sPool.childCount];
            for (int s = 0; s < sPool.childCount; s++) sparkPool[s] = sPool.GetChild(s);
        }
        switch (sparkType)
        {
            case SparkType.spark: index = 0; break;
            case SparkType.blood: index = 1; break;
        }
        for (int b = 0; b < sparkPool[index].childCount; b++)
        {
            if (!sparkPool[index].GetChild(b).gameObject.activeInHierarchy)
                return sparkPool[index].GetChild(b).gameObject;
        }
        if (GameSystem.expandBulletPool)
        {
            GameObject newSpark = Instantiate(sparkPrefab[index], sparkPool[index]);
            return newSpark;
        }
        else
            return null;
    }
}
