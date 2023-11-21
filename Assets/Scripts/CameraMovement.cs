using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public Vector3 viewDirection;
    public AnimationCurve zoomCurve;

    private Camera cam;
    private Vector3 targetPos;
    private float targetSize;

    private void Awake()
    {
        cam = GetComponent<Camera>();

        transform.rotation = Quaternion.LookRotation(viewDirection, Vector3.up);
    }

    public void FocusOn(Vector3 focus, bool instant = true, float zoomFactor = 1, Action onFinish = null)
    {
        onFinish ??= () => { };

        Vector2Int levelSize = LevelManager.main.level.size;
        targetSize = Mathf.Max(levelSize.y * 0.55f, levelSize.x * 9 / 16 * 0.7f) / zoomFactor;
        targetPos = focus - viewDirection.normalized * 30f;
        if (instant)
        {
            transform.position = targetPos;
            cam.orthographicSize = targetSize;
            onFinish();
        }
        else
        {
            StartCoroutine(FocusRoutine(targetPos, targetSize, onFinish));
        }
    }

    IEnumerator FocusRoutine(Vector3 targetPos, float targetSize, Action onFinish)
    {
        float startSize = cam.orthographicSize;
        Vector3 startPos = transform.position;
        float duration = 2f;
        float timePassed = 0;

        while (timePassed < duration)
        {
            cam.orthographicSize = Mathf.Lerp(startSize, targetSize, zoomCurve.Evaluate(timePassed / duration));
            transform.position = Vector3.Lerp(startPos, targetPos, timePassed / duration);
            timePassed += Time.deltaTime;
            yield return null;
        }

        cam.orthographicSize = targetSize;
        transform.position = targetPos;
        onFinish();
    }
}
