using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class GameUI : MonoBehaviour
{
    public static GameUI Instance;

    [Header("HUD")]
    public Image hpBar;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI highScoreText;
    public TextMeshProUGUI waveText;

    [Header("Feedback")]
    public TextMeshProUGUI feedbackText;
    public Image screenEdgeFlash;
    public TextMeshProUGUI comboText;
    public Image inputBoxImage;

    [Header("You Win")]
    public GameObject youWinPanel;

    [Header("Game Over")]
    public TextMeshProUGUI gameOverText;

    // Warna
    Color correctColor = new Color(0.41f, 0.94f, 0.68f);
    Color wrongColor   = new Color(1f, 0.32f, 0.32f);

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        feedbackText?.gameObject.SetActive(false);
        comboText?.gameObject.SetActive(false);

        if (gameOverText != null)
            gameOverText.gameObject.SetActive(false);

        if (screenEdgeFlash != null)
            screenEdgeFlash.color = new Color(1, 0, 0, 0);

        int hs = GameManager.Instance.GetHighScore(GameManager.Instance.GetCurrentLevel());
        if (highScoreText != null)
            highScoreText.text = $"Best: {hs}";
    }

    // ===================== HP =====================

    public void UpdateHP(int current, int max)
    {
        if (hpBar != null)
            hpBar.fillAmount = (float)current / max;
    }

    public void ShowHPLostEffect()
    {
        StartCoroutine(FlashScreenEdge());
    }

    IEnumerator FlashScreenEdge()
    {
        if (screenEdgeFlash == null) yield break;
        screenEdgeFlash.color = new Color(1, 0, 0, 0.4f);
        yield return new WaitForSeconds(0.5f);
        screenEdgeFlash.color = new Color(1, 0, 0, 0);
    }

    // ===================== SKOR =====================

    public void UpdateScore(int score)
    {
        if (scoreText != null)
            scoreText.text = $"Score: {score}";
    }

    // ===================== WAVE =====================

    public void UpdateWaveText(int current, int total)
    {
        if (waveText != null)
            waveText.text = $"Wave {current}/{total}";
    }

    // ===================== FEEDBACK =====================

    public void ShowFeedback(bool isCorrect)
    {
        StopCoroutine("FeedbackRoutine");
        StartCoroutine(FeedbackRoutine(isCorrect));

        if (!isCorrect)
            StartCoroutine(FlashInputBox());
    }

    IEnumerator FeedbackRoutine(bool isCorrect)
    {
        if (feedbackText == null) yield break;

        feedbackText.text = isCorrect ? "CORRECT!" : "WRONG!";
        feedbackText.color = isCorrect ? correctColor : wrongColor;
        feedbackText.gameObject.SetActive(true);

        yield return new WaitForSeconds(0.8f);
        feedbackText.gameObject.SetActive(false);
    }

    IEnumerator FlashInputBox()
    {
        if (inputBoxImage == null) yield break;
        Color original = inputBoxImage.color;
        inputBoxImage.color = wrongColor;
        yield return new WaitForSeconds(0.3f);
        inputBoxImage.color = original;
    }

    // ===================== COMBO =====================

    public void ShowComboEffect(int combo)
    {
        StopCoroutine("ComboRoutine");
        StartCoroutine(ComboRoutine(combo));
    }

    IEnumerator ComboRoutine(int combo)
    {
        if (comboText == null) yield break;

        comboText.text = $"COMBO x{combo}! +200";
        comboText.gameObject.SetActive(true);

        float elapsed = 0f;
        Color start = new Color(1f, 0.55f, 0.1f, 1f);
        Color end   = new Color(1f, 0.55f, 0.1f, 0f);
        comboText.color = start;

        while (elapsed < 1.2f)
        {
            elapsed += Time.deltaTime;
            comboText.color = Color.Lerp(start, end, elapsed / 1.2f);
            yield return null;
        }

        comboText.gameObject.SetActive(false);
    }

    // ===================== GAME OVER =====================

    public void ShowGameOver()
    {
        if (gameOverText != null)
        {
            gameOverText.gameObject.SetActive(true);
            StartCoroutine(BlinkGameOver());
        }
    }

    public void ShowYouWin()
    {
        if (youWinPanel != null)
            youWinPanel.SetActive(true);
    }

    IEnumerator BlinkGameOver()
    {
        if (gameOverText == null) yield break;

        gameOverText.text = "GAME OVER";

        Color visible   = new Color(1f, 0.2f, 0.2f, 1f);
        Color invisible = new Color(1f, 0.2f, 0.2f, 0f);

        // kedip selama 3 detik
        float timer = 0f;
        while (timer < 3f)
        {
            gameOverText.color = visible;
            yield return new WaitForSecondsRealtime(0.5f);
            gameOverText.color = invisible;
            yield return new WaitForSecondsRealtime(0.5f);
            timer += 1f;
        }

        // setelah 3 detik, pindah ke main menu
        GameManager.Instance.LoadMainMenu();
    }

    // ===================== BUTTON CALLBACKS =====================

    public void OnResumeButton()    => GameManager.Instance.TogglePause();
    public void OnRestartButton()   => GameManager.Instance.RestartLevel();
    public void OnMainMenuButton()  => GameManager.Instance.LoadMainMenu();
    public void OnNextLevelButton() => GameManager.Instance.LoadNextLevel();
    public void OnSaveButton()
    {
        GameManager.Instance.SaveGame();
    }
}