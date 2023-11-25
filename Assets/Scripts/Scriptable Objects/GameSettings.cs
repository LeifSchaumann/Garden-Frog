using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GameSettings", menuName = "Game Settings")]
public class GameSettings : ScriptableObject
{
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
    public float stepHeight;

    [Header("Camera Settings")]
    public Vector3 viewDirection;
    public AnimationCurve zoomCurve;
}
