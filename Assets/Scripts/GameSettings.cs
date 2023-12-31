using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GameSettings", menuName = "Game Settings")]
public class GameSettings : ScriptableObject
{
    // Scriptable object to store all of the game settings in a centralized place

    public bool saveThumbnails;
    public bool unlockAllLevels;
    public bool showLevelNames;
    
    [Header("Levels")]
    public TextAsset[] levelSequence;

    [Header("Falling Animation")]
    public AnimationCurve fallInMotion;
    public AnimationCurve fallInAlpha;
    public AnimationCurve fallOutMotion;
    public AnimationCurve fallOutAlpha;
    public float fallHeight;
    public float fallDuration;

    [Header("Level Settings")]
    public GameObject frogPrefab;
    public GameObject waterPrefab;
    public GameObject lilyPadPrefab;
    public GameObject rockPrefab;
    public GameObject goalPrefab;
    public GameObject wallPrefab;
    public GameObject logPrefab;
    public float stepHeight;

    [Header("Camera Settings")]
    public Vector3 viewDirection;
    public AnimationCurve zoomCurve;

    [Header("Colors")]
    public Color algaeColor;
    public Color shallowColor;
}
