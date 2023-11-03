using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            LevelManager.instance.Move(Vector2Int.up);
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            LevelManager.instance.Move(Vector2Int.left);
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            LevelManager.instance.Move(Vector2Int.down);
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            LevelManager.instance.Move(Vector2Int.right);
        }
    }
}
