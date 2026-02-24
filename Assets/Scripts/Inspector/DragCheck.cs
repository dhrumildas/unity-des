using System;
using MailSorting.Data;
using UnityEngine;

public class DragCheck : MonoBehaviour
{

    public Collider2D dropZoneCollider;
    private Vector3 ogPos;
    private Vector3 offset;
    private Collider2D thisCollider;
    private Camera mainCamera;
    private MailSorting.UI.MailInspectionManager inspectionManager;
    public Mail_Items_SO mailData;
    public Collider2D rulerCollider;
    public Collider2D weighingscaleCollider;
    public GameObject inspectButton;
    //[SerializeField] private Collider2D rulerCollider;
    //[SerializeField] private Collider2D weighingscaleCollider;
    //[SerializeField] private GameObject inspectButton;

    private bool isHoveringRuler = false;
    private bool isHoveringScale = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        mainCamera = Camera.main;
        thisCollider = GetComponent<Collider2D>();
        ogPos = transform.position;
    }

    public void SetInspectionManager(MailSorting.UI.MailInspectionManager manager)
    {
        inspectionManager = manager;
    }

    private void OnMouseDown()
    {
        Vector3 mouseWorldPos = GetMouseWorldPos();
        offset = transform.position - mouseWorldPos;
    }

    private void OnMouseDrag()
    {
        Vector3 mouseWorldPos = GetMouseWorldPos();
        transform.position = mouseWorldPos + offset;
        CheckInspectionTools();
    }

    void OnMouseUp()
    {
        if (dropZoneCollider != null && IsFullyInside(thisCollider.bounds, dropZoneCollider.bounds))
        {
            Debug.Log("Dropped in the zone!");
            inspectButton.SetActive(true);

            // wire the button to open the correct inspection panel
            var btn = inspectButton.GetComponent<UnityEngine.UI.Button>();
            if (btn != null)
            {
                btn.onClick.RemoveAllListeners();
                btn.onClick.AddListener(() =>
                {
                    if (inspectionManager != null)
                        inspectionManager.InspectMail(mailData);

                    inspectButton.SetActive(false);
                });
            }
        }
        else
        {
            transform.position = ogPos;
            inspectButton.SetActive(false);
            ClearTooltips();
        }
    }

    private void CheckInspectionTools()
    {
        bool overRuler = rulerCollider != null && thisCollider.bounds.Intersects(rulerCollider.bounds);
        bool overScale = weighingscaleCollider != null && thisCollider.bounds.Intersects(weighingscaleCollider.bounds);

        if (overRuler && !isHoveringRuler)
        {
            TooltipCheck._instance.SetAndShowTooltip($"{mailData.dimensions.x} x {mailData.dimensions.y} cm");
            isHoveringRuler = true;
            isHoveringScale = false;
        }
        else if (overScale && !isHoveringScale)
        {
            TooltipCheck._instance.SetAndShowTooltip($"{mailData.weight}g");
            isHoveringScale = true;
            isHoveringRuler = false;
        }
        else if (!overRuler && !overScale && (isHoveringRuler || isHoveringScale))
        {
            ClearTooltips();
        }
    }

    private void ClearTooltips()
    {
        TooltipCheck._instance.HideTooltip();
        isHoveringRuler = false;
        isHoveringScale = false;
    }

    private bool IsFullyInside(Bounds bounds1, Bounds bounds2)
    {
        return bounds1.min.x >= bounds2.min.x &&
               bounds1.max.x <= bounds2.max.x &&
               bounds1.min.y >= bounds2.min.y &&
               bounds1.max.y <= bounds2.max.y;
    }

    private Vector3 GetMouseWorldPos()
    {
        Vector3 mousePoint = Input.mousePosition;
        mousePoint.z = Mathf.Abs(mainCamera.transform.position.z);
        return mainCamera.ScreenToWorldPoint(mousePoint);
    }

    // Update is called once per frame
    void Update()
    {

    }
}