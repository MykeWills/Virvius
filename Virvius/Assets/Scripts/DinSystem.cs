using UnityEngine;
using UnityEngine.AI;
using System.Text;

public class DinSystem : MonoBehaviour
{
    private CommandSystem commandSystem;
    private GameSystem gameSystem;
    private OptionsSystem optionsSystem;
    private PowerupSystem powerupSystem;
    private PlayerSystem playerSystem;
    private GoreSystem goreSystem;
    private AudioSource audioSrc;
    [HideInInspector]
    public NavMeshAgent navAgent;
    private Animator anim;
    [SerializeField]
    private MeshCollider[] meshColliders;
    private StringBuilder stringBuilderState = new StringBuilder();
    [SerializeField]
    private Transform[] movePositions = new Transform[4];
    private Vector3 distanceVector = Vector3.zero;
    private RaycastHit playerHit;
    [SerializeField]
    private bool randomizePositions = false;
    [SerializeField]
    private bool backTrackPositions = false;
    [SerializeField]
    private bool waitBetweenPositions;
    [SerializeField]
    private GameObject goreExplosion;
    [SerializeField]
    private GameObject enemyBody;
    [SerializeField]
    private AudioClip[] enemySounds;
    [SerializeField]
    private float viewAngle = 75; 
    [SerializeField]
    private float navStopDist = 12.2f;
    [SerializeField]
    private bool behindWall = false;
    private float[] enemyStateRanges = new float[3]
    {
        25f, //Stopping Range
        40.0f, //Awareness Range
        200.0f //Player Lost Range
    };
    private float idleSpeed = 0;
    private float walkSpeed = 2.5f;
    private float chaseSpeed = 30;
    public enum State { Idle, Walk, Chase, Attack, Jump, Damage, Death }
    public State curState = State.Walk;
    private State orgState;
    private string[] stateName = new string[7]
    {
        "Idling",
        "Walking",
        "Chasing",
        "Attacking",
        "Jumping",
        "Damaged",
        "Dead"
    };
    [SerializeField]
    private string currentStateName;
    [SerializeField]
    private int stateIndex = 0;
    private int curStateIndex = 9;
    private int curPositionIndex = 0;
    [SerializeField]
    private float curSpeed = 0;
    private float viewDistance = 0;
    private string[] animNames = new string[7] { "Idle", "Walk", "Chase", "Death", "Attack", "Jump", "Damage" };
    private string curAnim = null;
    [HideInInspector]
    public bool isDead = false;
    [HideInInspector]
    public float health = 50;
    [HideInInspector]
    public float maxHealth = 0;
    private float dmgTaken = 0;
    private float time;
    private float stateTimer = 0;
    [HideInInspector]
    public bool playerFound = false;
    private float waitTime = 2;
    private float waitTimer;
    private bool isWaiting = false;
    private float lookTime = 10;
    private float lookTimer;
    private bool lookForPlayer = false;
    private float damageTime = 5;
    private float damageTimer;
    private bool isDamaged = false;
    private bool playerVisible = false;
    private float randomSpeed = 0;
    private Vector3 chasePosition;
    private Vector3 originalPosition;
    private Quaternion originalRotation;
    //goldenPath
    public Transform target;
    private NavMeshPath path;
    private float elapsed = 0.0f;
    private bool updateNextPosition = false;
    private bool allowMovement = true;
    private float playerDistance = 0;
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
        navAgent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        originalPosition = transform.localPosition;
        originalRotation = transform.localRotation;
        orgState = curState;
        stateTimer = 1;
        maxHealth = health;
        viewDistance = 150;
        waitTime = Random.Range(1, 5);
        waitTimer = waitTime;
        damageTime = Random.Range(4f, 7);
        damageTimer = damageTime;
        lookTimer = lookTime;
        randomSpeed = DifficultyRNDSpeed();
        path = new NavMeshPath();
        elapsed = 0.0f;
    }
    private void Update()
    {
        if (gameSystem.BlockedAttributesActive()) return;
        // if the enemy is dead disregard all
        if (isDead) return;
        playerDistance = Vector3.Magnitude(transform.position - PlayerPosition());
        if (playerDistance > 300) return;
        time = Time.deltaTime;
        // if the player is dead make the enemy return to org state
        if ((playerSystem.isDead && curState != orgState) || commandSystem.masterCodesActive[2] && curState != orgState) 
        { 
            curState = orgState;
            playerVisible = false;
            playerFound = false;
        }
           
        ActiveState(curState);
        IsDamaged();
        CheckPath();
        LineOfSight();
        LookAtPlayerAttack();
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
        float diffRange = Random.Range(15, 26);
        //if (optionsSystem.difficultyActive[0]) diffRange = Random.Range(10, 16);
        //else if (optionsSystem.difficultyActive[1]) diffRange = Random.Range(15, 21);
        //else if (optionsSystem.difficultyActive[2]) diffRange = Random.Range(20, 26);
        //else if (optionsSystem.difficultyActive[3]) diffRange = Random.Range(25, 31);
        return diffRange * 2.3f;
    }
    public void MutilateEnemy()
    {
        OverKill();
    }
    public void EngagePlayer()
    {
        //set the current state to chase. 
        //navAgent.updatePosition = false;
        //navAgent.updateRotation = false;
        curState = State.Chase;
        //activate the movement random speed based on difficulty
        randomSpeed = DifficultyRNDSpeed();
        //Enemy has found the player
        FoundPlayer();
    }
    private void ActiveState(State state)
    {
        anim.SetFloat("Speed", 1.5f);
        if (!allowMovement)
        {
            if (curSpeed != idleSpeed) curSpeed = idleSpeed;
            if (navAgent.destination != transform.position) SetNav(transform.position, 0, true);
            return;
        }
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
                    if (curSpeed != walkSpeed) curSpeed = walkSpeed;
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
                    SetAnimationSpeed(curSpeed / 5);
                    //if the current Speed doesnt already equal the random value grab a random destination on the nav mesh based on the player radius
                    if (curSpeed != randomSpeed)
                    {
                        //random position near the player or players position
                        chasePosition = GetPositionOnNavMesh(PlayerPosition());
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

            case State.Attack:
                {
                    stateIndex = 4;
                    SetAnimation();
                    if (curSpeed != 9) curSpeed = 9;
                    if (navAgent.pathStatus == NavMeshPathStatus.PathComplete && updateNextPosition)
                    {
                        chasePosition = PlayerPosition();
                        updateNextPosition = false;
                        //navAgent.updatePosition = true;
                    }
                    else { randomSpeed = DifficultyRNDSpeed(); }


                    if (navAgent.destination != chasePosition) 
                        SetNav(chasePosition, navStopDist, true);

                    if (!AnimatorIsPlaying("Attack"))
                    {
                        if (playerDistance <= enemyStateRanges[0])
                        {
                            if (playerVisible) { curState = State.Attack; }
                            else curState = State.Chase;
                        }
                        else if (playerDistance > enemyStateRanges[0] && playerDistance <= enemyStateRanges[1])
                        {
                            if (playerVisible) { curState = State.Jump; }
                            else curState = State.Chase;
                        }
                        else if (playerDistance > enemyStateRanges[1])
                        {
                            curState = State.Walk;
                            playerFound = false;
                        }
                        updateNextPosition = true;
                        //navAgent.updatePosition = false;
                    }
                    break;
                }
            case State.Jump:
                {
                    stateIndex = 5;
                    SetAnimation();
                    if (curSpeed != chaseSpeed) curSpeed = chaseSpeed * 3.2f;
                    if (navAgent.pathStatus == NavMeshPathStatus.PathComplete && updateNextPosition)
                    {
                        chasePosition = PlayerPosition();
                        updateNextPosition = false;
                    }
                    else { randomSpeed = DifficultyRNDSpeed(); }
                    if (navAgent.destination != chasePosition) SetNav(chasePosition, 1, true);
                    if (!AnimatorIsPlaying("Jump"))
                    {
                        if (playerDistance <= enemyStateRanges[0])
                        {
                            if (playerVisible) { curState = State.Attack; }
                            else curState = State.Chase;
                        }
                        else if (playerDistance > enemyStateRanges[0] && playerDistance <= enemyStateRanges[1])
                        {
                            if (playerVisible) { curState = State.Jump; }
                            else curState = State.Chase;
                        }
                        else if (playerDistance > enemyStateRanges[2])
                        {
                            curState = State.Walk;
                            playerFound = false;
                        }
                    }
                    break;
                }
            case State.Damage:
                {
                    stateIndex = 6;
                    SetAnimation();
                    if (allowMovement) allowMovement = false;
                    break;
                }
            case State.Death:
                {
                    stateIndex = 3;
                    SetAnimation();
                    if (curSpeed != 0) curSpeed = 0;
                    if (navAgent.destination != navAgent.transform.position) { SetNav(navAgent.transform.position, 0, false); }

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
        else if (stateIndex == 4 && !anim.GetCurrentAnimatorStateInfo(0).IsName("Attack")) ActiveAnimation(curAnim);
    }
    private void FreezeMovement()
    {
        if (curSpeed != 0) curSpeed = 0;
        if (chasePosition != transform.localPosition) chasePosition = transform.localPosition;
        //Set destination to itself
        if (navAgent.destination != chasePosition) SetNav(chasePosition, 0, true);
    }
    private void CheckRadius(float distance)
    {
        //if player is visible & close to enemy then fire at player, or idle
        if (distance <= enemyStateRanges[0]) curState = playerVisible ? State.Attack : State.Idle;
        //if player is visible & farther away from enemy then choose to shoot or chase
        else if (distance > enemyStateRanges[0] && distance <= enemyStateRanges[1])
        {
            int rnd = Random.Range(0, 2);
            curState = rnd == 0 ? State.Chase : playerVisible ? State.Attack : State.Chase;
            if (curState != State.Attack) { randomSpeed = DifficultyRNDSpeed(); }

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
    private void IsDamaged()
    {
        if (!isDamaged) return;
        damageTimer -= time;
        damageTimer = Mathf.Clamp(damageTimer, 0, damageTime);
        if (damageTimer == 0)
        {
            damageTime = Random.Range(3f, 6);
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
    }
    public void PlayAttackSound()
    {
        audioSrc.PlayOneShot(enemySounds[1]);
    }
    public void AllowMovement()
    {
        allowMovement = true;
    }
    private void ActiveWaitTimer()
    {
        if (!isWaiting) return;
        waitTimer -= time;
        waitTimer = Mathf.Clamp(waitTimer, 0, waitTime);
        if (waitTimer == 0)
        {
            waitTime = Random.Range(1, 5);
            waitTimer = waitTime;
            isWaiting = false;
            ChangeDestination(randomizePositions, backTrackPositions);
            if (navAgent.destination != Destination()) SetNav(Destination(), navStopDist, true);
        }
    }
    private void NextStateTimer(float distance)
    {
        //shoot the player after a time.
        stateTimer -= time;
        stateTimer = Mathf.Clamp(stateTimer, 0, 5);
        if (stateTimer == 0)
        {
            stateTimer = SetStateTimer();
            if (distance <= enemyStateRanges[0])
            {
                if (playerVisible) { curState = State.Attack; }
                else curState = State.Chase;
            }
            else if (distance > enemyStateRanges[0] && distance <= enemyStateRanges[1])
            {
                if (playerVisible) { curState = State.Jump; }
                else curState = State.Chase;
            }
            else if (distance > enemyStateRanges[1] && distance < enemyStateRanges[2])
            {
                randomSpeed = DifficultyRNDSpeed(); 
                curState = State.Chase;
            }
            else if (distance > enemyStateRanges[2])
            {
                curState = State.Walk;
                playerFound = false;
            }
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
            case 0: { repeatTime = Random.Range(3, 7); break; }
            case 1: { repeatTime = Random.Range(2, 6); break; }
            case 2: { repeatTime = Random.Range(1, 5); break; }
            case 3: { repeatTime = Random.Range(0, 4); break; }
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
            if(curStateIndex > 3)
            {
                if (animNames[an] == animation) anim.SetTrigger(animNames[an]);
            }
            else
            {
                if (animNames[an] == animation) anim.SetBool(animNames[an], true);
                else anim.SetBool(animNames[an], false);
            }
           
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
    //-------------------------------------------------------------------------------------------------------
    //---------------------------------NAVAGATION------------------------------------------------------------
    //-------------------------------------------------------------------------------------------------------
    private void SetNav(Vector3 pos, float distance, bool active)
    {
        if (isDead) return;

        //navAgent.updatePosition = active ? true : false;
        //navAgent.updateRotation = active ? true : false;
        navAgent.enabled = true;
        navAgent.autoBraking = false;
        navAgent.autoRepath = active;
        navAgent.speed = active ? curSpeed : 0;
        navAgent.angularSpeed = active ? 300 : 0;
        navAgent.stoppingDistance = distance;
        navAgent.destination = active ? pos : transform.position;

    }
    private void ChangeDestination(bool random, bool backTrack)
    {
        //curPositionIndex = random ? Random.Range(0, movePositions.Length) : backTrack ? curPositionIndex-- : curPositionIndex++;
        if (random) curPositionIndex = Random.Range(0, movePositions.Length);
        else curPositionIndex += backTrack ? -1 : 1;

        if (curPositionIndex > movePositions.Length - 1) curPositionIndex = 0;
        else if (curPositionIndex < 0) curPositionIndex = movePositions.Length - 1;
    }
    private Vector3 Destination()
    {
        if (movePositions[0] == null) return Vector3.zero;
        Vector3 direction = new Vector3(movePositions[curPositionIndex].localPosition.x, transform.localPosition.y, movePositions[curPositionIndex].localPosition.z);
        return direction;
    }
    private Vector3 MoveInRadius(float distanceX, float distanceZ)
    {
        Vector3[] positions = new Vector3[3]
        {
            new Vector3(PlayerPosition().x + distanceX, transform.localPosition.y, PlayerPosition().z + distanceZ),
            new Vector3(transform.localPosition.x + distanceX, transform.localPosition.y, transform.localPosition.z + distanceX),
            PlayerPosition()
        };
        int rndIndex = 2;
        Vector3 movePosition = positions[rndIndex];
        return movePosition;
    }
    public Vector3 GetPositionOnNavMesh(Vector3 position)
    {
        NavMeshHit hit;
        bool positionFound = NavMesh.SamplePosition(position, out hit, viewDistance, NavMesh.AllAreas);
        return positionFound ? hit.position : position;
    }
    private void CheckPath()
    {
        elapsed += time;
        if (elapsed > 2.0f)
        {
            elapsed -= 2.0f;
            NavMesh.CalculatePath(navAgent.transform.position, chasePosition, NavMesh.AllAreas, path);
            updateNextPosition = true;
           
        }
        //for (int i = 0; i < path.corners.Length - 1; i++)
        //Debug.DrawLine(path.corners[i], path.corners[i + 1], Color.red);
    }
    private void LineOfSight()
    {
        if (commandSystem.masterCodesActive[2]) return;
        //If player is dead disregard method
        if (playerSystem.isDead)
            return;
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
                    if(!playerFound)FoundPlayer();
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
            curState = State.Chase;
            audioSrc.PlayOneShot(enemySounds[0]);
        }
    }
    private void LookAtPlayerAttack()
    {
        if (!AnimatorIsPlaying("Attack") && !AnimatorIsPlaying("Jump")) return;
        Vector3 lookVector = PlayerPosition() - transform.position;
        //lookVector.y = transform.position.y;
        Quaternion rot = Quaternion.LookRotation(lookVector);
        Quaternion newRotation = Quaternion.Slerp(transform.rotation, rot, 0.5f);
        newRotation.x = 0;
        newRotation.z = 0;
        transform.rotation = newRotation;
    }
    //-------------------------------------------------------------------------------------------------------
    //---------------------------------DAMAGE----------------------------------------------------------------
    //-------------------------------------------------------------------------------------------------------
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
        else OnDeath();

    }
    private void OnDamaged()
    {
        if (!isDamaged)
        {
            isDamaged = true;
            if(!AnimatorIsPlaying("Jump")) curState = State.Damage;
            audioSrc.PlayOneShot(enemySounds[2]);
        }
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
        for (int mc = 0; mc < meshColliders.Length; mc++)
        {
            if (meshColliders[mc].enabled) meshColliders[mc].enabled = false;
        }
        health = 0;
        enemyBody.SetActive(false);
        goreSystem.ExplodeGore();
        isDead = true;
        navAgent.enabled = false;
    }
    private void OnDeath()
    {
        for (int mc = 0; mc < meshColliders.Length; mc++)
        {
            if (meshColliders[mc].enabled) meshColliders[mc].enabled = false;
        }
        curState = State.Death;
        navAgent.enabled = false;
        audioSrc.PlayOneShot(enemySounds[3]);
        ActiveAnimation("Death");
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
        randomSpeed = DifficultyRNDSpeed();
        allowMovement = true;
        curSpeed = 0;
        //navAgent.updateRotation = false;
        //navAgent.updatePosition = false;
        viewDistance = 150;
        audioSrc.Stop();
        curState = orgState;
        switch (orgState)
        {
            case State.Idle: stateIndex = 0; break;
            case State.Walk: stateIndex = 1; break;
            case State.Chase: stateIndex = 2; break;
            case State.Death: stateIndex = 3; break;
            case State.Attack: stateIndex = 4; break;
            case State.Jump: stateIndex = 5; break;
            case State.Damage: stateIndex = 6; break;
           
        }
        navAgent.enabled = true;
        SetAnimation();
        for (int mc = 0; mc < meshColliders.Length; mc++)
        {
            if (!meshColliders[mc].enabled) meshColliders[mc].enabled = true;
        }
        if (!setOrgPos) return;
        navAgent.Warp(originalPosition);
        transform.position = originalPosition;
       

    }
}
