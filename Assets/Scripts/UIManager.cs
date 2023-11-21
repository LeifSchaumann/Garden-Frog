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
    private Dictionary<GameScreen, VisualTreeAsset> screenToVTA;

    private void Awake()
    {
        instance = this;

        uiDoc = GetComponent<UIDocument>();
        screenToVTA = new Dictionary<GameScreen, VisualTreeAsset> { {GameScreen.title, titleUI}, {GameScreen.play, playUI} };
    }

    public void FadeInScreen(GameScreen screen, Action onFinish = null)
    {
        onFinish ??= () => { };

        uiDoc.visualTreeAsset = screenToVTA[screen];
        VisualElement mainContainer = uiDoc.rootVisualElement.Q("Main");
        StartCoroutine(FadeIn(mainContainer, 1f, onFinish));
    }

    public void FadeOutScreen(Action onFinish = null)
    {
        onFinish ??= () => { };

        VisualElement mainContainer = uiDoc.rootVisualElement.Q("Main");
        StartCoroutine(FadeOut(mainContainer, 1f, onFinish));
    }

    IEnumerator FadeOut(VisualElement element, float time, Action onFinish = null)
    {
        onFinish ??= () => { };

        if (element == null)
        {
            onFinish();
        }
        else
        {
            float timePassed = 0;
            while (timePassed < time)
            {
                timePassed += Time.deltaTime;
                element.style.opacity = 1 - timePassed / time;
                yield return null;
            }
            element.style.opacity = 0;
            onFinish();
        }
    }

    IEnumerator FadeIn(VisualElement element, float time, Action onFinish = null)
    {
        onFinish ??= () => { };

        float timePassed = 0;
        while (timePassed < time)
        {
            timePassed += Time.deltaTime;
            element.style.opacity = timePassed / time;
            yield return null;
        }
        element.style.opacity = 1f;
        onFinish();
    }
}
