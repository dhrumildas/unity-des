using UnityEngine;
using UnityEngine.UI;

public class RuleBook : MonoBehaviour
{
    [Header("Pages")]
    public GameObject[] pages;

    [Header("Buttons")]
    public Button nextButton;
    public Button closeButton;

    private int currentPage = 0;

    void Awake()
    {
        nextButton.onClick.AddListener(OnNextClicked);
        closeButton.onClick.AddListener(OnCloseClicked);
    }

    public void Open()
    {
        currentPage = 0;
        gameObject.SetActive(true);
        ShowPage(currentPage);
    }

    void OnNextClicked()
    {
        currentPage = (currentPage + 1) % pages.Length; //loops back to first page
        ShowPage(currentPage);
    }

    void ShowPage(int index)
    {
        for (int i = 0; i < pages.Length; i++)
            pages[i].SetActive(i == index); //current page Display
    }

    void OnCloseClicked()
    {
        gameObject.SetActive(false);
    }

    void OnDestroy()
    {
        nextButton.onClick.RemoveAllListeners();
        closeButton.onClick.RemoveAllListeners();
    }
}