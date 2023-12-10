using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell
{
    // Class to represent the contents of a single grid square in a level

    public int height;
    public PuzzleObject.L0 PO0;
    public PuzzleObject.L1 PO1;
    public PuzzleObject.L2 PO2;
    public PuzzleObject.L3 PO3;
    public Vector2Int pos;

    private Level level;

    public Cell(Level level, Vector2Int pos)
    {
        this.level = level;
        this.pos = pos;
        this.height = -1;
        this.PO0 = new PuzzleObject.L0.None();
        this.PO1 = new PuzzleObject.L1.None();
        this.PO2 = new PuzzleObject.L2.None();
        this.PO3 = new PuzzleObject.L3.None();
    }

    public void SetPO(PuzzleObject po)
    {
        po.pos = pos;
        po.cell = this;
        po.level = level;
        switch (po)
        {
            case PuzzleObject.L0 poX:
                PO0 = poX;
                break;
            case PuzzleObject.L1 poX:
                PO1 = poX;
                break;
            case PuzzleObject.L2 poX:
                PO2 = poX;
                break;
            case PuzzleObject.L3 poX:
                PO3 = poX;
                break;
        }
    }

    public void ClearPO(PuzzleObject po)
    {
        switch (po)
        {
            case PuzzleObject.L0:
                SetPO(new PuzzleObject.L0.None());
                break;
            case PuzzleObject.L1:
                SetPO(new PuzzleObject.L1.None());
                break;
            case PuzzleObject.L2:
                SetPO(new PuzzleObject.L2.None());
                break;
            case PuzzleObject.L3:
                SetPO(new PuzzleObject.L3.None());
                break;
        }
    }

    public Cell AdjacentCell(Vector2Int dir)
    {
        return level.GetCell(pos + dir);
    }
}
