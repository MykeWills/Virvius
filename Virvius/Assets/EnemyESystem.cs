using UnityEngine.AI;
using System.Text;
using UnityEngine;

public class EnemyESystem : MonoBehaviour
{
    //=================================ScriptAssignment====================================//
    private CommandSystem commandSystem;
    private OptionsSystem optionsSystem;
    private PowerupSystem powerupSystem;
    private PlayerSystem playerSystem;
    private AudioSource audioSrc;
    [HideInInspector]
    public NavMeshAgent navAgent;
    private Animator animator;
    private StringBuilder currentState = new StringBuilder();
    private StringBuilder collisionTag = new StringBuilder();
    private GameSystem gameSystem;
    private WeaponSystem weaponSystem;
    private CharacterController characterController;
    private BoxCollider boxCollider;

    public enum EnemyState { idle, walk, chase, attack, damage, death }

    //starting state
    [Header("Starting State")]
    [SerializeField]
    private EnemyState startState;

    [Space]
    [Header("Inspector Assignment")]
    [SerializeField]
    private Transform bulletPool;
    [SerializeField]
    private MeshRenderer grenadeMuzzle;
    [SerializeField]
    private Light grenadeLight;
    [SerializeField]
    private Transform ammoDropPool;
    [SerializeField]
    private Transform emitter;
    [SerializeField]
    private GameObject bulletPrefab;
    [SerializeField]
    private GameObject ammoDropPrefab;
    [SerializeField]
    private GameObject goreExplosion;
    [SerializeField]
    private GameObject enemyBody;
    [SerializeField]
    private AudioClip[] enemySounds;
    [SerializeField]
    private Transform[] walkPositions;


    [Space]
    [Header("Enemy Values")]
    [HideInInspector]
    public bool isDead;
    private float LOSAngle = 160;
    [SerializeField]
    private float LOSDistance = 0;
    [SerializeField]
    private float angularSpeed = 0;
    [SerializeField]
    private float damageResistanceTime = 2;
    [SerializeField]
    private float health = 8;
    [SerializeField]
    private bool damageResistance = false;
    [SerializeField]
    private float offMeshLinkSpeed = 20f;
    private float walkPositionDistance = 2;

    private RaycastHit playerHit;
    private Vector3 randomPosition;
    private Vector3 originalPosition;
    private EnemyState enemyState;
    private EnemyState activeState;
    private int walkPositionIndex = 0;
    private int distanceIndex = 0;

    private string[] animStates = new string[7]
    {
        "Idle",
        "Walk",
        "Chase",
        "Shoot",
        "Damage",
        "Death",
        "Attack"
    };
    private string[] collisionTags = new string[12]
    {
        "Sword",
        "ShotgunBullet",
        "SpikeBullet",
        "MinigunBullet",
        "GrenadeBullet",
        "RocketBullet",
        "RailBullet",
        "PhotonBullet",
        "SigmaBullet",
        "ObstacleBullet",
        "RocketBulletMini",
        "ObstacleExplosive"
     };

    private float[] distanceRanges = new float[4]
    {
        30.0f,
        60.0f,
        90.0f,
        180.0f,

    };
    private float[] movementSpeeds = new float[4]
    {
        0,
        10,
        25,
        50
    };

    private float movementSpeed = 0;
    private float stoppingDistance = 0;
    private float damageResistanceTimer;
    private float gunflashTime = 0.1f;
    private float gunflashTimer;
    private float time;
    private float maxHealth = 0;
    private float dmgTaken = 0;
    private float elapsed = 2.5f;

    [HideInInspector]
    public bool playerFound = false;
    private bool autoBraking = false;
    private bool playerVisible = false;
    private bool gunFlash = false;
    private bool isDamaged = false;
    private bool updateNextPosition = false;
    private bool rebootEnemy = false;
    private bool returnToDefault = false;
    private bool wasOnLink = false;


    private void Start()
    {
        CheckActiveComponents();
        originalPosition = transform.position;
        enemyState = startState;
        maxHealth = health;
        ActiveState();
    }
    private void Update()
    {
        if (gameSystem.BlockedAttributesActive()) return;
        if (isDead) return;
        if (ShutdownEnemy()) return;
        RebootEnemy();
        IdleDistance();
        WalkDistance();
        if (DefaultEnemy()) return;
        LineOfSight();
        if (!playerFound) return;
        CheckDistance();
        time = Time.deltaTime;
        IsDamaged();
        CheckPath();
        LookAtPlayerShooting();
        GunFlash();
    }
    private void FixedUpdate()
    {
        if (navAgent.isOnOffMeshLink && !wasOnLink)
        {
            navAgent.speed = offMeshLinkSpeed;
            wasOnLink = true;
        }
        else if (!navAgent.isOnOffMeshLink && wasOnLink)
        {
            navAgent.speed = movementSpeed;
            navAgent.velocity = Vector3.zero;
            wasOnLink = false;
        }
    }
    public void EngagePlayer()
    {
        FoundPlayer();
        EnemyState state = playerVisible ? EnemyState.attack : EnemyState.chase;
        enemyState = state;
        if (currentState.Length > 0) currentState.Clear();
        ActiveState();
    }
    private void IdleDistance()
    {
        if (activeState != EnemyState.idle) return;
        //HAS FOUND THE PLAYER - PLAYER GOT TOO CLOSE, ACTIVE IF ENEMY IS NOT BEHIND A WALL
        if (PlayerDistance() <= distanceRanges[1]) { if (playerVisible) { if (!playerFound) LineOfSight(); } }
    }
    private void WalkDistance()
    {
        if (activeState != EnemyState.walk) return;
        float dist = Vector3.Distance(WalkDestination(), transform.position);
        if (dist < walkPositionDistance) { ChangeDestination(); ActiveState(); }
        //HAS FOUND THE PLAYER - PLAYER GOT TOO CLOSE, ACTIVE IF ENEMY IS NOT BEHIND A WALL
        if (PlayerDistance() <= distanceRanges[1]) { if (playerVisible) { if (!playerFound) LineOfSight(); } }
    }
    private void RebootEnemy()
    {
        if (!rebootEnemy) return;
        if (currentState.Length > 0) currentState.Clear();
        enemyState = startState;
        ActiveState();
        playerFound = false;
        rebootEnemy = false;
    }
    private bool DefaultEnemy()
    {
        bool playerActive = playerSystem.isDead ? false : true;
        if (!returnToDefault)
        {
            if (currentState.Length > 0) currentState.Clear();
            enemyState = startState;
            ActiveState();
            playerFound = false;
            returnToDefault = true;
        }
        return !playerActive;
    }
    private bool ShutdownEnemy()
    {
        bool enemyActive = PlayerDistance() > 300 ? false : true;
        if (!enemyActive) rebootEnemy = true;
        animator.enabled = enemyActive;
        navAgent.enabled = enemyActive;
        return !enemyActive;
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (rebootEnemy) return;
        if (collisionTag.Length > 0) collisionTag.Clear();
        collisionTag = collisionTag.Append(collision.gameObject.tag);
        for (int t = 0; t < collisionTags.Length; t++)
        {
            if (collisionTags[t] == collisionTag.ToString())
            {
                if (weaponSystem == null) weaponSystem = WeaponSystem.weaponSystem;
                float dmgAmt = 0;
                switch (t)
                {
                    //sword
                    case 0: dmgAmt = Random.Range(1.50f, 2.01f); break;
                    //shotgun
                    case 1: dmgAmt = !weaponSystem.weaponEquipped[3] ? Random.Range(0.5f, 1.01f) : Random.Range(0.5f, 1.51f); break;
                    //spiker
                    case 2: dmgAmt = Random.Range(1.90f, 2.01f); break;
                    //minigun
                    case 3: dmgAmt = Random.Range(0.25f, 1.25f); break;
                    //grenade
                    case 4: if (health <= (maxHealth / 4)) OverKill(); else dmgAmt = Random.Range(5f, 10.1f); break;
                    //rocket
                    case 5: if (health <= (maxHealth / 4)) OverKill(); else dmgAmt = Random.Range(10f, 15.1f); break;
                    //railgun
                    case 6: if (health <= (maxHealth / 4)) OverKill(); else dmgAmt = Random.Range(30.75f, 41.01f); break;
                    //photon
                    case 7: dmgAmt = Random.Range(3.60f, 5.1f); break;
                    //Sigma
                    case 8: OverKill(); break;
                    //Obstacle
                    case 9: dmgAmt = 1; break;
                    //MiniRocket
                    case 10: if (health <= (maxHealth / 4)) OverKill(); else dmgAmt = Random.Range(2.5f, 5); break;
                }
                float damage = powerupSystem.powerEnabled[2] ? 999 : powerupSystem.powerEnabled[0] ? Mathf.CeilToInt(dmgAmt) * 5 : dmgAmt;
                Damage(damage);
                if (t != 0)
                {
                    if (collisionTags[t] == "GrenadeBullet" || collisionTags[t] == "RocketBullet" || collisionTags[t] == "RocketBulletMini")
                    {
                        if (collision.gameObject.TryGetComponent(out GrenadeSystem grenadeSystem))
                            grenadeSystem.Detonate();
                        if (collision.gameObject.TryGetComponent(out RocketSystem rocketSystem))
                            rocketSystem.Detonate();
                        if (collision.gameObject.TryGetComponent(out RocketSubSystem rocketSubSystem))
                            rocketSubSystem.Detonate();
                    }
                    else
                    {
                        if (collisionTags[t] != "RailBullet" && collisionTags[t] != "SigmaBullet" && collisionTags[t] != "ObstacleExplosive") collision.gameObject.SetActive(false);
                    }
                }
                return;
            }
        }
        return;

    }
    private void CheckActiveComponents()
    {
        if (gameSystem == null) gameSystem = GameSystem.gameSystem;
        if (commandSystem == null) commandSystem = CommandSystem.commandSystem;
        if (playerSystem == null) playerSystem = PlayerSystem.playerSystem;
        if (optionsSystem == null) optionsSystem = OptionsSystem.optionsSystem;
        if (powerupSystem == null) powerupSystem = PowerupSystem.powerupSystem;
        if (navAgent == null) navAgent = GetComponent<NavMeshAgent>();
        if (audioSrc == null) audioSrc = GetComponent<AudioSource>();
        if (animator == null) animator = GetComponent<Animator>();
        if (boxCollider == null) boxCollider = GetComponent<BoxCollider>();
    }
    private void LineOfSight()
    {
        //If player is dead disregard method
        if (playerSystem.isDead || commandSystem.masterCodesActive[2])
            return;

        //distance of the raycast from enemy to player
        Vector3 distanceVector = PlayerPosition() - transform.position;
        //viewing angle from enemy to player
        float angle = Vector3.Angle(transform.forward, distanceVector);
        //when angle is below 60 engage player
        if (angle < LOSAngle && angle > -LOSAngle)
        {
            Debug.DrawRay(transform.position, distanceVector - transform.forward, Color.magenta);
            if (Physics.Raycast(transform.position, distanceVector - transform.forward, out playerHit, LOSDistance))
            {
                //if the raycast hit the player player is now found, activate shooting if player is within range
                if (playerHit.collider.gameObject.CompareTag("Player"))
                {
                    if (playerVisible) return;
                    playerVisible = true;
                    if (!playerFound) audioSrc.PlayOneShot(enemySounds[0]);
                    EngagePlayer();
                }
                else playerVisible = false;
            }
        }
    }
    private void FoundPlayer()
    {
        if (powerupSystem.powerEnabled[1]) return;
        if (playerFound) return;
        audioSrc.PlayOneShot(enemySounds[0]);
        updateNextPosition = true;
        playerFound = true;
    }
    private void CheckPath()
    {
        elapsed += time;
        if (elapsed > 1.5f)
        {
            elapsed -= 1.5f;
            updateNextPosition = true;
        }
    }
    public void PlaySawBladeSound()
    {
        audioSrc.PlayOneShot(enemySounds[5]);
    }
    private void CheckDistance()
    {
        if (PlayerSystem.playerSystem == null || PlayerDistance() == 0) return;
        if (!updateNextPosition) return;
        if (enemyState == EnemyState.attack || enemyState == EnemyState.death || enemyState == EnemyState.damage) return;

        //SHOOT IN CLOSE RANGE
        if (PlayerDistance() <= distanceRanges[0])
        {
            EnemyState state = playerVisible ? EnemyState.attack : EnemyState.chase;
            enemyState = state;
            distanceIndex = 0;
            ActiveState();
        }
        //SHOOT IN LONG RANGE
        if (PlayerDistance() > distanceRanges[0] && PlayerDistance() <= distanceRanges[1])
        {
            enemyState = EnemyState.chase;
            ActiveState();
        }
        //SHOOT ONLY IF HARD OR VERYHARD [EASY & NORMAL WILL CHASE]
        if (PlayerDistance() > distanceRanges[1] && PlayerDistance() <= distanceRanges[2])
        {
            if (optionsSystem.difficultyActive[2] || optionsSystem.difficultyActive[3])
            {
                EnemyState state = playerVisible ? EnemyState.attack : EnemyState.chase;
                enemyState = state;
            }
            else enemyState = EnemyState.chase;
            distanceIndex = 1;
            ActiveState();
        }
        //SHOOT ONLY IF VERYHARD [EASY, NORMAL & HARD WILL CHASE]
        if (PlayerDistance() > distanceRanges[2] && PlayerDistance() <= distanceRanges[3])
        {
            if (optionsSystem.difficultyActive[3])
            {
                EnemyState state = playerVisible ? EnemyState.attack : EnemyState.chase;
                enemyState = state;
            }
            else enemyState = EnemyState.chase;
            distanceIndex = 1;
            ActiveState();
        }
        //HAS LOST THE PLAYER - ANIMATION SET TO BACK DEFAULT [WALK OR IDLE]
        if (PlayerDistance() > distanceRanges[3])
        {
            playerFound = false;
            playerVisible = false;
            enemyState = startState;
            ActiveState();
        }
        updateNextPosition = false;
    }
    public void AnimationFinished()
    {
        if (currentState.Length > 0) currentState.Clear();
        enemyState = EnemyState.chase;
        if (!damageResistance) isDamaged = false;
    }
    private void ActiveState()
    {
        if (!gameObject.activeInHierarchy) return;
        activeState = enemyState;
        switch (activeState)
        {
            case EnemyState.idle:
                {
                    SetAnimation(0);
                    HaltMovement();
                    break;
                }
            case EnemyState.walk:
                {
                    if (animator.GetFloat("Speed") != 1.5f)
                        animator.SetFloat("Speed", 1.5f);
                    SetAnimation(1);
                    movementSpeed = movementSpeeds[1];
                    stoppingDistance = 0;
                    autoBraking = false;
                    SetNav(WalkDestination(), true);
                    break;
                }
            case EnemyState.chase:
                {
                    if (!AnimatorIsPlaying("Shoot") && !AnimatorIsPlaying("Damage") && !AnimatorIsPlaying("Attack"))
                    {
                        if (animator.GetFloat("Speed") != 2f)
                            animator.SetFloat("Speed", 2f);
                        SetAnimation(2);
                        movementSpeed = movementSpeeds[2];
                        stoppingDistance = 0;
                        autoBraking = false;
                        SetNav(RandomDestination(), true);
                    }
                    break;
                }
            case EnemyState.attack:
                {
                    float difficultyInterval = 1;

                    //VERYHARD
                    if (optionsSystem.difficultyActive[3])
                        difficultyInterval = 2f;
                    //HARD
                    else if (optionsSystem.difficultyActive[2])
                        difficultyInterval = 1.5f;
                    //NORMAL
                    else if (optionsSystem.difficultyActive[1])
                        difficultyInterval = 1.25f;
                    //EASY
                    else if (optionsSystem.difficultyActive[0])
                        difficultyInterval = 1;
                    if (animator.GetFloat("Speed") != difficultyInterval)
                        animator.SetFloat("Speed", (difficultyInterval));

                    int attackID = (distanceIndex != 0) ? 3 : 6;
                    SetAnimation(attackID);
                    movementSpeed = (distanceIndex != 0) ? movementSpeeds[0] : movementSpeeds[1];
                    stoppingDistance = (distanceIndex != 0) ? 12.2f : 0;
                    autoBraking = false;

                    if (distanceIndex != 0) HaltMovement();
                    else SetNav(playerSystem.transform.position, true);

                    break;
                }
            case EnemyState.damage:
                {
                    SetAnimation(4);
                    HaltMovement();
                    break;
                }
            case EnemyState.death:
                {
                    SetAnimation(5);
                    HaltMovement();
                    break;
                }
        }
    }
    private void ChangeDestination()
    {
        if (walkPositions[0] == null) return;
        walkPositionIndex++;
        if (walkPositionIndex > walkPositions.Length - 1) walkPositionIndex = 0;
        else if (walkPositionIndex < 0) walkPositionIndex = walkPositions.Length - 1;
    }
    private void HaltMovement()
    {
        movementSpeed = movementSpeeds[0];
        stoppingDistance = 0;
        autoBraking = false;
        SetNav(transform.localPosition, false);
    }
    private Vector3 PlayerPosition()
    {
        if (playerSystem.isDead || PlayerSystem.playerSystem == null) return transform.position;
        return playerSystem.transform.position;
    }
    private void SetNav(Vector3 nextPosition, bool active)
    {
        if (!gameObject.activeInHierarchy) return;
        if (!enemyBody.activeInHierarchy) return;
        Vector3 newPos = active ? nextPosition : transform.position;
        if (isDead) { navAgent.destination = transform.position; return; }
        navAgent.destination = newPos;
        navAgent.autoBraking = autoBraking;
        navAgent.autoRepath = active;
        navAgent.speed = movementSpeed;
        navAgent.angularSpeed = angularSpeed;
        navAgent.stoppingDistance = stoppingDistance;
    }
    public Vector3 GetPositionOnNavMesh(Vector3 position)
    {
        NavMeshHit hit;
        bool positionFound = NavMesh.SamplePosition(position, out hit, 2, NavMesh.AllAreas);
        return positionFound ? hit.position : position;
    }
    private Vector3 MoveInPlayerRadius(float offsetX, float offsetZ)
    {
        Vector3 player = (playerSystem != null) ? playerSystem.transform.position : Vector3.zero;
        if (player == Vector3.zero) { return transform.position; }
        Vector3[] positions = new Vector3[3]
        {
            new Vector3(player.x + offsetX, transform.position.y, player.z + offsetZ),
            new Vector3(transform.position.x + offsetX, transform.position.y, transform.position.z + offsetX),
            player
        };

        int rndIndex = Random.Range(0, 3);
        Vector3 movePosition = positions[rndIndex];
        return movePosition;
    }
    private Vector3 WalkDestination()
    {
        if (walkPositions[0] == null) return transform.position;
        Vector3 direction = new Vector3(walkPositions[walkPositionIndex].position.x, transform.position.y, walkPositions[walkPositionIndex].position.z);
        return direction;
    }
    private Vector3 RandomDestination()
    {
        if (optionsSystem.difficultyActive[3]) randomPosition = PlayerPosition();
        else randomPosition = MoveInPlayerRadius(Random.Range(-15, 15), transform.TransformDirection(Vector3.forward).z * Random.Range(-4, 4));
        Vector3 direction = new Vector3(randomPosition.x, transform.position.y, randomPosition.z);
        return direction;

    }
    private float PlayerDistance()
    {
        Vector3 player = (PlayerSystem.playerSystem != null) ? PlayerSystem.playerSystem.transform.position : Vector3.zero;
        if (player == Vector3.zero) { return 0; }
        playerSystem = PlayerSystem.playerSystem;
        float playerDistance = Vector3.Magnitude(transform.position - playerSystem.transform.position);
        return playerDistance;
    }
    private void SetAnimation(int index)
    {
        if (currentState.Length > 0) currentState.Clear();
        if (currentState.ToString() != animStates[index])
        {
            currentState.Append(animStates[index]);
            ActiveAnimation(currentState.ToString());
        }
    }
    private void ActiveAnimation(string animation)
    {
        if (!enemyBody.activeInHierarchy) return;
        for (int an = 0; an < animStates.Length; an++)
        {
            if (animStates[an] == animation)
                animator.SetBool(animStates[an], true);
            else
                animator.SetBool(animStates[an], false);
        }
    }
    private bool AnimatorIsPlaying()
    {
        return animator.GetCurrentAnimatorStateInfo(0).length >
               animator.GetCurrentAnimatorStateInfo(0).normalizedTime - 1;
    }
    private bool AnimatorIsPlaying(string stateName)
    {
        return AnimatorIsPlaying() && animator.GetCurrentAnimatorStateInfo(0).IsName(stateName);
    }
    private void Shoot()
    {
        //Method is triggered in animation.
        if (isDead) return;
        gunFlash = true;
        audioSrc.PlayOneShot(enemySounds[1]);
        GameObject bullet = AccessPool(bulletPool, bulletPrefab);
        if (characterController == null) { playerSystem = PlayerSystem.playerSystem; characterController = playerSystem.GetComponent<CharacterController>(); }
        Physics.IgnoreCollision(bullet.GetComponent<Collider>(), characterController);
        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        //emitter.transform.LookAt(Camera.main.transform.position);
        rb.velocity = Vector3.zero;
        bullet.transform.position = emitter.position;
        //bullet.transform.localRotation = emitter.localRotation;
        bullet.SetActive(true);
        rb.AddForce(emitter.transform.forward * 15000f);
    }
    private void LookAtPlayerShooting()
    {
        if (commandSystem.masterCodesActive[2]) return;
        if (enemyState != EnemyState.attack) return;
        Vector3 lookVector = PlayerPosition() - transform.position;
        lookVector.y = transform.position.y - 5;
        Quaternion rot = Quaternion.LookRotation(lookVector);
        Quaternion newRotation = Quaternion.Slerp(transform.rotation, rot, 1f);
        newRotation.x = 0;
        newRotation.z = 0;
        transform.rotation = newRotation;
        navAgent.transform.rotation = newRotation;
    }
    private void GunFlash()
    {
        if (!gunFlash) return;
        grenadeMuzzle.enabled = true;
        grenadeLight.enabled = optionsSystem.showFlashEffects;
        gunflashTimer -= time;
        gunflashTimer = Mathf.Clamp(gunflashTimer, 0.0f, 0.05f);
        if (gunflashTimer == 0.0f)
        {
            grenadeMuzzle.transform.Rotate(0, 30, 0);
            grenadeMuzzle.enabled = false;
            gunFlash = false;
            grenadeLight.enabled = false;
            gunflashTimer = gunflashTime;
        }
    }
    public void OverKill()
    {
        if (boxCollider.enabled)
        {
            for (int b = 0; b < bulletPool.childCount; b++)
            {
                if (bulletPool.GetChild(b).gameObject.activeInHierarchy)
                    bulletPool.GetChild(b).gameObject.SetActive(false);
            }
            health = 0;
            goreExplosion.SetActive(true);
            isDead = true;
            OnDeath();
            enemyBody.SetActive(false);
            boxCollider.enabled = false;
            navAgent.enabled = false;
            grenadeMuzzle.transform.Rotate(0, 30, 0);
            grenadeMuzzle.enabled = false;
            gunFlash = false;
            grenadeLight.enabled = false;
            gunflashTimer = gunflashTime;
            DropAmmo();
        }
    }
    private bool EligibleForOverKill()
    {
        int rng = Random.Range(0, 11);
        if (powerupSystem.powerEnabled[0] || powerupSystem.powerEnabled[2]) rng = 1;
        if (dmgTaken > maxHealth && rng == 1) return true;
        return false;
    }
    public void Damage(float amt)
    {
        if (isDead) return;
        dmgTaken += amt;
        if (amt >= health)
        {
            if (EligibleForOverKill()) { OverKill(); return; }
        }
        health -= amt;
        health = Mathf.Clamp(health, 0, maxHealth);
        isDead = health < 1 ? true : false;
        if (!isDead) OnDamaged();
        else { audioSrc.PlayOneShot(enemySounds[3]); OnDeath(); }
    }
    private void OnDamaged()
    {
        if (!isDamaged)
        {
            isDamaged = true;
            if (!playerFound) FoundPlayer();
            if (currentState.Length > 0) currentState.Clear();
            enemyState = EnemyState.damage;
            audioSrc.PlayOneShot(enemySounds[2]);
            ActiveState();
        }
    }
    private void IsDamaged()
    {
        if (!damageResistance) return;
        if (!isDamaged) return;
        damageResistanceTimer -= time;
        damageResistanceTimer = Mathf.Clamp(damageResistanceTimer, 0, damageResistanceTime);
        if (damageResistanceTimer == 0)
        {
            damageResistanceTimer = damageResistanceTime;
            isDamaged = false;
        }
    }
    private void OnDeath()
    {
        if (boxCollider.enabled)
        {
            for (int b = 0; b < bulletPool.childCount; b++)
            {
                if (bulletPool.GetChild(b).gameObject.activeInHierarchy)
                    bulletPool.GetChild(b).gameObject.SetActive(false);
            }
            enemyState = EnemyState.death;
            ActiveState();
            navAgent.enabled = false;
            boxCollider.enabled = false;
            gunFlash = false;
            grenadeMuzzle.transform.Rotate(0, 30, 0);
            grenadeMuzzle.enabled = false;
            grenadeLight.enabled = false;
            gunflashTimer = gunflashTime;
            DropAmmo();
        }
    }
    private void DropAmmo()
    {
        GameObject ammoPack = AccessPool(ammoDropPool, ammoDropPrefab);
        ammoPack.transform.position = transform.position;
        ammoPack.transform.localRotation = Quaternion.identity;
        ammoPack.SetActive(true);
    }
    private GameObject AccessPool(Transform pool, GameObject instantiateObj)
    {
        for (int b = 0; b < pool.childCount; b++)
        {
            if (!pool.GetChild(b).gameObject.activeInHierarchy)
                return pool.GetChild(b).gameObject;
        }
        if (GameSystem.expandBulletPool)
        {
            GameObject newObj = Instantiate(instantiateObj, pool);
            return newObj;
        }
        else return null;
    }
    public void ResetObject(bool setOrgPos)
    {
        if (animator == null || audioSrc == null) Start();
        goreExplosion.SetActive(false);
        enemyBody.SetActive(true);
        animator.Rebind();
        boxCollider.enabled = true;
        navAgent.enabled = true;
        damageResistanceTimer = damageResistanceTime;
        gunFlash = false;
        gunflashTimer = gunflashTime;
        elapsed = 0.0f;
        health = maxHealth;
        dmgTaken = 0;
        isDamaged = false;
        playerFound = false;
        playerVisible = false;
        returnToDefault = false;
        rebootEnemy = false;
        isDead = false;
        grenadeLight.enabled = false;
        grenadeMuzzle.enabled = false;
        audioSrc.Stop();
        for (int a = 0; a < ammoDropPool.childCount; a++)
        {
            if (ammoDropPool.GetChild(a).gameObject.activeInHierarchy)
                ammoDropPool.GetChild(a).gameObject.SetActive(false);
        }
        if (currentState.Length > 0) currentState.Clear();
        enemyState = startState;
        ActiveState();
        if (!setOrgPos) return;
        navAgent.Warp(originalPosition);
        transform.position = originalPosition;
    }
}