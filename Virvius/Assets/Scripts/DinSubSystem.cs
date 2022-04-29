using UnityEngine;

public class DinSubSystem : MonoBehaviour
{
    public DinSystem dinSystem;
    private void OnCollisionEnter(Collision collision)
    {
        dinSystem.CollisionDamage(collision.gameObject.tag, collision.gameObject);
    }
}
