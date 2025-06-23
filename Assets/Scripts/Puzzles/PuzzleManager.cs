using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages a puzzle by tracking a list of objectives (any MonoBehaviour implementing IPuzzleObjective).
/// When all objectives are complete, spawns a reward prefab at the specified location.
/// </summary>
public class PuzzleManager : MonoBehaviour
{
    [Header("Configuration")]
    [SerializeField] private List<MonoBehaviour> puzzleObjectives = new List<MonoBehaviour>();
    [SerializeField] private GameObject rewardPrefab;
    [SerializeField] private Transform rewardSpawnPoint;

    // Internal list of objectives as interface
    private List<IPuzzleObjective> objectives = new List<IPuzzleObjective>();

    private void Start()
    {
        // Convert MonoBehaviours to IPuzzleObjective and subscribe to their events
        foreach (var mb in puzzleObjectives)
        {
            var obj = mb as IPuzzleObjective;
            if (obj != null)
            {
                objectives.Add(obj);
                obj.onCompleted += CheckObjectives;
            }
            else
            {
                Debug.LogWarning($"{mb.name} does not implement IPuzzleObjective!");
            }
        }
    }

    /// <summary>
    /// Checks if all objectives are complete and spawns the reward if so.
    /// </summary>
    void CheckObjectives()
    {
        foreach (var objective in objectives)
        {
            if (!objective.isComplete)
            {
                return;
            }
        }
        // All completed!
        Instantiate(rewardPrefab, rewardSpawnPoint.position, rewardPrefab.transform.rotation);
    }

    private void OnDestroy()
    {
        foreach (var objective in objectives)
        {
            if (objective != null)
            {
                objective.onCompleted -= CheckObjectives;
            }
        }
    }
}
