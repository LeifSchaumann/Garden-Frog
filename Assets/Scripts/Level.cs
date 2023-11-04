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

        bool smallJump = false;
        Vector2Int newPos = frogPos + dir;
        if (GetCell(newPos).layer1 == Layer1.lilyPad)
        {
            frogPos = newPos;
            smallJump = true;
            LevelManager.instance.AddUpdate(new FrogJump(newPos));
        }
        if (!smallJump)
        {
            newPos = frogPos + 2 * dir;
            if (GetCell(newPos).layer1 == Layer1.lilyPad)
            {
                frogPos = newPos;
                LevelManager.instance.AddUpdate(new FrogJump(newPos));

                Vector2Int floatStart = newPos;
                while (GetCell(newPos + dir).layer1 == Layer1.none)
                {
                    newPos += dir;
                }
                if (newPos != floatStart)
                {
                    Cell startCell = GetCell(floatStart);
                    Cell endCell = GetCell(newPos);
                    endCell.layer1 = Layer1.lilyPad;
                    endCell.lilyId = startCell.lilyId;
                    startCell.layer1 = Layer1.none;
                    startCell.lilyId = -1;
                    frogPos = newPos;

                    LevelManager.instance.AddUpdate(new LilyFloat(endCell.lilyId, newPos));
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


