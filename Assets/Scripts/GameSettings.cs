using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewGameSettings", menuName = "Game/Game Settings")]
public class GameSettings : ScriptableObject
{
    public string startingLevel;

    public AnimationCurve fallingCurve;
    public AnimationCurve fallingCurveAlpha;
    public float fallingInitialHeight;
    public float fallingDuration;
}
