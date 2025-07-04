using System.Collections.Generic;
using UnityEngine;

public class PuzzleManager : MonoBehaviour
{
    [Header("Configuration")]
    [SerializeField] private List<MonoBehaviour> puzzleObjectives = new List<MonoBehaviour>();
    [SerializeField] private GameObject rewardPrefab;
    [SerializeField] private Transform rewardSpawnPoint;

    public List<IPuzleObjective> objectives = new List<IPuzleObjective>();
    public bool puzzleCompleted = false;

    private void Start()
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

    private void Update()
    {
        if (!puzzleCompleted)
        {
            CheckObjectives();
        }
    }

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
