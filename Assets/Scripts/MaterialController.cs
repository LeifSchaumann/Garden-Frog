using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialController : MonoBehaviour
{
    private Material material;
    private FallingSettings settings;

    private void Start()
    {
        material = GetComponent<Renderer>().material;
        settings = GameManager.instance.fallingSettings;

        SetFallState(0f);
    }

    public void FallIn(Vector3 origin)
    {
        Vector3 distance2D = transform.position - origin;
        distance2D.y = 0;
        StartCoroutine(FallInRoutine(distance2D.magnitude * 0.1f));
    }

    private void SetFallState(float t)
    {
        material.SetFloat("_HeightOffset", settings.motionCurve.Evaluate(t / settings.duration) * settings.initialHeight);
        material.SetFloat("_Alpha", settings.alphaCurve.Evaluate(t / settings.duration));
    }

    private IEnumerator FallInRoutine(float delay)
    {
        yield return new WaitForSeconds(delay);
        float timePassed = 0;
        while (timePassed <= settings.duration)
        {
            SetFallState(timePassed);
            yield return null;
            timePassed += Time.deltaTime;
        }
        
    }
}
