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
        float turnTime = 0.15f;
        Quaternion targetRotation = Quaternion.LookRotation(targetPos - startPos);
        Quaternion startRotation = transform.rotation;
        float timePassed = 0;

        while (timePassed < jumpTime)
        {
            if (timePassed < turnTime)
            {
                transform.rotation = Quaternion.Lerp(startRotation, targetRotation, timePassed / turnTime);
            }
            transform.position = Vector3.Lerp(startPos, targetPos, timePassed / jumpTime) + Vector3.up * jumpCurve.Evaluate(timePassed / jumpTime) * jumpDistance;
            timePassed += Time.deltaTime;
            yield return null;
        }
        
        transform.rotation = targetRotation;
        transform.position = targetPos;
        onFinish();
    }
}
