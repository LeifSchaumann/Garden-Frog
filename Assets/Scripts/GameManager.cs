using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public GameSettings settings;

    private CameraMovement camMovement;
    private int currentLevel;

    private void Awake()
    {
        instance = this;

        camMovement = Camera.main.GetComponent<CameraMovement>();
        currentLevel = 0;
    }

    private void Start()
    {
        NextLevel(true);
    }

    public void NextLevel(bool instant = false)
    {
        LevelManager.instance.UnloadLevel(() =>
        {
            currentLevel++;
            if (settings.levelSequence.Length > currentLevel)
            {
                LevelManager.instance.LoadLevel(settings.levelSequence[currentLevel], onDefined: () => {
                    camMovement.FocusOn(LevelManager.instance.transform.position, instant);
                });
            }
        });
    }
}
