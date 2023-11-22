using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEditor.Rendering.FilterWindow;

public enum UIInput
{
    reset,
    back
}

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    public VisualTreeAsset titleUI;
    public VisualTreeAsset playUI;

    public event Action UIUpdate;
    public event Action UIClear;

    private UIDocument uiDoc;

    private void Awake()
    {
        instance = this;

        uiDoc = GetComponent<UIDocument>();
    }

    public void FadeInScreen(GameScreen screen, Action onFinish = null)
    {
        onFinish ??= () => { };

        LoadScreen(screen);
        VisualElement mainContainer = uiDoc.rootVisualElement.Q("Main");
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
        switch (screen)
        {
            case GameScreen.title:
                uiDoc.visualTreeAsset = titleUI;
                break;
            case GameScreen.play:
                uiDoc.visualTreeAsset = playUI;

                Func<bool> levelNotUpdating = () => { return !LevelManager.main.IsUpdating(); };

                Button resetButton = uiDoc.rootVisualElement.Q<Button>("Reset");
                new IconButton(resetButton, () =>
                {
                    if (LevelManager.main.ResetLevel())
                    {
                        Spin(resetButton, 0.5f);
                    }
                }, levelNotUpdating, KeyCode.R);

                Button backButton = uiDoc.rootVisualElement.Q<Button>("Back");
                new IconButton(backButton, () =>
                {
                    GameManager.instance.SetScreen(GameScreen.title);
                }, levelNotUpdating, KeyCode.Escape);

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
            float timePassed = 0;
            float startOpacity = element.style.opacity.value;
            while (timePassed < duration)
            {
                timePassed += Time.deltaTime;
                element.style.opacity = Mathf.Lerp(startOpacity, opacity, timePassed / duration);
                yield return null;
            }
            element.style.opacity = opacity;
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
