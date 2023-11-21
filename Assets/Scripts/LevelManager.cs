using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.UI.Image;

public class LevelManager : MonoBehaviour
{
    public static LevelManager main;

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
        if (LevelManager.main == null)
        {
            main = this;
        }
        
        updateQueue = new Queue<LevelUpdate>();
    }

    public void LoadLevel(TextAsset levelJson, bool instant = false, Action onFinish = null, Action onDefined = null)
    {
        onFinish ??= () => { };
        onDefined ??= () => { };

        level = LevelData.Load(levelJson);
        onDefined();
        GenerateLevel();
        FallIn(instant, () =>
        {
            onFinish();
        });
    }

    public void UnloadLevel(bool instant = false, Action onFinish = null)
    {
        onFinish ??= () => { };

        if (level != null)
        {
            FallOut(instant, () =>
            {
                level = null;
                //updateQueue.Clear();
                foreach (Transform child in transform)
                {
                    Destroy(child.gameObject);
                }
                transform.DetachChildren();
                onFinish();
            });
        }
        else
        {
            onFinish();
        }
    }

    public void ResetLevel()
    {
        if (!IsUpdating())
        {
            AddUpdate(new LevelUpdate.Load(level.json, false));
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
                child.GetComponent<FallingMatController>().VisibleState();
            }
            onFinish();
        }
        else
        {
            int fallingCount = 0;
            foreach (Transform child in transform) // IN THEORY THIS CAN FAIL IF THE BLOCKS FALL TOO FAST
            {
                FallingMatController matController = child.GetComponent<FallingMatController>();
                if (matController != null)
                {
                    fallingCount++;
                    matController.FallIn(LevelToWorld(0, 0), () =>
                    {
                        fallingCount--;
                        if (fallingCount == 0)
                        {
                            onFinish();
                        }
                    });
                }
            }
        }
    }

    private void FallOut(bool instant, Action onFinish)
    {
        if (instant)
        {
            foreach (Transform child in transform)
            {
                child.GetComponent<FallingMatController>().HiddenState();
            }
            onFinish();
        }
        else
        {
            //float maxDelay = 0;
            //Vector3 origin = LevelToWorld(level.frog.pos);
            Vector3 origin = LevelToWorld(0, 0);
            /*
            foreach (Transform child in transform)
            {
                Vector3 distance2D = child.transform.position - origin;
                distance2D.y = 0;
                maxDelay = Mathf.Max(distance2D.magnitude * 0.1f, maxDelay);
            }
            */
            int fallingCount = 0;
            foreach (Transform child in transform) // IN THEORY THIS CAN FAIL IF THE BLOCKS FALL TOO FAST
            {
                fallingCount++;
                Vector3 distance2D = child.transform.position - origin;
                distance2D.y = 0;
                child.GetComponent<FallingMatController>().FallOut(distance2D.magnitude * 0.1f, () =>
                {
                    fallingCount--;
                    if (fallingCount == 0)
                    {
                        onFinish();
                    }
                });
            }
            //StartCoroutine(waitThenCall(GameManager.instance.settings.fallDuration + 2f, onFinish));
        }
    }

    public void AddUpdate(LevelUpdate update, bool directCall = true)
    {
        //Debug.Log("Adding to queue");
        bool startedEmpty = !IsUpdating();
        foreach (LevelUpdate preUpdate in update.preUpdates)
        {
            AddUpdate(preUpdate, false);
        }
        updateQueue.Enqueue(update);
        foreach (LevelUpdate postUpdate in update.postUpdates)
        {
            AddUpdate(postUpdate, false);
        }
        if (directCall && startedEmpty)
        {
            NextUpdate();
        }
    }
    private void NextUpdate()
    {
        updateQueue.Peek().execute(this);
    }
    public void UpdateFinished()
    {
        //Debug.Log("Removing from queue");
        updateQueue.Dequeue();
        if (updateQueue.Count > 0)
        {
            NextUpdate();
        }
    }
    public bool IsUpdating()
    {
        return updateQueue.Count > 0;
    }

    public void Move(Vector2Int dir)
    {
        if (!IsUpdating())
        {
            level.Move(dir);
        }
    }

    public Vector3 LevelToWorld(Vector2Int gridPos) // gives the center of this cell in world space
    {
        return transform.TransformPoint(new Vector3(gridPos.x - level.size.x / 2f + 0.5f, level.GetCell(gridPos).height * stepHeight, gridPos.y - level.size.y / 2f + 0.5f));
    }
    public Vector3 LevelToWorld(int x, int y)
    {
        return LevelToWorld(new Vector2Int(x, y));
    }
    public Vector3 LevelCenter()
    {
        return transform.position;
    }
}

