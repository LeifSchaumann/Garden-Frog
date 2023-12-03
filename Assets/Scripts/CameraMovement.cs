using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public LevelManager levelManager;

    private Camera cam;
    private Vector3 targetPos;
    private float targetSize;

    private void Awake()
    {
        cam = GetComponent<Camera>();
    }

    private void Start()
    {
        transform.rotation = Quaternion.LookRotation(GameManager.main.settings.viewDirection, Vector3.up);

    }

    public void SetZoom(bool instant = true, float zoomFactor = 1, Action onFinish = null)
    {
        onFinish ??= () => { };

        Vector3 focus = levelManager.LevelCenter();
        transform.position = focus - GameManager.main.settings.viewDirection.normalized * 30f;
        Vector2Int levelSize = levelManager.level.size;
        Vector3 leftCornerViewPos = cam.WorldToViewportPoint(levelManager.LevelToWorld(-1, -1));
        Vector3 rightCornerViewPos = cam.WorldToViewportPoint(levelManager.LevelToWorld(levelSize.x, -1));
        leftCornerViewPos = 2f * (leftCornerViewPos - 0.5f * Vector3.one);
        rightCornerViewPos = 2f * (rightCornerViewPos - 0.5f * Vector3.one);
        float zoomAdjustment = Mathf.Max(Mathf.Abs(leftCornerViewPos.x), Mathf.Abs(leftCornerViewPos.y), Mathf.Abs(rightCornerViewPos.x), Mathf.Abs(rightCornerViewPos.y));
        targetSize = cam.orthographicSize * zoomAdjustment / zoomFactor;
        if (instant)
        {
            cam.orthographicSize = targetSize;
            onFinish();
        }
        else
        {
            StartCoroutine(FocusRoutine(targetSize, onFinish));
        }
    }

    IEnumerator FocusRoutine(float targetSize, Action onFinish)
    {
        float startSize = cam.orthographicSize;
        float duration = 1f;
        float timePassed = 0;

        while (timePassed < duration)
        {
            cam.orthographicSize = Mathf.Lerp(startSize, targetSize, GameManager.main.settings.zoomCurve.Evaluate(timePassed / duration));
            timePassed += Time.deltaTime;
            yield return null;
        }

        cam.orthographicSize = targetSize;
        onFinish();
    }
}
