using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneHandler : MonoBehaviour
{
    public void StartGame()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.LoadLevel(1);
        else
            SceneManager.LoadScene("LEVEL_1");
    }
}