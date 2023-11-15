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
    public FrogJump frog;
    public Queue<LevelUpdate> updateQueue;

    private void Awake()
    {
        instance = this;
        updateQueue = new Queue<LevelUpdate>();
    }

    public void LoadLevel(TextAsset levelJson, Action onFinish = null, Action onDefined = null, bool instant = false)
    {
        onFinish ??= () => { };
        onDefined ??= () => { };
        level = LevelData.Load(levelJson);
        onDefined();
        GenerateLevel();
        FallIn(instant, onFinish);
    }

    public void UnloadLevel(bool instant = false, Action onFinish = null) // INSTANT IS UNUSED
    {
        onFinish ??= () => { };
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
                FallOut(false, () =>
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
                if (PO0 is PuzzleObject.L0.Water)
                {
                    Vector3 waterPos = LevelToWorld(x, y) + Vector3.down * waterPrefab.transform.localScale.y / 2;
                    Instantiate(waterPrefab, waterPos, Quaternion.identity, transform);
                }
                PuzzleObject PO1 = level.GetCell(x, y).PO1;
                switch (PO1)
                {
                    case PuzzleObject.L1.LilyPad:
                        Vector3 lilyPos = LevelToWorld(x, y) + Vector3.up * lilyPadPrefab.transform.localScale.y / 2;
                        PO1.gameObject = Instantiate(lilyPadPrefab, lilyPos, Quaternion.identity, transform);
                        break;
                    case PuzzleObject.L1.Rock:
                        Vector3 rockPos = LevelToWorld(x, y) + Vector3.up * rockPrefab.transform.localScale.y / 2;
                        Instantiate(rockPrefab, rockPos, Quaternion.identity, transform);
                        break;
                }
                PuzzleObject PO2 = level.GetCell(x, y).PO2;
                switch (PO2)
                {
                    case PuzzleObject.L2.Frog:
                        Vector3 frogPos = LevelToWorld(x, y) + Vector3.up * frogPrefab.transform.localScale.y / 2;
                        PO2.gameObject = Instantiate(frogPrefab, frogPos, Quaternion.identity, transform);
                        break;
                }
                PuzzleObject PO3 = level.GetCell(x, y).PO3;
                switch (PO3)
                {
                    case PuzzleObject.L3.Goal:
                        Vector3 goalPos = LevelToWorld(x, y) + Vector3.up * 0.5f;
                        PO2.gameObject = Instantiate(goalPrefab, goalPos, Quaternion.identity, transform);
                        break;
                }
            }
        }
    }

    private void FallIn(bool instant, Action onFinish)
    {
        if (instant)
        {
            foreach (Transform child in transform)
            {
                child.GetComponent<MaterialController>().VisibleState();
            }
            onFinish();
        }
        else
        {
            foreach (Transform child in transform)
            {
                child.GetComponent<MaterialController>().FallIn(LevelToWorld(0, 0));
            }
            StartCoroutine(waitThenCall(GameManager.instance.settings.fallDuration, onFinish));
        }
    }

    private void FallOut(bool instant, Action onFinish)
    {
        if (instant)
        {
            foreach (Transform child in transform)
            {
                child.GetComponent<MaterialController>().HiddenState();
            }
            onFinish();
        }
        else
        {
            foreach (Transform child in transform)
            {
                child.GetComponent<MaterialController>().FallOut(LevelToWorld(level.frog.pos));
            }
            StartCoroutine(waitThenCall(GameManager.instance.settings.fallDuration + 2f, onFinish));
        }
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
            updateQueue.Peek().execute(UpdateFinished);
        }
        
    }

    public void UpdateFinished()
    {
        updateQueue.Dequeue();
        if (updateQueue.Count > 0)
        {
            //Debug.Log("Executing queue, now has length " + updateQueue.Count);
            updateQueue.Peek().execute(UpdateFinished);
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

