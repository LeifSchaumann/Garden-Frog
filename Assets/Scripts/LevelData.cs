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

        Cell[,] cells = new Cell[levelData.size.x, levelData.size.y];
        Vector2Int frogPos = Vector2Int.zero;

        int currentLilyId = 0;
        for (int x = 0; x < levelData.size.x; x++)
        {
            for (int y = 0; y < levelData.size.y; y++)
            {
                cells[x, y] = new Cell();
                cells[x, y].pos = new Vector2Int(x, y);

                char heightMapChar = levelData.heightMap[x][y];
                if (heightMapChar - '0' >= 0 && heightMapChar - '0' < 10)
                {
                    cells[x, y].height = heightMapChar - '0';
                    cells[x, y].PO0 = new PuzzleObject.Water();
                }

                switch (levelData.layer1[x][y])
                {
                    case 'L':
                        cells[x, y].PO1 = new PuzzleObject.LilyPad();
                        currentLilyId++;
                        break;
                    case 'R':
                        cells[x, y].PO1 = new PuzzleObject.Rock();
                        break;
                    default:
                        cells[x, y].PO1 = new PuzzleObject();
                        break;
                }

                switch (levelData.layer2[x][y])
                {
                    case 'F':
                        frogPos = new Vector2Int(x, y);
                        break;
                    case 'G':
                        cells[x, y].PO2 = new PuzzleObject.Goal();
                        break;
                    default:
                        cells[x, y].PO2 = new PuzzleObject();
                        break;
                }
            }
        }
        return new Level(cells, frogPos);
    }
}

public enum ObjectType
{
    none,
    water,
    lilyPad,
    rock,
    goal
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

    public class Water : PuzzleObject
    {
        public Water()
        {
            this.type = ObjectType.water;
        }
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

    public class Goal : PuzzleObject
    {
        public Goal()
        {
            this.type = ObjectType.goal;
        }
    }
}

public class Cell
{
    public int height;
    public PuzzleObject PO0;
    public PuzzleObject PO1;
    public PuzzleObject PO2;
    public Vector2Int pos;

    public Cell()
    {
        this.height = -1;
        this.PO0 = new PuzzleObject();
        this.PO1 = new PuzzleObject();
        this.PO2 = new PuzzleObject();
        this.pos = Vector2Int.zero;
    }
}