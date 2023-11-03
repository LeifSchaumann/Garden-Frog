using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class LevelUpdate
{
    public abstract void Execute();
    public void OnFinish()
    {
        LevelManager.instance.updateQueue.Dequeue();
        //Debug.Log("Removed from queue, now has length " + LevelManager.instance.updateQueue.Count);
        LevelManager.instance.NextUpdate();
    }
}

public class FrogJump : LevelUpdate
{
    public Vector2Int newGridPos;

    public FrogJump(Vector2Int newPos)
    {
        this.newGridPos = newPos;
    }

    public override void Execute()
    {
        LevelManager.instance.frog.Jump(newGridPos, OnFinish);
    }
}

public class LilyFloat : LevelUpdate
{
    public int lilyId;
    public Vector2Int newPos;

    public LilyFloat(int lilyId, Vector2Int newPos)
    {
        this.lilyId = lilyId;
        this.newPos = newPos;
    }

    public override void Execute()
    {
        LevelManager.instance.lilyPads[lilyId].Float(newPos, OnFinish);
    }
}