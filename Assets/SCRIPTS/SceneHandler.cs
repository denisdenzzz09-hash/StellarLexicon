using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneHandler : MonoBehaviour
{
    public void StartGame()
    {
        SceneManager.LoadScene("LEVEL_1");
    }
}