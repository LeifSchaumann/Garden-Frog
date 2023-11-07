using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GameSettings", menuName = "Game Settings")]
public class GameSettings : ScriptableObject
{
    [Header("Levels")]
    public string startingLevel;

    [Header("Falling Animation")]
    public AnimationCurve fallInMotion;
    public AnimationCurve fallInAlpha;
    public AnimationCurve fallOutMotion;
    public AnimationCurve fallOutAlpha;
    public float fallHeight;
    public float fallDuration;
}
