using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIRotation : MonoBehaviour
{
    RectTransform reticle;
    public float reticleSpeed = 20f;
    void Start()
    {
        reticle = GetComponent<RectTransform>();
    }

    Vector3 rotationEuler;
    void Update()
    {
        rotationEuler += Vector3.forward * reticleSpeed * Time.unscaledDeltaTime;
        reticle.rotation = Quaternion.Euler(rotationEuler);
    }
}
