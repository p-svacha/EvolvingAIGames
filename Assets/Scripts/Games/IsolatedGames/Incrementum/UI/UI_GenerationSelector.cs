using System;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

namespace Incrementum
{
    public class UI_GenerationSelector : MonoBehaviour
    {
        private UI_Incrementum UI;

        public int SelectedGenerationIndex { get; private set; }
        public int MaxGenerationIndex { get; private set; }

        [Header("Elements")]
        public TextMeshProUGUI GenerationLabel;
        public GameObject SelectorBackgroundBar; // should have an Image with Raycast Target enabled
        public GameObject DraggableSelector;      // handle (grey), child of the bar

        // cached
        private RectTransform _barRT;
        private RectTransform _handleRT;
        private Image _barImage;
        private Image _handleImage;
        private Camera _uiCam; // null uses Screen Space - Overlay

        public void Init(UI_Incrementum ui)
        {
            UI = ui;

            _barRT = SelectorBackgroundBar.GetComponent<RectTransform>();
            _handleRT = DraggableSelector.GetComponent<RectTransform>();
            _barImage = SelectorBackgroundBar.GetComponent<Image>();
            _handleImage = DraggableSelector.GetComponent<Image>();
            if (_barImage != null) _barImage.raycastTarget = true;
            if (_handleImage != null) _handleImage.raycastTarget = true;

            // Attach event listeners programmatically (no need to set up EventTrigger in the inspector)
            AddPointerHandlers(SelectorBackgroundBar,
                onDown: (e) => JumpToPointer(e),
                onDrag: null);

            AddPointerHandlers(DraggableSelector,
                onDown: (e) => JumpToPointer(e), // clicking handle also snaps
                onDrag: (e) => JumpToPointer(e));

            // start at 0
            SetMaxGenerationIndex(0, selectNewMax: false);
            SetValue(0, notify: false);
        }

        public void SetMaxGenerationIndex(int value, bool selectNewMax = true)
        {
            MaxGenerationIndex = Mathf.Max(0, value);

            if (selectNewMax)
            {
                // if current selection exceeds new max, clamp
                int clamped = Mathf.Clamp(SelectedGenerationIndex, 0, MaxGenerationIndex);

                // UX: by default, snap to the latest generation when max increases
                if (value > 0) clamped = MaxGenerationIndex;

                SetValue(clamped, notify: true);
            }

            UpdateLabel();
            UpdateHandlePosition();
        }

        public void SetValue(int value, bool notify)
        {
            value = Mathf.Clamp(value, 0, MaxGenerationIndex);
            if (value == SelectedGenerationIndex && !notify) return;

            SelectedGenerationIndex = value;
            UpdateLabel();
            UpdateHandlePosition();

            if (notify && UI != null)
                UI.DisplayGenerationStats(value);
        }

        private void UpdateLabel()
        {
            if (GenerationLabel != null)
                GenerationLabel.text = $"Generation {SelectedGenerationIndex} / {MaxGenerationIndex}";
        }

        private void UpdateHandlePosition()
        {
            if (_barRT == null || _handleRT == null) return;

            float t = (MaxGenerationIndex == 0) ? 0f : (float)SelectedGenerationIndex / MaxGenerationIndex;

            var bar = _barRT.rect;
            float w = _handleRT.rect.width;
            float px = _handleRT.pivot.x;

            float minCenter = bar.xMin + w * px;           // left edge aligned at t=0
            float maxCenter = bar.xMax - w * (1f - px);    // right edge aligned at t=1

            float x = Mathf.Lerp(minCenter, maxCenter, t);
            _handleRT.anchoredPosition = new Vector2(x, _handleRT.anchoredPosition.y);
        }

        private void JumpToPointer(PointerEventData e)
        {
            if (_barRT == null || _handleRT == null) return;

            Vector2 local;
            if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(_barRT, e.position, _uiCam, out local))
                return;

            var bar = _barRT.rect;
            float w = _handleRT.rect.width;
            float px = _handleRT.pivot.x;

            float minCenter = bar.xMin + w * px;
            float maxCenter = bar.xMax - w * (1f - px);

            // Clamp the pointer to the permitted center range so edges align
            float clampedX = Mathf.Clamp(local.x, minCenter, maxCenter);
            float t = (minCenter == maxCenter) ? 0f : Mathf.InverseLerp(minCenter, maxCenter, clampedX);

            int idx = Mathf.RoundToInt(t * MaxGenerationIndex);
            SetValue(idx, notify: true);
        }

        // Helper to wire pointer events without needing EventTrigger in the inspector
        private void AddPointerHandlers(GameObject go,
            Action<PointerEventData> onDown,
            Action<PointerEventData> onDrag)
        {
            var trigger = go.GetComponent<EventTrigger>();
            if (trigger == null) trigger = go.AddComponent<EventTrigger>();

            if (onDown != null)
            {
                var entry = new EventTrigger.Entry { eventID = EventTriggerType.PointerDown };
                entry.callback.AddListener((data) => onDown((PointerEventData)data));
                trigger.triggers.Add(entry);
            }

            if (onDrag != null)
            {
                var entry = new EventTrigger.Entry { eventID = EventTriggerType.Drag };
                entry.callback.AddListener((data) => onDrag((PointerEventData)data));
                trigger.triggers.Add(entry);
            }

            // Also handle clicks on the bar (PointerClick) to jump
            if (go == SelectorBackgroundBar)
            {
                var click = new EventTrigger.Entry { eventID = EventTriggerType.PointerClick };
                click.callback.AddListener((data) => onDown?.Invoke((PointerEventData)data));
                trigger.triggers.Add(click);
            }
        }

        private void OnRectTransformDimensionsChange()
        {
            // Re-align to edges if layout/scale changes
            UpdateHandlePosition();
        }
    }
}
