using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialController : MonoBehaviour
{
    private Material material;
    private GameSettings settings;

    private void Awake()
    {
        material = GetComponent<Renderer>().material;
        settings = GameManager.instance.settings;

        HiddenState();
    }

    public void FallIn(Vector3 origin)
    {
        Vector3 distance2D = transform.position - origin;
        distance2D.y = 0;
        StartCoroutine(FallInRoutine(distance2D.magnitude * 0.1f));
    }

    public void FallOut(Vector3 origin)
    {
        Vector3 distance2D = transform.position - origin;
        distance2D.y = 0;
        StartCoroutine(FallOutRoutine(2f - distance2D.magnitude * 0.1f));
    }

    private void FallInState(float t)
    {
        material.SetFloat("_HeightOffset", settings.fallInMotion.Evaluate(t / settings.fallDuration) * settings.fallHeight);
        material.SetFloat("_Alpha", settings.fallInAlpha.Evaluate(t / settings.fallDuration));
    }

    public void VisibleState()
    {
        FallInState(settings.fallDuration);
    }

    public void HiddenState()
    {
        FallInState(0f);
    }

    private void FallOutState(float t)
    {
        material.SetFloat("_HeightOffset", settings.fallOutMotion.Evaluate(t / settings.fallDuration) * settings.fallHeight);
        material.SetFloat("_Alpha", settings.fallOutAlpha.Evaluate(t / settings.fallDuration));
    }

    private IEnumerator FallInRoutine(float delay)
    {
        yield return new WaitForSeconds(delay);
        float timePassed = 0;
        while (timePassed <= settings.fallDuration)
        {
            FallInState(timePassed);
            yield return null;
            timePassed += Time.deltaTime;
        }
        FallInState(settings.fallDuration);
    }

    private IEnumerator FallOutRoutine(float delay)
    {
        yield return new WaitForSeconds(delay);
        float timePassed = 0;
        while (timePassed <= settings.fallDuration)
        {
            FallOutState(timePassed);
            yield return null;
            timePassed += Time.deltaTime;
        }
        FallOutState(settings.fallDuration);
    }
}
