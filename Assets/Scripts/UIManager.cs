using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    public VisualTreeAsset titleUI;

    private UIDocument uiDoc;

    private void Awake()
    {
        instance = this;

        uiDoc = GetComponent<UIDocument>();
    }

    public void ChangeScreen(GameScreen screen)
    {
        VisualElement mainContainer;
        switch (screen)
        {
            case GameScreen.title:
                uiDoc.visualTreeAsset = titleUI;
                mainContainer = uiDoc.rootVisualElement.Q("Main");
                StartCoroutine(FadeIn(mainContainer, 1f));
                break;
            case GameScreen.play:
                mainContainer = uiDoc.rootVisualElement.Q("Main");
                StartCoroutine(FadeOut(mainContainer, 1f));
                break;
        }
    }

    IEnumerator FadeOut(VisualElement element, float time)
    {
        float timePassed = 0;
        while (timePassed < time)
        {
            timePassed += Time.deltaTime;
            element.style.opacity = 1 - timePassed/time;
            yield return null;
        }
        element.style.opacity = 0;
    }

    IEnumerator FadeIn(VisualElement element, float time)
    {
        float timePassed = 0;
        while (timePassed < time)
        {
            timePassed += Time.deltaTime;
            element.style.opacity = timePassed / time;
            yield return null;
        }
        element.style.opacity = 1f;
    }
}
