using UnityEngine;
using TMPro; // Make sure to include this for TextMeshPro

public class WeighingScaleUI : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI weightText;

    // The mail script will call this when dropped on the scale
    public void UpdateWeightDisplay(float weight)
    {
        weightText.text = weight.ToString() + "g";
    }

    // The mail script will call this when picked up from the scale
    public void ClearWeightDisplay()
    {
        weightText.text = "0g";
    }
}