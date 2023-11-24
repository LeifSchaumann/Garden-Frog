using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEditor.Rendering.FilterWindow;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    public VisualTreeAsset titleUI;
    public VisualTreeAsset playUI;
    public VisualTreeAsset levelsUI;

    public event Action UIUpdate;
    public event Action UIClear;

    public bool isFading;

    private UIDocument uiDoc;

    private void Awake()
    {
        instance = this;

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
        Func<bool> doneTransitioning = GameManager.instance.DoneTransitioning;
        switch (screen)
        {
            case GameScreen.title:
                uiDoc.visualTreeAsset = titleUI;

                Button playButton = uiDoc.rootVisualElement.Q<Button>("Play");
                new IconButton(playButton, () =>
                {
                    GameManager.instance.SetScreen(GameScreen.play);
                }, doneTransitioning, KeyCode.Space);

                Button editButton = uiDoc.rootVisualElement.Q<Button>("Edit");
                new IconButton(editButton, () =>
                {

                }, doneTransitioning, KeyCode.E);

                Button levelsButton = uiDoc.rootVisualElement.Q<Button>("Levels");
                new IconButton(levelsButton, () =>
                {
                    GameManager.instance.SetScreen(GameScreen.levels);
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
                    GameManager.instance.SetScreen(GameScreen.title);
                }, doneTransitioning, KeyCode.Escape);
                
                break;
            case GameScreen.levels:
                uiDoc.visualTreeAsset = levelsUI;

                backButton = uiDoc.rootVisualElement.Q<Button>("Back");
                new IconButton(backButton, () =>
                {
                    GameManager.instance.SetScreen(GameScreen.title);
                }, doneTransitioning, KeyCode.Escape);
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
}
