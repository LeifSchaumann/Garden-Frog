using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class IconButton
{
    // Class to control the behavior of a simple button

    private Button button;
    private bool hoveredOver;
    private float targetOpacity;
    private Func<bool> activeCondition;
    private Action onClick;
    private Action action;
    private KeyCode hotKeyCode;

    public IconButton(Button button, Action action = null, Func<bool> activeCondition = null, KeyCode hotKeyCode = KeyCode.None)
    {
        this.button = button;
        this.hoveredOver = false;
        this.hotKeyCode = hotKeyCode;
        this.action = action ?? (() => { });
        this.activeCondition = activeCondition ?? (() => { return true; });
        this.onClick = () =>
        {
            if (this.activeCondition())
            {
                this.action();
                UIManager.main.Press(button);
            }
        };

        button.RegisterCallback<MouseEnterEvent>((MouseEnterEvent) => { this.hoveredOver = true; });
        button.RegisterCallback<MouseLeaveEvent>((MouseLeaveEvent) => { this.hoveredOver = false; });
        button.clicked += onClick;
        UIManager.main.UIUpdate += Update;
        UIManager.main.UIClear += Clear;
    }

    private void Clear()
    {
        UIManager.main.UIUpdate -= Update;
        UIManager.main.UIClear -= Clear;
        button.clicked -= onClick;
    }

    public void Update()
    {
        if (Input.GetKeyDown(hotKeyCode))
        {
            onClick();
        }
        if (activeCondition() || UIManager.main.isFading)
        {
            if (hoveredOver && !UIManager.main.isFading)
            {
                targetOpacity = 1f;
            }
            else
            {
                targetOpacity = 0.5f;
            }
        }
        else
        {
            targetOpacity = 0.2f;
        }
        button.style.opacity = Mathf.Lerp(button.style.opacity.value, targetOpacity, 0.03f);
    }
}
