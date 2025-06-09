using System;
using UnityEngine;
using UnityEngine.UI; // For UI elements like Image (loading screen)
using TMPro;        // For TextMeshPro (countdown text)
using System.Collections;
using DentedPixel;

public class LoadingAndCountdownManager : MonoBehaviour, IGameStateListener
{
    [Header("Countdown")]
    [SerializeField] private TextMeshProUGUI countdownText; // Assign your countdown TMP Text
    [SerializeField] private float countdownAnimationDuration = 0.8f;
    [SerializeField] private float countdownHoldDuration = 0.2f; // Time to display number before animating out
    [SerializeField] private Vector3 countdownEndPositionOffset = new Vector3(0, 200f, 0); // Relative to initial pos
    [SerializeField] private float countdownEndScale = 0.5f;
    [SerializeField] private AudioClip timerClick;
    [SerializeField] private AudioClip timerEnd;

    [SerializeField] private string _1;
    [SerializeField] private string _2;
    [SerializeField] private string _3;
    [SerializeField] private string _run;

    [Header("Events")]
    public UnityEngine.Events.UnityEvent OnCountdownFinished; // Triggered after "Run!"

    // --- Private State ---
    private bool isCountingDown = false;

    private Vector3 initialCountdownPosition;
    private Vector3 initialCountdownScale;
    private Color initialCountdownColor;


    void Start()
    {
        // --- Initial Setup ---
        if (countdownText == null)
        {
            Debug.LogError("LoadingAndCountdownManager: Countdown Text not assigned!", this);
            enabled = false; return;
        }

        // Start with loading screen active and countdown text hidden
        countdownText.gameObject.SetActive(false);

        // Store initial properties of the countdown text for animation reset
        initialCountdownPosition = countdownText.rectTransform.anchoredPosition;
        initialCountdownScale = countdownText.rectTransform.localScale;
        initialCountdownColor = countdownText.color;
     
    }


    private void StartCountdownSequence()
    {
        if (isCountingDown) return; // Prevent multiple countdowns
        isCountingDown = true;
        countdownText.gameObject.SetActive(true); // Show countdown text element

        StartCoroutine(CountdownCoroutine());
    }

    private IEnumerator CountdownCoroutine()
    {
        // --- 3 ---
        yield return AnimateCountdownNumber(_3);

        // --- 2 ---
        yield return AnimateCountdownNumber(_2);

        // --- 1 ---
        yield return AnimateCountdownNumber(_1);

        // --- Run! ---
        yield return AnimateCountdownNumber(_run, true); // Last element might have slightly different feel

        // Countdown finished
        countdownText.gameObject.SetActive(false); // Hide text after "Run!"
        isCountingDown = false;
        Debug.Log("Countdown Finished!");
        OnCountdownFinished?.Invoke(); // Notify other systems (e.g., start player movement)
    }

    private IEnumerator AnimateCountdownNumber(string text, bool isLastElement = false)
    {
        // 1. Reset and Prepare Text
        countdownText.text = text;
        countdownText.rectTransform.anchoredPosition = initialCountdownPosition;
        countdownText.rectTransform.localScale = initialCountdownScale;
        countdownText.color = initialCountdownColor;
        countdownText.gameObject.SetActive(true); // Ensure it's visible for the animation

        // Optional: Initial "pop-in" or appear animation for the number
        // LeanTween.scale(countdownText.rectTransform, initialCountdownScale * 1.2f, 0.1f).setEasePunch();
        // yield return new WaitForSeconds(0.1f); // Wait for pop-in

        // 2. Hold the number for a bit
        yield return new WaitForSecondsRealtime(countdownHoldDuration); // Use Realtime if game might be paused/slowed

        // 3. Animate Out (Move up, Scale down, Fade out)
        // Move
        LeanTween.move(countdownText.rectTransform, initialCountdownPosition + countdownEndPositionOffset, countdownAnimationDuration)
            .setEase(LeanTweenType.easeInQuad); // Ease in for a feeling of acceleration upwards

        // Scale
        LeanTween.scale(countdownText.rectTransform, initialCountdownScale * countdownEndScale, countdownAnimationDuration)
            .setEase(LeanTweenType.easeInQuad);

        // Fade (Alpha)
        LeanTween.alphaText(countdownText.rectTransform, 0f, countdownAnimationDuration)
            .setEase(LeanTweenType.easeInQuad);

        // Wait for the "out" animation to complete
        yield return new WaitForSecondsRealtime(countdownAnimationDuration);

        if(!isLastElement)
        {
            SpeedGameAudioManager.Instance.PlaySFX(timerClick);
        }
        else
        {
            SpeedGameAudioManager.Instance.PlaySFX(timerEnd);
        }

        // Hide if not the absolute last "Run!" text that might fade out naturally
        if (!isLastElement && countdownText.color.a <= 0.01f)
        {
            // countdownText.gameObject.SetActive(false); // Hide if fully faded
        }
    }

    public void OnGameStateChange(GameManager.GameState gameState)
    {
        if(gameState == GameManager.GameState.Countdown)
            StartCountdownSequence();
    }
}