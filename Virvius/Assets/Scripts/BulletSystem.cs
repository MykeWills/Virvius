using UnityEngine;

public class BulletSystem : MonoBehaviour
{
    //Sparks are the particles that are activated like blood, weapon debris & sparks
    private AudioSource audioSrc;
    [SerializeField]
    private GameObject[] sparkPrefab;
    private Transform[] sparkPool;
    //Hole types are the type of sprite that gets plastered on objects
    //actual sprite gameobject that gets assigned to hole prefab based on hole type
    [SerializeField]
    private GameObject[] holePrefab;
    //assighnment field that becomes the the instantiated sprite gameobject
    private GameObject currentHole;
    //pools to grab inactive sprite gameobjects
    private Transform[] holePool;
    //assignment field that becomes the active pool
    private Transform currentPool;
    private enum HoleType { bullet, spike, plasma, sigma }
    public enum SparkType { spark, blood, plasma, water, acid, lava }
    private HoleType holeType;
    [HideInInspector]
    public SparkType sparkType;
    private HoleSystem holeSystem;
    private Transform hPool;
    private Transform sPool;
    private new ParticleSystem particleSystem;
    private RaycastHit hit;
    private Vector3 hitPosition;
    private Vector3 fwd;
    private Rigidbody rb;
    private bool isCollided = false;
    private float bulletTimer = 0;
    private bool bulletActive = false;
    private SecretDoorSystem secretDoorSystem;
    [SerializeField]
    private AudioClip projectileSfx;
    private void OnEnable()
    {
        SearchForTransforms();
    }
    private void Update()
    {
        RayCast();
        fwd = transform.TransformDirection(Vector3.forward);
        KillTimer();
    }
    private void SearchForTransforms()
    {
        if (audioSrc == null) audioSrc = GetComponent<AudioSource>();
        if (rb == null) rb = GetComponent<Rigidbody>();
        if (hPool == null)
        {
            hPool = GameObject.Find("GameSystem/Game/ObjectPool/HolePool").transform;
            holePool = new Transform[hPool.childCount]; 
            for (int h = 0; h < hPool.childCount; h++) holePool[h] = hPool.GetChild(h);
        }
        if (sPool == null)
        {
            sPool = GameObject.Find("GameSystem/Game/ObjectPool/SparkPool").transform;
            sparkPool = new Transform[sPool.childCount];
            for (int s = 0; s < sPool.childCount; s++) sparkPool[s] = sPool.GetChild(s);
        }
    }
    public void SetupLifeTime(float bulletTime)
    {
        if (rb == null)
            rb = GetComponent<Rigidbody>();
        rb.WakeUp();
        bulletTimer = bulletTime;
        bulletActive = true;
    }
    private void KillTimer()
    {
        if (!bulletActive)
            return;
        bulletTimer -= Time.deltaTime;
        if (bulletTimer < 0) { bulletTimer = 0; bulletActive = false; gameObject.SetActive(false); }
    }
    private void OnCollisionEnter(Collision collision)
    {
        string currentTag = collision.gameObject.tag;
        isCollided = true;
        if (transform.tag == "PhotonBullet" || transform.tag == "RailBullet" || transform.tag == "SigmaBullet")
            sparkType = SparkType.plasma;
        else
        {
            sparkType = SparkType.spark;

        }
        switch (currentTag)
        {
            case "Enemy": isCollided = false; sparkType = SparkType.blood; EnableSpark(); if (transform.tag != "RailBullet" && transform.tag != "SigmaBullet") transform.gameObject.SetActive(false); return;
            case "DinEnemy": isCollided = false; sparkType = SparkType.blood; EnableSpark(); if (transform.tag != "RailBullet" && transform.tag != "SigmaBullet") transform.gameObject.SetActive(false); return;
            case "Secret":
                {
                    isCollided = false;
                    if (CheckSecretOpen(collision))
                    {
                        if (transform.tag != "SigmaBullet")
                            transform.gameObject.SetActive(false);
                        return;
                    }
                    sparkType = SparkType.blood;
                    EnableSpark();
                    if (transform.tag != "SigmaBullet")
                        transform.gameObject.SetActive(false);
                    return;
                }
            case "Player":
                {
                    isCollided = false;
                    if (transform.tag != "SigmaBullet")
                        transform.gameObject.SetActive(false); 
                    return;
                }
            //case "Water": isCollided = false; sparkType = SparkType.water; EnableSpark(); transform.gameObject.SetActive(false); return;
            //case "Acid": isCollided = false; sparkType = SparkType.acid; EnableSpark(); transform.gameObject.SetActive(false); return;
            //case "Lava": isCollided = false; sparkType = SparkType.lava; EnableSpark(); transform.gameObject.SetActive(false); return;
        }
        EnableSpark();
    }
    private void RayCast()
    {
        if (Physics.Raycast(transform.position, fwd, out hit, Mathf.Infinity))
        {
            if (!isCollided) return;
            switch (hit.collider.tag)
            {
                case "Player": return;
                case "Enemy": return;
            }
            EnableHole();
        }
    }
    private bool CheckSecretOpen(Collision collision)
    {
        secretDoorSystem = collision.gameObject.GetComponent<SecretDoorSystem>();
        if (secretDoorSystem == null) return true;
        if (secretDoorSystem.open) return true;
        else return false;
    }
    public void EnableSpark()
    {
        GameObject spark = AccessSpark();
        if(particleSystem != spark.GetComponent<ParticleSystem>()) particleSystem = spark.GetComponent<ParticleSystem>();
        if(sparkType == SparkType.spark && projectileSfx != null && audioSrc != null & audioSrc.isActiveAndEnabled) audioSrc.PlayOneShot(projectileSfx);
        if (Physics.Raycast(transform.position, fwd, out hit, Mathf.Infinity))
            spark.transform.position = hit.point;
        spark.SetActive(true);
        particleSystem.Play();
 
    }
    private GameObject AccessSpark()
    {
        int index = 0;

        switch (sparkType)
        {
            case SparkType.spark: index = 0; break;
            case SparkType.blood: index = 1; break;
            case SparkType.plasma: index = 2; break;
            //case SparkType.water: index = 3; break;
            //case SparkType.acid: index = 4; break;
            //case SparkType.lava: index = 5; break;
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
  
    private void EnableHole()
    {
        if (transform.tag == "Untagged" || transform.tag == "GrenadeBullet" || transform.tag == "RocketBullet" || transform.tag == "SigmaBullet") return;

        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        float x = 0;
        float y = 0;
        float z = 0;
        float v = 0.01f;

        GameObject hole = AccessHole();
        bool isSpike = false;
        if (holeType == HoleType.spike) isSpike = true;
        else isSpike = false;
        hole.transform.rotation = isSpike ? transform.rotation : Quaternion.FromToRotation(Vector3.up, hit.normal);
        hole.transform.position = hit.point;
        int curRotX = Mathf.FloorToInt(hole.transform.eulerAngles.x);
        int curRotZ = Mathf.FloorToInt(hole.transform.eulerAngles.z);
        if (curRotZ != 0)
        {
            switch (curRotZ)
            {
                case 90: x = -v; break;
                case 270: x = v; break;
            }
        }
        switch (curRotX)
        {
            case 0: y = v; break;
            case 180: y = -v; break;
            case 90: z = v; break;
            case 270: z = -v; break;
        }
        Vector3 newAdj = new Vector3(hole.transform.position.x + x, hole.transform.position.y + y, hole.transform.position.z + z);
        hole.transform.position = newAdj;
        hole.SetActive(true);
        if (holeSystem != hole.transform.GetComponent<HoleSystem>()) holeSystem = hole.GetComponent<HoleSystem>();
        int lifeTime = 0;
        switch (transform.tag)
        {
            case "ShotgunBullet": lifeTime = 5; break;
            case "SpikeBullet": lifeTime = 10; break;
            case "ObstacleBullet": lifeTime = 10; break;
            case "MinigunBullet": lifeTime = 5; break;
            case "GrenadeBullet": lifeTime = 5; break;
            case "RocketBullet": lifeTime = 10; break;
            case "SigmaBullet": lifeTime = 10; break;
            case "RailBullet": lifeTime = 2; break;
            case "PhotonBullet": lifeTime = 2; break;
        }
        holeSystem.SetupHole(lifeTime);
        isCollided = false;
        if(transform.tag != "GrenadeBullet")
            gameObject.SetActive(false);
    }
   
    private GameObject AccessHole()
    {
        string tag = transform.tag;
        if (tag == "Untagged" || tag == null)
            return null;
        int holeIndex = 0;
        switch (tag)
        {
            case "ShotgunBullet": holeType = HoleType.bullet; break;
            case "SpikeBullet": holeType = HoleType.spike;  break;
            case "ObstacleBullet": holeType = HoleType.spike;  break;
            case "MinigunBullet": holeType = HoleType.bullet;  break;
            case "GrenadeBullet": holeType = HoleType.bullet;  break;
            case "RocketBullet": holeType = HoleType.bullet;  break;
            case "RailBullet": holeType = HoleType.plasma; break;
            case "PhotonBullet": holeType = HoleType.plasma; break;
            case "SigmaBullet": holeType = HoleType.sigma; break;
        }
        switch (holeType)
        {
            case HoleType.bullet: holeIndex = 0; break;
            case HoleType.spike: holeIndex = 1; break;
            case HoleType.plasma: holeIndex = 2; break;
            case HoleType.sigma: holeIndex = 3; break;
        }
        currentPool = holePool[holeIndex]; 
        currentHole = holePrefab[holeIndex];
        for (int b = 0; b < currentPool.childCount; b++)
        {
            if (!currentPool.GetChild(b).gameObject.activeInHierarchy)
                return currentPool.GetChild(b).gameObject;
        }
        if (GameSystem.expandBulletPool)
        {
            GameObject newHole = Instantiate(currentHole, Vector3.zero, Quaternion.identity, currentPool);
            return newHole;
        }
        else
            return null;
    }
}
