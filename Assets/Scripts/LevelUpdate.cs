using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelUpdate
{
    public Action<LevelManager> execute;
    public LevelUpdate[] preUpdates;
    public LevelUpdate[] postUpdates;

    public LevelUpdate() {
        this.preUpdates = new LevelUpdate[0];
        this.postUpdates = new LevelUpdate[0];
    }
    public LevelUpdate(Action<LevelManager> execute, LevelUpdate[] preUpdates = null, LevelUpdate[] postUpdates = null)
    {
        this.execute = execute;
        this.preUpdates = preUpdates ?? new LevelUpdate[0];
        this.postUpdates = postUpdates ?? new LevelUpdate[0];
    }

    public class Load : LevelUpdate
    {
        public Load(TextAsset levelJson, bool instant = false, Action onFinish = null, Action onDefined = null)
        {
            onFinish ??= () => { };
            onDefined ??= () => { };

            this.preUpdates = new LevelUpdate[] { new Unload(instant) };
            this.postUpdates = new LevelUpdate[] { new ChangeGridOpacity(1f) };

            this.execute = (LevelManager manager) => {
                manager.LoadLevel(levelJson, instant, () =>
                {
                    onFinish();
                    manager.UpdateFinished();
                }, onDefined);
            };
        }
    }

    public class Unload : LevelUpdate
    {
        public Unload(bool instant = false, Action onFinish = null)
        {
            onFinish ??= () => { };

            this.preUpdates = new LevelUpdate[] { new ChangeGridOpacity(0f) };

            this.execute = (LevelManager manager) => {
                manager.UnloadLevel(instant, () =>
                {
                    onFinish();
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

    public class ChangeGridOpacity : LevelUpdate
    {
        public ChangeGridOpacity(float targetOpacity)
        {
            this.execute = (LevelManager manager) => {
                foreach (Transform child in manager.transform)
                {
                    GridMatController gmc = child.GetComponentInChildren<GridMatController>();
                    if (gmc != null)
                    {
                        gmc.OpacityLerp(targetOpacity, 0.5f, () => { });
                    }
                }
                manager.UpdateFinished();
            };
        }
    }
}