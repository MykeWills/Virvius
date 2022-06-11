using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;
public class InputSystem : MonoBehaviour
{  
    //========================================================================================//
    //===================================[STATIC FIELDS]=======================================//
    //========================================================================================//
    public static InputSystem inputSystem;
    //========================================================================================//
    //===================================[PRIVATE FIELDS]======================================//
    //========================================================================================//
    //[Class Access]++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++//
    [SerializeField] private float playerToFloorLength;
    private OptionsSystem optionsSystem;
    private EnvironmentSystem environmentSystem;
    private PlayerSystem playerSystem;
    private CharacterController controller;
    private PowerupSystem powerupSystem;
    private GameSystem gameSystem;
    private AudioSystem audioSystem;
    private CommandSystem commandSystem;
    [HideInInspector]
    public Player inputPlayer;
    //[Components]++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++//
    private Transform head;
    private Transform headBob;
    //[Structs & Enums]+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++//
    public enum weaponAnimType { up, left, right, jump, idle, origin };
    public enum FootElement { Wood, Metal, Water, Normal }
    private enum RotationAxes { XY, X, Y };
    private RotationAxes axis = RotationAxes.XY;
    private FootElement footElement = FootElement.Normal;
    private FootElement currentFootElement = FootElement.Normal;
    private Vector3 contactPoint = new Vector3();
    private Vector3 fallStartLevel = new Vector3();
    private Vector3 lastStepPosition = new Vector3();
    private Vector3 headStartPos = new Vector3();
    private Vector3 swimDir = Vector3.zero;
    private Vector3[] bobVectors = new Vector3[3];
    private RaycastHit playerHit = new RaycastHit();
    private RaycastHit wallHit = new RaycastHit();
    private RaycastHit controllerHit = new RaycastHit();
    //[Variables]+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++//
    private bool fallDamage = false;
    [HideInInspector]
    public bool isGrounded = false;
    private bool isSliding = false;
    private bool isSinking = false;
    private bool playLedgeJumpSound = false;
    private bool swimGrounded = false;
    [HideInInspector]
    public bool overTilt = false;
    private bool startSwimTimer = false;
    private bool startSwimSound = false;
    private bool switchDir = false;
    private bool breathOut = false;
    private bool breathOutActive = false;
    private bool diveUnder = false;
    [SerializeField]
    private float moveSpeed = 40;
    [SerializeField]
    private float runSpeed = 60;
    [SerializeField]
    private float strafeSpeed = 20;
    [SerializeField]
    private float runStrafeSpeed = 40;

    private float clampLookAngle = 75;
    private float lookAxis = 0;
    [SerializeField]
    private float jumpSpeed = 40f;
    private float antiBumpFactor = 0.75f;
    private float tiltAngle = 0;
    private float tiltSpeed = 0.5f;
    private float tiltReturnRate = 120;
    private float fallingDamageThreshold = 10.0f;
    private float swimTime = 1;
    private float swimTimer = 0;
    private float lastFootstepPlayedTime = 0;
    private float velocityPollPeriod = 0.2f;
    private float currentFootstepsWaitingPeriod = 0;
    [SerializeField]
    private float stepSoundRatioA = 1.0f;
    [SerializeField]
    private float stepSoundRatioN = 2.0f;
    [SerializeField]
    private float stepSoundRatioB = 1.0f;
    [SerializeField]
    private float stepSoundRatioC = 80.0f;
    private float timePerStep = 0;
    private float stepsPerSecond = 0;
    private float headbobSpeedY = 3f;
    private float AboveOrBelowZeroY = 0;
    private float UpAndDownAmount = 10;
    private float upAmount = 1.5f;
    private float leftRightAmount = 0.5f;
    private float jumpAmount = 1.5f;
    private float bobSpeed = 4.1f;
    private float waterGravity = 5;
    private float time = 0;
    [HideInInspector]
    public float[] lookRotation = new float[2];
    private int antiJumpFactor = 1;
    private int bobIndex = 0;
    private int divisionIndex = 3;
    private Vector3 returnPosition = Vector3.zero;
    private Vector3 onePointPosition;
    private Vector3 twoPointPosition;
    private bool twoPoint = false;
    private float twoPointReturnRate;
    [HideInInspector]
    public float shakeAmt = 0.5f;
    [SerializeField]
    private float shakeTime = 1;
    private float shakeTimer = 0;
    private bool isShaking = false;
    //[public Access (Non Inspector)]+++++++++++++++++++++++++++++++++++++++++++++++++++++++++//
    [HideInInspector]
    public Vector3 moveDirection = Vector3.zero;
    [HideInInspector]
    public float inputX;
    [HideInInspector]
    public float inputY;
    [HideInInspector]
    public int jumpTimer;
    [HideInInspector]
    public float gravity;
    [HideInInspector]
    public bool isJumping = true;
    [HideInInspector]
    public bool isSwimming = false;
    [HideInInspector]
    public bool isFalling = false;
    [HideInInspector]
    public bool isRunning = false;
    [HideInInspector]
    public bool limitDiagonalSpeed = false;
    //========================================================================================//
    //===================================[INSPECTOR FIELDS]====================================//
    //========================================================================================//
    //[private Access (Inspector)]++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++//
    [Range(20, 120)]
    [SerializeField]
    private float gravityPull = 100;
    // Player Sliding
    [Range(0, 70)]
    [SerializeField]
    private float slideAngle = 50;
    [Range(0, 100)]
    [SerializeField]
    private float slideSpeed = 40;
    [SerializeField]
    private bool slideOnAngle = true;
    [SerializeField]
    private bool slideOnTag = false;
    [SerializeField]
    private AudioClip playerJSound;
    [SerializeField]
    private AudioClip[] playerSWSound = new AudioClip[2];
    [SerializeField]
    private AudioClip[] playerFSNormSounds = new AudioClip[1];
    [SerializeField]
    private AudioClip[] playerFSWaterSounds = new AudioClip[3];
    [SerializeField]
    private AudioClip[] playerFSWoodSounds = new AudioClip[3];
    [SerializeField]
    private AudioClip[] playerFSMetalSounds = new AudioClip[4];

    //========================================================================================//
    //===================================[UNITY FUNCTIONS]====================================//
    //========================================================================================//
    private void Awake()
    {
        inputSystem = this;
    }
    public void Start()
    {
        gameSystem = GameSystem.gameSystem;
        environmentSystem = EnvironmentSystem.environmentSystem;
        playerSystem = PlayerSystem.playerSystem;
        powerupSystem = PowerupSystem.powerupSystem;
        optionsSystem = OptionsSystem.optionsSystem;
        audioSystem = AudioSystem.audioSystem;
        commandSystem = CommandSystem.commandSystem;
        // grab the head object transform for look
        head = transform.GetChild(0);
        headBob = head.transform.GetChild(0);
        headStartPos = headBob.localPosition;
        bobVectors[0] = WeaponAnimation(weaponAnimType.up);
        // get the player input system from rewired
        inputPlayer = ReInput.players.GetPlayer(0);
        // grab character controller component
        controller = GetComponent<CharacterController>();
        // set the jump timer
        jumpTimer = antiJumpFactor;
        // set the gravity
        gravity = gravityPull;
        swimTimer = swimTime;
        InvokeRepeating("EstimatePlayerVelocity_InvokeRepeating", 1.0f, velocityPollPeriod);
    }
    private void Update()
    {
        if (gameSystem.BlockedAttributesActive()) return;

        time = Time.deltaTime * (commandSystem.masterCodesActive[3] ? 1.5f : (powerupSystem.powerEnabled[3] ? 1.25f : 1));
        bobVectors[0] = WeaponAnimation(weaponAnimType.up);
        if (overTilt)
        {
            head.GetChild(0).localRotation = Quaternion.RotateTowards(head.GetChild(0).localRotation, Quaternion.Euler(returnPosition), time * tiltReturnRate);
            if (head.GetChild(0).localRotation == Quaternion.Euler(returnPosition))
            {
                if (twoPoint) { returnPosition = twoPointPosition; twoPoint = false; tiltReturnRate = twoPointReturnRate; }
                else { returnPosition = Vector3.zero; overTilt = false; }
            }
        }

        if (playerSystem.isDead)
        {
            XYLook(optionsSystem.smoothRotation ? time * 50 : 1);
        }
        else
        {
            Look();
            Move();
            BobHeadMaster();
        }
        IsOffGround();
        ScreenShakeEffect();
    }

    public bool IsOffGround()
    {
        if (isJumping) return true;
        RaycastHit hit;
        Vector3 touchingFloor = transform.position + (-transform.up);
        Debug.DrawRay(touchingFloor, -transform.up * playerToFloorLength, Color.yellow);
        if (!Physics.Raycast(touchingFloor, -transform.up, out hit, controller.height / 2 * playerToFloorLength)) return true;  
        else return false;
    }
    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        isJumping = false;
        if (!fallDamage)
        {
            //if ( !isGrounded || isJumping)
            // play landing sound
        }
        if (hit.gameObject.CompareTag("Wood")) SetFootElement(FootElement.Wood);
        else if (hit.gameObject.CompareTag("Metal")) SetFootElement(FootElement.Metal);
        else
        {
            if (!swimGrounded)
                SetFootElement(FootElement.Normal);
        }
        contactPoint = hit.point;
    }
    private void OnTriggerExit(Collider other)
    {
        for (int e = 0; e < environmentSystem.environmentTag.Length; e++)
        {
            if (other.gameObject.CompareTag(environmentSystem.environmentTag[e]))
            {
                if(e == 2)
                {
                    if (moveDirection.y > 0)
                        moveDirection.y = 0;
                }
                environmentSystem.ActivateSwimming(false);
            }
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        for (int e = 0; e < environmentSystem.environmentTag.Length; e++)
        {
            playLedgeJumpSound = false;
            if (other.gameObject.CompareTag(environmentSystem.environmentTag[e]))
            {
                if (e != 0)
                {
                    int rng = Random.Range(0, playerFSWaterSounds.Length);
                    audioSystem.PlayAudioSource(playerFSWaterSounds[rng], Random.Range(0.6f, 0.8f), Random.Range(0.5f, 0.8f), 128);
                }
                isJumping = false;
                // Stop jitter on entering water
                moveDirection.x = 0;
                moveDirection.z = 0;
                moveDirection.y -= 75;
                swimDir = Vector3.zero;
                environmentSystem.ActivateSwimming(true);
            }
        }
    }
    //========================================================================================//
    //====================================[GAME FUNCTIONS]====================================//
    //========================================================================================//
    private void Move()
    {
      
        // player input of left stick or Arrow Keys
        inputX = inputPlayer.GetAxis("LSH");
        inputY = inputPlayer.GetAxis("LSV");

        // if no player input and angle limit true, slow down input factor [For player control when moving diagonal]
        float inputModifyFactor = (inputX > 0.5f && inputY > 0.5f && limitDiagonalSpeed) ? 0.8f : 1.0f;
        float curMoveSpeed = optionsSystem.alwaysRun ? runSpeed : moveSpeed;
        float curStrafeSpeed = optionsSystem.alwaysRun ? runStrafeSpeed : strafeSpeed;
        
      
        if (isSwimming)
        {
            // Shut off grounding/falling attributes
            if (isGrounded) isGrounded = false;
            if (isFalling) isFalling = false;
            // Delta Time

            if (startSwimTimer)
            {
                float clampVal = 0.25f;
               
                swimTimer -= time / divisionIndex;
                swimTimer = Mathf.Clamp(swimTimer, clampVal, swimTime);
                if (!startSwimSound)
                { 
                    // Start playing the swim sound
                    audioSystem.PlayAltAudioSource(2, playerSWSound[0], Random.Range(0.6f, 0.7f), 1f, false, true);
                    startSwimSound = true;
                }
                if (swimTimer == clampVal)
                {
                    divisionIndex = Random.Range(1, 4);
                    // reset the swim timer values
                    swimTimer = swimTime;
                    // shut off swim timer
                    startSwimTimer = false;
                    // reset the swim sound bool
                    if (startSwimSound) startSwimSound = false;
                    // player is not sinking above water
                    isSinking = false;
                }
               
            }
            //Debug.DrawRay(head.position + transform.forward * 2.5f, transform.forward * 10, Color.yellow);
            // when the players head is [NOT] underwater
            if (!environmentSystem.headUnderWater)
            {
                if (diveUnder) diveUnder = false;
                // Only look for ledge if player is close to wall
               
                Vector3 touchingWall = transform.position + transform.forward;
                touchingWall.y += 2;
                //LayerMask mask = LayerMask.GetMask("Level");
                Debug.DrawRay(touchingWall, transform.forward * 5, Color.white);
                if (Physics.Raycast(touchingWall, transform.forward, out playerHit, 5))
                {
                    // Set the position of the second raycast to the head
                    Vector3 aboveLedge = head.position + transform.forward * 2.5f;
                    // Add height adjustment a bit above the head
                    aboveLedge.y += 1.1f;
                    // Draw second ray in the editor from the head
                    
                    // Check if second raycast collides with a ledge or not
                    if (Physics.Raycast(aboveLedge, transform.forward, out wallHit, 10))
                    {
                        //Dont Jump up if something is there
                    }
                    else if (!Physics.Raycast(aboveLedge, transform.forward, out wallHit, 10))
                    {
                        // Time to jump up nothing is there
                        breathOut = false;
                        breathOutActive = false;
                        swimTimer = swimTime;
                        startSwimTimer = false;
                        if (startSwimSound) startSwimSound = false;
                        isSinking = false;
                        moveDirection.y = 55;
                        controller.Move(moveDirection * time);
                        if (!playLedgeJumpSound)
                        {
                            audioSystem.PlayAudioSource(playerSWSound[1], 1f, 1, 128);
                            playLedgeJumpSound = true;
                        }
                    }
                }
                // If player is now above water shut off timer attributes
                if (startSwimTimer)
                {
                    // Stop playing the swim sound
                    audioSystem.PlayAltAudioSource(2, playerSWSound[0], Random.Range(0.6f, 0.7f), 1f, false, false);
                    // reset the swim timer values
                    swimTimer = swimTime;
                    // shut off swim timer
                    startSwimTimer = false;
                    // reset the swim sound bool
                    if (startSwimSound) startSwimSound = false;
                    // player is not sinking above water
                    isSinking = false;
                }
                // Move speed in the water
                float waterSpeed = 55;
                // Move Horizontal
                moveDirection.x = inputX * waterSpeed * inputModifyFactor;
                // Move Applicate
                moveDirection.z = inputY * waterSpeed * inputModifyFactor;
                // When player is above water and [IS] grounded
                if(!breathOut)
                {
                    // Jump normally
                    if (!inputPlayer.GetButton("A") && !isSliding) jumpTimer++;
                    else if (inputPlayer.GetButton("A") && jumpTimer >= antiJumpFactor && swimGrounded)
                    {
                        isJumping = true;
                        moveDirection.y = jumpSpeed;
                        jumpTimer = 0;
                        audioSystem.PlayAudioSource(playerJSound, 1f, 1, 128);
                        // play splash sound when jumping in water
                        int rng = Random.Range(0, playerFSWaterSounds.Length);
                        audioSystem.PlayAudioSource(playerFSWaterSounds[rng], Random.Range(0.6f, 0.8f), Random.Range(0.5f, 0.8f), 128);
                    }
                   
                    // Apply Gravity
                    float appliedGravity = gravity * 1.3f * time;
                    moveDirection.y -= appliedGravity;
                }
                // When player is above water and [NOT] grounded
                else moveDirection.y = 0; 
                // Transform direction from local to world
                moveDirection = transform.TransformDirection(moveDirection);
                //Shut off above water swimming when grounded
                if (swimGrounded && breathOutActive)
                {
                    breathOut = false;
                    breathOutActive = false;
                }
                // when player directs camera into water start diving
                if (breathOutActive && lookRotation[1] < -21) breathOut = false;
                // when player direct camera away from water, stay above water
                else if (breathOutActive && lookRotation[1] > -20) breathOut = true;
                // Collision flags if player is touching bottom inside water but not underwater
                swimGrounded = (controller.Move(moveDirection * time * 0.5f) & CollisionFlags.Below) != 0;
            }
            // when the players head is [IS] underwater
            else
            {
                if (!diveUnder)
                {
                    audioSystem.PlayAudioSource(playerSWSound[4], Random.Range(0.7f, 1), 1, 128);
                    swimDir = new Vector3(0, swimDir.y, 0);
                    diveUnder = true;
                }
                // prepare player to stay above water when head exits trigger
                if (!breathOutActive) breathOutActive = true;
                breathOut = true;
                // Player is no longer in shallow water
                if (swimGrounded) swimGrounded = false;
                // Apply underwater moving speed
                float underWaterMoveSpeed = 55;
                // Apply turok type swimming, moving player based on timer from 1-0
                float swimSpeed = (underWaterMoveSpeed * swimTimer) * 1.5f;
                // rotation of the look axis
                float rotVal = lookRotation[1];
                // convert vertical axis to decimal value -0.75/0.75
                float vDir = (rotVal / 100);
                // convert forward/horizontal movement value opposite of rotation value 0.25
                float haDir = Mathf.Abs(AxisModule(rotVal) - vDir);
                // Pressing jump activates swim timer.
                if (inputPlayer.GetButton("A"))
                {
                    if (!startSwimTimer)
                    {
                        startSwimSound = false;
                        isSinking = false;
                        startSwimTimer = true;
                    }
                    swimDir.y = swimSpeed * swimTimer;
                }
                // shut off the swimming sound if not moving
                else if (!inputPlayer.GetButton("A"))
                    startSwimSound = true;
                // If player is pressing the move keys, move the player.
                if (AxisActive())
                {
                    // start the swim timer
                    if (!startSwimTimer)
                    {
                        startSwimSound = false;
                        startSwimTimer = true;
                    }
                    // if player isnt swimming upwards with Jump then move based on rot value
                    if (!inputPlayer.GetButton("A"))
                        swimDir.y = vDir * swimSpeed;
                    // move player based on forward/horizontal movement value
                    swimDir.x = inputX * haDir * swimSpeed;
                    swimDir.z = inputY * haDir * swimSpeed;
                    //player isnt sinking when swimming
                    isSinking = false;
                }
                else swimDir = new Vector3(0, swimDir.y, 0);
               
                // if player is doing nothing then activate sinking
                if (!isSinking && !AxisActive() && !inputPlayer.GetButton("A") && !startSwimTimer) 
                {
                    swimDir = new Vector3(0, swimDir.y, 0);
                    swimDir.y = -7; 
                    isSinking = true; 
                }
                // if player is sinking then apply water gravity
                else if(isSinking)
                {
                    swimDir.y -= waterGravity * time;
                    swimDir.y = Mathf.Clamp(swimDir.y, -7, underWaterMoveSpeed / 3);
                }
                // Transform direction from local to world
                swimDir = transform.TransformDirection(swimDir);
                // Move the character controller component based on delta time
                controller.Move(swimDir * time * 0.5f);
            }
        }
        // if player is [NOT] swimming
        else
        {
            // Make the player run
            if (!optionsSystem.alwaysRun)
            {
                if (inputPlayer.GetButton("Shift"))
                {

                    if (curMoveSpeed != runSpeed) curMoveSpeed = runSpeed;
                    if (curStrafeSpeed != runStrafeSpeed) curStrafeSpeed = runStrafeSpeed;
                    if (!isRunning) isRunning = true;
                }
                else
                {
                    if (curMoveSpeed != moveSpeed) curMoveSpeed = moveSpeed;
                    if (curStrafeSpeed != strafeSpeed) curStrafeSpeed = strafeSpeed;
                    if (isRunning) isRunning = false;
                }
            }
            else 
            {
                if (curMoveSpeed != runSpeed) curMoveSpeed = runSpeed;
                if (curStrafeSpeed != runStrafeSpeed) curStrafeSpeed = runStrafeSpeed;
                if (!isRunning) isRunning = true;
            }
            // when the player is grounded
            if (isGrounded)
            {
                
                // [PLAYER SLIDING] -----------------------------------------------------------------------------
                swimGrounded = false;
                isSliding = false;
                // when player transform collides with slide angle, sliding = true
                if (Physics.Raycast(transform.position, -Vector3.up, out controllerHit))
                {
                    if (Vector3.Angle(controllerHit.normal, Vector3.up) > slideAngle)
                        isSliding = true;
                }
                // when player collision contact point collides with slide angle, sliding = true
                else
                {
                    Physics.Raycast(contactPoint + Vector3.up, -Vector3.up, out controllerHit);
                    if (Vector3.Angle(controllerHit.normal, Vector3.up) > slideAngle)
                        isSliding = true;
                }
                // start sliding the player based on angle or tag in direction of the angle
                if ((isSliding && slideOnAngle) || (isSliding && slideOnTag))
                {
                    Vector3 hitNormal = controllerHit.normal;
                    moveDirection = new Vector3(hitNormal.x, -hitNormal.y, hitNormal.z);
                    Vector3.OrthoNormalize(ref hitNormal, ref moveDirection);
                    moveDirection *= slideSpeed;
                }
                // Start moving the player based on player input
                else
                {
                    moveDirection = new Vector3(inputX * curStrafeSpeed, -antiBumpFactor, inputY * curMoveSpeed);
                    moveDirection = transform.TransformDirection(moveDirection) * inputModifyFactor;
                }
                // [PLAYER FALLING] -------------------------------------------------------------------------------------
                fallDamage = false;
                if (isFalling)
                {
                    isFalling = false;
                    // set the player falling damage threshold when grounded
                    if (transform.position.y < fallStartLevel.y - fallingDamageThreshold)
                        FallingDamageAlert(fallStartLevel.y - transform.position.y);
                }
                // [PLAYER JUMPING] -------------------------------------------------------------------------------------
                if (!inputPlayer.GetButton("A") && !isSliding) jumpTimer++;
                else if (inputPlayer.GetButton("A") && jumpTimer >= antiJumpFactor)
                {
                    isJumping = true;
                    moveDirection.y = jumpSpeed;
                    jumpTimer = 0;
                    audioSystem.PlayAudioSource(playerJSound, 1, 1, 128);
                }
                if (!overTilt)
                {
                    if (inputX > 0.3f) tiltAngle = -3;
                    else if (inputX < -0.3f) tiltAngle = 3;
                    else tiltAngle = 0;
                    Vector3 headRot = new Vector3(0, 0, tiltAngle);
                    Quaternion rot = Quaternion.Euler(headRot);
                    // Tilt Camera angle to angle specified
                    if (inputX != 0)
                        head.GetChild(0).localRotation = Quaternion.RotateTowards(head.GetChild(0).localRotation, rot, time * (tiltSpeed * 10));
                    // Return Camera rotation back to 0 (Caution**  Might interfere with other Camera rotations)
                    else head.GetChild(0).localRotation = Quaternion.RotateTowards(head.GetChild(0).localRotation, Quaternion.identity, time * (tiltSpeed * 10));
                    // [PLAYER FOOSTEPS] -------------------------------------------------------------------------------------
                }
            }
            else
            {
                if (!isFalling)
                {
                    //start falling if not grounded
                    isFalling = true;
                    // reset the gravity 
                    gravity = gravityPull;
                    // set last grounded position
                    if (fallStartLevel != transform.position)
                        fallStartLevel = transform.position;
                }
                // Move the player in the air based on control
                if (optionsSystem.airControl)
                {
                    // Move horizontal
                    moveDirection.x = inputX * curStrafeSpeed * inputModifyFactor ;
                    // Move Forward/back
                    moveDirection.z = inputY * curMoveSpeed * inputModifyFactor;
                    // set current movement
                    moveDirection = transform.TransformDirection(moveDirection);
                }
            }
            // Always force the player downwards
            
            if (playerSystem.isDead || gameSystem.isLoading)
                return;
            moveDirection.y -= gravity * time * 2;

            // Set the isGrounded collision flags if player has landed 
            isGrounded = (controller.Move(moveDirection * time) & CollisionFlags.Below) != 0;
        }
        if (isGrounded && !idle() || swimGrounded && !idle())
        {
            if (Time.time - lastFootstepPlayedTime > currentFootstepsWaitingPeriod && !isJumping && !environmentSystem.headUnderWater)
            {
                if (swimGrounded && footElement != FootElement.Water)
                    footElement = FootElement.Water;
                else if (isGrounded && footElement != currentFootElement)
                    footElement = currentFootElement;
                FootStep(footElement);
                lastFootstepPlayedTime = Time.time;
            }
        }
        else currentFootstepsWaitingPeriod = Mathf.Infinity;
    }
    private int AxisModule(float axis)
    {
        if (axis > 0) return 1;
        else return -1;

    }
    private bool AxisActive()
    {
        if (inputX == 0 && inputY == 0) return false;
        else return true;
    }
    private bool idle()
    {
        if (inputSystem.inputX == 0 && inputSystem.inputY == 0) return true;
        else return false;
    }
    private void Look()
    {
        // activate rotation smoothing value
        float smoothing = optionsSystem.smoothRotation ? time * 50 : 1;
        // switch the look rotation
        switch (axis)
        {
            // XY rotation
            case RotationAxes.XY: XLook(smoothing); YLook(smoothing); break;
            // X rotation
            case RotationAxes.X: XLook(smoothing); break;
            // Y rootation
            case RotationAxes.Y: YLook(smoothing); break;
        }
    }
    private void XLook(float smoothing)
    { 
        
        // rotation input times smoothing & sensitivity
        lookRotation[0] = inputPlayer.GetAxisRaw("RSH") * smoothing * optionsSystem.sensitivity[0];
        // rotate only player transform
        transform.Rotate(0, lookRotation[0], 0);

    }

    private void YLook(float smoothing)
    {
        // rotation input times smoothing, sensitivity and inversion
        lookAxis = inputPlayer.GetAxisRaw("RSV") * smoothing * optionsSystem.sensitivity[1] * (optionsSystem.invertY ? -1 : 1);
        // Set look axis to rotation
        lookRotation[1] += lookAxis;
        // clamp rotation of the Y between 55/-55
        lookRotation[1] = Mathf.Clamp(lookRotation[1], -clampLookAngle, clampLookAngle);
        // rotate the head up or down
        head.transform.localEulerAngles = new Vector3(optionsSystem.lockYAxis ? 0 : -lookRotation[1], 0, 0);

    }
    private void XYLook(float smoothing)
    {
        // rotation input times smoothing, sensitivity and inversion
        float lookAxisY = inputPlayer.GetAxisRaw("RSV") * smoothing * optionsSystem.sensitivity[1] * (optionsSystem.invertY ? -1 : 1);
        float lookAxisX = inputPlayer.GetAxisRaw("RSH") * smoothing * optionsSystem.sensitivity[0] * 1;
        // Set look axis to rotation
        lookRotation[1] += lookAxisY;
        lookRotation[0] += lookAxisX;
        // clamp rotation of the Y between 55/-55
        lookRotation[1] = Mathf.Clamp(lookRotation[1], -clampLookAngle, clampLookAngle);
        // rotate the head up or down
        head.transform.localEulerAngles = new Vector3(optionsSystem.lockYAxis ? 0 : -lookRotation[1], lookRotation[0], 0);
    }
    private void FallingDamageAlert(float fallDistance)
    {
        fallDamage = true;
        if(fallDistance > 24 && fallDistance < 76)
        {
            audioSystem.PlayAudioSource(playerSWSound[3], Random.Range(0.6f, 1), 1, 128);
        }
        else if (fallDistance > 75 && fallDistance < 126)
        {
            if (powerupSystem.powerEnabled[5]) return;
            playerSystem.fallDamage = true;
            playerSystem.Damage(25);
            DamageAnimation();
        }
        else if (fallDistance > 125)
        {
            if (powerupSystem.powerEnabled[5]) return;
            playerSystem.fallDamage = true;
            playerSystem.Damage(999);
            DamageAnimation();
        }
    }
    public void DamageAnimation()
    {
        if (isShaking) return;
        SetVibration(0, 2, 0.2f);
        SetVibration(1, 2, 0.2f);
        RecoilEffect(0, 0, Random.Range(-5.5f, 5.5f), 120);
    }
    private void EstimatePlayerVelocity_InvokeRepeating()
    {

        float distanceMagnitude = (transform.position - lastStepPosition).magnitude;
        lastStepPosition = transform.position;
        float estimatedPlayerVelocity = distanceMagnitude / velocityPollPeriod;
        if (estimatedPlayerVelocity < 15.0f) { currentFootstepsWaitingPeriod = Mathf.Infinity; return; }
        float mappedPlayerSpeed = estimatedPlayerVelocity / 5.0f;
        //Convert the speed so that walking speed is about 6
        bool strafing = (inputX > 0f) ? true : false;

        float rcRate = isRunning ? (strafing ? 70 : 90) : (strafing ? 60 : stepSoundRatioC);
        stepsPerSecond = ((stepSoundRatioA * Mathf.Pow(mappedPlayerSpeed, stepSoundRatioN)) + (stepSoundRatioB * mappedPlayerSpeed) + rcRate) / 60.0f;
        timePerStep = (1.0f / stepsPerSecond);
        currentFootstepsWaitingPeriod = timePerStep;
    }
    private void BobHeadMaster()
    {
        if (!optionsSystem.cameraBobbing) return;
        if (environmentSystem.headUnderWater) return;
        if (!inputSystem.isJumping)
        {
            if (idle())
            {
                weaponAnimType type = weaponAnimType.idle;
                if (bobIndex != 0) { bobIndex = 0; switchDir = false; }
                headBob.localPosition = MoveTowards(WeaponAnimation(type), bobSpeed);
            }
            else
            {
                if (!isFalling)
                {
                    float inputModifyFactor = (inputX != 0.0f && inputY != 0.0f && limitDiagonalSpeed) ? 0.7071f : 1.0f;
                    float bobSpeedModule = environmentSystem.headUnderWater ? bobSpeed / 4 : bobSpeed;
                    Vector3 newBobValue = new Vector3(bobVectors[bobIndex].x, bobVectors[bobIndex].y, bobVectors[bobIndex].z);
                    headBob.localPosition = MoveTowards(newBobValue, bobSpeedModule * (Mathf.Abs(inputX / 2) + Mathf.Abs(inputY)) * inputModifyFactor);
                    if (headBob.localPosition == newBobValue)
                    {
                        bobIndex += switchDir ? -1 : +1;
                        if (bobIndex > 1 || bobIndex < 0) { bobIndex = 0; switchDir = !switchDir; }
                    }
                }
            }
        }
        else
        {
            float speed = isFalling ? bobSpeed / 1.5f : bobSpeed + 0.75f;
            weaponAnimType type = isFalling ? weaponAnimType.origin : weaponAnimType.jump;
            headBob.localPosition = MoveTowards(WeaponAnimation(type), speed);
        }
    }
    private Vector3 WeaponAnimation(weaponAnimType type)
    {
        float x = 0;
        float y = 0;
        float z = 0;
        Vector3 animVector;
        switch (type)
        {
            case weaponAnimType.up: x = headStartPos.x; y = headStartPos.y + upAmount; z = headStartPos.z; break;
            case weaponAnimType.left: x = headStartPos.x + leftRightAmount; y = headStartPos.y; z = headStartPos.z; break;
            case weaponAnimType.right: x = headStartPos.x + -leftRightAmount; y = headStartPos.y; z = headStartPos.z; break;
            case weaponAnimType.jump: x = headStartPos.x; y = headStartPos.y - jumpAmount; z = headStartPos.z; break;
            case weaponAnimType.idle:
                {
                    float newY = ((Mathf.Sin(Time.time * headbobSpeedY) + AboveOrBelowZeroY) / UpAndDownAmount);
                    x = 0; y = headStartPos.y + -newY; z = 0; break;
                }
            case weaponAnimType.origin: x = headStartPos.x; y = headStartPos.x; z = headStartPos.x; break;
        }
        animVector = new Vector3(x, y, z);
        return animVector;
    }
    private void FootStep(FootElement element)
    {
        if (idle()) return;
        switch (element)
        {
            case FootElement.Normal: { int rng = Random.Range(0, playerFSNormSounds.Length); audioSystem.PlayAudioSource(playerFSNormSounds[rng], Random.Range(0.6f, 0.8f), Random.Range(0.5f, 0.8f), 128); break; }
            case FootElement.Metal: { int rng = Random.Range(0, playerFSMetalSounds.Length); audioSystem.PlayAudioSource(playerFSMetalSounds[rng], Random.Range(0.6f, 0.8f), Random.Range(0.5f, 0.8f), 128); break; }
            case FootElement.Wood: { int rng = Random.Range(0, playerFSWoodSounds.Length); audioSystem.PlayAudioSource(playerFSWoodSounds[rng], Random.Range(0.6f, 0.8f), Random.Range(0.5f, 0.8f), 128); break; }
            case FootElement.Water: { int rng = Random.Range(0, playerFSWaterSounds.Length); audioSystem.PlayAudioSource(playerFSWaterSounds[rng], Random.Range(0.6f, 0.8f), Random.Range(0.5f, 0.8f), 128); break; }
        }
    }
    public void ResetInputSystem()
    {
        isGrounded = false;
        isJumping = true;
        isSliding = false;
        isFalling = false;
        isSwimming = false;
        isSinking = false;
        jumpTimer = antiJumpFactor;
        gravity = gravityPull;
        swimTimer = swimTime;
        for (int l = 0; l < lookRotation.Length; l++)
            lookRotation[l] = 0;
        moveDirection = Vector3.zero;
        transform.localRotation = Quaternion.identity;
        head.GetChild(0).localRotation = Quaternion.Euler(Vector3.zero);
        head.transform.localEulerAngles = Vector3.zero;
    }
    public void RecoilEffect(float angleX, float angleY, float angleZ, float returnRate)
    {
        Vector3 headRot = new Vector3(angleX, angleY, angleZ * 5);
        Quaternion rot = Quaternion.Euler(headRot);
        head.GetChild(0).localRotation = rot;
        tiltReturnRate = returnRate;
        overTilt = true;
    }
    private void RumbleEffect(float strength)
    {
        head.GetChild(0).localPosition = Vector3.zero + Random.insideUnitSphere * strength / 2;
    }
    private void ScreenShakeEffect()
    {
        if (!isShaking) return;
        RumbleEffect(shakeAmt);
        shakeTimer -= time;
        shakeTimer = Mathf.Clamp(shakeTimer, 0, shakeTime);
        if (shakeTimer == 0)
        {
            head.GetChild(0).localPosition = Vector3.zero;
            isShaking = false;
            shakeAmt = 0;
        }
    }
    public void SetScreenShakeEffect(float strength, float duration)
    {
        shakeAmt = strength;
        shakeTime = duration;
        shakeTimer = shakeTime;
        isShaking = true;
    }
    public void AnimationEffect(Vector3 one, float returnRate, float returnRate2)
    {
        onePointPosition = one;
        twoPointPosition = Vector3.zero;
        returnPosition = onePointPosition;
        tiltReturnRate = returnRate;
        twoPointReturnRate = returnRate2;
        twoPoint = true;
        overTilt = true;
    }
    //========================================================================================//
    //====================================[UTILITY FUNCTIONS]=================================//
    //========================================================================================//
    public Vector3 MoveTowards(Vector3 destPosition, float transitionSpeed)
    {
        Vector3 move = Vector3.MoveTowards(headBob.localPosition, destPosition, time * transitionSpeed);
        return move;
    }
    public void SetVibration(int motorIndex, float motorLevel, float duration)
    {
        if (!optionsSystem.vibration) return;
        inputPlayer.SetVibration(motorIndex, motorLevel, duration);
    }
    public void SetFootElement(FootElement element)
    {
        if (element != FootElement.Water)
            currentFootElement = element;
        footElement = element;
    }
}
