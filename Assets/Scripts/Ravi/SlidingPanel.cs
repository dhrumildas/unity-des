using UnityEngine;
using UnityEngine.EventSystems;

public class SlidingPanel : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Slide Settings")]
    public RectTransform panel;
    public float hiddenX = -200f;    // hidden Position (off-screen)
    public float shownX = 0f;        // Showing Area
    public float slideSpeed = 6f;    // sliding Effect speed

    private bool isHovered = false;
    private Vector2 targetPos;

    void Start()
    {
        targetPos = new Vector2(hiddenX, panel.anchoredPosition.y);
        panel.anchoredPosition = targetPos;
    }

    void Update()
    {
        panel.anchoredPosition = Vector2.Lerp(
            panel.anchoredPosition,
            targetPos,
            Time.deltaTime * slideSpeed
        );
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        targetPos = new Vector2(shownX, panel.anchoredPosition.y);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        targetPos = new Vector2(hiddenX, panel.anchoredPosition.y);
    }
}