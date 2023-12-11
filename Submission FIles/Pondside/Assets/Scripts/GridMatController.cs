using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridMatController : MonoBehaviour
{
    // Gives the grid markers the ability to fade in and out
    
    private Material material;

    private void Awake()
    {
        material = GetComponent<Renderer>().material;
    }

    public void OpacityLerp(float targetOpacity, float duration, Action onFinish)
    {
        StartCoroutine(OpacityLerpRoutine(targetOpacity, duration, onFinish));
    }

    IEnumerator OpacityLerpRoutine(float targetOpacity, float duration, Action onFinish)
    {
        float timePassed = 0f;
        float startingOpacity = material.GetFloat("_Opacity");
        while (timePassed < duration)
        {
            material.SetFloat("_Opacity", Mathf.Lerp(startingOpacity, targetOpacity, timePassed / duration));
            yield return null;
            timePassed += Time.deltaTime;
        }
        material.SetFloat("_Opacity", targetOpacity);
        onFinish();
    }
}
