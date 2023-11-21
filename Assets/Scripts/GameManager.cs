using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;
using UnityEngine.UIElements;

public enum GameScreen
{
    none,
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
        currentScreen = GameScreen.none;
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
                if (currentScreen == GameScreen.none)
                {
                    LevelManager.main.AddUpdate(new LevelUpdate.Load(settings.levelSequence[currentLevel], false, onDefined: () => {
                        camMovement.FocusOn(LevelManager.main.LevelCenter(), true, 0.5f);
                    }, onFinish: () =>
                    {
                        UIManager.instance.FadeInScreen(screen, onFinish: () =>
                        {
                            transitioning = false;
                        });
                    }));
                }
                else
                {
                    UIManager.instance.FadeOutScreen();
                    LevelManager.main.AddUpdate(new LevelUpdate.Load(settings.levelSequence[currentLevel], false, onDefined: () => {
                        camMovement.FocusOn(LevelManager.main.LevelCenter(), true, 0.5f);
                    }, onFinish: () =>
                    {
                        UIManager.instance.FadeInScreen(screen, onFinish: () =>
                        {
                            transitioning = false;
                        });
                    }));
                }
                
                break;
            case GameScreen.play:
                if (currentScreen == GameScreen.title) // WIP
                {
                    UIManager.instance.FadeOutScreen();
                    camMovement.FocusOn(LevelManager.main.LevelCenter(), false, 1f, () =>
                    {
                        UIManager.instance.FadeInScreen(screen);
                        transitioning = false;
                    });
                }
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
                        if (!LevelManager.main.IsUpdating())
                        {
                            SetScreen(GameScreen.title);
                        }
                    }
                }
                break;
        }
    }
}
