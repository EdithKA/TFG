using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Stats : MonoBehaviour
{
    [Header("Player Stats")]
    int sanity;
    int health;
    float decreaseInterval = 1.5f;

    // Marcadores - UI
    public TextMeshProUGUI sanityCounter;
    public TextMeshProUGUI healthCounter;

    [Header("World Corruption")]
    public string corruptibleTag; // Tag de los objetos corrompibles
    public float moveRange = 0.5f; // Rango de movimiento aleatorio
    public float rotationSpeed = 50f; // Velocidad de rotaci�n del objeto
    public int sanityThreshold; //inicio de la corrupcion
    public float rotationRange = 45f;

    private List<GameObject> corruptibleObjects = new List<GameObject>();
    private Dictionary<GameObject, Vector3> originalPositions = new Dictionary<GameObject, Vector3>();
    private int lastSanityTrigger; // �ltimo m�ltiplo de 5 en el que se ejecut� la corrupci�n

    private void Start()
    {
        // Identificar los objetos que podr�n corromperse
        GameObject[] corruptObjects = GameObject.FindGameObjectsWithTag(corruptibleTag);
        corruptibleObjects.AddRange(corruptObjects);

        // Guardar las posiciones originales de los objetos
        foreach (GameObject obj in corruptibleObjects)
        {
            if (obj != null)
            {
                originalPositions[obj] = obj.transform.position;
            }
        }

        // Inicializaci�n de las estad�sticas
        sanity = 100;
        health = 100;
        lastSanityTrigger = sanity;

        StartCoroutine(DecreaseSanity());
    }

    private void Update()
    {
        UpdateUI();
    }

    void UpdateUI()
    {
        sanityCounter.text = sanity.ToString();
        healthCounter.text = health.ToString();
    }

    IEnumerator DecreaseSanity()
    {
        while (true)
        {
            yield return new WaitForSeconds(decreaseInterval);

            if (sanity > 0)
            {
                sanity--;
                if(sanity < sanityThreshold)
                {
                    StartCoroutine(CorruptObjects());
                }
            }
            else if (health > 0)
            {
                health--;
            }
        }
    }

    

    IEnumerator CorruptObjects()
    {
        Debug.Log("Corrompiendo objetos debido a la cordura: " + sanity);

        foreach (GameObject obj in corruptibleObjects)
        {
            if (obj != null)
            {
                // Movimiento aleatorio dentro del rango especificado
                Vector3 randomOffset = new Vector3(
                    Random.Range(-moveRange, moveRange),
                    Random.Range(-moveRange, moveRange),
                    Random.Range(-moveRange, moveRange)
                );
                obj.transform.position += randomOffset;

                // Rotaci�n aleatoria
                float randomXRotation = Random.Range(-rotationRange, rotationRange);
                float randomYRotation = Random.Range(-rotationRange, rotationRange);
                float randomZRotation = Random.Range(-rotationRange, rotationRange);
                obj.transform.Rotate(new Vector3(randomXRotation, randomYRotation, randomZRotation));
            }
        }

        yield return new WaitForSeconds(2f); // Duraci�n del efecto de corrupci�n

        ResetObjects();
    }

    private void ResetObjects()
    {
        Debug.Log("Restaurando objetos a sus posiciones originales.");

        foreach (GameObject obj in corruptibleObjects)
        {
            if (obj != null && originalPositions.ContainsKey(obj))
            {
                obj.transform.position = originalPositions[obj];
                obj.transform.rotation = Quaternion.identity;
            }
        }
    }
}
