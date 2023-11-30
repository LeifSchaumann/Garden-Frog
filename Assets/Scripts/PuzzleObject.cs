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
        public float walkHeight;
        public virtual void Push(Vector2Int dir) { }
        public class None : L1
        {
            public None()
            {
                this.isWalkable = false;
                walkHeight = 0;
            }
        }
        public class LilyPad : L1
        {
            public LilyPad()
            {
                this.isWalkable = true;
                this.walkHeight = 0.05f;
            }
            public override void Push(Vector2Int dir)
            {
                PuzzleObject.L2 carry = cell.PO2;
                while (true)
                {
                    Cell nextCell = cell.AdjacentCell(dir);
                    if (nextCell.PO0.hasWater && nextCell.PO1 is PuzzleObject.L1.None && nextCell.height == cell.height)
                    {
                        Move(nextCell);
                    }
                    else
                    {
                        break;
                    }
                }
                if (carry.isCarried)
                {
                    carry.Move(cell);
                    level.manager.AddUpdate(new LevelUpdate.Float(gameObject, pos, carry.gameObject.transform));
                }
                else
                {
                    level.manager.AddUpdate(new LevelUpdate.Float(gameObject, pos, null));
                }
            }
        }
        public class Rock : L1
        {
            public Rock()
            {
                this.isWalkable = true;
                this.walkHeight = 0.13f;
            }
        }
    }
    public abstract class L2 : PuzzleObject
    {
        public bool isObstacle;
        public bool isCarried;
        public class None : L2
        {
            public None()
            {
                this.isObstacle = false;
                this.isCarried = false;
            }
        }
        public class Frog : L2
        {
            public Frog()
            {
                this.isObstacle = true;
                this.isCarried = true;
            }
            public void Jump(Vector2Int dir)
            {
                Cell targetCell = cell.AdjacentCell(dir);
                if (targetCell.PO1.isWalkable &&
                    !targetCell.PO2.isObstacle &&
                    targetCell.height < cell.height + 2)
                {
                    Move(dir);
                    level.manager.AddUpdate(new LevelUpdate((LevelManager manager) =>
                    {
                        gameObject.GetComponent<FrogJump>().Jump(cell.pos, () => { manager.UpdateFinished(); });
                    }));
                    CheckForGoal();
                }
                else
                {
                    targetCell = cell.AdjacentCell(dir * 2);
                    if (targetCell.PO1.isWalkable &&
                        !targetCell.PO2.isObstacle &&
                        targetCell.height < cell.height + 2 &&
                        cell.AdjacentCell(dir).height < cell.height + 2)
                    {
                        Move(dir * 2);
                        level.manager.AddUpdate(new LevelUpdate((LevelManager manager) =>
                        {
                            gameObject.GetComponent<FrogJump>().Jump(cell.pos, () => { manager.UpdateFinished(); });
                        }));
                        CheckForGoal();
                        cell.PO1.Push(dir);
                    }
                }
            }
            private void CheckForGoal()
            {
                if (cell.PO3 is PuzzleObject.L3.Goal goal)
                {
                    goal.Complete();
                }
            }
        }
    }
    public abstract class L3 : PuzzleObject
    {
        public class None : L3 { }
        public class Goal : L3
        {
            public void Complete()
            {
                level.manager.AddUpdate(new LevelUpdate((LevelManager manager) =>
                {
                    GameManager.main.NextLevel();
                    manager.UpdateFinished();
                }));
            }
        }
    }
}
