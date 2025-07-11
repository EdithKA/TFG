using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * @brief Manages cinema puzzle state.
 */
public class CinemaManager : MonoBehaviour, IPuzleObjective
{
    public Material screenMaterial;         ///< Material for screen when complete
    public List<DoorInteractable> doors;    ///< Doors to check
    public bool isComplete { get; private set; } ///< Puzzle done

    /**
     * @brief Check doors each frame.
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
     * @brief True if all doors open.
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
