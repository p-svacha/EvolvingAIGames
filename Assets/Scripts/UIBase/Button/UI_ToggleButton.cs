using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

/// <summary>
/// Button that can also act as a toggle, with fully script-driven colors and interactability.
/// - Ignores Image/Button color settings (Button.transition is forced to None).
/// - Interactability is controlled exclusively via this script (IsInteractable).
/// </summary>
[DisallowMultipleComponent]
[RequireComponent(typeof(Image), typeof(Button))]
public class UI_ToggleButton : MonoBehaviour,
    IPointerEnterHandler, IPointerExitHandler,
    IPointerDownHandler, IPointerUpHandler
{
    [Header("Elements")]
    public TextMeshProUGUI LabelText;
    public Image Icon;
    [SerializeField] private Image targetImage;
    [SerializeField] private Button Button;
    private CanvasGroup canvasGroup;

    [Header("Toggle Mode")]
    [Tooltip("If true, clicking toggles the 'on' state and fires OnToggle.")]
    [SerializeField] private bool actsAsToggle = false;
    [Tooltip("Initial toggle state if Acts As Toggle is enabled.")]
    [SerializeField] private bool isToggled = false;

    /// <summary>
    /// Current toggle state.
    /// Setting this property raises <see cref="OnToggle"/> and refreshes visuals.
    /// If you need to change without notifying, call SetToggled(value, invokeEvent: false).
    /// </summary>
    public bool IsToggled
    {
        get => isToggled;
        set => SetToggled(value, invokeEvent: true);
    }

    [Header("Interactability")]
    [Tooltip("Initial interactable state (script-controlled).")]
    [SerializeField] private bool IsInteractable = true;

    [Header("Colors (script-controlled)")]
    [Tooltip("Default color when idle.")]
    [SerializeField] private Color BaseColor = Color.white;
    [Tooltip("Color when the pointer is over.")]
    [SerializeField] private Color HoveredColor = Color.white;
    [Tooltip("Color while pressed or toggled.")]
    [SerializeField] private Color PressedColor = Color.white;

    public event Action<bool> OnToggle;

    private bool isPointerOver;
    private bool isPointerDown;

    private void Reset()
    {
        targetImage = GetComponent<Image>();
        Button = GetComponent<Button>();
        canvasGroup = GetComponent<CanvasGroup>() ?? gameObject.AddComponent<CanvasGroup>();
    }

    private void Awake()
    {
        targetImage = GetComponent<Image>();
        Button = GetComponent<Button>();
        canvasGroup = GetComponent<CanvasGroup>() ?? gameObject.AddComponent<CanvasGroup>();

        if (Button)
        {
            Button.transition = Selectable.Transition.None;
            Button.interactable = true; // we gate via IsInteractable/CanvasGroup
        }

        ApplyInteractable();
        RefreshVisual();
    }

    private void OnEnable()
    {
        isPointerOver = isPointerDown = false;
        ApplyInteractable(); // respect serialized IsInteractable
        RefreshVisual();
    }

    private void OnDisable()
    {
        isPointerOver = isPointerDown = false;
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (!targetImage) targetImage = GetComponent<Image>();
        if (!Button) Button = GetComponent<Button>();
        if (!canvasGroup) canvasGroup = GetComponent<CanvasGroup>();

        if (Button)
        {
            Button.transition = Selectable.Transition.None;
            Button.interactable = true;
        }

        if (isActiveAndEnabled)
        {
            ApplyInteractable();
            RefreshVisual();
        }
    }
#endif

    #region Public API

    public void SetOnClick(UnityEngine.Events.UnityAction action)
    {
        Button.onClick.RemoveAllListeners();
        Button.onClick.AddListener(action);
    }

    public void Show()
    {
        if (!canvasGroup) return;
        canvasGroup.alpha = 1f;
        ApplyInteractable();
    }

    public void Hide()
    {
        if (!canvasGroup) return;
        canvasGroup.alpha = 0f;
        canvasGroup.blocksRaycasts = false;
        canvasGroup.interactable = false;
    }

    public void SetActsAsToggle(bool value)
    {
        actsAsToggle = value;
        RefreshVisual();
    }

    public void SetToggled(bool value, bool invokeEvent = false)
    {
        if (isToggled == value) { RefreshVisual(); return; }

        isToggled = value;
        RefreshVisual();

        if (invokeEvent)
            OnToggle?.Invoke(isToggled);
    }

    public bool GetToggled() => isToggled;

    public void SetInteractable(bool value)
    {
        IsInteractable = value;
        ApplyInteractable();
        RefreshVisual();
    }

    public void Enable() => SetInteractable(true);
    public void Disable() => SetInteractable(false);

    public void SetText(string text)
    {
        if (LabelText) LabelText.text = text;
    }

    #endregion

    #region Internal Logic

    private void ApplyInteractable()
    {
        if (Button)
        {
            Button.transition = Selectable.Transition.None;
            Button.interactable = true;
        }

        if (canvasGroup)
        {
            canvasGroup.interactable = IsInteractable;   // gates navigation/select
            canvasGroup.blocksRaycasts = IsInteractable; // gates pointer events
        }
    }

    private void RefreshVisual(bool forcePressed = false)
    {
        if (!targetImage) return;

        Color c;
        if (!IsInteractable)
        {
            c = BaseColor;
            c.a *= 0.5f; // dim when disabled
        }
        else
        {
            bool pressed = forcePressed || (isPointerDown && isPointerOver);

            // Priority: Pressed > Toggled > Hovered > Base
            if (pressed)
                c = PressedColor;
            else if (isToggled)
                c = PressedColor;
            else if (isPointerOver)
                c = HoveredColor;
            else
                c = BaseColor;
        }

        targetImage.color = c;

        if (Button && Button.transition != Selectable.Transition.None)
            Button.transition = Selectable.Transition.None;
    }

    #endregion

    #region Pointer Events

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!IsInteractable) return;
        if (eventData.button != PointerEventData.InputButton.Left) return;

        isPointerDown = true;
        RefreshVisual(forcePressed: true);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (!IsInteractable) return;
        if (eventData.button != PointerEventData.InputButton.Left) return;

        isPointerDown = false;

        if (isPointerOver && actsAsToggle)
            SetToggled(!isToggled, invokeEvent: true);

        RefreshVisual();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!IsInteractable) return;
        isPointerOver = true;
        if (!isPointerDown) RefreshVisual();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!IsInteractable) return;
        isPointerOver = false;
        if (!isPointerDown) RefreshVisual();
    }

    #endregion
}
