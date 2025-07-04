using UnityEngine;
using UnityEngine.SceneManagement;

// Controla el men� de pausa, cambio de escenas y salida del juego.
public class GameManager : MonoBehaviour
{
    // Pause Menu Settings.
    public GameObject pauseMenu; // Men� de pausa.
    public bool isLevel; // Indica si estamos en un nivel o un men�.
    private bool isPaused = false; // Estado de pausa.
    public bool gameComplete = false; // Indica si el juego ha sido completado.


    private void Update()
    {
        // Si es un nivel, tecla ESC para pausar/reanudar.
        if (isLevel && Input.GetKeyDown(KeyCode.Escape))
            if (isPaused)
                Resume();
            else
                Pause();
        // Si el juego est� completo se carga la escena final.
        if (isLevel && gameComplete)
            LoadSceneByName("End");
    }

    // Reanuda el juego desde el men� de pausa.
    public void Resume()
    {
        AudioListener.pause = false;
        pauseMenu.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Activa el men� de pausa y detiene el tiempo.
    void Pause()
    {
        pauseMenu.SetActive(true);
        AudioListener.pause = true;
        Time.timeScale = 0f;
        isPaused = true;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    // Cambia de escena seg�n el nombre recibido.
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

    // Cierra la aplicaci�n.
    public void QuitGame()
    {
        Application.Quit();
    }
}
