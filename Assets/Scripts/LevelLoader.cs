using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelLoader
{
    public static Level Load(string levelName)
    {
        TextAsset levelJson = Resources.Load<TextAsset>("Levels/" + levelName);
        LevelSetup setup = JsonUtility.FromJson<LevelSetup>(levelJson.text);

        int[,] heights = new int[setup.size.x, setup.size.y];
        int[,] lilyPads = new int[setup.size.x, setup.size.y];

        int currentLilyId = 0;
        for (int x = 0; x < setup.size.x; x++)
        {
            for (int y = 0; y < setup.size.y; y++)
            {
                heights[x, y] = setup.heights[x][y] - '0';
                if (setup.lilyPads[x][y] == '1')
                {
                    lilyPads[x, y] = currentLilyId;
                    currentLilyId++;
                }
                else
                {
                    lilyPads[x, y] = -1;
                }
            }
        }
        return new Level(heights, lilyPads, setup.frogPos);
    }
}

[System.Serializable]
public class LevelSetup
{
    public Vector2Int size;
    public string[] heights;
    public string[] lilyPads;
    public Vector2Int frogPos;
}