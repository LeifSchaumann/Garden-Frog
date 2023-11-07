using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level
{
    public Cell[,] cells;
    public Vector2Int frogPos;
    public Vector2Int size;

    public Level(Cell[,] cells, Vector2Int frogPos)
    {
        this.cells = cells;
        this.frogPos = frogPos;

        this.size = new Vector2Int(cells.GetLength(0), cells.GetLength(1));
    }

    public void Move(Vector2Int dir)
    {
        List<LevelUpdate> updates = new List<LevelUpdate>();

        if (GetCell(frogPos + dir).PO1.isWalkable)
        {
            frogPos += dir;
            LevelManager.instance.AddUpdate(new LevelUpdate.FrogJump(frogPos));
            if (GetCell(frogPos).PO2.type == ObjectType.goal)
            {
                LevelManager.instance.AddUpdate(new LevelUpdate.GoalReached());
            }
        }
        else if (GetCell(frogPos + 2 * dir).PO1.isWalkable)
        {
            Cell landingCell = GetCell(frogPos + 2 * dir);
            frogPos = landingCell.pos;
            LevelManager.instance.AddUpdate(new LevelUpdate.FrogJump(frogPos));
            if (GetCell(frogPos).PO2.type == ObjectType.goal)
            {
                LevelManager.instance.AddUpdate(new LevelUpdate.GoalReached());
            }

            if (landingCell.PO1.type == ObjectType.lilyPad)
            {
                Vector2Int newPos = landingCell.pos;
                while (GetCell(newPos + dir).PO1.type == ObjectType.none && GetCell(newPos + dir).height == landingCell.height)
                {
                    newPos += dir;
                }
                if (newPos != landingCell.pos)
                {
                    Cell endCell = GetCell(newPos);
                    endCell.PO1 = landingCell.PO1;
                    landingCell.PO1 = new PuzzleObject();
                    frogPos = newPos;
                    LevelManager.instance.AddUpdate(new LevelUpdate.Float(endCell.PO1, newPos));
                }
            }
        }
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
            return new Cell();
        }
    }

    public Cell GetCell(int x, int y)
    {
        return GetCell(new Vector2Int(x, y));
    }
}


