using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class Cutscenes : MonoBehaviour
{
    [Header("Slideshow Settings")]
    public Sprite[] slides;
    public float timePerSlide = 4f;
    public string nextSceneName;

    [Header("UI References")]
    public Image slideDisplay;
    public GameObject pressSpacePrompt;

    public GameObject holdToSkipPrompt;

    [Header("Skip Settings")]
    public float requiredSkipHoldTime = 1.5f;
    public Slider skipProgressBar;

    private int currentSlideIndex = 0;
    private float slideTimer = 0f;
    private float currentHoldTime = 0f;
    private bool isCutsceneOver = false;

    void Start()
    {
        if (slides.Length > 0) slideDisplay.sprite = slides[0];

        if (pressSpacePrompt != null) pressSpacePrompt.SetActive(false);
        if (skipProgressBar != null) skipProgressBar.gameObject.SetActive(false);
        if (holdToSkipPrompt != null) holdToSkipPrompt.SetActive(true);

    }

    void Update()
    {
        if (isCutsceneOver)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                SceneManager.LoadScene(nextSceneName);
            }
            return;
        }

        if (Input.GetKey(KeyCode.Space))
        {
            currentHoldTime += Time.deltaTime;

            if (skipProgressBar != null)
            {
                skipProgressBar.gameObject.SetActive(true);
                skipProgressBar.value = currentHoldTime / requiredSkipHoldTime;
            }

            if (currentHoldTime >= requiredSkipHoldTime)
            {
                FinishCutscene();
            }
        }
        else
        {
            currentHoldTime -= Time.deltaTime * 2f;
            currentHoldTime = Mathf.Clamp(currentHoldTime, 0, requiredSkipHoldTime);

            if (skipProgressBar != null)
            {
                skipProgressBar.value = currentHoldTime / requiredSkipHoldTime;
                if (currentHoldTime <= 0) skipProgressBar.gameObject.SetActive(false);
            }
        }

        if (!isCutsceneOver)
        {
            slideTimer += Time.deltaTime;
            if (slideTimer >= timePerSlide)
            {
                NextSlide();
            }
        }
    }

    private void NextSlide()
    {
        slideTimer = 0f;
        currentSlideIndex++;

        if (currentSlideIndex >= slides.Length)
        {
            FinishCutscene();
        }
        else
        {
            slideDisplay.sprite = slides[currentSlideIndex];
        }
    }

    private void FinishCutscene()
    {
        isCutsceneOver = true;

        if (skipProgressBar != null) skipProgressBar.gameObject.SetActive(false);
        if (holdToSkipPrompt != null) holdToSkipPrompt.SetActive(false);

        if (pressSpacePrompt != null) pressSpacePrompt.SetActive(true);
    }
}