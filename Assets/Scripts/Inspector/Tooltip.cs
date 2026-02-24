using UnityEngine;

public class Tooltip : MonoBehaviour
{
    public string message;

    private void OnMouseEnter()
    {
        TooltipCheck._instance.SetAndShowTooltip(message);
    }

    private void OnMouseExit()
    {
        TooltipCheck._instance.HideTooltip();
    }
}
