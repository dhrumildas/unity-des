using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PunchClock : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private RectTransform targetImage;

    [Header("Sprite on Overlap")]
    [SerializeField] private Sprite overlappedSprite;

    private Image image;
    private Sprite originalSprite;
    private RectTransform rectTransform;
    private bool hasTriggered;

    private void Awake()
    {
        image = GetComponent<Image>();
        rectTransform = GetComponent<RectTransform>();
        originalSprite = image.sprite;
    }

    private void Update()
    {
        if (hasTriggered || targetImage == null || !targetImage.gameObject.activeSelf)
            return;

        if (RectOverlaps(rectTransform, targetImage))
        {
            hasTriggered = true;

            image.sprite = overlappedSprite;
            image.SetNativeSize();

            targetImage.gameObject.SetActive(false);
        }
    }

    private bool RectOverlaps(RectTransform a, RectTransform b)
    {
        Rect rectA = GetScreenRect(a);
        Rect rectB = GetScreenRect(b);
        return rectA.Overlaps(rectB);
    }

    private Rect GetScreenRect(RectTransform rt)
    {
        Vector3[] corners = new Vector3[4];
        rt.GetWorldCorners(corners);

        Canvas canvas = rt.GetComponentInParent<Canvas>();
        Camera cam = (canvas.renderMode == RenderMode.ScreenSpaceOverlay) ? null : canvas.worldCamera;

        Vector2 min = RectTransformUtility.WorldToScreenPoint(cam, corners[0]);
        Vector2 max = RectTransformUtility.WorldToScreenPoint(cam, corners[2]);

        return new Rect(min.x, min.y, max.x - min.x, max.y - min.y);
    }

    public void ResetState()
    {
        hasTriggered = false;
        image.sprite = originalSprite;
        image.SetNativeSize();

        if (targetImage != null)
            targetImage.gameObject.SetActive(true);
    }
}
