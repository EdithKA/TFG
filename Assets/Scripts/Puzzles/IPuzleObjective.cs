using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * @brief Interface to implement an objective to be completed for a minigame.
 */
public interface IPuzleObjective
{
    /**
     * @brief Indicates if the objective is complete.
     */
    bool isComplete { get; }
}
