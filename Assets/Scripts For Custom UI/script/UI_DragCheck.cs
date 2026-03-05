using UnityEngine;
using UnityEngine.EventSystems;
using MailSorting.Data;

public class UI_DragCheck : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public Mail_Items_SO mailData;

    private RectTransform rt;
    private Canvas mainCanvas;
    private CanvasGroup canvasGroup;
    private Transform originalParent;
    private Canvas sortingCanvas;

    private bool isDragging = false;

    //void Awake()
    //{
    //    rt = GetComponent<RectTransform>();
    //    mainCanvas = GetComponentInParent<Canvas>();

    //    canvasGroup = GetComponent<CanvasGroup>();
    //    if (canvasGroup == null) canvasGroup = gameObject.AddComponent<CanvasGroup>();

    //    sortingCanvas = GetComponent<Canvas>();
    //    if (sortingCanvas == null) sortingCanvas = gameObject.AddComponent<Canvas>();

    //    if (GetComponent<UnityEngine.UI.GraphicRaycaster>() == null)
    //        gameObject.AddComponent<UnityEngine.UI.GraphicRaycaster>();

    //    originalParent = transform.parent;

    //    // Removed the Weighing Scale Awake caching here to prevent Null Errors!
    //}

    // Changed Awake to Start since envelope is breaking on click and drag 
    void Start()
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
    }

    // void SetOriginalParent(Transform newParent)
    //{
    //    originalParent = newParent;
    //}

    public void OnPointerDown(PointerEventData eventData)
    {
        transform.SetAsLastSibling();

        sortingCanvas.overrideSorting = true;
        sortingCanvas.sortingOrder = 100;

        // NEW: Dynamically check if we are picking it up FROM a scale
        WeighingScaleUI currentScale = GetComponentInParent<WeighingScaleUI>();
        if (currentScale != null)
        {
            currentScale.ClearWeightDisplay();
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        isDragging = true;
        transform.SetParent(originalParent, true);
        canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (mainCanvas == null)
            mainCanvas = GetComponentInParent<Canvas>();

        rt.anchoredPosition += eventData.delta / mainCanvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        isDragging = false;
        canvasGroup.blocksRaycasts = true;

        GameObject droppedObject = eventData.pointerCurrentRaycast.gameObject;
        WeighingScaleUI targetScale = null;

        // Safely see if we dropped it on a scale
        if (droppedObject != null)
        {
            targetScale = droppedObject.GetComponentInParent<WeighingScaleUI>();
        }

        if (targetScale != null)
        {
            // Safety check in case the Scriptable Object isn't assigned in the inspector
            if (mailData != null)
            {
                targetScale.UpdateWeightDisplay(mailData.weight);
            }
            else
            {
                Debug.LogWarning("[UI_DragCheck] Mail Data is missing in the Inspector! Cannot read weight.");
            }

            // Parent to the specific scale we dropped it on
            transform.SetParent(targetScale.GetComponent<RectTransform>(), true);

            sortingCanvas.overrideSorting = true;
            sortingCanvas.sortingOrder = 10;
        }
        else
        {
            transform.SetParent(originalParent, true);
            sortingCanvas.overrideSorting = false;
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (!isDragging)
        {
            // If we just clicked it while it was on the scale, keep it safely on the scale
            if (GetComponentInParent<WeighingScaleUI>() != null)
            {
                sortingCanvas.overrideSorting = true;
                sortingCanvas.sortingOrder = 10;
            }
            else
            {
                sortingCanvas.overrideSorting = false;
            }
        }
    }
    
}