using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(RectTransform))]
/// <summary>
/// When the ToggleExpand() function is called, this UI element will smoothly collapse and expand its RectTransform with the configured settings.
/// </summary>
public class Expandable : MonoBehaviour
{
    [Header("Height")]
    /// <summary>
    /// The minimum height the element will collapse to. The element will start as this height.
    /// </summary>
    public float CollapsedHeight = 48f;
    /// <summary>
    /// The maximum height the element will expand to.
    /// </summary>
    public float ExpandedHeight = 144f;

    /// <summary>
    /// The settings for how the expanding/collapsing animation will be eased.
    /// </summary>
    public EaseSettings EaseSettings;

    /// <summary>
    /// The RectTransform which will toggle between collapsed and expanded when ToggleExpand() is called.
    /// </summary>
    private RectTransform rectTransform;
    /// <summary>
    /// The easing function we will use to animate the expansion/collapse. We cache it because the library maker said to.
    /// </summary>
    private EasingFunction.Function easingFunction;
    /// <summary>
    /// Whether or not this element is currently expanded. If not expanded, it is considered collapsed.
    /// </summary>
    private bool expanded = false;
    /// <summary>
    /// The Coroutine currently executing the expanding animation, if any.
    /// Can be interrupted if EaseHeight() is called.
    /// </summary>
    private Coroutine heightRoutine = null;

    /// <summary>
    /// Caches the RectTransform and easing function for later use.
    /// </summary>
    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        easingFunction = EasingFunction.GetEasingFunction(EaseSettings.EasingFunction);
    }

    /// <summary>
    /// Initializes the RectTransform's height to the collapsed height set in the inspector.
    /// </summary>
    private void Start()
    {
        rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, CollapsedHeight);
    }

    /// <summary>
    /// Toggle whether the element is expanded or collapsed.
    /// Call this via a button click or any other event or function.
    /// </summary>
    public void ToggleExpand()
    {
        expanded = !expanded;
        EaseHeight(expanded ? ExpandedHeight : CollapsedHeight);
    }

    /// <summary>
    /// Cancels the current coroutine modifying the height and triggers a new coroutine targeting a new height.
    /// </summary>
    /// <param name="targetHeight"></param>
    private void EaseHeight(float targetHeight)
    {
        if (heightRoutine != null) { StopCoroutine(heightRoutine); }
        heightRoutine = StartCoroutine(HeightCoroutine(targetHeight));
    }

    /// <summary>
    /// Updates the RectTransform's height to the given value over the duration set in the inspector.
    /// </summary>
    /// <param name="targetHeight">The height that the coroutine will get the RectTransform to when it completes.</param>
    /// <returns>An IEnumerator because it's a Coroutine.</returns>
    private IEnumerator HeightCoroutine(float targetHeight)
    {
        float t = 0f;
        float startHeight = rectTransform.sizeDelta.y;

        while (t < EaseSettings.Duration)
        {
            rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, easingFunction.Invoke(startHeight, targetHeight, t / EaseSettings.Duration));
            yield return new WaitForEndOfFrame();
            t += Time.deltaTime;
        }

        rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, targetHeight);
    }
}
