using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class LoadSystem : MonoBehaviour
{
    public static LoadSystem Instance;
    private GameSaveData pendingData;

    void Awake()
    {
        Debug.Log("LoadSystem Awake, Instance null: " + (Instance == null));
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
    }

    public void LoadSavedGame()
    {
        Debug.Log("LoadSavedGame dipanggil");
        pendingData = SaveSystem.Load();
        if (pendingData == null)
        {
            Debug.Log("Tidak ada save file!");
            return;
        }

        Debug.Log("Save file ditemukan, level: " + pendingData.scene);
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.LoadScene("LEVEL_1");
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("Scene loaded: " + scene.name);
        SceneManager.sceneLoaded -= OnSceneLoaded;
        StartCoroutine(RestoreAfterLoad(pendingData));
    }

    IEnumerator RestoreAfterLoad(GameSaveData data)
    {
        yield return new WaitForSeconds(0.5f);

        GameManager.Instance.SetHP(data.health);
        GameManager.Instance.SetScore(data.score);

        if (data.isLevelWon) // ← tambah ini
        {
            GameManager.Instance.TriggerLevelComplete();
            yield break;
        }
        
        WaveSpawner.Instance.SetWave(data.wave);
        GameObject bg1 = GameObject.Find("Background1");
        GameObject bg2 = GameObject.Find("Background2");
        if (bg1 != null) bg1.transform.position = new Vector3(data.bg1PosX, bg1.transform.position.y, bg1.transform.position.z);
        if (bg2 != null) bg2.transform.position = new Vector3(data.bg2PosX, bg2.transform.position.y, bg2.transform.position.z);       

        foreach (EnemySaveData enemyData in data.enemies)
        {
            QuestionData question = Resources.Load<QuestionData>(enemyData.questionName);
            Vector3 pos = new Vector3(enemyData.posX, enemyData.posY, 0);
            GameObject enemyObj = Instantiate(WaveSpawner.Instance.enemyPrefab, pos, Quaternion.identity);
            EnemyAI enemy = enemyObj.GetComponent<EnemyAI>();
            enemy.questionData = question;
            enemy.moveSpeed = enemyData.moveSpeed;
            enemy.levelIndex = enemyData.levelIndex;
            enemy.SetFirstAttempt(enemyData.firstAttempt);
        }

        pendingData = null;
    }
}