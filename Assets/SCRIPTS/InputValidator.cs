using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Linq;

public class InputValidator : MonoBehaviour
{
    public static InputValidator Instance;

    [Header("UI")]
    public TMP_InputField answerInputField;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        // Cek setiap kali ada perubahan teks
        answerInputField.onValueChanged.AddListener(OnTyping);

        answerInputField.Select();
        answerInputField.ActivateInputField();
    }

    void Update()
    {
        if (!answerInputField.isFocused && !GameManager.Instance.IsGamePaused())
        {
            answerInputField.Select();
            answerInputField.ActivateInputField();
        }
    }

    void OnTyping(string currentText)
    {
        // Cek enemy biasa dulu
        EnemyAI targetEnemy = FindClosestEnemy();
        BossEnemy targetBoss = FindBoss();

        // Tentukan siapa yang lebih dekat
        bool shootBoss = false;
        if (targetEnemy == null && targetBoss == null) return;
        if (targetEnemy == null) shootBoss = true;
        else if (targetBoss != null && targetBoss.transform.position.x < targetEnemy.transform.position.x)
            shootBoss = true;

        QuestionData questionData = shootBoss ? targetBoss.GetQuestionData() : targetEnemy.GetQuestionData();
        bool isCorrect = ValidateAnswer(currentText, questionData);

        if (isCorrect)
        {
            PlayerShip player = FindFirstObjectByType<PlayerShip>();

            if (shootBoss)
            {
                player?.ShootAt(targetBoss);
            }
            else
            {
                player?.ShootAt(targetEnemy);
            }

            GameUI.Instance.ShowFeedback(true);
            ClearInput();
        }
    }

    BossEnemy FindBoss()
    {
        return FindFirstObjectByType<BossEnemy>();
    }

    bool ValidateAnswer(string playerAnswer, QuestionData data)
    {
        if (data == null || data.keywords == null || data.keywords.Length == 0)
            return false;

        string lowerAnswer = playerAnswer.ToLower();

        foreach (string keyword in data.keywords)
        {
            if (!lowerAnswer.Contains(keyword.ToLower()))
                return false;
        }

        return true;
    }

    EnemyAI FindClosestEnemy()
    {
        EnemyAI[] enemies = FindObjectsByType<EnemyAI>(FindObjectsSortMode.None);
        if (enemies.Length == 0) return null;
        return enemies.OrderBy(e => e.transform.position.x).FirstOrDefault();
    }

    void ClearInput()
    {
        answerInputField.text = "";
        answerInputField.Select();
        answerInputField.ActivateInputField();
    }
}