using UnityEngine;
using TMPro;
using System;
[RequireComponent(typeof(TMP_Text))]
public class ObjectAccessibleText : MonoBehaviour
{
    private TMP_Text tmpText;
    void Awake()
    {
        tmpText = GetComponent<TMP_Text>();
    }

    void OnEnable()
    {
        if (GlobalFontManager.Instance != null)
        {
            GlobalFontManager.Instance.ApplyFontToSpecificText(tmpText);
            GlobalFontManager.OnFontChanged += UpdateFont;
        }
    }

    void OnDisable()
    {
        if (GlobalFontManager.Instance != null)
        {
            GlobalFontManager.OnFontChanged -= UpdateFont;
        }
    }

    private void UpdateFont()
    {
        if (GlobalFontManager.Instance != null && tmpText != null)
        {
            GlobalFontManager.Instance.ApplyFontToSpecificText(tmpText);
        }
    }
}
