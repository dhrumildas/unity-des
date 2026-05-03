using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TimeCard : MonoBehaviour
{
    [SerializeField] private Sprite normalSprite;
    [SerializeField] private Sprite hoverSprite;
    [SerializeField] private Sprite pressedSprite;
    [SerializeField] private Sprite dragSprite;

    private Image image;
    private RectTransform rectTransform; // NEW: Cached for better performance
    private Canvas canvas;               // NEW: Reference to your Canvas
    private bool isPressed;
    private bool isDragging;

    private void Awake()
    {
        image = GetComponent<Image>();
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>(); // Finds the Canvas this card lives inside

        SetSprite(normalSprite);
    }

    public void OnPointerEnter(BaseEventData eventData)
    {
        if (!isPressed && !isDragging)
            SetSprite(hoverSprite);
    }

    public void OnPointerExit(BaseEventData eventData)
    {
        if (!isPressed && !isDragging)
            SetSprite(normalSprite);
    }

    public void OnPointerDown(BaseEventData eventData)
    {
        isPressed = true;
        SetSprite(pressedSprite);
    }

    public void OnPointerUp(BaseEventData eventData)
    {
        isPressed = false;

        if (isDragging) return;

        PointerEventData pointerData = (PointerEventData)eventData;
        SetSprite(pointerData.hovered.Contains(gameObject) ? hoverSprite : normalSprite);
    }

    public void OnBeginDrag(BaseEventData eventData)
    {
        isDragging = true;
        SetSprite(dragSprite != null ? dragSprite : hoverSprite);
    }

    public void OnDrag(BaseEventData eventData)
    {
        PointerEventData pointerData = (PointerEventData)eventData;

        // NEW: Divide the delta by the Canvas scale factor for a perfectly synced 1:1 drag
        rectTransform.anchoredPosition += pointerData.delta / canvas.scaleFactor;
    }

    public void OnEndDrag(BaseEventData eventData)
    {
        isDragging = false;
        isPressed = false;

        PointerEventData pointerData = (PointerEventData)eventData;
        SetSprite(pointerData.hovered.Contains(gameObject) ? hoverSprite : normalSprite);
    }

    private void SetSprite(Sprite sprite)
    {
        if (sprite == null) return;
        image.sprite = sprite;
        image.SetNativeSize();
    }
}