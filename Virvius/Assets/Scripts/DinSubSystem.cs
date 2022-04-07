using UnityEngine;
using System.Text;

public class DinSubSystem : MonoBehaviour
{
    // Start is called before the first frame update
    private StringBuilder tagSb = new StringBuilder();
    public DinSystem dinSystem;
    private WeaponSystem weaponSystem;
    private PowerupSystem powerupSystem;
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
        if (tagSb.Length > 0) tagSb.Clear();
        tagSb = tagSb.Append(collision.gameObject.tag);
        if (weaponSystem == null) weaponSystem = WeaponSystem.weaponSystem;
        if (powerupSystem == null) powerupSystem = PowerupSystem.powerupSystem;
        for (int t = 0; t < tags.Length; t++)
        {
            if (tags[t] == tagSb.ToString())
            {
               
               
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
                    case 3: dmgAmt = Random.Range(2.75f, 3.51f); break;
                    //grenade
                    case 4: if (dinSystem.health <= (dinSystem.maxHealth / 4)) dinSystem.MutilateEnemy(); else dmgAmt = Random.Range(5f, 10f); break;
                    //rocket
                    case 5: if (dinSystem.health <= (dinSystem.maxHealth / 4)) dinSystem.MutilateEnemy(); else dmgAmt = Random.Range(10f, 15); break;
                    //railgun
                    case 6: dmgAmt = Random.Range(30.75f, 41.01f); break;
                    //photon
                    case 7: dmgAmt = Random.Range(3.60f, 5.1f); break;
                    //Sigma
                    case 8: dinSystem.MutilateEnemy(); break;
                    //Obstacle
                    case 9: dmgAmt = 2; break;
                    //MiniRocket
                    case 10: if (dinSystem.health <= (dinSystem.maxHealth / 4)) dinSystem.MutilateEnemy(); else dmgAmt = Random.Range(2.5f, 5); break;
                }
                float damage = powerupSystem.powerEnabled[2] ? 999 : powerupSystem.powerEnabled[0] ? Mathf.CeilToInt(dmgAmt) * 5 : dmgAmt;
                dinSystem.Damage(damage);
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
                        if (tags[t] != "RailBullet" && tags[t] != "SigmaBullet") collision.gameObject.SetActive(false);
                    }
                }
                return;
            }
        }
        return;

    }
}
