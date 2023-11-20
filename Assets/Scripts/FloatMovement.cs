using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatMovement : MonoBehaviour
{
    public float floatSpeed;

    public void Float(Vector2Int targetGridPos, Transform carry, Action onFinish)
    {
        StartCoroutine(FloatRoutine(targetGridPos, carry, onFinish));
    }

    private IEnumerator FloatRoutine(Vector2Int targetGridPos, Transform carry, Action onFinish)
    {
        Vector3 targetPos = LevelManager.main.LevelToWorld(targetGridPos) + Vector3.up * transform.localScale.y / 2;
        Vector3 startPos = transform.position;
        float floatTime = Vector3.Distance(targetPos, startPos) / floatSpeed;
        float timePassed = 0;
        if (carry != null)
        {
            carry.parent = transform;
        }

        while (timePassed < floatTime)
        {
            transform.position = Vector3.Lerp(startPos, targetPos, timePassed / floatTime);
            timePassed += Time.deltaTime;
            yield return null;
        }

        transform.position = targetPos;
        if (carry != null)
        {
            carry.parent = transform.parent;
        }
        onFinish();
    }
}
