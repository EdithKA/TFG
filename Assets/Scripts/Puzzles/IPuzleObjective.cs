using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * @brief Interface for a minigame objective.
 */
public interface IPuzleObjective
{
    /**
     * @brief True if the objective is complete.
     */
    bool isComplete { get; }
}
