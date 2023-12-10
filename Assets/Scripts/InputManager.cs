using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    // Tracks the player's input and includes an input forgiveness system

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
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            inputDir = Vector2Int.up;
        }
        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            inputDir = Vector2Int.left;
        }
        if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            inputDir = Vector2Int.down;
        }
        if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
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
            LevelManager.main.Move(inputDir);
        }
    }
}
