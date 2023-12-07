using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatMovement : MonoBehaviour
{
    public float floatSpeed;

    public void Float(Vector3 targetPos, Transform carry, Action onFinish)
    {
        StartCoroutine(FloatRoutine(targetPos, carry, onFinish));
    }

    public void Rotate(Vector3 pivotPos, float angle, Transform carry, Action onFinish)
    {
        StartCoroutine(RotateRoutine(pivotPos, angle, carry, onFinish));
    }

    private IEnumerator FloatRoutine(Vector3 targetPos, Transform carry, Action onFinish)
    {
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

    private IEnumerator RotateRoutine(Vector3 pivotPos, float angle, Transform carry, Action onFinish)
    {
        Quaternion endRotation = Quaternion.AngleAxis(angle, Vector3.up);
        float duration = 0.6f;
        float timePassed = 0;
        if (carry != null)
        {
            carry.parent = transform;
        }
        Transform pivot = new GameObject("Pivot").transform;
        pivot.transform.position = pivotPos;
        pivot.transform.rotation = Quaternion.identity;
        pivot.parent = transform.parent;
        transform.parent = pivot;
        while (timePassed < duration)
        {
            pivot.rotation = Quaternion.Lerp(Quaternion.identity, endRotation, timePassed / duration);
            timePassed += Time.deltaTime;
            yield return null;
        }
        pivot.rotation = endRotation;
        transform.parent = pivot.parent;
        Destroy(pivot.gameObject);
        if (carry != null)
        {
            carry.parent = transform.parent;
        }
        onFinish();
    }
}
