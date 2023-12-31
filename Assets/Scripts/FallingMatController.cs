using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingMatController : MonoBehaviour
{
    // Script to animate objects by setting the properties of their FallingObject material

    private Material material;
    private GameSettings settings;

    private void Awake()
    {
        material = GetComponent<Renderer>().material;
        settings = GameManager.main.settings;

        HiddenState();
    }

    public void FallIn(Vector3 origin, Action onFinish)
    {
        Vector3 distance2D = transform.position - origin;
        distance2D.y = 0;
        StartCoroutine(FallInRoutine(distance2D.magnitude * 0.1f, onFinish));
    }

    public void FallOut(float delay, Action onFinish)
    {
        StartCoroutine(FallOutRoutine(delay, onFinish));
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

    private IEnumerator FallInRoutine(float delay, Action onFinish)
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
        onFinish();
    }

    private IEnumerator FallOutRoutine(float delay, Action onFinish)
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
        onFinish();
    }
}
