using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelUpdate
{
    public Action<Action> execute;

    public LevelUpdate() { }
    public LevelUpdate(Action<Action> execute)
    {
        this.execute = execute;
    }

    public class Float : LevelUpdate // ASSUMES PLAYER RIDES WITH OBJECT
    {
        public Float(PuzzleObject.L1 PO1, Vector2Int newPos)
        {
            this.execute = (Action onFinish) => {
                PO1.gameObject.GetComponent<FloatMovement>().Float(newPos, onFinish);
            };
        }
    }
}