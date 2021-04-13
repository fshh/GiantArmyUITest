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

    private float maxHeight { get => Screen.height - TopMargin - BottomMargin; }
    private RectTransform rectTransform;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    private void Update()
    {
        float otherElementsHeight = rectTransform.sizeDelta.y - PropertiesRect.sizeDelta.y;
        float propertiesHeight = Mathf.Min(PropertiesContent.sizeDelta.y, maxHeight - otherElementsHeight);
        PropertiesRect.sizeDelta = new Vector2(PropertiesRect.sizeDelta.x, propertiesHeight);
    }
}
