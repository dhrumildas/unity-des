using UnityEngine;
using UnityEngine.EventSystems; // Required for dragging UI!

public class RulerTool : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("References")]
    public RectTransform yellowTape;
    public RectTransform tapeOrigin; // We will use the DragHandle for this

    void Start()
    {
        // Ensure the tape is fully retracted when the game starts
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

        // This converts the screen mouse position into the Yellow Tape's local UI space.
        // Because the tape's pivot is on the far right (1, 0.5), pulling left gives us a negative X value.
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            yellowTape,
            eventData.position,
            eventData.pressEventCamera,
            out Vector2 localPoint
        );

        // If we are dragging to the left, expand the width of the tape
        if (localPoint.x < 0)
        {
            yellowTape.sizeDelta = new Vector2(Mathf.Abs(localPoint.x), yellowTape.sizeDelta.y);
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // Snap the tape back into the housing when you let go
        yellowTape.sizeDelta = new Vector2(0, yellowTape.sizeDelta.y);
    }

    private void SyncTapePosition()
    {
        // Constantly lock the yellow tape to the handle's position in the world.
        // This ensures they stay connected even if the drawer is scrolled up or down!
        if (yellowTape != null && tapeOrigin != null)
        {
            yellowTape.position = tapeOrigin.position;
        }
    }
}