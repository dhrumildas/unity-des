using UnityEngine;
using UnityEngine.EventSystems;

public class TapeMeasureDrag : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("Tape References")]
    public RectTransform yellowTape;
    public RectTransform tapeOrigin; // The DragHandle itself

    [Header("Hook Reference (Manual Sync)")]
    public RectTransform metalHook;

    void Start()
    {
        if (yellowTape != null)
        {
            yellowTape.sizeDelta = new Vector2(0, yellowTape.sizeDelta.y);
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        SyncTapePosition();
    }

    public void OnDrag(PointerEventData eventData)
    {
        SyncTapePosition();

        // 1. Calculate the tape width
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            yellowTape,
            eventData.position,
            eventData.pressEventCamera,
            out Vector2 localPoint
        );

        if (localPoint.x < 0)
        {
            // Update the tape visually
            yellowTape.sizeDelta = new Vector2(Mathf.Abs(localPoint.x), yellowTape.sizeDelta.y);

            // 2. MANUALLY move the hook's X position to match the mouse drag
            if (metalHook != null)
            {
                RectTransformUtility.ScreenPointToWorldPointInRectangle(
                    yellowTape,
                    eventData.position,
                    eventData.pressEventCamera,
                    out Vector3 worldPoint
                );

                Vector3 hookPos = metalHook.position;
                hookPos.x = worldPoint.x;
                metalHook.position = hookPos;
            }
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // Snap the tape width back to 0
        if (yellowTape != null)
        {
            yellowTape.sizeDelta = new Vector2(0, yellowTape.sizeDelta.y);
        }

        // Snap the hook's X position back to the origin
        if (metalHook != null && tapeOrigin != null)
        {
            Vector3 resetPos = metalHook.position;
            resetPos.x = tapeOrigin.position.x;
            metalHook.position = resetPos;
        }
    }

    private void SyncTapePosition()
    {
        if (yellowTape != null && tapeOrigin != null)
        {
            yellowTape.position = tapeOrigin.position;
        }
    }
}