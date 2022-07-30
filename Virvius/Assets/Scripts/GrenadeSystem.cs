using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrenadeSystem : MonoBehaviour
{
    private float gravity = 1;
    [Range(0.01f, 200)]
    [SerializeField]
    private float gravityMultiplier = 1;
    private Rigidbody rb;
    private bool isExplode = false;
    [HideInInspector]
    public bool isDetonated = false;
    [SerializeField]
    private float explosionTime = 2;
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
    private float explosionDelay = 2;
    [SerializeField]
    private GameObject grenadeObj;
    [SerializeField]
    private ParticleSystem[] ps = new ParticleSystem[3];
    private float velocityReduction = 1;
    private void OnEnable()
    {
        ResetObject();
    }
    void Update()
    {
        if (gameObject.activeInHierarchy)
        {
            time = Time.deltaTime;
            gravity = gravity += (time * gravityMultiplier);
            rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y - gravity, rb.velocity.z);
            ActiveGrenadeTimer();
            ExplosionRadius();
        }
    }
    private void ResetObject()
    {
        grenadeObj.SetActive(true);
        if (rb == null) rb = GetComponent<Rigidbody>();
        if (sphereCollider == null) sphereCollider = GetComponent<SphereCollider>();
        if (capsuleCollider == null) capsuleCollider = GetComponent<CapsuleCollider>();
        ps[1].gameObject.SetActive(true);
        ps[2].gameObject.SetActive(true);
        gravity = 1;
        rb.velocity = Vector3.zero;
        explosionObject.SetActive(false);
        explosionTime = explosionDelay;
        rb.useGravity = true;
        isExplode = false;
        curRadius = 1;
        expandRadius = true;
        sphereCollider.radius = 1;
        capsuleCollider.enabled = true;
        sphereCollider.enabled = false;
        isDetonated = false;
    }
    private void ActiveGrenadeTimer()
    {
        if (isExplode) return;
        explosionTime -= time;
        explosionTime = Mathf.Clamp(explosionTime, 0, explosionDelay);

        if (explosionTime == 0)
        {
            grenadeObj.SetActive(false);
            gravity = 1;
            rb.velocity = Vector3.zero;
            rb.useGravity = false;
            isExplode = true;
            isDetonated = true;
        }
    }
    public void Detonate()
    {
        explosionTime = 0;
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
        else if (curRadius < 1 ) 
            sphereCollider.enabled = false;
        if (!ps[0].isPlaying)
        {
            isDetonated = false;
            ps[1].gameObject.SetActive(false);
            ps[2].gameObject.SetActive(false);
            gameObject.SetActive(false);
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject)
        {
            velocityReduction -= 5f;
            velocityReduction = Mathf.Clamp(velocityReduction, 0, rb.velocity.y);
            rb.velocity = new Vector3(rb.velocity.x - velocityReduction, rb.velocity.y - velocityReduction, rb.velocity.z - velocityReduction);
        }
          
    }
}
