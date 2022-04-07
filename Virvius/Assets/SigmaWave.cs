using UnityEngine;

public class SigmaWave : MonoBehaviour
{
    private bool activateWave = false;
    private Renderer rend;
    [SerializeField]
    private float activeWaveTime = 2f;
    private float scaleAlphaRate = 0;
    void OnEnable()
    {
        if (rend == null) rend = GetComponent<Renderer>();
        rend.material.SetColor("_Color", Color.white);
        transform.localScale = Vector3.zero;
        scaleAlphaRate = 0;
        activateWave = true;
    }

    // Update is called once per frame
    void Update()
    {
        Wave();
    }
    private void Wave()
    {
        if (!activateWave) return;
        if (activeWaveTime == 0) return;
        float speedAbsolute = 1.0f / activeWaveTime; 
        float deltaVolume = Time.deltaTime * speedAbsolute; 
        scaleAlphaRate += deltaVolume;  // implement change

        scaleAlphaRate = Mathf.Clamp(scaleAlphaRate, 0.0f, 1.0f);
        rend.material.SetColor("_Color", Color.Lerp(Color.white, Color.clear, scaleAlphaRate * 2));
        transform.localScale = Vector3.Lerp(Vector3.zero, (Vector3.one * 15), scaleAlphaRate * 2);
        if (scaleAlphaRate == 1.0f) activateWave = false;
    }

}
