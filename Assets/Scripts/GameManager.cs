using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;
using UnityEngine.UIElements;

public enum GameScreen
{
    title,
    play,
    edit
}

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public GameSettings settings;
    public GameScreen currentScreen;

    private CameraMovement camMovement;
    private int currentLevel;
    private bool transitioning;

    private void Awake()
    {
        instance = this;

        camMovement = Camera.main.GetComponent<CameraMovement>();
        currentLevel = 0;
        transitioning = false;
        currentScreen = GameScreen.title;
    }

    private void Start()
    {
        SetScreen(GameScreen.title);
    }

    public void SetScreen(GameScreen screen)
    {
        transitioning = true;
        switch (screen)
        {
            case GameScreen.title:
                bool instantZoom = currentScreen == GameScreen.title;
                LevelManager.main.AddUpdate(new LevelUpdate.Load(settings.levelSequence[currentLevel], false, onDefined: () => {
                    camMovement.FocusOn(LevelManager.main.LevelCenter(), instantZoom, 0.5f);
                }, onFinish: () =>
                {
                    UIManager.instance.SetScreen(screen, () =>
                    {
                        transitioning = false;
                    });
                }));
                break;
            case GameScreen.play:
                UIManager.instance.SetScreen(screen);
                LevelManager.main.AddUpdate(new LevelUpdate.Load(settings.levelSequence[currentLevel], true, onDefined: () =>
                {
                    camMovement.FocusOn(LevelManager.main.LevelCenter(), false, 1f);
                }, onFinish: () =>
                {
                    transitioning = false;
                }));
                break;
        }
        currentScreen = screen;
    }

    public void NextLevel()
    {
        if (settings.levelSequence.Length > currentLevel + 1)
        {
            currentLevel++;
            LevelManager.main.AddUpdate(new LevelUpdate.Load(settings.levelSequence[currentLevel], onDefined: () => {
                camMovement.FocusOn(LevelManager.main.LevelCenter(), true);
            }));
        }
    }

    private void Update()
    {
        switch (currentScreen)
        {
            case GameScreen.title:
                if (!transitioning)
                {
                    if (Input.GetMouseButtonDown(0))
                    {
                        SetScreen(GameScreen.play);
                    }
                }
                break;
            case GameScreen.play:
                if (!transitioning)
                {
                    InputManager.instance.AcceptInput();
                    if (Input.GetKeyDown(KeyCode.R))
                    {
                        LevelManager.main.ResetLevel();
                    }
                    else if (Input.GetKeyDown(KeyCode.Escape))
                    {
                        SetScreen(GameScreen.title);
                    }
                }
                break;
        }
    }
}
