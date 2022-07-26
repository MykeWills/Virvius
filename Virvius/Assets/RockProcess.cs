using UnityEngine;

public class RockProcess : MonoBehaviour
{
    [SerializeField] private GameObject rockExplode;
    [SerializeField] private GameObject goreExplode;
    private float rockRestoreTimer = 0;
    private bool startTimer = false;

    [SerializeField] private float rockRestoreTime = 3.5f;

    void Update()
    {
        RestoreRock();
    }
    private void RestoreRock()
    {
        if (!startTimer) return;
        rockRestoreTimer -= Time.deltaTime;
        rockRestoreTimer = Mathf.Clamp(rockRestoreTimer, 0, rockRestoreTime);
        if(rockRestoreTimer == 0)
        {
            rockExplode.SetActive(false);
            startTimer = false;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Rock"))
        {
            if(rockExplode.activeInHierarchy) rockExplode.SetActive(false);
            rockRestoreTimer = rockRestoreTime;
            startTimer = true;
            rockExplode.SetActive(true);

            other.gameObject.SetActive(false);
        }
        else if (other.gameObject.CompareTag("Body"))
        {
            if (goreExplode.activeInHierarchy) goreExplode.SetActive(false);
            rockRestoreTimer = rockRestoreTime;
            startTimer = true;
            goreExplode.SetActive(true);

            other.gameObject.SetActive(false);
        }
    }
}
