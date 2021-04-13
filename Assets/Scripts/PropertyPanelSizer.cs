using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class PropertyPanelSizer : MonoBehaviour
{
    public float TopMargin = 48f;
    public float BottomMargin = 48f;
    public RectTransform PropertiesRect;
    public RectTransform PropertiesContent;

    private float maxHeight { get => ((RectTransform)rectTransform.parent).sizeDelta.y - TopMargin - BottomMargin; }
    private RectTransform rectTransform;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    private void Update()
    {
        float otherElementsHeight = rectTransform.sizeDelta.y - PropertiesRect.sizeDelta.y;
        PropertiesRect.sizeDelta = new Vector2(PropertiesRect.sizeDelta.x, Mathf.Min(PropertiesContent.sizeDelta.y, maxHeight - otherElementsHeight));
    }
}
