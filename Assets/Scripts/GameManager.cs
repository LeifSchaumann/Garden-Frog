using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public FallingSettings fallingSettings;
    public MainSettings settings;

    private CameraMovement camMovement;

    private void Awake()
    {
        instance = this;

        camMovement = Camera.main.GetComponent<CameraMovement>();
    }

    private void Start()
    {
        LevelManager.instance.LoadLevel(settings.startingLevel);
        camMovement.CenterOnLevel();
    }
}
