using UnityEngine;

public class KillBullet : MonoBehaviour
{
    private BulletSystem bulletSystem;
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("ShotgunBullet"))
        {
            bulletSystem = collision.gameObject.GetComponent<BulletSystem>();
            bulletSystem.EnableSpark();
            collision.gameObject.SetActive(false);
        }
        else if (collision.gameObject.CompareTag("SpikeBullet"))
        {
            bulletSystem = collision.gameObject.GetComponent<BulletSystem>();
            bulletSystem.EnableSpark();
            collision.gameObject.SetActive(false);
        }
    }

}
