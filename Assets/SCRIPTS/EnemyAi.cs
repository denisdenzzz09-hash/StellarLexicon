using UnityEngine;
using TMPro;

public class EnemyAI : MonoBehaviour
{
    [Header("Data Soal")]
    public QuestionData questionData;

    [Header("Tampilan")]
    public TextMeshPro questionText; // TMP di atas alien

    [Header("Gerakan - diisi otomatis oleh WaveSpawner")]
    public int levelIndex = 1; // 1, 2, atau 3
    public float moveSpeed = 2f;

    // Internal
    private bool isDead = false;
    private bool firstAttempt = true;
    private Transform playerTransform;
    private float stopTimer = 0f;
    private bool isStopped = false;

    void Start()
    {
        if (questionData != null && questionText != null)
            questionText.text = questionData.englishSentence;

        // Cari player di scene
        PlayerShip player = FindFirstObjectByType<PlayerShip>();
        if (player != null)
            playerTransform = player.transform;
    }

    void Update()
    {
        if (isDead) return;

        MoveByLevel();
        CheckReachedPlayer();
    }

    void MoveByLevel()
    {
        if (playerTransform == null) return;

        Vector3 direction = (playerTransform.position - transform.position).normalized;

        switch (levelIndex)
        {
            case 1:
                // Homing lurus ke player, kecepatan tetap
                transform.position += direction * moveSpeed * Time.deltaTime;

                // Rotate menghadap player
                float angle1 = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.Euler(0, 0, angle1 + - 90f);
                break;

            case 2:
                // Homing + sesekali berhenti lalu sprint
                if (isStopped)
                {
                    stopTimer -= Time.deltaTime;
                    if (stopTimer <= 0f) isStopped = false;
                }
                else
                {
                    transform.position += direction * moveSpeed * Time.deltaTime;

                    float angle3 = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                    transform.rotation = Quaternion.Euler(0, 0, angle3 + -90f);

                    // Random berhenti sejenak lalu sprint
                    if (Random.Range(0f, 1f) < 0.002f)
                    {
                        isStopped = true;
                        stopTimer = Random.Range(0.3f, 0.6f);
                    }
                }
                break;
        }
    }

    void CheckReachedPlayer()
    {
        if (playerTransform == null) return;

        float dist = Vector3.Distance(transform.position, playerTransform.position);
        if (dist < 0.8f)
        {
            // Alien menyentuh player → HP berkurang
            GameManager.Instance.TakeDamage();
            WaveSpawner.Instance.OnEnemyDefeated();
            Destroy(gameObject);
        }
    }

    // Dipanggil oleh InputValidator saat jawaban benar
    public void OnCorrectAnswer()
    {
        if (isDead) return;
        isDead = true;

        int scoreToAdd = firstAttempt ? 100 : 50;
        GameManager.Instance.AddScore(scoreToAdd);
        GameManager.Instance.AddCombo();

        WaveSpawner.Instance.OnEnemyDefeated();
        Destroy(gameObject);
    }

    // Dipanggil saat jawaban salah
    public void OnWrongAnswer()
    {
        firstAttempt = false;
        GameManager.Instance.ResetCombo();
    }

    public QuestionData GetQuestionData() => questionData;
    public bool IsFirstAttempt() => firstAttempt;

    public void SetFirstAttempt(bool value)
    {
        firstAttempt = value;
    }
}