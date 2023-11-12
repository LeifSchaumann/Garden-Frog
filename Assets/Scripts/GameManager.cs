using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
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
        currentLevel = -1;
    }

    private void Start()
    {
        NextLevel(true);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            LevelManager.instance.UnloadLevel(instant: true, onFinish: () =>
            {
                if (settings.levelSequence.Length > currentLevel)
                {
                    LevelManager.instance.LoadLevel(settings.levelSequence[currentLevel], instant: true);
                }
            });
        }
    }

    public void NextLevel(bool instant = false)
    {
        LevelManager.instance.UnloadLevel(instant, () =>
        {
            currentLevel++;
            if (settings.levelSequence.Length > currentLevel)
            {
                LevelManager.instance.LoadLevel(settings.levelSequence[currentLevel], instant: instant, onDefined: () => {
                    camMovement.FocusOn(LevelManager.instance.transform.position, instant);
                });
            }
        });
    }
}
