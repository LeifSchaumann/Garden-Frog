using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager instance;

    public GameObject waterPrefab;
    public GameObject lilyPadPrefab;
    public float stepHeight;

    public Level level;
    public Frog frog;
    public List<LilyPad> lilyPads;
    public Queue<LevelUpdate> updateQueue;

    private void Awake()
    {
        instance = this;
        updateQueue = new Queue<LevelUpdate>();
    }

    void Start()
    {
        level = new Level(
            new int[,] {
                { 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0 },
            },
            new int[,] {
                { 0, 1, 2, 3, 4 },
                { -1, -1, -1, -1, -1 },
                { -1, 5, -1, -1, -1 },
                { -1, -1, -1, -1, -1 },
                { -1, 6, 7, -1, -1 },
            },
            Vector2Int.zero
        );

        GenerateLevel();
    }

    public void GenerateLevel()
    {
        for (int x = 0; x < level.size.x; x++)
        {
            for (int y = 0; y < level.size.y; y++)
            {
                Vector3 waterPos = LevelToWorld(new Vector2Int(x, y)) + Vector3.down * waterPrefab.transform.localScale.y / 2;
                Instantiate(waterPrefab, waterPos, Quaternion.identity);

                if (level.lilyPads[x, y] >= 0)
                {
                    Vector3 lilyPos = LevelToWorld(new Vector2Int(x, y)) + Vector3.up * lilyPadPrefab.transform.localScale.y / 2;
                    lilyPads.Add(Instantiate(lilyPadPrefab, lilyPos, Quaternion.identity).GetComponent<LilyPad>());
                }
            }
        }
        frog.transform.position = LevelToWorld(level.frogPos) + Vector3.up * frog.transform.localScale.y / 2;
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
        return new Vector3(gridPos.x, level.heights[gridPos.x, gridPos.y] * stepHeight, gridPos.y);
    }
}

