using UnityEngine;

public class NailShooter : MonoBehaviour
{
    private PlayerSystem playerSystem;
    private AudioSource audioSrc;
    [SerializeField]
    private AudioClip shootSfx;
    [SerializeField]
    private GameObject bulletPrefab;
    [SerializeField]
    private Transform emitter;
    [SerializeField]
    private Transform pool;
    [SerializeField]
    private float engageDistance = 50;
    [SerializeField]
    private float fireRate = 1;
    private float nextFire = 0;
    private float time = 0;
    private bool engage = false;
    [SerializeField]
    private bool autoFire = false;
    [SerializeField]
    private bool useTrigger = false;    
    [SerializeField]
    private bool waitToShoot = true;
    private RaycastHit hit;

    void Start()
    {
        playerSystem = PlayerSystem.playerSystem;
        if(waitToShoot) nextFire = fireRate;
    }

    // Update is called once per frame
    void Update()
    {
        if (playerSystem.isDead)
            return;
        time = Time.time;
        CheckPlayerInView();
        Shoot();
    }
    public void EngagePlayer(bool active)
    {
        engage = active;
    }
    private void Shoot()
    {
        if (!engage && !autoFire) return;
        if(time > nextFire)
        {
            SetupBullet(emitter, 10000);
            nextFire = time + fireRate;
        }
    }
    private void SetupBullet(Transform emitter, float bulletForce)
    {
        GameObject bullet = AccessBullet();
        BulletSystem bulletSystem = bullet.GetComponent<BulletSystem>();
        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        rb.velocity = Vector3.zero;
        bullet.transform.position = emitter.position;
        bullet.transform.rotation = emitter.rotation;
        bullet.SetActive(true);
        bulletSystem.SetupLifeTime(5);
        rb.AddForce(emitter.transform.forward * bulletForce);
        if (audioSrc == null) audioSrc = GetComponent<AudioSource>();
        audioSrc.PlayOneShot(shootSfx);
    }
    private GameObject AccessBullet()
    {
        for (int b = 0; b < pool.childCount; b++)
        {
            if (!pool.GetChild(b).gameObject.activeInHierarchy)
                return pool.GetChild(b).gameObject;
        }
        if (GameSystem.expandBulletPool)
        {
            GameObject newBullet = Instantiate(bulletPrefab, pool);
            return newBullet;
        }
        else
            return null;
    }
    private void CheckPlayerInView()
    {
        if (useTrigger) return;
        if (Physics.Raycast(transform.position, transform.forward, out hit, engageDistance))
        {
            //if the raycast hit the player player is now found, activate shooting if player is within range
            if (hit.collider.gameObject.CompareTag("Player")) engage = true;
            if (hit.collider.gameObject.CompareTag("Enemy")) engage = true;
            if (hit.collider.gameObject.CompareTag("DinEnemy")) engage = true;
            else engage = false;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (!useTrigger) return;
        if (engage) return;
        if (other.gameObject.CompareTag("Player")) engage = true;
        if (other.gameObject.CompareTag("Enemy")) engage = true;
        if (other.gameObject.CompareTag("DinEnemy")) engage = true;
    }
    private void OnTriggerExit(Collider other)
    {
        if (!useTrigger) return;
        if (!engage) return;
        if (other.gameObject.CompareTag("Player")) engage = false;
        if (other.gameObject.CompareTag("Enemy")) engage = false;
        if (other.gameObject.CompareTag("DinEnemy")) engage = false;
    }
}
