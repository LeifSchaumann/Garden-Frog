using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelData
{
    public static Level Load(string levelName)
    {
        TextAsset levelJson = Resources.Load<TextAsset>("Levels/" + levelName);
        LevelSetup setup = JsonUtility.FromJson<LevelSetup>(levelJson.text);

        Cell[,] cells = new Cell[setup.size.x, setup.size.y];
        Vector2Int frogPos = Vector2Int.zero;

        int currentLilyId = 0;
        for (int x = 0; x < setup.size.x; x++)
        {
            for (int y = 0; y < setup.size.y; y++)
            {
                cells[x, y] = new Cell();
                cells[x, y].height = setup.heightMap[x][y] - '0';

                if (setup.layer1[x][y] == 'L')
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

                if (setup.layer2[x][y] == 'F')
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

[System.Serializable]
public class LevelSetup
{
    public Vector2Int size;
    public string[] heightMap;
    public string[] layer1;
    public string[] layer2;
}