using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    public VisualTreeAsset titleUI;
    public VisualTreeAsset playUI;

    private UIDocument uiDoc;
    //private Dictionary<GameScreen, VisualTreeAsset> screenToVTA;

    private void Awake()
    {
        instance = this;

        uiDoc = GetComponent<UIDocument>();
        //screenToVTA = new Dictionary<GameScreen, VisualTreeAsset> { {GameScreen.title, titleUI}, {GameScreen.play, playUI} };
    }

    public void FadeInScreen(GameScreen screen, Action onFinish = null)
    {
        onFinish ??= () => { };

        LoadScreen(screen);
        VisualElement mainContainer = uiDoc.rootVisualElement.Q("Main");
        StartCoroutine(SetOpacity(mainContainer, 1f, 1f, onFinish));
    }

    public void FadeOutScreen(Action onFinish = null)
    {
        onFinish ??= () => { };

        VisualElement mainContainer = uiDoc.rootVisualElement.Q("Main");
        StartCoroutine(SetOpacity(mainContainer, 0f, 1f, onFinish));
    }

    private void LoadScreen(GameScreen screen)
    {
        switch (screen)
        {
            case GameScreen.title:
                uiDoc.visualTreeAsset = titleUI;
                break;
            case GameScreen.play:
                uiDoc.visualTreeAsset = playUI;
                Button resetButton = uiDoc.rootVisualElement.Q<Button>("Reset");
                resetButton.RegisterCallback<MouseEnterEvent>((MouseEnterEvent) => { resetButton.style.opacity = 1f; });
                resetButton.RegisterCallback<MouseLeaveEvent>((MouseLeaveEvent) => { resetButton.style.opacity = 0.5f; });
                resetButton.clicked += () =>
                {
                    UIInput(() =>
                    {
                        LevelManager.main.ResetLevel();
                    });
                };
                break;
        }
    }

    public void UIInput(Action action)
    {
        if (!GameManager.instance.transitioning)
        {
            action();
        }
    }

    IEnumerator SetOpacity(VisualElement element, float opacity, float duration, Action onFinish = null)
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
}
