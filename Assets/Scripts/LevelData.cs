using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LevelData
{
    // Stores immutable data about the level setup, JSON files deserialize into this

    public int lilyRequirement;
    public string[] heightMap;
    public string[] layer0;
    public string[] layer1;
    public string[] layer2;

    [NonSerialized]
    public bool completed;
    [NonSerialized]
    public bool locked;
    [NonSerialized]
    public Vector2Int size;
    [NonSerialized]
    public TextAsset json;
    [NonSerialized]
    public Texture2D photo;

    public static LevelData LoadData(TextAsset levelJson)
    {
        LevelData levelData = JsonUtility.FromJson<LevelData>(levelJson.text);
        levelData.completed = false;
        levelData.size = new Vector2Int(levelData.heightMap[0].Length, levelData.heightMap.Length);
        levelData.json = levelJson;
        levelData.locked = levelData.lilyRequirement > 0;
        return levelData;
    }

    public Level MakeLevel()
    {
        Level level = new Level(size);
        level.data = this;

        for (int x = 0; x < size.x; x++)
        {
            for (int y = 0; y < size.y; y++)
            {
                int accessX = size.y - y - 1;
                int accessY = x;

                char heightMapChar = heightMap[accessX][accessY];
                if (heightMapChar - '0' >= 0 && heightMapChar - '0' < 10)
                {
                    level.cells[x, y].height = heightMapChar - '0';
                }

                if (level.cells[x, y].height != -1)
                {
                    switch (layer0[accessX][accessY])
                    {
                        case 'S':
                            level.cells[x, y].SetPO(new PuzzleObject.L0.ShallowWater());
                            break;
                        case 'A':
                            level.cells[x, y].SetPO(new PuzzleObject.L0.Algae());
                            break;
                        default:
                            level.cells[x, y].SetPO(new PuzzleObject.L0.Water());
                            break;
                    }
                }

                switch (layer1[accessX][accessY])
                {
                    case 'L':
                        level.cells[x, y].SetPO(new PuzzleObject.L1.LilyPad());
                        break;
                    case 'R':
                        level.cells[x, y].SetPO(new PuzzleObject.L1.Rock());
                        break;
                    case 'v':
                        PuzzleObject.L1.Log log = new PuzzleObject.L1.Log();
                        PuzzleObject.L1.Log log2 = new PuzzleObject.L1.Log();
                        level.cells[x, y].SetPO(log);
                        level.cells[x, y - 1].SetPO(log2);
                        log.SetPartner(log2);
                        break;
                    case '>':
                        log = new PuzzleObject.L1.Log();
                        log2 = new PuzzleObject.L1.Log();
                        level.cells[x, y].SetPO(log);
                        level.cells[x + 1, y].SetPO(log2);
                        log.SetPartner(log2);
                        break;
                }

                switch (layer2[accessX][accessY])
                {
                    case 'F':
                        PuzzleObject.L2.Frog frog = new PuzzleObject.L2.Frog();
                        level.cells[x, y].SetPO(frog);
                        level.frog = frog;
                        break;
                    case 'G':
                        level.cells[x, y].SetPO(new PuzzleObject.L3.Goal());
                        break;
                }
            }
        }
        return level;
    }
    
}