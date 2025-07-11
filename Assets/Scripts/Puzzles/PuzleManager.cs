using System.Collections.Generic;
using UnityEngine;

/**
 * @brief Manages puzzle state and reward spawning.
 */
public class PuzleManager : MonoBehaviour
{
    [Header("Puzle Settings")]
    public List<MonoBehaviour> puzzleObjectives = new List<MonoBehaviour>(); ///< Objectives to complete
    public GameObject rewardPrefab;           ///< Reward prefab
    public Transform rewardSpawnPoint;        ///< Reward spawn point

    public List<IPuzleObjective> objectives = new List<IPuzleObjective>(); ///< Objectives as interface
    public bool puzzleCompleted = false;      ///< Puzzle done

    /**
     * @brief Get objectives as interface.
     */
    void Start()
    {
        foreach (var mb in puzzleObjectives)
        {
            var obj = mb as IPuzleObjective;
            if (obj != null)
            {
                objectives.Add(obj);
            }
        }
    }

    /**
     * @brief Check completion each frame.
     */
    void Update()
    {
        if (!puzzleCompleted)
        {
            CheckObjectives();
        }
    }

    /**
     * @brief If all objectives done, spawn reward.
     */
    void CheckObjectives()
    {
        foreach (var objective in objectives)
        {
            if (!objective.isComplete)
            {
                return;
            }
        }

        puzzleCompleted = true;
        Instantiate(rewardPrefab, rewardSpawnPoint.position, rewardPrefab.transform.rotation);
    }
}
