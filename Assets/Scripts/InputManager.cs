using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public static InputManager instance;
    public float inputForgiveness;

    private float lastInputTime;
    private Vector2Int lastInputDir;

    private void Awake()
    {
        instance = this;
    }
    public void AcceptInput()
    {
        Vector2Int inputDir = Vector2Int.zero;
        if (Input.GetKeyDown(KeyCode.W))
        {
            inputDir = Vector2Int.up;
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            inputDir = Vector2Int.left;
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            inputDir = Vector2Int.down;
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            inputDir = Vector2Int.right;
        }

        if (inputDir != Vector2Int.zero)
        {
            lastInputDir = inputDir;
            lastInputTime = Time.time;
        }

        if (Time.time < lastInputTime + inputForgiveness) { 
            inputDir = lastInputDir;
        }

        if (inputDir != Vector2Int.zero)
        {
            LevelManager.instance.Move(inputDir);
        }
    }
}
