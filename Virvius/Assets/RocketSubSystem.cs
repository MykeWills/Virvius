using UnityEngine;

public class RocketSubSystem : MonoBehaviour
{
    [SerializeField]
    private GameObject rocketBase;
    [HideInInspector]
    public bool isDetonated = false;
    private float time = 0;
    private bool expandRadius = true;
    [SerializeField]
    private float blastRadius = 6;
    private float curRadius = 1;
    [SerializeField]
    private float explosionSpeed = 5;
    private CapsuleCollider capsuleCollider;
    private SphereCollider sphereCollider;
    [SerializeField]
    private GameObject explosionObject;
    [SerializeField]
    private ParticleSystem smokeParticleSystem;
    private Transform OriginTransform;
    private GameObject miniRocketSubPool;
    private Rigidbody rb;
    private void OnEnable()
    {
        ResetObject();
    }
    // Update is called once per frame
    void Update()
    {
        time = Time.deltaTime;
        ExplosionRadius();
    }
    public void Detonate()
    {
        isDetonated = true;
        if (rb == null) rb = GetComponent<Rigidbody>();
        curRadius = 1;
        expandRadius = true;
        capsuleCollider.enabled = false;
        sphereCollider.radius = 1;
        sphereCollider.enabled = true;
        rb.constraints = RigidbodyConstraints.FreezeAll;
        rb.velocity = Vector3.zero;
        rocketBase.SetActive(false);
        if (miniRocketSubPool == null) miniRocketSubPool = GameObject.Find("GameSystem/Game/ObjectPool/RocketSubPool");
    }
    private void ExplosionRadius()
    {
        if (!isDetonated) return;
        if (!explosionObject.activeInHierarchy)
        {
            explosionObject.SetActive(true);
            capsuleCollider.enabled = false;
            sphereCollider.enabled = true;
        }
        int exVal = expandRadius ? 1 : -1;
        curRadius += (time * explosionSpeed) * exVal;
        sphereCollider.radius = Mathf.Clamp(curRadius, 0, blastRadius);
        if (curRadius > blastRadius)
            expandRadius = false;
        else if (curRadius < 1 && !smokeParticleSystem.isPlaying)
        {
            isDetonated = false;
            sphereCollider.enabled = false;
            explosionObject.SetActive(false);
            if (miniRocketSubPool == null) miniRocketSubPool = GameObject.Find("GameSystem/Game/ObjectPool/RocketSubPool");
            transform.parent = miniRocketSubPool.transform;
            gameObject.SetActive(false);
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (isDetonated) return;
        if (collision.gameObject.tag == "RocketBullet" || collision.gameObject.tag == "RocketBulletMini") return;
        if(collision.gameObject) Detonate();
    }
    public void ResetObject()
    {
        if (sphereCollider == null) sphereCollider = GetComponent<SphereCollider>();
        if (capsuleCollider == null) capsuleCollider = GetComponent<CapsuleCollider>();
        if (rb == null) rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.None;
        rb.constraints = RigidbodyConstraints.FreezeRotation;
        explosionObject.SetActive(false);
        curRadius = 1;
        isDetonated = false;
        rocketBase.SetActive(true);
        capsuleCollider.enabled = true;
        sphereCollider.radius = 1;
        sphereCollider.enabled = false;
        expandRadius = true;
    }
}
