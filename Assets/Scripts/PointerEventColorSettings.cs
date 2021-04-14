using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[System.Serializable]
/// <summary>
/// The types of pointer events that PointerEventColorSettings can respond to.
/// </summary>
public enum PointerEventType
{
    Hover,
    ToggleSelect,
    Pressed
}

[System.Serializable]
/// <summary>
/// A setting which is given via the inspector, saying that the image should become the given color when the  given event type happens.
/// </summary>
public class PointerEventSetting
{
    public PointerEventType eventType;
    public Color color = Color.white;
}

/// <summary>
/// Component that updates a target image's color based on settings which respond to pointer events from this object.
/// </summary>
public class PointerEventColorSettings : MonoBehaviour,
    IPointerClickHandler, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler
    {
        /// <summary>
        /// The image whose color should be adjusted in response to pointer events on this object.
        /// Will try to get an image from this object if none is provided.
        /// </summary>
        public Image TargetImage;
        /// <summary>
        /// The default color the image should revert to if no event conditions are met.
        /// </summary>
        public Color DefaultColor;
        /// <summary>
        /// An ordered list of settings saying what color the image should be for each pointer event.
        /// These settings are prioritized based on their ordering, with element 0 having highest priority.
        /// </summary>
        public List<PointerEventSetting> EventPriorities = new List<PointerEventSetting>();
        /// <summary>
        /// Settings for how the color easing should behave.
        /// </summary>
        public EaseSettings EaseSettings;

        /// <summary>
        /// The image to be manipulated.
        /// </summary>
        private Image image;
        /// <summary>
        /// The easing function we will use to animate the color change. We cache it because the library maker said to.
        /// </summary>
        private EasingFunction.Function easingFunction;
        /// <summary>
        /// The Coroutine currently executing the color change animation, if any.
        /// Can be interrupted if EaseColor() is called.
        /// </summary>
        private Coroutine colorRoutine = null;
        /// <summary>
        /// Is this object being hovered over by the mouse?
        /// </summary>
        private bool hovered = false;
        /// <summary>
        /// Has this object been toggle-selected by the mouse?
        /// </summary>
        private bool selected = false;
        /// <summary>
        /// Is this object currently being pressed by the mouse?
        /// </summary>
        private bool pressed = false;

        /// <summary>
        /// Cache the target image and easing function for use later.
        /// </summary>
        private void Awake()
        {
            image = (TargetImage == null) ? GetComponent<Image>() : TargetImage;
            easingFunction = EasingFunction.GetEasingFunction(EaseSettings.EasingFunction);
        }

        /// <summary>
        /// Initially set the color to default, then try updating it based on settings.
        /// </summary>
        private void Start()
        {
            image.color = DefaultColor;
            UpdateColor();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            selected = !selected;
            UpdateColor();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            pressed = true;
            UpdateColor();
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            pressed = false;
            UpdateColor();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            hovered = true;
            UpdateColor();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            hovered = false;
            UpdateColor();
        }

        /// <summary>
        /// Update the target image's color depending on which of the PointerEventSettings' conditions is met first.
        /// </summary>
        private void UpdateColor()
        {
            Color color = DefaultColor;
            foreach (PointerEventSetting setting in EventPriorities)
            {
                PointerEventType eventType = setting.eventType;
                if (eventType == PointerEventType.Hover && hovered ||
                    eventType == PointerEventType.ToggleSelect && selected ||
                    eventType == PointerEventType.Pressed && pressed)
                {
                    color = setting.color;
                    break;
                }
            }
            EaseColor(color);
        }

        /// <summary>
        /// Start a coroutine to ease the target image's color to the given value.
        /// Cancels any existing coroutine modifying the color before starting.
        /// </summary>
        /// <param name="targetColor">The color to set on the target image.</param>
        private void EaseColor(Color targetColor)
        {
            if (colorRoutine != null) { StopCoroutine(colorRoutine); }
            colorRoutine = StartCoroutine(ColorCoroutine(targetColor));
        }

        /// <summary>
        /// Ease the target image's color to the given value smoothly over a duration based on the ease settings.
        /// </summary>
        /// <param name="targetColor">The color to set on the target image.</param>
        /// <returns>An IEnumerator because it's a Coroutine.</returns>
        private IEnumerator ColorCoroutine(Color targetColor)
        {
            float t = 0f;
            Color startColor = image.color;

            while (t < EaseSettings.Duration)
            {
                image.color = Color.Lerp(startColor, targetColor, easingFunction.Invoke(0f, 1f, t / EaseSettings.Duration));
                yield return new WaitForEndOfFrame();
                t += Time.deltaTime;
            }

            image.color = targetColor;
        }
    }
