using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class PropertyPanelSizer : MonoBehaviour
{
    /// <summary>
    /// The margin from the top of the screen which the panel can expand to before activating scrollbar.
    /// </summary>
    public float TopMargin = 48f;
    /// <summary>
    /// The margin from the bottom of the screen which the panel can expand to before activating scrollbar.
    /// </summary>
    public float BottomMargin = 48f;
    /// <summary>
    /// The RectTransform for the top-level object containing all the properties.
    /// </summary>
    public RectTransform PropertiesRect;
    /// <summary>
    /// The RectTransform for the content which will be masked by the property area's viewport.
    /// </summary>
    public RectTransform PropertiesContent;

    /// <summary>
    /// The maximum height the panel will be able to reach before becoming fixed.
    /// </summary>
    private float maxHeight { get => Screen.height - TopMargin - BottomMargin; }
    /// <summary>
    /// The current height of the entire panel.
    /// </summary>
    private float height { get => rectTransform.sizeDelta.y; }
    /// <summary>
    /// The y-coordinate of the top of the panel, in screen-space coordinates.
    /// </summary>
    private float top { get => rectTransform.position.y + height / 2f; }
    /// <summary>
    /// The y-coordinate of the bottom of the panel, in screen-space coordinates.
    /// </summary>
    private float bottom { get => rectTransform.position.y - height / 2f; }
    /// <summary>
    /// Is the top of the panel past the top margin?
    /// </summary>
    private bool exceedingTop { get => top > Screen.height - TopMargin; }
    /// <summary>
    /// Is the bottom of the panel past the bottom margin?
    /// </summary>
    private bool exceedingBottom { get => bottom < BottomMargin; }
    /// <summary>
    /// The RectTransform attached to this object.
    /// </summary>
    private RectTransform rectTransform;

    /// <summary>
    /// Cache a reference to the RectTransform on this object.
    /// </summary>
    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    /// <summary>
    /// Keep the panel within the top and bottom margins, locking the size of the properties area if necessary.
    /// </summary>
    private void Update()
    {
        AdjustHeight();
        AdjustPosition();
    }

    /// <summary>
    /// Lock the panel's height so that it is never too tall to have to exceeed the margins.
    /// Locks the panel's height by locking the property area's height, which activates a scrollbar in response.
    /// </summary>
    private void AdjustHeight()
    {
        float otherElementsHeight = height - PropertiesRect.sizeDelta.y;
        float propertiesHeight = Mathf.Min(PropertiesContent.sizeDelta.y, maxHeight - otherElementsHeight);
        PropertiesRect.sizeDelta = new Vector2(PropertiesRect.sizeDelta.x, propertiesHeight);
    }

    /// <summary>
    /// Move the panel up or down to keep it within the margins.
    /// Does nothing if the panel is exceeding it's maximum height, though that should never be the case thanks to AdjustHeight().
    /// </summary>
    private void AdjustPosition()
    {
        // no point moving the panel around if there's no way it will fit
        if (height <= maxHeight)
        {
            if (exceedingTop && !exceedingBottom)
            {
                float extra = top - (Screen.height - TopMargin);
                rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x, rectTransform.anchoredPosition.y - extra);
            }
            else if (exceedingBottom && !exceedingTop)
            {
                float extra = bottom - BottomMargin;
                rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x, rectTransform.anchoredPosition.y + extra);
            }
        }
    }
}
