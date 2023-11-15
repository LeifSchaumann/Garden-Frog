using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level
{
    public Cell[,] cells;
    public PuzzleObject.L2.Frog frog;
    public Vector2Int size;
    public LevelManager manager;

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
    /*
    public void Move(Vector2Int dir)
    {
        if (GetCell(frogPos + dir).PO1.isWalkable)
        {
            frogPos += dir;
            manager.AddUpdate(new LevelUpdate.FrogJump(frogPos));
            if (GetCell(frogPos).PO3 is PuzzleObject.L3.Goal)
            {
                manager.AddUpdate(new LevelUpdate.GoalReached());
            }
        }
        else if (GetCell(frogPos + 2 * dir).PO1.isWalkable)
        {
            Cell landingCell = GetCell(frogPos + 2 * dir);
            frogPos = landingCell.pos;
            manager.AddUpdate(new LevelUpdate.FrogJump(frogPos));
            if (GetCell(frogPos).PO3 is PuzzleObject.L3.Goal)
            {
                manager.AddUpdate(new LevelUpdate.GoalReached());
            }
            if (landingCell.PO1.canFloat)
            {
                Vector2Int floatEndPos = landingCell.pos;
                while (true)
                {
                    Cell nextCell = GetCell(floatEndPos + dir);
                    if (nextCell.height == landingCell.height && nextCell.PO0.hasWater && nextCell.PO1 is PuzzleObject.L1.None)
                    {
                        floatEndPos += dir;
                    }
                    else
                    {
                        break;
                    }
                }
                if (floatEndPos != landingCell.pos)
                {
                    Cell endCell = GetCell(floatEndPos);
                    endCell.PO1 = landingCell.PO1;
                    landingCell.PO1 = new PuzzleObject.L1.None();
                    frogPos = floatEndPos;
                    manager.AddUpdate(new LevelUpdate.Float(endCell.PO1, floatEndPos));
                }
            }
        }
    }
    */

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


