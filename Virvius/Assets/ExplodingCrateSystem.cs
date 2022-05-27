using UnityEngine;

public class ExplodingCrateSystem : MonoBehaviour
{
    private enum ObjectType { crate, wall }
    [SerializeField]
    private ObjectType objectType;
    [HideInInspector]
    public bool isDetonated = false;
    private float time = 0;
    private bool expandRadius = true;
    [SerializeField]
    private float blastRadius = 6;
    private float curRadius = 1;
    [SerializeField]
    private float explosionSpeed = 5;
    private SphereCollider sphereCollider;
    [SerializeField]
    private GameObject explosionPrefab;
    [SerializeField]
    private ParticleSystem ps;
    [SerializeField]
    private GameObject viewableCrate;
    private ContactPoint contactPoint;

    private string[] collisionTags = new string[7]
    {
        "Player",
        "Enemy",
        "DinEnemy",
        "ObstacleExplosive",
        "GrenadeBullet",
        "RocketBullet",
        "RocketBulletMini"
    };
    void Start()
    {
        sphereCollider = GetComponent<SphereCollider>();
        sphereCollider.enabled = false;

    }

    // Update is called once per frame
    void Update()
    {
        ExplosionRadius();
    }
    private void ExplosionRadius()
    {
        if (!isDetonated) return;
        time = Time.deltaTime;
        if (!explosionPrefab.activeInHierarchy)
        {
            explosionPrefab.SetActive(true);
            if (objectType != ObjectType.wall)
                sphereCollider.enabled = true;
            else sphereCollider.enabled = false;
        }
        if (viewableCrate.activeInHierarchy) 
            viewableCrate.SetActive(false);
        int exVal = expandRadius ? 1 : -1;
        curRadius += (time * explosionSpeed) * exVal;
        sphereCollider.radius = Mathf.Clamp(curRadius, 0, blastRadius);
        if (curRadius > blastRadius)
            expandRadius = false;

        else if (curRadius < 1 && !ps.isPlaying)
        {
            isDetonated = false;
            sphereCollider.enabled = false;
            explosionPrefab.SetActive(false);
        }
    }
    public void Explode()
    {
        isDetonated = true;
    }
    private float Map(float value, float inMin, float inMax, float outMin, float outMax)
    {
        return (value - inMin) * (outMax - outMin) / (inMax - inMin) + outMin;
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (!isDetonated) return;
        string tag = collision.gameObject.tag;

        for(int t = 0; t < collisionTags.Length; t++)
        {
            if(tag == collisionTags[t])
            {
                contactPoint = collision.GetContact(0);
                float dist = Vector3.Distance(sphereCollider.bounds.center, contactPoint.point);
                float ratio = Map(dist, 1, blastRadius, 0, 200);
                float damage = 200 - ratio;
                if (objectType == ObjectType.wall) return;
                if (collision.gameObject.TryGetComponent(out PlayerSystem playerSystem)) playerSystem.Damage(Mathf.FloorToInt(damage));
                if (collision.gameObject.TryGetComponent(out EnemyGSystem gruntSystem)) gruntSystem.Damage(damage);
                if (collision.gameObject.TryGetComponent(out DinSubSystem dinSubSystem)) dinSubSystem.Damage(damage);
                if (collision.gameObject.TryGetComponent(out ExplodingCrateSystem explodingCrate)) explodingCrate.Explode();
                if (collision.gameObject.TryGetComponent(out GrenadeSystem grenadeSystem)) grenadeSystem.Detonate();
            }
        }
    }
    public void ResetObject()
    {
        isDetonated = false;
        curRadius = 1;
        sphereCollider.enabled = false;
        expandRadius = true;
        viewableCrate.SetActive(true);
        explosionPrefab.SetActive(false);
    }
}
