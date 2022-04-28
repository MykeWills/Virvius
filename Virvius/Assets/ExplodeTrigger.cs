using UnityEngine;

public class ExplodeTrigger : MonoBehaviour
{
    [SerializeField]
    private float triggerTime = 0.25f;
    private float triggerTimer = 0f;
    private bool active = false;
    private int triggerInterval = 0;
    private bool isTriggered = false;
    [SerializeField]
    private ExplodingCrateSystem[] crateSystems;
    [SerializeField]
    private GameObject activateObject;

    void Update()
    {
        TriggerMultiple();
    }
    private void TriggerMultiple()
    {
        if (!active) return;
        triggerTimer -= Time.deltaTime;
        triggerTimer = Mathf.Clamp(triggerTimer, 0, triggerTime);
        if (triggerTimer == 0)
        {
            crateSystems[triggerInterval].Explode();
            if (triggerInterval == 0) 
            { 
                if(activateObject != null) 
                    activateObject.SetActive(true); 
                active = false; 
                return; 
            }
            triggerInterval--;
            triggerTimer = triggerTime;
        }
    }
    private void TriggerExplosions()
    {
        isTriggered = true;
        triggerInterval = crateSystems.Length -1;
        triggerTimer = triggerTime;
        active = true;

    }
    private void OnTriggerEnter(Collider other)
    {
        if (isTriggered) return;
        if (other.gameObject.CompareTag("Player")) TriggerExplosions();
    }
    public void ResetObject()
    {
        triggerInterval = 0;
        active = false;
        isTriggered = false;
        triggerTimer = triggerTime;
        for (int t = 0; t < crateSystems.Length; t++) crateSystems[t].ResetObject();
    }
}
