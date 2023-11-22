using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class IconButton
{
    private Button button;
    private bool hoveredOver;
    private float targetOpacity;
    private Func<bool> activeCondition;
    private Action onClick;
    private KeyCode hotKeyCode;

    public IconButton(Button button, Action action, Func<bool> activeCondition = null, KeyCode hotKeyCode = KeyCode.None)
    {
        this.button = button;
        this.hoveredOver = false;
        this.hotKeyCode = hotKeyCode;
        this.onClick = () =>
        {
            if (!GameManager.instance.transitioning && activeCondition())
            {
                action();
            }
        };
        this.activeCondition = activeCondition ?? (() => { return true; });

        button.RegisterCallback<MouseEnterEvent>((MouseEnterEvent) => { this.hoveredOver = true; });
        button.RegisterCallback<MouseLeaveEvent>((MouseLeaveEvent) => { this.hoveredOver = false; });
        button.clicked += onClick;
        UIManager.instance.UIUpdate += Update;
        UIManager.instance.UIClear += Clear;
    }

    private void Clear()
    {
        UIManager.instance.UIUpdate -= Update;
        UIManager.instance.UIClear -= Clear;
        button.clicked -= onClick;
    }

    public void Update()
    {
        if (Input.GetKeyDown(hotKeyCode))
        {
            onClick();
        }
        if (activeCondition())
        {
            if (hoveredOver)
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
