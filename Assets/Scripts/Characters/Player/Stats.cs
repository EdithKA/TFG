using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI; // Cambiado desde UIElements

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
    public float rotationSpeed = 50f; // Velocidad de rotación del objeto
    public int sanityThreshold; // Inicio de la corrupción
    public float rotationRange = 45f;

    private List<GameObject> corruptibleObjects = new List<GameObject>();
    private Dictionary<GameObject, Vector3> originalPositions = new Dictionary<GameObject, Vector3>();
    private int lastSanityTrigger; // Último múltiplo de 5 en el que se ejecutó la corrupción

    [Header("Health")]
    public Image heartIcon; // Referencia al objeto del corazón
    public Sprite[] heartStats; // Diferentes sprites del corazón
    public List<GameObject> healthBars;

    [Header("Sanity")]
    public Image sanityIcon; // Referencia al objeto del cerebro
    public Sprite[] brainStats; // Diferentes sprites del cerebro
    public List<GameObject> sanityBars;
    private void Start()
    {
        // Identificar los objetos que podrán corromperse
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

        // Inicialización de las estadísticas
        sanity = 100;
        health = 100;
        lastSanityTrigger = sanity;

        StartCoroutine(DecreaseSanity());
    }

    private void Update()
    {
        UpdateUI();
        SetHealth();
        SetSanity();
        UpdateBars();
    }

    void UpdateUI()
    {
        sanityCounter.text = sanity.ToString();
        healthCounter.text = health.ToString();
        SetHealth();
        SetSanity();
    }

    void SetHealth()
    {
        

        switch (health)
        {
            case <= 0:
                heartIcon.sprite = heartStats[0]; 
                break;
            case <= 25:
                heartIcon.sprite = heartStats[1];
                break;
            case > 25 and <= 50:
                heartIcon.sprite = heartStats[2];
                break;
            case > 50 and <= 75:
                heartIcon.sprite = heartStats[3];
                break;
            case > 75:
                heartIcon.sprite = heartStats[4];
                break;
        }
    }

    void SetSanity()
    {
       

        switch (sanity)
        {
            case <= 0:
                sanityIcon.sprite = brainStats[0];
                break;
            case <= 25:
                sanityIcon.sprite = brainStats[1];
                break;
            case > 25 and <= 50:
                sanityIcon.sprite = brainStats[2];
                break;
            case > 50 and <= 75:
                sanityIcon.sprite = brainStats[3];
                break;
            case > 75:
                sanityIcon.sprite = brainStats[4];
                break;
        }
    }

    IEnumerator DecreaseSanity()
    {
        while (true)
        {
            yield return new WaitForSeconds(decreaseInterval);

            if (sanity > 0)
            {
                sanity--;
                if (sanity < sanityThreshold)
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

    void UpdateBars()
    {
        UpdateBar(health, healthBars);
        UpdateBar(sanity, sanityBars);
    }

    void UpdateBar(int statValue, List<GameObject> bars)
    {
        int activeBarsCount = Mathf.CeilToInt(statValue / 25f); // Calcula cuántos objetos activar según el valor

        for (int i = 0; i < bars.Count; i++)
        {
            if (i < activeBarsCount)
            {
                bars[i].SetActive(true); // Activa los objetos necesarios
            }
            else
            {
                bars[i].SetActive(false); // Desactiva los demás objetos
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
                Vector3 randomOffset = new Vector3(
                    Random.Range(-moveRange, moveRange),
                    Random.Range(-moveRange, moveRange),
                    Random.Range(-moveRange, moveRange)
                );
                obj.transform.position += randomOffset;

                float randomXRotation = Random.Range(-rotationRange, rotationRange);
                float randomYRotation = Random.Range(-rotationRange, rotationRange);
                float randomZRotation = Random.Range(-rotationRange, rotationRange);
                obj.transform.Rotate(new Vector3(randomXRotation, randomYRotation, randomZRotation));
            }
        }

        yield return new WaitForSeconds(2f);

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
