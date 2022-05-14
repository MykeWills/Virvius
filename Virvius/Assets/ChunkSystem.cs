using UnityEngine;

public class ChunkSystem : MonoBehaviour
{
    private PlayerSystem playerSystem;
    [SerializeField]
    private Rigidbody chunkRigidBody;
    [SerializeField]
    private Transform startPos;
    [SerializeField]
    private Transform mover;
    private float moveSpeed = 5f;
    private float chunkForce = 3000.0f;
    private float launchTime = 0;
    private float launchTimer;    
    private float killTime = 7.5f;
    private float killTimer;
    private bool kill = false;
    private float playerDistance = 0;
    private bool relaunch = false;
    private Vector3 newPosition;
    private Vector3 randomRot = Vector3.zero;
    private void Start()
    {
        playerSystem = PlayerSystem.playerSystem;
    }
    void Update()
    {
        playerDistance = Vector3.Magnitude(transform.position - PlayerPosition());
        if (playerDistance > 100 || playerSystem.isDead) { if (chunkRigidBody.gameObject.activeInHierarchy) { ChunkActivation(false); } return; }
        ChunkLauncher();
        ChunkRotation();
        ChunkMover();
        KillTimer();
    }
    public void ChunkActivation(bool active)
    {
        chunkRigidBody.gameObject.SetActive(active);
        if (active)
        {
            relaunch = false;
            killTimer = killTime;
            kill = true;
            chunkRigidBody.velocity = Vector3.zero;
            chunkRigidBody.AddForce(Vector3.up * chunkForce);
        }
        else
        {
            kill = false;
            float size = Random.Range(0.15f, 0.3f);
            chunkRigidBody.mass = Random.Range(1, 1.5f);
            chunkRigidBody.transform.localScale = new Vector3(size, size, size);
            moveSpeed = Random.Range(5f, 10f);
            chunkForce = Random.Range(1750f, 3000f);
            randomRot = new Vector3(Random.Range(-5, 5), Random.Range(-5, 5), Random.Range(-5, 5));
            launchTime = Random.Range(1, 5);
            launchTimer = launchTime;
            chunkRigidBody.transform.localPosition = startPos.localPosition;
            mover.transform.localPosition = Vector3.zero;
            newPosition = new Vector3(Random.Range(-15f, 15f), 0, Random.Range(-15f, 15f));
            relaunch = true;
        }
    }
    private void ChunkRotation()
    {
        if (!chunkRigidBody.gameObject.activeInHierarchy) return;
        chunkRigidBody.transform.Rotate(randomRot, Space.Self);
    }
    private void ChunkLauncher()
    {
        if (!relaunch) return;
        launchTimer -= Time.deltaTime;
        launchTimer = Mathf.Clamp(launchTimer, 0, launchTime);
        if (launchTimer == 0) ChunkActivation(true);
    }
    private void KillTimer()
    {
        if (!kill) return;
        killTimer -= Time.deltaTime;
        killTimer = Mathf.Clamp(killTimer, 0, killTime);
        if (killTimer == 0) ChunkActivation(false);
    }
    private void ChunkMover()
    {
        if (!chunkRigidBody.gameObject.activeInHierarchy) return;
        mover.transform.localPosition = Vector3.MoveTowards(mover.transform.localPosition, newPosition, Time.deltaTime * moveSpeed);
    }
    private Vector3 PlayerPosition()
    {
        if (playerSystem.isDead) return Vector3.zero;
        return playerSystem.transform.position;
    }
}
