using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class IconButton
{
    private Button button;
    private bool hoveredOver;
    private Func<bool> activeCondition;

    public IconButton(Button button, Action onClick, Func<bool> activeCondition = null)
    {
        this.button = button;
        this.hoveredOver = false;
        this.activeCondition = activeCondition ?? (() => { return true; });

        button.RegisterCallback<MouseEnterEvent>((MouseEnterEvent) => { this.hoveredOver = true; });
        button.RegisterCallback<MouseLeaveEvent>((MouseLeaveEvent) => { this.hoveredOver = false; });
        button.clicked += () =>
        {
            if (!GameManager.instance.transitioning && activeCondition())
            {
                onClick();
            }
        };
        GameManager.instance.UIUpdate += Update;
    }

    public void Update()
    {
        if (activeCondition())
        {
            if (hoveredOver)
            {
                button.style.opacity = 1f;
            }
            else
            {
                button.style.opacity = 0.5f;
            }
        }
        else
        {
            button.style.opacity = 0.2f;
        }
    }
}
