using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplodeCrate : MonoBehaviour
{
    [SerializeField]
    private ExplodingCrateSystem crateSystem;
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
    private void OnCollisionEnter(Collision collision)
    {
        string tag = collision.gameObject.tag;
        for (int t = 0; t < tags.Length; t++)
        {
            if (tag == tags[t])
            {
                if(t == 4)
                {
                    if (collision.gameObject.TryGetComponent(out GrenadeSystem grenadeSystem))
                        grenadeSystem.Detonate();
                }
                crateSystem.Explode();

                break;
            }
        }
    }
}
