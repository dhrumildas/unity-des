using UnityEngine;
using UnityEngine.EventSystems;
using MailSorting.Data;

public class UI_DragCheck : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("Mail Data")]
    public Mail_SO mailData;

    [Header("Spawner Reference")]
    public MailSpawner spawner;
    public bool isTutorialMail = false;

    private RectTransform rt;
    private Canvas mainCanvas;
    private CanvasGroup canvasGroup;
    private Transform originalParent;
    private Canvas sortingCanvas;

    private WeighingScaleUI weighingScale;
    private RectTransform weighingScaleRect;

    void Awake()
    {
        rt = GetComponent<RectTransform>();
        mainCanvas = GetComponentInParent<Canvas>();

        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null) canvasGroup = gameObject.AddComponent<CanvasGroup>();

        sortingCanvas = GetComponent<Canvas>();
        if (sortingCanvas == null) sortingCanvas = gameObject.AddComponent<Canvas>();

        if (GetComponent<UnityEngine.UI.GraphicRaycaster>() == null)
            gameObject.AddComponent<UnityEngine.UI.GraphicRaycaster>();

        originalParent = transform.parent;

        weighingScale = FindFirstObjectByType<WeighingScaleUI>();
        if (weighingScale != null)
            weighingScaleRect = weighingScale.GetComponent<RectTransform>();
    }

    // --- 1. THE GRAB (Left Mouse Button DOWN) ---
    public void OnPointerDown(PointerEventData eventData)
    {
        transform.SetAsLastSibling(); // Move to front of its local group

        // Instantly lift the paper into the air over the tools
        sortingCanvas.overrideSorting = true;
        sortingCanvas.sortingOrder = 100;

        // If we are picking it up FROM the scale, clear the scale screen
        if (weighingScale != null && transform.parent == weighingScaleRect)
        {
            weighingScale.ClearWeightDisplay();
        }
    }

    // --- 2. THE MOVE ---
    public void OnBeginDrag(PointerEventData eventData)
    {
        transform.SetParent(originalParent, true);
        canvasGroup.blocksRaycasts = false; // Let the mouse see through the paper
    }

    public void OnDrag(PointerEventData eventData)
    {
        rt.anchoredPosition += eventData.delta / mainCanvas.scaleFactor;
    }

    // --- 3. THE DROP ---
    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = true; // Turn hitboxes back on

        GameObject droppedObject = eventData.pointerCurrentRaycast.gameObject;

        // Did the mouse land on the scale?
        if (droppedObject != null && droppedObject.GetComponentInParent<WeighingScaleUI>() != null)
        {
            weighingScale.UpdateWeightDisplay(mailData.weight);
            transform.SetParent(weighingScaleRect, true); // Attach to scale
        }
        else
        {
            transform.SetParent(originalParent, true); // Fall back to the desk
        }
        ActionButtonsController.Instance?.SetActiveMail(mailData, isTutorialMail);
    }

    // --- 4. THE LET GO (Left Mouse Button UP) ---
    public void OnPointerUp(PointerEventData eventData)
    {
        // OnPointerUp always fires exactly when you let go of the mouse button, 
        // even if you didn't drag the item!

        if (weighingScaleRect != null && transform.parent == weighingScaleRect)
        {
            // If it ended up on the scale, leave it slightly elevated
            sortingCanvas.overrideSorting = true;
            sortingCanvas.sortingOrder = 10;
        }
        else
        {
            // If it ended up on the desk, drop it flat so the ruler can render over it
            sortingCanvas.overrideSorting = false;
        }
    }
}