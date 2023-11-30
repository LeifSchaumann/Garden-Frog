using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class FrogJump : MonoBehaviour
{
    public float jumpSpeed;
    public AnimationCurve jumpCurve;

    public void Jump(Vector2Int targetGridPos, Action onFinish)
    {
        StartCoroutine(JumpRoutine(targetGridPos, onFinish));
    }

    private IEnumerator JumpRoutine(Vector2Int targetGridPos, Action onFinish)
    {
        Vector3 targetPos = LevelManager.main.LevelToWalkPos(targetGridPos);
        Vector3 startPos = transform.position;
        float jumpDistance = Vector3.Distance(targetPos, startPos);
        float jumpTime = jumpDistance / jumpSpeed;
        float timePassed = 0;

        while (timePassed < jumpTime)
        {
            transform.position = Vector3.Lerp(startPos, targetPos, timePassed / jumpTime) + Vector3.up * jumpCurve.Evaluate(timePassed / jumpTime) * jumpDistance;
            timePassed += Time.deltaTime;
            yield return null;
        }
        
        transform.position = targetPos;
        onFinish();
    }
}
