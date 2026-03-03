using System;
using MailSorting.Data;
using UnityEngine;

public class NewDragCheck : MonoBehaviour
{
    [Header("Mail Data")]
    public Mail_Items_SO mailData;

    private Vector3 ogPos;
    private Vector3 offset;
    private Camera mainCamera;

    // We need to remember where the mail originally lived in the hierarchy
    private Transform originalParent;

    [Header("UI Tools")]
    private WeighingScaleUI weighingScale;
    private RectTransform scaleRectTransform;

    void Start()
    {
        mainCamera = Camera.main;
        ogPos = transform.position;
        originalParent = transform.parent; // Save its original home

        weighingScale = FindFirstObjectByType<WeighingScaleUI>();
        if (weighingScale != null)
        {
            scaleRectTransform = weighingScale.GetComponent<RectTransform>();
        }
    }

    private void OnMouseDown()
    {
        Vector3 mouseWorldPos = GetMouseWorldPos();
        offset = transform.position - mouseWorldPos;

        // Immediately detach the mail from the scale so it is free to drag
        transform.SetParent(originalParent, true);

        if (weighingScale != null)
        {
            weighingScale.ClearWeightDisplay();
        }
    }

    private void OnMouseDrag()
    {
        Vector3 mouseWorldPos = GetMouseWorldPos();
        transform.position = mouseWorldPos + offset;
    }

    void OnMouseUp()
    {
        if (scaleRectTransform != null && RectTransformUtility.RectangleContainsScreenPoint(scaleRectTransform, Input.mousePosition, mainCamera))
        {
            weighingScale.UpdateWeightDisplay(mailData.weight);

            // THE FIX: Attach the mail to the UI scale so it scrolls with it!
            // The 'true' ensures it doesn't change its visual size/rotation when parented
            transform.SetParent(scaleRectTransform, true);
        }
        else
        {
            // Snap back and ensure it returns to its original hierarchy
            transform.SetParent(originalParent, true);
            transform.position = ogPos;
        }
    }

    private Vector3 GetMouseWorldPos()
    {
        Vector3 mousePoint = Input.mousePosition;
        mousePoint.z = Mathf.Abs(mainCamera.transform.position.z);
        return mainCamera.ScreenToWorldPoint(mousePoint);
    }
}