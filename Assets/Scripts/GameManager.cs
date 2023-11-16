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

    private void Awake()
    {
        instance = this;

        camMovement = Camera.main.GetComponent<CameraMovement>();
        currentLevel = 0;
        currentScreen = GameScreen.title;
    }

    private void Start()
    {
        SetScreen(GameScreen.title);
    }

    public void SetScreen(GameScreen screen)
    {
        
        switch (screen)
        {
            case GameScreen.title:
                bool instantZoom = currentScreen == GameScreen.title;
                LevelManager.instance.LoadLevel(settings.levelSequence[currentLevel], instant: false, onDefined: () => {
                    camMovement.FocusOn(LevelManager.instance.LevelCenter(), instantZoom, 0.5f);
                }, onFinish: () =>
                {
                    UIManager.instance.ChangeScreen(screen);
                });
                break;
            case GameScreen.play:
                UIManager.instance.ChangeScreen(screen);
                LevelManager.instance.LoadLevel(settings.levelSequence[currentLevel], instant: true, onDefined: () => {
                    camMovement.FocusOn(LevelManager.instance.LevelCenter(), false, 1f);
                });
                break;
        }
        currentScreen = screen;
    }

    public void NextLevel()
    {
        currentLevel++;
        if (settings.levelSequence.Length > currentLevel)
        {
            LevelManager.instance.LoadLevel(settings.levelSequence[currentLevel], onDefined: () => {
                camMovement.FocusOn(LevelManager.instance.LevelCenter(), true);
            });
        }
    }

    private void Update()
    {
        switch (currentScreen)
        {
            case GameScreen.title:
                if (Input.GetMouseButtonDown(0))
                {
                    SetScreen(GameScreen.play);
                }
                break;
            case GameScreen.play:
                InputManager.instance.AcceptInput();
                if (Input.GetKeyDown(KeyCode.R))
                {
                    LevelManager.instance.ResetLevel();
                }
                else if (Input.GetKeyDown(KeyCode.Escape))
                {
                    SetScreen(GameScreen.title);
                }
                break;
        }
    }
}
