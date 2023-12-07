using System;
using System.Collections.Generic;
using UnityEngine;
using static PuzzleObject.L1;

public class LevelManager : MonoBehaviour
{
    public static LevelManager main;
    
    public bool isMain;
    public Level level;
    public Queue<LevelUpdate> updateQueue;

    [System.NonSerialized]
    public bool levelIsLoaded;

    private void Awake()
    {
        if (LevelManager.main == null && isMain)
        {
            main = this;
        }
        
        updateQueue = new Queue<LevelUpdate>();
        levelIsLoaded = false;
    }

    public void LoadLevel(LevelData levelData, bool instant = false, Action onFinish = null, Action onDefined = null)
    {
        onFinish ??= () => { };
        onDefined ??= () => { };

        levelIsLoaded = false;
        level = levelData.MakeLevel();
        GenerateLevel();
        onDefined();
        FallIn(instant, () =>
        {
            levelIsLoaded = true;
            onFinish();
        });
    }

    public void UnloadLevel(bool instant = false, Action onFinish = null)
    {
        onFinish ??= () => { };

        levelIsLoaded = false;
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

    public bool ResetLevel()
    {
        if (!IsUpdating())
        {
            AddUpdate(new LevelUpdate.Load(level.data, false));
            return true;
        }
        return false;
    }

    private void GenerateLevel()
    {
        GameObject waterPrefab = GameManager.main.settings.waterPrefab;
        GameObject lilyPadPrefab = GameManager.main.settings.lilyPadPrefab;
        GameObject rockPrefab = GameManager.main.settings.rockPrefab;
        GameObject frogPrefab = GameManager.main.settings.frogPrefab;
        GameObject goalPrefab = GameManager.main.settings.goalPrefab;
        GameObject wallPrefab = GameManager.main.settings.wallPrefab;
        GameObject logPrefab = GameManager.main.settings.logPrefab;

        for (int x = 0; x < level.size.x; x++)
        {
            for (int y = 0; y < level.size.y; y++)
            {
                Cell cell = level.GetCell(x, y);
                PuzzleObject.L0 PO0 = cell.PO0;
                if (PO0.hasWater)
                {
                    float waterHeight = GameManager.main.settings.stepHeight * (cell.height + 1);
                    Vector3 waterPos = LevelToWorld(x, y) + Vector3.down * waterHeight / 2;
                    GameObject water = Instantiate(waterPrefab, waterPos, Quaternion.identity, transform);
                    water.transform.localScale = new Vector3(1f, waterHeight, 1f);

                    if (PO0 is PuzzleObject.L0.Algae)
                    {
                        water.GetComponent<Renderer>().material.color = GameManager.main.settings.algaeColor;
                        water.GetComponent<Renderer>().material.SetInt("_hasWavePattern", 1);
                    }
                    else if (PO0 is PuzzleObject.L0.ShallowWater)
                    {
                        water.GetComponent<Renderer>().material.color = GameManager.main.settings.shallowColor;
                        water.GetComponent<Renderer>().material.SetInt("_hasWavePattern", 1);
                    }
                }
                PuzzleObject PO1 = cell.PO1;
                switch (PO1)
                {
                    case PuzzleObject.L1.LilyPad:
                        Vector3 lilyPos = LevelToWorld(x, y);
                        PO1.gameObject = Instantiate(lilyPadPrefab, lilyPos, RandomRotation(), transform);
                        break;
                    case PuzzleObject.L1.Rock:
                        Vector3 rockPos = LevelToWorld(x, y);
                        PO1.gameObject = Instantiate(rockPrefab, rockPos, RandomRotation(), transform);
                        break;
                    case PuzzleObject.L1.Log log:
                        if (log.partnerDir == Vector2Int.down)
                        {
                            Vector3 logPos = LevelToWorld(x, y) - Vector3.forward / 2f;
                            GameObject logGO = Instantiate(logPrefab, logPos, Quaternion.AngleAxis(90f, Vector3.up), transform);
                            log.gameObject = logGO;
                            log.partner.gameObject = logGO;
                        }
                        if (log.partnerDir == Vector2Int.right)
                        {
                            Vector3 logPos = LevelToWorld(x, y) + Vector3.right / 2f;
                            GameObject logGO = Instantiate(logPrefab, logPos, Quaternion.identity, transform);
                            log.gameObject = logGO;
                            log.partner.gameObject = logGO;
                        }
                        break;
                }
                PuzzleObject PO2 = cell.PO2;
                switch (PO2)
                {
                    case PuzzleObject.L2.Frog:
                        Vector3 frogPos = LevelToWalkPos(x, y);
                        PO2.gameObject = Instantiate(frogPrefab, frogPos, Quaternion.identity, transform);
                        break;
                }
                PuzzleObject PO3 = cell.PO3;
                switch (PO3)
                {
                    case PuzzleObject.L3.Goal:
                        Vector3 goalPos = LevelToWorld(x, y) + Vector3.up * 0.5f;
                        PO2.gameObject = Instantiate(goalPrefab, goalPos, Quaternion.identity, transform);
                        break;
                }
            }
        }

        for (int x = 0; x <= level.size.x; x++)
        {
            for (int y = 0; y <= level.size.y; y++)
            {
                Cell cell = level.GetCell(x, y);
                Cell leftCell = level.GetCell(x - 1, y);

                int maxHeight = Math.Max(cell.height, leftCell.height);
                if (maxHeight >= 0 && cell.height != leftCell.height)
                {
                    float wallHeight = GameManager.main.settings.stepHeight * (maxHeight + 1) + 0.05f;
                    Vector3 wallPos = LevelToWorld(x, y);
                    wallPos.y = 0f;
                    wallPos += Vector3.down * GameManager.main.settings.stepHeight + Vector3.up * wallHeight / 2f + Vector3.left * 0.5f;
                    GameObject wall = Instantiate(wallPrefab, wallPos, Quaternion.identity, transform);
                    wall.transform.localScale = new Vector3(0.1f, wallHeight, 1.1f);
                }

                Cell bottomCell = level.GetCell(x, y - 1);

                maxHeight = Math.Max(cell.height, bottomCell.height);
                if (maxHeight >= 0 && cell.height != bottomCell.height)
                {
                    float wallHeight = GameManager.main.settings.stepHeight * (maxHeight + 1) + 0.05f;
                    Vector3 wallPos = LevelToWorld(x, y);
                    wallPos.y = 0f;
                    wallPos += Vector3.down * GameManager.main.settings.stepHeight + Vector3.up * wallHeight / 2f + Vector3.back * 0.5f;
                    GameObject wall = Instantiate(wallPrefab, wallPos, Quaternion.identity, transform);
                    wall.transform.localScale = new Vector3(1.1f, wallHeight, 0.1f);
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
        float y = level.GetCell(gridPos).height * GameManager.main.settings.stepHeight;
        return transform.TransformPoint(new Vector3(gridPos.x - level.size.x / 2f + 0.5f, y, gridPos.y - level.size.y / 2f + 0.5f));
    }
    public Vector3 LevelToWorld(int x, int y)
    {
        return LevelToWorld(new Vector2Int(x, y));
    }
    public Vector3 LevelCenter()
    {
        return transform.position;
    }
    public Vector3 LevelToWalkPos(Vector2Int gridPos)
    {
        return LevelToWorld(gridPos) + Vector3.up * level.GetCell(gridPos).PO1.walkHeight;
    }
    public Vector3 LevelToWalkPos(int x, int y)
    {
        return LevelToWorld(x, y) + Vector3.up * level.GetCell(x, y).PO1.walkHeight;
    }
    private Quaternion RandomRotation()
    {
        return Quaternion.AngleAxis(UnityEngine.Random.Range(0f, 360f), Vector3.up);
    }
}

