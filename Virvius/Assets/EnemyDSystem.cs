﻿using UnityEngine.AI;
using System.Text;
using UnityEngine;
public class EnemyDSystem : MonoBehaviour
{
    //=================================ScriptAssignment====================================//
    private CommandSystem commandSystem;
    private OptionsSystem optionsSystem;
    private PowerupSystem powerupSystem;
    private PlayerSystem playerSystem;
    private AudioSource audioSrc;
    private NavMeshAgent navAgent;
    private Animator animator;
    private StringBuilder currentState = new StringBuilder();
    private StringBuilder collisionTag = new StringBuilder();
    private GameSystem gameSystem;
    private WeaponSystem weaponSystem;
    [SerializeField]
    private MeshCollider[] meshColliders;

    public enum EnemyState { idle, walk, chase, attack, damage, death }

    //starting state
    [Header("Starting State")]
    [SerializeField]
    private EnemyState startState;

    [Space]
    [Header("Inspector Assignment")]
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
    [SerializeField]
    private float LOSAngle = 0;
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
    private bool behindWall = false;


    private RaycastHit playerHit;
    private Vector3 randomPosition;
    private Vector3 originalPosition;
    private EnemyState enemyState;
    private EnemyState activeState;
    private int distanceIndex = 0;
    private int walkPositionIndex = 0;
    private string[] animStates = new string[7]
    {
        "Idle",
        "Walk",
        "Chase",
        "Attack",
        "Damage",
        "Death",
        "Jump"
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
        90.0f,
        120.0f,
        180.0f,
    };
    private float[] movementSpeeds = new float[5]
    {
        0,
        25,
        50,
        75,
        100
    };
    private float movementSpeed = 0;
    private float stoppingDistance = 0;
    private float damageResistanceTimer;
    private float time;
    private float maxHealth = 0;
    private float dmgTaken = 0;
    private float elapsed = 2.5f;
    [HideInInspector]
    public bool playerFound = false;
    private bool disableFindRange = false;
    private bool autoBraking = false;
    private bool playerVisible = false;
    private bool isDamaged = false;
    private bool updateNextPosition = false;

    private void Start()
    {
        CheckActiveComponents();
        enemyState = startState;
        maxHealth = health;
        ActiveState();
    }
    void Update()
    {
        if (gameSystem.BlockedAttributesActive()) return;
        if (isDead) return;
        time = Time.deltaTime;
        CheckPath();
        CheckDistance();
        IsDamaged();
        LineOfSight();
        LookAtPlayerShooting();
    }
    public void PlayAttackSound()
    {
        audioSrc.PlayOneShot(enemySounds[1]);
    }
    public void CollisionDamage(string tag, GameObject colGameObject)
    {
        if (collisionTag.Length > 0) collisionTag.Clear();
        collisionTag = collisionTag.Append(tag);
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
                    case 4: if (health <= (maxHealth / 4)) OverKill(); else dmgAmt = Random.Range(5f, 10f); break;
                    //rocket
                    case 5: if (health <= (maxHealth / 4)) OverKill(); else dmgAmt = Random.Range(10f, 15); break;
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
                        if (colGameObject.TryGetComponent(out GrenadeSystem grenadeSystem))
                            grenadeSystem.Detonate();
                        if (colGameObject.TryGetComponent(out RocketSystem rocketSystem))
                            rocketSystem.Detonate();
                        if (colGameObject.TryGetComponent(out RocketSubSystem rocketSubSystem))
                            rocketSubSystem.Detonate();
                    }
                    else
                    {
                        if (collisionTags[t] != "RailBullet" && collisionTags[t] != "SigmaBullet" && collisionTags[t] != "ObstacleExplosive") colGameObject.SetActive(false);
                    }
                }
                return;
            }
        }
        return;
    }
    private void CheckActiveComponents()
    {
        if (commandSystem == null) commandSystem = CommandSystem.commandSystem;
        if (playerSystem == null) playerSystem = PlayerSystem.playerSystem;
        if (optionsSystem == null) optionsSystem = OptionsSystem.optionsSystem;
        if (powerupSystem == null) powerupSystem = PowerupSystem.powerupSystem;
        if (navAgent == null) navAgent = GetComponent<NavMeshAgent>();
        if (audioSrc == null) audioSrc = GetComponent<AudioSource>();
        if (animator == null) animator = GetComponent<Animator>();
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
            //Debug.DrawRay(transform.position, distanceVector - transform.forward, Color.magenta);
            if (Physics.Raycast(transform.position, distanceVector - transform.forward, out playerHit, LOSDistance))
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
        if (powerupSystem.powerEnabled[1]) return;
        if (disableFindRange || playerFound) return;
        audioSrc.PlayOneShot(enemySounds[0]);
        playerFound = true;
    }
    private void CheckPath()
    {
        elapsed += time;
        if (elapsed > 0.5f)
        {
            elapsed -= 0.5f;
            updateNextPosition = true;
        }
    }
    private void CheckDistance()
    {
        if (!updateNextPosition) return;
        if (PlayerSystem.playerSystem == null || PlayerDistance() == 0) return;
        if (AnimatorIsPlaying("Attack") || AnimatorIsPlaying("Jump") || AnimatorIsPlaying("Damage") || enemyState == EnemyState.death) return;
        //HAS FOUND THE PLAYER - PLAYER GOT TOO CLOSE, ACTIVE IF ENEMY IS NOT BEHIND A WALL
        if (PlayerDistance() <= distanceRanges[2]) { if(!behindWall) FoundPlayer(); }
        //HAS LOST THE PLAYER - ANIMATION SET TO BACK DEFAULT [WALK OR IDLE]
        if (PlayerDistance() > distanceRanges[3])
        {
            playerFound = false;
            playerVisible = false;
            enemyState = startState;
            ActiveState();
        }

        if (!playerFound) return;
        //ATTACKS IN CLOSE RANGE
        if (PlayerDistance() <= distanceRanges[0])
        {
            EnemyState state = playerVisible ? EnemyState.attack : EnemyState.chase;
            enemyState = state;
            distanceIndex = state == EnemyState.attack ? 0 : 2;
            ActiveState();
        }
        //ATTACKS FROM DISTANCE
        if (PlayerDistance() > distanceRanges[0] && PlayerDistance() <= distanceRanges[1])
        {
            EnemyState state = playerVisible ? EnemyState.attack : EnemyState.chase;
            enemyState = state;
            distanceIndex = state == EnemyState.attack ? 1 : 3;
            ActiveState();
        }
        //MOVES TO PLAYER POSITION
        if (PlayerDistance() > distanceRanges[1] && PlayerDistance() <= distanceRanges[2])
        {
            enemyState = EnemyState.chase;
            distanceIndex = 2;
            ActiveState();
        }
        //MOVES TO STAGGERED PLAYER POSITION
        if (PlayerDistance() > distanceRanges[2] && PlayerDistance() <= distanceRanges[3])
        {
            enemyState = EnemyState.chase;
            distanceIndex = 3;
            ActiveState();
        }
        updateNextPosition = false;
    }
    public void AnimationFinished()
    {
        if (currentState.Length > 0) currentState.Clear();
        enemyState = EnemyState.chase;
        if(!damageResistance) isDamaged = false;
    }
    private void ActiveState()
    {
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
                    if ((!AnimatorIsPlaying("Attack") || !AnimatorIsPlaying("Jump")) && !AnimatorIsPlaying("Damage"))
                    {
                        if (animator.GetFloat("Speed") != 1.5f)
                            animator.SetFloat("Speed", 1.5f);
                        SetAnimation(1);
                        movementSpeed = movementSpeeds[1];
                        stoppingDistance = 0;
                        autoBraking = false;
                        float dist = Vector3.Distance(WalkDestination(), transform.position);
                        if (dist < 2)
                        {
                            ChangeDestination();
                        }
                        SetNav(WalkDestination(), true);
                    }
                    break;
                }
            case EnemyState.chase:
                {
                    if ((!AnimatorIsPlaying("Attack") || !AnimatorIsPlaying("Jump")) && !AnimatorIsPlaying("Damage"))
                    {
                        if (animator.GetFloat("Speed") != 1.1f)
                            animator.SetFloat("Speed", 1.1f);
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
                        animator.SetFloat("Speed", difficultyInterval);

                    ////ATTACK ANIM ID [ATTACK = 3] [JUMP = 6]
                    int attackID = (distanceIndex == 0) ? 3 : 6;
                    SetAnimation(attackID);
                    movementSpeed = (distanceIndex == 0) ? movementSpeeds[1] : movementSpeeds[4];
                    stoppingDistance = (distanceIndex == 0) ? 12.2f : 0;
                    autoBraking = false;
                    SetNav(playerSystem.transform.position, true);

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
        Vector3 newPos = active ? nextPosition : transform.localPosition;
        if (isDead) { navAgent.destination = transform.localPosition; return; }
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
    //private Vector3 MoveInPlayerRadius(float offsetX, float offsetZ)
    //{
    //    Vector3 player = (playerSystem != null) ? playerSystem.transform.position : Vector3.zero;
    //    if (player == Vector3.zero) { return transform.position; }
    //    Vector3[] positions = new Vector3[3]
    //    {
    //        new Vector3(player.x + offsetX, transform.position.y, player.z + offsetZ),
    //        new Vector3(transform.position.x + offsetX, transform.position.y, transform.position.z + offsetX),
    //        player
    //    };

    //    int rndIndex = Random.Range(0, 3);
    //    Vector3 movePosition = positions[rndIndex];
    //    return movePosition;
    //}
    private Vector3 WalkDestination()
    {
        if (walkPositions[0] == null) return transform.localPosition;
        Vector3 direction = new Vector3(walkPositions[walkPositionIndex].position.x, transform.position.y, walkPositions[walkPositionIndex].position.z);
        return direction;
    }
    private Vector3 RandomDestination()
    {
        randomPosition = PlayerPosition();
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
        if (currentState.ToString() != animStates[index])
        {
            if (currentState.Length > 0) currentState.Clear();
            currentState.Append(animStates[index]);
            ActiveAnimation(currentState.ToString());
        }
    }
    private void ActiveAnimation(string animation)
    {
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
    private void LookAtPlayerShooting()
    {
        if (commandSystem.masterCodesActive[2]) return;
        if (!AnimatorIsPlaying("Attack") && !AnimatorIsPlaying("Jump") && !AnimatorIsPlaying("Damage")) return;
        Vector3 lookVector = PlayerPosition() - transform.position;
        lookVector.y = transform.position.y - 5;
        Quaternion rot = Quaternion.LookRotation(lookVector);
        Quaternion newRotation = Quaternion.Slerp(transform.rotation, rot, 1f);
        newRotation.x = 0;
        newRotation.z = 0;
        transform.rotation = newRotation;
        navAgent.transform.rotation = newRotation;
    }
    public void OverKill()
    {
        for (int mc = 0; mc < meshColliders.Length; mc++)
        {
            if (meshColliders[mc].enabled) meshColliders[mc].enabled = false;
        }
        health = 0;
        enemyBody.SetActive(false);
        goreExplosion.SetActive(true);
        isDead = true;
        navAgent.enabled = false;
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
        else OnDeath();
    }
    private void OnDamaged()
    {
        if (!isDamaged)
        {
            isDamaged = true;
            if (currentState.Length > 0) currentState.Clear();
            enemyState = EnemyState.damage;
            audioSrc.PlayOneShot(enemySounds[2]);
            ActiveState();
        }
        else
        {
            if (currentState.Length > 0) currentState.Clear();
            enemyState = EnemyState.chase;
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
        for (int mc = 0; mc < meshColliders.Length; mc++)
        {
            if (meshColliders[mc].enabled) meshColliders[mc].enabled = false;
        }
        enemyState = EnemyState.death;
        ActiveState();
        navAgent.enabled = false;
        audioSrc.PlayOneShot(enemySounds[3]);
    }
    public void ResetObject(bool setOrgPos)
    {
        if (animator == null || audioSrc == null) Start();
        goreExplosion.SetActive(false);
        enemyBody.SetActive(true);
        animator.Rebind();
        damageResistanceTimer = damageResistanceTime;
        elapsed = 0.0f;
        health = maxHealth;
        dmgTaken = 0;
        isDamaged = false;
        playerFound = false;
        playerVisible = false;
        isDead = false;
        audioSrc.Stop();
        enemyState = startState;
        for (int mc = 0; mc < meshColliders.Length; mc++)
        {
            if (meshColliders[mc].enabled) meshColliders[mc].enabled = true;
        }
        if (!setOrgPos) return;
        navAgent.Warp(originalPosition);
        transform.position = originalPosition;
    }
}