using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CinemaManager : MonoBehaviour, IPuzzleObjective
{
    public Material screenMaterial;
    public List<DoorInteractable> doors;

    public bool isComplete { get; private set; }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(checkDoors())
        {
            isComplete = true;
            this.gameObject.GetComponent<MeshRenderer>().material = screenMaterial;
        }
    }

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
