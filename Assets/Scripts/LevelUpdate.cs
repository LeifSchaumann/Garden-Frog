using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelUpdate
{
    public Action<LevelManager> execute;

    public LevelUpdate() { }
    public LevelUpdate(Action<LevelManager> execute)
    {
        this.execute = execute;
    }

    public class Load : LevelUpdate
    {
        public Load(TextAsset levelJson, bool instant = false, Action onFinish = null, Action onDefined = null)
        {
            onFinish ??= () => { };
            onDefined ??= () => { };

            this.execute = (LevelManager manager) => {
                if (manager.level == null)
                {
                    manager.LoadLevel(levelJson, instant, () =>
                    {
                        onFinish();
                        manager.UpdateFinished();
                    }, onDefined);
                }
                else
                {
                    manager.UnloadLevel(instant, () =>
                    {
                        manager.LoadLevel(levelJson, instant, () =>
                        {
                            onFinish();
                            manager.UpdateFinished();
                        }, onDefined);
                    });
                }
            };
        }
    }

    public class Unload : LevelUpdate
    {
        public Unload(bool instant = false, Action onFinish = null)
        {
            this.execute = (LevelManager manager) => {
                manager.UnloadLevel(instant, () =>
                {
                    onFinish();
                    Debug.Log("calling UpdateFinished");
                    manager.UpdateFinished();
                });
            };
        }
    }

    public class Float : LevelUpdate // ASSUMES PLAYER RIDES WITH OBJECT
    {
        public Float(GameObject gameObject, Vector2Int newPos, Transform carry)
        {
            this.execute = (LevelManager manager) => {
                gameObject.GetComponent<FloatMovement>().Float(newPos, carry, () =>
                {
                    manager.UpdateFinished();
                });
            };
        }
    }
}