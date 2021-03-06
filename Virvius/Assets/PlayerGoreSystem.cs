using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGoreSystem : MonoBehaviour
{
    private AudioSystem audioSystem;
    [SerializeField]
    private GameObject explodeObj;
    private Rigidbody[] goreBodies;
    private float lifeTime = 5;
    public float explosionMultiplier = 1;
    [SerializeField]
    private AudioClip goreSoundFx;
    private float lifeTimer = 0;
    private bool lifeTimeActive = false;
    private float time = 0;
    private void Update()
    {
        time = Time.deltaTime;
        LifeTime();
    }
    public void ActivateGore(bool active)
    {
        if (audioSystem == null) audioSystem = AudioSystem.audioSystem;
        if (goreBodies == null)
        {
            goreBodies = new Rigidbody[explodeObj.transform.childCount];
            for (int g = 0; g < explodeObj.transform.childCount; g++)
                goreBodies[g] = explodeObj.transform.GetChild(g).GetComponent<Rigidbody>();
        }
        if (active)
        {
            explodeObj.SetActive(active);
            lifeTimer = lifeTime;
            for (int g = 0; g < goreBodies.Length; g++)
            {
                goreBodies[g].constraints = RigidbodyConstraints.None;
                Vector3 size = goreBodies[g].transform.localScale;
                float X = Random.Range(-3, 4);
                float Y = Random.Range(-3, 4);
                float Z = Random.Range(-5, -15);
                float explosionForce = Random.Range(5, 50);
                goreBodies[g].transform.localPosition = new Vector3(X, Y, Random.Range(0, -4));
                Vector3 dir = (goreBodies[g].transform.position - PlayerSystem.playerSystem.transform.position).normalized;
                //goreBodies[g].transform.localRotation = Quaternion.Euler(dir);
                goreBodies[g].transform.localRotation = Quaternion.identity;
                float magnitude = 0;
                if (size.magnitude > 1 && size.magnitude < 1.2f) magnitude = 0.1f;
                else if (size.magnitude >= 1.2) magnitude = 0.25f;
                else magnitude = 0.5f;
                goreBodies[g].AddRelativeForce(new Vector3(X, Y, Z) * (explosionForce * Random.Range(magnitude, magnitude * 1.5f)), ForceMode.Impulse);
                goreBodies[g].useGravity = true;
         
            }
            audioSystem.PlayAudioSource(goreSoundFx, 1, 1, 128);
            lifeTimeActive = true;
        }
        else
        {
            for (int g = 0; g < goreBodies.Length; g++)
            {
                goreBodies[g].useGravity = false;
                goreBodies[g].transform.localPosition = Vector3.zero;
                goreBodies[g].transform.localRotation = Quaternion.identity;
                goreBodies[g].velocity = Vector3.zero;
                goreBodies[g].angularVelocity = Vector3.zero;
            }
            lifeTimer = lifeTime;
            lifeTimeActive = false;
            explodeObj.SetActive(active);
        }
       

    }
    private void DisableGoreMovement()
    {
        for (int g = 0; g < goreBodies.Length; g++)
        {
            goreBodies[g].useGravity = false;
            goreBodies[g].velocity = Vector3.zero;
            goreBodies[g].angularVelocity = Vector3.zero;
            goreBodies[g].constraints = RigidbodyConstraints.FreezeAll;
        }
        lifeTimeActive = false;
    }
    private void LifeTime()
    {
        if (!lifeTimeActive) return;
        lifeTimer -= time;
        lifeTimer = Mathf.Clamp(lifeTimer, 0, lifeTime);
        if (lifeTimer == 0)
        {
            DisableGoreMovement();
        }
    }
}
