using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

/**
 * @brief Handles pause menu, scene changes, and quitting the game.
 */
public class GameManager : MonoBehaviour
{
    // --- Pause Menu ---
    public GameObject pauseMenu;      ///< Pause menu panel
    public bool isLevel;              ///< True if in a level
    bool isPaused = false;            ///< Pause state

    // --- Menu ---
    public AudioSource buttonPlayer;  ///< Audio source for buttons
    public AudioClip buttonSound;     ///< Button sound

    /**
     * @brief Pause/resume logic each frame.
     */
    void Update()
    {
        if (isLevel && Input.GetKeyDown(KeyCode.Escape))
            if (isPaused)
                Resume();
            else
                Pause();
    }

    /**
     * @brief Resume from pause.
     */
    void Resume()
    {
        AudioListener.pause = false;
        pauseMenu.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    /**
     * @brief Pause the game.
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
     * @brief Play button sound and change scene.
     */
    public void ButtonToScene(string sceneName)
    {
        StartCoroutine(PlaySoundAndChangeScene(sceneName));
    }

    private IEnumerator PlaySoundAndChangeScene(string sceneName)
    {
        buttonPlayer.PlayOneShot(buttonSound);
        yield return new WaitForSeconds(buttonSound.length);
        SceneManager.LoadScene(sceneName);
    }

    /**
     * @brief Change scene by name.
     */
    public void ChangeScene(string name)
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
     * @brief Quit the game.
     */
    public void QuitGame()
    {
        buttonPlayer.PlayOneShot(buttonSound);
        Application.Quit();
    }
}
