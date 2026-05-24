using UnityEngine;
using TMPro;

public class BossEnemy : MonoBehaviour
{
    [Header("Data Soal")]
    public QuestionData questionData;

    [Header("Tampilan")]
    public TextMeshPro questionText;
    public TextMeshPro hpText;

    [Header("Stat Bos - atur di Inspector")]
    public int bossHP = 3;
    public float moveSpeed = 0.2f;
    public int levelIndex = 1;

    // Internal
    private bool isDead = false;
    private bool firstAttempt = true;
    private Transform playerTransform;

    void Start()
    {
        if (questionData != null && questionText != null)
            questionText.text = questionData.englishSentence;

        UpdateHPDisplay();
    }

    void Update()
    {
        if (isDead) return;

        // Kalau player belum ketemu, coba cari lagi tiap frame
        if (playerTransform == null)
        {
            PlayerShip player = FindFirstObjectByType<PlayerShip>();
            if (player != null)
                playerTransform = player.transform;
            else
                return;
        }

        MoveTowardPlayer();
        CheckReachedPlayer();
    }

    void MoveTowardPlayer()
    {
        if (playerTransform == null) return;

        Vector3 direction = (playerTransform.position - transform.position).normalized;
        transform.position += direction * moveSpeed * Time.deltaTime;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle - 90f);
    }

    void CheckReachedPlayer()
    {
        if (playerTransform == null) return;

        float dist = Vector3.Distance(transform.position, playerTransform.position);
        if (dist < 0.8f)
        {
            GameManager.Instance.TriggerInstantDeath();
            Destroy(gameObject);
        }
    }

    public void OnCorrectAnswer()
    {
        if (isDead) return;

        int scoreToAdd = firstAttempt ? 100 : 50;
        firstAttempt = false;

        bossHP--;
        UpdateHPDisplay();

        GameManager.Instance.AddScore(scoreToAdd);
        GameManager.Instance.AddCombo();

        RefreshQuestion();

        if (bossHP <= 0)
        {
            isDead = true;
            GameManager.Instance.AddScore(300);
            WaveSpawner.Instance.OnBossDefeated();
            Destroy(gameObject);
        }
    }

    public void OnWrongAnswer()
    {
        firstAttempt = false;
        GameManager.Instance.ResetCombo();
    }

    void RefreshQuestion()
    {
        QuestionData next = WaveSpawner.Instance.GetRandomQuestion();
        if (next != null)
        {
            questionData = next;
            if (questionText != null)
                questionText.text = questionData.englishSentence;
        }
    }

    void UpdateHPDisplay()
    {
        if (hpText != null)
            hpText.text = $"HP: {bossHP}";
    }

    public QuestionData GetQuestionData() => questionData;
    public bool IsFirstAttempt() => firstAttempt;
}
