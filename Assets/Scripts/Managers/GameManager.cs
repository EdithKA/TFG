using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
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

    [Header("Save System Config")]
    public GameObject player;
    public Stats playerStats;
    public InventoryManager inventoryManager;

    public static GameManager Instance;
    public static SaveData pendingLoadData = null;

    private string saveFile => Path.Combine(Application.persistentDataPath, "savegame.json");

    void Awake()
    {
        if (Instance == null)
        {
            DontDestroyOnLoad(gameObject);
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        if (pendingLoadData != null)
        {
            RestoreGame(pendingLoadData);
            pendingLoadData = null;
        }
    }

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

    public void SaveGame()
    {
        SaveData data = new SaveData();
        data.playerPosition = player.transform.position;
        data.health = playerStats.health;
        data.sanity = playerStats.sanity;
        data.stamina = player.GetComponent<PlayerController>().stamina;
        data.inventoryItems = inventoryManager.GetInventoryItemIDs();
        data.currentScene = SceneManager.GetActiveScene().name;

        data.enemies = new List<EnemyData>();
        foreach (EnemyController enemy in FindObjectsOfType<EnemyController>())
        {
            var id = enemy.GetComponent<GObjectId>().id;
            data.enemies.Add(new EnemyData
            {
                id = id,
                position = enemy.transform.position,
                
            });
        }

        data.doors = new List<DoorData>();
        foreach (DoorInteractable door in FindObjectsOfType<DoorInteractable>())
        {
            var id = door.GetComponent<GObjectId>().id;
            data.doors.Add(new DoorData
            {
                id = id,
                isOpen = door.isOpen
            });
        }

        data.puzzles = new List<PuzzleData>();
        foreach (PuzzleManager puzzle in FindObjectsOfType<PuzzleManager>())
        {
            var id = puzzle.GetComponent<GObjectId>().id;
            data.puzzles.Add(new PuzzleData
            {
                id = id,
                isSolved = puzzle.puzzleCompleted
            });
        }

        SaveSystem.Save(data);
        Debug.Log("Game saved: " + saveFile);
    }

    public void LoadGame()
    {
        if (!SaveSystem.SaveExists())
        {
            Debug.LogWarning("No save file found!");
            return;
        }

        SaveData data = SaveSystem.Load();
        pendingLoadData = data;
        SceneManager.LoadScene(data.currentScene);
    }

    void RestoreGame(SaveData data)
    {
        player = GameObject.FindGameObjectWithTag("Player");
        player.transform.position = data.playerPosition;

        playerStats = FindObjectOfType<Stats>();
        playerStats.health = data.health;
        playerStats.sanity = data.sanity;
        player.GetComponent<PlayerController>().stamina = data.stamina;

        inventoryManager = FindObjectOfType<InventoryManager>();
        inventoryManager.RestoreInventory(data.inventoryItems);

        foreach (EnemyData enemyData in data.enemies)
        {
            foreach (EnemyController enemy in FindObjectsOfType<EnemyController>())
            {
                if (enemy.GetComponent<GObjectId>().id == enemyData.id)
                {
                    enemy.transform.position = enemyData.position;
                }
            }
        }

        foreach (DoorData dd in data.doors)
        {
            foreach (DoorInteractable door in FindObjectsOfType<DoorInteractable>())
            {
                if (door.GetComponent<GObjectId>().id == dd.id)
                {
                    door.isOpen = dd.isOpen;
                }
            }
        }

        foreach (PuzzleData puzzleData in data.puzzles)
        {
            foreach (PuzzleManager puzzle in FindObjectsOfType<PuzzleManager>())
            {
                if (puzzle.GetComponent<GObjectId>().id == puzzleData.id)
                {
                    puzzle.puzzleCompleted = puzzleData.isSolved;
                }
            }
        }

        RemoveCollectedPickups(data.inventoryItems);
        Debug.Log("Game loaded.");
    }

    void RemoveCollectedPickups(List<string> inventoryIDs)
    {
        foreach (ItemInteractable itemInteractable in FindObjectsOfType<ItemInteractable>())
        {
            string itemID = itemInteractable.GetComponent<GObjectId>().id;
            if (inventoryIDs.Contains(itemID))
            {
                Destroy(itemInteractable.gameObject);
            }
        }
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
