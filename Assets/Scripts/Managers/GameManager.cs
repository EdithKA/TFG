using UnityEngine;
using UnityEngine.SceneManagement;

/**
 * @brief Handles core game management actions such as scene transitions and quitting the game.
 *        Simple and reusable for menus, checkpoints, or any global game logic.
 */
public class GameManager : MonoBehaviour
{
    [Header("Pause Menu Config")]
    public GameObject pauseMenu;
    public bool isLevel;
    private bool isPaused = false;
    public bool gameComplete = false;

    private void Update()
    {
        if (isLevel && Input.GetKeyDown(KeyCode.Escape))
            if (isPaused)
                Resume();
            else
                Pause();
        if (isLevel && gameComplete)
            LoadSceneByName("MainMenu");
    }

    public void Resume()
    {
        AudioListener.pause = false;
        pauseMenu.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Pause()
    {
        pauseMenu.SetActive(true);
        AudioListener.pause = true;
        Time.timeScale = 0f;
        isPaused = true;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    /**
     * @brief Loads a new scene by its name.
     * @param name The name of the scene to load.
     */
    public void LoadSceneByName(string name)
    {
        if (name != "MainLevel")
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        Time.timeScale = 1f;
        SceneManager.LoadScene(name);
    }

    /**
     * @brief Quits the game application.
     *        Note: This will only work in a built game, not in the Unity Editor.
     */
    public void QuitGame()
    {
        Application.Quit();
    }
}
