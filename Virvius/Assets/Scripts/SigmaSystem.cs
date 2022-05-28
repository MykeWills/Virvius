using UnityEngine;

public class SigmaSystem : MonoBehaviour
{
    private PlayerSystem playerSystem;
    private InputSystem inputSystem;
    private Rigidbody rb;
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
    private GameObject bulletEffect;
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
        ExplosionRadius();
    }

    public void Detonate()
    {
        isDetonated = true;
        if (rb == null) rb = GetComponent<Rigidbody>();
        if (inputSystem == null) inputSystem = InputSystem.inputSystem;
        inputSystem.SetScreenShakeEffect(5, 1.25f);
        inputSystem.SetVibration(0, 3, 1.25f);
        curRadius = 1;
        expandRadius = true;
        sphereCollider.radius = sphereCollider.radius = 0.625f;
        sphereCollider.enabled = true;
        bulletEffect.SetActive(false);
        rb.constraints = RigidbodyConstraints.FreezeAll;
        rb.velocity = Vector3.zero;
    }
    private void ExplosionRadius()
    {
        if (!isDetonated) return;
        if (!explosionObject.activeInHierarchy)
        {
            if(playerSystem == null) playerSystem = PlayerSystem.playerSystem;
            playerSystem.SetSigmaFlash(true);
            explosionObject.SetActive(true);
        }
        int exVal = expandRadius ? 1 : -1;
        curRadius += (time * explosionSpeed) * exVal;
        sphereCollider.radius = Mathf.Clamp(curRadius, sphereCollider.radius = 0.625f, blastRadius);
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
        if (collision.gameObject.tag == "SigmaBullet") return;
        if (collision.gameObject) Detonate();
    }
 
    public void ResetObject()
    {

        if (sphereCollider == null) sphereCollider = GetComponent<SphereCollider>();
        if (rb == null) rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.None;
        rb.constraints = RigidbodyConstraints.FreezeRotation;
        explosionObject.SetActive(false);
        curRadius = 1;
        isDetonated = false;
        sphereCollider.radius = 0.625f;
        sphereCollider.enabled = true;
        bulletEffect.SetActive(true);
        expandRadius = true;
    }
}
