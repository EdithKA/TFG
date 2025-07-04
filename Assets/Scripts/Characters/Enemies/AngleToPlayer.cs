using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class AngleToPlayer : MonoBehaviour
{

    private Transform player; // Referencia al jugador
    private Vector3 targetPos; // Posicion del jugador proyectada en el plano horizontal
    private Vector3 targetDir; // Direcci�n desde el enemigo hacia el jugador
    private float angle; // �ngulo calculado entre el forward del enemigo y el jugador
    public int lastIndex; // Indice de direcci�n para las animaciones
    private SpriteRenderer spriteRenderer;

    // Al inicio se asignan las referencias autom�ticamente
    void Start()
    {
        player = FindObjectOfType<PlayerController>().transform;
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    //En cada frame se calcula el �ngulo y la direcci�n para el enemigo
    void Update()
    {
        // Se obtiene la posici�n del jugador y se calcula la direcci�n hacia este
        targetPos = new Vector3(player.position.x, transform.position.y, player.position.z);
        targetDir = targetPos - transform.position;

        // Se calcula el �ngulo entre el forward del enemigo y la direcci�n
        angle = Vector3.SignedAngle(targetDir, transform.forward, Vector3.up);
        
        // Se calcula el �ndice para la animaci�n del enemigo
        lastIndex = GetIndex(angle); 
    }

    int GetIndex(float angle)
    {
        // Front-facing directions
        if (angle > -22.5f && angle < 22.6f) return 0; // Front-center
        if (angle >= 22.5f && angle < 67.5f) return 7; // Front-right
        if (angle >= 67.5f && angle < 112.5f) return 6; // Right profile
        if (angle >= 112.5f && angle < 157.5f) return 5; // Back-right

        // Back-facing directions
        if (angle <= -157.5f || angle >= 157.5f) return 4; // Directly behind
        if (angle >= -157.4f && angle < -112.5f) return 3; // Back-left
        if (angle >= -112.5f && angle < -67.5f) return 2; // Left profile
        if (angle >= -67.5f && angle <= -22.5f) return 1; // Front-left

        return lastIndex; // Fallback to previous value
    }

}
