using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PuzzleObject
{
    public GameObject gameObject;
    public Level level;
    public Cell cell;
    public Vector2Int pos;
    public PuzzleObject()
    {
        this.gameObject = null;
        this.level = null;
    }
    public void Move(Cell newCell)
    {
        cell.ClearPO(this);
        newCell.SetPO(this);
    }
    public void Move(Vector2Int dir)
    {
        Move(level.GetCell(pos + dir));
    }
    public abstract class L0 : PuzzleObject
    {
        public bool hasWater;
        
        public class None : L0
        {
            public None()
            {
                this.hasWater = false;
            }
        }
        public class Water : L0
        {
            public Water()
            {
                this.hasWater = true;
            }
        }
    }
    public abstract class L1 : PuzzleObject
    {
        public bool isWalkable;
        public bool canFloat;
        public class None : L1
        {
            public None()
            {
                this.isWalkable = false;
                this.canFloat = false;
            }
        }
        public class LilyPad : L1
        {
            public LilyPad()
            {
                this.isWalkable = true;
                this.canFloat = true;
            }
        }
        public class Rock : L1
        {
            public Rock()
            {
                this.isWalkable = true;
                this.canFloat = false;
            }
        }
    }
    public abstract class L2 : PuzzleObject
    {
        public bool isObstacle;
        public class None : L2
        {
            public None()
            {
                this.isObstacle = false;
            }
        }
        public class Frog : L2
        {
            public Frog()
            {
                this.isObstacle = true;
            }
            public void Jump(Vector2Int dir)
            {
                Cell targetCell = cell.AdjacentCell(dir);
                if (targetCell.PO1.isWalkable)
                {
                    Move(dir);
                    level.manager.AddUpdate(new LevelUpdate((Action onFinish) =>
                    {
                        gameObject.GetComponent<FrogJump>().Jump(cell.pos, onFinish);
                    }));
                }
            }
        }
    }
    public abstract class L3 : PuzzleObject
    {
        public class None : L3
        {
            public None() { }
        }
        public class Goal : L3
        {
            public Goal() { }
        }
    }
}
