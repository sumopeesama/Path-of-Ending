using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public bool isPaused = false;
    public bool isGameOver = false;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Update()
    {
        // Pause/Unpause with Escape key
        if (Input.GetKeyDown(KeyCode.Escape) && !isGameOver)
        {
            if (isPaused) ResumeGame();
            else PauseGame();
        }
    }

    public void WinGame()
    {
        if (isGameOver) return;

        isGameOver = true;
        Debug.Log("You Win!");
        Time.timeScale = 0f;
    }

    public void LoseGame()
    {
        if (isGameOver) return;

        isGameOver = true;
        Debug.Log("You Lose!");
        Time.timeScale = 0f;
    }

    public void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0f;
        Debug.Log("Game Paused");
    }

    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f;
        Debug.Log("Game Resumed");
    }

    public void RestartLevel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}