using UnityEngine;

public class Hook : MonoBehaviour
{
    [Header("Scroll Tracker")]
    [Tooltip("Drag the TapeMeasureHousing (the red box in the drawer) here")]
    public RectTransform targetHousing;

    private RectTransform hookRect;

    void Start()
    {
        hookRect = GetComponent<RectTransform>();
    }

    void LateUpdate()
    {
        if (targetHousing != null)
        {
            Vector3 newPos = hookRect.position;
            newPos.y = targetHousing.position.y;
            hookRect.position = newPos;
        }
    }
}