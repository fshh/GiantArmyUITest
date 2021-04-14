using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
/// <summary>
/// A simple class for easily repeatable configuring of easing functions, potentially via the inspector.
/// </summary>
public class EaseSettings
{
    /// <summary>
    /// How long should the ease last for?
    /// </summary>
    public float Duration = 1f;
    /// <summary>
    /// What type of easing function should be used?
    /// </summary>
    public Ease EasingFunction = Ease.EaseInOutQuad;
}
