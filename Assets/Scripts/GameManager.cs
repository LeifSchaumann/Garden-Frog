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
    levels,
    completed
}

public class GameManager : MonoBehaviour
{
    public static GameManager main;
    
    public GameSettings settings;
    public GameScreen currentScreen;
    public int currentLevel;
    public LevelData[] levelSequence;
    public Photographer photographer;

    public int lilyCount;

    private CameraMovement camMovement;

    private void Awake()
    {
        main = this;

        camMovement = Camera.main.GetComponent<CameraMovement>();
        currentLevel = 0;
        currentScreen = GameScreen.none;
        lilyCount = 0;

        levelSequence = new LevelData[settings.levelSequence.Length];
        for (int i = 0; i < settings.levelSequence.Length; i++)
        {
            levelSequence[i] = LevelData.LoadData(settings.levelSequence[i]);
        }
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
                    LevelManager.main.AddUpdate(new LevelUpdate.Load(levelSequence[currentLevel], false, onDefined: () => {
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
                        LevelManager.main.AddUpdate(new LevelUpdate.Load(levelSequence[currentLevel], false, onDefined: () => {
                            camMovement.FocusOn(LevelManager.main.LevelCenter(), true, 0.5f);
                        }, onFinish: () =>
                        {
                            UIManager.main.FadeInScreen(screen);
                        }));
                    });
                }
                
                break;
            case GameScreen.play:
                if (currentScreen == GameScreen.title)
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
                        LevelManager.main.AddUpdate(new LevelUpdate.Load(levelSequence[currentLevel], false, onDefined: () => {
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
            case GameScreen.completed:
                UIManager.main.FadeOutScreen();
                camMovement.FocusOn(LevelManager.main.LevelCenter(), false, 0.7f, () =>
                {
                    UIManager.main.FadeInScreen(screen);
                });
                break;
        }
        currentScreen = screen;
    }

    public void NextLevel()
    {
        for (int i = currentLevel + 1; i < levelSequence.Length; i++)
        {
            if (!levelSequence[i].completed)
            {
                currentLevel = i;
                SetScreen(GameScreen.play);
                return;
            }
        }
        for (int i = 0; i < currentLevel; i++)
        {
            if (!levelSequence[i].completed)
            {
                currentLevel = i;
                SetScreen(GameScreen.play);
                return;
            }
        }
        if (currentLevel + 1 < levelSequence.Length)
        {
            currentLevel++;
        }
        else
        {
            currentLevel = 0;
        }
        SetScreen(GameScreen.play);
    }

    public void LevelComplete()
    {
        if (levelSequence[currentLevel].completed == false)
        {
            lilyCount++;
            levelSequence[currentLevel].completed = true;
        }
        SetScreen(GameScreen.completed);
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
