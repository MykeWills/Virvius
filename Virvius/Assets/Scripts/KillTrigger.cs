using UnityEngine;

public class KillTrigger : MonoBehaviour
{
    private PlayerSystem playerSystem;
    public enum Type { Kill, Damage, Mutilate }
    [SerializeField]
    private Type killType;
    [SerializeField]
    private int damageAmount = 0;
    [SerializeField]
    private SwitchSystem switchSystem;
    [SerializeField]
    private bool isElevator = false;
    [SerializeField]
    private bool isCollisionCrush = false;
    private Vector3 dir = Vector3.zero;
    private Vector3 playerPoint = Vector3.zero;
    private Vector3 boxPoint = Vector3.zero;
    private Vector3 playerBox = new Vector3(6, 13, 6);
    private Vector3 objectBox;
    private Collider curCollider;
    private bool checkCrush = false;
    public enum Direction { forward, back, left, right, down, up }
    [SerializeField]
    private Direction crushDirection;

    private void Start()
    {
        curCollider = GetComponent<Collider>();
        objectBox = new Vector3(curCollider.bounds.size.x, curCollider.bounds.size.y, curCollider.bounds.size.z);
        switch (crushDirection)
        {
            case Direction.forward: dir = Vector3.forward;break;
            case Direction.back: dir = Vector3.back; break;
            case Direction.left: dir = Vector3.left; break;
            case Direction.right: dir = Vector3.right; break;
            case Direction.down: dir = Vector3.down; break;
            case Direction.up: dir = Vector3.up; break;
        }
    }
    private void Update()
    {
       
        if (!checkCrush) return;
        boxPoint = new Vector3((transform.position.x + objectBox.x / 2) * dir.x, (transform.position.y + objectBox.y / 2) * dir.y, (transform.position.z + objectBox.z / 2) * dir.z);

        if (boxPoint.magnitude < playerPoint.magnitude)
        {
            if (playerSystem.isDead) return;
            switch (killType)
            {
                case Type.Damage: playerSystem.Damage(damageAmount); break;
                case Type.Kill: playerSystem.Damage(999); break;
                case Type.Mutilate: playerSystem.overKill = true; playerSystem.Damage(999); break;
            }
            boxPoint = Vector3.zero;
            playerPoint = Vector3.zero;
            checkCrush = false;
            if (isElevator)
            {
                if (switchSystem == null) return;
                //for when elevator hits player to reverse direction
                switchSystem.DeactivateSwitch(switchSystem.switchSubType);
            }
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (isCollisionCrush) return;
        if (other.gameObject.CompareTag("Player"))
        {
            if (playerSystem == null) playerSystem = other.gameObject.GetComponent<PlayerSystem>();
            if (playerSystem.isDead) return;
            switch (killType)
            {

                case Type.Damage: playerSystem.Damage(damageAmount); break;
                case Type.Kill: playerSystem.Damage(999); break;
                case Type.Mutilate: playerSystem.overKill = true; playerSystem.Damage(999); break;
            }
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (!isCollisionCrush) return;
        if (collision.gameObject.CompareTag("Player"))
        {
            if (TryGetComponent(out CraneDropSystem craneDropSystem))
            {
                if (craneDropSystem.craneActive)
                {
                    if (playerSystem.isDead) return;
                    switch (killType)
                    {
                        case Type.Damage: playerSystem.Damage(damageAmount); break;
                        case Type.Kill: playerSystem.Damage(999); break;
                        case Type.Mutilate: playerSystem.overKill = true; playerSystem.Damage(999); break;
                    }
                }
            }
            else
            {
                if (playerSystem == null) playerSystem = collision.gameObject.GetComponent<PlayerSystem>();
                switch (crushDirection)
                {
                    case Direction.forward: if (collision.GetContact(0).point.z > curCollider.bounds.center.z) break; else return;
                    case Direction.back: if (collision.GetContact(0).point.z < curCollider.bounds.center.z) break; else return;

                    case Direction.right: if (collision.GetContact(0).point.x > curCollider.bounds.center.x) break; else return;
                    case Direction.left: if (collision.GetContact(0).point.x < curCollider.bounds.center.x) break; else return;

                    case Direction.up: if (collision.GetContact(0).point.y > curCollider.bounds.center.y) break; else return;
                    case Direction.down: if (collision.GetContact(0).point.y < curCollider.bounds.center.y) break; else return;
                }

                playerPoint = new Vector3((playerSystem.transform.position.x + playerBox.x / 2) * dir.x, (playerSystem.transform.position.y + playerBox.y / 2) * dir.y, (playerSystem.transform.position.z + playerBox.z / 2) * dir.z);
                checkCrush = true;
            }
            
        }
    }
}

