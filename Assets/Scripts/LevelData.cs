using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LevelData
{
    public Vector2Int size;
    public string[] heightMap;
    public string[] layer1;
    public string[] layer2;
    public static Level Load(TextAsset levelJson)
    {
        //TextAsset levelJson = Resources.Load<TextAsset>("Levels/" + levelName);
        LevelData levelData = JsonUtility.FromJson<LevelData>(levelJson.text);
        Level level = new Level(levelData.size);

        for (int x = 0; x < levelData.size.x; x++)
        {
            for (int y = 0; y < levelData.size.y; y++)
            {
                int accessX = levelData.size.y - y - 1;
                int accessY = x;

                char heightMapChar = levelData.heightMap[accessX][accessY];
                if (heightMapChar - '0' >= 0 && heightMapChar - '0' < 10)
                {
                    level.cells[x, y].height = heightMapChar - '0';
                    level.cells[x, y].SetPO(new PuzzleObject.L0.Water());
                }

                switch (levelData.layer1[accessX][accessY])
                {
                    case 'L':
                        level.cells[x, y].SetPO(new PuzzleObject.L1.LilyPad());
                        break;
                    case 'R':
                        level.cells[x, y].SetPO(new PuzzleObject.L1.Rock());
                        break;
                    default:
                        level.cells[x, y].SetPO(new PuzzleObject.L1.None());
                        break;
                }

                switch (levelData.layer2[accessX][accessY])
                {
                    case 'F':
                        PuzzleObject.L2.Frog frog = new PuzzleObject.L2.Frog();
                        level.cells[x, y].SetPO(frog);
                        level.frog = frog;
                        break;
                    case 'G':
                        level.cells[x, y].SetPO(new PuzzleObject.L3.Goal());
                        break;
                    default:
                        level.cells[x, y].SetPO(new PuzzleObject.L2.None());
                        break;
                }
            }
        }
        return level;
    }
}