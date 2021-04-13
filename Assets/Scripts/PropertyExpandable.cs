using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class PropertyExpandable : MonoBehaviour
{
    public float CollapsedHeight = 48f;
    public float ExpandedHeight = 144f;
    public float EaseDuration = 1f;
    public Ease EaseType;

    private RectTransform rectTransform;
    private bool expanded = false;
    private Coroutine expansionRoutine = null;
    private EasingFunction.Function easingFunction;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        easingFunction = EasingFunction.GetEasingFunction(EaseType);
    }

    private void Start()
    {
        rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, CollapsedHeight);
    }

    public void ToggleExpand()
    {
        expanded = !expanded;
        EaseToHeight(expanded ? ExpandedHeight : CollapsedHeight);
    }

    private void EaseToHeight(float targetHeight)
    {
        if (expansionRoutine != null) { StopCoroutine(expansionRoutine); }
        float startHeight = rectTransform.sizeDelta.y;
        expansionRoutine = StartCoroutine(EaseCoroutine(startHeight, targetHeight));
    }

    private IEnumerator EaseCoroutine(float startHeight, float targetHeight)
    {
        float t = 0f;

        while (t < EaseDuration)
        {
            rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, easingFunction.Invoke(startHeight, targetHeight, t / EaseDuration));
            yield return new WaitForEndOfFrame();
            t += Time.deltaTime;
        }

        rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, targetHeight);
    }
}
