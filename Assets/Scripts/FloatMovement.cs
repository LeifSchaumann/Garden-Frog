using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatMovement : MonoBehaviour
{
    public float floatSpeed;

    public void Float(Vector2Int targetGridPos, Action onFinish)
    {
        StartCoroutine(FloatRoutine(targetGridPos, onFinish));
    }

    private IEnumerator FloatRoutine(Vector2Int targetGridPos, Action onFinish)
    {
        Vector3 targetPos = LevelManager.instance.LevelToWorld(targetGridPos) + Vector3.up * transform.localScale.y / 2;
        Vector3 startPos = transform.position;
        float floatTime = Vector3.Distance(targetPos, startPos) / floatSpeed;
        float timePassed = 0;
        LevelManager.instance.frog.transform.parent = transform;

        while (timePassed < floatTime)
        {
            transform.position = Vector3.Lerp(startPos, targetPos, timePassed / floatTime);
            timePassed += Time.deltaTime;
            yield return null;
        }

        transform.position = targetPos;
        LevelManager.instance.frog.transform.parent = transform.parent;
        onFinish();
    }
}
