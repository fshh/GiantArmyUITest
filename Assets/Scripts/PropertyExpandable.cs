using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
[RequireComponent(typeof(Image))]
public class PropertyExpandable : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Height")]
    public float CollapsedHeight = 48f;
    public float ExpandedHeight = 144f;

    [Header("Color")]
    public Color CollapsedColor = Color.HSVToRGB(0f, 0f, 0.07f);
    public Color ExpandedColor = Color.HSVToRGB(0f, 0f, 0.17f);
    public Color HoveredColor = Color.HSVToRGB(0f, 0f, 0.12f);

    [Header("Easing")]
    public float EaseDuration = 1f;
    public Ease EaseType;

    private RectTransform rectTransform;
    private Image image;
    private EasingFunction.Function easingFunction;
    private bool expanded = false;
    private Coroutine heightRoutine = null;
    private Coroutine colorRoutine = null;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        image = GetComponent<Image>();
        easingFunction = EasingFunction.GetEasingFunction(EaseType);
    }

    private void Start()
    {
        rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, CollapsedHeight);
        image.color = CollapsedColor;
    }

    public void ToggleExpand()
    {
        expanded = !expanded;
        EaseVisual(expanded ? ExpandedHeight : CollapsedHeight, expanded ? ExpandedColor : CollapsedColor);
    }

    private void EaseVisual(float targetHeight, Color targetColor)
    {
        EaseHeight(targetHeight);
        EaseColor(targetColor);
    }

    private void EaseHeight(float targetHeight)
    {
        if (heightRoutine != null) { StopCoroutine(heightRoutine); }
        heightRoutine = StartCoroutine(HeightCoroutine(targetHeight));
    }

    private IEnumerator HeightCoroutine(float targetHeight)
    {
        float t = 0f;
        float startHeight = rectTransform.sizeDelta.y;

        while (t < EaseDuration)
        {
            rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, easingFunction.Invoke(startHeight, targetHeight, t / EaseDuration));
            yield return new WaitForEndOfFrame();
            t += Time.deltaTime;
        }

        rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, targetHeight);
    }

    private void EaseColor(Color targetColor)
    {
        if (colorRoutine != null) { StopCoroutine(colorRoutine); }
        colorRoutine = StartCoroutine(ColorCoroutine(targetColor));
    }

    private IEnumerator ColorCoroutine(Color targetColor)
    {
        float t = 0f;
        Color startColor = image.color;

        while (t < EaseDuration)
        {
            image.color = Color.Lerp(startColor, targetColor, easingFunction.Invoke(0f, 1f, t / EaseDuration));
            yield return new WaitForEndOfFrame();
            t += Time.deltaTime;
        }

        image.color = targetColor;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!expanded)
        {
            EaseColor(HoveredColor);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!expanded)
        {
            EaseColor(CollapsedColor);
        }
    }
}
