using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoleSystem : MonoBehaviour
{
    private bool holeActive = false;
    private float lifeTimer;
    private float heatTimer = 1f;
    private bool isFading = false;
    [HideInInspector]
    public bool isHot = false;
    private float fadePercentage = 1;
    private MeshRenderer meshRend;
    private Color hotHoleColor = new Color(1, 0.3f, 0, 1);
    private Color orgHoleColor = new Color(0.2f, 0.2f, 0.2f, 1);

    void Update()
    {
        KillTimer();
        HeatDissipate();
        FadeOut();
    }
    private void KillTimer()
    {
        if (!holeActive)
            return;
        lifeTimer -= Time.deltaTime;
        if (lifeTimer < 0) { lifeTimer = 0; holeActive = false; isFading = true; }
    }
    public void SetupHole(float lifeTime)
    {
        if(meshRend == null)
            meshRend = GetComponent<MeshRenderer>();
        meshRend.material.EnableKeyword("_EMISSION");
        meshRend.material.SetColor("_EmissionColor", hotHoleColor);
        Color fadeColor = new Color(meshRend.material.color.r, meshRend.material.color.g, meshRend.material.color.b, 1);
        meshRend.material.SetColor("_Color", fadeColor);
        lifeTimer = lifeTime;
        heatTimer = 1;
        holeActive = true;
        isHot = true;
    }
    public void HeatDissipate()
    {
        if (!holeActive && !isHot)
            return;
        heatTimer -= Time.deltaTime;
        float clamp = Mathf.Clamp(heatTimer, 0, 1);
        meshRend.material.SetColor("_EmissionColor", Color.Lerp(orgHoleColor, hotHoleColor * 3, clamp));
        if (clamp == 0)
            isHot = false;
    }
    private void FadeOut()
    {
        if (isFading == false) return;
        float speedAbsolute = 2.0f / 2;  // speed desired by user
        float speedDirection = speedAbsolute;  // + or -
        float deltaFade = Time.deltaTime * speedDirection;  // how much volume changes in 1 frame
        fadePercentage -= deltaFade;  // implement change
        fadePercentage = Mathf.Clamp(fadePercentage, 0.0f, 1.0f);  // make sure you're in 0..100% 
        Color fadeColor = new Color(meshRend.material.color.r, meshRend.material.color.g, meshRend.material.color.b, fadePercentage) ;
        meshRend.material.SetColor("_Color", fadeColor);
        if (fadePercentage == 0.0f) { isFading = false; gameObject.SetActive(false); }
    }
}
