using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "FallingSettings", menuName = "Game Settings/Falling Settings")]
public class FallingSettings : ScriptableObject
{
    public AnimationCurve motionCurve;
    public AnimationCurve alphaCurve;
    public float initialHeight;
    public float duration;
}
