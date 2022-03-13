using UnityEngine;

public class RocketSystem : MonoBehaviour
{
    private WeaponSystem weaponSystem;
    [SerializeField]
    private GameObject rocketBase;
    [SerializeField]
    private Transform[] miniRotator = new Transform[2];  
    [SerializeField]
    private Transform[] miniRockets = new Transform[2];
    private Rigidbody rb;
    private Rigidbody[] miniRocketsRb = new Rigidbody[2];
    private GameObject miniRocketSubPool;
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
    private ParticleSystem smokeParticle;
    private void OnEnable()
    {
        ResetObject();
    }

    // Update is called once per frame
    void Update()
    {
        time = Time.deltaTime;
        RotateMiniRockets();
        ExplosionRadius();
    }
    private void RotateMiniRockets()
    {
        for (int r = 0; r < miniRockets.Length; r++)
        {
            if(miniRockets[r].TryGetComponent(out RocketSubSystem rocketSubSystem))
            {
                if (rocketSubSystem.isDetonated) return;
            }
            int inverse = (r == 0) ? 1 : -1;
            miniRotator[r].transform.Rotate(0, 0, (Time.deltaTime * 750) * inverse);
            miniRockets[r].transform.localPosition = new Vector3((0.9f * inverse) + (Mathf.PingPong(Time.time, 1)), miniRockets[r].transform.localPosition.y, miniRockets[r].transform.localPosition.z);
        }
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
        for (int r = 0; r < miniRocketsRb.Length; r++)
        {
            miniRockets[r].parent = miniRocketSubPool.transform;
        }
      
    }
    private void ExplosionRadius()
    {
        if (!isDetonated) return;
        if (!explosionObject.activeInHierarchy)
        {
          
            explosionObject.SetActive(true);
        }
        int exVal = expandRadius ? 1 : -1;
        curRadius += (time * explosionSpeed) * exVal;
        sphereCollider.radius = Mathf.Clamp(curRadius, 0, blastRadius);
        if (curRadius > blastRadius)
            expandRadius = false;
        else if (curRadius < 1)
            sphereCollider.enabled = false;
        if (!smokeParticle.isPlaying)
        {
            isDetonated = false;
            explosionObject.SetActive(false);
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (isDetonated) return;
        if (collision.gameObject.tag == "RocketBullet" || collision.gameObject.tag == "RocketBulletMini") return;
        if (collision.gameObject) Detonate();
    }
    public void SetupMiniRockets(float bulletForce)
    {
        for (int r = 0; r < miniRocketsRb.Length; r++)
        {
            miniRockets[r].gameObject.SetActive(true);
            if (miniRocketsRb[r] == null) miniRocketsRb[r] = miniRockets[r].GetComponent<Rigidbody>();
            miniRockets[r].parent = miniRotator[r].transform;
            miniRockets[r].localPosition = weaponSystem.weaponEmitter[r + 1].localPosition;
            miniRocketsRb[r].AddForce(weaponSystem.weaponEmitter[r + 1].transform.forward * bulletForce);
        }
    }
    public void ResetObject()
    {
        weaponSystem = WeaponSystem.weaponSystem;
        if (sphereCollider == null) sphereCollider = GetComponent<SphereCollider>();
        if (capsuleCollider == null) capsuleCollider = GetComponent<CapsuleCollider>();
        if (rb == null) rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.None;
        rb.constraints = RigidbodyConstraints.FreezeRotation;
        explosionObject.SetActive(false);
        for (int r = 0; r < miniRocketsRb.Length; r++)
        {
            miniRockets[r].gameObject.SetActive(true);
            if (miniRocketsRb[r] == null) miniRocketsRb[r] = miniRockets[r].GetComponent<Rigidbody>();
            miniRocketsRb[r].velocity = Vector3.zero;
            miniRockets[r].parent = miniRotator[r].transform;
            miniRockets[r].localPosition = weaponSystem.weaponEmitter[r + 1].localPosition;
            miniRocketsRb[r].AddForce(weaponSystem.weaponEmitter[r + 1].transform.forward * Random.Range(15000, 25000));
        }
        curRadius = 1;
        isDetonated = false;
        rocketBase.SetActive(true);
        capsuleCollider.enabled = true;
        sphereCollider.radius = 1;
        sphereCollider.enabled = false;
        expandRadius = true;
    }
}
