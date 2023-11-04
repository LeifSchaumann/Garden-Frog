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

        if (GetCell(frogPos + dir).layer1.isWalkable)
        {
            frogPos += dir;
            LevelManager.instance.AddUpdate(new FrogJump(frogPos));
        }
        else if (GetCell(frogPos + 2 * dir).layer1.isWalkable)
        {
            Cell landingCell = GetCell(frogPos + 2 * dir);
            frogPos = landingCell.pos;
            LevelManager.instance.AddUpdate(new FrogJump(frogPos));

            if (landingCell.layer1.type == Layer1Type.lilyPad)
            {
                LevelManager.instance.AddUpdate(new FrogJump(frogPos));

                Vector2Int newPos = landingCell.pos;
                while (GetCell(newPos + dir).layer1.type == Layer1Type.water && GetCell(newPos + dir).height == landingCell.height)
                {
                    newPos += dir;
                }
                if (newPos != landingCell.pos)
                {
                    Cell endCell = GetCell(newPos);
                    endCell.layer1 = landingCell.layer1;
                    landingCell.layer1 = new WaterData();
                    frogPos = newPos;

                    LevelManager.instance.AddUpdate(new LilyFloat(endCell.layer1.id, newPos));
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


