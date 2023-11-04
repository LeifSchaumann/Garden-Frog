using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialUpdate : MonoBehaviour
{
    private float animationTime;
    private Material material;
    private AnimationCurve fallingCurve;
    private AnimationCurve fallingCurveAlpha;
    private float fallingInitialHeight;
    private float fallingDuration;
    private Vector3 fallingOrigin;

    private void Start()
    {
        animationTime = 0;
        material = GetComponent<Renderer>().material;

        fallingCurve = GameManager.instance.settings.fallingCurve;
        fallingCurveAlpha = GameManager.instance.settings.fallingCurveAlpha;
        fallingInitialHeight = GameManager.instance.settings.fallingInitialHeight;
        fallingDuration = GameManager.instance.settings.fallingDuration;
        fallingOrigin = Vector3.zero;
    }

    void Update()
    {
        Vector3 distance2D = transform.position - fallingOrigin;
        distance2D.y = 0;
        float delay = distance2D.magnitude * 0.1f;
        animationTime += Time.deltaTime;
        material.SetFloat("_HeightOffset", fallingCurve.Evaluate((animationTime - delay) / fallingDuration) * fallingInitialHeight);
        material.SetFloat("_Alpha", fallingCurveAlpha.Evaluate((animationTime - delay) / fallingDuration));
    }
}
