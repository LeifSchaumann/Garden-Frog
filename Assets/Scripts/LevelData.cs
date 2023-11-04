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
    public static Level Load(string levelName)
    {
        TextAsset levelJson = Resources.Load<TextAsset>("Levels/" + levelName);
        LevelData levelData = JsonUtility.FromJson<LevelData>(levelJson.text);

        Cell[,] cells = new Cell[levelData.size.x, levelData.size.y];
        Vector2Int frogPos = Vector2Int.zero;

        int currentLilyId = 0;
        for (int x = 0; x < levelData.size.x; x++)
        {
            for (int y = 0; y < levelData.size.y; y++)
            {
                cells[x, y] = new Cell();
                cells[x, y].pos = new Vector2Int(x, y);
                cells[x, y].height = levelData.heightMap[x][y] - '0';

                switch (levelData.layer1[x][y])
                {
                    case 'L':
                        cells[x, y].mainPO = new PuzzleObject.LilyPad();
                        currentLilyId++;
                        break;
                    case 'R':
                        cells[x, y].mainPO = new PuzzleObject.Rock();
                        break;
                    default:
                        cells[x, y].mainPO = new PuzzleObject();
                        break;
                }

                if (levelData.layer2[x][y] == 'F')
                {
                    frogPos = new Vector2Int(x, y);
                }
            }
        }
        return new Level(cells, frogPos);
    }
}

public enum ObjectType
{
    none,
    lilyPad,
    rock,
    flower
}

public class PuzzleObject
{
    public ObjectType type;
    public bool isWalkable;
    public bool canFloat;
    public GameObject gameObject;

    public PuzzleObject()
    {
        type = ObjectType.none;
        isWalkable = false;
        canFloat = false;
        gameObject = null;
    }

    public class LilyPad : PuzzleObject
    {
        public LilyPad()
        {
            this.type = ObjectType.lilyPad;
            this.isWalkable = true;
            this.canFloat = true;
        }
    }

    public class Rock : PuzzleObject
    {
        public Rock()
        {
            this.type = ObjectType.rock;
            this.isWalkable = true;
        }
    }
}

public class Cell
{
    public int height;
    public PuzzleObject mainPO;
    public PuzzleObject secondPO;
    public Vector2Int pos;

    public Cell()
    {
        this.height = -1;
        this.mainPO = new PuzzleObject();
        this.secondPO = new PuzzleObject();
        this.pos = Vector2Int.zero;
    }
}