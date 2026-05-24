using UnityEngine;
using System.Collections.Generic;
using System.IO;

[System.Serializable]
public class EnemySaveData
{
    public float posX;
    public float posY;
    public string questionName;
    public float moveSpeed;
    public int levelIndex;
    public bool firstAttempt;
}

[System.Serializable]
public class GameSaveData
{
    public int scene;
    public int health;
    public int score;
    public int wave;
    public float bg1PosX;
    public float bg2PosX;
    public bool isLevelWon;
    public List<EnemySaveData> enemies = new List<EnemySaveData>();
    // FIX: tambah data boss
    public bool hasBoss;
    public float bossPosX;
    public float bossPosY;
    public string bossQuestionName;
    public float bossMoveSpeed;
    public int bossLevelIndex;
    public int bossHP;
}

public static class SaveSystem
{
    static string savePath => Application.persistentDataPath + "/savegame.json";

    public static void Save(GameSaveData data)
    {
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(savePath, json);
        Debug.Log("Game saved: " + savePath);
    }

    public static GameSaveData Load()
    {
        if (!File.Exists(savePath)) return null;
        string json = File.ReadAllText(savePath);
        return JsonUtility.FromJson<GameSaveData>(json);
    }

    public static bool HasSave() => File.Exists(savePath);
}