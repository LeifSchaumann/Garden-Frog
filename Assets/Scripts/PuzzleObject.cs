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
        public class ShallowWater : L0
        {
            public ShallowWater()
            {
                this.hasWater = true;
            }
        }
        public class Algae : L0
        {
            public Algae()
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
        public bool CanFloatTo(Cell targetCell, int height)
        {
            return targetCell.PO0.hasWater && targetCell.PO1 is None && targetCell.height == height;
        }
        public class None : L1
        {
            public None()
            {
                this.isWalkable = false;
                walkHeight = -0.2f;
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
        public class LilyPad : L1
        {
            public LilyPad()
            {
                this.isWalkable = true;
                this.walkHeight = 0.05f;
            }
            private bool Slide(Vector2Int dir)
            {
                if (CanFloatTo(cell.AdjacentCell(dir), cell.height))
                {
                    Move(dir);
                    return true;
                }
                else
                {
                    return false;
                }
            }
            public override void Push(Vector2Int dir)
            {
                L2 carry = cell.PO2;
                while (true)
                {
                    if (!Slide(dir)) { break; }
                    if (cell.PO0 is L0.Algae) { break; }
                }
                Vector3 newPos = level.manager.LevelToWorld(pos);
                if (carry.isCarried)
                {
                    carry.Move(cell);
                    level.manager.AddUpdate(new LevelUpdate.Float(gameObject, newPos, carry.gameObject.transform));
                }
                else
                {
                    level.manager.AddUpdate(new LevelUpdate.Float(gameObject, newPos, null));
                }
            }
        }

        public class Log : L1
        {
            public Log partner;
            public Vector2Int partnerDir;
            public Log()
            {
                this.isWalkable = true;
                this.walkHeight = 0.25f;
            }
            public void SetPartner(Log log)
            {
                partner = log;
                log.partner = this;
                partnerDir = partner.pos - pos;
                log.partnerDir = -partnerDir;
            }
            private bool Slide(Vector2Int dir)
            {
                if (partnerDir == dir)
                {
                    return partner.Slide(dir);
                }
                else
                {
                    if (CanFloatTo(cell.AdjacentCell(dir), cell.height))
                    {
                        Move(dir);
                        partner.Move(dir);
                        return true;
                    }
                    else { return false; }
                }
            }
            private bool Rotate(Vector2Int dir)
            {
                Cell diagonalCell = cell.AdjacentCell(dir);
                Cell endCell = cell.AdjacentCell(dir + partnerDir);
                if (CanFloatTo(diagonalCell, cell.height) && CanFloatTo(endCell, cell.height))
                {
                    Move(endCell);
                    SetPartner(partner);
                    return true;
                }
                else { return false; }
            }
            public override void Push(Vector2Int dir)
            {
                L2 carry = cell.PO2;
                Vector3 offset = gameObject.transform.position - level.manager.LevelToWorld(pos);
                if (Vector2.Dot(dir, partnerDir) > 0f)
                {
                    while (true)
                    {
                        if (!Slide(dir)) { break; }
                        if (cell.PO0 is L0.Algae || partner.cell.PO0 is L0.Algae) { break; }
                    }
                    Vector3 newPos = level.manager.LevelToWorld(pos) + offset;
                    if (carry.isCarried)
                    {
                        carry.Move(cell);
                        level.manager.AddUpdate(new LevelUpdate.Float(gameObject, newPos, carry.gameObject.transform));
                    }
                    else
                    {
                        level.manager.AddUpdate(new LevelUpdate.Float(gameObject, newPos, null));
                    }
                }
                else
                {
                    float angle = -90 * (dir.x - dir.y) * (partnerDir.x + partnerDir.y); // Math magic!
                    if (Rotate(dir))
                    {
                        if (carry.isCarried)
                        {
                            carry.Move(cell);
                            level.manager.AddUpdate(new LevelUpdate.Rotate(gameObject, level.manager.LevelToWorld(partner.pos), angle, carry.gameObject.transform));
                        }
                        else
                        {
                            level.manager.AddUpdate(new LevelUpdate.Rotate(gameObject, level.manager.LevelToWorld(partner.pos), angle, null));
                        }
                    }
                }
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
                if ((targetCell.PO1.isWalkable || targetCell.PO0 is PuzzleObject.L0.ShallowWater) &&
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
                    GameManager.main.LevelComplete();
                    manager.UpdateFinished();
                }));
            }
        }
    }
}
