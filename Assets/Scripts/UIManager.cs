using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class UIManager : MonoBehaviour
{
    public static UIManager main;

    public VisualTreeAsset titleUI;
    public VisualTreeAsset playUI;
    public VisualTreeAsset levelsUI;
    public VisualTreeAsset completedUI;
    public VisualTreeAsset levelTemplate;
    public VisualTreeAsset lockedLevelTemplate;

    public AnimationCurve pressScaleCurve;

    public event Action UIUpdate;
    public event Action UIClear;

    public bool isFading;

    private UIDocument uiDoc;

    private void Awake()
    {
        main = this;

        uiDoc = GetComponent<UIDocument>();
        isFading = false;
    }

    public void FadeInScreen(GameScreen screen, Action onFinish = null)
    {
        onFinish ??= () => { };

        LoadScreen(screen);
        VisualElement mainContainer = uiDoc.rootVisualElement.Q("Main"); // ALL UIS MUST HAVE A "Main" CONTAINER
        SetOpacity(mainContainer, 1f, 0.5f, onFinish);
    }

    public void FadeOutScreen(Action onFinish = null)
    {
        onFinish ??= () => { };

        VisualElement mainContainer = uiDoc.rootVisualElement.Q("Main");
        SetOpacity(mainContainer, 0f, 0.5f, onFinish);
    }

    private void LoadScreen(GameScreen screen)
    {
        ClearUI();
        Func<bool> doneTransitioning = GameManager.main.DoneTransitioning;
        switch (screen)
        {
            case GameScreen.title:
                uiDoc.visualTreeAsset = titleUI;

                Button playButton = uiDoc.rootVisualElement.Q<Button>("Play");
                new IconButton(playButton, () =>
                {
                    GameManager.main.SetScreen(GameScreen.play);
                }, doneTransitioning, KeyCode.Space);
                /*
                Button editButton = uiDoc.rootVisualElement.Q<Button>("Edit");
                new IconButton(editButton, () =>
                {

                }, doneTransitioning, KeyCode.E);
                */
                Button levelsButton = uiDoc.rootVisualElement.Q<Button>("Levels");
                new IconButton(levelsButton, () =>
                {
                    GameManager.main.SetScreen(GameScreen.levels);
                }, doneTransitioning, KeyCode.L);

                break;
            case GameScreen.play:
                uiDoc.visualTreeAsset = playUI;

                Button resetButton = uiDoc.rootVisualElement.Q<Button>("Reset");
                new IconButton(resetButton, () =>
                {
                    if (LevelManager.main.ResetLevel())
                    {
                        Spin(resetButton, 0.5f);
                    }
                }, doneTransitioning, KeyCode.R);

                Button backButton = uiDoc.rootVisualElement.Q<Button>("Back");
                new IconButton(backButton, () =>
                {
                    GameManager.main.SetScreen(GameScreen.title);
                }, doneTransitioning, KeyCode.Escape);

                levelsButton = uiDoc.rootVisualElement.Q<Button>("Levels");
                new IconButton(levelsButton, () =>
                {
                    GameManager.main.SetScreen(GameScreen.levels);
                }, doneTransitioning, KeyCode.L);

                Label levelNumber = uiDoc.rootVisualElement.Q<Label>("LevelNumber");
                levelNumber.text = (GameManager.main.currentLevel + 1).ToString();

                break;
            case GameScreen.completed:
                uiDoc.visualTreeAsset = completedUI;

                backButton = uiDoc.rootVisualElement.Q<Button>("Back");
                new IconButton(backButton, () =>
                {
                    GameManager.main.SetScreen(GameScreen.title);
                }, doneTransitioning, KeyCode.Escape);

                resetButton = uiDoc.rootVisualElement.Q<Button>("Reset");
                new IconButton(resetButton, () =>
                {
                    GameManager.main.SetScreen(GameScreen.play);
                }, doneTransitioning, KeyCode.R);

                levelsButton = uiDoc.rootVisualElement.Q<Button>("Levels");
                new IconButton(levelsButton, () =>
                {
                    GameManager.main.SetScreen(GameScreen.levels);
                }, doneTransitioning, KeyCode.L);

                Button nextButton = uiDoc.rootVisualElement.Q<Button>("Next");
                new IconButton(nextButton, () =>
                {
                    GameManager.main.NextLevel();
                }, doneTransitioning, KeyCode.Space);

                //Label completeMessage = uiDoc.rootVisualElement.Q<Label>("Title");
                //completeMessage.text = "Level " + (GameManager.main.currentLevel + 1).ToString() + " complete!";

                break;
            case GameScreen.levels:
                uiDoc.visualTreeAsset = levelsUI;

                backButton = uiDoc.rootVisualElement.Q<Button>("Back");
                new IconButton(backButton, () =>
                {
                    GameManager.main.SetScreen(GameScreen.title);
                }, doneTransitioning, KeyCode.Escape);

                Label lilyCountLabel = uiDoc.rootVisualElement.Q<Label>("LilyCountText");
                lilyCountLabel.text = GameManager.main.lilyCount.ToString();

                VisualElement levelsContainer = uiDoc.rootVisualElement.Q("LevelsContainer");
                for (int i = 0; i < GameManager.main.levelSequence.Length; i++)
                {
                    LevelData levelData = GameManager.main.levelSequence[i];
                    if (!levelData.locked)
                    {
                        VisualElement level = levelTemplate.CloneTree().Q("Level");

                        if (levelData.photo != null)
                        {
                            VisualElement image = level.Q("LevelImage");
                            image.style.backgroundImage = levelData.photo;
                        }

                        VisualElement stamp = level.Q("LilyStamp");
                        if (levelData.completed)
                        {
                            stamp.style.opacity = 1f;
                        }
                        else
                        {
                            stamp.style.opacity = 0f;
                        }
                        
                        Button button = level.Q<Button>("Button");
                        int levelIndex = i;
                        button.clicked += () => {
                            if (GameManager.main.DoneTransitioning())
                            {
                                Press(level);
                                GameManager.main.currentLevel = levelIndex;
                                GameManager.main.SetScreen(GameScreen.play);
                            }
                        };
                        if (GameManager.main.settings.showLevelNames)
                        {
                            level.Q<Label>("Title").text = levelData.json.name;
                        }
                        else
                        {
                            level.Q<Label>("Title").text = (levelIndex + 1).ToString();
                        }
                        levelsContainer.Add(level);
                    }
                    else
                    {
                        VisualElement level = lockedLevelTemplate.CloneTree().Q("Level");
                        Button button = level.Q<Button>("Button");
                        button.clicked += () => {
                            if (GameManager.main.DoneTransitioning())
                            {
                                Press(level);
                            }
                        };
                        level.Q<Label>("LilyRequirement").text = (levelData.lilyRequirement - GameManager.main.lilyCount).ToString() + " more";
                        levelsContainer.Add(level);
                    }
                }
                break;
        }
    }

    public void UpdateUI()
    {
        if (UIUpdate != null)
        {
            UIUpdate();
        }
    }

    private void ClearUI()
    {
        if (UIClear != null)
        {
            UIClear();
        }
    }

    public void SetOpacity(VisualElement element, float opacity, float duration, Action onFinish = null)
    {
        StartCoroutine(SetOpacityRoutine(element, opacity, duration, onFinish));
    }

    IEnumerator SetOpacityRoutine(VisualElement element, float opacity, float duration, Action onFinish)
    {
        onFinish ??= () => { };

        if (element == null)
        {
            onFinish();
        }
        else
        {
            isFading = true;
            float timePassed = 0;
            float startOpacity = element.style.opacity.value;
            while (timePassed < duration)
            {
                timePassed += Time.deltaTime;
                element.style.opacity = Mathf.Lerp(startOpacity, opacity, timePassed / duration);
                yield return null;
            }
            element.style.opacity = opacity;
            isFading = false;
            onFinish();
        }
    }

    public void Spin(VisualElement element, float duration, Action onFinish = null)
    {
        StartCoroutine(SpinRoutine(element, duration, onFinish));
    }

    IEnumerator SpinRoutine(VisualElement element, float duration, Action onFinish)
    {
        onFinish ??= () => { };

        if (element == null)
        {
            onFinish();
        }
        else
        {
            float timePassed = 0;
            while (timePassed < duration)
            {
                timePassed += Time.deltaTime;
                element.style.rotate = new StyleRotate(new Rotate(Mathf.Lerp(0f, 360f, timePassed / duration)));
                yield return null;
            }
            element.style.rotate = new StyleRotate(new Rotate(0f));
            onFinish();
        }
    }

    public void Press(VisualElement element, float duration = 0.35f, Action onFinish = null)
    {
        StartCoroutine(PressRoutine(element, duration, onFinish));
    }

    IEnumerator PressRoutine(VisualElement element, float duration, Action onFinish)
    {
        onFinish ??= () => { };

        if (element == null)
        {
            onFinish();
        }
        else
        {
            float timePassed = 0;
            while (timePassed < duration)
            {
                timePassed += Time.deltaTime;
                float scaleFactor = pressScaleCurve.Evaluate(timePassed / duration);
                element.style.scale = new StyleScale(new Scale(new Vector2(scaleFactor, scaleFactor)));
                yield return null;
            }
            element.style.scale = new StyleScale(new Scale(new Vector2(1, 1)));
            onFinish();
        }
    }
}
