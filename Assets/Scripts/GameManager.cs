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
    edit,
    levels
}

public class GameManager : MonoBehaviour
{
    public static GameManager main;
    
    public GameSettings settings;
    public GameScreen currentScreen;
    public int currentLevel;
    public Photographer photographer;

    private CameraMovement camMovement;

    private void Awake()
    {
        main = this;

        camMovement = Camera.main.GetComponent<CameraMovement>();
        currentLevel = 0;
        currentScreen = GameScreen.none;
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
                if (currentScreen == GameScreen.none)
                {
                    LevelManager.main.AddUpdate(new LevelUpdate.Load(settings.levelSequence[currentLevel], false, onDefined: () => {
                        camMovement.FocusOn(LevelManager.main.LevelCenter(), true, 0.5f);
                    }, onFinish: () =>
                    {
                        UIManager.main.FadeInScreen(screen);
                    }));
                }
                else
                {
                    UIManager.main.FadeOutScreen(() =>
                    {
                        LevelManager.main.AddUpdate(new LevelUpdate.Load(settings.levelSequence[currentLevel], false, onDefined: () => {
                            camMovement.FocusOn(LevelManager.main.LevelCenter(), true, 0.5f);
                        }, onFinish: () =>
                        {
                            UIManager.main.FadeInScreen(screen);
                        }));
                    });
                }
                
                break;
            case GameScreen.play:
                if (currentScreen == GameScreen.title) // WIP
                {
                    UIManager.main.FadeOutScreen();
                    camMovement.FocusOn(LevelManager.main.LevelCenter(), false, 1f, () =>
                    {
                        UIManager.main.FadeInScreen(screen);
                    });
                }
                else
                {
                    UIManager.main.FadeOutScreen(() =>
                    {
                        LevelManager.main.AddUpdate(new LevelUpdate.Load(settings.levelSequence[currentLevel], false, onDefined: () => {
                            camMovement.FocusOn(LevelManager.main.LevelCenter(), true, 1f);
                        }, onFinish: () =>
                        {
                            UIManager.main.FadeInScreen(screen);
                        }));
                    });
                }
                break;
            case GameScreen.levels:
                    UIManager.main.FadeOutScreen();
                    LevelManager.main.AddUpdate(new LevelUpdate.Unload(onFinish: () =>
                    {
                        UIManager.main.FadeInScreen(screen);
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

    public bool DoneTransitioning()
    {
        return !UIManager.main.isFading && !LevelManager.main.IsUpdating(); 
    }

    private void Update()
    {
        UIManager.main.UpdateUI();
        
        switch (currentScreen)
        {
            case GameScreen.title:
                break;
            case GameScreen.play:
                if (LevelManager.main.levelIsLoaded)
                {
                    InputManager.instance.AcceptInput();
                }
                break;
        }
    }
}
