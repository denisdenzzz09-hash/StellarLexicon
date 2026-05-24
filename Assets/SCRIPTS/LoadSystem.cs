using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class LoadSystem : MonoBehaviour
{
    public static LoadSystem Instance;
    private GameSaveData pendingData;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
    }

    public void LoadSavedGame()
    {
        pendingData = SaveSystem.Load();
        if (pendingData == null)
        {
            Debug.Log("Tidak ada save file!");
            return;
        }

        GameManager.Instance.SetNextSceneIsLoad(true);
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.LoadScene("LEVEL_" + pendingData.scene);
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        StartCoroutine(RestoreAfterLoad(pendingData));
    }

    IEnumerator RestoreAfterLoad(GameSaveData data)
    {
        yield return new WaitForSeconds(0.5f);
        Time.timeScale = 1f;

        GameManager.Instance.SetHP(data.health);
        GameManager.Instance.SetScore(data.score);
        WaveSpawner.Instance.SetLevel(data.scene);

        if (data.isLevelWon)
        {
            GameManager.Instance.TriggerLevelComplete();
            yield break;
        }

        WaveSpawner.Instance.SetWave(data.wave);

        GameObject bg1 = GameObject.Find("Background1");
        GameObject bg2 = GameObject.Find("Background2");
        if (bg1 != null) bg1.transform.position = new Vector3(data.bg1PosX, bg1.transform.position.y, bg1.transform.position.z);
        if (bg2 != null) bg2.transform.position = new Vector3(data.bg2PosX, bg2.transform.position.y, bg2.transform.position.z);

        // Restore musuh biasa
        foreach (EnemySaveData enemyData in data.enemies)
        {
            QuestionData question = Resources.Load<QuestionData>(enemyData.questionName);
            Vector3 pos = new Vector3(enemyData.posX, enemyData.posY, 0);
            GameObject enemyObj = Instantiate(WaveSpawner.Instance.enemyPrefab, pos, Quaternion.identity);
            EnemyAI enemy = enemyObj.GetComponent<EnemyAI>();
            enemy.questionData  = question;
            enemy.moveSpeed     = enemyData.moveSpeed;
            enemy.levelIndex    = enemyData.levelIndex;
            enemy.SetFirstAttempt(enemyData.firstAttempt);
        }

        // FIX: set enemiesAlive agar OnEnemyDefeated tidak trigger wave terlalu cepat
        WaveSpawner.Instance.SetEnemiesAlive(data.enemies.Count);

        // Restore boss jika ada, kalau tidak lanjut spawn wave
        if (data.hasBoss)
        {
            QuestionData bossQuestion = Resources.Load<QuestionData>(data.bossQuestionName);
            Vector3 bossPos = new Vector3(data.bossPosX, data.bossPosY, 0);
            GameObject bossObj = Instantiate(WaveSpawner.Instance.bossPrefab, bossPos, Quaternion.identity);
            BossEnemy boss = bossObj.GetComponent<BossEnemy>();
            boss.questionData = bossQuestion;
            boss.moveSpeed    = data.bossMoveSpeed;
            boss.levelIndex   = data.bossLevelIndex;
            boss.bossHP       = data.bossHP;
        }
        else if (data.enemies.Count == 0)
        {
            // Tidak ada musuh dan tidak ada boss → lanjut wave berikutnya
            StartCoroutine(WaveSpawner.Instance.ResumeFromLoad());
        }

        pendingData = null;
    }
}