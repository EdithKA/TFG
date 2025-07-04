using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Este script permite que el sprite del enemigo esté siempre orientado hacia el jugador
public class EnemySpriteLook : MonoBehaviour
{
    public Transform target; // La posición del jugador, el enemigo siempre mira hacia el
    public bool canLookVertically; // El enemigo puede mirar hacia arriba/abajo al jugador

    void Start()
    {
        target = FindAnyObjectByType<PlayerController>().transform; // Se asigna en la referencia al jugador en la escena
    }

    void Update()
    {
        if (canLookVertically)
        {
            transform.LookAt(target);
        }
        else
        {
            Vector3 modifiedTarget = target.position;
            modifiedTarget.y = transform.position.y;
            transform.LookAt(modifiedTarget);
        }
    }
}
