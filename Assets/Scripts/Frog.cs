using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Frog : MonoBehaviour
{
    public float jumpSpeed;

    public void Jump(Vector2Int targetGridPos, Action onFinish)
    {
        StartCoroutine(JumpRoutine(targetGridPos, onFinish));
    }

    private IEnumerator JumpRoutine(Vector2Int targetGridPos, Action onFinish)
    {
        Vector3 targetPos = LevelManager.instance.LevelToWorld(targetGridPos) + Vector3.up * transform.localScale.y / 2;
        Vector3 startPos = transform.position;
        float jumpTime = Vector3.Distance(targetPos, startPos) / jumpSpeed;
        float timePassed = 0;

        //Debug.Log("Starting jump which will take " + jumpTime);

        while (timePassed < jumpTime)
        {
            transform.position = Vector3.Lerp(startPos, targetPos, timePassed / jumpTime);
            timePassed += Time.deltaTime;
            yield return null;
        }
        
        transform.position = targetPos;
        onFinish();
    }
}
