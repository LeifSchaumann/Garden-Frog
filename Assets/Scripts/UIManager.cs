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
        switch (screen)
        {
            case GameScreen.title:
                uiDoc.visualTreeAsset = titleUI;
                break;
        }
    }
}
