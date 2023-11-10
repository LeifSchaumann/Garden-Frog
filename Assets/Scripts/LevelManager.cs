using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager instance;

    public GameObject frogPrefab;
    public GameObject waterPrefab;
    public GameObject lilyPadPrefab;
    public GameObject rockPrefab;
    public GameObject goalPrefab;
    public float stepHeight;
    
    public Level level;
    public Frog frog;
    public Queue<LevelUpdate> updateQueue;

    private void Awake()
    {
        instance = this;
        updateQueue = new Queue<LevelUpdate>();
    }

    public void LoadLevel(TextAsset levelJson, Action onFinish = null, Action onDefined = null)
    {
        onFinish ??= () => { };
        onDefined ??= () => { };
        level = LevelData.Load(levelJson);
        onDefined();
        GenerateLevel();
        FallIn(onFinish);
    }

    public void UnloadLevel(Action onFinish, bool instant = false) // INSTANT IS UNUSED
    {
        if (level != null)
        {
            if (instant)
            {
                level = null;
                updateQueue.Clear();
                foreach (Transform child in transform)
                {
                    Destroy(child.gameObject);
                }
                onFinish();
            }
            else
            {
                FallOut(() =>
                {
                    level = null;
                    updateQueue.Clear();
                    foreach (Transform child in transform)
                    {
                        Destroy(child.gameObject);
                    }
                    onFinish();
                });
            }
        }
        else
        {
            onFinish();
        }
    }

    private void GenerateLevel()
    {
        for (int x = 0; x < level.size.x; x++)
        {
            for (int y = 0; y < level.size.y; y++)
            {
                PuzzleObject PO0 = level.GetCell(x, y).PO0;
                if (PO0.type == ObjectType.water)
                {
                    Vector3 waterPos = LevelToWorld(x, y) + Vector3.down * waterPrefab.transform.localScale.y / 2;
                    Instantiate(waterPrefab, waterPos, Quaternion.identity, transform);
                }
                PuzzleObject PO1 = level.GetCell(x, y).PO1;
                switch (PO1.type)
                {
                    case ObjectType.lilyPad:
                        Vector3 lilyPos = LevelToWorld(x, y) + Vector3.up * lilyPadPrefab.transform.localScale.y / 2;
                        PO1.gameObject = Instantiate(lilyPadPrefab, lilyPos, Quaternion.identity, transform);
                        break;
                    case ObjectType.rock:
                        Vector3 rockPos = LevelToWorld(x, y) + Vector3.up * rockPrefab.transform.localScale.y / 2;
                        Instantiate(rockPrefab, rockPos, Quaternion.identity, transform);
                        break;
                }
                PuzzleObject PO2 = level.GetCell(x, y).PO2;
                switch (PO2.type)
                {
                    case ObjectType.goal:
                        Vector3 goalPos = LevelToWorld(x, y) + Vector3.up * 0.5f;
                        PO2.gameObject = Instantiate(goalPrefab, goalPos, Quaternion.identity, transform);
                        break;
                }
            }
        }
        frog = Instantiate(frogPrefab, Vector3.zero, Quaternion.identity, transform).GetComponent<Frog>();
        frog.transform.position = LevelToWorld(level.frogPos) + Vector3.up * frog.transform.localScale.y / 2;
    }

    private void FallIn(Action onFinish)
    {
        //Debug.Log("FallIn called");
        foreach (Transform child in transform)
        {
            child.GetComponent<MaterialController>().FallIn(LevelToWorld(0, 0));
        }
        StartCoroutine(waitThenCall(GameManager.instance.settings.fallDuration, onFinish));
    }

    private void FallOut(Action onFinish)
    {
        foreach (Transform child in transform)
        {
            child.GetComponent<MaterialController>().FallOut(LevelToWorld(level.frogPos));
        }
        StartCoroutine(waitThenCall(GameManager.instance.settings.fallDuration + 2f, onFinish));
    }

    IEnumerator waitThenCall(float t, Action onFinish)
    {
        yield return new WaitForSeconds(t);
        onFinish();
    }

    public void AddUpdate(LevelUpdate update)
    {
        updateQueue.Enqueue(update);
        //Debug.Log("Added to queue, now has length " + updateQueue.Count);
        if (updateQueue.Count == 1)
        {
            NextUpdate();
        }
        
    }

    public void NextUpdate()
    {
        if (updateQueue.Count > 0)
        {
            //Debug.Log("Executing queue, now has length " + updateQueue.Count);
            updateQueue.Peek().Execute();
        }
    }

    public void Move(Vector2Int dir)
    {
        if (updateQueue.Count == 0)
        {
            level.Move(dir);
        }
    }

    public Vector3 LevelToWorld(Vector2Int gridPos)
    {
        return transform.TransformPoint(new Vector3(gridPos.x - level.size.x/2, level.GetCell(gridPos).height * stepHeight, gridPos.y - level.size.y / 2));
    }
    public Vector3 LevelToWorld(int x, int y)
    {
        return LevelToWorld(new Vector2Int(x, y));
    }
    public Vector3 LevelToWorld(Vector3 pos)
    {
        return transform.TransformPoint(pos + Vector3.left * level.size.x + Vector3.back * level.size.y);
    }
}

