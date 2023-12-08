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
    public bool unlockLevels;

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
            if (unlockLevels)
            {
                levelSequence[i].locked = false;
            }
        }
    }

    private void Start()
    {
        ChainOperation((int i, Action onFinish) =>
        {
            Photographer.main.PhotographLevel(levelSequence[i], onFinish);
        }, 0, levelSequence.Length, () => { 
            SetScreen(GameScreen.title);
        });
    }

    private void ChainOperation(Action<int, Action> action, int i, int maxI, Action onFinish)
    {
        if (i < maxI)
        {
            action(i, () =>
            {
                ChainOperation(action, i + 1, maxI, onFinish);
            });
        }
        else
        {
            onFinish();
        }
    }

    public void SetScreen(GameScreen screen)
    {
        switch (screen)
        {
            case GameScreen.title:
                if (currentScreen == GameScreen.none)
                {
                    LevelManager.main.AddUpdate(new LevelUpdate.Load(levelSequence[currentLevel], false, onDefined: () => {
                        camMovement.SetZoom(true, 0.5f);
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
                            camMovement.SetZoom(true, 0.5f);
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
                    camMovement.SetZoom(false, 1f, () =>
                    {
                        UIManager.main.FadeInScreen(screen);
                    });
                }
                else
                {
                    UIManager.main.FadeOutScreen(() =>
                    {
                        LevelManager.main.AddUpdate(new LevelUpdate.Load(levelSequence[currentLevel], false, onDefined: () => {
                            camMovement.SetZoom(true, 1f);
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
                camMovement.SetZoom(false, 0.65f, () =>
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
            if (!levelSequence[i].completed && !levelSequence[i].locked)
            {
                currentLevel = i;
                SetScreen(GameScreen.play);
                return;
            }
        }
        for (int i = 0; i < currentLevel; i++)
        {
            if (!levelSequence[i].completed && !levelSequence[i].locked)
            {
                currentLevel = i;
                SetScreen(GameScreen.play);
                return;
            }
        }
        if (currentLevel + 1 < levelSequence.Length)
        {
            if (!levelSequence[currentLevel + 1].locked)
            {
                currentLevel++;
                SetScreen(GameScreen.play);
                return;
            }
        }
        currentLevel = 0;
        SetScreen(GameScreen.play);
    }

    public void LevelComplete()
    {
        if (levelSequence[currentLevel].completed == false)
        {
            lilyCount++;
            levelSequence[currentLevel].completed = true;
            for (int i = 0; i < levelSequence.Length; i++)
            {
                if (levelSequence[i].lilyRequirement <= lilyCount)
                {
                    levelSequence[i].locked = false;
                }
            }
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
