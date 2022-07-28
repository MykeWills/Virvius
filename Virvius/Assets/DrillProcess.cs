using UnityEngine;

public class DrillProcess : MonoBehaviour
{
    [SerializeField] private Transform drill;
    [SerializeField] private Transform drillPartUpper;
    [SerializeField] private Transform drillPartLower;
    [SerializeField] private GameObject bigRock;
    [SerializeField] private GameObject[] rubble;
    [SerializeField] private ExplodingCrateSystem explodingCrateSystem;
    [SerializeField] private AudioClip pressFx;
    [SerializeField] private int maxCount = 5;
    [SerializeField] private float drillMoveSpeed = 20;
    [SerializeField] private float drillRotSpeed = 20;
    [SerializeField] private Light[] lights = new Light[4];
    private Color lightColor = new Color(1, 0.85f, 0.58f, 1);
    private MeshCollider drillPartCol;
    private Vector3 drillEndPosition = new Vector3(0, -35, 0);
    private bool isDrilling = false;
    private bool isRetracting = false;
    private int drillTimeCounter = 0;
    private float drillTimer = 0;
    private bool finishedMoving = false;
    private float time = 0;

    void Start()
    {
        
        for(int r = 0; r < rubble.Length; r++)
            rubble[r].SetActive(false);
        for (int l = 0; l < lights.Length; l++) lights[l].color = lightColor;
    }

    // Update is called once per frame
    void Update()
    {
        RotateDrill();
        Drill();
    }
    public void ActivateDrill()
    {
        AudioSystem.audioSystem.PlayAudioSource(pressFx, 1, 1, 128);
        if (drillPartCol == null) drillPartCol = drillPartLower.GetComponent<MeshCollider>();
        if (!drillPartCol.isTrigger) drillPartCol.isTrigger = true;
        for(int l = 0; l < lights.Length; l++) lights[l].color = Color.red;
        isDrilling = true;
    }
    private void Drill()
    {
        if (!isDrilling) return;
        time = Time.deltaTime;
        drillPartUpper.transform.Rotate(0, -drillRotSpeed / 2 * drillTimeCounter + 1, 0);
        drillPartLower.transform.Rotate(0, drillRotSpeed * 2 * drillTimeCounter + 1, 0);
        if (drillTimeCounter > maxCount)
        {
            drillTimeCounter = 0;
            finishedMoving = false;
            explodingCrateSystem.Explode();
        }

        if (!finishedMoving)
        {
            drill.localPosition = Vector3.MoveTowards(drill.localPosition, isRetracting ? Vector3.zero : drillEndPosition, time * drillMoveSpeed);
            if (drill.localPosition == Vector3.zero)
            {
                finishedMoving = true;
                isRetracting = !isRetracting;
                isDrilling = false;
                finishedMoving = false;
                for (int l = 0; l < lights.Length; l++) lights[l].color = lightColor;
                if (drillPartCol.isTrigger) drillPartCol.isTrigger = false;
            }
            else if (drill.localPosition == drillEndPosition)
            {
                finishedMoving = true;
                isRetracting = !isRetracting;
                drillTimer = 1;
                drillTimeCounter = 0;
                if (bigRock.activeInHierarchy)
                {
                    rubble[drillTimeCounter].SetActive(true);
                    rubble[drillTimeCounter].transform.localPosition = new Vector3(Random.Range(-16, 16), 10, Random.Range(-16, 16));
                }
            }
        }
        else
        {
            drillTimer -= Time.deltaTime;
            drillTimer = Mathf.Clamp01(drillTimer);
            if (drillTimer == 0)
            {
                drillTimer = 1;
                drillTimeCounter++;
                if (bigRock.activeInHierarchy)
                {
                    rubble[drillTimeCounter].SetActive(true);
                    rubble[drillTimeCounter].transform.localPosition = new Vector3(Random.Range(-16, 16), 10, Random.Range(-16, 16));
                }
            }
        }

    }
    private void RotateDrill()
    {
        drillPartUpper.transform.Rotate(0, -drillRotSpeed / 16, 0);
        drillPartLower.transform.Rotate(0, drillRotSpeed / 8, 0);
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Player") && !isDrilling)
        {
            if (isDrilling) return;
            ActivateDrill();
        }
    }
}
