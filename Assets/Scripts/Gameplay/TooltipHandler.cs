using TMPro;
using UnityEngine;

public class TooltipHandler : MonoBehaviour
{
    public GameObject tooltipUI;
    public TextMeshProUGUI tooltipText;
    public Canvas parentCanvas;

    [Tooltip("Pixel offset from cursor so the tooltip doesn't sit right under it")]
    public Vector2 offset = new Vector2(15f, -15f);

    private RectTransform _tooltipRect;
    private Camera _canvasCam;
    private bool _isShowing;

    void Start()
    {
        if (tooltipUI != null)
        {
            _tooltipRect = tooltipUI.GetComponent<RectTransform>();
            tooltipUI.SetActive(false);
        }

        if (parentCanvas != null)
            _canvasCam = parentCanvas.worldCamera;
    }

    void Update()
    {
        if (_isShowing && _tooltipRect != null)
            FollowMouse();
    }

    public void OnHoverEnter()
    {
        if (tooltipUI == null) return;
        tooltipUI.SetActive(true);
        _isShowing = true;
        FollowMouse(); // snap immediately so it doesn't flash at old position
    }

    public void OnHoverEnterAccept()
    {
        if (tooltipUI == null) return;
        tooltipUI.SetActive(true);
        _isShowing = true;
        tooltipText.text = "ACCEPT";
        FollowMouse(); // snap immediately so it doesn't flash at old position
    }

    public void OnHoverEnterReply()
    {
        if (tooltipUI == null) return;
        tooltipUI.SetActive(true);
        _isShowing = true;
        tooltipText.text = "REPLY";
        FollowMouse(); // snap immediately so it doesn't flash at old position
    }

    public void OnHoverEnterReject()
    {
        if (tooltipUI == null) return;
        tooltipUI.SetActive(true);
        _isShowing = true;
        tooltipText.text = "REJECT";
        FollowMouse(); // snap immediately so it doesn't flash at old position
    }

    public void OnHoverEnterReport()
    {
        if (tooltipUI == null) return;
        tooltipUI.SetActive(true);
        _isShowing = true;
        tooltipText.text = "REPORT";
        FollowMouse(); // snap immediately so it doesn't flash at old position
    }

    public void OnHoverExit()
    {
        if (tooltipUI == null) return;
        tooltipUI.SetActive(false);
        _isShowing = false;
        tooltipText.text = "";
    }

    private void FollowMouse()
    {
        Vector2 mousePos = Input.mousePosition;

        // 1. Dynamic Pivot Adjustment
        // Check which half of the screen the mouse is on to prevent clipping
        float pivotX = mousePos.x > Screen.width / 2f ? 1f : 0f; // Right half = 1, Left half = 0
        float pivotY = mousePos.y > Screen.height / 2f ? 1f : 0f; // Top half = 1, Bottom half = 0

        _tooltipRect.pivot = new Vector2(pivotX, pivotY);

        // 2. Convert to Canvas Space
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            parentCanvas.transform as RectTransform,
            mousePos,
            _canvasCam,
            out Vector2 localPoint);

        // 3. Dynamic Offset (Optional but recommended)
        // Flips the offset so it always pushes away from the cursor, not into the screen edge
        Vector2 dynamicOffset = new Vector2(
            pivotX == 1f ? -offset.x : offset.x,
            pivotY == 1f ? offset.y : -offset.y
        );

        _tooltipRect.localPosition = localPoint + dynamicOffset;
    }
}