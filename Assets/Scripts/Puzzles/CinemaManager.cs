using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Este script maneja el estao del puzle del cine.
public class CinemaManager : MonoBehaviour, IPuzleObjective
{
    public Material screenMaterial; // Material que se asignar� a la pantalla cuando el puzle est� completo.
    public List<DoorInteractable> doors; // Lista de puertas qe deben estar abiertas para completar el puzle.

    public bool isComplete { get; private set; } // Indica si el objetivo del puzle est� completo.

    // Revisa constantemente si las puertas necesarias est�n abiertas.
    void Update()
    {
        if(checkDoors())
        {
            isComplete = true;
            this.gameObject.GetComponent<MeshRenderer>().material = screenMaterial;
        }
    }

    // Devuelve true solo si todas las puertas de la lista est�n abiertas.
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
