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

        if (GetCell(frogPos + dir).mainPO.isWalkable)
        {
            frogPos += dir;
            LevelManager.instance.AddUpdate(new LevelUpdate.FrogJump(frogPos));
        }
        else if (GetCell(frogPos + 2 * dir).mainPO.isWalkable)
        {
            Cell landingCell = GetCell(frogPos + 2 * dir);
            frogPos = landingCell.pos;
            LevelManager.instance.AddUpdate(new LevelUpdate.FrogJump(frogPos));

            if (landingCell.mainPO.type == ObjectType.lilyPad)
            {
                Vector2Int newPos = landingCell.pos;
                while (GetCell(newPos + dir).mainPO.type == ObjectType.none && GetCell(newPos + dir).height == landingCell.height)
                {
                    newPos += dir;
                }
                if (newPos != landingCell.pos)
                {
                    Cell endCell = GetCell(newPos);
                    endCell.mainPO = landingCell.mainPO;
                    landingCell.mainPO = new PuzzleObject();
                    frogPos = newPos;
                    LevelManager.instance.AddUpdate(new LevelUpdate.Float(endCell.mainPO, newPos));
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


