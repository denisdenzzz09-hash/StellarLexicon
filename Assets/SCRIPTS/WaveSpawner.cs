using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WaveSpawner : MonoBehaviour
{
    public static WaveSpawner Instance;

    [Header("Prefab")]
    public GameObject enemyPrefab; // Prefab alien dengan EnemyAI attached

    [Header("Bos")]
    public GameObject bossPrefab; // Prefab bos, assign di Inspector

    [Header("Data Soal per Level")]
    public QuestionData[] level1Questions;
    public QuestionData[] level2Questions;
    public QuestionData[] level3Questions;

    [Header("Pengaturan Wave")]
    public int totalWaves = 4;         // Jumlah wave per level
    public float timeBetweenWaves = 3f; // Jeda antar wave (detik)
    public float timeBetweenSpawns = 1.5f; // Jeda antar alien dalam satu wave

    [Header("Spawn Position")]
    public float spawnX = 11f;          // Posisi X spawn (kanan layar)
    public float spawnYMin = -3f;       // Batas bawah posisi Y spawn
    public float spawnYMax = 3f;        // Batas atas posisi Y spawn

    // Internal
    private int currentWave = 0;
    private int enemiesAlive = 0;
    private int currentLevel = 1;
    private bool isSpawning = false;
    private List<QuestionData> availableQuestions = new List<QuestionData>();

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        currentLevel = GameManager.Instance.GetCurrentLevel();
        LoadQuestionsForLevel(currentLevel);
        StartCoroutine(StartNextWave());
    }

    void LoadQuestionsForLevel(int level)
    {
        availableQuestions.Clear();

        QuestionData[] source = level switch
        {
            1 => level1Questions,
            2 => level2Questions,
            3 => level3Questions,
            _ => level1Questions
        };

        availableQuestions.AddRange(source);
        ShuffleQuestions();
    }

    void ShuffleQuestions()
    {
        for (int i = availableQuestions.Count - 1; i > 0; i--)
        {
            int randIndex = Random.Range(0, i + 1);
            (availableQuestions[i], availableQuestions[randIndex]) =
                (availableQuestions[randIndex], availableQuestions[i]);
        }
    }

    IEnumerator StartNextWave()
    {
        if (currentWave >= totalWaves)
        {
            // Semua wave beres → spawn bos
            yield return new WaitForSeconds(timeBetweenWaves);
            SpawnBoss();
            yield break;
        }

        yield return new WaitForSeconds(timeBetweenWaves);

        currentWave++;
        GameUI.Instance.UpdateWaveText(currentWave, totalWaves);

        int enemyCount = GetEnemyCountForWave(currentLevel);
        yield return StartCoroutine(SpawnWave(enemyCount));
    }

    IEnumerator SpawnWave(int count)
    {
        isSpawning = true;
        enemiesAlive = count;

        for (int i = 0; i < count; i++)
        {
            SpawnEnemy();
            yield return new WaitForSeconds(timeBetweenSpawns);
        }

        isSpawning = false;
    }

    void SpawnEnemy()
    {
        if (availableQuestions.Count == 0)
            LoadQuestionsForLevel(currentLevel); // reload kalau habis

        // Ambil soal random
        int randIndex = Random.Range(0, availableQuestions.Count);
        QuestionData question = availableQuestions[randIndex];
        availableQuestions.RemoveAt(randIndex);

        // Posisi spawn
        float spawnY = Random.Range(spawnYMin, spawnYMax);
        Vector3 spawnPos = new Vector3(spawnX, spawnY, 0);

        GameObject enemyObj = Instantiate(enemyPrefab, spawnPos, Quaternion.identity);
        EnemyAI enemy = enemyObj.GetComponent<EnemyAI>();

        // Setup enemy
        enemy.questionData = question;
        enemy.levelIndex = currentLevel;
        enemy.moveSpeed = GetSpeedForLevel(currentLevel);
    }

    // Dipanggil EnemyAI saat alien mati atau lolos
    public void OnEnemyDefeated()
    {
        enemiesAlive--;

        if (enemiesAlive <= 0 && !isSpawning)
            StartCoroutine(StartNextWave());
    }

    int GetEnemyCountForWave(int level)
    {
        return level switch
        {
            1 => Random.Range(3, 6),   // 3-5 alien
            2 => Random.Range(5, 9),   // 5-8 alien
            3 => Random.Range(8, 13),  // 8-12 alien
            _ => 3
        };
    }

    float GetSpeedForLevel(int level)
    {
        return level switch
        {
            1 => 2f,
            2 => 3.5f,
            3 => 5f,
            _ => 2f
        };
    }

    public int GetCurrentWave() => currentWave;
    public int GetTotalWaves() => totalWaves;

    void SpawnBoss()
    {
        if (availableQuestions.Count == 0)
            LoadQuestionsForLevel(currentLevel);

        int randIndex = Random.Range(0, availableQuestions.Count);
        QuestionData question = availableQuestions[randIndex];
        availableQuestions.RemoveAt(randIndex);

        float spawnY = Random.Range(spawnYMin, spawnYMax);
        Vector3 spawnPos = new Vector3(spawnX, spawnY, 0);

        GameObject bossObj = Instantiate(bossPrefab, spawnPos, Quaternion.identity);
        BossEnemy boss = bossObj.GetComponent<BossEnemy>();

        boss.questionData = question;
        boss.levelIndex = currentLevel;
        boss.moveSpeed = GetSpeedForLevel(currentLevel);
    }

    public void OnBossDefeated()
    {
        GameManager.Instance.OnLevelComplete();
    }

    public QuestionData GetRandomQuestion()
    {
        if (availableQuestions.Count == 0)
            LoadQuestionsForLevel(currentLevel);

        int randIndex = Random.Range(0, availableQuestions.Count);
        QuestionData question = availableQuestions[randIndex];
        availableQuestions.RemoveAt(randIndex);
        return question;
    }

    public void SetWave(int wave)
    {
        currentWave = wave;
        GameUI.Instance?.UpdateWaveText(currentWave, totalWaves);
    }
}