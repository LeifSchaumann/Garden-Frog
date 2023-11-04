using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class LevelUpdate
{
    public abstract void Execute();
    public void OnFinish()
    {
        LevelManager.instance.updateQueue.Dequeue();
        LevelManager.instance.NextUpdate();
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

    public class Float : LevelUpdate // ASSUMES PLAYER RIDES WITH OBJECT
    {
        public GameObject gameObject;
        public Vector2Int newPos;

        public Float(PuzzleObject layer1Data, Vector2Int newPos)
        {
            this.gameObject = layer1Data.gameObject;
            this.newPos = newPos;
        }

        public override void Execute()
        {
            gameObject.GetComponent<FloatMovement>().Float(newPos, OnFinish);
        }
    }

    public class GoalReached : LevelUpdate
    {
        public override void Execute()
        {
            Debug.Log("Goal reached!");
            OnFinish();
        }
    }
}