using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public string startingLevel;

    private CameraMovement camMovement;

    private void Awake()
    {
        instance = this;

        camMovement = Camera.main.GetComponent<CameraMovement>();
    }

    private void Start()
    {
        LevelManager.instance.LoadLevel(startingLevel);
        camMovement.CenterOnLevel();
    }
}
