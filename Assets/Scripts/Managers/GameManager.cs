using UnityEngine;
using UnityEngine.SceneManagement;

/**
 * @brief Controls the pause menu, scene changes, and quitting the game.
 */
public class GameManager : MonoBehaviour
{
    // Pause Menu Settings.
    public GameObject pauseMenu; ///< Pause menu.
    public bool isLevel; ///< Indicates if we are in a level or a menu.
    bool isPaused = false; ///< Pause state.
    public bool gameComplete = false; ///< Indicates if the game has been completed.

    /**
     * @brief Handles pause/resume and checks for game completion every frame.
     */
    void Update()
    {
        // If it's a level, ESC key to pause/resume.
        if (isLevel && Input.GetKeyDown(KeyCode.Escape))
            if (isPaused)
                Resume();
            else
                Pause();
        // If the game is complete, load the final scene.
        if (isLevel && gameComplete)
            LoadSceneByName("End");
    }

    /**
     * @brief Resumes the game from the pause menu.
     */
    public void Resume()
    {
        AudioListener.pause = false;
        pauseMenu.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    /**
     * @brief Activates the pause menu and stops time.
     */
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
     * @brief Changes the scene according to the received name.
     * @param name Name of the scene to load.
     */
    public void LoadSceneByName(string name)
    {
        if (AudioListener.pause == true)
            AudioListener.pause = false;
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
     * @brief Closes the application.
     */
    public void QuitGame()
    {
        Application.Quit();
    }
}
