using UnityEngine;
using TMPro;

public class EnemyAI : MonoBehaviour
{
    [Header("Data Soal")]
    public QuestionData questionData;

    [Header("Tampilan")]
    public TextMeshPro questionText;

    [Header("Gerakan - diisi otomatis oleh WaveSpawner")]
    public int levelIndex = 1;
    public float moveSpeed = 2f;

    // Internal
    private bool isDead = false;
    private bool firstAttempt = true;
    private Transform playerTransform;

    // Untuk gerakan zigzag (level 2)
    private float zigzagTimer = 0f;
    private float zigzagDirection = 1f;

    void Start()
    {
        if (questionData != null && questionText != null)
            questionText.text = questionData.englishSentence;
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
                return; // belum ketemu, skip dulu
        }

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
                // Homing lurus ke player
                transform.position += direction * moveSpeed * Time.deltaTime;

                float angle1 = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.Euler(0, 0, angle1 + 90f);
                break;

            case 2:
                // Zigzag ke arah player
                zigzagTimer += Time.deltaTime;
                if (zigzagTimer >= 1f)
                {
                    zigzagTimer = 0f;
                    zigzagDirection *= -1f;
                }

                Vector3 zigzag = new Vector3(direction.x, direction.y + zigzagDirection * 0.8f, 0).normalized;
                transform.position += zigzag * moveSpeed * Time.deltaTime;

                float angle2 = Mathf.Atan2(zigzag.y, zigzag.x) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.Euler(0, 0, angle2 + 90f);
                break;

            case 3:
                // Homing cepat + sedikit memutar sebelum menyerang
                transform.position += direction * moveSpeed * Time.deltaTime;

                float angle3 = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.Euler(0, 0, angle3 + 90f);
                break;
        }
    }

    void CheckReachedPlayer()
    {
        if (playerTransform == null) return;

        float dist = Vector3.Distance(transform.position, playerTransform.position);
        if (dist < 0.8f)
        {
            GameManager.Instance.TakeDamage();
            WaveSpawner.Instance.OnEnemyDefeated();
            Destroy(gameObject);
        }
    }

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
