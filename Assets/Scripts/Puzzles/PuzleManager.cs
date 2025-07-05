using System.Collections.Generic;
using UnityEngine;

/**
 * @brief This script manages the state of a minigame based on its assigned objectives.
 */
public class PuzleManager : MonoBehaviour
{
    [Header("Puzle Settings")]
    List<MonoBehaviour> puzzleObjectives = new List<MonoBehaviour>(); ///< List of objectives to complete the puzzle.
    public GameObject rewardPrefab; ///< Prefab of the reward object for completing the puzzle.
    public Transform rewardSpawnPoint; ///< Point where the reward will be instantiated.

    public List<IPuzleObjective> objectives = new List<IPuzleObjective>(); ///< List of objectives converted to the IPuzleObjective interface.
    public bool puzzleCompleted = false;

    /**
     * @brief At start, converts the objectives to IPuzleObjective and adds them to the objectives list.
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
     * @brief Constantly checks if the puzzle is completed.
     */
    void Update()
    {
        if (!puzzleCompleted)
        {
            CheckObjectives();
        }
    }

    /**
     * @brief Checks if all objectives are complete; if so, instantiates the reward and marks the puzzle as completed.
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
