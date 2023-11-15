using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PuzzleObject
{
    public GameObject gameObject;
    public Level level;
    public Cell cell;
    public PuzzleObject()
    {
        this.gameObject = null;
        this.level = null;
    }
    public abstract class L0 : PuzzleObject
    {
        public bool hasWater;
        public void Move(Cell newCell)
        {
            cell.SetPO(new PuzzleObject.L0.None());
            newCell.SetPO(this);
        }
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
        public void Move(Cell newCell)
        {
            cell.SetPO(new PuzzleObject.L1.None());
            newCell.SetPO(this);
        }
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
        public void Move(Cell newCell)
        {
            cell.SetPO(new PuzzleObject.L2.None());
            newCell.SetPO(this);
        }
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
        }
    }
    public abstract class L3 : PuzzleObject
    {
        public void Move(Cell newCell)
        {
            cell.SetPO(new PuzzleObject.L3.None());
            newCell.SetPO(this);
        }
        public class None : L3
        {
            public None()
            {

            }
        }
        public class Goal : L3
        {
            public Goal()
            {

            }
        }
    }
}
