using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * @brief This script manages the state of the cinema puzzle.
 */
public class CinemaManager : MonoBehaviour, IPuzleObjective
{
    /**
     * @brief Material to assign to the screen when the puzzle is complete.
     */
    public Material screenMaterial;

    /**
     * @brief List of doors that must be open to complete the puzzle.
     */
    public List<DoorInteractable> doors;

    /**
     * @brief Indicates if the puzzle objective is complete.
     */
    public bool isComplete { get; private set; }

    /**
     * @brief Constantly checks if the required doors are open.
     */
    void Update()
    {
        if (checkDoors())
        {
            isComplete = true;
            this.gameObject.GetComponent<MeshRenderer>().material = screenMaterial;
        }
    }

    /**
     * @brief Returns true only if all doors in the list are open.
     * @return True if all doors are open, false otherwise.
     */
    bool checkDoors()
    {
        foreach (DoorInteractable door in doors)
        {
            if (!door.isOpen)
                return false;
        }
        return true;
    }
}
