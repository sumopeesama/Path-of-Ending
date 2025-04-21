using UnityEngine;
using UnityEngine.InputSystem; // If you're using the new Input System

public class PauseManager : MonoBehaviour
{
    public static bool isPaused = false;
    public GameObject pauseMenuUI; // Optional: assign a UI Panel in the Inspector

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused) ResumeGame();
            else PauseGame();
        }
    }

    public void PauseGame()
    {
        Time.timeScale = 0f;
        isPaused = true;
        if (pauseMenuUI != null)
            pauseMenuUI.SetActive(true);
        Debug.Log("Game Paused");
    }

    public void ResumeGame()
    {
        Time.timeScale = 1f;
        isPaused = false;
        if (pauseMenuUI != null)
            pauseMenuUI.SetActive(false);
        Debug.Log("Game Resumed");
    }
    void FixedUpdate()
    {
        if (PauseManager.isPaused) return;
        Animator animator = GetComponent<Animator>();
        animator.speed = PauseManager.isPaused ? 0 : 1;

        // Rigidbody physics logic
    }


}
