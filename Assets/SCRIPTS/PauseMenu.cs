using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenuPanel;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Debug.Log("ESC dipencet, isPaused: " + GameManager.Instance.IsGamePaused());
            if (GameManager.Instance.IsLevelComplete()) return;
            
            GameManager.Instance.TogglePause();

            bool isPaused = GameManager.Instance.IsGamePaused();
            pauseMenuPanel.SetActive(isPaused);
        }
    }

    public void OnResumeButton()
    {
        GameManager.Instance.TogglePause();
        pauseMenuPanel.SetActive(false);
    }

    public void OnMainMenuButton()
    {
        GameManager.Instance.LoadMainMenu();
    }

    public void OnQuitButton()
    {
        Application.Quit();
    }

    public void OnSaveButton()
    {
        GameManager.Instance.SaveGame();
        Debug.Log("Game Saved!");
    }

}