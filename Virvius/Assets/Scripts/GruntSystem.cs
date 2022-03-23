using UnityEngine;
using UnityEngine.AI;
using System.Text;
public class GruntSystem : MonoBehaviour
{
    private GameSystem gameSystem;
    private CommandSystem commandSystem;
    private OptionsSystem optionsSystem;
    private PowerupSystem powerupSystem;
    private WeaponSystem weaponSystem;
    private CharacterController characterController;
    private PlayerSystem playerSystem;
    private GoreSystem goreSystem;
    private AudioSource audioSrc;
    [HideInInspector]
    public NavMeshAgent navAgent;
    private Animator anim;
    private BoxCollider boxCollider;
    private StringBuilder stringBuilderState = new StringBuilder();
    private StringBuilder tagSb = new StringBuilder();
    [SerializeField]
    private Transform[] movePositions = new Transform[4];
    private Vector3 distanceVector = Vector3.zero;
    private RaycastHit playerHit;
    [SerializeField]
    private bool behindWall = false; 
    [SerializeField]
    private bool randomizePositions = false;
    [SerializeField]
    private bool backTrackPositions = false;
    [SerializeField]
    private bool waitBetweenPositions;
    [SerializeField]
    private MeshRenderer shotgunMuzzle;
    [SerializeField]
    private Light shotgunLight;
    [SerializeField]
    private Transform bulletPool;
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
    private float viewAngle = 100;
    private float[] enemyStateRanges = new float[3]
    {
        15.0f, //Stopping Range
        120.0f, //Awareness Range
        200.0f //Player Lost Range
    };
    private float walkSpeed = 5;
    public  enum State { Idle, Walk, Chase, Shoot, Damage, Death }
    public State curState = State.Idle;
    private State orgState;
    private string[] stateName = new string[6]
    {
        "Idling",
        "Walking",
        "Chasing",
        "Shooting",
        "Damaged",
        "Dead"
    };
    [SerializeField]
    private string currentStateName;
    private int stateIndex = 0;
    private int curStateIndex = 9;
    private int curPositionIndex = 0;
    private float curSpeed = 0;
    private float viewDistance = 0;
    private string[] animNames = new string[6] { "Idle", "Walk", "Chase", "Shoot", "Damage", "Death" };
    private string[] tags = new string[11]
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
   };
    private string curAnim = null;
    [HideInInspector]
    public bool isDead = false;
    private float health = 8;
    private float maxHealth = 0;
    private float dmgTaken = 0;
    [SerializeField]
    private float bulletForce = 10000;
    private float gunflashTime = 0.05f;
    private float gunflashTimer;
    private float time;
    private bool gunFlash = false;
    private float stateTimer = 0;
    [HideInInspector]
    public bool playerFound = false;
    private bool allowMovement = true;
    private float waitTime = 2;
    private float waitTimer;
    private bool isWaiting = false;
    private float lookTime = 10;
    private float lookTimer;
    private bool lookForPlayer = false;
    private float damageTime = 2;
    private float damageTimer;
    private bool isDamaged = false;
    private bool playerVisible = false;
    private float randomSpeed = 0;
    private Vector3 chasePosition;
    private Vector3 originalPosition;

    //goldenPath
    public Transform target;
    private NavMeshPath path;
    private float elapsed = 0.0f;
    private bool updateNextPosition = false;
    public bool debug = false;
    //-------------------------------------------------------------------------------------------------------
    //---------------------------------UNITY-----------------------------------------------------------------
    //-------------------------------------------------------------------------------------------------------
    public void Start()
    {
        optionsSystem = OptionsSystem.optionsSystem;
        playerSystem = PlayerSystem.playerSystem;
        gameSystem = GameSystem.gameSystem;
        powerupSystem = PowerupSystem.powerupSystem;
        commandSystem = CommandSystem.commandSystem;
        goreSystem = goreExplosion.GetComponent<GoreSystem>();
       
        audioSrc = GetComponent<AudioSource>();
        boxCollider = GetComponent<BoxCollider>();
        navAgent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        originalPosition = transform.position;
        orgState = curState;
        stateTimer = 1;
        maxHealth = health;
        viewDistance = 150;
        waitTime = Random.Range(1, 5);
        waitTimer = waitTime;
        stateTimer = SetStateTimer();
        damageTime = Random.Range(0.25f, 0.5f);
        damageTimer = damageTime;
        lookTimer = lookTime;
        randomSpeed = DifficultyRNDSpeed();
        path = new NavMeshPath();
        elapsed = 0.0f;
    }
    void Update()
    {
        if (gameSystem.BlockedAttributesActive()) return;
        if (isDead) return;
        time = Time.deltaTime;
        if ((playerSystem.isDead && curState != orgState) || commandSystem.masterCodesActive[2] && curState != orgState) 
        { 
            curState = orgState;
            shotgunMuzzle.transform.Rotate(0, 30, 0);
            shotgunMuzzle.enabled = false;
            gunFlash = false;
            shotgunLight.enabled = false;
            gunflashTimer = gunflashTime;
            playerFound = false;
            playerVisible = false;
        }
       
        CheckPath();
        LineOfSight();
        ActiveState(curState);
        //IsDamaged();
        LookAtPlayerShooting();
        GunFlash();
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (tagSb.Length > 0) tagSb.Clear();
        tagSb = tagSb.Append(collision.gameObject.tag);
        for (int t = 0; t < tags.Length; t++)
        {
            if (tags[t] == tagSb.ToString())
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
                    case 4: if (health <= (maxHealth / 4)) MutilateEnemy(); else dmgAmt = Random.Range(5f, 10f); break;
                    //rocket
                    case 5: if (health <= (maxHealth / 4)) MutilateEnemy(); else dmgAmt = Random.Range(10f, 15); break;
                    //railgun
                    case 6: if (health <= (maxHealth / 4)) MutilateEnemy(); else dmgAmt = Random.Range(30.75f, 41.01f); break;
                    //photon
                    case 7: dmgAmt = Random.Range(3.60f, 5.1f); break;
                    //Sigma
                    case 8: MutilateEnemy(); break;
                    //Obstacle
                    case 9: dmgAmt = 1; break;
                    //MiniRocket
                    case 10: if (health <= (maxHealth / 4)) MutilateEnemy(); else dmgAmt = Random.Range(2.5f, 5); break;
                }
                float damage = powerupSystem.powerEnabled[2] ? 999 : powerupSystem.powerEnabled[0] ? Mathf.CeilToInt(dmgAmt) * 5 : dmgAmt;
                Damage(damage);
                if (t != 0)
                {
                    if (tags[t] == "GrenadeBullet" || tags[t] == "RocketBullet" || tags[t] == "RocketBulletMini")
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
                        if(tags[t] != "RailBullet") collision.gameObject.SetActive(false);
                    }
                }
                return;
            }
        }
        return;

    }
    //-------------------------------------------------------------------------------------------------------
    //---------------------------------STATES&ENGAGEMENT-----------------------------------------------------
    //-------------------------------------------------------------------------------------------------------
    private Vector3 PlayerPosition()
    {
        if (playerSystem.isDead) return Vector3.zero;
        // Player position in world space
        return playerSystem.transform.position;
    }
    private float DifficultyRNDSpeed()
    {
        if (optionsSystem == null) optionsSystem = OptionsSystem.optionsSystem;
        float diffRange = Random.Range(25, 31);
        //if (optionsSystem.difficultyActive[0]) diffRange = Random.Range(10, 16);
        //else if (optionsSystem.difficultyActive[1]) diffRange = Random.Range(15, 21);
        //else if (optionsSystem.difficultyActive[2]) diffRange = Random.Range(20, 26);
        //else if (optionsSystem.difficultyActive[3]) diffRange = Random.Range(25, 31);
        return diffRange;
    }
    public void EngagePlayer()
    {
        curState = State.Chase;
        //activate the movement random speed based on difficulty
        randomSpeed = DifficultyRNDSpeed();
        //Enemy has found the player
        FoundPlayer();
        SetAnimation();
        ActiveState(curState);
    }
    private void FreezeMovement()
    {
        if (curSpeed != 0) curSpeed = 0;
        if (chasePosition != transform.localPosition) chasePosition = transform.localPosition;
        //Set destination to itself
        if (navAgent.destination != chasePosition) SetNav(chasePosition, 0, true);
    }
    private void ActiveState(State state)
    {
        float playerDistance = Vector3.Magnitude(transform.position - PlayerPosition());
        if (!allowMovement) FreezeMovement();
        switch (state)
        {
            case State.Idle:
                {
                    //Current state 0
                    stateIndex = 0;
                    SetAnimation();
                    //State is Idle No Movement
                    FreezeMovement();

                    if (!playerFound && playerDistance <= enemyStateRanges[0])
                    {
                        //dont engage player if behind a wall
                        if (behindWall) return;
                        //if the enemy sees the player then engage
                        if (playerVisible) { EngagePlayer(); return; }
                    }
                    else if (playerFound) { CheckRadius(playerDistance); return; }

                    break;
                }
            case State.Walk:
                {

                    //Current state at 1
                    stateIndex = 1;
                    SetAnimation();
                    //set the spee to walk
                    curSpeed = walkSpeed;
                    //set the navigation to assigned positions
                    ActiveWaitTimer();

                    float dist = Vector3.Distance(chasePosition, transform.position);
                    if (dist < 2)
                    {
                        if (waitBetweenPositions) isWaiting = true;
                        else ChangeDestination(randomizePositions, backTrackPositions);
                    }
                    if (!isWaiting)
                    {
                        if (chasePosition != Destination()) chasePosition = Destination();
                        if (navAgent.destination != chasePosition) SetNav(chasePosition, 0, true);
                    }
                    if (!playerFound && playerDistance <= enemyStateRanges[0])
                    {
                        //dont engage player if behind a wall
                        if (behindWall) return;
                        //if the enemy sees the player then engage
                        if (playerVisible) { EngagePlayer(); return; }
                    }
                    else if (playerFound) { CheckRadius(playerDistance); return; }
                    break;
                }
            case State.Chase:
                {
                    NextStateTimer(playerDistance);
                    //Current state at 2
                    stateIndex = 2;
                    SetAnimation();
                    //set the animation speed based on movement speed;
                    SetAnimationSpeed(curSpeed/1.25f);
                    //if the current Speed doesnt already equal the random value grab a random destination on the nav mesh based on the player radius
                    if (curSpeed != randomSpeed)
                    {
                        //random position near the player or players position
                        chasePosition = GetPositionOnNavMesh(MoveInRadius(Random.Range(-20, 21), transform.TransformDirection(Vector3.forward).z * Random.Range(-6, 6)));
                        //set the nav to chase position
                        SetNav(chasePosition, 0, true);
                        //set the curent move speed to random speed so it doesnt kepp changing destination
                        curSpeed = randomSpeed;
                    }

                    if (!lookForPlayer)
                    {
                        if (navAgent.pathStatus == NavMeshPathStatus.PathComplete && updateNextPosition)
                        {
                            chasePosition = GetPositionOnNavMesh(PlayerPosition());
                            if (navAgent.destination != chasePosition) SetNav(chasePosition, 5, true);
                            updateNextPosition = false;
                        }
                        //if the nav agent doesnt have a complete path to destination point then change the destination
                        else if (navAgent.pathStatus != NavMeshPathStatus.PathComplete)
                        {
                            randomSpeed = DifficultyRNDSpeed();
                            lookForPlayer = true;
                        }
                        else if (navAgent.pathStatus == NavMeshPathStatus.PathComplete && !updateNextPosition)
                        {
                            float dist = navAgent.remainingDistance;
                            if (dist != Mathf.Infinity && navAgent.remainingDistance < 10)
                            {
                                //changing the random speed reactivates case 0 so enemy continues looking for player;
                                randomSpeed = DifficultyRNDSpeed();
                            }
                        }
                    }
                    else
                    {
                        //player was lost but got close reset the looktimer
                        if (playerDistance < enemyStateRanges[1])
                        {
                            randomSpeed = DifficultyRNDSpeed();
                            lookForPlayer = false;
                            lookTimer = lookTime;
                        }
                        //continue looking for player, once time runs out... walk.
                        else if (playerDistance > enemyStateRanges[2])
                        {
                            lookTimer -= time;
                            lookTimer = Mathf.Clamp(lookTimer, 0, lookTime);
                            if (lookTime == 0)
                            {
                                randomSpeed = DifficultyRNDSpeed();
                                lookForPlayer = false;
                                curState = State.Walk;
                                lookTimer = lookTime;
                            }
                        }
                    }
                    break;
                }
            case State.Shoot:
                {
                    stateIndex = 3;
                    SetAnimation();
                    allowMovement = false;
                    FreezeMovement();
                    float difficultyInterval = 1;
                    if (optionsSystem.difficultyActive[3])
                        difficultyInterval = 2f;
                    else if (optionsSystem.difficultyActive[2])
                        difficultyInterval = 1.5f;
                    else if (optionsSystem.difficultyActive[1])
                        difficultyInterval = 1.25f;
                    else if (optionsSystem.difficultyActive[0])
                        difficultyInterval = 1;
                    if (anim.GetFloat("Speed") != difficultyInterval)
                        anim.SetFloat("Speed", difficultyInterval);
                    if (!AnimatorIsPlaying("Shoot"))  CheckRadius(playerDistance);
                    break;
                }
            case State.Damage:
                {
                    stateIndex = 4;
                    if (!AnimatorIsPlaying("Damage") && !isDamaged) 
                    {
                        CheckRadius(playerDistance); 
                    }
                    else
                    {
                        SetAnimation();
                        FreezeMovement();
                        allowMovement = false;
                    }
                    break;
                }
            case State.Death:
                {
                    stateIndex = 5;
                    SetAnimation();
                    FreezeMovement();
                    break;
                }
        }
      
    }
    private void SetAnimation()
    {
        if (curStateIndex != stateIndex)
        {
            currentStateName = StateName();
            curStateIndex = stateIndex;
            curAnim = animNames[stateIndex];
            ActiveAnimation(curAnim);
        }
    }
    private void CheckRadius(float distance)
    {
        
        //if player is visible & close to enemy then fire at player, or idle
        if (distance <= enemyStateRanges[0]) curState = playerVisible ? State.Shoot : State.Idle;
        //if player is visible & farther away from enemy then choose to shoot or chase
        else if (distance > enemyStateRanges[0] && distance <= enemyStateRanges[1])
        {
            int rnd = Random.Range(0, 2);
            curState = rnd == 0 ? State.Chase : playerVisible ? State.Shoot : State.Chase;
            if (curState != State.Shoot) { randomSpeed = DifficultyRNDSpeed(); }

        }
        else if (distance > enemyStateRanges[1] && distance <= enemyStateRanges[2]) 
        { 
            curState = State.Chase; 
            randomSpeed = DifficultyRNDSpeed(); 
        }

        else if (distance > enemyStateRanges[2])
        {
            curState = State.Walk;
            playerFound = false;
            playerVisible = false;
        }
    }
    private void ActiveWaitTimer()
    {
        //wait between time before changing the destination
        if (!isWaiting) return;
        if (curSpeed != 0) curSpeed = 0;
        if (chasePosition != transform.position) chasePosition = transform.position;
        if (navAgent.destination != chasePosition) SetNav(chasePosition, 0, false);
        if(!AnimatorIsPlaying("Idle")) ActiveAnimation("Idle");
        waitTimer -= time;
        waitTimer = Mathf.Clamp(waitTimer, 0, waitTime);
        if (waitTimer == 0)
        {
            if (!AnimatorIsPlaying("Walk")) ActiveAnimation("Walk");
            waitTime = Random.Range(1, 5);
            waitTimer = waitTime;
            isWaiting = false;
            //Set the position index
            ChangeDestination(randomizePositions, backTrackPositions);
            if (chasePosition != Destination()) chasePosition = Destination();
            if (navAgent.destination != chasePosition) SetNav(chasePosition, 0, true);
            //Set the assigned position to position index value
        }
    }
    private void NextStateTimer(float distance)
    {
        //change state after time is reduced
        stateTimer -= time;
        stateTimer = Mathf.Clamp(stateTimer, 0, 5);
        if (stateTimer == 0)
        {
            //check the player radius
            CheckRadius(distance);
            //reset the state timer
            stateTimer = SetStateTimer();
        }
    }
    private float SetStateTimer()
    {
        bool[] difficultyActive = new bool[4] { optionsSystem.difficultyActive[0], optionsSystem.difficultyActive[1], optionsSystem.difficultyActive[2], optionsSystem.difficultyActive[3] };
        int difVal = 0;
        float repeatTime = 0;
        for (int d = 0; d < difficultyActive.Length; d++)
        {
            if (difficultyActive[d]) difVal = d;
        }
        switch (difVal)
        {
            case 0: { repeatTime = Random.Range(1, 6); break; }
            case 1: { repeatTime = Random.Range(1, 4); break; }
            case 2: { repeatTime = Random.Range(1, 2); break; }
            case 3: { repeatTime = 0.1f; break; }
        }
        return repeatTime;
    }
    private string StateName()
    {
        if (stringBuilderState.Length > 0) stringBuilderState.Clear();
        stringBuilderState.Append(stateName[stateIndex]);
        return stringBuilderState.ToString();
    }
    //-------------------------------------------------------------------------------------------------------
    //---------------------------------ANIMATION-------------------------------------------------------------
    //-------------------------------------------------------------------------------------------------------
    private void ActiveAnimation(string animation)
    {
        for (int an = 0; an < animNames.Length; an++)
        {
            if (animNames[an] == animation) anim.SetBool(animNames[an], true);
            else anim.SetBool(animNames[an], false);
        }
    }
    private bool AnimatorIsPlaying()
    {
        return anim.GetCurrentAnimatorStateInfo(0).length >
               anim.GetCurrentAnimatorStateInfo(0).normalizedTime - 1;
    }
    private bool AnimatorIsPlaying(string stateName)
    {
        return AnimatorIsPlaying() && anim.GetCurrentAnimatorStateInfo(0).IsName(stateName);
    }
    private void SetAnimationSpeed(float speed)
    {
        if (speed < 10) return;
        if (stateIndex != 2) speed = 1;
        else
        {
            float multiplier = speed / 10;
            speed = multiplier;
        }
        if (anim.GetFloat("Speed") != speed)
            anim.SetFloat("Speed", speed);
    }
    private void IsDamaged()
    {
        if (!isDamaged) return;
        damageTimer -= time;
        damageTimer = Mathf.Clamp(damageTimer, 0, damageTime);
        if (damageTimer == 0)
        {
            damageTime = Random.Range(4f, 7);
            damageTimer = damageTime;
            isDamaged = false;
        }

    }
    public void DMGAnimFinished()
    {
        curState = isDead ? State.Death : State.Chase;
        if (curState == State.Chase)
        {
            allowMovement = true;
            randomSpeed = DifficultyRNDSpeed();
        }
        isDamaged = false;
    }
    public void ShootAnimFinished()
    {
        allowMovement = true;
    }
    //-------------------------------------------------------------------------------------------------------
    //---------------------------------NAVAGATION------------------------------------------------------------
    //-------------------------------------------------------------------------------------------------------
    private void SetNav(Vector3 pos, float stop, bool active)
    {
        Vector3 newPos = active ? pos : transform.localPosition;
        if (navAgent.destination != newPos)
            navAgent.destination = newPos;
        else return;
        if (isDead) return;
        //navAgent.updatePosition = true;
        //navAgent.updateRotation = true;
        navAgent.enabled = true;
        navAgent.autoBraking = false;
        navAgent.autoRepath = active;
        navAgent.speed = active ? curSpeed : 0;
        navAgent.angularSpeed = active ? 300 : 0;
        navAgent.stoppingDistance = stop;
  
    }
    private void ChangeDestination(bool random, bool backTrack)
    {
        if (random) curPositionIndex = Random.Range(0, movePositions.Length);
        else curPositionIndex += backTrack ? -1 : 1;

        if (curPositionIndex > movePositions.Length - 1) curPositionIndex = 0;
        else if (curPositionIndex < 0) curPositionIndex = movePositions.Length - 1;
    }
    private Vector3 Destination()
    {
        if (movePositions[0] == null) return Vector3.zero;
        Vector3 direction = new Vector3(movePositions[curPositionIndex].position.x, transform.position.y, movePositions[curPositionIndex].position.z);
        return direction;
    }
    private Vector3 MoveInRadius(float distanceX, float distanceZ)
    {
        Vector3[] positions = new Vector3[3]
        {
            new Vector3(PlayerPosition().x + distanceX, transform.position.y, PlayerPosition().z + distanceZ),
            new Vector3(transform.position.x + distanceX, transform.position.y, transform.position.z + distanceX),
            PlayerPosition()
        };
        int rndIndex = Random.Range(0, 3);
        Vector3 movePosition = positions[rndIndex];
        return movePosition;
    }
    public Vector3 GetPositionOnNavMesh(Vector3 position)
    {
        NavMeshHit hit;
        bool positionFound = NavMesh.SamplePosition(position, out hit, 2, NavMesh.AllAreas);
        return positionFound ? hit.position : position;
    }
    private void CheckPath()
    {
        elapsed += time;
        if (elapsed > 1.5f)
        {
            elapsed -= 1.5f;
            NavMesh.CalculatePath(transform.position, chasePosition, NavMesh.AllAreas, path);
            updateNextPosition = true;
        }
        for (int i = 0; i < path.corners.Length - 1; i++)
            Debug.DrawLine(path.corners[i], path.corners[i + 1], Color.red);
    }
    private void LineOfSight()
    {
        //If player is dead disregard method
        if (playerSystem.isDead || commandSystem.masterCodesActive[2])
            return;
        //if (playerFound) return;
        //distance of the raycast from enemy to player
        distanceVector = PlayerPosition() - transform.position;
        //viewing angle from enemy to player
        float angle = Vector3.Angle(transform.forward, distanceVector);
        //when angle is below 60 engage player
        if (angle < viewAngle && angle > -viewAngle)
        {
            //Debug.DrawRay(transform.position, distanceVector - transform.forward, Color.magenta);
            if (Physics.Raycast(transform.position, distanceVector - transform.forward, out playerHit, viewDistance))
            {
                //if the raycast hit the player player is now found, activate shooting if player is within range
                if (playerHit.collider.gameObject.CompareTag("Player"))
                {
                    FoundPlayer();
                    playerVisible = true;
                }
                else playerVisible = false;
            }
        }
    }
    private void FoundPlayer()
    {
        if (playerFound) return;
        //if invisibility is active player is not found
        playerFound = powerupSystem.powerEnabled[1] ? false : true;
        if (playerFound)
        {
            waitTime = Random.Range(1, 5);
            waitTimer = waitTime;
            isWaiting = false;
            audioSrc.PlayOneShot(enemySounds[0]);
        }
    }
    //-------------------------------------------------------------------------------------------------------
    //---------------------------------WEAPON----------------------------------------------------------------
    //-------------------------------------------------------------------------------------------------------
    private void Shoot()
    {
        //Method is triggered in animation.
        if (isDead) return;
        gunFlash = true;
        audioSrc.PlayOneShot(enemySounds[1]);
        GameObject bullet = AccessPool(bulletPool, bulletPrefab);
        if (characterController == null) { playerSystem = PlayerSystem.playerSystem; characterController = playerSystem.GetComponent<CharacterController>(); }
        Physics.IgnoreCollision(bullet.GetComponent<Collider>(), characterController);
        BulletSystem bulletSystem = bullet.GetComponent<BulletSystem>();
        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        emitter.transform.LookAt(Camera.main.transform.position);
        rb.velocity = Vector3.zero;
        bullet.transform.position = emitter.position;
        bullet.transform.rotation = emitter.rotation;
        bullet.SetActive(true);
        bulletSystem.SetupLifeTime(5);
        rb.AddForce(emitter.transform.forward * bulletForce);
       // float distance = Vector3.Magnitude(transform.position - PlayerPosition());
        curState = State.Chase;
        randomSpeed = DifficultyRNDSpeed();
    }
    private void GunFlash()
    {
        if (!gunFlash) return;
        shotgunMuzzle.enabled = true;
        shotgunLight.enabled = optionsSystem.showFlashEffects;
        gunflashTimer -= time;
        gunflashTimer = Mathf.Clamp(gunflashTimer, 0.0f, 0.05f);
        if (gunflashTimer == 0.0f)
        {
            shotgunMuzzle.transform.Rotate(0, 30, 0);
            shotgunMuzzle.enabled = false;
            gunFlash = false;
            shotgunLight.enabled = false;
            gunflashTimer = gunflashTime;
        }
    }
    private void LookAtPlayerShooting()
    {
        if (commandSystem.masterCodesActive[2]) return;
        if (!AnimatorIsPlaying("Shoot") && !AnimatorIsPlaying("Damage")) return;
        Vector3 lookVector = PlayerPosition() - transform.position;
        lookVector.y = transform.position.y;
        Quaternion rot = Quaternion.LookRotation(lookVector);
        Quaternion newRotation = Quaternion.Slerp(transform.rotation, rot, 1);
        newRotation.x = 0;
        newRotation.z = 0;
        transform.rotation = newRotation;
        navAgent.transform.rotation = newRotation;
        //navAgent.updateRotation = true;
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
    //-------------------------------------------------------------------------------------------------------
    //---------------------------------DAMAGE----------------------------------------------------------------
    //-------------------------------------------------------------------------------------------------------
    public void Damage(float amt)
    {
        if (isDead) return;
        allowMovement = false;
        dmgTaken += amt;
        if (amt >= health)
        {
            if (EligibleForOverKill()) { OverKill(); return; }
        }
        health -= amt;
        health = Mathf.Clamp(health, 0, maxHealth);
        isDead = health < 1 ? true : false;
        if (!isDead) OnDamaged();
        else OnDeath();
      
    }
    private void OnDamaged()
    {
        if (!isDamaged)
        {
            isDamaged = true;
            curState = State.Damage;
            audioSrc.PlayOneShot(enemySounds[2]);
        }
    }
    public void MutilateEnemy()
    {
        OverKill();
    }
    private bool EligibleForOverKill()
    {
        int rng = Random.Range(0, 11);
        if (powerupSystem.powerEnabled[0] || powerupSystem.powerEnabled[2]) rng = 1;
        if (dmgTaken > maxHealth && rng == 1) return true;
        return false;
    }
    private void OverKill()
    {
        if (boxCollider.enabled)
        {
            for (int b = 0; b < bulletPool.childCount; b++)
            {
                if (bulletPool.GetChild(b).gameObject.activeInHierarchy)
                    bulletPool.GetChild(b).gameObject.SetActive(false);
            }
            health = 0;
            enemyBody.SetActive(false);
            goreSystem.ExplodeGore();
            isDead = true;
            navAgent.enabled = false;
            boxCollider.enabled = false;
            shotgunMuzzle.transform.Rotate(0, 30, 0);
            shotgunMuzzle.enabled = false;
            gunFlash = false;
            shotgunLight.enabled = false;
            gunflashTimer = gunflashTime;
            DropAmmo();
        }
    }
    private void OnDeath()
    {
        if (boxCollider.enabled)
        {
            for(int b = 0; b < bulletPool.childCount; b++) 
            {
                if (bulletPool.GetChild(b).gameObject.activeInHierarchy)
                    bulletPool.GetChild(b).gameObject.SetActive(false);
            }
            curState = State.Death;
            navAgent.enabled = false;
            boxCollider.enabled = false;
            audioSrc.PlayOneShot(enemySounds[3]);
            ActiveAnimation("Death");
            shotgunMuzzle.transform.Rotate(0, 30, 0);
            shotgunMuzzle.enabled = false;
            gunFlash = false;
            shotgunLight.enabled = false;
            gunflashTimer = gunflashTime;
            DropAmmo();
        }
    }
    private void DropAmmo()
    {

        GameObject ammoPack = AccessPool(ammoDropPool, ammoDropPrefab);
        ammoPack.transform.localPosition = Vector3.zero;
        ammoPack.transform.localRotation = Quaternion.identity;
        ammoPack.SetActive(true);
    }

 
    public void ResetObject(bool setOrgPos)
    {
        if (anim == null || audioSrc == null) Start();
        goreSystem.ResetGore();
        enemyBody.SetActive(true);
        anim.Rebind();
        stateTimer = 1;
        waitTime = Random.Range(1, 5);
        waitTimer = Random.Range(0, 10);
        damageTime = Random.Range(0.25f, 0.5f);
        damageTimer = damageTime;
        lookTimer = lookTime;
        gunFlash = false;
        gunflashTimer = gunflashTime;
        stateTimer = SetStateTimer();
        elapsed = 0.0f;
        health = maxHealth;
        dmgTaken = 0;
        isWaiting = false;
        isDamaged = false;
        playerFound = false;
        playerVisible = false;
        lookForPlayer = false;
        isDead = false;
        shotgunLight.enabled = false;
        shotgunMuzzle.enabled = false;
        randomSpeed = DifficultyRNDSpeed();
        allowMovement = true;
        curSpeed = 0;
       
        audioSrc.Stop();
        curState = orgState;
        switch (curState)
        {
            case State.Idle: stateIndex = 0; break;
            case State.Walk: stateIndex = 1; break;
            case State.Chase: stateIndex = 2; break;
            case State.Shoot: stateIndex = 3; break;
            case State.Damage: stateIndex = 4; break;
            case State.Death: stateIndex = 5; break;
        }
       
        for (int a = 0; a < ammoDropPool.childCount; a++) 
        {
            if (ammoDropPool.GetChild(a).gameObject.activeInHierarchy)
                ammoDropPool.GetChild(a).gameObject.SetActive(false);
        }
        navAgent.enabled = true;
        boxCollider.enabled = true;
        SetAnimation();
        if (!setOrgPos) return;
        navAgent.Warp(originalPosition);
        transform.position = originalPosition;
    }
}
