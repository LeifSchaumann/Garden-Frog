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
        public Float(GameObject gameObject, Vector2Int newPos, Transform carry)
        {
            this.execute = (Action onFinish) => {
                gameObject.GetComponent<FloatMovement>().Float(newPos, carry, onFinish);
            };
        }
    }
}