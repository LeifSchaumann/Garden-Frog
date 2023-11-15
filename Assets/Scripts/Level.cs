using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level
{
    public Cell[,] cells;
    public PuzzleObject.L2.Frog frog;
    public Vector2Int size;
    public LevelManager manager;
    public TextAsset json;

    public Level(Vector2Int size)
    {
        this.manager = LevelManager.instance;
        this.size = size;
        this.cells = new Cell[size.x, size.y];
        for (int x = 0; x < size.x; x++)
        {
            for (int y = 0; y < size.y; y++)
            {
                cells[x, y] = new Cell(this, new Vector2Int(x, y));
            }
        }
    }
    public void Move(Vector2Int dir)
    {
        frog.Jump(dir);
    }

    private bool InBounds(Vector2Int pos)
    {
        return 0 <= pos.x && pos.x < size.x && 0 <= pos.y && pos.y < size.y;
    }

    public Cell GetCell(Vector2Int pos)
    {
        if (InBounds(pos))
        {
            return cells[pos.x, pos.y];
        }
        else
        {
            return new Cell(this, pos);
        }
    }

    public Cell GetCell(int x, int y)
    {
        return GetCell(new Vector2Int(x, y));
    }
}


