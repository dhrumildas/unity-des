using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GlobalFontManager : MonoBehaviour
{

    public static GlobalFontManager Instance;
    public static event System.Action OnFontChanged;
    public TMP_FontAsset defaultFont;
    public TMP_FontAsset accessibleFont;

    public bool isAccessible = false;

    void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void OnDestroy()
    {
        if(Instance == this)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.F1))
        {
            ToggleAccessibility();
        }
    }

    public void ToggleAccessibility()
    {
        isAccessible = !isAccessible;
        ApplyFontToEntireScene();
        OnFontChanged?.Invoke();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (isAccessible)
        {
            ApplyFontToEntireScene();
        }
    }

    private void ApplyFontToEntireScene()
    {
        TMP_FontAsset fontToApply = isAccessible ? accessibleFont : defaultFont;

        TMP_Text[] allTextObjects = Object.FindObjectsByType<TMP_Text>(FindObjectsInactive.Include, FindObjectsSortMode.None);

        foreach (TMP_Text textComponent in allTextObjects)
        {
            textComponent.font = fontToApply;
        }
    }

    public void ApplyFontToSpecificText(TMP_Text textComponent)
    {
        textComponent.font = isAccessible ? accessibleFont : defaultFont;
    }
}
