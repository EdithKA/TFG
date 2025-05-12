using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/**
 * @brief Handles core game management actions such as scene transitions and quitting the game.
 *        Simple and reusable for menus, checkpoints, or any global game logic.
 */
public class GameManager : MonoBehaviour
{
    /**
     * @brief Loads a new scene by its name.
     * @param name The name of the scene to load.
     */
    public void LoadSceneByName(string name)
    {
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
