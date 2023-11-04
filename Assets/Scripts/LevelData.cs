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
                cells[x, y].height = levelData.heightMap[x][y] - '0';

                if (levelData.layer1[x][y] == 'L')
                {
                    cells[x, y].lilyId = currentLilyId;
                    cells[x, y].layer1 = Layer1.lilyPad;
                    currentLilyId++;
                }
                else
                {
                    cells[x, y].layer1 = Layer1.none;
                }

                cells[x, y].layer2 = Layer2.none;

                if (levelData.layer2[x][y] == 'F')
                {
                    frogPos = new Vector2Int(x, y);
                }
            }
        }
        return new Level(cells, frogPos);
    }
}

public enum Layer1
{
    missing,
    none,
    lilyPad,
    rock
}

public enum Layer2
{
    missing,
    none,
    flower
}

public class Cell
{
    public int height;
    public Layer1 layer1;
    public Layer2 layer2;
    public int lilyId;

    public Cell()
    {
        this.height = -1;
        this.layer1 = Layer1.missing;
        this.layer2 = Layer2.missing;
        this.lilyId = -1;
    }

    public Cell(int height, Layer1 layer1, Layer2 layer2, int lilyId)
    {
        this.height = height;
        this.layer1 = layer1;
        this.layer2 = layer2;
        this.lilyId = lilyId;
    }
}