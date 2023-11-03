using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level
{
    public int[,] heights;
    public int[,] lilyPads;
    public Vector2Int frogPos;
    public Vector2Int size;

    public Level(int[,] heights, int[,] lilyPads, Vector2Int frogPos)
    {
        this.heights = heights;
        this.lilyPads = lilyPads;
        this.frogPos = frogPos;

        this.size = new Vector2Int(heights.GetLength(0), heights.GetLength(1));
    }

    public void Move(Vector2Int dir)
    {
        List<LevelUpdate> updates = new List<LevelUpdate>();

        bool smallJump = false;
        Vector2Int newPos = frogPos + dir;
        if (InBounds(newPos))
        {
            if (lilyPads[newPos.x, newPos.y] >= 0)
            {
                frogPos = newPos;
                smallJump = true;
                LevelManager.instance.AddUpdate(new FrogJump(newPos));
            }
        }
        if (!smallJump)
        {
            newPos = frogPos + 2 * dir;
            if (InBounds(newPos))
            {
                int lilyId = GetLily(newPos);
                if (lilyId >= 0)
                {
                    frogPos = newPos;
                    LevelManager.instance.AddUpdate(new FrogJump(newPos));

                    bool floated = false;
                    while (IsWater(newPos + dir))
                    {
                        floated = true;
                        SetLily(newPos, -1);
                        newPos += dir;
                        frogPos = newPos;
                        SetLily(newPos, lilyId);
                    }

                    if (floated)
                    {
                        LevelManager.instance.AddUpdate(new LilyFloat(lilyId, newPos));
                    }
                }
            }
        }
    }

    private bool InBounds(Vector2Int pos)
    {
        return 0 <= pos.x && pos.x < size.x && 0 <= pos.y && pos.y < size.y;
    }

    private bool IsWater(Vector2Int pos)
    {
        return InBounds(pos) && GetLily(pos) == -1;
    }

    private int GetLily(Vector2Int pos)
    {
        if (InBounds(pos))
        {
            return lilyPads[pos.x, pos.y];
        }
        else
        {
            return -1;
        }
    }

    private void SetLily(Vector2Int pos, int lily)
    {
        if (InBounds(pos))
        {
            lilyPads[pos.x, pos.y] = lily;
        }
    }
}


