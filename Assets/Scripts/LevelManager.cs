using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager instance;

    public GameObject waterPrefab;
    public GameObject lilyPadPrefab;
    public GameObject rockPrefab;
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

    public void LoadLevel(string levelName)
    {
        level = LevelData.Load(levelName);
        GenerateLevel();
    }

    public void GenerateLevel()
    {
        for (int x = 0; x < level.size.x; x++)
        {
            for (int y = 0; y < level.size.y; y++)
            {
                Vector3 waterPos = LevelToWorld(x, y) + Vector3.down * waterPrefab.transform.localScale.y / 2;
                Instantiate(waterPrefab, waterPos, Quaternion.identity);

                Layer1Data layer1 = level.GetCell(x, y).layer1;
                switch (layer1.type)
                {
                    case Layer1Type.lilyPad:
                        Vector3 lilyPos = LevelToWorld(x, y) + Vector3.up * lilyPadPrefab.transform.localScale.y / 2;
                        lilyPads.Add(Instantiate(lilyPadPrefab, lilyPos, Quaternion.identity).GetComponent<LilyPad>());
                        break;
                    case Layer1Type.rock:
                        Vector3 rockPos = LevelToWorld(x, y) + Vector3.up * rockPrefab.transform.localScale.y / 2;
                        Instantiate(rockPrefab, rockPos, Quaternion.identity);
                        break;
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
        return new Vector3(gridPos.x, level.GetCell(gridPos).height * stepHeight, gridPos.y);
    }
    public Vector3 LevelToWorld(int x, int y)
    {
        return LevelToWorld(new Vector2Int(x, y));
    }
}

