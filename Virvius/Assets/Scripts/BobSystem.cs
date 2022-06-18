using UnityEngine;

public class BobSystem : MonoBehaviour
{
    private WeaponSystem weaponSystem;
    private InputSystem inputSystem;
    private PlayerSystem playerSystem;
    private EnvironmentSystem environmentSystem;
    private PowerupSystem powerupSystem;
    private AudioSystem audioSystem;
    public enum weaponAnimType { up, left, right, jump, idle, origin };

    [Header("Weapon Object")]
    public float weaponReloadRate = 5;
    private float weaponReloadTime = 5;
    public float weaponBlastBackRate = 6;
    public float weaponReturnRate = 1;
    private Vector3 weaponEndPosition;
    [Header("Animated Strict Settings")]
    public bool holdAnimation = false;
    public float holdTime = 0.5f;
    private bool holdAnimLoad = false;
    private float holdTimer;
    [Space]
    [Header("Animating Values")]
    public float animatedBlastBackRate = 3.5f;
    public float animatedReturnRate = 3.5f;
    public float animatedZStartPosition;
    public float animatedZEndPosition;
    private float[] animatedXPosition;
    private float[] animatedYPosition;
    public AudioClip[] weaponAnimatedSounds = new AudioClip[2];
    public Transform[] weaponAnimatedObj = new Transform[1];
    private float heightAmount = .5f;
    private float bobSpeed = 1.25f;
    [Space]
    [Header("ApplicateZ Amount")]
    public float backAmount = 0.5f;
    private float upAmount = 0.25f;
    private float leftRightAmount = 0.25f;
    [Space]
    private bool[] lerpAhead = new bool[2] { false, false };
    private bool[] lerpFinished = new bool[2] { false, false };
    private Vector3[] bobVectors = new Vector3[3];
    private float MovementSpeedY = 1;
    private float AboveOrBelowZeroY = 0;
    private float UpAndDownAmount = 15;
    private float time = 0;
    private int bobIndex = 0;
    private bool switchDir = false;
    private bool idle = true;
    private bool fall = false;
    [HideInInspector]
    public bool isRecoiling = false;
    public void Start()
    {
        inputSystem = InputSystem.inputSystem;
        playerSystem = PlayerSystem.playerSystem;
        environmentSystem = EnvironmentSystem.environmentSystem;
        powerupSystem = PowerupSystem.powerupSystem;
        audioSystem = AudioSystem.audioSystem;
        weaponSystem = WeaponSystem.weaponSystem;
        SetBobVectors();
        weaponReloadTime = weaponReloadRate;
        holdTimer = holdTime;
        if (holdAnimation) holdAnimLoad = true;
        animatedXPosition = new float[weaponAnimatedObj.Length];
        animatedYPosition = new float[weaponAnimatedObj.Length];
        for(int a = 0; a < weaponAnimatedObj.Length; a++)
        {
            animatedYPosition[a] = weaponAnimatedObj[a].transform.localPosition.y;
            animatedXPosition[a] = weaponAnimatedObj[a].transform.localPosition.x;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (playerSystem.isDead)
            return;
        time = Time.deltaTime * powerupSystem.BPowerMultiplier;
        AnimatePlayer();
        RecoilWeapon();
    }
    public void SetBobVectors()
    {
        bobVectors[0] = WeaponAnimation(weaponAnimType.right);
        bobVectors[1] = WeaponAnimation(weaponAnimType.up);
        bobVectors[2] = WeaponAnimation(weaponAnimType.left);
    }
    private void AnimatePlayer()
    {
        if (weaponSystem.switchingWeapon) return;
        if (!inputSystem.isJumping)
        {
            if (inputSystem.inputX == 0 && inputSystem.inputY == 0)
            {
                fall = false;
                weaponAnimType type = idle ? weaponAnimType.idle : weaponAnimType.origin;
                if (bobIndex != 0) { bobIndex = 0; switchDir = false; }
                transform.localPosition = MoveTowards(WeaponAnimation(type), bobSpeed);
                if (transform.localPosition == WeaponAnimation(weaponAnimType.origin) && !idle) idle = true;
            }
            else
            {
                if (!inputSystem.IsOffGround() && weaponSystem.isWeaponMovementFinished)
                {
                    float inputModifyFactor = (inputSystem.inputX != 0.0f && inputSystem.inputY != 0.0f && inputSystem.limitDiagonalSpeed) ? 0.7071f : 1.0f;
                    if (fall) fall = false; if (idle) idle = false;
                    float curBobSpeed = inputSystem.isRunning ? bobSpeed * 1.35f : bobSpeed;
                    float bobSpeedModule = inputSystem.isSwimming ? environmentSystem.headUnderWater ? curBobSpeed / 4f: curBobSpeed * 1.5f : curBobSpeed;
                    float X = HolsteredWeapon() ? weaponSystem.holsteredPos[weaponSystem.weaponIndex].x : bobVectors[bobIndex].x;
                    //float Y = HolsteredWeapon() ? weaponSystem.holsteredPos[weaponSystem.weaponIndex].y : bobVectors[bobIndex].y;
                    float Z = HolsteredWeapon() ? weaponSystem.holsteredPos[weaponSystem.weaponIndex].z : bobVectors[bobIndex].z;
                    Vector3 newBobValue = new Vector3(X, bobVectors[bobIndex].y, Z);
                    transform.localPosition = MoveTowards(newBobValue, bobSpeedModule * (Mathf.Abs(inputSystem.inputX / 2) + Mathf.Abs(inputSystem.inputY)) * inputModifyFactor);
                    if (transform.localPosition == newBobValue)
                    {
                        bobIndex += switchDir ? -1 : +1;
                        if (bobIndex > 2 || bobIndex < 0) { bobIndex = 1; switchDir = !switchDir; }
                    }
                }
            }
        }
        else
        {
            idle = false;
            float speed = fall ? bobSpeed / 1.5f : bobSpeed + 0.75f;
            weaponAnimType type = fall ? weaponAnimType.origin : weaponAnimType.jump;
            transform.localPosition = MoveTowards(WeaponAnimation(type), speed);
            if (transform.localPosition == WeaponAnimation(weaponAnimType.jump) && !fall) fall = true;
        }
    }
    public Vector3 WeaponAnimation(weaponAnimType type)
    {
        float x = 0;
        float y = 0;
        float z = 0;
        Vector3 animVector;
 
        float X = HolsteredWeapon() ? weaponSystem.holsteredPos[weaponSystem.weaponIndex].x : WeaponStartPosition().x;
        float Z = HolsteredWeapon() ? weaponSystem.holsteredPos[weaponSystem.weaponIndex].z : WeaponStartPosition().z;
        switch (type)
        {
            case weaponAnimType.up: x = WeaponStartPosition().x; y = WeaponStartPosition().y + upAmount; z = transform.localPosition.z; break;
            case weaponAnimType.left: x = WeaponStartPosition().x + -leftRightAmount; y = WeaponStartPosition().y; z = transform.localPosition.z; break;
            case weaponAnimType.right: x = WeaponStartPosition().x + leftRightAmount; y = WeaponStartPosition().y; z = transform.localPosition.z; break;
            case weaponAnimType.jump: x = WeaponStartPosition().x; y = WeaponStartPosition().y + heightAmount; z = transform.localPosition.z; break;
            case weaponAnimType.idle: 
                {
                    float newY = ((Mathf.Sin(Time.time * MovementSpeedY) + AboveOrBelowZeroY) / UpAndDownAmount);
                    x = X; y = WeaponStartPosition().y + newY; z = Z; break; 
                }
            case weaponAnimType.origin: x = X; y = WeaponStartPosition().y; z = Z; break;
        }
        animVector = new Vector3(x, y, z);
        return animVector;
    }
    public bool HolsteredWeapon()
    {
        if (weaponSystem == null) weaponSystem = WeaponSystem.weaponSystem;
        if (weaponSystem.ammoIsEmpty || weaponSystem.holsterActive) return true;
        else return false;
    }
    public Vector3 MoveTowards(Vector3 destPosition, float transitionSpeed)
    {
        Vector3 move = Vector3.MoveTowards(transform.localPosition, destPosition, time * transitionSpeed);
        return move;
    }    
    public Quaternion RotateTowards(Quaternion destRotation, float transitionSpeed)
    {
        Quaternion move = Quaternion.RotateTowards(transform.localRotation, destRotation, time * transitionSpeed);
        return move;
    }
    private Vector3 WeaponStartPosition()
    {
        float X = HolsteredWeapon() ? weaponSystem.holsteredPos[weaponSystem.weaponIndex].x : weaponSystem.holsteredReturnPos[weaponSystem.weaponIndex].x;
        float Y = HolsteredWeapon() ? weaponSystem.holsteredPos[weaponSystem.weaponIndex].y : weaponSystem.holsteredReturnPos[weaponSystem.weaponIndex].y;
        float Z = HolsteredWeapon() ? weaponSystem.holsteredPos[weaponSystem.weaponIndex].z : weaponSystem.holsteredReturnPos[weaponSystem.weaponIndex].z;
        Vector3 weaponStartPosition = new Vector3(X, Y, Z);
        return weaponStartPosition;
    }
    private void RecoilWeapon()
    {
        if (!isRecoiling) return;
        weaponReloadTime -= time;
        float clamp = Mathf.Clamp(weaponReloadTime, 0, weaponReloadRate);
        weaponEndPosition = new Vector3(WeaponStartPosition().x, WeaponStartPosition().y, WeaponStartPosition().z - backAmount);
        Vector3 lerpPosition = !lerpAhead[0] ? weaponEndPosition : WeaponStartPosition();
      
        float[] rate = new float[2]
        {
            !lerpAhead[0] ? weaponBlastBackRate : weaponReturnRate,
            !lerpAhead[1] ? animatedBlastBackRate : animatedReturnRate
        };
        switch (WeaponSystem.wType)
        {
            case WeaponSystem.WeaponType.Shotgun:
                {
                    if (!lerpFinished[0])
                    {
                        // Move the gun forward to end
                        transform.localPosition = Vector3.MoveTowards(transform.localPosition, lerpPosition, time * rate[0]);
                        if (transform.localPosition == weaponEndPosition && !lerpAhead[0])
                            lerpAhead[0] = true;
                        // Move the gun back to start
                        else if (transform.localPosition == WeaponStartPosition() && lerpAhead[0])
                        {
                            lerpAhead[0] = false;
                            lerpFinished[0] = true;
                        }
                    }
                    if (!lerpFinished[1] && lerpFinished[0])
                    {
                        if (holdAnimLoad && holdAnimation)
                        {
                            holdTimer -= time;
                            float clampHoldTimer = Mathf.Clamp(holdTimer, 0, holdTime);
                            if (clampHoldTimer == 0)
                            {
                                holdTimer = holdTime;
                                holdAnimLoad = false;
                            }
                        }
                        else if (!holdAnimLoad)
                        {
                            Vector3 start = new Vector3(animatedXPosition[0], animatedYPosition[0], animatedZStartPosition);
                            Vector3 end = new Vector3(animatedXPosition[0], animatedYPosition[0], animatedZEndPosition);
                            Vector3 lerpReload = !lerpAhead[1] ? end : start;
                            if (holdAnimLoad) return;
                            // Move the animated object forward to end
                            weaponAnimatedObj[0].localPosition = Vector3.MoveTowards(weaponAnimatedObj[0].localPosition, lerpReload, time * rate[1]);
                            if (weaponAnimatedObj[0].localPosition == end && !lerpAhead[1])
                            {
                                audioSystem.PlayAudioSource(environmentSystem.headUnderWater ? weaponAnimatedSounds[1] : weaponAnimatedSounds[0], 1, 1, 128);
    
                                lerpAhead[1] = true;
                            }
                            // Move the animated object back to start
                            else if (weaponAnimatedObj[0].localPosition == start && lerpAhead[1] && lerpFinished[0])
                            {
                                lerpAhead[1] = false;
                                lerpFinished[1] = true;
                            }
                        }
                    }
                    // when both are finished animating, ready to fire the gun
                    else if (lerpFinished[0] && lerpFinished[1] && clamp == 0)
                    {
                        isRecoiling = false;
                        holdAnimLoad = true;
                        weaponReloadTime = weaponReloadRate;
                        Vector3 start = new Vector3(animatedXPosition[0], animatedYPosition[0], animatedZStartPosition);
                        weaponAnimatedObj[0].localPosition = start;
                        for (int l = 0; l < lerpFinished.Length; l++)
                            lerpFinished[l] = false;
                    }
                    break;
                }
            case WeaponSystem.WeaponType.Spiker:
                {
                    if (!lerpFinished[0])
                    {
                        // Move the gun forward to end
                        transform.localPosition = Vector3.MoveTowards(transform.localPosition, lerpPosition, time * rate[0]);
                        if (transform.localPosition == weaponEndPosition && !lerpAhead[0])
                            lerpAhead[0] = true;
                        // Move the gun back to start
                        else if (transform.localPosition == WeaponStartPosition() && lerpAhead[0])
                        {
                            lerpAhead[0] = false;
                            lerpFinished[0] = true;
                        }
                    }
                    if (!lerpFinished[1])
                    {
                        Vector3 start = new Vector3(animatedXPosition[WeaponSystem.spikerBarrelIndex], animatedYPosition[WeaponSystem.spikerBarrelIndex], animatedZStartPosition);
                        Vector3 end = new Vector3(animatedXPosition[WeaponSystem.spikerBarrelIndex], animatedYPosition[WeaponSystem.spikerBarrelIndex], animatedZEndPosition);
                        Vector3 lerpReload = !lerpAhead[1] ? end : start;
                        // Move the animated object forward to end
                        weaponAnimatedObj[WeaponSystem.spikerBarrelIndex].localPosition = Vector3.MoveTowards(weaponAnimatedObj[WeaponSystem.spikerBarrelIndex].localPosition, lerpReload, time * rate[1]);
                        if (weaponAnimatedObj[WeaponSystem.spikerBarrelIndex].localPosition == end && !lerpAhead[1])
                            lerpAhead[1] = true;
                        // Move the animated object back to start
                        else if (weaponAnimatedObj[WeaponSystem.spikerBarrelIndex].localPosition == start && lerpAhead[1] && lerpFinished[0])
                        {
                            lerpAhead[1] = false;
                            lerpFinished[1] = true;
                        }
                    }
                    // when both are finished animating, ready to fire the gun
                    else if (lerpFinished[0] && lerpFinished[1] && clamp == 0)
                    {
                        isRecoiling = false;
                        weaponReloadTime = weaponReloadRate;
                        Vector3 start = new Vector3(animatedXPosition[WeaponSystem.spikerBarrelIndex], animatedYPosition[WeaponSystem.spikerBarrelIndex], animatedZStartPosition);
                        weaponAnimatedObj[WeaponSystem.spikerBarrelIndex].localPosition = start;
                        WeaponSystem.spikerBarrelIndex++;
                        if (WeaponSystem.spikerBarrelIndex > 2)
                            WeaponSystem.spikerBarrelIndex = 0;
                        for (int l = 0; l < lerpFinished.Length; l++)
                            lerpFinished[l] = false;
                    }
                    break;
                }
            case WeaponSystem.WeaponType.UltraShotgun:
                {
                    if (!lerpFinished[0])
                    {
                        // Move the gun forward to end
                        transform.localPosition = Vector3.MoveTowards(transform.localPosition, lerpPosition, time * rate[0]);

                        if (transform.localPosition == weaponEndPosition && !lerpAhead[0])
                            lerpAhead[0] = true;
                        // Move the gun back to start
                        else if (transform.localPosition == WeaponStartPosition() && lerpAhead[0])
                        {
                            lerpAhead[0] = false;
                            lerpFinished[0] = true;
                        }
                    }
                    else if (!lerpFinished[1] && lerpFinished[0])
                    {
                        if (holdAnimLoad && holdAnimation)
                        {
                            holdTimer -= time;
                            float clampHoldTimer = Mathf.Clamp(holdTimer, 0, holdTime);
                            if (clampHoldTimer == 0)
                            {

                                holdTimer = holdTime;
                                holdAnimLoad = false;
                            }
                        }
                        else if (!holdAnimLoad)
                        {
                            Vector3 start = new Vector3(animatedXPosition[0], animatedYPosition[0], animatedZStartPosition);
                            Vector3 end = new Vector3(animatedXPosition[0], animatedYPosition[0], animatedZEndPosition);
                            Vector3 lerpReload = !lerpAhead[1] ? end : start;
                            // Move the animated object forward to end
                            weaponAnimatedObj[0].localPosition = Vector3.MoveTowards(weaponAnimatedObj[0].localPosition, lerpReload, time * rate[1]);
                            if (weaponAnimatedObj[0].localPosition == end && !lerpAhead[1] && !holdAnimLoad)
                            {
                                audioSystem.PlayAudioSource(environmentSystem.headUnderWater ? weaponAnimatedSounds[2] : weaponAnimatedSounds[0], 1, 1, 128);
                                holdAnimLoad = true;
                                lerpAhead[1] = true;
                            }
                            // Move the animated object back to start
                            else if (weaponAnimatedObj[0].localPosition == start && lerpAhead[1] && lerpFinished[0])
                            {
                                audioSystem.PlayAudioSource(environmentSystem.headUnderWater ? weaponAnimatedSounds[3] : weaponAnimatedSounds[1], 1, 1, 128);
                                lerpAhead[1] = false;
                                lerpFinished[1] = true;
                            }

                        }
                    }
                    // when both are finished animating, ready to fire the gun
                    else if (lerpFinished[0] && lerpFinished[1] && clamp == 0)
                    {
                        isRecoiling = false;
                        weaponReloadTime = weaponReloadRate;
                        Vector3 start = new Vector3(animatedXPosition[0], animatedYPosition[0], animatedZStartPosition);
                        weaponAnimatedObj[0].localPosition = start;
                        for (int l = 0; l < lerpFinished.Length; l++)
                            lerpFinished[l] = false;
                    }
                    break;
                }
            case WeaponSystem.WeaponType.MiniGun:
                {
                    if (!lerpFinished[0])
                    {
                        // Move the gun forward to end
                        transform.localPosition = Vector3.MoveTowards(transform.localPosition, lerpPosition, time * rate[0]);
                        if (transform.localPosition == weaponEndPosition && !lerpAhead[0])
                            lerpAhead[0] = true;
                        // Move the gun back to start
                        else if (transform.localPosition == WeaponStartPosition() && lerpAhead[0])
                        {
                            lerpAhead[0] = false;
                            lerpFinished[0] = true;
                        }
                    }
                    // when finished animating, ready to fire the gun
                    else if (lerpFinished[0] && clamp == 0)
                    {
                        isRecoiling = false;
                        weaponReloadTime = weaponReloadRate;
                        for (int l = 0; l < lerpFinished.Length; l++)
                            lerpFinished[l] = false;
                    }
                    break;
                }
            case WeaponSystem.WeaponType.GrenadeLauncher:
                {
                    if (!lerpFinished[0])
                    {
                        // Move the gun forward to end
                        transform.localPosition = Vector3.MoveTowards(transform.localPosition, lerpPosition, time * rate[0]);
                        if (transform.localPosition == weaponEndPosition && !lerpAhead[0])
                            lerpAhead[0] = true;
                        // Move the gun back to start
                        else if (transform.localPosition == WeaponStartPosition() && lerpAhead[0])
                        {
                            lerpAhead[0] = false;
                            lerpFinished[0] = true;
                        }
                    }
                    // when finished animating, ready to fire the gun
                    else if (lerpFinished[0] && clamp == 0)
                    {
                        isRecoiling = false;
                        weaponReloadTime = weaponReloadRate;
                        for (int l = 0; l < lerpFinished.Length; l++)
                            lerpFinished[l] = false;
                    }
                    break;
                }
            case WeaponSystem.WeaponType.RocketLauncher:
                {
                    if (!lerpFinished[0])
                    {
                        // Move the gun forward to end
                        transform.localPosition = Vector3.MoveTowards(transform.localPosition, lerpPosition, time * rate[0]);
                        if (transform.localPosition == weaponEndPosition && !lerpAhead[0])
                            lerpAhead[0] = true;
                        // Move the gun back to start
                        else if (transform.localPosition == WeaponStartPosition() && lerpAhead[0])
                        {
                            lerpAhead[0] = false;
                            lerpFinished[0] = true;
                        }
                    }
                    // when finished animating, ready to fire the gun
                    else if (lerpFinished[0] && clamp == 0)
                    {
                        isRecoiling = false;
                        weaponReloadTime = weaponReloadRate;
                        for (int l = 0; l < lerpFinished.Length; l++)
                            lerpFinished[l] = false;
                    }
                    break;
                }
            case WeaponSystem.WeaponType.RailGun:
                {
                    if (!lerpFinished[0])
                    {
                        // Move the gun forward to end
                        transform.localPosition = Vector3.MoveTowards(transform.localPosition, lerpPosition, time * rate[0]);
                        if (transform.localPosition == weaponEndPosition && !lerpAhead[0])
                            lerpAhead[0] = true;
                        // Move the gun back to start
                        else if (transform.localPosition == WeaponStartPosition() && lerpAhead[0])
                        {
                            lerpAhead[0] = false;
                            lerpFinished[0] = true;
                        }
                    }
                    // when finished animating, ready to fire the gun
                    else if (lerpFinished[0] && clamp == 0)
                    {
                        isRecoiling = false;
                        weaponReloadTime = weaponReloadRate;
                        for (int l = 0; l < lerpFinished.Length; l++)
                            lerpFinished[l] = false;
                    }
                    break;
                }
            case WeaponSystem.WeaponType.PhotonCannon:
                {
                    if (!lerpFinished[0])
                    {
                        // Move the gun forward to end
                        transform.localPosition = Vector3.MoveTowards(transform.localPosition, lerpPosition, time * rate[0]);
                        if (transform.localPosition == weaponEndPosition && !lerpAhead[0])
                            lerpAhead[0] = true;
                        // Move the gun back to start
                        else if (transform.localPosition == WeaponStartPosition() && lerpAhead[0])
                        {
                            lerpAhead[0] = false;
                            lerpFinished[0] = true;
                        }
                    }
                    // when finished animating, ready to fire the gun
                    else if (lerpFinished[0] && clamp == 0)
                    {
                        isRecoiling = false;
                        weaponReloadTime = weaponReloadRate;
                        for (int l = 0; l < lerpFinished.Length; l++)
                            lerpFinished[l] = false;
                    }
                    break;
                }
            case WeaponSystem.WeaponType.MSigma:
                {
                    if (!lerpFinished[0])
                    {
                        // Move the gun forward to end
                        transform.localPosition = Vector3.MoveTowards(transform.localPosition, lerpPosition, time * rate[0]);
                        if (transform.localPosition == weaponEndPosition && !lerpAhead[0])
                            lerpAhead[0] = true;
                        // Move the gun back to start
                        else if (transform.localPosition == WeaponStartPosition() && lerpAhead[0])
                        {
                            lerpAhead[0] = false;
                            lerpFinished[0] = true;
                        }
                    }
                    // when finished animating, ready to fire the gun
                    else if (lerpFinished[0] && clamp == 0)
                    {
                        isRecoiling = false;
                        weaponReloadTime = weaponReloadRate;
                        for (int l = 0; l < lerpFinished.Length; l++)
                            lerpFinished[l] = false;
                    }
                    break;
                }
        }
    }
    public void resetBobSystem(WeaponSystem.WeaponType type)
    {
        isRecoiling = false;
        weaponReloadTime = weaponReloadRate;
        switch (WeaponSystem.wType)
        {
            case WeaponSystem.WeaponType.Shotgun:
                {
                   
                    Vector3 start = new Vector3(animatedXPosition[0], animatedYPosition[0], animatedZStartPosition);
                    weaponAnimatedObj[0].localPosition = start;
                    break;
                }
            case WeaponSystem.WeaponType.Spiker:
                {
                    Vector3 start = new Vector3(animatedXPosition[WeaponSystem.spikerBarrelIndex], animatedYPosition[WeaponSystem.spikerBarrelIndex], animatedZStartPosition);
                    weaponAnimatedObj[WeaponSystem.spikerBarrelIndex].localPosition = start;
                    WeaponSystem.spikerBarrelIndex++;
                    if (WeaponSystem.spikerBarrelIndex > 2)
                        WeaponSystem.spikerBarrelIndex = 0;
                    break;
                }
            case WeaponSystem.WeaponType.UltraShotgun:
                {
                    Vector3 start = new Vector3(animatedXPosition[0], animatedYPosition[0], animatedZStartPosition);
                    weaponAnimatedObj[0].localPosition = start;
                    break;
                }
            case WeaponSystem.WeaponType.MiniGun:
                {
                    break;
                }
            case WeaponSystem.WeaponType.GrenadeLauncher:
                {
                    break;
                }  
            case WeaponSystem.WeaponType.RocketLauncher:
                {
                    break;
                }
            case WeaponSystem.WeaponType.RailGun:
                {
                    break;
                }
            case WeaponSystem.WeaponType.PhotonCannon:
                {
                    break;
                } 
            case WeaponSystem.WeaponType.MSigma:
                {
                    break;
                }
        }
        for (int l = 0; l < lerpFinished.Length; l++)
            lerpFinished[l] = false;
    }
}
