using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Pengaturan HP")]
    public int maxHP = 3;

    private int currentHP;
    private int currentScore;
    private int comboCount;
    private int currentLevel = 1;
    private bool isPaused = false;
    private bool gameOver = false;
    private bool isLevelComplete = false;

    const string KEY_LEVEL = "CurrentLevel";
    const string KEY_HS_1 = "HighScore_Level1";
    const string KEY_HS_2 = "HighScore_Level2";
    const string KEY_HS_3 = "HighScore_Level3";
    const string KEY_UNLOCKED = "UnlockedLevel";

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
    }

    void Start()
    {
        currentLevel = PlayerPrefs.GetInt(KEY_LEVEL, 1);
        ResetSession();
    }

    void ResetSession()
    {
        currentHP = maxHP;
        currentScore = 0;
        comboCount = 0;
        gameOver = false;
        isLevelComplete = false; // ← tambah ini
        GameUI.Instance?.UpdateHP(currentHP, maxHP);
        GameUI.Instance?.UpdateScore(currentScore);
    }

    // ===================== HP =====================

    public void TakeDamage()
    {
        if (gameOver) return;

        currentHP--;
        GameUI.Instance.UpdateHP(currentHP, maxHP);
        GameUI.Instance.ShowHPLostEffect();

        if (currentHP <= 0)
            TriggerGameOver();
    }

    void TriggerGameOver()
    {
        gameOver = true;
        SaveHighScore();
        Time.timeScale = 0f;
        GameUI.Instance?.ShowGameOver();
    }

    public void TriggerInstantDeath()
    {
        currentHP = 0;
        GameUI.Instance?.UpdateHP(currentHP, maxHP);
        TriggerGameOver();
    }

    // ===================== SKOR & COMBO =====================

    public void AddScore(int amount)
    {
        currentScore += amount;
        GameUI.Instance.UpdateScore(currentScore);
    }

    public void AddCombo()
    {
        comboCount++;

        if (comboCount > 0 && comboCount % 5 == 0)
        {
            AddScore(200);
            GameUI.Instance.ShowComboEffect(comboCount);
        }
    }

    public void ResetCombo()
    {
        comboCount = 0;
    }

    // ===================== LEVEL =====================

    public void OnLevelComplete()
    {
        isLevelComplete = true;
        SaveHighScore();

        int unlockedLevel = PlayerPrefs.GetInt(KEY_UNLOCKED, 1);
        if (currentLevel + 1 > unlockedLevel)
            PlayerPrefs.SetInt(KEY_UNLOCKED, currentLevel + 1);

        Time.timeScale = 0f;              // freeze game
        GameUI.Instance?.ShowYouWin();    // munculkan panel You Win
    }

    public void TriggerLevelComplete()
    {
        isLevelComplete = true;
        Time.timeScale = 0f;
        GameUI.Instance?.ShowYouWin();
    }

    int CalculateStars()
    {
        if (currentHP == maxHP) return 3;
        if (currentHP == maxHP - 1) return 2;
        return 1;
    }

    // ===================== SAVE SYSTEM =====================

    void SaveHighScore()
    {
        string key = currentLevel switch
        {
            1 => KEY_HS_1,
            2 => KEY_HS_2,
            3 => KEY_HS_3,
            _ => KEY_HS_1
        };

        int saved = PlayerPrefs.GetInt(key, 0);
        if (currentScore > saved)
            PlayerPrefs.SetInt(key, currentScore);

        PlayerPrefs.Save();
    }

    public int GetHighScore(int level)
    {
        return level switch
        {
            1 => PlayerPrefs.GetInt(KEY_HS_1, 0),
            2 => PlayerPrefs.GetInt(KEY_HS_2, 0),
            3 => PlayerPrefs.GetInt(KEY_HS_3, 0),
            _ => 0
        };
    }
    public int GetUnlockedLevel() => PlayerPrefs.GetInt(KEY_UNLOCKED, 1);

    public void SetHP(int hp)
    {
        currentHP = hp;
        GameUI.Instance?.UpdateHP(currentHP, maxHP);
    }

    public void SetScore(int score)
    {
        currentScore = score;
        GameUI.Instance?.UpdateScore(currentScore);
    }


       // ===================== SAVE SYSTEM =====================
    public void SaveGame()
    {
        EnemyAI[] enemies = FindObjectsByType<EnemyAI>(FindObjectsSortMode.None);
        GameSaveData data = new GameSaveData
        {
            scene   = GetCurrentLevel(),
            health  = GetCurrentHP(),
            score   = GetCurrentScore(),
            wave    = WaveSpawner.Instance.GetCurrentWave(),
            isLevelWon = IsLevelComplete(),
            bg1PosX = GameObject.Find("Background1")?.transform.position.x ?? 0f,
            bg2PosX = GameObject.Find("Background2")?.transform.position.x ?? 0f
        };

        foreach (EnemyAI e in enemies)
        {
            data.enemies.Add(new EnemySaveData
            {
                posX         = e.transform.position.x,
                posY         = e.transform.position.y,
                questionName = e.questionData.name,
                moveSpeed    = e.moveSpeed,
                levelIndex   = e.levelIndex,
                firstAttempt = e.IsFirstAttempt()
            });
        }

        SaveSystem.Save(data);
    }

    // ===================== SCENE MANAGEMENT =====================

    public void LoadLevel(int level)
    {
        currentLevel = level;
        Time.timeScale = 1f;
        ResetSession();
        SceneManager.LoadScene("LEVEL_" + level);
    }

    public void LoadMainMenu()
    {
        Time.timeScale = 1f;
        ResetSession();
        SceneManager.LoadScene("MainMenu");
    }

    public void RestartLevel()
    {
        Time.timeScale = 1f;
        ResetSession();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void LoadNextLevel()
    {
        int next = currentLevel + 1;
        Debug.Log("currentLevel: " + currentLevel + ", next: " + next); // ← tambah ini
        if (next <= 3) LoadLevel(next);
        else LoadMainMenu();
    }

    // ===================== PAUSE =====================

    public void TogglePause()
    {
        if (gameOver) return;
        if (isLevelComplete) return;
        isPaused = !isPaused;
        Time.timeScale = isPaused ? 0f : 1f;
        Debug.Log("TogglePause dipanggil, isPaused: " + isPaused + ", timeScale: " + Time.timeScale);
    }

    public bool IsGamePaused() => isPaused;
    public int GetCurrentLevel() => currentLevel;
    public int GetCurrentScore() => currentScore;
    public int GetCurrentHP() => currentHP;
    public bool IsLevelComplete() => isLevelComplete;
}