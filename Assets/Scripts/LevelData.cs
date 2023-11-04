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
                        cells[x, y].layer1 = new LilyPadData(currentLilyId);
                        currentLilyId++;
                        break;
                    case 'R':
                        cells[x, y].layer1 = new RockData();
                        break;
                    default:
                        cells[x, y].layer1 = new WaterData();
                        break;
                }

                cells[x, y].layer2 = Layer2Data.none;

                if (levelData.layer2[x][y] == 'F')
                {
                    frogPos = new Vector2Int(x, y);
                }
            }
        }
        return new Level(cells, frogPos);
    }
}

public enum Layer1Type
{
    water,
    lilyPad,
    rock
}

public abstract class Layer1Data
{
    public Layer1Type type;
    public bool isWalkable;
    public bool canFloat;
    public int id = -1;
}

public class LilyPadData : Layer1Data
{
    public LilyPadData(int id) { 
        this.id = id;
        this.type = Layer1Type.lilyPad;
        this.isWalkable = true;
        this.canFloat = true;
    }
}

public class RockData : Layer1Data
{
    public RockData()
    {
        this.type = Layer1Type.rock;
        this.isWalkable = true;
        this.canFloat = false;
    }
}

public class WaterData : Layer1Data
{
    public WaterData()
    {
        this.type = Layer1Type.water;
        this.isWalkable = false;
        this.canFloat = false;
    }
}

public enum Layer2Data
{
    missing,
    none
}

public class Cell
{
    public int height;
    public Layer1Data layer1;
    public Layer2Data layer2;
    public Vector2Int pos;

    public Cell()
    {
        this.height = -1;
        this.layer1 = new WaterData();
        this.layer2 = Layer2Data.missing;
        this.pos = Vector2Int.zero;
    }
}